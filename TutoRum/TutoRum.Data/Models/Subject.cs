using System;
using System.Collections.Generic;
using TutoRum.Data.Abstract;

namespace TutoRum.Data.Models
{
    public partial class Subject : Auditable
    {
        public Subject()
        {
            TutorSubjects = new HashSet<TutorSubject>();
        }

        public int SubjectId { get; set; }
        public string? SubjectName { get; set; }
        public bool? IsVerified { get; set; }

        public virtual ICollection<TutorSubject> TutorSubjects { get; set; }
    }
}