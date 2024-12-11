using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Models;
using TutoRum.Data.Abstract;

namespace TutoRum.Services.ViewModels
{


    public class TutorDto
    {
        public Guid TutorId { get; set; }
        public string? Experience { get; set; }
        public string? Specialization { get; set; }
        public string? Major { get; set; }
        public string? BriefIntroduction { get; set; }
        public int? EducationalLevelID { get; set; }
        public string? EducationalLevelName { get; set; }
        public string? ShortDescription { get; set; }
        public decimal? Rating { get; set; }
        public string? Status { get; set; }
        public string? ProfileDescription { get; set; }
        public string? videoUrl { get; set; }
        public string? FullName { get; set; }
        public string? AvatarUrl { get; set; }
        public string? AddressID { get; set; }
        public string? AddressDetail { get; set; }
        public string? EntityType { get; set; }
        public bool? IsVerified { get; set; }
        public bool IsAccepted { get; set; }

        public List<CertificateDTO> Certificates { get; set; } = new List<CertificateDTO>();
        public List<TutorSubjectDto> TutorSubjects { get; set; } = new List<TutorSubjectDto>();
        public List<TeachingLocationViewDTO> TeachingLocations { get; set; } = new List<TeachingLocationViewDTO>();
        public List<ScheduleDTO> Schedules { get; set; } = new List<ScheduleDTO>();
    }
    public class TutorSummaryDto
    {
        public Guid TutorId { get; set; }
        public string? Experience { get; set; }
        public string? Specialization { get; set; }
        public string? Major { get; set; }
        public string? BriefIntroduction { get; set; }
        public string? EducationalLevel { get; set; }
        public decimal? Rating { get; set; }
        public string? Status { get; set; }
        public string? ProfileDescription { get; set; }
        public bool? IsVerified { get; set; }
        public bool IsAccepted { get; set; } = false;
        public string FullName { get; set; }
        public string AvatarUrl { get; set; }
        public string AddressId { get; set; }
        public int? NumberOfStudents { get; set; }

        public List<CertificateDTO> Certificates { get; set; } = new List<CertificateDTO>();
        public List<TutorSubjectDto> TutorSubjects { get; set; } = new List<TutorSubjectDto>();
        public List<TeachingLocationViewDTO> TeachingLocations { get; set; } = new List<TeachingLocationViewDTO>();
        public List<ScheduleDTO> Schedules { get; set; } = new List<ScheduleDTO>();
    }
    public class AdminHomePageDTO
    {
        public IEnumerable<AdminTutorDto> Tutors { get; set; }
        public int TotalRecordCount { get; set; }
    }
    public  class AdminTutorDto
    {
        public Guid TutorId { get; set; }
        public string? Experience { get; set; }
        public string? Specialization { get; set; }
        public string? Major { get; set; }
        public string? BriefIntroduction { get; set; }
        public string? EducationalLevel { get; set; }
        public decimal? Rating { get; set; }
        public string? Status { get; set; }
        public string? ProfileDescription { get; set; }
        public bool? IsVerified { get; set; }
        public int? TutorQualificationId { get; set; }
        public string FullName { get; set; }
        public string AvatarUrl { get; set; }
        public int AddressId { get; set; }
        public bool IsAccepted { get; set; } = false;

        public DateTime? CreatedDate { get; set; }

        public List<CertificateDTO> Certificates { get; set; } = new List<CertificateDTO>();
        public List<TutorSubjectDto> TutorSubjects { get; set; } = new List<TutorSubjectDto>();
        public List<TeachingLocationViewDTO> TeachingLocations { get; set; } = new List<TeachingLocationViewDTO>();
        public List<ScheduleDTO> Schedules { get; set; } = new List<ScheduleDTO>();
    }
    public class TutorTeachingLocationDto
    {
        public Guid TutorId { get; set; }
        public int TeachingLocationId { get; set; }
    }
    public class TutorSubjectDto
    {
        public int TutorSubjectId { get; set; }
        public Guid? TutorId { get; set; }
        public int? SubjectId { get; set; }
        public decimal? Rate { get; set; }
        public string? Description { get; set; }
        public bool? IsVerified { get; set; }
        public string? EntityType { get; set; }
        public string? SubjectType { get; set; }
        public string? Status { get; set; }
        public int? RateRangeId { get; set; }
        public SubjectDTO? Subject { get; set; }
    }

