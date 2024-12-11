using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TutoRum.FE.Common;
using TutoRum.Services.IService;
using TutoRum.Services.Service;
using TutoRum.Services.ViewModels;

namespace TutoRum.FE.Controllers
{
    public class TutorRequestController : ApiControllerBase
    {
        private readonly ITutorRequestService _tutorRequestService;

        public TutorRequestController(ITutorRequestService tutorRequestService)
        {
            _tutorRequestService = tutorRequestService;
        }

        [HttpGet]
        [Route(Common.Url.User.TutorRequest.GetAllTutorRequests)]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<TutorRequestDTO>>), 200)]
        public async Task<IActionResult> GetAllTutorRequests([FromQuery] TutorRequestHomepageFilterDto filter,int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var tutorRequests = await _tutorRequestService.GetAllTutorRequestsAsync(filter, pageIndex, pageSize);
                var response = ApiResponseFactory.Success(tutorRequests);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }

        [HttpGet]
        [Route(Common.Url.User.TutorRequest.GetTutorRequestById + "/{tutorRequestId}")]
        [ProducesResponseType(typeof(ApiResponse<TutorRequestDTO>), 200)]
        public async Task<IActionResult> GetTutorRequestById(int tutorRequestId)
        {
            try
            {
                var tutorRequest = await _tutorRequestService.GetTutorRequestByIdAsync(tutorRequestId);

                if (tutorRequest == null)
                {
                    return NotFound();
                }

                var response = ApiResponseFactory.Success(tutorRequest);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }

        [HttpPost]
        [Route(Common.Url.User.TutorRequest.CreateTutorRequest)]
        [ProducesResponseType(typeof(ApiResponse<TutorRequestDTO>), 201)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> CreateTutorRequestAsync([FromBody] TutorRequestDTO tutorRequestDto)
        {
            if (tutorRequestDto == null)
            {
                return BadRequest(ApiResponseFactory.BadRequest<TutorRequestDTO>("Invalid request data."));
            }

            try
            {
                // Gọi service để tạo yêu cầu gia sư
                var tutorRequestId = await _tutorRequestService.CreateTutorRequestAsync(tutorRequestDto, User);

                var response = ApiResponseFactory.Success(tutorRequestId);
                return CreatedAtAction(nameof(GetTutorRequestById), new { tutorRequestId }, response);
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }


        [HttpPut]
        [Route(Common.Url.User.TutorRequest.UpdateTutorRequest + "/{tutorRequestId}")]
        public async Task<IActionResult> UpdateTutorRequestAsync(int tutorRequestId, [FromBody] TutorRequestDTO tutorRequestDto)
        {
            if (tutorRequestDto == null)
            {
                return BadRequest(ApiResponseFactory.BadRequest<TutorRequestDTO>("Invalid request data."));
            }

            try
            {
                var success = await _tutorRequestService.UpdateTutorRequestAsync(tutorRequestId, tutorRequestDto, User);
                if (success)
                {
                    return Ok(ApiResponseFactory.Success<object>(null, "Tutor request updated successfully."));
                }
                else
                {
                    return NotFound(ApiResponseFactory.NotFound<object>("Tutor request not found."));
                }
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }

        [HttpPut]
        [Route(Common.Url.User.TutorRequest.ChooseTutorForTutorRequestAsync + "/{tutorRequestId}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> ChooseTutorForTutorRequestAsync(int tutorRequestId, Guid tutorID)
        {
            try
            {
                var success = await _tutorRequestService.ChooseTutorForTutorRequestAsync(tutorRequestId, tutorID, User);
                if (success)
                {
                    return Ok(ApiResponseFactory.Success<object>(null, "Tutor selected successfully."));
                }
                else
                {
                    return NotFound(ApiResponseFactory.NotFound<object>("Tutor request not found."));
                }
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }

        [HttpPost]
        [Route(Common.Url.User.TutorRequest.AddTutorToRequest)]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> AddTutorToRequestAsync(int tutorRequestId, Guid tutorId)
        {
            try
            {
                var success = await _tutorRequestService.AddTutorToRequestAsync(tutorRequestId, tutorId);
                if (success)
                {
                    return Ok(ApiResponseFactory.Success(true, "Tutor successfully added to the request."));
                }
                else
                {
                    return NotFound(ApiResponseFactory.NotFound<object>("Tutor or request not found."));
                }
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }

        [HttpGet]
        [Route(Common.Url.User.TutorRequest.GetListTutorsByTutorRequest + "/{tutorRequestId}")]
        [ProducesResponseType(typeof(ApiResponse<TutorRequestWithTutorsDTO>), 200)]
        public async Task<IActionResult> GetListTutorsByTutorRequestAsync(int tutorRequestId)
        {
            try
            {
                var result = await _tutorRequestService.GetListTutorsByTutorRequestAsync(tutorRequestId);
                var response = ApiResponseFactory.Success(result);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }



        [HttpGet(Common.Url.User.TutorRequest.GetListTutorRequestsByLeanrerID +  "/{learnerId}")]
        public async Task<ActionResult<PagedResult<ListTutorRequestDTO>>> GetTutorRequestsByLearnerId(
        Guid learnerId,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 20)
        {
            try
            {
                var result = await _tutorRequestService.GetTutorRequestsByLearnerIdAsync(learnerId, pageIndex, pageSize);

                // Trả về kết quả với mã trạng thái OK (200)
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có
                return StatusCode(500, new { message = ex.Message });
            }
        }


        [HttpGet(Common.Url.User.TutorRequest.GetListTutorRequestsByTutorID + "/{tutorId}")]
        public async Task<ActionResult<PagedResult<ListTutorRequestForTutorDto>>> GetListTutorRequestsByTutorID(
        Guid tutorId,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 20)
        {
            try
            {
                var result = await _tutorRequestService.GetTutorRequestsByTutorIdAsync(tutorId, pageIndex, pageSize);

                // Trả về kết quả với mã trạng thái OK (200)
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet(Common.Url.User.TutorRequest.GetTutorRequestsAdmin)]
        public async Task<ActionResult<PagedResult<ListTutorRequestDTO>>> GetTutorRequestsAdmin(
            [FromQuery] TutorRequestFilterDto filter,
            [FromQuery] int pageIndex = 0,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var result = await _tutorRequestService.GetTutorRequestsAdmin(filter, User, pageIndex, pageSize);

                // Trả về kết quả với mã trạng thái OK (200)
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("send-tutor-request-email/{tutorRequestId}")]
        public async Task<IActionResult> SendTutorRequestEmailAsync(int tutorRequestId, Guid tutorID)
        {
            try
            {
                // Gọi phương thức dịch vụ để gửi email yêu cầu gia sư
                await _tutorRequestService.SendTutorRequestEmailAsync(tutorRequestId, tutorID);

                return Ok(new { message = "Email has been sent successfully." });
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet(Common.Url.User.TutorRequest.GetTutorLearnerSubjectInfoByTutorRequestId + "/{tutorRequestId}")]
        [ProducesResponseType(typeof(ApiResponse<TutorLearnerSubjectDetailDto>), 200)]
        public async Task<ActionResult<TutorLearnerSubjectDetailDto>> GetTutorLearnerSubjectInfoByTutorRequestId(
       int tutorRequestId)
        {
            try
            {
                var result = await _tutorRequestService.GetTutorLearnerSubjectInfoByTutorRequestId(User, tutorRequestId);
                var response = ApiResponseFactory.Success(result);

                // Trả về kết quả với mã trạng thái OK (200)
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route(Common.Url.User.TutorRequest.CloseTutorRequest + "/{tutorRequestId}")]

        public async Task<IActionResult> CloseTutorRequest(int tutorRequestId)
        {
            try
            {
                var success = await _tutorRequestService.CloseTutorRequestAsync(tutorRequestId, User);
                if (success)
                {
                    return Ok(ApiResponseFactory.Success<object>(null, "Tutor request close successfully."));
                }
                else
                {
                    return NotFound(ApiResponseFactory.NotFound<object>("Tutor request not found."));
                }
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }

    }
}

