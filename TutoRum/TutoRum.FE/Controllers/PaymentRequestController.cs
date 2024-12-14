using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using TutoRum.Data.Models;
using TutoRum.FE.Common;
using TutoRum.Services.IService;
using TutoRum.Services.Service;
using TutoRum.Services.ViewModels;

namespace TutoRum.FE.Controllers
{
    public class PaymentRequestController : ApiControllerBase
    {
        private readonly IPaymentRequestService _paymentRequestService;

        public PaymentRequestController(IPaymentRequestService paymentRequestService)
        {
            _paymentRequestService = paymentRequestService;
        }

        /// <summary>
        /// Create a new payment request
        /// </summary>
        /// <param name="requestDto">Payment request data</param>
        /// <returns>Result of payment request creation</returns>
        [HttpPost]
        [Route("CreatePaymentRequest")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> CreatePaymentRequest([FromBody] CreatePaymentRequestDTO requestDto)
        {
            try
            {
                var result = await _paymentRequestService.CreatePaymentRequestAsync(requestDto, User);
                if (result != null)
                {
                    var token = await _paymentRequestService.GeneratePaymentRequestTokenAsync(result.PaymentRequestId);
                    var confirmationUrl = Url.Action("ConfirmPaymentRequest", "PaymentRequest",
             new { requestId = result.PaymentRequestId, token }, Request.Scheme);
                    await _paymentRequestService.SendPaymentRequestConfirmationEmailAsync(User, result, confirmationUrl);

                    return Ok(new ApiResponse<string>(200, true, "Payment request created successfully", null));
                }

                return BadRequest(new ApiResponse<string>(400, false, "Failed to create payment request", null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(500, false, ex.Message, null));
            }
        }

        /// <summary>
        /// Get list of payment requests
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="searchKeyword">Search keyword (optional)</param>
        /// <param name="status">Payment request status (optional)</param>
        /// <returns>Paged list of payment requests</returns>
        [HttpGet]
        [Route("GetListPaymentRequests")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<PaymentRequestDTO>>), 200)]
        public async Task<IActionResult> GetListPaymentRequests(
            [FromQuery] PaymentRequestFilterDTO filterDto,
            int pageIndex = 0,
            int pageSize = 20)
        {
            try
            {
                var result = await _paymentRequestService.GetListPaymentRequestsAsync(filterDto, pageIndex, pageSize);
                return Ok(new ApiResponse<PagedResult<PaymentRequestDTO>>(200, true, "Payment requests retrieved successfully", result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(500, false, ex.Message, null));
            }
        }

        /// <summary>
        /// Get list of payment requests
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="searchKeyword">Search keyword (optional)</param>
        /// <param name="status">Payment request status (optional)</param>
        /// <returns>Paged list of payment requests</returns>
        [HttpGet]
        [Route("GetListPaymentRequestsByTutor")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<PaymentRequestDTO>>), 200)]
        public async Task<IActionResult> GetListPaymentRequestsByTutor(
            int pageIndex = 0,
            int pageSize = 20)
        {
            try
            {
                var result = await _paymentRequestService.GetListPaymentRequestsByTutorAsync(User,pageIndex, pageSize);
                return Ok(new ApiResponse<PagedResult<PaymentRequestDTO>>(200, true, "Payment requests retrieved successfully", result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(500, false, ex.Message, null));
            }
        }

        /// <summary>
        /// Approve a payment request
        /// </summary>
        /// <param name="paymentRequestId">ID of the payment request</param>
        /// <returns>Result of the operation</returns>
        [HttpPut]
        [Route("ApprovePaymentRequest/{paymentRequestId}")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> ApprovePaymentRequest(int paymentRequestId)
        {
            try
            {
                await _paymentRequestService.ApprovePaymentRequestAsync(paymentRequestId, User);
                return Ok(new ApiResponse<string>(200, true, "Payment request approved successfully.", null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(500, false, ex.Message, null));
            }
        }

        /// <summary>
        /// Reject a payment request
        /// </summary>
        /// <param name="paymentRequestId">ID of the payment request</param>
        /// <param name="rejectionReason">Reason for rejection</param>
        /// <returns>Result of the operation</returns>
        [HttpPut]
        [Route("RejectPaymentRequest/{paymentRequestId}")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> RejectPaymentRequest(int paymentRequestId, [FromBody] RejectPaymentRequestDTO rejectionDto)
        {
            try
            {
                if (string.IsNullOrEmpty(rejectionDto.RejectionReason))
                {
                    return BadRequest(new ApiResponse<string>(400, false, "Rejection reason is required.", null));
                }

                await _paymentRequestService.RejectPaymentRequestAsync(paymentRequestId, rejectionDto.RejectionReason, User);
                return Ok(new ApiResponse<string>(200, true, "Payment request rejected successfully.", null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(500, false, ex.Message, null));
            }
        }

        [HttpPut("UpdatePaymentRequest/{id}")]
        public async Task<IActionResult> UpdatePaymentRequest(int id, [FromBody] UpdatePaymentRequestDTO updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _paymentRequestService.UpdatePaymentRequestByIdAsync(id, updateDto, User);
                if (!result)
                {
                    return NotFound("Payment request not found or could not be updated.");
                }
                return Ok("Payment request updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ConfirmPaymentRequest")]
        public async Task<IActionResult> ConfirmPaymentRequest(int requestId, Guid token)
        {
            try
            {
                var result = await _paymentRequestService.ConfirmPaymentRequest(requestId, token);
                if (!result)
                {
                    return BadRequest("Email confirmation failed.");
                }
                return Redirect("https://tutor-connect-deploy-six.vercel.app/user/settings/wallet/");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeletePaymentRequestById/{id}")]
        public async Task<IActionResult> DeletePaymentRequestById(int id)
        {
            try
            {
                var result = await _paymentRequestService.DeletePaymentRequest(id, User);
                if (!result)
                {
                    return NotFound("Payment request not found or could not be deleted.");
                }
                return Ok("Payment request deleted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("upload-payment-requests")]
        public async Task<IActionResult> UploadPaymentRequests(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                var paymentRequests = await ParseExcelFileAsync(file);

                await _paymentRequestService.ProcessPaymentRequests(paymentRequests);

                return Ok(new { Message = "File processed successfully.", Result = paymentRequests });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error processing file.", Details = ex.Message });
            }
        }

        [HttpGet("download-payment-requests")]
        public IActionResult DownloadPaymentRequests()
        {
            // Giả sử bạn có phương thức lấy danh sách PaymentRequest
            List<PaymentRequest> paymentRequests = _paymentRequestService.GetPendingPaymentRequests();

            // Map dữ liệu sang file Excel
            var fileContents = MapToExcel(paymentRequests);

            // Trả về file Excel dưới dạng file tải về
            return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PaymentRequests.xlsx");
        }

        private byte[] MapToExcel(List<PaymentRequest> paymentRequests)
        {
            using (var package = new ExcelPackage())
            {
                // Tạo sheet mới trong workbook
                var worksheet = package.Workbook.Worksheets.Add("PaymentRequests");

                // Thiết lập tiêu đề cho các cột
                worksheet.Cells[1, 1].Value = "Mã yêu cầu";
                worksheet.Cells[1, 2].Value = "Mã ngân hàng";
                worksheet.Cells[1, 3].Value = "Số tài khoản";
                worksheet.Cells[1, 4].Value = "Tên tài khoản";
                worksheet.Cells[1, 5].Value = "Số tiền cần chuyển";
                worksheet.Cells[1, 6].Value = "Đã chuyển tiền";
                worksheet.Cells[1, 7].Value = "Ghi chú";

                // Gắn dữ liệu vào các ô
                for (int row = 0; row < paymentRequests.Count; row++)
                {
                    var paymentRequest = paymentRequests[row];
                    worksheet.Cells[row + 2, 1].Value = paymentRequest.PaymentRequestId;
                    worksheet.Cells[row + 2, 2].Value = paymentRequest.BankCode;
                    worksheet.Cells[row + 2, 3].Value = paymentRequest.AccountNumber;
                    worksheet.Cells[row + 2, 4].Value = paymentRequest.FullName;
                    worksheet.Cells[row + 2, 5].Value = paymentRequest.Amount;
                    worksheet.Cells[row + 2, 6].Value = paymentRequest.IsPaid;
                    worksheet.Cells[row + 2, 7].Value = paymentRequest.reasonDesc;
                }

                // Trả về file dưới dạng byte[]
                return package.GetAsByteArray();
            }
        }

        private async Task<List<PaymentRequest>> ParseExcelFileAsync(IFormFile file)
        {
            var paymentRequests = new List<PaymentRequest>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // Sheet đầu tiên
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++) // Bắt đầu từ hàng 2 (bỏ header)
                    {
                        var paymentRequest = new PaymentRequest
                        {
                            PaymentRequestId = int.Parse(worksheet.Cells[row, 1].Text.Trim()),
                            BankCode = worksheet.Cells[row, 2].Text.Trim(),
                            AccountNumber = worksheet.Cells[row, 3].Text.Trim(),
                            FullName = worksheet.Cells[row,4].Text.Trim(),
                            Amount = decimal.Parse(worksheet.Cells[row, 5].Text.Trim()),
                            IsPaid = bool.Parse(worksheet.Cells[row, 6].Text.Trim()),
                            reasonDesc = worksheet.Cells[row, 7].Text.Trim()
                        };

                        paymentRequests.Add(paymentRequest);
                    }
                }
            }

            return paymentRequests;
        }

    }

}
