using DinkToPdf.Contracts;
using DinkToPdf;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.Models;
using TutoRum.Services.IService;
using TutoRum.Services.ViewModels;

using System.IO;
using System.Threading.Tasks;
using QRCoder;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using TutoRum.Data.Enum;

namespace TutoRum.Services.Service
{
    public class BillService : IBillService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly IEmailService _emailService;


        public BillService(IUnitOfWork unitOfWork, UserManager<AspNetUser> userManager, IEmailService emailService)
        {
            this._unitOfWork = unitOfWork;
            this._userManager = userManager;
            _emailService = emailService;
        }


        public async Task<BillDTOS> GetAllBillsAsync(ClaimsPrincipal user, int pageIndex = 0, int pageSize = 20)
        {
            // Lấy thông tin người dùng hiện tại
            var currentUser = await _userManager.GetUserAsync(user);

            if (currentUser == null)
            {
                throw new Exception("User not found.");
            }

            // Lấy tất cả Bill từ cơ sở dữ liệu với phân trang
            int totalRecords;
            var bills = _unitOfWork.Bill.GetMultiPaging(
                bill => !bill.IsDelete, // Không có bộ lọc, lấy tất cả các Bill
                out totalRecords,
                index: pageIndex,
                size: pageSize);

            if (bills == null || !bills.Any())
            {
                throw new Exception("No bills found.");
            }

            // Chuyển đổi các Bill sang BillDTO để trả về
            var billDTOs = bills.Select(bill => new BillDTO
            {
                BillId = bill.BillId,
                Discount = bill.Discount,
                StartDate = bill.StartDate,
                Description = bill.Description,
                TotalBill = bill.TotalBill,
                Status = bill.Status,
                CreatedDate = bill.CreatedDate,
                UpdatedDate = bill.UpdatedDate,
                
            }).ToList();

            // Trả về đối tượng BillDTOS chứa totalRecords và danh sách BillDTO
            return new BillDTOS
            {
                TotalRecords = totalRecords,
                Bills = billDTOs
            };
        }



        public async Task<int> GenerateBillFromBillingEntriesAsync(List<int> billingEntryIds, ClaimsPrincipal user)
        {
            // Lấy thông tin người dùng hiện tại
            var currentUser = await _userManager.GetUserAsync(user);

            if (currentUser == null)
            {
                throw new Exception("User not found.");
            }
            else
            {
                // Lấy danh sách các BillingEntry từ database dựa trên billingEntryIds
                var billingEntries =  _unitOfWork.BillingEntry
                                    .GetMultiAsQueryable(be => billingEntryIds.Contains(be.BillingEntryId))
                                    .ToList();

                if (!billingEntries.Any())
                {
                    throw new InvalidOperationException("No valid billing entries found for the provided IDs.");
                }

                // Tính tổng số tiền của tất cả các BillingEntry
                decimal totalAmount = billingEntries.Sum(entry => entry.TotalAmount ?? 0);

                // Tạo đối tượng Bill mới
                var newBill = new Bill
                {
                    Discount = 0, // Có thể tính toán giảm giá nếu cần
                    StartDate = DateTime.UtcNow, // Hoặc có thể lấy từ ngày sớm nhất trong các BillingEntry
                    Description = "Generated bill from billing entries",
                    TotalBill = totalAmount,
                    Deduction = totalAmount*5/100,
                    Status = "Pending", // Trạng thái ban đầu của hóa đơn
                    CreatedBy = currentUser.Id,
                    CreatedDate = DateTime.UtcNow,
                };

                // Thêm hóa đơn vào UnitOfWork
                _unitOfWork.Bill.Add(newBill);

                // Lưu thay đổi để Bill có ID (nếu dùng SQL Server, ID sẽ được gán sau khi lưu vào DB)
                await _unitOfWork.CommitAsync();

                // Gán BillId cho từng BillingEntry và cập nhật trong database
                foreach (var entry in billingEntries)
                {
                    entry.BillId = newBill.BillId;
                    _unitOfWork.BillingEntry.Update(entry);
                }

                // Lưu thay đổi cuối cùng vào cơ sở dữ liệu
                await _unitOfWork.CommitAsync();
                return newBill.BillId;
            }
        }

