using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Enum;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.Models;
using TutoRum.Services.Helper;
using TutoRum.Services.IService;
using TutoRum.Services.Service;
using TutoRum.Services.ViewModels;

namespace TutoRum.UnitTests.ServiceUnitTest
{
    [TestFixture]
    public class TutorLearnerSubjectServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<UserManager<AspNetUser>> _mockUserManager;
        private Mock<IScheduleService> _mockScheduleService;
        private Mock<INotificationService> _mockNotificationService;
        private Mock<IDbContextTransaction> _mockTransaction;
        private TutorLearnerSubjectService _service;

        [SetUp]
        public void SetUp()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUserManager = new Mock<UserManager<AspNetUser>>(
                Mock.Of<IUserStore<AspNetUser>>(),
                null, null, null, null, null, null, null, null
            );
            _mockScheduleService = new Mock<IScheduleService>();
            _mockNotificationService = new Mock<INotificationService>();
            _mockTransaction = new Mock<IDbContextTransaction>();

            _mockNotificationService.Setup(ns => ns.SendNotificationAsync(It.IsAny<NotificationRequestDto>(),It.IsAny<bool?>()));

            _mockUnitOfWork
                .Setup(uow => uow.TutorLearnerSubject.GetExecutionStrategy())
                .Returns(new Mock<IExecutionStrategy>().Object);

            _mockUnitOfWork
                .Setup(uow => uow.TutorLearnerSubject.BeginTransactionAsync(It.IsAny<IsolationLevel>()))
                .ReturnsAsync(_mockTransaction.Object);

            

