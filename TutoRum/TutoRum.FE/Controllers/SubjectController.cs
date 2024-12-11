using Microsoft.AspNetCore.Mvc;
using TutoRum.Data.Models;
using TutoRum.FE.Common;
using TutoRum.Services.IService;
using TutoRum.Services.ViewModels;

namespace TutoRum.FE.Controllers
{
    public class SubjectController : ApiControllerBase
    {

        private readonly ISubjectService _subjectService;

        public SubjectController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }


        [HttpGet]
        [Route(Common.Url.User.Subject.GetAllSubjects)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<SubjectFilterDTO>>), 200)]
        public async Task<IActionResult> GetAllSubjectAsync()
        {
            try
            {
                var Subjects = await _subjectService.GetAllSubjectAsync();
                var apiResponse = ApiResponseFactory.Success(Subjects);
                return Ok(apiResponse);
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
        [Route(Common.Url.User.Subject.CreateSubjectForSuperAdmin)]
        [ProducesResponseType(typeof(ApiResponse<Subject>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> CreateSubjectAsync([FromBody] SubjectDTO subjectDto)
        {
            try
            {
                var createdSubject = await _subjectService.CreateSubjectAsync(subjectDto, User);
                var apiResponse = ApiResponseFactory.Success(createdSubject);
                return Ok(apiResponse);
            }
            catch (ArgumentException ex)
            {
                var response = ApiResponseFactory.BadRequest<object>(ex.Message);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }

        [HttpGet]
        [Route("get-top-subject/{size}")]
        [ProducesResponseType(typeof(ApiResponse<List<SubjectFilterDTO>>), 200)]
        public async Task<IActionResult> GetTopSubject(int size)
        {
            try
            {
                var Subjects = await _subjectService.GetTopSubjectsAsync(size);
                var apiResponse = ApiResponseFactory.Success(Subjects);
                return Ok(apiResponse);
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
            catch (ArgumentException ex)
            {
                var response = ApiResponseFactory.BadRequest<object>(ex.Message);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }


        [HttpGet]
        [Route(Common.Url.User.Subject.GetSubjectByIdForSuperAdmin)]
        [ProducesResponseType(typeof(ApiResponse<Subject>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> GetSubjectByIdAsync([FromRoute] int subjectId)
        {
            try
            {
                var subject = await _subjectService.GetSubjectByIdAsync(subjectId);
                var apiResponse = ApiResponseFactory.Success(subject);
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.NotFound<object>(ex.Message);
                return NotFound(response);
            }
        }

        [HttpPut]
        [Route(Common.Url.User.Subject.UpdateSubjectForSuperAdmin + "{subjectId}")]
        [ProducesResponseType(typeof(ApiResponse<Subject>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> UpdateSubjectAsync([FromRoute] int subjectId, [FromBody] SubjectDTO subjectDto)
        {
            try
            {

                await _subjectService.UpdateSubjectAsync(subjectId, subjectDto, User);
                var apiResponse = ApiResponseFactory.Success("Subject updated successfully.");
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.NotFound<object>(ex.Message);
                return NotFound(response);
            }
        }

        [HttpDelete]
        [Route(Common.Url.User.Subject.DeleteSubjectForSuperAdmin + "/{subjectId}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> DeleteSubjectAsync([FromRoute] int subjectId)
        {
            try
            {

                await _subjectService.DeleteSubjectAsync(subjectId, User);
                var apiResponse = ApiResponseFactory.Success("Subject deleted successfully.");
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.NotFound<object>(ex.Message);
                return NotFound(response);
            }
        }

    }
}
