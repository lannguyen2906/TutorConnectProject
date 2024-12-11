using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.Models;
using TutoRum.Services.IService;
using TutoRum.Services.ViewModels;

namespace TutoRum.Services.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBillService _billService;
        private readonly IEmailService _emailService;
        private readonly UserManager<AspNetUser> userManager;

        public PaymentService(IUnitOfWork unitOfWork, IBillService billService, IEmailService emailService, UserManager<AspNetUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _billService = billService;
            _emailService = emailService;
            this.userManager = userManager;
        }

        public async Task<bool> ProcessPaymentAsync(PaymentResponseModel response, ClaimsPrincipal user)
        {
            if (response == null || response.VnPayResponseCode != "00")
            {
                throw new Exception("Payment failed or invalid response.");
            }

            // Retrieve the bill using the OrderId
            var bill = _unitOfWork.Bill.GetSingleById(int.Parse(response.OrderId));

            if (bill == null)
            {
                throw new Exception("Bill not found.");
            }

            // Check if the bill is already paid
            if (bill.IsPaid)
            {
                throw new Exception("Bill is already paid.");
            }

            var currentUser = await userManager.GetUserAsync(user);

            // Create a new payment record
            var payment = new Payment
            {
                BillId = bill.BillId,
                AmountPaid = response.Success ? bill.TotalBill : 0,
                PaymentMethod = response.PaymentMethod,
                TransactionId = response.TransactionId,
                ResponseCode = response.VnPayResponseCode,
                PaymentDate = DateTime.UtcNow,
                PaymentStatus = response.Success ? "Success" : "Failed",
                OrderId = response.OrderId
            };

            // Update the bill status
            bill.IsPaid = response.Success;
            bill.Status = response.Success ? "Paid" : "Failed";
            bill.PaymentDate = response.Success ? DateTime.UtcNow : null;

            // Transfer payment to tutor
            if (response.Success)
            {

                var bill1 = await _unitOfWork.Bill.GetMultiAsQueryable(b => b.BillId == bill.BillId)
                .Include(b => b.BillingEntries)
                .ThenInclude(be => be.TutorLearnerSubject)
                .ThenInclude(tls => tls.TutorSubject)
                .ThenInclude(ts => ts.Tutor)
                .FirstOrDefaultAsync();


                // Ensure there are billing entries
                var tutor = bill1.BillingEntries
                    .Select(be => be.TutorLearnerSubject?.TutorSubject?.Tutor)
                    .FirstOrDefault();

                //if (tutor == null)
                //{
                //    throw new Exception($"No tutor found for Bill ID {billId}.");
                //}



                //// Get the associated tutor
                //var tutor = bill.BillingEntries
                //    .Select(be => be.TutorLearnerSubject?.TutorSubject?.Tutor)
                //    .FirstOrDefault();

                if (tutor == null)
                {
                    throw new Exception($"No tutor found for Bill ID {bill.BillId}.");
                }

                // Update the tutor's balance
                tutor.Balance += bill.TotalBill ?? 0;

                // Save the tutor's updated balance
                _unitOfWork.Tutors.Update(tutor);
            }

            // Save the payment and update the bill
            _unitOfWork.Payment.Add(payment);
            _unitOfWork.Bill.Update(bill);

            // Commit the changes to the database
            await _unitOfWork.CommitAsync();

            // Send a payment success email (if needed)
            if (response.Success)
            {
                await SendPaymentSuccessEmailAsync(bill, currentUser.Email);
            }

            return true;
        }



        public async Task SendPaymentSuccessEmailAsync(Bill bill, string email)
        {
            var billDetails = await _billService.GetBillDetailsByIdAsync(bill.BillId);

            if (billDetails == null)
            {
                throw new Exception("Bill details not found for email.");
            }

            // Generate email content
            string emailContent = $@"
                    <h1>Payment Successful</h1>
                    <p>Dear Customer,</p>
                    <p>We are pleased to inform you that your payment for Bill #{billDetails.BillId} has been successfully processed.</p>
                    <p><strong>Amount Paid:</strong> {billDetails.TotalBill:C}</p>
                    <p>Thank you for your business.</p>";

            string subject = $"Payment Successful for Bill #{billDetails.BillId}";

            // Send email (using parent's email as an example)
            await _emailService.SendEmailAsync(email, subject, emailContent);
        }

        public async Task<PaymentDetailsDTO> GetPaymentByIdAsync(int paymentId)
        {
            // Retrieve the payment record from the database
            var payment =  _unitOfWork.Payment.GetSingleById(paymentId);


            // Check if payment exists
            if (payment == null)
            {
                throw new Exception($"Payment with ID {paymentId} not found.");
            }

            // Map the payment to a DTO
            var paymentDetails = new PaymentDetailsDTO
            {
                PaymentId = payment.PaymentId,
                BillId = payment.BillId,
                AmountPaid = payment.AmountPaid,
                PaymentMethod = payment.PaymentMethod,
                PaymentDate = payment.PaymentDate,
                TransactionId = payment.TransactionId,
                PaymentStatus = payment.PaymentStatus,
                OrderId = payment.OrderId,
                Currency = payment.Currency,
                BillStatus = payment.Bill?.Status,
                BillTotalAmount = payment.Bill?.TotalBill
            };

            return paymentDetails;
        }

        public async Task<PagedResult<PaymentDetailsDTO>> GetListPaymentAsync(
            int pageIndex = 0,
            int pageSize = 20,
            string searchKeyword = null,
            string paymentStatus = null)
        {
            // Build the predicate (filtering conditions)
            Expression<Func<Payment, bool>> predicate = payment =>
                (string.IsNullOrEmpty(searchKeyword) ||
                 payment.TransactionId.Contains(searchKeyword) ||
                 payment.OrderId.Contains(searchKeyword)) &&
                (string.IsNullOrEmpty(paymentStatus) ||
                 payment.PaymentStatus == paymentStatus);

            // Use GetMultiPaging to retrieve filtered and paginated data
            var totalRecords = 0;
            var payments = _unitOfWork.Payment.GetMultiPaging(
                predicate,
                out totalRecords,
                index: pageIndex,
                size: pageSize,
                includes: new[] { "Bill" } // Include associated Bill data
            );

            // Map to PaymentDetailsDTO
            var paymentList = payments.Select(payment => new PaymentDetailsDTO
            {
                PaymentId = payment.PaymentId,
                BillId = payment.BillId,
                AmountPaid = payment.AmountPaid,
                PaymentMethod = payment.PaymentMethod,
                PaymentDate = payment.PaymentDate,
                TransactionId = payment.TransactionId,
                PaymentStatus = payment.PaymentStatus,
                OrderId = payment.OrderId,
                Currency = payment.Currency,
                BillStatus = payment.Bill?.Status,
                BillTotalAmount = payment.Bill?.TotalBill
            }).ToList();

            // Wrap the result in a PagedResult object
            return new PagedResult<PaymentDetailsDTO>
            {
                Items = paymentList,
                TotalRecords = totalRecords
            };
        }

        public async Task<PagedResult<PaymentDetailsDTO>> GetPaymentsByTutorLearnerSubjectIdAsync(
                int tutorLearnerSubjectId,
                int pageIndex = 0,
                int pageSize = 20)
        {
            // Build the predicate (filtering condition) to find payments related to the specified TutorLearnerSubjectId
            Expression<Func<Payment, bool>> predicate = payment =>
                payment.Bill != null &&
                payment.Bill.BillingEntries.Any(entry => entry.TutorLearnerSubjectId == tutorLearnerSubjectId);

            // Use GetMultiPaging to retrieve filtered and paginated payments
            var totalRecords = 0;
            var payments = _unitOfWork.Payment.GetMultiPaging(
                predicate,
                out totalRecords,
                index: pageIndex,
                size: pageSize,
                includes: new[] { "Bill", "Bill.BillingEntries" } 
            );

            // Map payments to PaymentDetailsDTO
            var paymentList = payments.Select(payment => new PaymentDetailsDTO
            {
                PaymentId = payment.PaymentId,
                BillId = payment.BillId,
                AmountPaid = payment.AmountPaid,
                PaymentMethod = payment.PaymentMethod,
                PaymentDate = payment.PaymentDate,
                TransactionId = payment.TransactionId,
                PaymentStatus = payment.PaymentStatus,
                OrderId = payment.OrderId,
                Currency = payment.Currency,
                BillStatus = payment.Bill?.Status,
                BillTotalAmount = payment.Bill?.TotalBill
            }).ToList();

            // Wrap the result in a PagedResult object
            return new PagedResult<PaymentDetailsDTO>
            {
                Items = paymentList,
                TotalRecords = totalRecords
            };
        }

        

    }
}
