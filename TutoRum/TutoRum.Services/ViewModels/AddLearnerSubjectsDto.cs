using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutoRum.Services.ViewModels
{
    public class AddLearnerSubjectsDto
    {
        public string LearnerId { get; set; }
        public List<LearnerSubjectDto> Subjects { get; set; }
    }

    public class LearnerSubjectDto
    {
        public int SubjectId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
        public int ScheduleID { get; set; }

    }
}
