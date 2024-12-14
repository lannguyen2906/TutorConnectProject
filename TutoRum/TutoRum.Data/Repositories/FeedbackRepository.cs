using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.IRepositories;
using TutoRum.Data.Models;

namespace TutoRum.Data.Repositories
{
    internal class FeedbackRepository : RepositoryBase<Feedback>, IFeedbackRepository
    {
        public FeedbackRepository(IDbFactory dbFactory)
           : base(dbFactory) { }

       
        public async Task<IEnumerable<Feedback>> GetFeedbacksByLearnerIdAsync(Guid learnerId)
        {
            return await  DbContext.Feedbacks
                .Include(f => f.TutorLearnerSubject)
                .Where(f => f.TutorLearnerSubject != null && f.TutorLearnerSubject.LearnerId == learnerId)
                .ToListAsync();
        }


        public async Task<Feedback> GetFeedbackByIdAsync(int feedbackId)
        {
            return await DbContext.Feedbacks
                .Include(f => f.TutorLearnerSubject)
                .FirstOrDefaultAsync(f => f.FeedbackId == feedbackId);
        }
         public async Task<Feedback> GetFeedbackByTutorLearnerSubjectIdAsync(int tutorLearnerSubjectId)
        {
            return await DbContext.Feedbacks
                .Include(f => f.TutorLearnerSubject)
                .FirstOrDefaultAsync(f => f.TutorLearnerSubjectId == tutorLearnerSubjectId);
        }

        public async Task<List<Feedback>> GetFeedbacksForTutorAsync(Guid tutorId)
        {
            return await DbContext.Feedbacks
                .Include(f => f.TutorLearnerSubject)
                .ThenInclude(tls => tls.TutorSubject)
                .Where(f => f.TutorLearnerSubject != null &&
                             f.TutorLearnerSubject.TutorSubject.TutorId == tutorId)
                .ToListAsync();
        }

    }


}
