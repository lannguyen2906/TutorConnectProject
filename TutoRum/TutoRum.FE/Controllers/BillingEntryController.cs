using Microsoft.AspNetCore.Mvc;
using TutoRum.FE.Common;
using TutoRum.Services.IService;
using TutoRum.Services.ViewModels;

namespace TutoRum.FE.Controllers
{

    public class BillingEntryController : ApiControllerBase
    {
        private readonly IBillingEntryService _billingEntryService;

        public BillingEntryController(IBillingEntryService billingEntryService)
        {
            _billingEntryService = billingEntryService;
        }

        [HttpGet]
        [Route(Common.Url.User.BillingEntry.GetAllBillingEntries)]
        [ProducesResponseType(typeof(ApiResponse<BillingEntryDTOS>), 200)]
        public async Task<IActionResult> GetAllBillingEntries([FromQuery] int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var billingEntries = await _billingEntryService.GetAllBillingEntriesAsync(User, pageIndex, pageSize);
                var response = ApiResponseFactory.Success(billingEntries);
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

        [HttpGet]
        [Route(Common.Url.User.BillingEntry.GetBillingEntryById)]
        [ProducesResponseType(typeof(ApiResponse<BillingEntryDTO>), 200)]
        public async Task<IActionResult> GetBillingEntryById([FromQuery] int billingEntryId)
        {
            try
            {
                var billingEntry = await _billingEntryService.GetBillingEntryByIdAsync(billingEntryId, User);
                var response = ApiResponseFactory.Success(billingEntry);
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
        [Route(Common.Url.User.BillingEntry.GetAllBillingEntriesByTutorLearnerSubjectId)]
        [ProducesResponseType(typeof(ApiResponse<BillingEntryDTOS>), 200)]
        public async Task<IActionResult> GetBillingEntriesByTutorLearnerSubjectId([FromQuery] int tutorLearnerSubjectId, int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var billingEntries = await _billingEntryService.GetAllBillingEntriesByTutorLearnerSubjectIdAsync(tutorLearnerSubjectId, User, pageIndex, pageSize);
                var response = ApiResponseFactory.Success(billingEntries);
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
        [Route(Common.Url.User.BillingEntry.AddBillingEntry)]
        [ProducesResponseType(typeof(ApiResponse<object>), 201)]
        public async Task<IActionResult> AddBillingEntry([FromBody] AdddBillingEntryDTO billingEntryDTO)
        {
            try
            {
                await _billingEntryService.AddBillingEntryAsync(billingEntryDTO, User);
                var response = ApiResponseFactory.Success<object>(null);
                return StatusCode(201, response);
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
        [Route(Common.Url.User.BillingEntry.AddDraftBillingEntry)]
        [ProducesResponseType(typeof(ApiResponse<object>), 201)]
        public async Task<IActionResult> AddDraftBillingEntry([FromBody] AdddBillingEntryDTO billingEntryDTO)
        {
            try
            {
                await _billingEntryService.AddDraftBillingEntryAsync(billingEntryDTO, User);
                var response = ApiResponseFactory.Success<object>(null);
                return StatusCode(201, response);
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

        [HttpPut]
        [Route(Common.Url.User.BillingEntry.UpdateBillingEntry)]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> UpdateBillingEntry([FromQuery] int billingEntryId, [FromBody] UpdateBillingEntryDTO billingEntryDTO)
        {
            try
            {
                await _billingEntryService.UpdateBillingEntryAsync(billingEntryId, billingEntryDTO, User);
                var response = ApiResponseFactory.Success<object>(null);
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

        [HttpDelete]
        [Route(Common.Url.User.BillingEntry.DeleteBillingEntry)]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> DeleteBillingEntries([FromBody] List<int> billingEntryIds)
        {
            try
            {
                await _billingEntryService.DeleteBillingEntriesAsync(billingEntryIds, User);
                var response = ApiResponseFactory.Success<object>(null);
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

        [HttpGet]
        [Route(Common.Url.User.BillingEntry.GetBillingEntryDetails)]
        [ProducesResponseType(typeof(ApiResponse<BillingEntryDetailsDTO>), 200)]
        public async Task<IActionResult> GetBillingEntryDetails([FromQuery] int tutorLearnerSubjectId)
        {
            try
            {
                var billingEntryDetails = await _billingEntryService.GetBillingEntryDetailsAsync(tutorLearnerSubjectId);
                var response = ApiResponseFactory.Success(billingEntryDetails);
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


        [HttpPost]
        [Route(Common.Url.User.BillingEntry.CalculateTotalAmount)]
        [ProducesResponseType(typeof(ApiResponse<decimal>), 200)]
        public IActionResult CalculateTotalAmount([FromBody] CalculateTotalAmountRequest request)
        {
            try
            {
                var totalAmount = _billingEntryService.CalculateTotalAmount(request.StartDateTime, request.EndDateTime, request.Rate);
                var response = ApiResponseFactory.Success(totalAmount);
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

    }

}