    public class AddTutorDTO
    {
        public string? Experience { get; set; }
        public string? Specialization { get; set; }
        public string? Status { get; set; }
        public string? ProfileDescription { get; set; }
        public string? Major { get; set; }
        public string? BriefIntroduction { get; set; }
        public int? EducationalLevelID { get; set; }
        public string? ShortDescription { get; set; }
        public string? videoUrl { get; set; }
        public string? AddressID { get; set; }
        public bool IsAccepted { get; set; }


        public virtual ICollection<CertificateDTO>? Certificates { get; set; }
        public virtual ICollection<ScheduleDTO>? Schedule { get; set; }
        public virtual ICollection<AddSubjectDTO>? Subjects { get; set; }
        public virtual ICollection<AddTeachingLocationViewDTO>? TeachingLocation { get; set; }

    }

    

    public class UpdateScheduleByTutorDTO
    {
        public int Id { get; set; }  // ID của lịch cần cập nhật
        public Guid? tutorID { get; set; } // ID của gia sư, dùng để xác nhận lịch thuộc về gia sư này
        public int? DayOfWeek { get; set; } // Ngày trong tuần của lịch (1 = Chủ Nhật, 2 = Thứ Hai, ..., 7 = Thứ Bảy)
        public List<FreeTimeDTO> FreeTimes { get; set; } // Danh sách thời gian rảnh của gia sư (có thể có nhiều khung giờ)
        public int? TutorLearnerSubjectId { get; set; } // ID của môn học mà gia sư dạy (null nếu là lịch rảnh)
    }

    public class UpdateScheduleDTO
    {
        public int Id { get; set; }  
        public Guid? tutorID { get; set; }
        public int? DayOfWeek { get; set; }
        public int? TutorLearnerSubjectID { get; set; }
        public List<FreeTimeDTO> FreeTimes { get; set; }

    }

    public class AddScheduleDTO
    {
        public int? DayOfWeek { get; set; }
        public List<FreeTimeDTO> FreeTimes { get; set; }
    }
    public class DeleteScheduleDTO
    {
        public int ScheduleId { get; set; } 
        public int? DayOfWeek { get; set; }
        public List<FreeTimeDTO> FreeTimes { get; set; }
    }


    public class FreeTimeDTO
    {
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
    }

    public class UpdateScheduleTutorDTO
    {
        public int ScheduleId { get; set; } // ID của lịch, 0 nếu là lịch mới
        public int DayOfWeek { get; set; }  // Ngày trong tuần
        public TimeSpan? StartTime { get; set; } // Thời gian bắt đầu
        public TimeSpan? EndTime { get; set; }   // Thời gian kết thúc
        public int TutorLearnerSubjectId { get; set; } // Liên kết tới lớp học, null nếu không có
    }


    public class TutorRequestSchedulesDTO
    {
        public int? DayOfWeek { get; set; }
        public List<FreeTimeDTO> FreeTimes { get; set; }

    }
    public class ScheduleGroupDTO
    {
        public int? DayOfWeek { get; set; } // Thứ trong tuần (0 = Chủ Nhật, 1 = Thứ Hai, ...)
        public List<ScheduleViewDTO> Schedules { get; set; }
    }


    public class ScheduleDTO
    {
        public int? DayOfWeek { get; set; }
        public List<FreeTimeDTO> FreeTimes { get; set; }
    }

