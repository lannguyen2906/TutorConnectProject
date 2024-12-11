using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Models;

namespace TutoRum.Data.IRepositories
{
    public interface IFeedbackRepository : IRepository<Feedback>
    {
       
        Task<IEnumerable<Feedback>> GetFeedbacksByLearnerIdAsync(Guid learnerId);

        Task<Feedback> GetFeedbackByIdAsync(int feedbackId);
        Task<Feedback> GetFeedbackByTutorLearnerSubjectIdAsync(int tutorLearnerSubjectId);
        Task<List<Feedback>> GetFeedbacksForTutorAsync(Guid tutorId);
    }
}
