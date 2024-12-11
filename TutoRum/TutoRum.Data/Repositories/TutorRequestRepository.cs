using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.IRepositories;
using TutoRum.Data.Models;

using Microsoft.EntityFrameworkCore;

namespace TutoRum.Data.Repositories
{
    public class TutorRequestRepository : RepositoryBase<TutorRequest>, ITutorRequestRepository
    {
        public TutorRequestRepository(IDbFactory dbFactory)
          : base(dbFactory) { }

       

        public async Task<IEnumerable<TutorRequest>> GetAllTutorRequestsAsync()
        {
            return await DbContext.TutorRequest
                .Include(tr => tr.AspNetUser)
                .Include(tr => tr.TutorQualification)
                .ToListAsync();
        }

        public async Task<TutorRequest> GetTutorRequestByIdAsync(int id)
        {
            return await DbContext.TutorRequest
                .Include(tr => tr.AspNetUser)
                .Include(tr => tr.TutorQualification)
                          
                .SingleOrDefaultAsync(tr => tr.Id == id && !tr.IsDelete);
        }

        public async Task<TutorRequest> GetTutorRequestByUserIdAsync(Guid userId)
        {
            return await DbContext.TutorRequest.FirstOrDefaultAsync(tr => tr.AspNetUserId == userId);
        }
    }
}
