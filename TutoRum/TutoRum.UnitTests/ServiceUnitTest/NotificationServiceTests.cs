using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Enum;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.Models;
using TutoRum.Services.IService;
using TutoRum.Services.Service;
using TutoRum.Services.ViewModels;

namespace TutoRum.UnitTests.ServiceUnitTest
{
    [TestFixture]
    public class NotificationServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IFirebaseService> _firebaseServiceMock;
        private Mock<IUserTokenService> _userTokenServiceMock;
        private Mock<UserManager<AspNetUser>> _userManagerMock;
        private NotificationService _notificationService;
        private Guid userId = Guid.NewGuid();
        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _firebaseServiceMock = new Mock<IFirebaseService>();
            _userTokenServiceMock = new Mock<IUserTokenService>();
            _userManagerMock = new Mock<UserManager<AspNetUser>>(
                Mock.Of<IUserStore<AspNetUser>>(), null, null, null, null, null, null, null, null);
            _unitOfWorkMock.Setup(st => st.Notifications.Add(It.IsAny<Notification>()));

            _notificationService = new NotificationService(
                _unitOfWorkMock.Object,
                _firebaseServiceMock.Object,
                _userTokenServiceMock.Object,
                _userManagerMock.Object);
        }

        // Test for GetNotificationsByUserAsync method
        [Test]
        public async Task GetNotificationsByUserAsync_UserNotFound_ThrowsException()
        {
            // Arrange
            var claimsPrincipal = new ClaimsPrincipal();
            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((AspNetUser)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _notificationService.GetNotificationsByUserAsync(claimsPrincipal));
            Assert.AreEqual("User not found.", ex.Message);
        }

        [Test]
        public async Task GetNotificationsByUserAsync_ValidUser_ReturnsNotifications()
        {
            // Arrange
            var currentUser = new AspNetUser { Id = userId };
            var notifications = new List<Notification>
        {
            new Notification { NotificationId = 1, UserId = userId, Title = "Test", Description = "Test Notification", IsRead = false, CreatedDate = DateTime.UtcNow }
        };
            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);
            var totalRecords = 1;
            _unitOfWorkMock.Setup(uow => uow.Notifications.GetMultiPaging(
                It.IsAny<Expression<Func<Notification, bool>>>(),
                out totalRecords,
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string[]>(),
                It.IsAny<Func<IQueryable<Notification>, IOrderedQueryable<Notification>>>()
            )).Returns(notifications);
            _unitOfWorkMock.Setup(u => u.Notifications.Count(It.IsAny< Expression<Func<Notification, bool>>>())).Returns(1);

            // Act
            var result = await _notificationService.GetNotificationsByUserAsync(new ClaimsPrincipal());

            // Assert
            Assert.AreEqual(1, result.TotalRecords);
            Assert.AreEqual(1, result.Notifications.Count);
            Assert.AreEqual("Test", result.Notifications.First().Title);
        }

        // Test for MarkNotificationAsReadAsync method
        [Test]
        public async Task MarkNotificationAsReadAsync_CallsMarkNotificationsAsReadAsync()
        {
            // Arrange
            var notificationIds = new List<int> { 1, 2, 3 };
            _unitOfWorkMock.Setup(u => u.Notifications.MarkNotificationsAsReadAsync(It.IsAny<List<int>>())).Returns(Task.CompletedTask);

            // Act
            await _notificationService.MarkNotificationAsReadAsync(notificationIds);

            // Assert
            _unitOfWorkMock.Verify(u => u.Notifications.MarkNotificationsAsReadAsync(It.IsAny<List<int>>()), Times.Once);
        }

        // Test for SendNotificationAsync method (sending to a specific user)
        [Test]
        public async Task SendNotificationAsync_SendsNotificationToUser()
        {
            // Arrange
            var notificationRequestDto = new NotificationRequestDto
            {
                UserId = Guid.NewGuid(),
                Title = "Test Notification",
                Description = "This is a test notification.",
                Href = "/test",
                Icon = "icon",
                Color = "red",
                NotificationType = NotificationType.AdminTutorApproval
            };

            var user = new AspNetUser { Id = userId, UserName = "testuser" };
            var userTokens = new List<string> { "token123" };

            _userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            _userTokenServiceMock.Setup(u => u.GetUserTokensByUserIdAsync(It.IsAny<Guid>())).ReturnsAsync(userTokens);
            _firebaseServiceMock.Setup(f => f.SendNotificationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>())).ReturnsAsync(true);

            // Act
            await _notificationService.SendNotificationAsync(notificationRequestDto, sendToAdmins: false);

            // Assert
            _unitOfWorkMock.Verify(u => u.Notifications.Add(It.IsAny<Notification>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
            _firebaseServiceMock.Verify(f => f.SendNotificationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()), Times.Once);
        }

        // Test for SendNotificationAsync method (sending to admins)
        [Test]
        public async Task SendNotificationAsync_SendsNotificationToAdmins()
        {
            // Arrange
            var notificationRequestDto = new NotificationRequestDto
            {
                UserId = Guid.NewGuid(),
                Title = "Test Notification to Admins",
                Description = "This is a test notification for all admins.",
                Href = "/test",
                Icon = "icon",
                Color = "blue",
                NotificationType = NotificationType.AdminTutorApproval
            };

            var adminUser = new AspNetUser { Id = userId, UserName = "adminuser" };
            var adminUsers = new List<AspNetUser> { adminUser };
            var userTokens = new List<string> { "adminToken123" };

            _userManagerMock.Setup(u => u.GetUsersInRoleAsync(It.IsAny<string>())).ReturnsAsync(adminUsers);
            _userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(adminUser);
            _userTokenServiceMock.Setup(u => u.GetUserTokensByUserIdAsync(It.IsAny<Guid>())).ReturnsAsync(userTokens);
            _firebaseServiceMock.Setup(f => f.SendNotificationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>())).ReturnsAsync(true);

            // Act
            await _notificationService.SendNotificationAsync(notificationRequestDto, sendToAdmins: true);

            // Assert
            _unitOfWorkMock.Verify(u => u.Notifications.Add(It.IsAny<Notification>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
            _firebaseServiceMock.Verify(f => f.SendNotificationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()), Times.Once);
        }
    }
}