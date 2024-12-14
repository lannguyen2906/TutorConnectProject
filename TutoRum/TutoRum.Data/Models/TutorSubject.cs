using System;
using System.Collections.Generic;
using TutoRum.Data.Abstract;
using TutoRum.Data.Enum;

namespace TutoRum.Data.Models
{
    public partial class TutorSubject : Auditable
    {
        public TutorSubject()
        {
            TutorLearnerSubjects = new HashSet<TutorLearnerSubject>();
        }

        public int TutorSubjectId { get; set; }
        public Guid? TutorId { get; set; }
        public int? SubjectId { get; set; }
        public decimal? Rate { get; set; }
        public string? Description { get; set; }
        public bool? IsVerified { get; set; }
        public string? EntityType { get; set; } = EntityTypeName.TutorSubject.ToString();
        public string? SubjectType { get; set; } = SubjectTypeName.Fixed.ToString();

        // Thêm khóa ngoại liên kết với RateRange
        public int? RateRangeId { get; set; }
        public virtual RateRange RateRange { get; set; }

        public string? Status { get; set; }
        public virtual Subject? Subject { get; set; }
        public virtual Tutor? Tutor { get; set; }
        public virtual ICollection<TutorLearnerSubject> TutorLearnerSubjects { get; set; }
    }
}