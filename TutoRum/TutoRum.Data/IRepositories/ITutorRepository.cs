using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Models;

namespace TutoRum.Data.IRepositories
{
    public interface ITutorRepository  : IRepository<Tutor>
    {
        Task<Tutor> GetByIdAsync(Guid id);

        Task<Tutor> GetTutorWithSubjectsAsync(Guid tutorId);
        Task SoftDeleteAsync(Guid tutorId);

        Task<Tutor> GetTutorWithSubjectsIDAsync(int subjectId);

        Task<List<Tutor>> GetAllTutorsAsync();

        Task<decimal?> GetAverageRatingForTutorAsync(Guid tutorId);
      
    }
}
