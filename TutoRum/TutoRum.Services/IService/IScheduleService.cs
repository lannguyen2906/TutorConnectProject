using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Models;
using TutoRum.Services.ViewModels;

namespace TutoRum.Services.IService
{
   public interface IScheduleService
    {
        // Task<IEnumerable<ScheduleDTO>> GetTutorAvailableTimesAsync(Guid tutorId);

        Task AddSchedulesAsync(IEnumerable<ScheduleDTO> schedules, Guid tutorId);
        Task RegisterSchedulesForClass(IEnumerable<ScheduleDTO> schedules, Guid learnerID, int tutorLearnerSubjectID);
        Task<List<UpdateScheduleDTO>> AdjustTutorFreeTime(List<UpdateScheduleDTO> tutorFreeSchedules, List<UpdateScheduleDTO> learnerSchedules);

        Task UpdateNewSchedule(Guid tutorId, List<UpdateScheduleDTO> newSchedules);
        Task<List<ScheduleGroupDTO>> GetSchedulesByTutorIdAsync(Guid tutorId);
        Task<(bool IsSuccess, string Message)> AdjustTutorSchedulesAsync(
    Guid tutorId,
    int TutorLearnerSubjectId,
    List<TutorRequestSchedulesDTO> tutorRequestSchedules);

        Task MergeScheduleWithFreeTime(Guid tutorId, List<TutorRequestSchedulesDTO> classCloseSchedules);
        Task AddSchedule(Guid tutorId, AddScheduleDTO newSchedule);
        Task DeleteSchedule(Guid tutorId, DeleteScheduleDTO scheduleToDelete);
        Task UpdateSchedule(Guid tutorId, UpdateScheduleDTO updatedSchedule);
    }
}
 