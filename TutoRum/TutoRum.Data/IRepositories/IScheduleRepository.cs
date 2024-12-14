using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Models;

namespace TutoRum.Data.IRepositories
{
    public interface IScheduleRepository : IRepository<Schedule>
    {
        Task<Schedule> GetScheduleByIdAsync(int id);
        Task<IEnumerable<Schedule>> GetSchedulesByTutorIdAsync(Guid tutorId);
       

        Task<int> GetCurrentWeekSessionsCountAsync(Guid tutorId);
        IEnumerable<Schedule> GetSchedulesByDayOfWeek(List<Schedule> schedules, DayOfWeek day);

        Task<Schedule> FindScheduleAsync(Guid tutorId, int dayOfWeek, TimeSpan startTime, TimeSpan endTime);
        Task<bool> AnyScheduleConflictsAsync(Guid tutorId, int dayOfWeek, TimeSpan startTime, TimeSpan endTime);
        void DetachSchedule(Schedule schedule);
    }
}
