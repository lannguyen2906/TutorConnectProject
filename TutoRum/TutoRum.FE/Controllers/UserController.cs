using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TutoRum.FE.Common;
using TutoRum.Services.IService;
using TutoRum.Services.ViewModels;

namespace TutoRum.FE.Controllers
{
    
    public class UserController : ApiControllerBase
    {
        private readonly IUserService _userService;
        private readonly IScheduleService _scheduleService;

        public UserController(IUserService userService, IScheduleService scheduleService)
        {
            _userService = userService;
            _scheduleService = scheduleService;
        }

        [HttpPost]
        [Route(Common.Url.User.UpdateProfile)]
        [ProducesResponseType(typeof(ApiResponse<UpdateUserDTO>), 200)]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserDTO userDto)
        {
            try
            {
                await _userService.UpdateUserProfileAsync(userDto, User);

                var response = ApiResponseFactory.Success( userDto );
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

        

    }

  


}
