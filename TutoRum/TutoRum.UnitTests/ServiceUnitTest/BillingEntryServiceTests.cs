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
using TutoRum.Data.Models;
using TutoRum.Services.Service;
using TutoRum.Services.ViewModels;
using static TutoRum.FE.Common.Url;

namespace TutoRum.UnitTests.ServiceUnitTest
{
    [TestFixture]
    public class BillingEntryServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<UserManager<AspNetUser>> _mockUserManager;
        private BillingEntryService _billingEntryService;

        [SetUp]
        public void SetUp()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUserManager = new Mock<UserManager<AspNetUser>>(
                Mock.Of<IUserStore<AspNetUser>>(), null, null, null, null, null, null, null, null);

            var billingEntries = new List<BillingEntry>
    {
        new BillingEntry { BillingEntryId = 1, Rate = 100, Description = "Entry 1" },
        new BillingEntry { BillingEntryId = 2, Rate = 200, Description = "Entry 2" }
            };

            var total = 1;
            _mockUnitOfWork.Setup(uow => uow.BillingEntry.GetMultiPaging(
                It.IsAny<Expression<Func<BillingEntry, bool>>>(),
                out total,
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string[]>(),
                It.IsAny<Func<IQueryable<BillingEntry>, IOrderedQueryable<BillingEntry>>>()
            )).Returns(billingEntries);

