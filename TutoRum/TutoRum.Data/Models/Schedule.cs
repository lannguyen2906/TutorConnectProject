using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Abstract;

namespace TutoRum.Data.Models
{
    public class Schedule : Auditable
    {
        public int ScheduleId { get; set; }
        public Guid? TutorId { get; set; } // Khóa ngoại liên kết với bảng Tutor
        public int? TutorRequestID { get; set; } // Khóa ngoại liên kết với TutorRequest
        public int? TutorLearnerSubjectId { get; set; } // Khóa ngoại liên kết với bảng Subject
        public int? DayOfWeek { get; set; } // Ngày trong tuần (1 = Chủ Nhật, 2 = Thứ Hai, ..., 7 = Thứ Bảy)
        public TimeSpan? StartTime { get; set; } // Giờ bắt đầu
        public TimeSpan? EndTime { get; set; } // Giờ kết thúc

        // Liên kết với các bảng khác
        public virtual Tutor? Tutor { get; set; } // Khóa ngoại liên kết đến bảng Tutor
        public virtual TutorLearnerSubject? tutorLearnerSubject { get; set; }
        public virtual TutorRequest? TutorRequest { get; set; } // Liên kết đến TutorRequest
    }
}