        public async Task DeleteBillAsync(int billId, ClaimsPrincipal user)
        {
            // Get the current user
            var currentUser = await _userManager.GetUserAsync(user);

            if (currentUser == null)
            {
                throw new Exception("User not found.");
            }

            // Fetch the Bill from the database
            var bill = _unitOfWork.Bill.GetSingleById(billId);

            if (bill == null)
            {
                throw new InvalidOperationException("Bill not found.");
            }
            else
            {
                bill.IsDelete = true;
                bill.UpdatedBy = currentUser.Id;
                bill.UpdatedDate = DateTime.UtcNow;
            }

            // Update the BillId of related BillingEntry records to null
            var billingEntries = _unitOfWork.BillingEntry.GetMultiAsQueryable(be => be.BillId == billId).ToList();
            foreach (var entry in billingEntries)
            {
                entry.BillId = null;
                _unitOfWork.BillingEntry.Update(entry);
            }

            // Delete the Bill

            _unitOfWork.Bill.Update(bill);

            // Save changes to the database
            await _unitOfWork.CommitAsync();
        }



        public async Task<BillDetailsDTO> GetBillDetailsByIdAsync(int billId)
        {
            // Lấy Bill từ cơ sở dữ liệu, bao gồm các BillingEntry liên quan
            var bill =  _unitOfWork.Bill.GetMultiAsQueryable(b => b.BillId == billId)
                        .Include(b => b.BillingEntries) // Bao gồm các BillingEntry liên quan
                        .ThenInclude(be => be.TutorLearnerSubject)
                        .FirstOrDefault();

            if (bill == null)
            {
                throw new InvalidOperationException("Bill not found.");
            }

            var learnerID = bill.BillingEntries.Select(be => be.TutorLearnerSubject.LearnerId).FirstOrDefault();

            var learner = new AspNetUser();
            if (learnerID.HasValue)
            {
                learner = _unitOfWork.Accounts.GetSingleByGuId(learnerID.Value);
            }

            // Chuyển đổi Bill và các BillingEntry thành DTO
            var billDetailsDTO = new BillDetailsDTO
            {
                BillId = bill.BillId,
                Discount = bill.Discount,
                Deduction = bill.Deduction,
                StartDate = bill.StartDate,
                Description = bill.Description,
                TotalBill = bill.TotalBill,
                IsApprove = bill.ISApprove,
                IsPaid = bill.IsPaid,
                Status = bill.Status,
                CreatedDate = bill.CreatedDate,
                UpdatedDate = bill.UpdatedDate,
                LearnerEmail = learner.Email,
                BillingEntries = bill.BillingEntries.Select(entry => new BillingEntryDTO
                {
                    TutorLearnerSubjectId = entry.TutorLearnerSubjectId,
                    Rate = entry.Rate,
                    StartDateTime = entry.StartDateTime,
                    EndDateTime = entry.EndDateTime,
                    Description = entry.Description,
                    TotalAmount = entry.TotalAmount
                }).ToList()
                
            };

            return billDetailsDTO;
        }


        public async Task<string> ViewBillHtmlAsync(int billId)
        {
            // Lấy thông tin chi tiết của hóa đơn từ cơ sở dữ liệu
            var billDetails = await GetBillDetailsByIdAsync(billId);

            if (billDetails == null)
            {
                throw new Exception("Bill not found.");
            }

            // Tạo mã QR từ URL thanh toán
            string paymentUrl = "https://yourwebsite.com/payment?billId=" + billDetails.BillId;
            string qrImageBase64 = GenerateQrCodeBase64(paymentUrl);

            // URL để tải xuống PDF
            string downloadUrl = "/api/bill/GenerateBillPdf?billId=" + billId;

            // Tạo nội dung HTML cho hóa đơn
            string htmlContent = GenerateBillHtmlContent(billDetails, qrImageBase64, paymentUrl, downloadUrl);

            return htmlContent;
        }


