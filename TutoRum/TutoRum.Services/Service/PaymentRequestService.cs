using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Enum;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.Models;
using TutoRum.Services.IService;
using TutoRum.Services.ViewModels;
using ZXing.Aztec.Internal;

namespace TutoRum.Services.Service
{
    public class PaymentRequestService : IPaymentRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly INotificationService _notificationService;

        public PaymentRequestService(IUnitOfWork unitOfWork, IEmailService emailService, UserManager<AspNetUser> userManager, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _userManager = userManager;
            _notificationService = notificationService;
        }

        // Create Payment Request
        public async Task<PaymentRequest> CreatePaymentRequestAsync(CreatePaymentRequestDTO requestDto, ClaimsPrincipal user)
        {
            if (requestDto == null || requestDto.Amount <= 0)
            {
                throw new ArgumentException("Invalid payment request data.");
            }

            var currentUser = await _userManager.GetUserAsync(user);

            if (currentUser == null)
            {
                throw new Exception("User not found.");
            }

            // Check if the tutor exists
            var tutor =  _unitOfWork.Tutors.GetSingleByGuId(currentUser.Id);
            if (tutor == null)
            {
                throw new Exception("Tutor not found.");
            }

            if (tutor.Balance < requestDto.Amount)
            {
                throw new Exception("Insufficient balance.");
            }

            // Create the payment request
            var paymentRequest = new PaymentRequest
            {
                TutorId = currentUser.Id,
                BankCode = requestDto.BankCode,
                AccountNumber = requestDto.AccountNumber,
                FullName = requestDto.FullName,
                Amount = requestDto.Amount,
                Status = "Pending",
                CreatedDate = DateTime.UtcNow,
                CreatedBy = currentUser.Id
            };

            // Save the payment request
            _unitOfWork.PaymentRequest.Add(paymentRequest);

            // Update the tutor's balance
            tutor.Balance -= requestDto.Amount;
            _unitOfWork.Tutors.Update(tutor);

            // Commit changes
            await _unitOfWork.CommitAsync();

            var notificationDto = new NotificationRequestDto
            {
                UserId = currentUser.Id,
                Title = "Gửi yêu cầu thành công",
                Description = "Bạn vui lòng kiểm tra email và xác nhận rút tiền để chúng tôi thực hiện chuyển tiền",
                NotificationType = NotificationType.GeneralUser,
                Href = "/user/settings/wallet/"
            };

            await _notificationService.SendNotificationAsync(notificationDto, false);

            return paymentRequest;
        }

        public async Task SendPaymentRequestConfirmationEmailAsync(ClaimsPrincipal user, PaymentRequest paymentRequest, string confirmationUrl)
        {
            var currentUser = await _userManager.GetUserAsync(user);

            if (currentUser == null)
            {
                throw new Exception("User not found.");
            }

            // Check if the tutor exists
            var tutor = _unitOfWork.Tutors.GetSingleByGuId(currentUser.Id);
            if (tutor == null)
            {
                throw new Exception("Tutor not found.");
            }

            string emailContent = $@"
            <h1>Yêu cầu Thanh toán Được Tạo</h1>
            <p>Kính gửi {tutor.TutorNavigation.Fullname},</p>
            <p>Yêu cầu thanh toán của bạn với số tiền {paymentRequest.Amount.ToString("N0")} VND đã được tạo thành công.</p>
            <p><strong>Mã Ngân hàng:</strong> {paymentRequest.BankCode}</p>
            <p><strong>Số Tài khoản:</strong> {paymentRequest.AccountNumber}</p>
            <p>Vui lòng nhấn vào nút bên dưới để xác nhận thanh toán, nếu không xác nhận hãy bỏ qua mail này:</p>
            <a href=""{confirmationUrl}""
               style=""display: inline-block; padding: 10px 20px; font-size: 16px; color: white; background-color: #28a745; text-decoration: none; border-radius: 5px;"">
               Xác nhận Thanh toán
            </a>
            ";

            string subject = "Xác nhận Yêu cầu Thanh toán";

            await _emailService.SendEmailAsync(tutor.TutorNavigation.Email, subject, emailContent);
        }

