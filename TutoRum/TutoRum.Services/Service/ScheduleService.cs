using Google.Api.Gax;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.IRepositories;
using TutoRum.Data.Models;
using TutoRum.Services.IService;
using TutoRum.Services.ViewModels;

namespace TutoRum.Services.Service
{
    public class ScheduleService : IScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AspNetUser> _userManager;



        public ScheduleService(IUnitOfWork unitOfWork, UserManager<AspNetUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }


        public async Task AddSchedulesAsync(IEnumerable<ScheduleDTO> schedules, Guid tutorId)
        {
            if (schedules != null && schedules.Any())
            {
                foreach (var scheduleDto in schedules)
                {
                    foreach (var freeTime in scheduleDto.FreeTimes)
                    {
                        var schedule = new Schedule
                        {
                            TutorId = tutorId,
                            TutorRequestID = null,
                            DayOfWeek = scheduleDto.DayOfWeek ?? 0,
                            StartTime = freeTime.StartTime ?? TimeSpan.Zero,
                            EndTime = freeTime.EndTime ?? TimeSpan.Zero,
                        };

                        _unitOfWork.schedule.Add(schedule);
                    }
                }

                await _unitOfWork.CommitAsync();
            }
        }
        public async Task UpdateSchedulesAsync(IEnumerable<ScheduleDTO> schedules, Guid tutorId)
        {
            if (schedules != null && schedules.Any())
            {
                foreach (var scheduleDto in schedules)
                {
                    foreach (var freeTime in scheduleDto.FreeTimes)
                    {

                        var dayOfWeek = scheduleDto.DayOfWeek ?? 0;
                        var startTime = freeTime.StartTime ?? TimeSpan.Zero;
                        var endTime = freeTime.EndTime ?? TimeSpan.Zero;


                        var existingSchedule = await _unitOfWork.schedule.FindScheduleAsync(
                            tutorId,
                            dayOfWeek,
                            startTime,
                            endTime
                        );

                        if (existingSchedule != null)
                        {

                            bool hasConflict = await _unitOfWork.schedule.AnyScheduleConflictsAsync(
                                tutorId,
                                dayOfWeek,
                                startTime,
                                endTime
                            );

                            if (hasConflict)
                            {
                                throw new Exception($"Xung đột lịch trình: Khung thời gian từ {startTime} đến {endTime} đã tồn tại cho ngày thứ {dayOfWeek}.");
                            }


                            existingSchedule.DayOfWeek = dayOfWeek;
                            existingSchedule.StartTime = startTime;
                            existingSchedule.EndTime = endTime;
                            existingSchedule.UpdatedDate = DateTime.UtcNow;

                            _unitOfWork.schedule.Update(existingSchedule);
                        }
                        else
                        {

                            var newSchedule = new Schedule
                            {
                                TutorId = tutorId,
                                TutorRequestID = null,
                                DayOfWeek = dayOfWeek,
                                StartTime = startTime,
                                EndTime = endTime,
                                UpdatedDate = DateTime.UtcNow
                            };

                            _unitOfWork.schedule.Add(newSchedule);
                        }
                    }
                }

                await _unitOfWork.CommitAsync();
            }
        }



        public async Task RegisterSchedulesForClass(IEnumerable<ScheduleDTO> schedules, Guid learnerID, int tutorLearnerSubjectID)
        {
            // Remove existing schedules for this tutorLearnerSubjectID to avoid duplicates
            _unitOfWork.schedule.DeleteMulti(s => s.TutorLearnerSubjectId == tutorLearnerSubjectID);

            if (schedules != null && schedules.Any())
            {
                foreach (var scheduleDto in schedules)
                {
                    foreach (var freeTime in scheduleDto.FreeTimes)
                    {
                        var schedule = new Schedule
                        {
                            TutorLearnerSubjectId = tutorLearnerSubjectID,
                            DayOfWeek = scheduleDto.DayOfWeek ?? 0,
                            StartTime = freeTime.StartTime ?? TimeSpan.Zero,
                            EndTime = freeTime.EndTime ?? TimeSpan.Zero,
                        };

                        _unitOfWork.schedule.Add(schedule);
                    }
                }

                await _unitOfWork.CommitAsync();
            }
        }


