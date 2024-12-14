using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.IRepositories;
using TutoRum.Data.Models;
using TutoRum.Services.Service;
using TutoRum.Services.ViewModels;

namespace TutoRum.UnitTests.ServiceUnitTest
{
    [TestFixture]
    public class ScheduleServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<UserManager<AspNetUser>> _mockUserManager;
        private ScheduleService _scheduleService;
        private Mock<IScheduleRepository> _mockScheduleRepository; // Use IScheduleRepository instead of IRepository<Schedule>

        [SetUp]
        public void SetUp()
        {
            // Mock the IUnitOfWork and UserManager
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUserManager = new Mock<UserManager<AspNetUser>>(
                new Mock<IUserStore<AspNetUser>>().Object,
                null, null, null, null, null, null, null, null
            );

            // Mock methods for the IScheduleRepository
            _mockScheduleRepository = new Mock<IScheduleRepository>(); // Mock IScheduleRepository
            _mockUnitOfWork.Setup(uow => uow.schedule).Returns(_mockScheduleRepository.Object); // Use the correct property

            // Setting up other methods you need to mock
            _mockUnitOfWork.Setup(st => st.schedule.Add(It.IsAny<Schedule>()));
            _mockUnitOfWork.Setup(st => st.schedule.Update(It.IsAny<Schedule>()));
            _mockUnitOfWork.Setup(u => u.schedule.DeleteMulti(It.IsAny<Expression<Func<Schedule, bool>>>()));

            // Instantiate the ScheduleService with the mock objects
            _scheduleService = new ScheduleService(_mockUnitOfWork.Object, _mockUserManager.Object);
        }


        [Test]
        public async Task AddSchedulesAsync_ShouldAddSchedules_WhenSchedulesAreValid()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var schedules = new List<ScheduleDTO>
            {
                new ScheduleDTO
                {
                    DayOfWeek = 1, // Monday
                    FreeTimes = new List<FreeTimeDTO>
                    {
                        new FreeTimeDTO { StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(10) },
                        new FreeTimeDTO { StartTime = TimeSpan.FromHours(14), EndTime = TimeSpan.FromHours(15) }
                    }
                }
            };

            // Act
            await _scheduleService.AddSchedulesAsync(schedules, tutorId);

