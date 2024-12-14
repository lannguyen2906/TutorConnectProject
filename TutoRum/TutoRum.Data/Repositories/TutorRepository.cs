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
    public class TutorRepository : RepositoryBase<Tutor>, ITutorRepository
    {
        public TutorRepository(IDbFactory dbFactory)
           : base(dbFactory) { }

        public async Task<Tutor> GetByIdAsync(Guid id)
        {
            return await DbContext.Tutors
                .Include(t => t.TutorNavigation)
                .Include(t => t.Certificates)
                .Include(t => t.TutorSubjects)
                    .ThenInclude(ts => ts.Subject)
                .Include(t => t.TutorTeachingLocations)
                    .ThenInclude(tl => tl.TeachingLocation)
                .Include(t => t.Schedules)
                    .ThenInclude(s => s.tutorLearnerSubject)
                        .ThenInclude(tls => tls.TutorSubject)
                        .ThenInclude(ts => ts.Subject)
                .FirstOrDefaultAsync(t => t.TutorNavigation.Id == id);
        }

        public async Task<Tutor> GetTutorWithSubjectsAsync(Guid tutorId)
        {
            return await DbContext.Tutors
                .Include(t => t.TutorSubjects)
                .ThenInclude(ts => ts.Subject)
                .FirstOrDefaultAsync(t => t.TutorId == tutorId);
        }


        public async Task SoftDeleteAsync(Guid tutorId)
        {
            var tutor = await GetByIdAsync(tutorId);
            if (tutor != null)
            {
                tutor.Status = "Inactive";
                Update(tutor);
                await DbContext.SaveChangesAsync();
            }
        }

        public async Task<Tutor> GetTutorWithSubjectsIDAsync(int subjectId)
        {
            var tutor = await DbContext.Tutors
                .Include(t => t.TutorSubjects)
                .ThenInclude(ts => ts.Subject)
                .FirstOrDefaultAsync(t => t.TutorSubjects.Any(ts => ts.SubjectId == subjectId));

            return tutor;
        }

        public async Task<List<Tutor>> GetAllTutorsAsync()
        {
            return await DbContext.Tutors.Include(t => t.TutorNavigation).Include(t => t.TutorSubjects).ThenInclude(ts => ts.Subject).ToListAsync();
        }

        public async Task<decimal?> GetAverageRatingForTutorAsync(Guid tutorId)
        {
            var averageRating = await DbContext.TutorLearnerSubjects
                .Where(tls => tls.TutorSubject.TutorId == tutorId)
                .SelectMany(tls => tls.Feedbacks)
                .Where(f => f.Rating.HasValue)
                .DefaultIfEmpty() 
                .AverageAsync(f => f.Rating ?? 0); 

            return averageRating == 0 ? (decimal?)null : averageRating;
        }
       
    }


}