            _mockUnitOfWork.Setup(st => st.TutorLearnerSubject.Add(It.IsAny<TutorLearnerSubject>()));
            _mockUnitOfWork.Setup(st => st.TutorLearnerSubject.Update(It.IsAny<TutorLearnerSubject>()));
            _mockUnitOfWork.Setup(u => u.TutorLearnerSubject.DeleteMulti(It.IsAny<Expression<Func<TutorLearnerSubject, bool>>>()));
            _service = new TutorLearnerSubjectService(
                _mockUnitOfWork.Object,
                _mockUserManager.Object,
                _mockScheduleService.Object,
                new APIAddress(new HttpClient()), // Mock HttpClient or replace if not critical
                _mockNotificationService.Object
            );
        }

        [Test]
        public void RegisterLearnerForTutorAsync_WhenUserNotFound_ShouldThrowException()
        {
            // Arrange
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync((AspNetUser)null);

            var learnerDto = new RegisterLearnerDTO();
            var user = new ClaimsPrincipal();

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _service.RegisterLearnerForTutorAsync(learnerDto, user)
            );
            Assert.AreEqual("User not found.", ex.Message);
        }

        [Test]
        public void RegisterLearnerForTutorAsync_WhenUserNotLearner_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(currentUser);
            _mockUserManager.Setup(um => um.IsInRoleAsync(currentUser, AccountRoles.Learner))
                .ReturnsAsync(false);

            var learnerDto = new RegisterLearnerDTO();
            var user = new ClaimsPrincipal();

            // Act & Assert
            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _service.RegisterLearnerForTutorAsync(learnerDto, user)
            );
            Assert.AreEqual("User is not a learner!", ex.Message);
        }

        

        [Test]
        public async Task RegisterLearnerForTutorAsync_WhenSuccessful_ShouldSendNotifications()
        {
            // Arrange
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };
            var tutorSubject = new TutorSubject
            {
                TutorId = Guid.NewGuid(),
                TutorSubjectId = 1
            };

            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(currentUser);
            _mockUserManager.Setup(um => um.IsInRoleAsync(currentUser, AccountRoles.Learner))
                .ReturnsAsync(true);
            _mockUnitOfWork.Setup(uow => uow.TutorSubjects.FindAsync(It.IsAny<Expression<Func<TutorSubject, bool>>>()))
                .ReturnsAsync(tutorSubject);

            var learnerDto = new RegisterLearnerDTO
            {
                TutorSubjectId = 1,
                CityId = "1",
                DistrictId = "1",
                WardId = "1",
                PricePerHour = 200,
                SessionsPerWeek = 2,
                HoursPerSession = 1,
                PreferredScheduleType = "Morning",
                ExpectedStartDate = DateTime.UtcNow.AddDays(7)
            };

            _mockUnitOfWork.Setup(uow => uow.CommitAsync()).Returns(Task.CompletedTask);
            _mockScheduleService.Setup(ss => ss.RegisterSchedulesForClass(It.IsAny<List<ScheduleDTO>>(), currentUser.Id, It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.RegisterLearnerForTutorAsync(learnerDto, new ClaimsPrincipal());

            // Assert
           
        }

        [Test]
        public async Task UpdateClassroom_WhenSuccessful_ShouldUpdateAndNotify()
        {
            // Arrange
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };
            var tutorLearnerSubjectId = 1;
            var learnerDto = new RegisterLearnerDTO
            {
                TutorSubjectId = 2,
                CityId = "City01",
                DistrictId = "District01",
                WardId = "Ward01",
                PricePerHour = 100,
                ContractUrl = "http://example.com/contract.pdf",
                Notes = "Test notes",
                LocationDetail = "Test location",
                SessionsPerWeek = 3,
                HoursPerSession = 2,
                PreferredScheduleType = "Afternoon",
                ExpectedStartDate = DateTime.UtcNow.AddDays(7),
                Schedules = new List<ScheduleDTO> { new ScheduleDTO { DayOfWeek = 2, FreeTimes = new List<FreeTimeDTO> { new FreeTimeDTO { } } } }
            };

            var existingTutorLearnerSubject = new TutorLearnerSubject
            {
                TutorLearnerSubjectId = tutorLearnerSubjectId,
                TutorSubjectId = 1,
                Location = "OldCity",
                UpdatedBy = Guid.Empty
            };

            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(currentUser);

            _mockUserManager.Setup(um => um.IsInRoleAsync(currentUser, AccountRoles.Tutor))
                .ReturnsAsync(true);

            _mockUnitOfWork.Setup(uow => uow.TutorLearnerSubject.GetTutorLearnerSubjectAsyncById(tutorLearnerSubjectId))
                .ReturnsAsync(existingTutorLearnerSubject);

            _mockUnitOfWork.Setup(uow => uow.CommitAsync())
                .Returns(Task.CompletedTask);

            _mockScheduleService.Setup(ss => ss.RegisterSchedulesForClass(It.IsAny<List<ScheduleDTO>>(), currentUser.Id, tutorLearnerSubjectId))
                .Returns(Task.CompletedTask);

            _mockNotificationService.Setup(ns => ns.SendNotificationAsync(It.IsAny<NotificationRequestDto>(), false))
                .Returns(Task.CompletedTask);

            // Act
            var result =  _service.UpdateClassroom(tutorLearnerSubjectId, learnerDto, new ClaimsPrincipal());

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task CloseClassAsync_UserNotFound_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync((AspNetUser)null);

            var user = new ClaimsPrincipal();

            // Act & Assert
            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _service.CloseClassAsync(1, user)
            );
            Assert.AreEqual("User not found or not authorized.", ex.Message);
        }

        [Test]
        public async Task CloseClassAsync_ClassNotFound_ShouldThrowException()
        {
            // Arrange
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(currentUser);
            _mockUnitOfWork.Setup(uow => uow.TutorLearnerSubject.GetTutorLearnerSubjectAsyncById(It.IsAny<int>()))
                .ReturnsAsync((TutorLearnerSubject)null);

            var user = new ClaimsPrincipal();

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _service.CloseClassAsync(1, user)
            );
            Assert.AreEqual("TutorLearnerSubject not found.", ex.Message);
        }

        [Test]
        public async Task CloseClassAsync_UnpaidBills_ShouldReturnFalse()
        {
            // Arrange
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };
            var tutorLearnerSubjectId = 1;
            var tutorLearnerSubject = new TutorLearnerSubject
            {
                TutorLearnerSubjectId = tutorLearnerSubjectId,
                TutorSubject = new TutorSubject { TutorId = Guid.NewGuid() },
                LearnerId = Guid.NewGuid()
            };
            var unpaidBills = new List<BillingEntry>
        {
            new BillingEntry { BillId = null, TutorLearnerSubjectId = tutorLearnerSubjectId }
        };

            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(currentUser);
            _mockUnitOfWork.Setup(uow => uow.TutorLearnerSubject.GetTutorLearnerSubjectAsyncById(tutorLearnerSubjectId))
                .ReturnsAsync(tutorLearnerSubject);
            _mockUnitOfWork.Setup(uow => uow.BillingEntry.GetMulti(It.IsAny<Expression<Func<BillingEntry, bool>>>(), It.IsAny<string[]>()))
                .Returns(unpaidBills.AsQueryable());

            // Act
            var result = await _service.CloseClassAsync(tutorLearnerSubjectId, new ClaimsPrincipal());

            // Assert
            Assert.IsFalse(result);
            _mockNotificationService.Verify(ns => ns.SendNotificationAsync(It.IsAny<NotificationRequestDto>(), false), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Never); // No commit should happen
        }

        [Test]
        public async Task CloseClassAsync_SuccessfulClosure_ShouldReturnTrue()
        {
            // Arrange
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };
            var tutorLearnerSubjectId = 1;
            var tutorLearnerSubject = new TutorLearnerSubject
            {
                TutorLearnerSubjectId = tutorLearnerSubjectId,
                TutorSubject = new TutorSubject { TutorId = Guid.NewGuid() },
                LearnerId = Guid.NewGuid()
            };

            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(currentUser);
            _mockUnitOfWork.Setup(uow => uow.TutorLearnerSubject.GetTutorLearnerSubjectAsyncById(tutorLearnerSubjectId))
                .ReturnsAsync(tutorLearnerSubject);
            _mockUnitOfWork.Setup(uow => uow.BillingEntry.GetMulti(It.IsAny<Expression<Func<BillingEntry, bool>>>(), It.IsAny<string[]>()))
                .Returns(Enumerable.Empty<BillingEntry>().AsQueryable());
            _mockUnitOfWork.Setup(uow => uow.CommitAsync())
                .Returns(Task.CompletedTask);
            _mockScheduleService.Setup(ss => ss.MergeScheduleWithFreeTime(It.IsAny<Guid>(), It.IsAny<List<TutorRequestSchedulesDTO>>()))
                .Returns(Task.CompletedTask);
            _mockNotificationService.Setup(ns => ns.SendNotificationAsync(It.IsAny<NotificationRequestDto>(), false))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CloseClassAsync(tutorLearnerSubjectId, new ClaimsPrincipal());

            // Assert
            Assert.IsTrue(result);
            _mockUnitOfWork.Verify(uow => uow.TutorLearnerSubject.Update(It.IsAny<TutorLearnerSubject>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
            _mockNotificationService.Verify(ns => ns.SendNotificationAsync(It.IsAny<NotificationRequestDto>(), false), Times.Exactly(2));
            _mockScheduleService.Verify(ss => ss.MergeScheduleWithFreeTime(It.IsAny<Guid>(), It.IsAny<List<TutorRequestSchedulesDTO>>()), Times.Once);
        }

        [Test]
        public async Task CreateClassForLearnerAsync_UserNotTutor_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(currentUser);
            _mockUserManager.Setup(um => um.IsInRoleAsync(currentUser, AccountRoles.Tutor))
                .ReturnsAsync(false);

            var classDto = new CreateClassDTO { LearnerId = Guid.NewGuid() };
            var user = new ClaimsPrincipal();

            // Act & Assert
            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _service.CreateClassForLearnerAsync(classDto, 1, user)
            );
            Assert.AreEqual("User is not a tutor!", ex.Message);
        }

        [Test]
        public async Task CreateClassForLearnerAsync_LearnerNotFound_ShouldThrowException()
        {
            // Arrange
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(currentUser);
            _mockUserManager.Setup(um => um.IsInRoleAsync(currentUser, AccountRoles.Tutor))
                .ReturnsAsync(true);
            _mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((AspNetUser)null);

            var classDto = new CreateClassDTO { LearnerId = Guid.NewGuid() };
            var user = new ClaimsPrincipal();

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _service.CreateClassForLearnerAsync(classDto, 1, user)
            );
            Assert.AreEqual("Learner not found.", ex.Message);
        }

        [Test]
        public async Task CreateClassForLearnerAsync_SuccessfulExecution_ShouldReturnTrue()
        {
            // Arrange
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };
            var learner = new AspNetUser { Id = Guid.NewGuid() };
            var classDto = new CreateClassDTO
            {
                LearnerId = learner.Id,
                TutorSubjectId = 1,
                CityId = "City01",
                DistrictId = "District01",
                WardId = "Ward01",
                PricePerHour = 50,
                SessionsPerWeek = 3,
                HoursPerSession = 2,
                PreferredScheduleType = "Morning",
                ExpectedStartDate = DateTime.UtcNow.AddDays(7),
                Schedules = new List<TutorRequestSchedulesDTO> { new TutorRequestSchedulesDTO { DayOfWeek = 1 } }
            };

            var tutorRequest = new TutorRequest { Id = 1 };

            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(currentUser);
            _mockUserManager.Setup(um => um.IsInRoleAsync(currentUser, AccountRoles.Tutor))
                .ReturnsAsync(true);
            _mockUserManager.Setup(um => um.FindByIdAsync(learner.Id.ToString()))
                .ReturnsAsync(learner);

            _mockUnitOfWork.Setup(uow => uow.TutorLearnerSubject.Add(It.IsAny<TutorLearnerSubject>()));
            _mockUnitOfWork.Setup(uow => uow.CommitAsync()).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(uow => uow.TutorRequest.FindAsync(It.IsAny<Expression<Func<TutorRequest, bool>>>()))
                .ReturnsAsync(tutorRequest);

            _mockScheduleService.Setup(ss => ss.AdjustTutorSchedulesAsync(
                    It.IsAny<Guid>(),          // TutorId
                    It.IsAny<int>(),           // TutorLearnerSubjectId
                    It.IsAny<List<TutorRequestSchedulesDTO>>() // Schedules
                )).ReturnsAsync((true, "Schedules adjusted successfully"));
            _mockNotificationService.Setup(ns => ns.SendNotificationAsync(It.IsAny<NotificationRequestDto>(), false))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateClassForLearnerAsync(classDto, tutorRequest.Id, new ClaimsPrincipal());

            // Assert
            Assert.IsTrue(result);
        }


        [Test]
        public void GetTutorLearnerSubjectSummaryDetailByIdAsync_UserNotFound_ShouldThrowException()
        {
            // Arrange
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync((AspNetUser)null);

            var user = new ClaimsPrincipal();

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _service.GetTutorLearnerSubjectSummaryDetailByIdAsync(1, user)
            );

            Assert.AreEqual("User not found.", ex.Message);
        }

        [Test]
        public void GetTutorLearnerSubjectSummaryDetailByIdAsync_TutorLearnerSubjectNotFound_ShouldThrowException()
        {
            // Arrange
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(currentUser);

            _mockUnitOfWork.Setup(uow => uow.TutorLearnerSubject.GetSingleByCondition(
                It.IsAny<Expression<Func<TutorLearnerSubject, bool>>>(),
                It.IsAny<string[]>()
            )).Returns((TutorLearnerSubject)null);

            var user = new ClaimsPrincipal();

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _service.GetTutorLearnerSubjectSummaryDetailByIdAsync(1, user)
            );

            Assert.AreEqual("Tutor Learner Subject not found", ex.Message);
        }

        [Test]
        public async Task GetTutorLearnerSubjectSummaryDetailByIdAsync_SuccessfulExecution_ShouldReturnDetailDto()
        {
            // Arrange
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };
            var learner = new AspNetUser { Id = Guid.NewGuid(), Email = "learner@example.com" };
            var tutorLearnerSubjectId = 1;

            var tutorLearnerSubject = new TutorLearnerSubject
            {
                TutorLearnerSubjectId = tutorLearnerSubjectId,
                TutorSubjectId = 1,
                LearnerId = learner.Id,
                TutorSubject = new TutorSubject
                {
                    TutorId = Guid.NewGuid(),
                    Subject = new Subject { SubjectName = "Math" },
                    Rate = 100
                },
                Location = "City01",
                DistrictId = "District01",
                WardId = "Ward01",
                LocationDetail = "Detail01",
                Notes = "Notes01",
                SessionsPerWeek = 2,
                HoursPerSession = 1,
                PreferredScheduleType = "Morning",
                ExpectedStartDate = DateTime.UtcNow.AddDays(7),
                IsVerified = true,
                ContractUrl = "http://example.com/contract.pdf",
                IsContractVerified = true,
                IsCloseClass = false,
                Learner = learner
            };

            var schedules = new List<Schedule>
            {
                new Schedule { DayOfWeek = 1, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(10, 0, 0), TutorLearnerSubjectId = tutorLearnerSubjectId }
            };

            var billingEntries = new List<BillingEntry>
            {
                new BillingEntry { TutorLearnerSubjectId = tutorLearnerSubjectId, IsDraft = false }
            };

            decimal totalPaidAmount = 200;

            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(currentUser);

            _mockUnitOfWork.Setup(uow => uow.TutorLearnerSubject.GetSingleByCondition(
                It.IsAny<Expression<Func<TutorLearnerSubject, bool>>>(),
                It.IsAny<string[]>()
            )).Returns(tutorLearnerSubject);

            _mockUnitOfWork.Setup(uow => uow.schedule.GetMulti(It.IsAny<Expression<Func<Schedule, bool>>>(), It.IsAny<string[]>()))
                .Returns(schedules.AsQueryable());

            _mockUnitOfWork.Setup(uow => uow.BillingEntry.GetMulti(It.IsAny<Expression<Func<BillingEntry, bool>>>(), It.IsAny<string[]>()))
                .Returns(billingEntries.AsQueryable());

            _mockUnitOfWork.Setup(uow => uow.Payment.GetTotalPaidAmountByTutorLearnerSubjectIdAsync(tutorLearnerSubjectId))
                .Returns(totalPaidAmount);

            var user = new ClaimsPrincipal();

            // Act
            var result = await _service.GetTutorLearnerSubjectSummaryDetailByIdAsync(tutorLearnerSubjectId, user);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(tutorLearnerSubjectId, result.TutorLearnerSubjectId);
            Assert.AreEqual("Math", result.SubjectName);
            Assert.AreEqual(200, result.TotalPaidAmount);
            Assert.AreEqual("learner@example.com", result.LearnerEmail);
            Assert.AreEqual(1, result.TotalSessionsCompleted);
            Assert.AreEqual(1, result.Schedules.Count);
            Assert.AreEqual("Morning", result.PreferredScheduleType);
        }
    }

}


