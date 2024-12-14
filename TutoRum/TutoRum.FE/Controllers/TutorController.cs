using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TutoRum.FE.Common;
using TutoRum.Services.IService;
using TutoRum.Services.Service;
using TutoRum.Services.ViewModels;

namespace TutoRum.FE.Controllers
{
    public class TutorController : ApiControllerBase
    {
        private readonly ITutorService _tutorService;
        private readonly ITeachingLocationsService _teachingLocationsService;
        private readonly ICertificatesSevice _certificatesSevice;
        private readonly ISubjectService _subjectService;
        public TutorController(ITutorService tutorService, ITeachingLocationsService teachingLocationsService, ICertificatesSevice certificatesSevice, ISubjectService subjectService)
        {
            _tutorService = tutorService;
            _teachingLocationsService = teachingLocationsService;
            _certificatesSevice = certificatesSevice;
            _subjectService = subjectService;
        }

        [HttpPost]
        [Route(Common.Url.User.Tutor.RegisterTutor)]
        [ProducesResponseType(typeof(ApiResponse<AddTutorDTO>), 200)]
        public async Task<IActionResult> RegisterTutor([FromBody] AddTutorDTO Dto)
        {
            try
            {
                await _tutorService.RegisterTutorAsync(Dto, User);

                var response = ApiResponseFactory.Success( Dto );
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
        [Route(Common.Url.User.Tutor.MajorsWithMinor)]
        [ProducesResponseType(typeof(ApiResponse<List<MajorMinorDto>>), 200)]
        public async Task<IActionResult> GetMajorsWithMinors()
        {
            try
            {
                var result = await _tutorService.GetAllTutorsWithMajorsAndMinorsAsync();
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
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }
    

   

        [HttpGet]
        [Route(Common.Url.User.Tutor.GetTutorById + "/{id}")]
        [ProducesResponseType(typeof(ApiResponse<TutorDto>), 200)]
        public async Task<IActionResult> GetTutorById(Guid id)
        {
            try
            {
                var tutorDto = await _tutorService.GetTutorByIdAsync(id);

                var response = ApiResponseFactory.Success(tutorDto);
                return Ok(response);
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
        [Route(Common.Url.User.Tutor.TutorHomePage)]
        [ProducesResponseType(typeof(ApiResponse<TutorHomePageDTO>), 200)]
        public async Task<IActionResult> GetTutorsHomePage([FromBody] TutorFilterDto tutorFilterDto, int index = 0, int size = 20 )
        {
            try
            {
                var tutorsHomePage = await _tutorService.GetTutorHomePage(tutorFilterDto, index, size );
                var response = ApiResponseFactory.Success(tutorsHomePage);
                return Ok(response);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponseFactory.NotFound<object>("No tutor requests found."));
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }

        [HttpDelete]
        [Route(Common.Url.User.Tutor.DeleteTutor + "/{tutorId}")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> DeleteTutor(Guid tutorId)
        {
            try
            {
                await _tutorService.DeleteTutorAsync(tutorId, User);
                var response = ApiResponseFactory.Success("Tutor deleted successfully.");
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
        [Route("all-with-feedback")]
        [ProducesResponseType(typeof(ApiResponse<List<TutorRatingDto>>), 200)]
        public async Task<IActionResult> GetAllTutorsWithFeedback()
        {
            try
            {
                var tutorRatingDtos = await _tutorService.GetAllTutorsWithFeedbackAsync();

                var response = ApiResponseFactory.Success(tutorRatingDtos);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }

        [HttpGet]
        [Route("get-top-tutor/{size}")]
        [ProducesResponseType(typeof(ApiResponse<List<TutorSummaryDto>>), 200)]
        public async Task<IActionResult> GetTopTutor(int size)
        {
            try
            {
                var tutorRatingDtos = await _tutorService.GetTopTutorAsync(size);

                var response = ApiResponseFactory.Success(tutorRatingDtos);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }

        [HttpGet]
        [Route("my-wallet-overview")]
        [ProducesResponseType(typeof(ApiResponse<WalletOverviewDto>), 200)]
        public async Task<IActionResult> GetWalletOverviewByTutor()
        {
            try
            {
                var walletOverview = await _tutorService.GetWalletOverviewDtoAsync(User);

                var response = ApiResponseFactory.Success(walletOverview);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }

        [HttpPut("update-tutor-info")]
        [ProducesResponseType(typeof(ApiResponse<UpdateTutorInforDTO>), 200)]
        public async Task<IActionResult> UpdateTutorInfoAsync([FromBody] UpdateTutorInforDTO tutorDto)
        {
            try
            {
                // Call service to update tutor info
                await _tutorService.UpdateTutorInfoAsync(tutorDto, User);

                // Return success response
                var response = ApiResponseFactory.Success("Tutor information updated successfully.");
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                // Handle unauthorized access exception
                var response = ApiResponseFactory.Unauthorized<object>(ex.Message);
                return StatusCode(403, response);
            }
            catch (KeyNotFoundException ex)
            {
                // Handle not found exception
                var response = ApiResponseFactory.NotFound<object>(ex.Message);
                return NotFound(response);
            }
            catch (Exception ex)
            {
                // Handle generic exceptions
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }

        [HttpDelete("delete-teaching-location")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> DeleteTeachingLocationAsync(Guid tutorId, int[] locationIds)
        {
            try
            {
                await _teachingLocationsService.DeleteTeachingLocationAsync(tutorId, locationIds);
                var response = ApiResponseFactory.Success("Teaching location deleted successfully.");
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                // Handle unauthorized access exception
                var response = ApiResponseFactory.Unauthorized<object>(ex.Message);
                return StatusCode(403, response);
            }
            catch (KeyNotFoundException ex)
            {
                // Handle not found exception
                var response = ApiResponseFactory.NotFound<object>(ex.Message);
                return NotFound(response);
            }
            catch (Exception ex)
            {
                // Handle generic exceptions
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }

        [HttpDelete("DeleteCertificateAsync/{certiID}")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> DeleteCertificateAsync(Guid tutorId, int certiID)
        {
            try
            {
                await _certificatesSevice.DeleteCertificatesAsync(certiID, tutorId);
                var response = ApiResponseFactory.Success("Certificate deleted successfully.");
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                // Handle unauthorized access exception
                var response = ApiResponseFactory.Unauthorized<object>(ex.Message);
                return StatusCode(403, response);
            }
            catch (KeyNotFoundException ex)
            {
                // Handle not found exception
                var response = ApiResponseFactory.NotFound<object>(ex.Message);
                return NotFound(response);
            }
            catch (Exception ex)
            {
                // Handle generic exceptions
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }

        [HttpDelete("DeleteTutorSubjectAsync/{tutorSubjectID}")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> DeleteTutorSubjectAsync(Guid tutorId, int tutorSubjectID)
        {
            try
            {
                await _subjectService.DeleteTutorSubjectAsync(tutorId, tutorSubjectID);
                var response = ApiResponseFactory.Success("Tutor subject deleted successfully.");
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                // Handle unauthorized access exception
                var response = ApiResponseFactory.Unauthorized<object>(ex.Message);
                return StatusCode(403, response);
            }
            catch (KeyNotFoundException ex)
            {
                // Handle not found exception
                var response = ApiResponseFactory.NotFound<object>(ex.Message);
                return NotFound(response);
            }
            catch (Exception ex)
            {
                // Handle generic exceptions
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }



    }


}

