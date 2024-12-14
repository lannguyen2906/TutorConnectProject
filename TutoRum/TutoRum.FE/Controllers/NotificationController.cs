using Microsoft.AspNetCore.Mvc;
using TutoRum.Services.IService;
using static TutoRum.Services.ViewModels.NotificationDto;
using TutoRum.Services.Service;
using TutoRum.FE.Common;
using TutoRum.Services.ViewModels;

namespace TutoRum.FE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly IFirebaseService _firebaseService;
        private readonly INotificationService _notificationService;
        private readonly IUserTokenService _userTokenService;

        public NotificationController(IFirebaseService firebaseService, INotificationService notificationService, IUserTokenService userTokenService)
        {
            _firebaseService = firebaseService;
            _notificationService = notificationService;
            _userTokenService = userTokenService;
        }
        [HttpGet]
        [Route(Common.Url.User.Notification.GetAllNotifications)]
        [ProducesResponseType(typeof(ApiResponse<NotificationDtos>), 200)]
        public async Task<IActionResult> GetAllNotifications([FromQuery] int pageIndex = 0, int pageSize = 10)
        {
            try
            {
                var notifications = await _notificationService.GetNotificationsByUserAsync(User, pageIndex, pageSize);
                var response = ApiResponseFactory.Success(notifications);
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
        [Route(Common.Url.User.Notification.SendNotification)]
        [ProducesResponseType(typeof(ApiResponse<NotificationRequestDto>), 200)]
        public async Task<IActionResult> SendNotification([FromBody] NotificationRequestDto request)
        {
            try
            {
                await _notificationService.SendNotificationAsync(request, false);
                var response = ApiResponseFactory.Success(request);
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
        [Route(Common.Url.User.Notification.SaveFCMToken)]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> SaveFCMToken([FromBody] UserTokenDto dto)
        {
            try
            {
                await _userTokenService.SaveTokenAsync(dto.UserId, dto.Token, dto.DeviceType);
                return Ok(ApiResponseFactory.Success("Save FCM Token success"));
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
        [Route(Common.Url.User.Notification.MarkNotificationsAsRead)]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> MarkNotificationsAsRead([FromBody] List<int> ids)
        {
            try
            {
                await _notificationService.MarkNotificationAsReadAsync(ids);
                return Ok(ApiResponseFactory.Success("Mark all notifications as read"));
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