        public async Task<byte[]> GenerateBillPdfAsync(int billId)
        {
            var _converter = new SynchronizedConverter(new PdfTools());
            var billDetails = await GetBillDetailsByIdAsync(billId);

            // Tạo mã QR từ URL thanh toán
            string paymentUrl = "https://yourwebsite.com/payment?billId=" + billDetails.BillId;
            string qrImageBase64 = GenerateQrCodeBase64(paymentUrl);


            // Check bill da chap thuan chua
            // Sử dụng phương thức chung để tạo nội dung HTML cho PDF
            string htmlContent = GenerateBillHtmlContent(billDetails, qrImageBase64, paymentUrl);

            var pdfDoc = new HtmlToPdfDocument()
            {
                GlobalSettings = new GlobalSettings
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait,
                    Out = null
                }
            };

            pdfDoc.Objects.Add(new ObjectSettings
            {
                HtmlContent = htmlContent,
                WebSettings = { DefaultEncoding = "utf-8" }
            });

            return _converter.Convert(pdfDoc);
        }

        private string GenerateQrCodeBase64(string paymentUrl)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(paymentUrl, QRCodeGenerator.ECCLevel.Q))
            using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
            {
                byte[] qrCodeImage = qrCode.GetGraphic(5);
                return Convert.ToBase64String(qrCodeImage);
            }
        }


        private string GenerateBillHtmlContent(BillDetailsDTO billDetails, string qrImageBase64 = null, string paymentUrl = null, string downloadUrl = null)
        {
            string htmlContent = $@"
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; }}
                    h1 {{ text-align: center; }}
                    p, button {{ font-size: 14px; }}
                    table {{ width: 100%; border-collapse: collapse; margin-top: 20px; }}
                    th, td {{ padding: 8px 12px; border: 1px solid #ddd; }}
                    th {{ background-color: #f2f2f2; text-align: left; }}
                    .payment-section {{ text-align: center; margin-top: 20px; display: flex; justify-content: center; flex-direction: column; align-items: center; }}
                    .qr-image {{ width: 150px; }} /* Set QR code image width */
                </style>
            </head>
            <body>
                <h1>HÓA ĐƠN THANH TOÁN</h1>
                <p><strong>Mã hóa đơn:</strong> {billDetails.BillId}</p>
                <p><strong>Ngày tạo:</strong> {billDetails.CreatedDate?.ToString("dd/MM/yyyy")}</p>
                <p><strong>Trạng thái:</strong> {billDetails.Status}</p>
                <p><strong>Mô tả:</strong> {billDetails.Description}</p>
                <p><strong>Tổng tiền hóa đơn:</strong> {billDetails.TotalBill?.ToString("N0")} VND</p>
                <p><strong>Giảm giá:</strong> {billDetails.Discount?.ToString("N0")} VND</p>
                <p><strong>Khấu trừ:</strong> {billDetails.Deduction?.ToString("N0")} VND</p>
                <p><strong>Tổng thanh toán:</strong> {(billDetails.TotalBill - (billDetails.Discount ?? 0))?.ToString("N0")} VND</p>

                <h2>Chi tiết các mục trong hóa đơn</h2>
                <table>
                    <tr>
                        <th>Mã môn học</th>
                        <th>Mô tả</th>
                        <th>Đơn giá</th>
                        <th>Ngày bắt đầu</th>
                        <th>Ngày kết thúc</th>
                        <th>Tổng tiền</th>
                    </tr>";

                    foreach (var entry in billDetails.BillingEntries)
                        {
                        htmlContent += $@"
                    <tr>
                        <td>{entry.TutorLearnerSubjectId}</td>
                        <td>{entry.Description}</td>
                        <td>{entry.Rate?.ToString("N0")} VND</td>
                        <td>{entry.StartDateTime?.ToString("dd/MM/yyyy HH:mm")}</td>
                        <td>{entry.EndDateTime?.ToString("dd/MM/yyyy HH:mm")}</td>
                        <td>{entry.TotalAmount?.ToString("N0")} VND</td>
                    </tr>";
                        }


            // Close the table and add the QR code section
            if (billDetails.IsApprove == true)
            {
                htmlContent += $@"
                </table>
                <div class='payment-section'>
                    <p>Quét mã QR để thanh toán:</p>
                    <img class='qr-image' src='data:image/png;base64,{qrImageBase64}' alt='QR Code' /> <!-- Apply class here -->
                    <br>
                </div>
            </body>
            </html>";
            }

            htmlContent += @"
            </div>
        </body>
        </html>";

            return htmlContent;
        }


        public async Task<bool> ApproveBillByIdAsync(int billId, ClaimsPrincipal user)
        {
            // Lấy thông tin người dùng hiện tại
            var currentUser = await _userManager.GetUserAsync(user);

            if (currentUser == null)
            {
                throw new Exception("User not found.");
            }

            // Tìm hóa đơn theo BillId
            var bill = _unitOfWork.Bill.GetSingleById(billId);

            if (bill == null)
            {
                throw new Exception("Bill not found.");
            }

            // Kiểm tra trạng thái hóa đơn trước khi phê duyệt
            if (bill.ISApprove == true)
            {
                throw new Exception("Bill is already approved.");
            }

            // Thực hiện phê duyệt hóa đơn
            bill.ISApprove = true;
            bill.Status = "Đã chấp nhận"; // Cập nhật trạng thái hóa đơn
            bill.UpdatedDate = DateTime.UtcNow; // Cập nhật thời gian chỉnh sửa cuối cùng

            // Cập nhật vào cơ sở dữ liệu
            _unitOfWork.Bill.Update(bill);

            // Đợi hoàn thành commit (không cần gán vì CommitAsync() trả về void)
            await _unitOfWork.CommitAsync();

            // Trả về true nếu không có ngoại lệ xảy ra
            return true;
        }


        public async Task SendBillEmailAsync(int billId)
        {
            // Get bill details
            var billDetails = await GetBillDetailsByIdAsync(billId);

            if (billDetails == null)
            {
                throw new Exception("Bill not found.");
            }


            // Generate bill HTML content
            string htmlContent = GenerateBillHtmlContent(billDetails);

            // Add approval button to the email content
            htmlContent += $@"
                <div style='text-align: center; margin-top: 20px;'>
                    <a href='https://tutor-rum-project.vercel.app/user/learning-classrooms/{billDetails.BillingEntries.First().TutorLearnerSubjectId}/bills/{billId}' 
                       style='display: inline-block; padding: 10px 20px; background-color: #28a745; color: #fff; text-decoration: none; border-radius: 5px;'>
                        Approve Bill
                    </a>
                </div>";

            // Send email
            var emailSubject = $"Review and Approve Bill #{billDetails.BillId}";
            await _emailService.SendEmailAsync(billDetails.LearnerEmail, emailSubject, htmlContent);
        }

        public async Task<PagedResult<BillDetailsDTO>> GetBillByTutorLearnerSubjectIdAsync(
         int tutorLearnerSubjectId,
         int pageIndex = 0,
         int pageSize = 10)
        {
            // Build the predicate to filter bills by TutorLearnerSubjectId
            Expression<Func<Bill, bool>> predicate = b => !b.IsDelete && 
                b.BillingEntries.Any(be => be.TutorLearnerSubjectId == tutorLearnerSubjectId);

            // Use GetMultiPaging to retrieve filtered and paginated data
            var totalRecords = 0;
            var bills = _unitOfWork.Bill.GetMultiPaging(
                predicate,
                out totalRecords,
                index: pageIndex,
                size: pageSize,
                includes: new[] { "BillingEntries", "BillingEntries.TutorLearnerSubject" }); // Include necessary relationships

            // Retrieve learner ID from the first relevant BillingEntry
            var learnerId = bills.SelectMany(b =>  b.BillingEntries)
                                 .Where(be => be.TutorLearnerSubjectId == tutorLearnerSubjectId)
                                 .Select(be => be.TutorLearnerSubject.LearnerId)
                                 .FirstOrDefault();

            // Retrieve learner details if a valid learner ID is found
            AspNetUser learner = null;
            if (learnerId.HasValue)
            {
                learner = _unitOfWork.Accounts.GetSingleByGuId(learnerId.Value);
            }

            // Map bills to BillDetailsDTO
            var billDetailsList = bills.Select(bill => new BillDetailsDTO
            {
                BillId = bill.BillId,
                Discount = bill.Discount,
                Deduction = bill.Deduction,
                StartDate = bill.StartDate,
                Description = bill.Description,
                TotalBill = bill.TotalBill,
                IsApprove = bill.ISApprove,
                Status = bill.Status,
                CreatedDate = bill.CreatedDate,
                UpdatedDate = bill.UpdatedDate,
                IsPaid = bill.IsPaid,
                LearnerEmail = learner?.Email,
                BillingEntries = bill.BillingEntries.Select(entry => new BillingEntryDTO
                {
                    BillingEntryID = entry.BillingEntryId,
                    TutorLearnerSubjectId = entry.TutorLearnerSubjectId,
                    Rate = entry.Rate,
                    StartDateTime = entry.StartDateTime,
                    EndDateTime = entry.EndDateTime,
                    Description = entry.Description,
                    TotalAmount = entry.TotalAmount
                }).ToList()
            }).ToList();

            // Wrap the result in a PagedResult object
            return new PagedResult<BillDetailsDTO>
            {
                Items = billDetailsList,
                TotalRecords = totalRecords
            };
        }

        public async Task<PagedResult<BillDetailsDTO>> GetBillByTutorAsync(
        ClaimsPrincipal user,
        int pageIndex = 0,
        int pageSize = 10)
        {
            // Lấy thông tin người dùng hiện tại
            var currentUser = await _userManager.GetUserAsync(user);

            if (currentUser == null)
            {
                throw new Exception("User not found.");
            }

            var isTutor = await _userManager.IsInRoleAsync(currentUser, AccountRoles.Tutor);

            if (!isTutor) {
                throw new Exception("User is not a tutor");
            }

            // Build the predicate to filter bills by TutorLearnerSubjectId
            Expression<Func<Bill, bool>> predicate = b => !b.IsDelete &&
                b.BillingEntries.Any(be => be.TutorLearnerSubject.TutorSubject.TutorId == currentUser.Id);

            // Use GetMultiPaging to retrieve filtered and paginated data
            var totalRecords = 0;
            var bills = _unitOfWork.Bill.GetMultiPaging(
                predicate,
                out totalRecords,
                index: pageIndex,
                size: pageSize,
                includes: new[] { "BillingEntries", "BillingEntries.TutorLearnerSubject", "BillingEntries.TutorLearnerSubject.TutorSubject", "BillingEntries.TutorLearnerSubject.Learner" }); // Include necessary relationships

            // Retrieve learner ID from the first relevant BillingEntry
            var learnerMail = bills.SelectMany(b => b.BillingEntries)
                                 .Where(be => be.TutorLearnerSubject.TutorSubject.TutorId == currentUser.Id)
                                 .Select(be => be.TutorLearnerSubject)
                                 .FirstOrDefault().Learner.Email;

            // Map bills to BillDetailsDTO
            var billDetailsList = bills.Select(bill => new BillDetailsDTO
            {
                BillId = bill.BillId,
                Discount = bill.Discount,
                Deduction = bill.Deduction,
                StartDate = bill.StartDate,
                Description = bill.Description,
                TotalBill = bill.TotalBill,
                IsApprove = bill.ISApprove,
                Status = bill.Status,
                CreatedDate = bill.CreatedDate,
                UpdatedDate = bill.UpdatedDate,
                IsPaid = bill.IsPaid,
                LearnerEmail = learnerMail,
                BillingEntries = bill.BillingEntries.Select(entry => new BillingEntryDTO
                {
                    BillingEntryID = entry.BillingEntryId,
                    TutorLearnerSubjectId = entry.TutorLearnerSubjectId,
                    Rate = entry.Rate,
                    StartDateTime = entry.StartDateTime,
                    EndDateTime = entry.EndDateTime,
                    Description = entry.Description,
                    TotalAmount = entry.TotalAmount
                }).ToList()
            }).ToList();

            // Wrap the result in a PagedResult object
            return new PagedResult<BillDetailsDTO>
            {
                Items = billDetailsList,
                TotalRecords = totalRecords
            };
        }

    }
}
