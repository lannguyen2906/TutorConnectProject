using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Services.ViewModels;

namespace TutoRum.Services.IService
{
    public interface ITeachingLocationsService
    {
        Task AddTeachingLocationsAsync(IEnumerable<AddTeachingLocationViewDTO> teachingLocations, Guid tutorId);
        Task DeleteTeachingLocationAsync(Guid tutorId, int[] locationIds);
    }
}
