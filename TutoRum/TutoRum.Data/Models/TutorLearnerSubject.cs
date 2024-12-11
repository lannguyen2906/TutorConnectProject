using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TutoRum.Data.Abstract;
using TutoRum.Data.Enum;

namespace TutoRum.Data.Models
{
    public partial class TutorLearnerSubject : Auditable
    {
        public TutorLearnerSubject()
        {
            BillingEntries = new HashSet<BillingEntry>();
            Feedbacks = new HashSet<Feedback>();
        }

        public int TutorLearnerSubjectId { get; set; }
        public int? TutorSubjectId { get; set; }
        public Guid? LearnerId { get; set; }


        public decimal? PricePerHour { get; set; }
        public string? Notes { get; set; }
        public string? Location { get; set; }
        public string? DistrictId { get; set; }
        public string? WardId { get; set; }
        public string? LocationDetail { get; set; }
        public int? SessionsPerWeek { get; set; } // Số buổi/tuần
        public int? HoursPerSession { get; set; } // Số giờ/buổi
        public string? PreferredScheduleType { get; set; } // Kiểu thời gian muốn học
        public DateTime? ExpectedStartDate { get; set; } // Thời gian dự kiến
        public string? ContractUrl { get; set; }

        public string? Status { get; set; }
        public bool? IsVerified { get; set; }
        public bool? IsContractVerified { get; set; }
        public string? ContractNote { get; set; }
        public string? EntityType { get; set; } = EntityTypeName.TutorLearnerSubject.ToString();
        public bool IsCloseClass { get; set; } = false;
        [ForeignKey("LearnerId")]
        public virtual AspNetUser? Learner { get; set; } // Navigation property to AspNetUsers
        public virtual TutorSubject? TutorSubject { get; set; }
        public virtual ICollection<BillingEntry> BillingEntries { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
    }
}