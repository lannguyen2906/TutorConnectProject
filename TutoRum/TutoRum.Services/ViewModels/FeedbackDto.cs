using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Models;

namespace TutoRum.Services.ViewModels
{
    public class FeedbackDto
    {
        public int FeedbackId { get; set; }
        public int TutorLearnerSubjectId { get; set; }
        public decimal? Rating { get; set; }
        public string? Comments { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Các câu hỏi bổ sung
        public int? Punctuality { get; set; }       // Sự đúng giờ của giảng viên
        public int? SupportQuality { get; set; }    // Hỗ trợ của giảng viên
        public int? TeachingSkills { get; set; }    // Kỹ năng sư phạm
        public int? ResponseToQuestions { get; set; } // Đáp ứng thắc mắc
        public int? Satisfaction { get; set; }      // Mức độ hài lòng
    }

    public class UserFeedbackDto
    {
        public int FeedbackId { get; set; }
        public int TutorLearnerSubjectId { get; set; }
        public string TutorLearnerSubjectName { get; set; }
        public string FullName { get; set; }
        public string AvatarUrl { get; set; }
        public decimal? Rating { get; set; }
        public string? Comments { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class FeedbackDetail
    {
        public decimal? AvarageRating { get; set; }
        public int? TotalFeedbacks { get; set; }
        public Dictionary<int, int> RatingsBreakdown { get; set; }
        public List<UserFeedbackDto> Feedbacks { get; set; }
    }

    public class CreateFeedbackDto
    {
        public int? TutorLearnerSubjectId { get; set; }
        public decimal? Rating { get; set; }
        public string? Comments { get; set; }

        // Các câu hỏi bổ sung
        public int? Punctuality { get; set; }       // Sự đúng giờ của giảng viên
        public int? SupportQuality { get; set; }    // Hỗ trợ của giảng viên
        public int? TeachingSkills { get; set; }    // Kỹ năng sư phạm
        public int? ResponseToQuestions { get; set; } // Đáp ứng thắc mắc
        public int? Satisfaction { get; set; }      // Mức độ hài lòng
    }


    public class TutorRatingDto
    {
        public Guid TutorId { get; set; }
        public decimal? AverageRating { get; set; }
        public List<FeedbackDto> Feedbacks { get; set; } = new List<FeedbackDto>();
    }

    public class FeedbackViewTutorDTO
    {

        public Guid TutorId { get; set; }
        public string FullName { get; set; }
        public string AvatarUrl { get; set; }
        public decimal? Rating { get; set; }
        public string Description { get; set; }
        public List<FeedbackDto> Feedbacks { get; set; } = new List<FeedbackDto>();
    }


    public class FeedbackStatisticsDto
    {
        public string Question { get; set; } // Tên câu hỏi (ví dụ: "Punctuality")
        public int AnswerOption { get; set; } // Giá trị lựa chọn (ví dụ: 1, 2, 3...)
        public int Count { get; set; } // Số lượng chọn lựa tương ứng
        public int TotalFeedbacks { get; set; } // Tổng số phản hồi
    }

    public class QuestionStatistics
    {
        public string QuestionType { get; set; }
        public string TotalAnswerCount { get; set; }
        public List<AnswerBreakdown> AnswerBreakdown { get; set; }
    }

    public class AnswerBreakdown
    {
        public int Value { get; set; }
        public string AnswerCount { get; set; }
    }

    public class FeedbackStatisticsResponse
    {
        public List<QuestionStatistics> Statistics { get; set; }
        public List<string> Comments { get; set; }
    }
}
