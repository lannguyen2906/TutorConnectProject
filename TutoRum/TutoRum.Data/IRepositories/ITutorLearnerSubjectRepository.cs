using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Models;

namespace TutoRum.Data.IRepositories
{
    public interface ITutorLearnerSubjectRepository : IRepository<TutorLearnerSubject>  
    {
        Task<TutorLearnerSubject> RegisterSubjectAsync(Guid learnerId, int subjectId);
        Task<TutorLearnerSubject> GetTutorLearnerSubjectAsyncById(int tutorLearnerSubjectId);
        Task<List<TutorLearnerSubject>> GetSubjectsByUserIdAndRoleAsync(Guid userId, bool isLearner, bool isTutor, bool isVerified);
        Task<List<TutorLearnerSubject>> GetAllTutorLearnerSubjectAsync();
    }
}
