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
    public class ScheduleRepository : RepositoryBase<Schedule>, IScheduleRepository
    {
        public ScheduleRepository(IDbFactory dbFactory)
          : base(dbFactory) { }

        public async Task<Schedule> GetScheduleByIdAsync(int id)
        {
            return await DbContext.Schedule
                .Where(schedule => schedule.ScheduleId == id)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Schedule>> GetSchedulesByTutorIdAsync(Guid tutorId)
        {
            return await DbContext.Schedule.Where(s => s.TutorId == tutorId).ToListAsync();
        }


        public async Task<int> GetCurrentWeekSessionsCountAsync(Guid tutorId)
        {
            return await DbContext.Schedule
                .Where(s => s.TutorId == tutorId)
                .GroupBy(s => s.DayOfWeek)
                .Select(g => g.Count())
                .SumAsync();
        }

       public IEnumerable<Schedule> GetSchedulesByDayOfWeek(List<Schedule> schedules, DayOfWeek day)
    {
        return schedules.Where(s => s.DayOfWeek == (int)day)
                        .OrderBy(s => s.StartTime)
                        .ToList();
    }
        public async Task<Schedule> FindScheduleAsync(Guid tutorId, int dayOfWeek, TimeSpan startTime, TimeSpan endTime)
        {
            return await DbContext.Schedule
                .FirstOrDefaultAsync(s =>
                    s.TutorId == tutorId &&
                    s.DayOfWeek == dayOfWeek &&

                    (s.StartTime < endTime && s.EndTime > startTime));
        }
        public async Task<bool> AnyScheduleConflictsAsync(Guid tutorId, int dayOfWeek, TimeSpan startTime, TimeSpan endTime)
        {
            return await DbContext.Schedule
                .AnyAsync(s =>
                    s.TutorId == tutorId &&
                    s.DayOfWeek == dayOfWeek &&
                    (s.StartTime < endTime && s.EndTime > startTime));
        }

        public void DetachSchedule(Schedule schedule)
        {
            var entry = DbContext.Entry(schedule);
            if (entry.State == EntityState.Detached)
            {
                return;
            }
            entry.State = EntityState.Detached;
        }



    }

}
