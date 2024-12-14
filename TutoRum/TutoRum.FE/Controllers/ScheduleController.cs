using Microsoft.AspNetCore.Mvc;
using TutoRum.Data.Models;
using TutoRum.FE.Common;
using TutoRum.Services.IService;
using TutoRum.Services.ViewModels;

namespace TutoRum.FE.Controllers
{
    public class ScheduleController : ApiControllerBase
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        
        [HttpGet("{tutorId}")]
        [ProducesResponseType(typeof(ApiResponse<List<ScheduleGroupDTO>>), 200)]
        public async Task<IActionResult> GetSchedulesByTutorId(Guid tutorId)
        {
            try
            {
                // Gọi service để lấy lịch theo tutorId
                var schedules = await _scheduleService.GetSchedulesByTutorIdAsync(tutorId);
                var response = ApiResponseFactory.Success(schedules);
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

        // API endpoint for adding a new schedule
        [HttpPost("add/{tutorId}")]
        [ProducesResponseType(typeof(ApiResponse<Schedule>), 200)]
        public async Task<IActionResult> AddScheduleAsync(Guid tutorId, [FromBody] AddScheduleDTO newSchedule)
        {
            try
            {
                await _scheduleService.AddSchedule(tutorId, newSchedule);

                // Return success response
                var response = ApiResponseFactory.Success("Schedule added successfully.");
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                // Handle unauthorized access exception
                var response = ApiResponseFactory.Unauthorized<object>(ex.Message);
                return StatusCode(403, response);
            }
            catch (Exception ex)
            {
                // Handle generic exceptions
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }

        // API endpoint for deleting a schedule
        [HttpDelete("delete/{tutorId}")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> DeleteScheduleAsync(Guid tutorId, [FromBody] DeleteScheduleDTO scheduleToDelete)
        {
            try
            {
                await _scheduleService.DeleteSchedule(tutorId, scheduleToDelete);

                // Return success response
                var response = ApiResponseFactory.Success("Schedule deleted successfully.");
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

        // API endpoint for updating a tutor's schedule
        [HttpPut("update/{tutorId}")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> UpdateScheduleAsync(Guid tutorId, [FromBody] UpdateScheduleDTO updatedSchedule)
        {
            try
            {
                await _scheduleService.UpdateSchedule(tutorId, updatedSchedule);

                // Return success response
                var response = ApiResponseFactory.Success("Schedule updated successfully.");
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
