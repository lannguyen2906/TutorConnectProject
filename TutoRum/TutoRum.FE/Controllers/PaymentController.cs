using Microsoft.AspNetCore.Mvc;
using TutoRum.FE.Common;
using TutoRum.FE.VNPay;
using TutoRum.Services.IService;
using TutoRum.Services.Service;
using TutoRum.Services.ViewModels;

namespace TutoRum.FE.Controllers
{
    public class PaymentController : ApiControllerBase
    {
        private readonly IVnPayService _vnPayService;
        private readonly IBillService _billService;
        private readonly IPaymentService _paymentService;


        public PaymentController(IVnPayService vnPayService, IBillService billService, IPaymentService paymentService)
        {
            _vnPayService = vnPayService;
            _billService = billService;
            _paymentService = paymentService;
        }

        /// <summary>
        /// Tạo URL thanh toán VNPAY
        /// </summary>
        /// <param name="model">Thông tin thanh toán</param>
        /// <returns>URL thanh toán</returns>
        [HttpPost]
        [Route("CreatePaymentUrl/{billId}")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> CreatePaymentUrl(int billId)
        {
            try
            {
                // Lấy thông tin hóa đơn dựa trên billId
                var billDetails = await _billService.GetBillDetailsByIdAsync(billId);
                if (billDetails == null)
                {
                    return NotFound(new { Message = "Bill not found." });
                }

                // Chuẩn bị PaymentInformationModel từ chi tiết hóa đơn
                var model = new PaymentInformationModel
                {
                    OrderType = "billpayment",
                    Amount = (double)(billDetails.TotalBill ?? 0),
                    OrderDescription = $"Thanh toán hóa đơn #{billId}",
                    Name = "Thông tin thanh toán hóa đơn",
                    billId = billId
                };

                // Tạo URL thanh toán VNPAY
                var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

                return Ok(new ApiResponse<string>(200, true, "Operation successful", url));

            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.ServerError<string>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }


        /// <summary>
        /// Xử lý phản hồi thanh toán từ VNPAY
        /// </summary>
        /// <param name="collections">Dữ liệu phản hồi từ VNPAY</param>
        /// <returns>Kết quả xử lý thanh toán</returns>
        [HttpGet]
        [Route("PaymentCallback")]
        [ProducesResponseType(typeof(ApiResponse<PaymentResponseModel>), 200)]
        public async Task<IActionResult> PaymentExecute()
        {
            try
            {
                var response = _vnPayService.PaymentExecute(Request.Query);

                // Process the payment
                var result = await _paymentService.ProcessPaymentAsync(response, User);

                if (result)
                {
                    return Ok(new ApiResponse<PaymentResponseModel>(200, true, "Payment processed successfully", response));
                }

                return BadRequest(new ApiResponse<PaymentResponseModel>(400, false, "Failed to process payment", response));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }


        [HttpGet]
        [Route("GetPaymentById/{paymentId}")]
        [ProducesResponseType(typeof(ApiResponse<PaymentDetailsDTO>), 200)]
        public async Task<IActionResult> GetPaymentById(int paymentId)
        {
            try
            {
                var paymentDetails = await _paymentService.GetPaymentByIdAsync(paymentId);
                return Ok(new ApiResponse<PaymentDetailsDTO>(200, true, "Payment retrieved successfully", paymentDetails));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(500, false, ex.Message, null));
            }
        }

        [HttpGet]
        [Route("GetListPayments")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<PaymentDetailsDTO>>), 200)]
        public async Task<IActionResult> GetListPayments(
            int pageIndex = 0,
            int pageSize = 20,
            string searchKeyword = null,
            string paymentStatus = null)
        {
            try
            {
                var result = await _paymentService.GetListPaymentAsync(pageIndex, pageSize, searchKeyword, paymentStatus);
                return Ok(new ApiResponse<PagedResult<PaymentDetailsDTO>>(200, true, "Payments retrieved successfully", result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(500, false, ex.Message, null));
            }
        }


        [HttpGet]
        [Route("GetPaymentsByTutorLearnerSubjectId/{tutorLearnerSubjectId}")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<PaymentDetailsDTO>>), 200)]
        public async Task<IActionResult> GetPaymentsByTutorLearnerSubjectId(int tutorLearnerSubjectId, int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var result = await _paymentService.GetPaymentsByTutorLearnerSubjectIdAsync(tutorLearnerSubjectId, pageIndex, pageSize);
                return Ok(new ApiResponse<PagedResult<PaymentDetailsDTO>>(200, true, "Payments retrieved successfully", result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(500, false, ex.Message, null));
            }
        }


        //[HttpGet]
        //[Route("GetTutorByBillId/{billId}")]
        //[ProducesResponseType(typeof(ApiResponse<TutorDto>), 200)]
        //public async Task<IActionResult> GetTutorByBillId(int billId)
        //{
        //    try
        //    {
        //        var tutor = await _paymentService.GetTutorByBillIdAsync(billId);
        //        return Ok(new ApiResponse<TutorDto>(200, true, "Tutor retrieved successfully", tutor));
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new ApiResponse<string>(500, false, ex.Message, null));
        //    }
        //}
    }
}
