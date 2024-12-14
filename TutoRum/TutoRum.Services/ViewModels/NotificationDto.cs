using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Enum;
using TutoRum.Data.Models;

namespace TutoRum.Services.ViewModels
{
    public class NotificationDto
    {
        public int NotificationId { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Href { get; set; }
        public string? Icon { get; set; }
        public string? Color { get; set; }
        public bool IsRead { get; set; } = false;

        public DateTime? CreatedDate { get; set; }
    }
    public class UserTokenDto
    {
        public Guid UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public string DeviceType { get; set; } = "web";
    }

    public class NotificationDtos
    {
        public int TotalRecords { get; set; }

        public int TotalUnreadNotifications { get; set; }

        public List<NotificationDto> Notifications { get; set; } = new List<NotificationDto>();
    }

    public class NotificationRequestDto
    {
        public Guid UserId { get; set; }
        public string Title { get; set; }  
        public string Description { get; set; }  
        public NotificationType NotificationType { get; set; }
        public string? Href { get; set; }
        public string? Icon { get; set; }
        public string? Color { get; set; }
    }
}
