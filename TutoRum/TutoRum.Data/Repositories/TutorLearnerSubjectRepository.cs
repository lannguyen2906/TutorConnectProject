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
    public class TutorLearnerSubjectRepository : RepositoryBase<TutorLearnerSubject>, ITutorLearnerSubjectRepository    
    {
        public TutorLearnerSubjectRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }

        public async Task<List<TutorLearnerSubject>> GetAllTutorLearnerSubjectAsync()
        {

            return await DbContext.TutorLearnerSubjects
                .Include(tls => tls.TutorSubject)
                .ThenInclude(ts => ts.Subject)
                .ToListAsync();
        }

        public async Task<TutorLearnerSubject> RegisterSubjectAsync(Guid learnerId, int subjectId)
        {
           
            return await DbContext.TutorLearnerSubjects
                .FirstOrDefaultAsync(t => t.LearnerId == learnerId && t.TutorSubjectId == subjectId);
        }

        public async Task<TutorLearnerSubject> GetTutorLearnerSubjectAsyncById(int tutorLearnerSubjectId)
        {

            return await DbContext.TutorLearnerSubjects
                .Include(ts => ts.TutorSubject)
                .ThenInclude(ts => ts.Subject)
                .FirstOrDefaultAsync(t => t.TutorLearnerSubjectId == tutorLearnerSubjectId)
                ;
        }


        public async Task<List<TutorLearnerSubject>> GetSubjectsByUserIdAndRoleAsync(Guid userId, bool isLearner, bool isTutor, bool isClassroom)
        {
            // Query TutorLearnerSubjects based on learner or tutor role
            IQueryable<TutorLearnerSubject> query = DbContext.TutorLearnerSubjects
                .Include(tls => tls.TutorSubject)
                .ThenInclude(ts => ts.Subject)
                ; // Include related TutorSubject and Subject entities

            if(isClassroom)
            {
                query = query.Where(tls => tls.IsVerified == true);
            }

            if (isLearner)
            {
                query = query.Where(tls => tls.LearnerId == userId);
            }
            else if (isTutor)
            {
                query = query.Where(tls => tls.TutorSubject.TutorId == userId);
            }

            return await query.ToListAsync();
        }
    }
}
