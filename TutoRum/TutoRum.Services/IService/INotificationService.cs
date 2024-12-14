using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Models;
using TutoRum.Services.ViewModels;

namespace TutoRum.Services.IService
{
    public interface INotificationService
    {
        Task<NotificationDtos> GetNotificationsByUserAsync(ClaimsPrincipal user, int pageIndex = 0, int pageSize = 20);
        Task MarkNotificationAsReadAsync(List<int> ids);
        Task SendNotificationAsync(NotificationRequestDto notificationRequestDto, bool? sendToAdmins);
    }
}
