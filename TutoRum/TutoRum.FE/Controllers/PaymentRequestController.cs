using Microsoft.AspNetCore.Mvc;
using TutoRum.Data.Models;
using TutoRum.FE.Common;
using TutoRum.Services.IService;
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
                return Redirect("https://tutor-rum-project.vercel.app/user/settings/wallet/");
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

    }

}
