using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Models;
using TutoRum.FE.Common;
using TutoRum.FE.Controllers;
using TutoRum.Services.IService;
using TutoRum.Services.ViewModels;
using static TutoRum.FE.Common.Url;

namespace TutoRum.UnitTests.TutoRum.FE.UnitTest.Controller
{
    [TestFixture]
    public class SubjectControllerTests
    {
        private Mock<ISubjectService> _mockSubjectService;
        private SubjectController _controller;
        private ClaimsPrincipal _user;


        [SetUp]
        public void Setup()
        {
            _mockSubjectService = new Mock<ISubjectService>();

            _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.Name, "testuser@example.com"),
            new Claim(ClaimTypes.NameIdentifier, "1")
            }, "mock"));

            _controller = new SubjectController(_mockSubjectService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = _user }
                }
            };
        }

        #region GetAllSubjectAsync Tests

        [Test]
        public async Task GetAllSubjectAsync_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var subjects = new List<SubjectFilterDTO>();
            _mockSubjectService.Setup(s => s.GetAllSubjectAsync()).ReturnsAsync(subjects);

            // Act
            var result = await _controller.GetAllSubjectAsync();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public async Task GetAllSubjectAsync_ThrowsUnauthorizedAccessException_Returns403()
        {
            // Arrange
            _mockSubjectService.Setup(s => s.GetAllSubjectAsync())
                .ThrowsAsync(new UnauthorizedAccessException("Unauthorized access"));

            // Act
            var result = await _controller.GetAllSubjectAsync();

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(403, statusCodeResult.StatusCode);
        }

        [Test]
        public async Task GetAllSubjectAsync_ThrowsKeyNotFoundException_Returns404()
        {
            // Arrange
            _mockSubjectService.Setup(s => s.GetAllSubjectAsync())
                .ThrowsAsync(new KeyNotFoundException("Subjects not found"));

            // Act
            var result = await _controller.GetAllSubjectAsync();

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        #endregion

        #region CreateSubjectAsync Tests

        [Test]
        public async Task CreateSubjectAsync_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var subjectDto = new SubjectDTO();
            var createdSubject = new Subject();
            _mockSubjectService.Setup(s => s.CreateSubjectAsync(subjectDto, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(createdSubject);

            // Act
            var result = await _controller.CreateSubjectAsync(subjectDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public async Task CreateSubjectAsync_ThrowsArgumentException_Returns400()
        {
            // Arrange
            var subjectDto = new SubjectDTO();
            _mockSubjectService.Setup(s => s.CreateSubjectAsync(subjectDto, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new ArgumentException("Invalid data"));

            // Act
            var result = await _controller.CreateSubjectAsync(subjectDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        #endregion

        #region GetTopSubject Tests

        [Test]
        public async Task GetTopSubject_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            int size = 5;
            var subjects = new List<SubjectFilterDTO>();
            _mockSubjectService.Setup(s => s.GetTopSubjectsAsync(size)).ReturnsAsync(subjects);

            // Act
            var result = await _controller.GetTopSubject(size);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public async Task GetTopSubject_ThrowsArgumentException_Returns400()
        {
            // Arrange
            int size = 5;
            _mockSubjectService.Setup(s => s.GetTopSubjectsAsync(size)).ThrowsAsync(new ArgumentException("Invalid size"));

            // Act
            var result = await _controller.GetTopSubject(size);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        #endregion

        #region GetSubjectByIdAsync Tests

        [Test]
        public async Task GetSubjectByIdAsync_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            int subjectId = 1;
            var subject = new Subject();
            _mockSubjectService.Setup(s => s.GetSubjectByIdAsync(subjectId)).ReturnsAsync(subject);

            // Act
            var result = await _controller.GetSubjectByIdAsync(subjectId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public async Task GetSubjectByIdAsync_ThrowsException_Returns404()
        {
            // Arrange
            int subjectId = 1;
            _mockSubjectService.Setup(s => s.GetSubjectByIdAsync(subjectId))
                .ThrowsAsync(new Exception("Subject not found"));

            // Act
            var result = await _controller.GetSubjectByIdAsync(subjectId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        #endregion

        #region UpdateSubjectAsync Tests

        [Test]
        public async Task UpdateSubjectAsync_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            int subjectId = 1;
            var subjectDto = new SubjectDTO();
            _mockSubjectService.Setup(s => s.UpdateSubjectAsync(subjectId, subjectDto, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateSubjectAsync(subjectId, subjectDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public async Task UpdateSubjectAsync_ThrowsException_Returns404()
        {
            // Arrange
            int subjectId = 1;
            var subjectDto = new SubjectDTO();
            _mockSubjectService.Setup(s => s.UpdateSubjectAsync(subjectId, subjectDto, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new Exception("Subject not found"));

            // Act
            var result = await _controller.UpdateSubjectAsync(subjectId, subjectDto);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        #endregion

        #region DeleteSubjectAsync Tests

        [Test]
        public async Task DeleteSubjectAsync_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            int subjectId = 1;
            _mockSubjectService.Setup(s => s.DeleteSubjectAsync(subjectId, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteSubjectAsync(subjectId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public async Task DeleteSubjectAsync_ThrowsException_Returns404()
        {
            // Arrange
            int subjectId = 1;
            _mockSubjectService.Setup(s => s.DeleteSubjectAsync(subjectId, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new Exception("Subject not found"));

            // Act
            var result = await _controller.DeleteSubjectAsync(subjectId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        #endregion

        #region GetAllSubjectAsync Tests

        [Test]
        public async Task GetAllSubjectAsync_ThrowsException_Returns500()
        {
            // Arrange
            _mockSubjectService.Setup(s => s.GetAllSubjectAsync())
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.GetAllSubjectAsync();

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion

        #region CreateSubjectAsync Tests


       

        [Test]
        public async Task CreateSubjectAsync_ThrowsException_Returns500()
        {
            // Arrange
            var subjectDto = new SubjectDTO();
            _mockSubjectService.Setup(s => s.CreateSubjectAsync(subjectDto, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.CreateSubjectAsync(subjectDto);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion

        #region GetTopSubject Tests

        

        [Test]
        public async Task GetTopSubject_ThrowsUnauthorizedAccessException_Returns403()
        {
            // Arrange
            int size = 5;
            _mockSubjectService.Setup(s => s.GetTopSubjectsAsync(size))
                .ThrowsAsync(new UnauthorizedAccessException("Unauthorized access"));

            // Act
            var result = await _controller.GetTopSubject(size);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(403, statusCodeResult.StatusCode);
        }

        [Test]
        public async Task GetTopSubject_ThrowsKeyNotFoundException_Returns404()
        {
            // Arrange
            int size = 5;
            _mockSubjectService.Setup(s => s.GetTopSubjectsAsync(size))
                .ThrowsAsync(new KeyNotFoundException("Subjects not found"));

            // Act
            var result = await _controller.GetTopSubject(size);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task GetTopSubject_ThrowsException_Returns500()
        {
            // Arrange
            int size = 5;
            _mockSubjectService.Setup(s => s.GetTopSubjectsAsync(size))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.GetTopSubject(size);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion
    }
}
