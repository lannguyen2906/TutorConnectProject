using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutoRum.Services.ViewModels
{
    public class SubjectDTO
    {
        public int? SubjectId { get; set; }
        public string? SubjectName { get; set; }
    }

    public class AddSubjectViewDTO
    {
        public string? SubjectName { get; set; }
        public decimal? Rate { get; set; }
        public string? Description { get; set; }
    }

    public class AddSubjectDTO
    {
        public string? SubjectName { get; set; }
        public decimal? Rate { get; set; }
        public string? Description { get; set; }
        public string? SubjectType { get; set; }
        public int? RateRangeId { get; set; }


    }

    public class UpdateSubjectDTO
    {
        public int SubjectId { get; set; }
        public string? SubjectName { get; set; }
        public decimal? Rate { get; set; }
        public string? Description { get; set; }
        public string? SubjectType { get; set; }
        public int? RateRangeId { get; set; }


    }

    public class SubjectFilterDTO
    {
        public int? SubjectId { get; set; }
        public string? SubjectName { get; set; }
        public int? RateRangeId { get; set; }
        public int? NumberOfUsages { get; set; }
    }
}