    public class ScheduleViewDTO
    {
        public int Id { get; set; }
        public int? DayOfWeek { get; set; }
        public List<FreeTimeDTO> FreeTimes { get; set; }
        public string SubjectNames { get; set; }
        public int? TutorLearnerSubjectId { get; set; }
    }

   
    public class UpdateTutorDTO
    {
        public string? Experience { get; set; }
        public string? Specialization { get; set; }
        public decimal? Rating { get; set; }
        public string? ProfileDescription { get; set; }

        public virtual ICollection<CertificateDTO>? Certificates { get; set; }
        public virtual ICollection<ScheduleDTO>? Schedule { get; set; }
        public virtual ICollection<SubjectDTO>? Subjects { get; set; }

        public virtual ICollection<TeachingLocationViewDTO>? TeachingLocation { get; set; }
    }

    public class UpdateTutorInforDTO
    {
        public string Experience { get; set; } // Kinh nghiệm của gia sư
        public string Specialization { get; set; } // Chuyên môn của gia sư
        public string ProfileDescription { get; set; } // Mô tả hồ sơ
        public string BriefIntroduction { get; set; } // Giới thiệu ngắn gọn
        public int EducationalLevelID { get; set; } // Mức độ học vấn của gia sư
        public string ShortDescription { get; set; } // Mô tả ngắn
        public string Major { get; set; } // Ngành học chính của gia sư
        public string VideoUrl { get; set; } // Link video (nếu có)
        public bool IsAccepted { get; set; } // Trạng thái chấp nhận gia sư (ví dụ: Chưa chấp nhận hoặc đã chấp nhận)
        public string AddressID { get; set; } // ID địa chỉ của gia sư
        public List<AddTeachingLocationViewDTO> TeachingLocation { get; set; } // Danh sách các địa điểm dạy học của gia sư
        public List<CertificateDTO> Certificates { get; set; } // Danh sách chứng chỉ của gia sư
        public List<AddSubjectDTO>? Subjects { get; set; }

    }


    public class CertificateDTO
    {
        public int? CertificateId { get; set; }
        public string? ImgUrl { get; set; }
        public string? Description { get; set; }
        public bool? IsVerified { get; set; }
        public string? EntityType { get; set; }
        public string? Status { get; set; }


        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }

    public class TeachingLocationDTO
    {
        public string? CityId { get; set; }
        public string? DistrictId { get; set; }
    }


    public class AddTeachingLocationViewDTO
    {
        public string? CityId { get; set; }
        public List<AddDistrictDTO> Districts { get; set; }
    }

    public class AddDistrictDTO
    {
        public string DistrictId { get; set; }

    }
    public class TeachingLocationViewDTO
    {
        public string? CityId { get; set; }
        public string? CityName { get; set; }
        public List<DistrictDTO> Districts { get; set; }
    }

    public class DistrictDTO
    {
        public int? TeachingLocationId { get; set; }
        public string DistrictId { get; set; }
        public string? DistrictName { get; set; }

    }

    public class CityResponse
    {
        public int error { get; set; }
        public string error_text { get; set; }
        public string name { get; set; }
        public List<DistrictResponse> data { get; set; }
    }

    public class DistrictResponse
    {
        public string name { get; set; }
    }


    public class AddressApiResponse
    {
        public int error { get; set; }
        public string error_text { get; set; }
        public AddressData data { get; set; }
    }

    public class AddressData
    {
        public string full_name { get; set; }
        public string full_name_en { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string tinh { get; set; }
        public string quan { get; set; }
        public string phuong { get; set; }
    }

    public class WalletOverviewDto
    {
        public decimal CurrentBalance { get; set; } // Số dư hiện tại
        public decimal TotalEarningsThisMonth { get; set; } // Tổng thu nhập tháng này
        public int PendingWithdrawals { get; set; } // Số yêu cầu rút tiền đang chờ xử lý
    }

}
