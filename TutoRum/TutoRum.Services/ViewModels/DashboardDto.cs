using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutoRum.Services.ViewModels
{
    public class DashboardKeyMetricsDto
    {
        public decimal? TotalRevenue { get; set; }
        public int? NumberOfTutors { get; set; }
        public int? NumberOfLearners { get; set; }
        public int? NumberOfClassrooms { get; set; }
        public int? NumberOfPendingPaymentRequests { get; set; }
    }
    // Doanh thu theo ngày
    public class MonthlyRevenueData
    {
        public int Month { get; set; } // Ngày
        public decimal RevenueAmount { get; set; } // Doanh thu của ngày đó
    }

    // Số lượng gia sư theo thành phố
    public class TutorsByCityData
    {
        public string City { get; set; } // Tên thành phố
        public int TutorCount { get; set; } // Số lượng gia sư
    }

    // Số lượng học viên theo thành phố
    public class LearnersByCityData
    {
        public string City { get; set; } // Tên thành phố
        public int LearnerCount { get; set; } // Số lượng học viên
    }

    // Đánh giá trung bình theo trình độ chuyên môn
    public class FeedbackByQualificationLevelData
    {
        public string Qualification { get; set; } // Trình độ chuyên môn
        public decimal AverageRating { get; set; } // Đánh giá trung bình
    }

    // Số lượng gia sư theo môn học
    public class TutorsBySubjectData
    {
        public string Subject { get; set; } // Tên môn học
        public int TutorCount { get; set; } // Số lượng gia sư
    }

    // Dữ liệu cho biểu đồ của dashboard
    public class DashboardChartData
    {
        public List<MonthlyRevenueData> MonthlyRevenues { get; set; } = new List<MonthlyRevenueData>(); // Doanh thu theo ngày
        public List<TutorsByCityData> TutorsByCity { get; set; } = new List<TutorsByCityData>(); // Gia sư theo thành phố
        public List<LearnersByCityData> LearnersByCity { get; set; } = new List<LearnersByCityData>(); // Học viên theo thành phố
        public List<FeedbackByQualificationLevelData> FeedbackByQualificationLevels { get; set; } = new List<FeedbackByQualificationLevelData>(); // Đánh giá theo trình độ
        public List<TutorsBySubjectData> TutorsBySubjects { get; set; } = new List<TutorsBySubjectData>(); // Gia sư theo môn học
    }

}
