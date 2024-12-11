using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TutoRum.FE.Common;
using TutoRum.Services.IService;
using TutoRum.Services.ViewModels;

namespace TutoRum.FE.Controllers
{
    public class FaqController : ApiControllerBase
    {
        private readonly IFaqService _faqService;

        public FaqController(IFaqService faqService)
        {
            _faqService = faqService;
        }
        [HttpGet]
        [Route(Common.Url.User.Faq.GetAllFAQs)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<FaqDto>>), 200)]
        public async Task<IActionResult> GetAllFAQs()
        {
            try
            {
                var faqs = await _faqService.GetAllFAQsAsync();
                var response = ApiResponseFactory.Success(faqs);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }

        [HttpGet]
        [Route(Common.Url.User.Faq.GetFAQById + "/{id}")] 
        [ProducesResponseType(typeof(ApiResponse<FaqDto>), 200)]
        public async Task<IActionResult> GetFAQById(int id)
        {
            try
            {
                var faq = await _faqService.GetFAQByIdAsync(id);
                if (faq == null)
                {
                    var responseNotFound = ApiResponseFactory.NotFound<FaqDto>($"FAQ with ID {id} not found."); // Đổi tên biến
                    return NotFound(responseNotFound);
                }

                var responseSingle = ApiResponseFactory.Success(faq); // Đổi tên biến
                return Ok(responseSingle);
            }
            catch (Exception ex)
            {
                var responseError = ApiResponseFactory.ServerError<object>(detail: ex.Message); // Đổi tên biến
                return StatusCode(500, responseError);
            }
        }
        [HttpPost]
        [Route(Common.Url.User.Faq.CreateFAQ)]
        [ProducesResponseType(typeof(ApiResponse<FaqDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> CreateFAQ([FromBody] FaqCreateDto faqCreateDto)
        {
            if (faqCreateDto == null || string.IsNullOrWhiteSpace(faqCreateDto.Question) || string.IsNullOrWhiteSpace(faqCreateDto.Answer))
            {
                var responseInvalid = ApiResponseFactory.BadRequest<object>("Question and Answer must not be empty.");
                return BadRequest(responseInvalid);
            }

            try
            {
               
                var createdFaq = await _faqService.CreateFAQAsync(faqCreateDto, User);

                var responseCreated = ApiResponseFactory.Success(createdFaq);
                return CreatedAtAction(nameof(GetFAQById), new { id = createdFaq.Id }, responseCreated);
            }
            catch (Exception ex)
            {
                var responseError = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, responseError);
            }
        }

        [HttpPut]
        [Route(Common.Url.User.Faq.UpdateFAQ + "/{id}")]
        [ProducesResponseType(typeof(ApiResponse<FaqDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> UpdateFAQ(int id, [FromBody] FaqUpdateDto faqUpdateDto)
        {
            if (faqUpdateDto == null || string.IsNullOrWhiteSpace(faqUpdateDto.Question) || string.IsNullOrWhiteSpace(faqUpdateDto.Answer))
            {
                var responseInvalid = ApiResponseFactory.BadRequest<object>("Question and Answer must not be empty.");
                return BadRequest(responseInvalid);
            }

            try
            {
                faqUpdateDto.Id = id; // Gán ID từ tham số URL vào DTO
                var updatedFaq = await _faqService.UpdateFAQAsync(faqUpdateDto, User);
                var responseSuccess = ApiResponseFactory.Success(updatedFaq);
                return Ok(responseSuccess);
            }
            catch (KeyNotFoundException)
            {
                var responseNotFound = ApiResponseFactory.NotFound<FaqDto>($"FAQ with ID {id} not found.");
                return NotFound(responseNotFound);
            }
            catch (Exception ex)
            {
                var responseError = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, responseError);
            }
        }
        [HttpDelete]
        [Route(Common.Url.User.Faq.DeleteFAQ + "/{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)] 
        [ProducesResponseType(typeof(ApiResponse<object>), 404)] 
        public async Task<IActionResult> DeleteFAQ(int id)
        {
            try
            {
                await _faqService.DeleteFAQAsync(id, User);
                var responseSuccess = ApiResponseFactory.Success("FAQ deleted successfully.");
                return Ok(responseSuccess);
            }
            catch (UnauthorizedAccessException)
            {
                var responseUnauthorized = ApiResponseFactory.Unauthorized<object>();
                return Unauthorized(responseUnauthorized);
            }
            catch (KeyNotFoundException)
            {
                var responseNotFound = ApiResponseFactory.NotFound<object>($"FAQ with ID {id} not found.");
                return NotFound(responseNotFound);
            }
            catch (Exception ex)
            {
                var responseError = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, responseError);
            }
        }

        [HttpGet]
        [Route(Common.Url.User.Faq.GetHomepageFAQs)] 
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<FaqDto>>), 200)]
        public async Task<IActionResult> GetHomepageFAQs(int index = 0, int size = 20)
        {
            try
            {
                var faqs = await _faqService.GetFaqHomePage(index, size); 
                var response = ApiResponseFactory.Success(faqs);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }


 
    }
}