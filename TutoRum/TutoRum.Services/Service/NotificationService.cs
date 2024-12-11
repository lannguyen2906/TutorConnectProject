using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Enum;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.Models;
using TutoRum.Data.Repositories;
using TutoRum.Services.IService;
using TutoRum.Services.ViewModels;
using Xceed.Document.NET;

namespace TutoRum.Services.Service
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFirebaseService _firebaseService;
        private readonly IUserTokenService _userTokenService;
        private readonly UserManager<AspNetUser> _userManager;

        public NotificationService(IUnitOfWork unitOfWork, IFirebaseService firebaseService, IUserTokenService userTokenService, UserManager<AspNetUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _firebaseService = firebaseService;
            _userTokenService = userTokenService;
            _userManager = userManager;
        }

        public async Task<NotificationDtos> GetNotificationsByUserAsync(ClaimsPrincipal user, int pageIndex = 0, int pageSize = 20)
        {
            // Lấy thông tin người dùng hiện tại
            var currentUser = await _userManager.GetUserAsync(user);

            // Kiểm tra nếu người dùng không tồn tại
            if (currentUser == null)
            {
                throw new Exception("User not found.");
            }

            var totalUnreadNotifications = _unitOfWork.Notifications.Count(n => n.IsRead == false && n.UserId == currentUser.Id);

            int totalRecords;
            var notifications = _unitOfWork.Notifications.GetMultiPaging(
                filter: n => n.UserId == currentUser.Id,
                out totalRecords,
                index: pageIndex,
                size: pageSize,
                orderBy: q => q.OrderByDescending(n => n.CreatedDate));

            if (notifications == null || !notifications.Any())
            {
                return new NotificationDtos
                {
                    TotalUnreadNotifications = 0,
                    Notifications = new List<NotificationDto>(),
                    TotalRecords = totalRecords
                };
            }

            return new NotificationDtos
            {
                Notifications = notifications.Select(n => new NotificationDto
                {
                    Color = n.Color,
                    CreatedDate = n.CreatedDate,
                    Description = n.Description,
                    Href = n.Href,
                    Icon = n.Icon,
                    IsRead = n.IsRead,
                    NotificationId = n.NotificationId,
                    Title = n.Title,
                    UserId = n.UserId   
                }).ToList(),
                TotalRecords = totalRecords,
                TotalUnreadNotifications = totalUnreadNotifications
            };
        }

        public async Task MarkNotificationAsReadAsync(List<int> ids)
        {
            await _unitOfWork.Notifications.MarkNotificationsAsReadAsync(ids);
        }


        public async Task SendNotificationAsync(NotificationRequestDto notificationRequestDto, bool? sendToAdmins = false)
        {
            var userIds = new List<Guid> { notificationRequestDto.UserId };

            if (sendToAdmins == true)
            {
                var users = await _userManager.GetUsersInRoleAsync(AccountRoles.Admin);
                userIds = users.Select(u => u.Id).ToList();
            }

            // Lặp qua danh sách UserIds để gửi thông báo
            foreach (var userId in userIds)
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    throw new Exception($"User with ID {userId} not found.");
                }

                var notification = new Notification
                {
                    UserId = userId,
                    Title = notificationRequestDto.Title,
                    Description = notificationRequestDto.Description,
                    Href = notificationRequestDto.Href,
                    Icon = notificationRequestDto.Icon,
                    Color = notificationRequestDto.Color,
                    CreatedDate = DateTime.Now,
                    Type = notificationRequestDto.NotificationType,
                    IsRead = false
                };

                _unitOfWork.Notifications.Add(notification);
                await _unitOfWork.CommitAsync();

                var userTokens = await _userTokenService.GetUserTokensByUserIdAsync(userId);

                if (userTokens != null && userTokens.Any())
                {
                    var data = new Dictionary<string, string>
                    {
                        ["type"] = notificationRequestDto.NotificationType.ToString()
                    };
                    if (!string.IsNullOrEmpty(notificationRequestDto.Href))
                    {
                        data["href"] = notificationRequestDto.Href;
                    }
                    if (!string.IsNullOrEmpty(notificationRequestDto.Icon))
                    {
                        data["icon"] = notificationRequestDto.Icon;
                    }
                    if (!string.IsNullOrEmpty(notificationRequestDto.Color))
                    {
                        data["color"] = notificationRequestDto.Color;
                    }

                    var invalidTokens = new List<string>();
                    foreach (var token in userTokens)
                    {
                        var success = await _firebaseService.SendNotificationAsync(token, notificationRequestDto.Title, notificationRequestDto.Description, data);
                        if (!success)
                        {
                            invalidTokens.Add(token);
                        }
                    }

                    // Xử lý token không hợp lệ
                    if (invalidTokens.Any())
                    {
                        await _userTokenService.RemoveTokensAsync(invalidTokens);
                    }
                }
            }
        }


    }
}
