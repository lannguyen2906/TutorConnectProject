using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.FE.Common;
using TutoRum.FE.Controllers;
using TutoRum.Services.IService;
using TutoRum.Services.ViewModels;

namespace TutoRum.UnitTests.TutoRum.FE.UnitTest.Controller
{
    [TestFixture]
    public class NotificationControllerTests
    {
        private Mock<IFirebaseService> _mockFirebaseService;
        private Mock<INotificationService> _mockNotificationService;
        private Mock<IUserTokenService> _mockUserTokenService;
        private NotificationController _controller;

        [SetUp]
        public void Setup()
        {
            _mockFirebaseService = new Mock<IFirebaseService>();
            _mockNotificationService = new Mock<INotificationService>();
            _mockUserTokenService = new Mock<IUserTokenService>();
            _controller = new NotificationController(
                _mockFirebaseService.Object,
                _mockNotificationService.Object,
                _mockUserTokenService.Object
            );
        }

        #region GetAllNotifications Tests

        [Test]
        public async Task GetAllNotifications_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var notifications = new NotificationDtos();
            _mockNotificationService.Setup(s => s.GetNotificationsByUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>(), 0, 10))
                .ReturnsAsync(notifications);

            // Act
            var result = await _controller.GetAllNotifications(0, 10);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as ApiResponse<NotificationDtos>;
            Assert.NotNull(response);
            Assert.AreEqual(notifications, response.Data);
        }

        [Test]
        public async Task GetAllNotifications_ThrowsUnauthorizedAccessException_Returns403()
        {
            // Arrange
            _mockNotificationService.Setup(s => s.GetNotificationsByUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>(), 0, 10))
                .ThrowsAsync(new UnauthorizedAccessException("Unauthorized"));

            // Act
            var result = await _controller.GetAllNotifications(0, 10);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(403, statusCodeResult.StatusCode);
        }

        [Test]
        public async Task GetAllNotifications_ThrowsException_Returns500()
        {
            // Arrange
            _mockNotificationService.Setup(s => s.GetNotificationsByUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>(), 0, 10))
                .ThrowsAsync(new Exception("Internal error"));

            // Act
            var result = await _controller.GetAllNotifications(0, 10);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion

        #region SendNotification Tests

        [Test]
        public async Task SendNotification_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var request = new NotificationRequestDto();
            _mockNotificationService.Setup(s => s.SendNotificationAsync(request, false))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.SendNotification(request);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as ApiResponse<NotificationRequestDto>;
            Assert.NotNull(response);
            Assert.AreEqual(request, response.Data);
        }

        [Test]
        public async Task SendNotification_ThrowsUnauthorizedAccessException_Returns403()
        {
            // Arrange
            var request = new NotificationRequestDto();
            _mockNotificationService.Setup(s => s.SendNotificationAsync(request, false))
                .ThrowsAsync(new UnauthorizedAccessException("Unauthorized"));

            // Act
            var result = await _controller.SendNotification(request);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(403, statusCodeResult.StatusCode);
        }

        [Test]
        public async Task SendNotification_ThrowsException_Returns500()
        {
            // Arrange
            var request = new NotificationRequestDto();
            _mockNotificationService.Setup(s => s.SendNotificationAsync(request, false))
                .ThrowsAsync(new Exception("Internal error"));

            // Act
            var result = await _controller.SendNotification(request);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion

        #region SaveFCMToken Tests

        [Test]
        public async Task SaveFCMToken_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var dto = new UserTokenDto { UserId = Guid.NewGuid(), Token = "token", DeviceType = "mobile" };
            _mockUserTokenService.Setup(s => s.SaveTokenAsync(dto.UserId, dto.Token, dto.DeviceType))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.SaveFCMToken(dto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as ApiResponse<string>;
            Assert.NotNull(response);
            Assert.AreEqual("Save FCM Token success", response.Data);
        }

        [Test]
        public async Task SaveFCMToken_ThrowsUnauthorizedAccessException_Returns403()
        {
            // Arrange
            var dto = new UserTokenDto { UserId = Guid.NewGuid(), Token = "token", DeviceType = "mobile" };
            _mockUserTokenService.Setup(s => s.SaveTokenAsync(dto.UserId, dto.Token, dto.DeviceType))
                .ThrowsAsync(new UnauthorizedAccessException("Unauthorized"));

            // Act
            var result = await _controller.SaveFCMToken(dto);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(403, statusCodeResult.StatusCode);
        }

        [Test]
        public async Task SaveFCMToken_ThrowsException_Returns500()
        {
            // Arrange
            var dto = new UserTokenDto { UserId = Guid.NewGuid(), Token = "token", DeviceType = "mobile" };
            _mockUserTokenService.Setup(s => s.SaveTokenAsync(dto.UserId, dto.Token, dto.DeviceType))
                .ThrowsAsync(new Exception("Internal error"));

            // Act
            var result = await _controller.SaveFCMToken(dto);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion

        #region MarkNotificationsAsRead Tests

        [Test]
        public async Task MarkNotificationsAsRead_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var ids = new List<int> { 1, 2, 3 };
            _mockNotificationService.Setup(s => s.MarkNotificationAsReadAsync(ids))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.MarkNotificationsAsRead(ids);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as ApiResponse<string>;
            Assert.NotNull(response);
            Assert.AreEqual("Mark all notifications as read", response.Data);
        }

        [Test]
        public async Task MarkNotificationsAsRead_ThrowsUnauthorizedAccessException_Returns403()
        {
            // Arrange
            var ids = new List<int> { 1, 2, 3 };
            _mockNotificationService.Setup(s => s.MarkNotificationAsReadAsync(ids))
                .ThrowsAsync(new UnauthorizedAccessException("Unauthorized"));

            // Act
            var result = await _controller.MarkNotificationsAsRead(ids);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(403, statusCodeResult.StatusCode);
        }

        [Test]
        public async Task MarkNotificationsAsRead_ThrowsException_Returns500()
        {
            // Arrange
            var ids = new List<int> { 1, 2, 3 };
            _mockNotificationService.Setup(s => s.MarkNotificationAsReadAsync(ids))
                .ThrowsAsync(new Exception("Internal error"));

            // Act
            var result = await _controller.MarkNotificationsAsRead(ids);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion
    }
}
