using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Models;

namespace TutoRum.Data.IRepositories
{
    public  interface ITutorRequestRepository : IRepository<TutorRequest>
    {
        Task<TutorRequest> GetTutorRequestByIdAsync(int id);
        Task<IEnumerable<TutorRequest>> GetAllTutorRequestsAsync();
        Task<TutorRequest> GetTutorRequestByUserIdAsync(Guid userId);
    }
}
