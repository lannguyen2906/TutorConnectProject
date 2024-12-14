using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.FE.Common;
using TutoRum.FE.Controllers;
using TutoRum.Services.IService;
using TutoRum.Services.ViewModels;
using static TutoRum.FE.Common.Url;

namespace TutoRum.UnitTests.TutoRum.FE.UnitTest.Controller
{
    [TestFixture]
    public class ScheduleControllerTests
    {
        private Mock<IScheduleService> _mockScheduleService;
        private ScheduleController _controller;
        private ClaimsPrincipal _user;

        [SetUp]
        public void Setup()
        {
            _mockScheduleService = new Mock<IScheduleService>();

            _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
          {
            new Claim(ClaimTypes.Name, "testuser@example.com"),
            new Claim(ClaimTypes.NameIdentifier, "1")
          }, "mock"));

            _controller = new ScheduleController(_mockScheduleService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = _user }
                }
            };
        }

        #region GetSchedulesByTutorId Tests

        [Test]
        public async Task GetSchedulesByTutorId_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var schedules = new List<ScheduleGroupDTO>
            {
                new ScheduleGroupDTO { /* Populate test data if needed */ }
            };
            _mockScheduleService.Setup(s => s.GetSchedulesByTutorIdAsync(tutorId)).ReturnsAsync(schedules);

            // Act
            var result = await _controller.GetSchedulesByTutorId(tutorId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as ApiResponse<List<ScheduleGroupDTO>>;
            Assert.NotNull(response);
            Assert.AreEqual(schedules, response.Data);
        }

        [Test]
        public async Task GetSchedulesByTutorId_ThrowsKeyNotFoundException_Returns404()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            _mockScheduleService.Setup(s => s.GetSchedulesByTutorIdAsync(tutorId))
                .ThrowsAsync(new KeyNotFoundException("Schedules not found"));

            // Act
            var result = await _controller.GetSchedulesByTutorId(tutorId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task GetSchedulesByTutorId_ThrowsException_Returns500()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            _mockScheduleService.Setup(s => s.GetSchedulesByTutorIdAsync(tutorId))
                .ThrowsAsync(new Exception("Internal error"));

            // Act
            var result = await _controller.GetSchedulesByTutorId(tutorId);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion

        #region AddScheduleAsync Tests

        [Test]
        public async Task AddScheduleAsync_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var newSchedule = new AddScheduleDTO();

            _mockScheduleService.Setup(s => s.AddSchedule(tutorId, newSchedule)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddScheduleAsync(tutorId, newSchedule);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as ApiResponse<string>;
            Assert.NotNull(response);
            Assert.AreEqual("Schedule added successfully.", response.Data);
        }

        [Test]
        public async Task AddScheduleAsync_ThrowsUnauthorizedAccessException_Returns403()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var newSchedule = new AddScheduleDTO();
            _mockScheduleService.Setup(s => s.AddSchedule(tutorId, newSchedule))
                .ThrowsAsync(new UnauthorizedAccessException("Unauthorized"));

            // Act
            var result = await _controller.AddScheduleAsync(tutorId, newSchedule);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(403, statusCodeResult.StatusCode);
        }

        [Test]
        public async Task AddScheduleAsync_ThrowsException_Returns500()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var newSchedule = new AddScheduleDTO();
            _mockScheduleService.Setup(s => s.AddSchedule(tutorId, newSchedule))
                .ThrowsAsync(new Exception("Internal error"));

            // Act
            var result = await _controller.AddScheduleAsync(tutorId, newSchedule);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion

        #region DeleteScheduleAsync Tests

        [Test]
        public async Task DeleteScheduleAsync_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var scheduleToDelete = new DeleteScheduleDTO();

            _mockScheduleService.Setup(s => s.DeleteSchedule(tutorId, scheduleToDelete)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteScheduleAsync(tutorId, scheduleToDelete);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as ApiResponse<string>;
            Assert.NotNull(response);
            Assert.AreEqual("Schedule deleted successfully.", response.Data);
        }

        [Test]
        public async Task DeleteScheduleAsync_ThrowsKeyNotFoundException_Returns404()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var scheduleToDelete = new DeleteScheduleDTO();
            _mockScheduleService.Setup(s => s.DeleteSchedule(tutorId, scheduleToDelete))
                .ThrowsAsync(new KeyNotFoundException("Schedule not found"));

            // Act
            var result = await _controller.DeleteScheduleAsync(tutorId, scheduleToDelete);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task DeleteScheduleAsync_ThrowsException_Returns500()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var scheduleToDelete = new DeleteScheduleDTO();
            _mockScheduleService.Setup(s => s.DeleteSchedule(tutorId, scheduleToDelete))
                .ThrowsAsync(new Exception("Internal error"));

            // Act
            var result = await _controller.DeleteScheduleAsync(tutorId, scheduleToDelete);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion

        #region UpdateScheduleAsync Tests

        [Test]
        public async Task UpdateScheduleAsync_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var updatedSchedule = new UpdateScheduleDTO();

            _mockScheduleService.Setup(s => s.UpdateSchedule(tutorId, updatedSchedule)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateScheduleAsync(tutorId, updatedSchedule);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as ApiResponse<string>;
            Assert.NotNull(response);
            Assert.AreEqual("Schedule updated successfully.", response.Data);
        }

        [Test]
        public async Task UpdateScheduleAsync_ThrowsKeyNotFoundException_Returns404()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var updatedSchedule = new UpdateScheduleDTO();
            _mockScheduleService.Setup(s => s.UpdateSchedule(tutorId, updatedSchedule))
                .ThrowsAsync(new KeyNotFoundException("Schedule not found"));

            // Act
            var result = await _controller.UpdateScheduleAsync(tutorId, updatedSchedule);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task UpdateScheduleAsync_ThrowsException_Returns500()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var updatedSchedule = new UpdateScheduleDTO();
            _mockScheduleService.Setup(s => s.UpdateSchedule(tutorId, updatedSchedule))
                .ThrowsAsync(new Exception("Internal error"));

            // Act
            var result = await _controller.UpdateScheduleAsync(tutorId, updatedSchedule);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion
    }
}
