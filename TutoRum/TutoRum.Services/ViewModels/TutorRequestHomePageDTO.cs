using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Models;

namespace TutoRum.Services.ViewModels
{
    public class TutorRequestHomePageDTO
    {
        public List<TutorRequestDTO> TutorRequests { get; set; }
        public int TotalRecordCount { get; set; }
    }

    public class TutorRequestDTO
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string RequestSummary { get; set; }
        public string CityId { get; set; }
        public string DistrictId { get; set; }
        public string WardId { get; set; }
        public string TeachingLocation { get; set; }
        public int NumberOfStudents { get; set; }
        public DateTime StartDate { get; set; }
        public string PreferredScheduleType { get; set; }
        public TimeSpan TimePerSession { get; set; }
        public string Subject { get; set; }
        public string StudentGender { get; set; }
        public string TutorGender { get; set; }
        public decimal Fee { get; set; }
        public string? LearnerName { get; set; }
        public int SessionsPerWeek { get; set; }
        public string DetailedDescription { get; set; }
        public int? TutorQualificationId { get; set; }
        public string? TutorQualificationName { get; set; }
        public string Status { get; set; }
        public string FreeSchedules { get; set; }
        public int? rateRangeId { get; set; }
        public Guid? CreatedUserId { get; set; }

        public List<Guid> RegisteredTutorIds { get; set; } = new List<Guid>();

    }

    public class ListTutorRequestDTO : TutorRequest
    {
        public int TutorRequestId { get; set; }
        public string Subject { get; set; } // Tên môn học
        public DateTime StartDate { get; set; } // Ngày đăng
        public string DetailedDescription { get; set; } // Mô tả chi tiết
        public string Status { get; set; } // Trạng thái yêu cầu
        public bool IsTutorAssigned { get; set; } // Trạng thái đã tìm được gia sư chưa
        public bool? IsVerified { get; set; } // Trạng thái đã được admin duyệt chưa
        public string FreeSchedules { get; set; }
        public Guid? ChosenTutorId { get; set; }
        public int NumberOfRegisteredTutor {  get; set; }
    }

    public class ListTutorRequestForTutorDto : TutorRequest
    {
        public bool? IsInterested { get; set; }
        public bool? IsChosen { get; set; }
    }

    public class TutorRequestFilterDto
    {
        public string? Search { get; set; }
        public DateTime? StartDate { get; set; }
        public string? Subject { get; set; }
        public bool? IsVerified { get; set; }
    }

    public class TutorRequestHomepageFilterDto
    {
        public string? Search { get; set; }
        public decimal? MinFee { get; set; }
        public decimal? MaxFee { get; set; }
        public string? CityId { get; set; }
        public string? DistrictId { get; set; }
        public string? Subject { get; set; }
        public string? TutorGender { get; set; }
        public int? TutorQualificationId { get; set; }
        public int? RateRangeId { get; set; }

    }
}
