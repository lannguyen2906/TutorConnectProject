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
    public class TutorTeachingLocationsRepository : RepositoryBase<TutorTeachingLocations>, ITutorTeachingLocationsRepository
    {
        public TutorTeachingLocationsRepository(IDbFactory dbFactory)
            : base(dbFactory) { }


        public async Task<IEnumerable<TutorTeachingLocations>> GetAllByTutorIdAsync(Guid tutorId)
        {
            return await Task.FromResult(GetMulti(t => t.TutorId == tutorId));
        }

        public async Task<bool> ExistsAsync(Guid tutorId, int teachingLocationId) 
        {
           
            return await DbContext.TutorTeachingLocations
                .AnyAsync(ttl => ttl.TutorId == tutorId && ttl.TeachingLocationId == teachingLocationId);
        }
    }
}
