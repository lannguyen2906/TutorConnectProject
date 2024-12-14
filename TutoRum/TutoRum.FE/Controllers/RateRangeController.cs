using Microsoft.AspNetCore.Mvc;
using TutoRum.Data.Models;
using TutoRum.FE.Common;
using TutoRum.Services.IService;

namespace TutoRum.FE.Controllers
{
    public class RateRangeController : ApiControllerBase
    {
        private readonly IRateRangeService _rateRangeService;

        public RateRangeController(IRateRangeService rateRangeService)
        {
            _rateRangeService = rateRangeService;
        }

        // Create a new RateRange
        [HttpPost]
        [Route(Common.Url.User.RateRange.Create)]
        [ProducesResponseType(typeof(ApiResponse<RateRange>), 200)]
        public async Task<IActionResult> CreateRateRange([FromBody] RateRange rateRange)
        {
            try
            {
                var result = await _rateRangeService.CreateRateRangeAsync(rateRange);
                var response = ApiResponseFactory.Success(result);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.ServerError<object>(ex.Message);
                return StatusCode(500, response);
            }
        }

        // Get all RateRanges
        [HttpGet]
        [Route(Common.Url.User.RateRange.GetAll)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<RateRange>>), 200)]
        public async Task<IActionResult> GetAllRateRanges()
        {
            try
            {
                var result = await _rateRangeService.GetAllRateRangesAsync();
                var response = ApiResponseFactory.Success(result);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.ServerError<object>(ex.Message);
                return StatusCode(500, response);
            }
        }

        // Get RateRange by Id
        [HttpGet]
        [Route(Common.Url.User.RateRange.GetById)]
        [ProducesResponseType(typeof(ApiResponse<RateRange>), 200)]
        public async Task<IActionResult> GetRateRangeById(int id)
        {
            try
            {
                var result = await _rateRangeService.GetRateRangeByIdAsync(id);
                var response = ApiResponseFactory.Success(result);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                var response = ApiResponseFactory.NotFound<object>(ex.Message);
                return NotFound(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.ServerError<object>(ex.Message);
                return StatusCode(500, response);
            }
        }

        // Update RateRange
        [HttpPut]
        [Route(Common.Url.User.RateRange.Update)]
        [ProducesResponseType(typeof(ApiResponse<RateRange>), 200)]
        public async Task<IActionResult> UpdateRateRange(int id, [FromBody] RateRange updatedRateRange)
        {
            try
            {
                var result = await _rateRangeService.UpdateRateRangeAsync(id, updatedRateRange);
                var response = ApiResponseFactory.Success(result);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                var response = ApiResponseFactory.NotFound<object>(ex.Message);
                return NotFound(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.ServerError<object>(ex.Message);
                return StatusCode(500, response);
            }
        }

        // Delete RateRange
        [HttpDelete]
        [Route(Common.Url.User.RateRange.Delete)]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> DeleteRateRange(int id)
        {
            try
            {
                await _rateRangeService.DeleteRateRangeAsync(id);
                var response = ApiResponseFactory.Success<object>(null);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                var response = ApiResponseFactory.NotFound<object>(ex.Message);
                return NotFound(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.ServerError<object>(ex.Message);
                return StatusCode(500, response);
            }
        }
    }
}
