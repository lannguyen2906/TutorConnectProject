using Microsoft.AspNetCore.Mvc;
using TutoRum.Data.Models;
using TutoRum.FE.Common;
using TutoRum.Services.IService;
using TutoRum.Services.Service;
using TutoRum.Services.ViewModels;

namespace TutoRum.FE.Controllers
{
    public class AdminController : ApiControllerBase
    {
        private readonly IAdminSevice _adminSevice;

        public AdminController(IAdminSevice adminSevice)
        {
            _adminSevice = adminSevice;
        }



        [HttpPost]
        [Route(Common.Url.User.Admin.AssignRoleAdmin)]
        [ProducesResponseType(typeof(ApiResponse<AssignRoleAdminDto>), 200)]
        public async Task<IActionResult> AssignRoleAdmin([FromBody] AssignRoleAdminDto dto)
        {
            try
            {
                await _adminSevice.AssignRoleAdmin(dto, User);

                var response = ApiResponseFactory.Success(dto);
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
        [Route(Common.Url.User.Admin.GetAdminListByTutor)]
        [ProducesResponseType(typeof(ApiResponse<List<AdminHomePageDTO>>), 200)]
        public async Task<IActionResult> GetAdminListByTutorHomePage([FromQuery] FilterDto filterDto,int index = 0, int size = 20)
        {
            try
            {
                var tutorsHomePage = await _adminSevice.GetAllTutors(User, filterDto, index, size);
                var response = ApiResponseFactory.Success(tutorsHomePage);
                return Ok(response);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponseFactory.NotFound<object>("No tutor requests found."));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid("You do not have permission to access this resource.");
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }

        [HttpGet]
        [Route(Common.Url.User.Admin.GetAdminMenuAction)]
        [ProducesResponseType(typeof(ApiResponse<List<AdminMenuAction>>), 200)]
        public async Task<IActionResult> GetAdminMenuAction()
        {
            try
            {
                var tutorsHomePage = await _adminSevice.GetAdminMenuActionAsync(User);
                var response = ApiResponseFactory.Success(tutorsHomePage);
                return Ok(response);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponseFactory.NotFound<object>("No tutor requests found."));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid("You do not have permission to access this resource.");
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }

        [HttpPost]
        [Route(Common.Url.User.Admin.VerificationStatus)]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> SetVerificationStatusAsync([FromBody] VerificationStatusDto dto)
        {
            if (dto == null)
            {
                var response = ApiResponseFactory.BadRequest<object>("Dữ liệu không hợp lệ.");
                return BadRequest(response);
            }

            try
            {
                await _adminSevice.SetVerificationStatusAsync(dto.EntityType, dto.GuidId, dto.Id, dto.IsVerified, dto.Reason, User);
                var response = ApiResponseFactory.Success("Xác thực thành công");
                return Ok(response);
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
                var response = ApiResponseFactory.ServerError<object>("Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.");
                return StatusCode(500, response);
            }
        }

    }
}