        public async Task<(bool IsSuccess, string Message)> AdjustTutorSchedulesAsync(
    Guid tutorId,
    int TutorLearnerSubjectId,
    List<TutorRequestSchedulesDTO> tutorRequestSchedules)
        {
            // Lấy danh sách lịch hiện tại của gia sư
            var existingSchedules = await _unitOfWork.schedule.GetSchedulesByTutorIdAsync(tutorId);
            var existingSchedulesList = existingSchedules.ToList();

            foreach (var requestSchedule in tutorRequestSchedules)
            {
                foreach (var requestFreeTime in requestSchedule.FreeTimes)
                {
                    // Kiểm tra xem lịch yêu cầu có trùng với lịch dạy
                    var isConflictingWithTeaching = existingSchedulesList.Any(schedule =>
                        schedule.TutorLearnerSubjectId != null &&
                        schedule.DayOfWeek == requestSchedule.DayOfWeek &&
                        schedule.StartTime <= requestFreeTime.EndTime &&
                        schedule.EndTime >= requestFreeTime.StartTime);

                    if (isConflictingWithTeaching)
                    {
                        return (false, $"Lịch yêu cầu vào ngày {requestSchedule.DayOfWeek} từ {requestFreeTime.StartTime} đến {requestFreeTime.EndTime} trùng với lịch dạy.");
                    }

                    // Kiểm tra xem lịch yêu cầu có trùng một phần với lịch rảnh
                    var overlappingFreeTime = existingSchedulesList
                        .Where(schedule =>
                            schedule.TutorLearnerSubjectId == null &&
                            schedule.DayOfWeek == requestSchedule.DayOfWeek &&
                            schedule.StartTime < requestFreeTime.EndTime &&
                            schedule.EndTime > requestFreeTime.StartTime)
                        .ToList();

                    foreach (var freeTime in overlappingFreeTime)
                    {
                        // Nếu yêu cầu bao trùm một phần thời gian rảnh
                        if (freeTime.StartTime < requestFreeTime.StartTime && freeTime.EndTime > requestFreeTime.EndTime)
                        {
                            // Chia nhỏ thời gian rảnh còn lại
                            var newFreeTimeAfter = new Schedule
                            {
                                TutorId = tutorId,
                                DayOfWeek = freeTime.DayOfWeek,
                                StartTime = requestFreeTime.EndTime,
                                EndTime = freeTime.EndTime,
                                TutorLearnerSubjectId = null // Lịch rảnh
                            };

                            if ((newFreeTimeAfter.EndTime - newFreeTimeAfter.StartTime)?.TotalMinutes > 30)
                            {
                                _unitOfWork.schedule.Add(newFreeTimeAfter);
                            }

                            freeTime.EndTime = requestFreeTime.StartTime;

                            if ((freeTime.EndTime - freeTime.StartTime)?.TotalMinutes <= 30)
                            {
                                existingSchedulesList.Remove(freeTime); // Xóa nếu < 30 phút
                            }
                        }
                        // Nếu yêu cầu trùng với phần đầu của thời gian rảnh
                        else if (freeTime.StartTime < requestFreeTime.EndTime && freeTime.EndTime <= requestFreeTime.EndTime)
                        {
                            freeTime.EndTime = requestFreeTime.StartTime;

                            if ((freeTime.EndTime - freeTime.StartTime)?.TotalMinutes <= 30)
                            {
                                existingSchedulesList.Remove(freeTime); // Xóa nếu < 30 phút
                            }
                        }
                        // Nếu yêu cầu trùng với phần cuối của thời gian rảnh
                        else if (freeTime.StartTime >= requestFreeTime.StartTime && freeTime.EndTime > requestFreeTime.StartTime)
                        {
                            freeTime.StartTime = requestFreeTime.EndTime;

                            if ((freeTime.EndTime - freeTime.StartTime)?.TotalMinutes <= 30)
                            {
                                existingSchedulesList.Remove(freeTime); // Xóa nếu < 30 phút
                            }
                        }
                    }

                    // Thêm lịch mới từ yêu cầu
                    var newTeachingSchedule = new Schedule
                    {
                        TutorId = tutorId,
                        DayOfWeek = requestSchedule.DayOfWeek,
                        StartTime = requestFreeTime.StartTime,
                        EndTime = requestFreeTime.EndTime,
                        TutorLearnerSubjectId = TutorLearnerSubjectId // Đây là lịch dạy
                    };

                    _unitOfWork.schedule.Add(newTeachingSchedule);
                }
            }

            // Lưu thay đổi
            await _unitOfWork.CommitAsync();

            return (true, "Lịch đã được cập nhật thành công.");
        }



