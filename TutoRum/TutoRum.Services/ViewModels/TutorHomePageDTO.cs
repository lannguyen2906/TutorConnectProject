using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutoRum.Services.ViewModels
{
    public class TutorHomePageDTO
    {
        public IEnumerable<TutorSummaryDto> Tutors { get; set; }
        public int TotalRecordCount { get; set; }
    }
    public class TutorsDTO
    {
        public Guid TutorId { get; set; }
        public string Fullname { get; set; }   
        public int? Experience { get; set; }       
        public string Specialization { get; set; } 
        public decimal? Rating { get; set; }      
        public string AvatarUrl { get; set; }   
        public string ProfileDescription { get; set; } 
        public string Status { get; set; }

        public bool? Gender { get; set; }
        public string AddressDetail { get; set; } 
        public string AddressId { get; set; }


        public List<TutorTeachingLocationDto> TutorTeachingLocations { get; set; } = new List<TutorTeachingLocationDto>();
        public List<ScheduleDTO> Schedules { get; set; } = new List<ScheduleDTO>();
    }

    public class TutorFilterDto
    {
        public List<int> Subjects { get; set; } = new List<int>();

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        public string? City { get; set; }

        public string? District { get; set; }

        public string? SearchingQuery { get; set; }
    }

}
