using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Services.ViewModels;

namespace TutoRum.Services.IService
{
    public interface IFeedbackService
    {
        Task<FeedbackDto> CreateFeedbackAsync(CreateFeedbackDto createFeedbackDto);
        Task<FeedbackDto?> UpdateFeedbackAsync(FeedbackDto  feedbackDto);
        Task<FeedbackDetail> GetFeedbackDetailByTutorIdAsync(Guid tutorId, bool showAll = false);
        Task<FeedbackDto> GetFeedbackByTutorLearnerSubjectIdAsync(int tutorLearnerSubjectId);
        Task<(List<QuestionStatistics>, List<string>)> GetFeedbackStatisticsForTutorAsync(Guid tutorId);
    }
}
