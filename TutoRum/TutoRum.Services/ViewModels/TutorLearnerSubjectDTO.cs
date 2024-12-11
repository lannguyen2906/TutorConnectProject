using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Models;

namespace TutoRum.Services.ViewModels
{
    public class SubjectDetailDto
    {
        public int TutorLearnerSubjectId { get; set; }
        public Guid LearnerId { get; set; }
        public Guid TutorId { get; set; }
        public string SubjectName { get; set; }
        public decimal Rate { get; set; }
        public string Location { get; set; }
        public DateTime? ExpectedStartDate { get; set; }
        public int HoursPerSession { get; set; }
        public string LocationDetail { get; set; }
        public decimal PricePerHour { get; set; }
        public int? SessionsPerWeek { get; set; }
        public bool? IsVerify { get; set; }
        public string? ContractUrl { get; set; }
        public bool? IsClosed {  get; set; }


    }

    public class TutorLearnerSubjectDetailDto
    {
        public int TutorLearnerSubjectId { get; set; }
        public int? TutorSubjectId { get; set; }
        public Guid LearnerId { get; set; }
        public Guid TutorId { get; set; }
        public string? CityId { get; set; }
        public string? DistrictId { get; set; }
        public string? WardId { get; set; }
        public string? LocationDetail { get; set; } // Địa chỉ chi tiết
        public string? ContractUrl { get; set; }
        public decimal? PricePerHour { get; set; } // Giá tiền trên một giờ
        public string? Notes { get; set; } // Ghi chú
        public int? SessionsPerWeek { get; set; } // Số buổi/tuần
        public int? HoursPerSession { get; set; } // Số giờ/buổi
        public string? PreferredScheduleType { get; set; } // Kiểu thời gian muốn học
        public DateTime? ExpectedStartDate { get; set; }
        public bool? IsVerified { get; set; }
        public List<ScheduleDTO>? Schedules { get; set; } = new List<ScheduleDTO>();

        public bool? IsContractVerified { get; set; }
    }


    public class TutorLearnerSubjectSummaryDetailDto
    {
        public int TutorLearnerSubjectId { get; set; }
        public int? TutorSubjectId { get; set; }
        public Guid LearnerId { get; set; }
        public Guid TutorId { get; set; }
        public string CityId { get; set; }
        public string DistrictId { get; set; }
        public string WardId { get; set; }
        public string LocationDetail { get; set; }
        public decimal PricePerHour { get; set; }
        public string Notes { get; set; }
        public int SessionsPerWeek { get; set; }
        public int HoursPerSession { get; set; }
        public string PreferredScheduleType { get; set; }
        public DateTime? ExpectedStartDate { get; set; }
        public bool? IsVerified { get; set; }
        public List<ScheduleDTO> Schedules { get; set; }
        public string ContractUrl { get; set; }
        public bool? IsContractVerified { get; set; }
        public string LearnerEmail { get; set; }
        public bool? IsClosed { get; set; }

        // Thông tin bổ sung từ ClassSummary
        public string ClassType { get; set; } // Loại lớp học (e.g., Cố định)
        public string SubjectName { get; set; } // Môn học (e.g., Tin học)
        public int TotalSessionsCompleted { get; set; } // Tổng số buổi đã học
        public decimal TotalPaidAmount { get; set; } // Tổng tiền đã thanh toán
    }


}
