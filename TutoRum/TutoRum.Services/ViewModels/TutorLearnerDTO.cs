using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutoRum.Services.ViewModels
{
    public class RegisterLearnerDTO
    {
        public int? TutorSubjectId { get; set; }
        public string? CityId { get; set; }
        public string? DistrictId { get; set; }
        public string? WardId { get; set; }
        public string? ContractUrl { get; set; }
        public decimal? PricePerHour { get; set; } // Giá tiền trên một giờ
        public string? Notes { get; set; } // Ghi chú
        public string? LocationDetail { get; set; } // Địa chỉ chi tiết
        public int? SessionsPerWeek { get; set; } // Số buổi/tuần
        public int? HoursPerSession { get; set; } // Số giờ/buổi
        public string? PreferredScheduleType { get; set; } // Kiểu thời gian muốn học
        public DateTime? ExpectedStartDate { get; set; } 
        
        public List<ScheduleDTO> Schedules { get; set; } = new List<ScheduleDTO>();


    }
}
