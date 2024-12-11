using System;
using System.Collections.Generic;

namespace TutoRum.Data.Models
{
    public partial class QualificationLevel
    {
        public QualificationLevel()
        {
            TutorRequests = new HashSet<TutorRequest>();
        }

        public int Id { get; set; }
        public string? Level { get; set; }

        public virtual ICollection<TutorRequest> TutorRequests { get; set; }

    }
}
