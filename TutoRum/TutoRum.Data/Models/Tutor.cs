using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using TutoRum.Data.Abstract;
using TutoRum.Data.Enum;

namespace TutoRum.Data.Models
{
    public partial class Tutor : Auditable
    {
        public Tutor()
        {
            Certificates = new HashSet<Certificate>();
            TutorSubjects = new HashSet<TutorSubject>();
            TutorTeachingLocations = new HashSet<TutorTeachingLocations>();
            Schedules = new HashSet<Schedule>();
            PaymentRequests = new HashSet<PaymentRequest>();
            TutorRequestTutors = new HashSet<TutorRequestTutor>();
        }

        public Guid TutorId { get; set; }
        public string? Experience { get; set; }
        public string? Specialization { get; set; }
        public decimal? Rating { get; set; }
        public string? Status { get; set; }
        public string? ProfileDescription { get; set; }
        public string? BriefIntroduction { get; set; }
        public string? Major { get; set; }
        public string? ShortDescription { get; set; }
        public int? EducationalLevel { get; set; }
        public string? videoUrl { get; set; }
        public bool? IsVerified { get; set; }
        public bool IsAccepted { get; set; } = false;
        public string? EntityType { get; set; } = EntityTypeName.Tutor.ToString();
        public virtual AspNetUser TutorNavigation { get; set; } = null!;
        public virtual ICollection<Certificate> Certificates { get; set; }
        public virtual ICollection<TutorSubject> TutorSubjects { get; set; }
        public decimal Balance { get; set; } = 0;
        public ICollection<TutorTeachingLocations> TutorTeachingLocations { get; set; }
        public ICollection<Schedule> Schedules { get; set; }
        public virtual ICollection<TutorRequestTutor> TutorRequestTutors { get; set; }
        public virtual ICollection<PaymentRequest> PaymentRequests { get; set; } = new HashSet<PaymentRequest>();
    }
}