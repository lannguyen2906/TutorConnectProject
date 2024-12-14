using System;
using System.Collections.Generic;
using TutoRum.Data.Abstract;

namespace TutoRum.Data.Models
{
    public partial class Feedback : Auditable
    {
        public int FeedbackId { get; set; }
        public int? TutorLearnerSubjectId { get; set; }
        public decimal? Rating { get; set; }
        public string? Comments { get; set; }

        public int? Punctuality { get; set; } // Sự đúng giờ của giảng viên
        public int? SupportQuality { get; set; } // Hỗ trợ của giảng viên
        public int? TeachingSkills { get; set; } // Kỹ năng sư phạm
        public int? ResponseToQuestions { get; set; } // Đáp ứng thắc mắc
        public int? Satisfaction { get; set; } // Mức độ hài lòng

        public virtual TutorLearnerSubject? TutorLearnerSubject { get; set; }
    }
}
