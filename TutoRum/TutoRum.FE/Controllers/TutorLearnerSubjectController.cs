using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using TutoRum.Data.Models;
using TutoRum.FE.Common;
using TutoRum.Services.IService;
using TutoRum.Services.Service;
using TutoRum.Services.ViewModels;

namespace TutoRum.FE.Controllers
{
    public class TutorLearnerSubjectController : ApiControllerBase
    {
        private readonly ITutorLearnerSubjectService _tutorLearnerSubjectService;
        private readonly IContractService _contractService;


        public TutorLearnerSubjectController(ITutorLearnerSubjectService tutorLearnerSubjectService, IContractService contractService)
        {
            _tutorLearnerSubjectService = tutorLearnerSubjectService;
            _contractService = contractService;
        }


        [HttpPost]
        [Route("register-learner")]
        [ProducesResponseType(typeof(ApiResponse<RegisterLearnerDTO>), 200)]
        public async Task<IActionResult> RegisterLearnerForTutor([FromBody] RegisterLearnerDTO learnerDto, Guid tutorId)
        {
            try
            {
                await _tutorLearnerSubjectService.RegisterLearnerForTutorAsync(learnerDto, User);
                var response = ApiResponseFactory.Success(learnerDto);
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
        [Route("download-contract/{tutorLearnerSubjectID}")]
        public async Task<IActionResult> DownloadContract(int tutorLearnerSubjectID)
        {
            try
            {
                // Gọi phương thức GenerateContract và nhận đường dẫn file đầu ra
                var filePath = await _contractService.GenerateContractAsync(tutorLearnerSubjectID);

                // Kiểm tra xem file có tồn tại hay không
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(new { message = "File not found." });
                }

                // Đọc file thành byte array để trả về
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                var fileName = Path.GetFileName(filePath);

                // Trả về file với định dạng dạng tải xuống
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while generating the contract.", detail = ex.Message });
            }
        }

        [HttpGet]
        [Route("get-subject-details")]
        [ProducesResponseType(typeof(ApiResponse<List<SubjectDetailDto>>), 200)]
        public async Task<IActionResult> GetSubjectDetailsByUserId(Guid userId, string viewType)
        {
            try
            {
                var subjectDetails = await _tutorLearnerSubjectService.GetSubjectDetailsByUserIdAsync(userId, viewType);
                var response = ApiResponseFactory.Success(subjectDetails);
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
        [Route("get-tutor-learner-subject-detail")]
        [ProducesResponseType(typeof(ApiResponse<TutorLearnerSubjectSummaryDetailDto>), 200)]
        public async Task<IActionResult> GetTutorLearnerSubjectDetailById(int id)
        {
            try
            {
                var subjectDetail = await _tutorLearnerSubjectService.GetTutorLearnerSubjectSummaryDetailByIdAsync(id, User);
                var response = ApiResponseFactory.Success(subjectDetail);
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
        [Route("get-classrooms")]
        [ProducesResponseType(typeof(ApiResponse<List<SubjectDetailDto>>), 200)]
        public async Task<IActionResult> GetClassroomsByUserIdAsync(Guid userId, string viewType)
        {
            try
            {
                var subjectDetails = await _tutorLearnerSubjectService.GetClassroomsByUserIdAsync(userId, viewType);
                var response = ApiResponseFactory.Success(subjectDetails);
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

        [HttpPut]
        [Route("update-classroom/{tutorLearnerSubjectID}")]
        [ProducesResponseType(typeof(ApiResponse<RegisterLearnerDTO>), 200)]
        public async Task<IActionResult> UpdateClassroom(int tutorLearnerSubjectID, [FromBody] RegisterLearnerDTO learnerDto)
        {
            try
            {
                await _tutorLearnerSubjectService.UpdateClassroom(tutorLearnerSubjectID, learnerDto, User);
                var response = ApiResponseFactory.Success(learnerDto);
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

        [HttpPost("handle-contract-upload")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> HandleContractUploadAsync([FromBody] HandleContractUploadDTO contractDto)
        {
            try
            {

                // Call the service method to handle contract upload and notifications
                await _tutorLearnerSubjectService.HandleContractUploadAndNotifyAsync(contractDto.TutorId, contractDto.TutorLearnerSubjectId, contractDto.ContractUrl);
                // Return success response
                var response = ApiResponseFactory.Success("Contract uploaded and notifications sent successfully.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Handle generic exceptions
                var response = ApiResponseFactory.ServerError<string>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }

        [HttpPost]
        [Route("api/tutorlearner/verifycontract")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> VerifyContract([FromQuery] int tutorLearnerSubjectId, bool isVerified, string? reason)
        {
            try
            {
                var user = User; // Người dùng hiện tại
                var result = await _tutorLearnerSubjectService.VerifyTutorLearnerContractAsync(tutorLearnerSubjectId, user, isVerified, reason);

                var response = ApiResponseFactory.Success(result);
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
        [Route("api/contracts")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<ContractDto>>), 200)]
        public async Task<IActionResult> GetContracts(
            [FromQuery] ContractFilterDto filter,
            [FromQuery] int pageIndex = 0,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                // Lấy danh sách hợp đồng
                var pagedContracts = await _tutorLearnerSubjectService.GetListContractAsync(filter, pageIndex, pageSize);

                // Trả về phản hồi thành công
                var response = ApiResponseFactory.Success(pagedContracts);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }


        [HttpPost("createClassFromTutorRequest/{tutorRequestId}")]
        public async Task<IActionResult> CreateClassForLearnerAsync([FromBody] CreateClassDTO classDto, [FromRoute] int tutorRequestId)
        {
            if (classDto == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                // Gọi phương thức tạo lớp học
                var result = await _tutorLearnerSubjectService.CreateClassForLearnerAsync(classDto, tutorRequestId, User);

                if (result)
                {
                    return Ok(new { message = "Class created successfully." });
                }

                return BadRequest("Failed to create class.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message); // Trả về 403 Forbidden nếu không có quyền
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}"); // Trả về lỗi server
            }
        }

        [HttpPut("closeClass/{tutorLearnerSubjectId}")]
        public async Task<IActionResult> CloseClass([FromRoute] int tutorLearnerSubjectId)
        {
            try
            {
                // Gọi phương thức tạo lớp học
                var result = await _tutorLearnerSubjectService.CloseClassAsync(tutorLearnerSubjectId, User);

                if (result)
                {
                    return Ok(new { message = "Class is closed successfully." });
                }

                return BadRequest("Failed to close class.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message); // Trả về 403 Forbidden nếu không có quyền
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}"); // Trả về lỗi server
            }
        }

    }
}
