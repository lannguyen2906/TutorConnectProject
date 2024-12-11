using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Models;

namespace TutoRum.Data.IRepositories
{
    public interface ITutorTeachingLocationsRepository : IRepository<TutorTeachingLocations>
    {
        Task<IEnumerable<TutorTeachingLocations>> GetAllByTutorIdAsync(Guid tutorId);

        Task<bool> ExistsAsync(Guid tutorId, int teachingLocationId);
    }
}
