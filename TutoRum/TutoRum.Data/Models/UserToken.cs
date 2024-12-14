using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Abstract;

namespace TutoRum.Data.Models
{
    public class UserToken : Auditable
    {
        public int UserTokenId { get; set; } 
        public Guid UserId { get; set; } 
        public string FcmToken { get; set; } = string.Empty; 
        public string DeviceType { get; set; } = "web"; 

        [ForeignKey("UserId")]
        public virtual AspNetUser? User { get; set; } // Liên kết đến user
    }
}
