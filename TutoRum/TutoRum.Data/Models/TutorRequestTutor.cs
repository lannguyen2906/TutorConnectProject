using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutoRum.Data.Models
{
    public class TutorRequestTutor
    {
        public int TutorRequestId { get; set; }
        public TutorRequest TutorRequest { get; set; }

        public Guid TutorId { get; set; }  // Đây là ID của gia sư từ bảng Tutor
        public Tutor Tutor { get; set; }  // Liên kết với bảng Tutor

        public bool? IsVerified { get; set; }
        public bool Ischoose { get; set; }
        
        public DateTime DateJoined { get; set; }  // Ngày gia sư đăng ký vào yêu cầu này
        public string Status { get; set; }  // Trạng thái gia sư (ví dụ: Pending, Accepted, Rejected)
    }
}