        public async Task<bool> ConfirmPaymentRequest(int requestId, Guid token)
        {
            var paymentRequest = await _unitOfWork.PaymentRequest.FindAsync(pr => pr.PaymentRequestId == requestId);
            if (paymentRequest == null || paymentRequest.Token != token)
            {
                throw new Exception("Invalid request or token.");
            }

            // Cập nhật trạng thái thanh toán
            paymentRequest.VerificationStatus = "Confirmed";
            _unitOfWork.PaymentRequest.Update(paymentRequest);
            await _unitOfWork.CommitAsync();

            return true;
        }

        // List Payment Requests
        public async Task<PagedResult<PaymentRequestDTO>> GetListPaymentRequestsAsync(
    PaymentRequestFilterDTO filter,
    int pageIndex = 0,
    int pageSize = 20)
        {
            Expression<Func<PaymentRequest, bool>> predicate = request =>
                // Điều kiện lọc theo tên gia sư (TutorName)
                (string.IsNullOrEmpty(filter.TutorName) || request.Tutor.TutorNavigation.Fullname == filter.TutorName) &&

                // Điều kiện lọc theo ID yêu cầu thanh toán (PaymentRequestId)
                (!filter.PaymentRequestId.HasValue || request.PaymentRequestId == filter.PaymentRequestId) &&

                // Điều kiện lọc theo trạng thái thanh toán (IsPaid)
                (!filter.IsPaid.HasValue || request.IsPaid == filter.IsPaid) &&

                // Điều kiện lọc theo trạng thái xác thực (VerificationStatus)
                (string.IsNullOrEmpty(filter.VerificationStatus) || request.VerificationStatus == filter.VerificationStatus) &&

                // Điều kiện lọc theo ngày tạo (FromDate)
                (!filter.FromDate.HasValue || request.CreatedDate >= filter.FromDate.Value) &&

                // Điều kiện lọc theo ngày tạo (ToDate)
                (!filter.ToDate.HasValue || request.CreatedDate <= filter.ToDate.Value);

            // Retrieve filtered and paginated payment requests
            var totalRecords = 0;
            var paymentRequests = _unitOfWork.PaymentRequest.GetMultiPaging(
                predicate,
                out totalRecords,
                index: pageIndex,
                size: pageSize,
                includes: new[] { "Tutor", "Tutor.TutorNavigation" } // Include associated Tutor data
            );

            // Map to PaymentRequestDTO
            var paymentRequestList = paymentRequests.Select(request => new PaymentRequestDTO
            {
                PaymentRequestId = request.PaymentRequestId,
                TutorId = request.TutorId,
                BankCode = request.BankCode,
                AccountNumber = request.AccountNumber,
                Amount = request.Amount,
                Status = request.Status,
                VerificationStatus = request.VerificationStatus,
                CreatedDate = request.CreatedDate,
                PaidDate = request.PaidDate,
                AdminNote = request.AdminNote,
                TutorName = request.Tutor?.TutorNavigation?.Fullname,
                IsPaid = request.IsPaid
            }).ToList();

            // Wrap the result in a PagedResult object
            return new PagedResult<PaymentRequestDTO>
            {
                Items = paymentRequestList,
                TotalRecords = totalRecords
            };
        }


        // List Payment Requests
        public async Task<PagedResult<PaymentRequestDTO>> GetListPaymentRequestsByTutorAsync(
            ClaimsPrincipal user,
            int pageIndex = 0,
            int pageSize = 20)
        {

            var currentUser = await _userManager.GetUserAsync(user);

            if (currentUser == null)
            {
                throw new Exception("User not found.");
            }

            var isTutor = await _userManager.IsInRoleAsync(currentUser, AccountRoles.Tutor);

            if (!isTutor)
            {
                throw new Exception("User is not tutor");
            }

            // Build the predicate (filtering conditions)
            Expression<Func<PaymentRequest, bool>> predicate = pr => !pr.IsDelete && pr.TutorId == currentUser.Id;

            // Retrieve filtered and paginated payment requests
            var totalRecords = 0;
            var paymentRequests = _unitOfWork.PaymentRequest.GetMultiPaging(
                predicate,
                out totalRecords,
                index: pageIndex,
                size: pageSize,
                includes: new[] { "Tutor", "Tutor.TutorNavigation" } // Include associated Tutor data
            );

            // Map to PaymentRequestDTO
            var paymentRequestList = paymentRequests.Select(request => new PaymentRequestDTO
            {
                PaymentRequestId = request.PaymentRequestId,
                TutorId = request.TutorId,
                BankCode = request.BankCode,
                AccountNumber = request.AccountNumber,
                Amount = request.Amount,
                Status = request.Status,
                VerificationStatus = request.VerificationStatus,
                CreatedDate = request.CreatedDate,
                PaidDate = request.PaidDate,
                AdminNote = request.AdminNote,
                TutorName = request.Tutor?.TutorNavigation?.Fullname,
                IsPaid = request.IsPaid
            }).ToList();

            // Wrap the result in a PagedResult object
            return new PagedResult<PaymentRequestDTO>
            {
                Items = paymentRequestList,
                TotalRecords = totalRecords
            };
        }

