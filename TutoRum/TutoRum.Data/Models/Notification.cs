using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Abstract;
using TutoRum.Data.Enum;

namespace TutoRum.Data.Models
{
    public class Notification : Auditable
    {
        public int NotificationId { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; } = string.Empty; 
        public string Description { get; set; } = string.Empty; 
        public string? Href { get; set; } 
        public string? Icon { get; set; } 
        public string? Color { get; set; }
        public bool IsRead { get; set; } = false;
        public NotificationType Type { get; set; } // Loại thông báo

        [ForeignKey("UserId")]
        public virtual AspNetUser? User { get; set; } // Liên kết đến user
    }
}