            _billingEntryService = new BillingEntryService(_mockUnitOfWork.Object, _mockUserManager.Object);
        }


        [Test]
        public async Task AddBillingEntryAsync_ShouldAddBillingEntry_WhenValid()
        {
            // Arrange
            var billingEntryDTO = new AdddBillingEntryDTO
            {
                TutorLearnerSubjectId = 1,
                Rate = 100,
                StartDateTime = DateTime.UtcNow,
                EndDateTime = DateTime.UtcNow.AddHours(1),
                Description = "Test Description",
                TotalAmount = 100
            };

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "userId") }));
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };

            _mockUserManager.Setup(um => um.GetUserAsync(user)).ReturnsAsync(currentUser);
            _mockUnitOfWork.Setup(u => u.BillingEntry.GetBillingEntriesByTutorLearnerSubjectIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<BillingEntry>());
            _mockUnitOfWork.Setup(u => u.BillingEntry.Add(It.IsAny<BillingEntry>()));
            _mockUnitOfWork.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

            // Act
            await _billingEntryService.AddBillingEntryAsync(billingEntryDTO, user);

            // Assert
            _mockUnitOfWork.Verify(u => u.BillingEntry.Add(It.IsAny<BillingEntry>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Test]
        public void AddBillingEntryAsync_ShouldThrowException_WhenOverlappingEntryExists()
        {
            // Arrange
            var billingEntryDTO = new AdddBillingEntryDTO
            {
                TutorLearnerSubjectId = 1,
                Rate = 100,
                StartDateTime = DateTime.UtcNow,
                EndDateTime = DateTime.UtcNow.AddHours(1),
                Description = "Test Description",
                TotalAmount = 100
            };

            _mockUnitOfWork.Setup(u => u.BillingEntry.GetBillingEntriesByTutorLearnerSubjectIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<BillingEntry>
                {
            new BillingEntry
            {
                StartDateTime = DateTime.UtcNow.AddMinutes(-30),
                EndDateTime = DateTime.UtcNow.AddMinutes(30)
            }
                });

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _billingEntryService.AddBillingEntryAsync(billingEntryDTO));
        }
        [Test]
        public async Task AddDraftBillingEntryAsync_ShouldAddDraftEntry_WhenValid()
        {
            // Arrange
            var billingEntryDTO = new AdddBillingEntryDTO
            {
                TutorLearnerSubjectId = 1,
                Rate = 100,
                StartDateTime = DateTime.UtcNow,
                EndDateTime = DateTime.UtcNow.AddHours(1),
                Description = "Test Description",
                TotalAmount = 100
            };

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "userId") }));
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };

            _mockUserManager.Setup(um => um.GetUserAsync(user)).ReturnsAsync(currentUser);
            _mockUnitOfWork.Setup(u => u.BillingEntry.Add(It.IsAny<BillingEntry>()));
            _mockUnitOfWork.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

            // Act
            await _billingEntryService.AddDraftBillingEntryAsync(billingEntryDTO, user);

            // Assert
            _mockUnitOfWork.Verify(u => u.BillingEntry.Add(It.IsAny<BillingEntry>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Test]
        public void AddDraftBillingEntryAsync_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            var billingEntryDTO = new AdddBillingEntryDTO
            {
                TutorLearnerSubjectId = 1,
                Rate = 100,
                StartDateTime = DateTime.UtcNow,
                EndDateTime = DateTime.UtcNow.AddHours(1),
                Description = "Test Description",
                TotalAmount = 100
            };

            var user = new ClaimsPrincipal();

            _mockUserManager.Setup(um => um.GetUserAsync(user)).ReturnsAsync((AspNetUser)null);

            // Act & Assert
            Assert.ThrowsAsync<Exception>(
                async () => await _billingEntryService.AddDraftBillingEntryAsync(billingEntryDTO, user));
        }
        [Test]
        public async Task UpdateBillingEntryAsync_ShouldUpdateBillingEntry_WhenValid()
        {
            // Arrange
            int billingEntryId = 1;
            var billingEntryDTO = new UpdateBillingEntryDTO
            {
                Rate = 150,
                StartDateTime = DateTime.UtcNow,
                EndDateTime = DateTime.UtcNow.AddHours(2),
                Description = "Updated Description",
                TotalAmount = 300
            };

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "userId") }));
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };
            var existingBillingEntry = new BillingEntry
            {
                BillingEntryId = billingEntryId,
                Rate = 100,
                StartDateTime = DateTime.UtcNow.AddHours(-1),
                EndDateTime = DateTime.UtcNow.AddHours(1),
                Description = "Old Description",
                TotalAmount = 200
            };

            _mockUserManager.Setup(um => um.GetUserAsync(user)).ReturnsAsync(currentUser);
            _mockUnitOfWork.Setup(u => u.BillingEntry.GetSingleById(billingEntryId)).Returns(existingBillingEntry);
            _mockUnitOfWork.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

            // Act
            await _billingEntryService.UpdateBillingEntryAsync(billingEntryId, billingEntryDTO, user);

            // Assert
            Assert.AreEqual(billingEntryDTO.Rate, existingBillingEntry.Rate);
            Assert.AreEqual(billingEntryDTO.Description, existingBillingEntry.Description);
            _mockUnitOfWork.Verify(u => u.BillingEntry.Update(existingBillingEntry), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Test]
        public void UpdateBillingEntryAsync_ShouldThrowException_WhenBillingEntryNotFound()
        {
            // Arrange
            int billingEntryId = 1;
            var billingEntryDTO = new UpdateBillingEntryDTO();

            _mockUnitOfWork.Setup(u => u.BillingEntry.GetSingleById(billingEntryId)).Returns((BillingEntry)null);

            // Act & Assert
            Assert.ThrowsAsync<Exception>(
                async () => await _billingEntryService.UpdateBillingEntryAsync(billingEntryId, billingEntryDTO, null));
        }
        [Test]
        public async Task DeleteBillingEntriesAsync_ShouldDeleteBillingEntries_WhenValid()
        {
            // Arrange
            var billingEntryIds = new List<int> { 1, 2 };
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "userId") }));
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };
            var billingEntries = new List<BillingEntry>
    {
        new BillingEntry { BillingEntryId = 1 },
        new BillingEntry { BillingEntryId = 2 }
    };

            _mockUserManager.Setup(um => um.GetUserAsync(user)).ReturnsAsync(currentUser);
            _mockUnitOfWork.Setup(u => u.BillingEntry.GetMulti(It.IsAny<Expression<Func<BillingEntry, bool>>>(), It.IsAny<string[]>())).Returns(billingEntries);
            _mockUnitOfWork.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

            // Act
            await _billingEntryService.DeleteBillingEntriesAsync(billingEntryIds, user);

            // Assert
            foreach (var id in billingEntryIds)
            {
                _mockUnitOfWork.Verify(u => u.BillingEntry.Delete(id), Times.Once);
            }
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Test]
        public void DeleteBillingEntriesAsync_ShouldThrowException_WhenNoEntriesFound()
        {
            // Arrange
            var billingEntryIds = new List<int> { 1, 2 };

            _mockUnitOfWork.Setup(u => u.BillingEntry.GetMulti(It.IsAny<Expression<Func<BillingEntry, bool>>>(), It.IsAny<string[]>())).Returns(new List<BillingEntry>());

            // Act & Assert
            Assert.ThrowsAsync<Exception>(
                async () => await _billingEntryService.DeleteBillingEntriesAsync(billingEntryIds, null));
        }
        [Test]
        public async Task GetAllBillingEntriesAsync_ShouldReturnPagedResults_WhenValid()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "userId") }));
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };
            int totalRecords;
            var billingEntries = new List<BillingEntry>
    {
        new BillingEntry { BillingEntryId = 1, Rate = 100, Description = "Entry 1" },
        new BillingEntry { BillingEntryId = 2, Rate = 200, Description = "Entry 2" }
    };

            _mockUserManager.Setup(um => um.GetUserAsync(user)).ReturnsAsync(currentUser);
            var total = 1;
            _mockUnitOfWork.Setup(uow => uow.BillingEntry.GetMultiPaging(
                It.IsAny<Expression<Func<BillingEntry, bool>>>(),
                out total,
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string[]>(),
                It.IsAny<Func<IQueryable<BillingEntry>, IOrderedQueryable<BillingEntry>>>()
            )).Returns(billingEntries);
            // Act
            var result = await _billingEntryService.GetAllBillingEntriesAsync(user, pageIndex: 0, pageSize: 2);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.BillingEntries.Count);
            Assert.AreEqual("Entry 1", result.BillingEntries.First().Description);
        }

        [Test]
        public void GetAllBillingEntriesAsync_ShouldThrowException_WhenNoEntriesFound()
        {
            // Arrange
            int totalRecords;
            var total = 1;
            _mockUnitOfWork.Setup(uow => uow.BillingEntry.GetMultiPaging(
                It.IsAny<Expression<Func<BillingEntry, bool>>>(),
                out total,
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string[]>(),
                It.IsAny<Func<IQueryable<BillingEntry>, IOrderedQueryable<BillingEntry>>>()
            )).Returns(new List<BillingEntry>());
            // Act & Assert
            Assert.ThrowsAsync<Exception>(
                async () => await _billingEntryService.GetAllBillingEntriesAsync(null));
        }


        [Test]
        public async Task GetBillingEntryByIdAsync_ShouldReturnBillingEntry_WhenValid()
        {
            // Arrange
            int billingEntryId = 1;
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "userId") }));
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };
            var billingEntry = new BillingEntry
            {
                BillingEntryId = billingEntryId,
                TutorLearnerSubjectId = 2,
                Rate = 100,
                StartDateTime = DateTime.UtcNow.AddHours(-1),
                EndDateTime = DateTime.UtcNow.AddHours(1),
                Description = "Valid Entry",
                TotalAmount = 200
            };

            _mockUserManager.Setup(um => um.GetUserAsync(user)).ReturnsAsync(currentUser);
            _mockUnitOfWork.Setup(u => u.BillingEntry.GetSingleById(billingEntryId)).Returns(billingEntry);

            // Act
            var result = await _billingEntryService.GetBillingEntryByIdAsync(billingEntryId, user);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(billingEntry.BillingEntryId, result.BillingEntryID);
            Assert.AreEqual(billingEntry.Description, result.Description);
        }

        [Test]
        public void GetBillingEntryByIdAsync_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            var user = new ClaimsPrincipal();
            _mockUserManager.Setup(um => um.GetUserAsync(user)).ReturnsAsync((AspNetUser)null);

            // Act & Assert
            Assert.ThrowsAsync<Exception>(
                async () => await _billingEntryService.GetBillingEntryByIdAsync(1, user),
                "User not found.");
        }

        [Test]
        public void GetBillingEntryByIdAsync_ShouldThrowException_WhenBillingEntryNotFound()
        {
            // Arrange
            int billingEntryId = 1;
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "userId") }));
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };

            _mockUserManager.Setup(um => um.GetUserAsync(user)).ReturnsAsync(currentUser);
            _mockUnitOfWork.Setup(u => u.BillingEntry.GetSingleById(billingEntryId)).Returns((BillingEntry)null);

            // Act & Assert
            Assert.ThrowsAsync<Exception>(
                async () => await _billingEntryService.GetBillingEntryByIdAsync(billingEntryId, user),
                "Billing entry not found.");
        }

        [Test]
        public async Task GetAllBillingEntriesByTutorLearnerSubjectIdAsync_ShouldReturnBillingEntries_WhenValid()
        {
            // Arrange
            int tutorLearnerSubjectId = 2;
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "userId") }));
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };
            int totalRecords = 2;
            var billingEntries = new List<BillingEntry>
    {
        new BillingEntry { BillingEntryId = 1, TutorLearnerSubjectId = tutorLearnerSubjectId, Rate = 100, Description = "Entry 1", TotalAmount = 200 },
        new BillingEntry { BillingEntryId = 2, TutorLearnerSubjectId = tutorLearnerSubjectId, Rate = 150, Description = "Entry 2", TotalAmount = 300 }
    };

            _mockUserManager.Setup(um => um.GetUserAsync(user)).ReturnsAsync(currentUser);

            // Act
            var result = await _billingEntryService.GetAllBillingEntriesByTutorLearnerSubjectIdAsync(tutorLearnerSubjectId, user, pageIndex: 0, pageSize: 2);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task GetAllBillingEntriesByTutorLearnerSubjectIdAsync_ShouldReturnEmpty_WhenNoEntriesFound()
        {
            // Arrange
            int tutorLearnerSubjectId = 2;
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "userId") }));
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };
            int totalRecords = 0;

            _mockUserManager.Setup(um => um.GetUserAsync(user)).ReturnsAsync(currentUser);

            // Act
            var result = await _billingEntryService.GetAllBillingEntriesByTutorLearnerSubjectIdAsync(tutorLearnerSubjectId, user, pageIndex: 0, pageSize: 2);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void GetAllBillingEntriesByTutorLearnerSubjectIdAsync_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            var user = new ClaimsPrincipal();
            _mockUserManager.Setup(um => um.GetUserAsync(user)).ReturnsAsync((AspNetUser)null);

            // Act & Assert
            Assert.ThrowsAsync<Exception>(
                async () => await _billingEntryService.GetAllBillingEntriesByTutorLearnerSubjectIdAsync(2, user),
                "User not found.");
        }

        [Test]
        public async Task GetBillingEntryDetailsAsync_ShouldReturnBillingEntryDetails_WhenValid()
        {
            // Arrange
            int tutorLearnerSubjectId = 1;
            var schedules = new List<Schedule>
    {
        new Schedule
        {
            TutorLearnerSubjectId = tutorLearnerSubjectId,
            DayOfWeek = (int)DateTime.Today.DayOfWeek + 1, // Today
            StartTime = new TimeSpan(10, 0, 0), // 10:00 AM
            EndTime = new TimeSpan(12, 0, 0)   // 12:00 PM
        }
    };
            var tutorLearnerSubject = new TutorLearnerSubject
            {
                TutorLearnerSubjectId = tutorLearnerSubjectId,
                PricePerHour = 100
            };

            _mockUnitOfWork.Setup(u => u.schedule.GetMulti(It.IsAny<Expression<Func<Schedule, bool>>>(), It.IsAny<string[]>()))
                           .Returns(schedules);

            _mockUnitOfWork.Setup(u => u.TutorLearnerSubject.FindAsync(It.IsAny<Expression<Func<TutorLearnerSubject, bool>>>()))
                           .ReturnsAsync(tutorLearnerSubject);

            // Act
            var result = await _billingEntryService.GetBillingEntryDetailsAsync(tutorLearnerSubjectId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(100, result.Rate);
            Assert.AreEqual(DateTime.Today.Add(schedules[0].StartTime.Value), result.StartDateTime);
            Assert.AreEqual(DateTime.Today.Add(schedules[0].EndTime.Value), result.EndDateTime);
        }

        [Test]
        public void GetBillingEntryDetailsAsync_ShouldThrowException_WhenSchedulesNotFound()
        {
            // Arrange
            int tutorLearnerSubjectId = 1;

            _mockUnitOfWork.Setup(u => u.schedule.GetMulti(It.IsAny<Expression<Func<Schedule, bool>>>(), It.IsAny<string[]>()))
                           .Returns((List<Schedule>)null);

            // Act & Assert
            Assert.ThrowsAsync<Exception>(
                async () => await _billingEntryService.GetBillingEntryDetailsAsync(tutorLearnerSubjectId),
                "Schedule not found for the specified TutorLearnerSubjectId.");
        }

        [Test]
        public void GetBillingEntryDetailsAsync_ShouldThrowException_WhenRateNotFound()
        {
            // Arrange
            int tutorLearnerSubjectId = 1;
            var schedules = new List<Schedule>
    {
        new Schedule
        {
            TutorLearnerSubjectId = tutorLearnerSubjectId,
            DayOfWeek = (int)DateTime.Today.DayOfWeek + 1, // Today
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(12, 0, 0)
        }
    };

            _mockUnitOfWork.Setup(u => u.schedule.GetMulti(It.IsAny<Expression<Func<Schedule, bool>>>(), It.IsAny<string[]>()))
                           .Returns(schedules);

            _mockUnitOfWork.Setup(u => u.TutorLearnerSubject.FindAsync(It.IsAny<Expression<Func<TutorLearnerSubject, bool>>>()))
                           .ReturnsAsync((TutorLearnerSubject)null);

            // Act & Assert
            Assert.ThrowsAsync<Exception>(
                async () => await _billingEntryService.GetBillingEntryDetailsAsync(tutorLearnerSubjectId),
                "Rate not found for the specified TutorLearnerSubjectId.");
        }

    }

}