        // Approve Payment Request
        public async Task<bool> ApprovePaymentRequestAsync(int paymentRequestId,  ClaimsPrincipal adminUser)
        {
            // Retrieve the payment request
            var paymentRequest = _unitOfWork.PaymentRequest.GetSingleById(paymentRequestId);
            if (paymentRequest == null)
            {
                throw new Exception("Payment request not found.");
            }

            // Check if the request is already processed
            if (paymentRequest.IsPaid || paymentRequest.Status != "Pending")
            {
                throw new Exception("Payment request has already been processed.");
            }

            var currentUser = await _userManager.GetUserAsync(adminUser);

            if (currentUser == null)
            {
                throw new Exception("User not found.");
            }

            // Update payment request status
            paymentRequest.IsPaid = true;
            paymentRequest.Status = "Approved";
            paymentRequest.PaidDate = DateTime.UtcNow;
            paymentRequest.AdminNote = $"Payment approved by {currentUser.Fullname}.";
            paymentRequest.UpdatedBy = currentUser.Id;
            paymentRequest.UpdatedDate = DateTime.UtcNow;

            // Save changes
            _unitOfWork.PaymentRequest.Update(paymentRequest);
            await _unitOfWork.CommitAsync();

            // Notify the tutor via email
            var tutor = _unitOfWork.Tutors.GetSingleByGuId(paymentRequest.TutorId);
            if (tutor != null)
            {
                await SendPaymentApprovalEmailAsync(tutor, paymentRequest);
            }

            return true;
        }

        // Reject Payment Request
        public async Task<bool> RejectPaymentRequestAsync(int paymentRequestId, string rejectionReason, ClaimsPrincipal adminUser)
        {
            // Retrieve the payment request
            var paymentRequest = _unitOfWork.PaymentRequest.GetSingleById(paymentRequestId);
            if (paymentRequest == null)
            {
                throw new Exception("Payment request not found.");
            }

            // Check if the request is already processed
            if (paymentRequest.Status != "Pending")
            {
                throw new Exception("Payment request has already been processed.");
            }
            var currentUser = await _userManager.GetUserAsync(adminUser);

            if (currentUser == null)
            {
                throw new Exception("User not found.");
            }
            // Update payment request status
            paymentRequest.IsPaid = false;
            paymentRequest.Status = "Rejected";
            paymentRequest.AdminNote = $"Payment rejected by {currentUser.Fullname}: {rejectionReason}";
            paymentRequest.UpdatedBy = currentUser.Id;
            paymentRequest.UpdatedDate = DateTime.UtcNow;
            paymentRequest.AdminNote = rejectionReason;
            // Refund the amount back to the tutor's balance
            var tutor = _unitOfWork.Tutors.GetSingleByGuId(paymentRequest.TutorId);
            if (tutor != null)
            {
                tutor.Balance += paymentRequest.Amount;
                _unitOfWork.Tutors.Update(tutor);
            }

            // Save changes
            _unitOfWork.PaymentRequest.Update(paymentRequest);
            await _unitOfWork.CommitAsync();

            // Notify the tutor via email
            if (tutor != null)
            {
                await SendPaymentRejectionEmailAsync(tutor, paymentRequest, rejectionReason);
            }

            return true;
        }

        private async Task SendPaymentApprovalEmailAsync(Tutor tutor, PaymentRequest paymentRequest)
        {
            var user = await _userManager.FindByIdAsync(tutor.TutorId.ToString());


            string emailContent = $@"
            <h1>Payment Request Approved</h1>
            <p>Dear {user.Fullname},</p>
            <p>Your payment request for the amount of {paymentRequest.Amount:C} has been approved and processed successfully.</p>
            <p>The amount has been transferred to your account:</p>
            <p><strong>Bank Code:</strong> {paymentRequest.BankCode}</p>
            <p><strong>Account Number:</strong> {paymentRequest.AccountNumber}</p>
            <p>Thank you for your service!</p>";

            string subject = "Payment Request Approved";

            await _emailService.SendEmailAsync(user.Email, subject, emailContent);
        }