        public async Task MergeScheduleWithFreeTime(Guid tutorId, List<TutorRequestSchedulesDTO> classCloseSchedules)
        {
            // Lấy danh sách lịch hiện tại của gia sư
            var existingSchedules = await _unitOfWork.schedule.GetSchedulesByTutorIdAsync(tutorId);
            var existingSchedulesList = existingSchedules.ToList();

            // Duyệt qua tất cả lịch cần đóng của lớp học
            foreach (var classCloseSchedule in classCloseSchedules)
            {
                foreach (var closeFreeTime in classCloseSchedule.FreeTimes)
                {
                    // Kiểm tra nếu lịch của lớp cần đóng trùng với thời gian rảnh của gia sư
                    var conflictingSchedules = existingSchedulesList
                        .Where(schedule =>
                            schedule.TutorLearnerSubjectId == null && // Chỉ kiểm tra lịch rảnh
                            schedule.DayOfWeek == classCloseSchedule.DayOfWeek && // Kiểm tra cùng ngày trong tuần
                            ((schedule.StartTime == closeFreeTime.EndTime) || // StartTime của lịch đóng == EndTime của lịch rảnh
                             (schedule.EndTime == closeFreeTime.StartTime))) // EndTime của lịch đóng == StartTime của lịch rảnh
                        .ToList();

                    foreach (var conflictingSchedule in conflictingSchedules)
                    {
                        // Nếu trùng, cập nhật lịch rảnh
                        conflictingSchedule.StartTime = closeFreeTime.StartTime; // Cập nhật lại thời gian bắt đầu
                        conflictingSchedule.EndTime = closeFreeTime.EndTime; // Cập nhật lại thời gian kết thúc

                        // Lưu lại thay đổi vào cơ sở dữ liệu
                        _unitOfWork.schedule.Update(conflictingSchedule);
                    }

                    // Thêm lịch đóng lớp mới (nếu cần thiết)
                    var newCloseSchedule = new Schedule
                    {
                        TutorId = tutorId,
                        DayOfWeek = classCloseSchedule.DayOfWeek,
                        StartTime = closeFreeTime.StartTime,
                        EndTime = closeFreeTime.EndTime,
                        TutorLearnerSubjectId = null // Đặt TutorLearnerSubjectId thành null để chỉ là lịch rảnh
                    };

                    _unitOfWork.schedule.Add(newCloseSchedule);
                }
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            await _unitOfWork.CommitAsync();
        }





        public async Task<List<UpdateScheduleDTO>> AdjustTutorFreeTime(List<UpdateScheduleDTO> tutorFreeSchedules, List<UpdateScheduleDTO> learnerSchedules)
        {
            var updatedSchedules = new List<UpdateScheduleDTO>();

            foreach (var tutorSchedule in tutorFreeSchedules)
            {
                var updatedFreeTimes = new List<FreeTimeDTO>();

                foreach (var freeTime in tutorSchedule.FreeTimes)
                {
                    var remainingTime = new List<FreeTimeDTO> { freeTime };

                    // Duyệt qua từng Schedule của học viên
                    foreach (var learnerSchedule in learnerSchedules)
                    {
                        // Chỉ xử lý nếu DayOfWeek trùng nhau
                        if (tutorSchedule.DayOfWeek == learnerSchedule.DayOfWeek)
                        {
                            foreach (var learnerFreeTime in learnerSchedule.FreeTimes)
                            {
                                var tempRemainingTime = new List<FreeTimeDTO>();

                                foreach (var remaining in remainingTime)
                                {
                                    // Trường hợp 1: Lịch của học viên chiếm toàn bộ khoảng thời gian rảnh của gia sư
                                    if (learnerFreeTime.StartTime <= remaining.StartTime && learnerFreeTime.EndTime >= remaining.EndTime)
                                    {
                                        // Khoảng thời gian rảnh bị chiếm hết, bỏ qua nó.
                                        continue;
                                    }

                                    // Trường hợp 2: Lịch của học viên nằm giữa khoảng thời gian rảnh
                                    if (learnerFreeTime.StartTime >= remaining.StartTime && learnerFreeTime.EndTime <= remaining.EndTime)
                                    {
                                        // Chia khoảng rảnh thành hai phần quanh lịch học của học viên
                                        var beforeTime = new FreeTimeDTO
                                        {
                                            StartTime = remaining.StartTime,
                                            EndTime = learnerFreeTime.StartTime
                                        };

                                        var afterTime = new FreeTimeDTO
                                        {
                                            StartTime = learnerFreeTime.EndTime,
                                            EndTime = remaining.EndTime
                                        };

                                        // Chỉ thêm khoảng thời gian nếu khoảng cách >= 30 phút
                                        if ((beforeTime.EndTime.HasValue && beforeTime.StartTime.HasValue) &&
                                            (beforeTime.EndTime.Value - beforeTime.StartTime.Value).TotalMinutes > 30)
                                        {
                                            tempRemainingTime.Add(beforeTime);
                                        }
                                        if ((afterTime.EndTime.HasValue && afterTime.StartTime.HasValue) &&
                                            (afterTime.EndTime.Value - afterTime.StartTime.Value).TotalMinutes > 30)
                                        {
                                            tempRemainingTime.Add(afterTime);
                                        }
                                    }
                                    else
                                    {
                                        // Không trùng lặp, giữ nguyên khoảng thời gian rảnh

                                        tempRemainingTime.Add(remaining);
                                    }
                                }

                                remainingTime = tempRemainingTime;
                            }
                        }
                    }

                    // Sau khi xử lý hết, thêm khoảng thời gian còn lại vào danh sách nếu >= 30 phút
                    updatedFreeTimes.AddRange(remainingTime.Where(time =>
                        time.EndTime.HasValue && time.StartTime.HasValue &&
                        (time.EndTime.Value - time.StartTime.Value).TotalMinutes > 30));
                }

                updatedSchedules.Add(new UpdateScheduleDTO
                {
                    DayOfWeek = tutorSchedule.DayOfWeek,
                    FreeTimes = updatedFreeTimes
                });
            }

            return updatedSchedules;
        }

        public async Task UpdateNewSchedule(Guid tutorId, List<UpdateScheduleDTO> newSchedules)
        {
            // Xóa tất cả các lịch trình cũ của gia sư trước khi cập nhật bằng cách sử dụng GetMulti
            var schedulesToDelete = _unitOfWork.schedule
                .GetMulti(s => s.TutorId == tutorId && s.TutorLearnerSubjectId == null); // Using GetMulti instead of GetMultiAsQueryable



            foreach (var schedule in schedulesToDelete)
            {
                _unitOfWork.schedule.Delete(schedule.ScheduleId);
            }



            // Thực hiện thêm các lịch trình mới vào cơ sở dữ liệu
            foreach (var updatedSchedule in newSchedules)
            {
                foreach (var freeTime in updatedSchedule.FreeTimes)
                {
                    _unitOfWork.schedule.Add(new Schedule
                    {
                        TutorId = tutorId,
                        DayOfWeek = updatedSchedule.DayOfWeek,
                        StartTime = freeTime.StartTime,
                        EndTime = freeTime.EndTime
                    });
                }
            }

            // Lưu tất cả các thay đổi vào cơ sở dữ liệu
            await _unitOfWork.CommitAsync();
        }


        public async Task<List<ScheduleGroupDTO>> GetSchedulesByTutorIdAsync(Guid tutorId)
        {
            // Truy vấn lịch của gia sư
            var schedules = await _unitOfWork.schedule.GetMultiAsQueryable(
                s => s.TutorId == tutorId || s.tutorLearnerSubject.TutorSubject.TutorId == tutorId,
                includes: new[] { "tutorLearnerSubject.TutorSubject.Subject" }
            ).ToListAsync();

            // Chuyển đổi dữ liệu và nhóm theo DayOfWeek
            var groupedSchedules = schedules
                .GroupBy(s => s.DayOfWeek)
                .Select(group => new ScheduleGroupDTO
                {
                    DayOfWeek = group.Key,
                    Schedules = group
                        .OrderBy(s => s.StartTime)
                        .Select(s => new ScheduleViewDTO
                        {
                            Id = s.ScheduleId,
                            DayOfWeek = s.DayOfWeek,
                            FreeTimes = new List<FreeTimeDTO>
                            {
                        new FreeTimeDTO
                        {
                            StartTime = s.StartTime,
                            EndTime = s.EndTime
                        }
                            },
                            SubjectNames = s.TutorLearnerSubjectId == null
                                ? "Lịch rảnh chưa có lớp"
                                : s.tutorLearnerSubject.TutorSubject.Subject.SubjectName,
                            TutorLearnerSubjectId = s.TutorLearnerSubjectId
                        })
                        .ToList()
                })
                .OrderBy(g => g.DayOfWeek) // Sắp xếp các nhóm theo DayOfWeek
                .ToList();

            return groupedSchedules;
        }


        public async Task<List<Schedule>> GetConflictingSchedulesAsync(
    Guid tutorId,
    int dayOfWeek,
    TimeSpan startTime,
    TimeSpan endTime)
        {
            // Query the existing schedules to find conflicts based on the day and time range
            var conflictingSchedules = _unitOfWork.schedule.GetMulti(
                s => s.TutorId == tutorId &&
                     s.DayOfWeek == dayOfWeek &&
                     ((s.StartTime < endTime && s.EndTime > startTime) ||
                      (s.StartTime >= startTime && s.StartTime < endTime) ||
                      (s.EndTime > startTime && s.EndTime <= endTime))
            ).ToList();

            return conflictingSchedules;
        }

        public async Task AddSchedule(Guid tutorId, AddScheduleDTO newSchedule)
        {
            // Thực hiện thêm các lịch trình mới vào cơ sở dữ liệu
            foreach (var freeTime in newSchedule.FreeTimes)
            {
                _unitOfWork.schedule.Add(new Schedule
                {
                    TutorId = tutorId,
                    DayOfWeek = newSchedule.DayOfWeek,
                    StartTime = freeTime.StartTime,
                    EndTime = freeTime.EndTime,
                    CreatedBy = tutorId,
                    CreatedDate = DateTime.UtcNow

                });
            }

            // Lưu tất cả các thay đổi vào cơ sở dữ liệu
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteSchedule(Guid tutorId, DeleteScheduleDTO scheduleToDelete)
        {
            // Tìm và xóa lịch trình trong cơ sở dữ liệu
            foreach (var freeTime in scheduleToDelete.FreeTimes)
            {
                var schedule =  _unitOfWork.schedule.GetSingleById(scheduleToDelete.ScheduleId);

                if (schedule != null)
                {
                    _unitOfWork.schedule.Delete(schedule.ScheduleId);
                }
            }

            // Lưu tất cả các thay đổi vào cơ sở dữ liệu
            await _unitOfWork.CommitAsync();
        }

        public async Task UpdateSchedule(Guid tutorId, UpdateScheduleDTO updatedSchedule)
        {
            // Tìm và cập nhật các lịch trình có sẵn
            foreach (var freeTime in updatedSchedule.FreeTimes)
            {
                var schedule = _unitOfWork.schedule.GetSingleById(updatedSchedule.Id);

                if (schedule != null)
                {
                    // Cập nhật lịch trình
                    schedule.EndTime = freeTime.EndTime;
                    _unitOfWork.schedule.Update(schedule);
                }
                else
                {
                    // Nếu không có lịch trình cũ, thêm mới
                    _unitOfWork.schedule.Add(new Schedule
                    {
                        TutorId = tutorId,
                        DayOfWeek = updatedSchedule.DayOfWeek,
                        StartTime = freeTime.StartTime,
                        TutorLearnerSubjectId = updatedSchedule.TutorLearnerSubjectID,
                        EndTime = freeTime.EndTime
                    });
                }
            }

            // Lưu tất cả các thay đổi vào cơ sở dữ liệu
            await _unitOfWork.CommitAsync();
        }


    }

}

