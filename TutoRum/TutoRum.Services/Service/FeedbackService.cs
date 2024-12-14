using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.IRepositories;
using TutoRum.Data.Models;
using TutoRum.Data.Repositories;
using TutoRum.Services.IService;
using TutoRum.Services.ViewModels;

namespace TutoRum.Services.Service
{
    public class FeedbackService : IFeedbackService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FeedbackService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }



        public async Task<FeedbackDetail> GetFeedbackDetailByTutorIdAsync(Guid tutorId, bool showAll = false)
        {
            // Build the predicate to filter feedback by TutorId via TutorSubject
            var allQuery = _unitOfWork.feedback.GetMultiAsQueryable(f =>
                f.TutorLearnerSubject != null &&
                f.TutorLearnerSubject.TutorSubject != null &&
                f.TutorLearnerSubject.TutorSubject.TutorId == tutorId)
                .Include(f => f.TutorLearnerSubject.Learner)
                .Include(f => f.TutorLearnerSubject.TutorSubject.Subject);

            // Apply limit if showAll is false
            var displayQuery = showAll ? allQuery : allQuery.Take(6);

            // Execute the query to fetch feedback data
            var feedbacks =  displayQuery.ToList();

            var feedbackDetail = new FeedbackDetail
            {
                AvarageRating = allQuery.Average(a => a.Rating),
                TotalFeedbacks = allQuery.Count(),
                RatingsBreakdown = new Dictionary<int, int>
                {
                    { 5, allQuery.Count(f => f.Rating > 4 && f.Rating <= 5) },
                    { 4, allQuery.Count(f => f.Rating > 3 && f.Rating <= 4) },
                    { 3, allQuery.Count(f => f.Rating > 2 && f.Rating <= 3) },
                    { 2, allQuery.Count(f => f.Rating > 1 && f.Rating <= 2) },
                    { 1, allQuery.Count(f => f.Rating > 0 && f.Rating <= 1) }
                },
                Feedbacks = feedbacks.Select(f => new UserFeedbackDto
                {
                    FeedbackId = f.FeedbackId,
                    TutorLearnerSubjectId = f.TutorLearnerSubject.TutorLearnerSubjectId,
                    TutorLearnerSubjectName = f.TutorLearnerSubject.TutorSubject.Subject.SubjectName,
                    AvatarUrl = f.TutorLearnerSubject.Learner.AvatarUrl,
                    FullName = f.TutorLearnerSubject.Learner.Fullname,
                    Rating = f.Rating,
                    Comments = f.Comments,
                    CreatedDate = f.CreatedDate,
                    UpdatedDate = f.UpdatedDate
                }).ToList()
            };

            // Map to DTOs
            return feedbackDetail;
        }



        public async Task<FeedbackDto> CreateFeedbackAsync(CreateFeedbackDto createFeedbackDto)
        {
            // Ánh xạ từ DTO sang mô hình Feedback
            var feedback = _mapper.Map<Feedback>(createFeedbackDto);
            feedback.CreatedDate = DateTime.UtcNow;

            // Thêm feedback vào cơ sở dữ liệu
            var createdFeedback = _unitOfWork.feedback.Add(feedback);
            await _unitOfWork.CommitAsync();

            // Lấy TutorId từ TutorLearnerSubject
            var tutorId = feedback.TutorLearnerSubject?.TutorSubject?.TutorId;

            if (tutorId.HasValue)
            {
                // Tính điểm trung bình mới cho gia sư
                var averageRating = await _unitOfWork.Tutors.GetAverageRatingForTutorAsync(tutorId.Value);

                if (averageRating.HasValue)
                {
                    // Lấy thông tin gia sư
                    var tutor = await _unitOfWork.Tutors.GetByIdAsync(tutorId.Value);
                    if (tutor != null)
                    {
                        tutor.Rating = averageRating.Value;
                        _unitOfWork.Tutors.Update(tutor);
                        await _unitOfWork.CommitAsync();
                    }
                }
            }

            // Trả về kết quả dưới dạng FeedbackDto
            return _mapper.Map<FeedbackDto>(createdFeedback);
        }




        public async Task<FeedbackDto> UpdateFeedbackAsync(FeedbackDto feedbackDto)
        {
            // Lấy phản hồi hiện tại dựa trên TutorLearnerSubjectId
            var existingFeedback = await _unitOfWork.feedback.GetFeedbackByTutorLearnerSubjectIdAsync(feedbackDto.TutorLearnerSubjectId);
            if (existingFeedback == null)
            {
                throw new KeyNotFoundException("Feedback not found.");
            }

            // Cập nhật các trường trong Feedback
            existingFeedback.Rating = feedbackDto.Rating;
            existingFeedback.Comments = feedbackDto.Comments;
            existingFeedback.Punctuality = feedbackDto.Punctuality;
            existingFeedback.SupportQuality = feedbackDto.SupportQuality;
            existingFeedback.TeachingSkills = feedbackDto.TeachingSkills;
            existingFeedback.ResponseToQuestions = feedbackDto.ResponseToQuestions;
            existingFeedback.Satisfaction = feedbackDto.Satisfaction;
            existingFeedback.UpdatedDate = DateTime.Now;

            // Cập nhật vào cơ sở dữ liệu
            _unitOfWork.feedback.Update(existingFeedback);
            await _unitOfWork.CommitAsync();

            // Lấy TutorId từ TutorLearnerSubject
            var tutorId = existingFeedback.TutorLearnerSubject?.TutorSubject?.TutorId;

            if (tutorId.HasValue)
            {
                // Tính toán lại điểm trung bình cho gia sư
                var averageRating = await _unitOfWork.Tutors.GetAverageRatingForTutorAsync(tutorId.Value);

                if (averageRating.HasValue)
                {
                    // Lấy thông tin gia sư và cập nhật điểm trung bình
                    var tutor = await _unitOfWork.Tutors.GetByIdAsync(tutorId.Value);
                    if (tutor != null)
                    {
                        tutor.Rating = averageRating.Value;
                        _unitOfWork.Tutors.Update(tutor);
                        await _unitOfWork.CommitAsync();
                    }
                }
            }

            // Trả về FeedbackDto sau khi cập nhật
            return _mapper.Map<FeedbackDto>(existingFeedback);
        }


        public async Task<FeedbackDto> GetFeedbackByTutorLearnerSubjectIdAsync(int tutorLearnerSubjectId)
        {
            var existingFeedback = await _unitOfWork.feedback.GetFeedbackByTutorLearnerSubjectIdAsync(tutorLearnerSubjectId);
            if (existingFeedback == null)
            {
                throw new KeyNotFoundException("Feedback not found.");
            }

            return new FeedbackDto()
            {
                Comments = existingFeedback.Comments,
                Punctuality = existingFeedback.Punctuality,
                ResponseToQuestions = existingFeedback.ResponseToQuestions,
                Satisfaction = existingFeedback.Satisfaction,
                SupportQuality = existingFeedback.SupportQuality,
                TeachingSkills = existingFeedback.TeachingSkills,
                Rating = existingFeedback.Rating,
                FeedbackId = existingFeedback.FeedbackId,
                TutorLearnerSubjectId = existingFeedback.TutorLearnerSubjectId ?? -1,
                CreatedDate = existingFeedback.CreatedDate,
                UpdatedDate = existingFeedback.UpdatedDate,
            };
        }



        public async Task<(List<QuestionStatistics>, List<string>)> GetFeedbackStatisticsForTutorAsync(Guid tutorId)
        {
            // Truy vấn tất cả các phản hồi liên quan đến gia sư
            var feedbackQuery = _unitOfWork.feedback.GetMultiAsQueryable(
                f => f.TutorLearnerSubject.TutorSubject.TutorId == tutorId,
                includes: new[] { "TutorLearnerSubject.TutorSubject" }
            );

            var totalFeedbacks = await feedbackQuery.CountAsync();
            if (totalFeedbacks == 0)
            {
                return (new List<QuestionStatistics>(), new List<string>());
            }

            // Lấy danh sách các bình luận
            var comments = await feedbackQuery
                .Where(f => !string.IsNullOrEmpty(f.Comments))
                .Select(f => f.Comments)
                .ToListAsync();

            // Danh sách ánh xạ câu hỏi và biểu thức điều kiện tương ứng
            var questionMappings = new Dictionary<string, Expression<Func<Feedback, int?>>>
    {
        { "punctuality", f => f.Punctuality },
        { "supportQuality", f => f.SupportQuality },
        { "teachingSkills", f => f.TeachingSkills },
        { "responseToQuestions", f => f.ResponseToQuestions },
        { "satisfaction", f => f.Satisfaction }
    };

            var statistics = new List<QuestionStatistics>();

            foreach (var question in questionMappings)
            {
                // Truy vấn và nhóm dữ liệu theo từng lựa chọn của câu hỏi
                var answerBreakdown = await feedbackQuery
                    .GroupBy(question.Value)
                    .Select(g => new AnswerBreakdown
                    {
                        Value = g.Key ?? 0, // Lựa chọn (1, 2, 3,...)
                        AnswerCount = g.Count().ToString()
                    })
                    .ToListAsync();

                // Thêm vào danh sách thống kê
                statistics.Add(new QuestionStatistics
                {
                    QuestionType = question.Key,
                    TotalAnswerCount = totalFeedbacks.ToString(),
                    AnswerBreakdown = answerBreakdown
                });
            }

            return (statistics, comments);
        }




    }
}