        private async Task SendPaymentRejectionEmailAsync(Tutor tutor, PaymentRequest paymentRequest, string rejectionReason)
        {
            var user = await _userManager.FindByIdAsync(tutor.TutorId.ToString());

            string emailContent = $@"
            <h1>Payment Request Rejected</h1>
            <p>Dear {user.Fullname},</p>
            <p>Your payment request for the amount of {paymentRequest.Amount:C} has been rejected.</p>
            <p><strong>Reason:</strong> {rejectionReason}</p>
            <p>If you have any questions, please contact support.</p>";

            string subject = "Payment Request Rejected";

            await _emailService.SendEmailAsync(user.Email, subject, emailContent);
        }

        public async Task<bool> UpdatePaymentRequestByIdAsync(int paymentRequestId, UpdatePaymentRequestDTO updateDto, ClaimsPrincipal adminUser)
        {
            if (updateDto == null)
            {
                throw new ArgumentException("Update data cannot be null.");
            }

            // Retrieve the payment request
            var paymentRequest = _unitOfWork.PaymentRequest.GetSingleById(paymentRequestId);
            if (paymentRequest == null)
            {
                throw new Exception("Payment request not found.");
            }

            var currentUser = await _userManager.GetUserAsync(adminUser);

            if (currentUser == null)
            {
                throw new Exception("User not found.");
            }

            // Update fields if they are provided in the DTO
            if (!string.IsNullOrWhiteSpace(updateDto.BankCode))
            {
                paymentRequest.BankCode = updateDto.BankCode;
            }

            if (!string.IsNullOrWhiteSpace(updateDto.AccountNumber))
            {
                paymentRequest.AccountNumber = updateDto.AccountNumber;
            }

            if (!string.IsNullOrWhiteSpace(updateDto.FullName))
            {
                paymentRequest.FullName = updateDto.FullName;
            }

            if (updateDto.Amount > 0)
            {
                paymentRequest.Amount = updateDto.Amount;
            }

            // Update metadata
            paymentRequest.UpdatedBy = currentUser.Id;
            paymentRequest.UpdatedDate = DateTime.UtcNow;

            // Save changes
            _unitOfWork.PaymentRequest.Update(paymentRequest);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<Guid> GeneratePaymentRequestTokenAsync(int paymentRequestId)
        {
            var token = Guid.NewGuid(); // Tạo token ngẫu nhiên
            var paymentRequest = await _unitOfWork.PaymentRequest.FindAsync(pr => pr.PaymentRequestId == paymentRequestId);
            if (paymentRequest == null)
                throw new Exception("Payment request not found.");

            paymentRequest.Token = token; // Gắn token vào yêu cầu
            _unitOfWork.PaymentRequest.Update(paymentRequest);
            await _unitOfWork.CommitAsync(); // Lưu thay đổi

            return token;
        }

        public async Task<bool> DeletePaymentRequest(int paymentRequestId, ClaimsPrincipal user)
        {
            var currentUser = await _userManager.GetUserAsync(user);

            if (currentUser == null)
            {
                throw new Exception("User not found.");
            }

            var paymentRequest = await _unitOfWork.PaymentRequest.FindAsync(pr => pr.PaymentRequestId == paymentRequestId);

            if (paymentRequest == null)
            {
                throw new Exception("PaymentRequest not found.");
            }

            if (paymentRequest.TutorId != currentUser.Id)
            {
                throw new Exception("User do not have role for this payment request");
            }

            if (paymentRequest.Status != "Pending")
            {
                throw new InvalidOperationException("Only pending payment requests can be deleted.");
            }

            var tutor = _unitOfWork.Tutors.GetSingleByGuId(paymentRequest.TutorId);
            if (tutor != null)
            {
                tutor.Balance += paymentRequest.Amount;
                _unitOfWork.Tutors.Update(tutor);
            }
            paymentRequest.IsDelete = true;
            paymentRequest.UpdatedDate = DateTime.UtcNow;
            paymentRequest.UpdatedBy = currentUser.Id;
            _unitOfWork.PaymentRequest.Update(paymentRequest);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
