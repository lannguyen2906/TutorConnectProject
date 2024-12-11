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
    public class TeachingLocationRepository : RepositoryBase<TeachingLocation>, ITeachingLocationRepository
    {
        public TeachingLocationRepository(IDbFactory dbFactory)
            : base(dbFactory) { }

        public async Task<TeachingLocation?> FindAsync(string? cityId, string? districtId)
        {
            return await DbContext.TeachingLocations
                .FirstOrDefaultAsync(tl => tl.CityId == cityId && tl.DistrictId == districtId);
        }
    }
}
