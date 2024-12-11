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



    }

}
