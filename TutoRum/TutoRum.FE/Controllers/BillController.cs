using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TutoRum.Services.IService;
using TutoRum.Services.ViewModels;
using TutoRum.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static TutoRum.FE.Common.Url;
using TutoRum.FE.Common;
using TutoRum.FE.Controllers;

namespace TutoRum.FE.Controllers
{
    public class BillController : ApiControllerBase
    {
        private readonly IBillService _billService;

        public BillController(IBillService billService)
        {
            _billService = billService;
        }

        [HttpGet]
        [Route(Common.Url.User.Bill.GetAllBills)]
        [ProducesResponseType(typeof(ApiResponse<BillDTOS>), 200)]
        public async Task<IActionResult> GetAllBills([FromQuery] int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var bills = await _billService.GetAllBillsAsync(User, pageIndex, pageSize);
                var response = ApiResponseFactory.Success(bills);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                var response = ApiResponseFactory.Unauthorized<object>(ex.Message);
                return StatusCode(403, response);
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }

        [HttpPost]
        [Route(Common.Url.User.Bill.GenerateBillFromBillingEntries)]
        [ProducesResponseType(typeof(ApiResponse<int>), 200)]
        public async Task<IActionResult> GenerateBillFromBillingEntries([FromBody] List<int> billingEntryIds)
        {
            try
            {
                var billId = await _billService.GenerateBillFromBillingEntriesAsync(billingEntryIds, User);
                var response = ApiResponseFactory.Success(billId);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                var response = ApiResponseFactory.Unauthorized<object>(ex.Message);
                return StatusCode(403, response);
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }

        [HttpDelete]
        [Route(Common.Url.User.Bill.DeleteBill)]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> DeleteBill([FromQuery] int billId)
        {
            try
            {
                await _billService.DeleteBillAsync(billId, User);
                var response = ApiResponseFactory.Success("Bill deleted successfully.");
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                var response = ApiResponseFactory.Unauthorized<object>(ex.Message);
                return StatusCode(403, response);
            }
            catch (KeyNotFoundException ex)
            {
                var response = ApiResponseFactory.NotFound<object>(ex.Message);
                return NotFound(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }

        [HttpGet]
        [Route(Common.Url.User.Bill.GenerateBillPdf)]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        public async Task<IActionResult> GenerateBillPdf([FromQuery] int billId)
        {
            try
            {
                var pdfContent = await _billService.GenerateBillPdfAsync(billId);

                return File(pdfContent, "application/pdf", $"Bill_{billId}.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi hệ thống: " + ex.Message);
            }
        }



        [HttpGet]
        [Route("api/bill/ViewBillHtml")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> ViewBillHtml([FromQuery] int billId)
        {
            try
            {
                // Gọi phương thức từ BillService để lấy nội dung HTML
                string htmlContent = await _billService.ViewBillHtmlAsync(billId);

                // Trả về nội dung HTML
                return Content(htmlContent, "text/html");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi hệ thống: " + ex.Message);
            }
        }

        [HttpPost]
        [Route(Common.Url.User.Bill.ApproveBill)]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> ApproveBill([FromQuery] int billId)
        {
            try
            {
                // Gọi service để phê duyệt hóa đơn
                var isApproved = await _billService.ApproveBillByIdAsync(billId, User);

                // Trả về phản hồi thành công với kết quả
                var response = ApiResponseFactory.Success(isApproved);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                // Trả về phản hồi lỗi không được cấp quyền
                var response = ApiResponseFactory.Unauthorized<object>(ex.Message);
                return StatusCode(403, response);
            }
            catch (Exception ex)
            {
                // Trả về phản hồi lỗi máy chủ
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }

        [HttpPost]
        [Route(Common.Url.User.Bill.SendBillEmail)]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> SendBillEmail([FromQuery] int billId, [FromQuery] string parentEmail)
        {
            try
            {
                // Gọi service để gửi email hóa đơn
                await _billService.SendBillEmailAsync(billId);

                // Trả về phản hồi thành công
                var response = ApiResponseFactory.Success(true);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                // Trả về phản hồi lỗi không được cấp quyền
                var response = ApiResponseFactory.Unauthorized<object>(ex.Message);
                return StatusCode(403, response);
            }
            catch (Exception ex)
            {
                // Trả về phản hồi lỗi máy chủ
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }


        [HttpGet]
        [Route("GetBillByTutorLearnerSubjectId")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<BillDetailsDTO>>), 200)]
        public async Task<IActionResult> GetBillByTutorLearnerSubjectId(
                int tutorLearnerSubjectId,
                int pageIndex = 0,
                int pageSize = 10)
        {
            try
            {
                var result = await _billService.GetBillByTutorLearnerSubjectIdAsync(tutorLearnerSubjectId, pageIndex, pageSize);
                return Ok(new ApiResponse<PagedResult<BillDetailsDTO>>(200, true, "Bills retrieved successfully", result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(500, false, ex.Message, null));
            }
        }

        [HttpGet]
        [Route("GetBillByTutor")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<BillDetailsDTO>>), 200)]
        public async Task<IActionResult> GetBillByTutor(
            int pageIndex = 0,
            int pageSize = 10)
        {
            try
            {
                var result = await _billService.GetBillByTutorAsync(User, pageIndex, pageSize);
                return Ok(new ApiResponse<PagedResult<BillDetailsDTO>>(200, true, "Bills retrieved successfully", result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(500, false, ex.Message, null));
            }
        }

        [HttpGet]
        [Route("GetBillDetailById")]
        [ProducesResponseType(typeof(ApiResponse<BillDetailsDTO>), 200)]
        public async Task<IActionResult> GetBillDetailById(
                int billId)
        {
            try
            {
                var result = await _billService.GetBillDetailsByIdAsync(billId);
                return Ok(new ApiResponse<BillDetailsDTO>(200, true, "Bill retrieved successfully", result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(500, false, ex.Message, null));
            }
        }

    }
}