            // Assert
            _mockUnitOfWork.Verify(u => u.schedule.Add(It.IsAny<Schedule>()), Times.Exactly(2), "Schedules should be added twice.");
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once, "CommitAsync should be called once.");
        }

        [Test]
        public async Task AddSchedulesAsync_ShouldNotAddSchedules_WhenSchedulesAreEmpty()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var schedules = new List<ScheduleDTO>(); // Empty schedule list

            // Act
            await _scheduleService.AddSchedulesAsync(schedules, tutorId);

            // Assert
            _mockUnitOfWork.Verify(u => u.schedule.Add(It.IsAny<Schedule>()), Times.Never, "No schedules should be added.");
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never, "CommitAsync should not be called.");
        }

        [Test]
        public async Task UpdateSchedulesAsync_ShouldUpdateSchedule_WhenExistingScheduleFound()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var schedules = new List<ScheduleDTO>
            {
                new ScheduleDTO
                {
                    DayOfWeek = 1,
                    FreeTimes = new List<FreeTimeDTO>
                    {
                        new FreeTimeDTO { StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(10) }
                    }
                }
            };

            var existingSchedule = new Schedule
            {
                TutorId = tutorId,
                DayOfWeek = 1,
                StartTime = TimeSpan.FromHours(9),
                EndTime = TimeSpan.FromHours(10),
                UpdatedDate = DateTime.UtcNow
            };

            _mockUnitOfWork.Setup(u => u.schedule.FindScheduleAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>()))
                .ReturnsAsync(existingSchedule);

            _mockUnitOfWork.Setup(u => u.schedule.AnyScheduleConflictsAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>()))
                .ReturnsAsync(false); // No conflict

            // Act
            await _scheduleService.UpdateSchedulesAsync(schedules, tutorId);

            // Assert
            _mockUnitOfWork.Verify(u => u.schedule.Update(It.IsAny<Schedule>()), Times.Once, "The existing schedule should be updated.");
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once, "CommitAsync should be called once.");
        }

        [Test]
        public async Task UpdateSchedulesAsync_ShouldAddSchedule_WhenNoExistingScheduleFound()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var schedules = new List<ScheduleDTO>
            {
                new ScheduleDTO
                {
                    DayOfWeek = 1,
                    FreeTimes = new List<FreeTimeDTO>
                    {
                        new FreeTimeDTO { StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(10) }
                    }
                }
            };

            _mockUnitOfWork.Setup(u => u.schedule.FindScheduleAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>()))
                .ReturnsAsync((Schedule)null); // No existing schedule

            // Act
            await _scheduleService.UpdateSchedulesAsync(schedules, tutorId);

            // Assert
            _mockUnitOfWork.Verify(u => u.schedule.Add(It.IsAny<Schedule>()), Times.Once, "A new schedule should be added.");
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once, "CommitAsync should be called once.");
        }

        [Test]
        public async Task UpdateSchedulesAsync_ShouldThrowException_WhenScheduleConflictExists()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var schedules = new List<ScheduleDTO>
            {
                new ScheduleDTO
                {
                    DayOfWeek = 1,
                    FreeTimes = new List<FreeTimeDTO>
                    {
                        new FreeTimeDTO { StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(10) }
                    }
                }
            };

            var existingSchedule = new Schedule
            {
                TutorId = tutorId,
                DayOfWeek = 1,
                StartTime = TimeSpan.FromHours(9),
                EndTime = TimeSpan.FromHours(10)
            };

            _mockUnitOfWork.Setup(u => u.schedule.FindScheduleAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>()))
                .ReturnsAsync(existingSchedule);

            _mockUnitOfWork.Setup(u => u.schedule.AnyScheduleConflictsAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>()))
                .ReturnsAsync(true); // Conflict exists

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _scheduleService.UpdateSchedulesAsync(schedules, tutorId));
            Assert.AreEqual("Xung đột lịch trình: Khung thời gian từ 09:00:00 đến 10:00:00 đã tồn tại cho ngày thứ 1.", ex.Message);
        }



        #region RegisterSchedulesForClass Tests

        [Test]
        public async Task RegisterSchedulesForClass_ShouldDeleteExistingSchedulesAndAddNewSchedules()
        {
            // Arrange
            var tutorLearnerSubjectID = 1;
            var schedules = new List<ScheduleDTO>
            {
                new ScheduleDTO
                {
                    DayOfWeek = 1,
                    FreeTimes = new List<FreeTimeDTO>
                    {
                        new FreeTimeDTO { StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(10) }
                    }
                }
            };

            // Act
            await _scheduleService.RegisterSchedulesForClass(schedules, Guid.NewGuid(), tutorLearnerSubjectID);

            // Assert
            _mockUnitOfWork.Verify(u => u.schedule.Add(It.IsAny<Schedule>()), Times.Once, "New schedules should be added.");
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once, "CommitAsync should be called once.");
        }

        #endregion

        [Test]
        public async Task AdjustTutorSchedulesAsync_ShouldReturnError_WhenScheduleConflictsWithTeaching()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var tutorLearnerSubjectId = 1;
            var tutorRequestSchedules = new List<TutorRequestSchedulesDTO>
        {
            new TutorRequestSchedulesDTO
            {
                DayOfWeek = 1,
                FreeTimes = new List<FreeTimeDTO>
                {
                    new FreeTimeDTO { StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(10) }
                }
            }
        };

            var existingSchedules = new List<Schedule>
        {
            new Schedule
            {
                TutorLearnerSubjectId = tutorLearnerSubjectId,
                DayOfWeek = 1,
                StartTime = TimeSpan.FromHours(9),
                EndTime = TimeSpan.FromHours(10)
            }
        };

            _mockUnitOfWork.Setup(u => u.schedule.GetSchedulesByTutorIdAsync(tutorId))
                .ReturnsAsync(existingSchedules);

            // Act
            var result = await _scheduleService.AdjustTutorSchedulesAsync(tutorId, tutorLearnerSubjectId, tutorRequestSchedules);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Lịch yêu cầu vào ngày 1 từ 09:00:00 đến 10:00:00 trùng với lịch dạy.", result.Message);
        }


        #region MergeScheduleWithFreeTime Tests

        [Test]
        public async Task MergeScheduleWithFreeTime_ShouldMergeSchedules_WhenNoConflicts()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var classCloseSchedules = new List<TutorRequestSchedulesDTO>
    {
        new TutorRequestSchedulesDTO
        {
            DayOfWeek = 1,
            FreeTimes = new List<FreeTimeDTO>
            {
                new FreeTimeDTO { StartTime = TimeSpan.FromHours(14), EndTime = TimeSpan.FromHours(15) }
            }
        }
    };

            var existingSchedules = new List<Schedule>
    {
        new Schedule
        {
            TutorLearnerSubjectId = null,
            DayOfWeek = 1,
            StartTime = TimeSpan.FromHours(13),
            EndTime = TimeSpan.FromHours(14)
        }
    };

            _mockUnitOfWork.Setup(u => u.schedule.GetSchedulesByTutorIdAsync(tutorId))
                .ReturnsAsync(existingSchedules);

            // Act
            await _scheduleService.MergeScheduleWithFreeTime(tutorId, classCloseSchedules);

            // Assert
            _mockUnitOfWork.Verify(u => u.schedule.Add(It.IsAny<Schedule>()), Times.Once, "A new class closing schedule should be added.");
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once, "CommitAsync should be called once.");
        }

        [Test]
        public async Task MergeScheduleWithFreeTime_ShouldUpdateFreeTime_WhenConflictDetected()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var classCloseSchedules = new List<TutorRequestSchedulesDTO>
    {
        new TutorRequestSchedulesDTO
        {
            DayOfWeek = 1,
            FreeTimes = new List<FreeTimeDTO>
            {
                new FreeTimeDTO { StartTime = TimeSpan.FromHours(14), EndTime = TimeSpan.FromHours(15) }
            }
        }
    };

            var existingSchedules = new List<Schedule>
    {
        new Schedule
        {
            TutorLearnerSubjectId = null,
            DayOfWeek = 1,
            StartTime = TimeSpan.FromHours(14),
            EndTime = TimeSpan.FromHours(15)
        }
    };

            _mockUnitOfWork.Setup(u => u.schedule.GetSchedulesByTutorIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingSchedules);

            // Act
            await _scheduleService.MergeScheduleWithFreeTime(tutorId, classCloseSchedules);

            // Assert
        }

        #endregion
        [Test]
        public async Task AdjustTutorFreeTime_ShouldReturnUnchangedSchedules_WhenNoConflicts()
        {
            // Arrange
            var tutorSchedules = new List<UpdateScheduleDTO>
            {
                new UpdateScheduleDTO
                {
                    DayOfWeek = 1,
                    FreeTimes = new List<FreeTimeDTO>
                    {
                        new FreeTimeDTO { StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(11) }
                    }
                }
            };

            var learnerSchedules = new List<UpdateScheduleDTO>
            {
                new UpdateScheduleDTO
                {
                    DayOfWeek = 2, // Different day
                    FreeTimes = new List<FreeTimeDTO>
                    {
                        new FreeTimeDTO { StartTime = TimeSpan.FromHours(10), EndTime = TimeSpan.FromHours(11) }
                    }
                }
            };

            // Act
            var result = await _scheduleService.AdjustTutorFreeTime(tutorSchedules, learnerSchedules);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].DayOfWeek);
            Assert.AreEqual(1, result[0].FreeTimes.Count);
            Assert.AreEqual(TimeSpan.FromHours(9), result[0].FreeTimes[0].StartTime);
            Assert.AreEqual(TimeSpan.FromHours(11), result[0].FreeTimes[0].EndTime);
        }

        [Test]
        public async Task AdjustTutorFreeTime_ShouldRemoveFreeTime_WhenFullyOverlapped()
        {
            // Arrange
            var tutorSchedules = new List<UpdateScheduleDTO>
            {
                new UpdateScheduleDTO
                {
                    DayOfWeek = 1,
                    FreeTimes = new List<FreeTimeDTO>
                    {
                        new FreeTimeDTO { StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(11) }
                    }
                }
            };

            var learnerSchedules = new List<UpdateScheduleDTO>
            {
                new UpdateScheduleDTO
                {
                    DayOfWeek = 1, // Same day
                    FreeTimes = new List<FreeTimeDTO>
                    {
                        new FreeTimeDTO { StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(11) }
                    }
                }
            };

            // Act
            var result = await _scheduleService.AdjustTutorFreeTime(tutorSchedules, learnerSchedules);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].DayOfWeek);
            Assert.IsEmpty(result[0].FreeTimes); // All free time should be removed
        }

        [Test]
        public async Task AdjustTutorFreeTime_ShouldSplitFreeTime_WhenPartiallyOverlapped()
        {
            // Arrange
            var tutorSchedules = new List<UpdateScheduleDTO>
            {
                new UpdateScheduleDTO
                {
                    DayOfWeek = 1,
                    FreeTimes = new List<FreeTimeDTO>
                    {
                        new FreeTimeDTO { StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(12) }
                    }
                }
            };

            var learnerSchedules = new List<UpdateScheduleDTO>
            {
                new UpdateScheduleDTO
                {
                    DayOfWeek = 1, // Same day
                    FreeTimes = new List<FreeTimeDTO>
                    {
                        new FreeTimeDTO { StartTime = TimeSpan.FromHours(10), EndTime = TimeSpan.FromHours(11) }
                    }
                }
            };

            // Act
            var result = await _scheduleService.AdjustTutorFreeTime(tutorSchedules, learnerSchedules);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].DayOfWeek);
            Assert.AreEqual(2, result[0].FreeTimes.Count);

            Assert.AreEqual(TimeSpan.FromHours(9), result[0].FreeTimes[0].StartTime);
            Assert.AreEqual(TimeSpan.FromHours(10), result[0].FreeTimes[0].EndTime);

            Assert.AreEqual(TimeSpan.FromHours(11), result[0].FreeTimes[1].StartTime);
            Assert.AreEqual(TimeSpan.FromHours(12), result[0].FreeTimes[1].EndTime);
        }

        [Test]
        public async Task AdjustTutorFreeTime_ShouldExcludeShortFreeTimes_WhenDurationIsLessThan30Minutes()
        {
            // Arrange
            var tutorSchedules = new List<UpdateScheduleDTO>
            {
                new UpdateScheduleDTO
                {
                    DayOfWeek = 1,
                    FreeTimes = new List<FreeTimeDTO>
                    {
                        new FreeTimeDTO { StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(11) }
                    }
                }
            };

            var learnerSchedules = new List<UpdateScheduleDTO>
            {
                new UpdateScheduleDTO
                {
                    DayOfWeek = 1, // Same day
                    FreeTimes = new List<FreeTimeDTO>
                    {
                        new FreeTimeDTO { StartTime = TimeSpan.FromHours(10), EndTime = TimeSpan.FromHours(10).Add(TimeSpan.FromMinutes(40)) }
                    }
                }
            };

            // Act
            var result = await _scheduleService.AdjustTutorFreeTime(tutorSchedules, learnerSchedules);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].DayOfWeek);
            Assert.AreEqual(1, result[0].FreeTimes.Count); // Only one slot >= 30 minutes should remain

            Assert.AreEqual(TimeSpan.FromHours(9), result[0].FreeTimes[0].StartTime);
            Assert.AreEqual(TimeSpan.FromHours(10), result[0].FreeTimes[0].EndTime);
        }


        [Test]
        public async Task UpdateNewSchedule_ShouldDeleteOldSchedulesAndAddNewSchedules()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var oldSchedules = new List<Schedule>
    {
        new Schedule { ScheduleId = 1, TutorId = tutorId, TutorLearnerSubjectId = null }
    };

            var newSchedules = new List<UpdateScheduleDTO>
    {
        new UpdateScheduleDTO
        {
            DayOfWeek = 1,
            FreeTimes = new List<FreeTimeDTO>
            {
                new FreeTimeDTO { StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(11) }
            }
        }
    };

            // Setup mock to return old schedules
            _mockUnitOfWork.Setup(u => u.schedule.GetMultiAsQueryable(It.IsAny<Expression<Func<Schedule, bool>>>(), It.IsAny<string[]>()))
                .Returns(oldSchedules.AsQueryable());

            // Setup mock to handle Delete method

            // Act
            await _scheduleService.UpdateNewSchedule(tutorId, newSchedules);

            // Assert
            _mockUnitOfWork.Verify(u => u.schedule.Add(It.IsAny<Schedule>()), Times.Once); // Verify Add is called for new schedule
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once); // Verify CommitAsync is called once
        }

        [Test]
        public async Task UpdateNewSchedsule_ShouldDeleteOldSchedulesAndAddNewSchedules()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var oldSchedules = new List<Schedule>
            {
                new Schedule { ScheduleId = 1, TutorId = tutorId, TutorLearnerSubjectId = null }
            };

            var newSchedules = new List<UpdateScheduleDTO>
            {
                new UpdateScheduleDTO
                {
                    DayOfWeek = 1,
                    FreeTimes = new List<FreeTimeDTO>
                    {
                        new FreeTimeDTO { StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(11) }
                    }
                }
            };

            // Setup mock to return old schedules
            _mockScheduleRepository.Setup(repo => repo.GetMulti(It.IsAny<Expression<Func<Schedule, bool>>>(), It.IsAny<string[]>()))
                .Returns(oldSchedules);

            // Act
            await _scheduleService.UpdateNewSchedule(tutorId, newSchedules);

            // Assert
            _mockScheduleRepository.Verify(repo => repo.Add(It.IsAny<Schedule>()), Times.Once); // Verify Add is called for new schedule
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once); // Verify CommitAsync is called once
        }

       

      


        [Test]
        public async Task AdjustTutorSchedulesAsync_ShouldReturnError_WhenConflictWithTeachingSchedule()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var tutorLearnerSubjectId = 1;
            var tutorRequestSchedules = new List<TutorRequestSchedulesDTO>
        {
            new TutorRequestSchedulesDTO
            {
                DayOfWeek = 1, // Monday
                FreeTimes = new List<FreeTimeDTO>
                {
                    new FreeTimeDTO
                    {
                        StartTime = TimeSpan.FromHours(9),
                        EndTime = TimeSpan.FromHours(10)
                    }
                }
            }
        };

            var existingSchedules = new List<Schedule>
        {
            new Schedule
            {
                TutorId = tutorId,
                DayOfWeek = 1,
                StartTime = TimeSpan.FromHours(9),
                EndTime = TimeSpan.FromHours(10),
                TutorLearnerSubjectId = tutorLearnerSubjectId
            }
        };

            _mockUnitOfWork.Setup(uow => uow.schedule.GetSchedulesByTutorIdAsync(tutorId)).ReturnsAsync(existingSchedules);

            // Act
            var result = await _scheduleService.AdjustTutorSchedulesAsync(tutorId, tutorLearnerSubjectId, tutorRequestSchedules);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Lịch yêu cầu vào ngày 1 từ 09:00:00 đến 10:00:00 trùng với lịch dạy.", result.Message);
        }

        [Test]
        public async Task AdjustTutorSchedulesAsync_ShouldAdjustFreeTimeCorrectly_WhenOverlapWithFreeTime()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var tutorLearnerSubjectId = 1;
            var tutorRequestSchedules = new List<TutorRequestSchedulesDTO>
        {
            new TutorRequestSchedulesDTO
            {
                DayOfWeek = 1, // Monday
                FreeTimes = new List<FreeTimeDTO>
                {
                    new FreeTimeDTO
                    {
                        StartTime = TimeSpan.FromHours(8),
                        EndTime = TimeSpan.FromHours(9)
                    }
                }
            }
        };

            var existingSchedules = new List<Schedule>
        {
            new Schedule
            {
                TutorId = tutorId,
                DayOfWeek = 1,
                StartTime = TimeSpan.FromHours(7),
                EndTime = TimeSpan.FromHours(10),
                TutorLearnerSubjectId = null // Free time
            }
        };

            _mockUnitOfWork.Setup(uow => uow.schedule.GetSchedulesByTutorIdAsync(tutorId)).ReturnsAsync(existingSchedules);

            // Act
            var result = await _scheduleService.AdjustTutorSchedulesAsync(tutorId, tutorLearnerSubjectId, tutorRequestSchedules);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public async Task AdjustTutorSchedulesAsync_ShouldAddNewSchedule_WhenNoConflicts()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var tutorLearnerSubjectId = 1;
            var tutorRequestSchedules = new List<TutorRequestSchedulesDTO>
        {
            new TutorRequestSchedulesDTO
            {
                DayOfWeek = 2, // Tuesday
                FreeTimes = new List<FreeTimeDTO>
                {
                    new FreeTimeDTO
                    {
                        StartTime = TimeSpan.FromHours(10),
                        EndTime = TimeSpan.FromHours(11)
                    }
                }
            }
        };

            var existingSchedules = new List<Schedule>(); // No existing schedules for the tutor

            _mockUnitOfWork.Setup(uow => uow.schedule.GetSchedulesByTutorIdAsync(tutorId)).ReturnsAsync(existingSchedules);

            // Act
            var result = await _scheduleService.AdjustTutorSchedulesAsync(tutorId, tutorLearnerSubjectId, tutorRequestSchedules);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            _mockUnitOfWork.Verify(uow => uow.schedule.Add(It.IsAny<Schedule>()), Times.Once); // Ensure new teaching schedule is added
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once); // Verify commit was called
        }

        [Test]
        public async Task AdjustTutorSchedulesAsync_ShouldSplitFreeTimeCorrectly_WhenOverlapPartial()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var tutorLearnerSubjectId = 1;
            var tutorRequestSchedules = new List<TutorRequestSchedulesDTO>
        {
            new TutorRequestSchedulesDTO
            {
                DayOfWeek = 1, // Monday
                FreeTimes = new List<FreeTimeDTO>
                {
                    new FreeTimeDTO
                    {
                        StartTime = TimeSpan.FromHours(8),
                        EndTime = TimeSpan.FromHours(9)
                    }
                }
            }
        };

            var existingSchedules = new List<Schedule>
        {
            new Schedule
            {
                TutorId = tutorId,
                DayOfWeek = 1,
                StartTime = TimeSpan.FromHours(7),
                EndTime = TimeSpan.FromHours(10),
                TutorLearnerSubjectId = null // Free time
            }
        };

            _mockUnitOfWork.Setup(uow => uow.schedule.GetSchedulesByTutorIdAsync(tutorId)).ReturnsAsync(existingSchedules);

            // Act
            var result = await _scheduleService.AdjustTutorSchedulesAsync(tutorId, tutorLearnerSubjectId, tutorRequestSchedules);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }


        #region DeleteSchedule Tests

        [Test]
        public async Task DeleteSchedule_ShouldDeleteSchedules_WhenSchedulesExist()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var scheduleToDelete = new DeleteScheduleDTO
            {
                ScheduleId = 1,
                FreeTimes = new List<FreeTimeDTO>
            {
                new FreeTimeDTO { StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(10) }
            }
            };

            // Act
            await _scheduleService.DeleteSchedule(tutorId, scheduleToDelete);

            // Assert
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Test]
        public async Task DeleteSchedule_ShouldNotDelete_WhenScheduleDoesNotExist()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var scheduleToDelete = new DeleteScheduleDTO
            {
                ScheduleId = 999, // ID that doesn't exist
                FreeTimes = new List<FreeTimeDTO>
            {
                new FreeTimeDTO { StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(10) }
            }
            };

            // Act
            await _scheduleService.DeleteSchedule(tutorId, scheduleToDelete);

            // Assert
            _mockUnitOfWork.Verify(u => u.schedule.Delete(It.IsAny<int>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        #endregion

        #region UpdateSchedule Tests

        [Test]
        public async Task UpdateSchedule_ShouldUpdateExistingSchedules()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var updatedSchedule = new UpdateScheduleDTO
            {
                Id = 1,
                TutorLearnerSubjectID = 123,
                DayOfWeek = 1,
                FreeTimes = new List<FreeTimeDTO>
            {
                new FreeTimeDTO { StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(11) }
            }
            };

            // Act
            await _scheduleService.UpdateSchedule(tutorId, updatedSchedule);

            // Assert
        }

        #region GetSchedulesByTutorIdAsync Tests

        [Test]
        public async Task GetSchedulesByTutorIdAsync_ShouldReturnGroupedSchedules_WhenSchedulesExist()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var mockSchedules = new List<Schedule>
        {
            new Schedule
            {
                ScheduleId = 1,
                TutorId = tutorId,
                DayOfWeek = 1,
                StartTime = TimeSpan.FromHours(9),
                EndTime = TimeSpan.FromHours(10),
                TutorLearnerSubjectId = 100,
                tutorLearnerSubject = new TutorLearnerSubject
                {
                    TutorSubject = new TutorSubject
                    {
                        TutorId = tutorId,
                        Subject = new Subject { SubjectName = "Math" }
                    }
                }
            },
            new Schedule
            {
                ScheduleId = 2,
                TutorId = tutorId,
                DayOfWeek = 1,
                StartTime = TimeSpan.FromHours(11),
                EndTime = TimeSpan.FromHours(12),
                TutorLearnerSubjectId = null
            }
        };

            _mockUnitOfWork.Setup(u => u.schedule.GetMultiAsQueryable(It.IsAny<Expression<Func<Schedule, bool>>>(), It.IsAny<string[]>()))
                .Returns(mockSchedules.AsQueryable());

            // Act
            var result = await _scheduleService.GetSchedulesByTutorIdAsync(tutorId);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(2, result[0].Schedules.Count);
            Assert.AreEqual("Math", result[0].Schedules[0].SubjectNames);
            Assert.AreEqual("Lịch rảnh chưa có lớp", result[0].Schedules[1].SubjectNames);
        }

        [Test]
        public async Task GetSchedulesByTutorIdAsync_ShouldReturnEmptyList_WhenNoSchedulesExist()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            _mockUnitOfWork.Setup(u => u.schedule.GetMultiAsQueryable(It.IsAny<Expression<Func<Schedule, bool>>>(), It.IsAny<string[]>()))
                .Returns(Enumerable.Empty<Schedule>().AsQueryable());

            // Act
            var result = await _scheduleService.GetSchedulesByTutorIdAsync(tutorId);

            // Assert
            Assert.IsEmpty(result);
        }

        #endregion

        #region GetConflictingSchedulesAsync Tests

        [Test]
        public async Task GetConflictingSchedulesAsync_ShouldReturnConflictingSchedules_WhenConflictsExist()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var conflictingSchedules = new List<Schedule>
        {
            new Schedule
            {
                ScheduleId = 1,
                TutorId = tutorId,
                DayOfWeek = 1,
                StartTime = TimeSpan.FromHours(9),
                EndTime = TimeSpan.FromHours(10)
            }
        };

            _mockUnitOfWork.Setup(u => u.schedule.GetMulti(It.IsAny<Expression<Func<Schedule, bool>>>(), null))
                .Returns(conflictingSchedules);

            // Act
            var result = await _scheduleService.GetConflictingSchedulesAsync(tutorId, 1, TimeSpan.FromHours(9), TimeSpan.FromHours(11));

            // Assert
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public async Task GetConflictingSchedulesAsync_ShouldReturnEmptyList_WhenNoConflictsExist()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            _mockUnitOfWork.Setup(u => u.schedule.GetMulti(It.IsAny<Expression<Func<Schedule, bool>>>(), null))
                .Returns(new List<Schedule>());

            // Act
            var result = await _scheduleService.GetConflictingSchedulesAsync(tutorId, 1, TimeSpan.FromHours(9), TimeSpan.FromHours(10));

            // Assert
            Assert.IsEmpty(result);
        }

        #endregion

        #region AddSchedule Tests

        [Test]
        public async Task AddSchedule_ShouldAddSchedules_WhenValidFreeTimesProvided()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var newSchedule = new AddScheduleDTO
            {
                DayOfWeek = 1,
                FreeTimes = new List<FreeTimeDTO>
            {
                new FreeTimeDTO { StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(10) },
                new FreeTimeDTO { StartTime = TimeSpan.FromHours(11), EndTime = TimeSpan.FromHours(12) }
            }
            };

            // Act
            await _scheduleService.AddSchedule(tutorId, newSchedule);

            // Assert
            _mockUnitOfWork.Verify(u => u.schedule.Add(It.IsAny<Schedule>()), Times.Exactly(2));
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Test]
        public async Task AddSchedule_ShouldNotAddSchedules_WhenNoFreeTimesProvided()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var newSchedule = new AddScheduleDTO
            {
                DayOfWeek = 1,
                FreeTimes = new List<FreeTimeDTO>() // Empty list
            };

            // Act
            await _scheduleService.AddSchedule(tutorId, newSchedule);

            // Assert
            _mockUnitOfWork.Verify(u => u.schedule.Add(It.IsAny<Schedule>()), Times.Never);
        }

        #endregion

        [Test]
        public async Task UpdateSchedule_ShouldAddNewSchedules_WhenNoExistingSchedulesFound()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var updatedSchedule = new UpdateScheduleDTO
            {
                Id = 999, // Non-existent schedule ID
                TutorLearnerSubjectID = 123,
                DayOfWeek = 2,
                FreeTimes = new List<FreeTimeDTO>
            {
                new FreeTimeDTO { StartTime = TimeSpan.FromHours(14), EndTime = TimeSpan.FromHours(15) }
            }
            };

            // Act
            await _scheduleService.UpdateSchedule(tutorId, updatedSchedule);

            // Assert
            _mockUnitOfWork.Verify(u => u.schedule.Add(It.IsAny<Schedule>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.schedule.Update(It.IsAny<Schedule>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Test]
        public async Task UpdateSchedule_ShouldUpdateAndAddSchedules_MixedScenario()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var updatedSchedule = new UpdateScheduleDTO
            {
                Id = 1,
                TutorLearnerSubjectID = 123,
                DayOfWeek = 1,
                FreeTimes = new List<FreeTimeDTO>
            {
                new FreeTimeDTO { StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(11) }, // Update existing
                new FreeTimeDTO { StartTime = TimeSpan.FromHours(14), EndTime = TimeSpan.FromHours(15) }  // Add new
            }
            };

            _mockUnitOfWork.Setup(u => u.schedule.GetSingleById(1)).Returns(new Schedule
            {
                ScheduleId = 1,
                TutorId = tutorId,
                DayOfWeek = 1,
                StartTime = TimeSpan.FromHours(9),
                EndTime = TimeSpan.FromHours(10),
            });

            _mockUnitOfWork.Setup(u => u.schedule.GetSingleById(999)).Returns((Schedule)null);

            // Act
            await _scheduleService.UpdateSchedule(tutorId, updatedSchedule);

            // Assert
        }

        #endregion
    }
}


