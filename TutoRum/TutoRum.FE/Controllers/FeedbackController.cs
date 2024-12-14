using Microsoft.AspNetCore.Mvc;
using TutoRum.FE.Common;
using TutoRum.Services.IService;
using TutoRum.Services.ViewModels;

namespace TutoRum.FE.Controllers
{
    public class FeedbackController : ApiControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [HttpGet]
        [Route("GetFeedbackDetailByTutorId/{tutorId}")]
        [ProducesResponseType(typeof(ApiResponse<FeedbackDetail>), 200)]
        public async Task<IActionResult> GetFeedbackDetailByTutorId(Guid tutorId, [FromQuery] bool showAll = false)
        {
            try
            {
                var feedbacks = await _feedbackService.GetFeedbackDetailByTutorIdAsync(tutorId, showAll);
                return Ok(new ApiResponse<FeedbackDetail>(200, true, "Feedbacks retrieved successfully", feedbacks));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(500, false, ex.Message, null));
            }
        }


        [HttpGet]
        [Route("GetByTutorLearnerSubjectId/{tutorLearnerSubjectId}")]
        [ProducesResponseType(typeof(ApiResponse<FeedbackDto>), 200)]
        public async Task<IActionResult> GetByFeedbackId(int tutorLearnerSubjectId)
        {
            try
            {
                var feedback = await _feedbackService.GetFeedbackByTutorLearnerSubjectIdAsync(tutorLearnerSubjectId);
                return Ok(new ApiResponse<FeedbackDto>(200, true, "Feedback retrieved successfully", feedback));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(500, false, ex.Message, null));
            }
        }

        [HttpPost]
        [Route(Common.Url.User.Feedback.CreateFeedback)]
        [ProducesResponseType(typeof(ApiResponse<FeedbackDto>), 200)]
        public async Task<IActionResult> CreateFeedback([FromBody] CreateFeedbackDto createFeedbackDto)
        {
            try
            {
                var createdFeedback = await _feedbackService.CreateFeedbackAsync(createFeedbackDto);

                var response = ApiResponseFactory.Success(createdFeedback);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }

        [HttpPut]
        [Route(Common.Url.User.Feedback.UpdateFeedback)]
        [ProducesResponseType(typeof(ApiResponse<FeedbackDto>), 200)]
        public async Task<IActionResult> UpdateFeedback([FromBody] FeedbackDto feedbackDto)
        {
            try
            {
                var updatedFeedback = await _feedbackService.UpdateFeedbackAsync(feedbackDto);

                var response = ApiResponseFactory.Success(updatedFeedback);
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
        [Route(Common.Url.User.Feedback.GetStatistics)]
        [ProducesResponseType(typeof(ApiResponse<FeedbackStatisticsResponse>), 200)]
        public async Task<IActionResult> GetFeedbackStatistics(Guid tutorId)
        {
            try
            {
                // Gọi service để lấy thống kê feedback
                var (statistics, comments) = await _feedbackService.GetFeedbackStatisticsForTutorAsync(tutorId);

                // Đóng gói dữ liệu trả về
                var response = new FeedbackStatisticsResponse
                {
                    Statistics = statistics,
                    Comments = comments
                };

                return Ok(ApiResponseFactory.Success(response));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponseFactory.NotFound<object>(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseFactory.ServerError<object>(detail: ex.Message));
            }
        }


    }
}
