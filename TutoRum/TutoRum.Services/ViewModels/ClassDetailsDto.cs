using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutoRum.Services.ViewModels
{
    public class ClassSummaryDto
    {
        public string ClassType { get; set; } // Loại lớp học
        public string SubjectName { get; set; } // Môn học
        public int TotalSessionsCompleted { get; set; } // Tổng số buổi đã học
        public decimal TotalPaidAmount { get; set; } // Tổng tiền đã thanh toán
        public decimal PricePerHour { get; set; } // Số tiền theo giờ
    }
}
