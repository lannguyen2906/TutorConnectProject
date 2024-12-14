using System;
using System.Collections.Generic;
using TutoRum.Data.Abstract;
using TutoRum.Data.Enum;

namespace TutoRum.Data.Models
{
    public partial class TutorRequest : Auditable
    {
        public int Id { get; set; }
        public string? PhoneNumber { get; set; }
        public string? RequestSummary { get; set; }
        public string? CityId { get; set; }
        public string? DistrictId { get; set; }
        public string? WardId { get; set; }
        public string? TeachingLocation { get; set; }
        public int NumberOfStudents { get; set; }
        public DateTime StartDate { get; set; }
        public string? PreferredScheduleType { get; set; } // Kiểu thời gian muốn học
        public TimeSpan TimePerSession { get; set; }
        public string? Subject { get; set; }
        public string? StudentGender { get; set; }
        public string? TutorGender { get; set; }
        public decimal Fee { get; set; }
        public int SessionsPerWeek { get; set; }
        public string? DetailedDescription { get; set; }
        public int? TutorQualificationId { get; set; }
        public Guid? AspNetUserId { get; set; }
        public string? FreeSchedules { get; set; }
        public int? TutorLearnerSubjectId { get; set; }
        public TutorLearnerSubject TutorLearnerSubject { get; set; }

        public bool? IsVerified { get; set; }
        public string? EntityType { get; set; } = EntityTypeName.TutorRequest.ToString();
        public string? Status { get; set; }
        public virtual AspNetUser? AspNetUser { get; set; }
        public virtual QualificationLevel? TutorQualification { get; set; }
        public virtual ICollection<TutorRequestTutor> TutorRequestTutors { get; set; } = new List<TutorRequestTutor>();
        // Liên kết với nhiều Schedule
        public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

        public int? RateRangeId { get; set; } // Foreign key property
        public virtual RateRange RateRange { get; set; } // Navigation property
    }

}
