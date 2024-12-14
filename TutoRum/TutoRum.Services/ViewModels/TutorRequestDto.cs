using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutoRum.Services.ViewModels
{

    public class TutorRequestWithTutorsDTO
    {
        public int TutorRequestId { get; set; }
        public string Subject { get; set; }
        public DateTime StartDate { get; set; }
        
        public List<TutorInTutorRequestDTO> Tutors { get; set; }
    }

    public class TutorInTutorRequestDTO
    {
        public Guid TutorId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Specialization { get; set; }
        public bool? IsVerified { get; set; }
    }

    public class CreateTutorRequestDto
    {
       
        public string? PhoneNumber { get; set; }
        public string RequestSummary { get; set; }
        public string? CityId { get; set; }
        public string? DistrictId { get; set; }
        public string? WardId { get; set; }
        public string TeachingLocation { get; set; }
        public int NumberOfStudents { get; set; }
        public DateTime StartDate { get; set; }
        public string? PreferredScheduleType { get; set; } // Kiểu thời gian muốn học
        public TimeSpan TimePerSession { get; set; }
        public string Subject { get; set; }
        public string StudentGender { get; set; }
        public string TutorGender { get; set; }
        public decimal Fee { get; set; }
        public int SessionsPerWeek { get; set; }
        public string? DetailedDescription { get; set; }
        public int TutorQualificationId { get; set; }
        public virtual ICollection<ScheduleDTO>? Schedule { get; set; }
    }
}
