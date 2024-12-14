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
    public class BillingEntryControllerTests
    {
        private Mock<IBillingEntryService> _mockBillingEntryService;
        private BillingEntryController _controller;
        private ClaimsPrincipal _user;

        [SetUp]
        public void Setup()
        {
            _mockBillingEntryService = new Mock<IBillingEntryService>();
            _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.Name, "testuser@example.com"),
            new Claim(ClaimTypes.NameIdentifier, "1")
            }, "mock"));

            _controller = new BillingEntryController(_mockBillingEntryService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = _user }
                }
            };
        }

        #region GetAllBillingEntries Tests

        [Test]
        public async Task GetAllBillingEntries_ThrowsUnauthorizedAccessException_Returns403()
        {
            // Arrange
            _mockBillingEntryService.Setup(s => s.GetAllBillingEntriesAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>(), 0, 20))
                .ThrowsAsync(new UnauthorizedAccessException("Unauthorized access"));

            // Act
            var result = await _controller.GetAllBillingEntries();

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(403, statusCodeResult.StatusCode);
        }

        [Test]
        public async Task GetAllBillingEntries_ThrowsException_Returns500()
        {
            // Arrange
            _mockBillingEntryService.Setup(s => s.GetAllBillingEntriesAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>(), 0, 20))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.GetAllBillingEntries();

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion

        #region GetBillingEntryById Tests

        [Test]
        public async Task GetBillingEntryById_ThrowsUnauthorizedAccessException_Returns403()
        {
            // Arrange
            _mockBillingEntryService.Setup(s => s.GetBillingEntryByIdAsync(It.IsAny<int>(), It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new UnauthorizedAccessException("Unauthorized access"));

            // Act
            var result = await _controller.GetBillingEntryById(1);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(403, statusCodeResult.StatusCode);
        }

        [Test]
        public async Task GetBillingEntryById_ThrowsKeyNotFoundException_Returns404()
        {
            // Arrange
            _mockBillingEntryService.Setup(s => s.GetBillingEntryByIdAsync(It.IsAny<int>(), It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new KeyNotFoundException("Billing entry not found"));

            // Act
            var result = await _controller.GetBillingEntryById(1);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task GetBillingEntryById_ThrowsException_Returns500()
        {
            // Arrange
            _mockBillingEntryService.Setup(s => s.GetBillingEntryByIdAsync(It.IsAny<int>(), It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.GetBillingEntryById(1);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion

        #region AddBillingEntry Tests

        [Test]
        public async Task AddBillingEntry_ThrowsUnauthorizedAccessException_Returns403()
        {
            // Arrange
            var billingEntryDTO = new AdddBillingEntryDTO();
            _mockBillingEntryService.Setup(s => s.AddBillingEntryAsync(billingEntryDTO, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new UnauthorizedAccessException("Unauthorized access"));

            // Act
            var result = await _controller.AddBillingEntry(billingEntryDTO);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(403, statusCodeResult.StatusCode);
        }

        [Test]
        public async Task AddBillingEntry_ThrowsException_Returns500()
        {
            // Arrange
            var billingEntryDTO = new AdddBillingEntryDTO();
            _mockBillingEntryService.Setup(s => s.AddBillingEntryAsync(billingEntryDTO, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.AddBillingEntry(billingEntryDTO);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion

        #region CalculateTotalAmount Tests

        [Test]
        public void CalculateTotalAmount_ThrowsUnauthorizedAccessException_Returns403()
        {
            // Arrange
            var request = new CalculateTotalAmountRequest();
            _mockBillingEntryService.Setup(s => s.CalculateTotalAmount(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<decimal>()))
                .Throws(new UnauthorizedAccessException("Unauthorized access"));

            // Act
            var result = _controller.CalculateTotalAmount(request);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(403, statusCodeResult.StatusCode);
        }

        [Test]
        public void CalculateTotalAmount_ThrowsException_Returns500()
        {
            // Arrange
            var request = new CalculateTotalAmountRequest();
            _mockBillingEntryService.Setup(s => s.CalculateTotalAmount(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<decimal>()))
                .Throws(new Exception("Internal server error"));

            // Act
            var result = _controller.CalculateTotalAmount(request);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion

        #region GetBillingEntriesByTutorLearnerSubjectId Tests

        [Test]
        public async Task GetBillingEntriesByTutorLearnerSubjectId_ThrowsUnauthorizedAccessException_Returns403()
        {
            // Arrange
            _mockBillingEntryService.Setup(s => s.GetAllBillingEntriesByTutorLearnerSubjectIdAsync(It.IsAny<int>(), It.IsAny<System.Security.Claims.ClaimsPrincipal>(), 0, 20))
                .ThrowsAsync(new UnauthorizedAccessException("Unauthorized access"));

            // Act
            var result = await _controller.GetBillingEntriesByTutorLearnerSubjectId(1);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(403, statusCodeResult.StatusCode);
        }

        [Test]
        public async Task GetBillingEntriesByTutorLearnerSubjectId_ThrowsException_Returns500()
        {
            // Arrange
            _mockBillingEntryService.Setup(s => s.GetAllBillingEntriesByTutorLearnerSubjectIdAsync(It.IsAny<int>(), It.IsAny<System.Security.Claims.ClaimsPrincipal>(), 0, 20))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.GetBillingEntriesByTutorLearnerSubjectId(1);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion

        #region AddDraftBillingEntry Tests

        [Test]
        public async Task AddDraftBillingEntry_ThrowsUnauthorizedAccessException_Returns403()
        {
            // Arrange
            var billingEntryDTO = new AdddBillingEntryDTO();
            _mockBillingEntryService.Setup(s => s.AddDraftBillingEntryAsync(billingEntryDTO, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new UnauthorizedAccessException("Unauthorized access"));

            // Act
            var result = await _controller.AddDraftBillingEntry(billingEntryDTO);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(403, statusCodeResult.StatusCode);
        }

        [Test]
        public async Task AddDraftBillingEntry_ThrowsException_Returns500()
        {
            // Arrange
            var billingEntryDTO = new AdddBillingEntryDTO();
            _mockBillingEntryService.Setup(s => s.AddDraftBillingEntryAsync(billingEntryDTO, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.AddDraftBillingEntry(billingEntryDTO);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion

        #region UpdateBillingEntry Tests

        [Test]
        public async Task UpdateBillingEntry_ThrowsUnauthorizedAccessException_Returns403()
        {
            // Arrange
            var billingEntryDTO = new UpdateBillingEntryDTO();
            _mockBillingEntryService.Setup(s => s.UpdateBillingEntryAsync(It.IsAny<int>(), billingEntryDTO, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new UnauthorizedAccessException("Unauthorized access"));

            // Act
            var result = await _controller.UpdateBillingEntry(1, billingEntryDTO);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(403, statusCodeResult.StatusCode);
        }

        [Test]
        public async Task UpdateBillingEntry_ThrowsKeyNotFoundException_Returns404()
        {
            // Arrange
            var billingEntryDTO = new UpdateBillingEntryDTO();
            _mockBillingEntryService.Setup(s => s.UpdateBillingEntryAsync(It.IsAny<int>(), billingEntryDTO, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new KeyNotFoundException("Billing entry not found"));

            // Act
            var result = await _controller.UpdateBillingEntry(1, billingEntryDTO);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task UpdateBillingEntry_ThrowsException_Returns500()
        {
            // Arrange
            var billingEntryDTO = new UpdateBillingEntryDTO();
            _mockBillingEntryService.Setup(s => s.UpdateBillingEntryAsync(It.IsAny<int>(), billingEntryDTO, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.UpdateBillingEntry(1, billingEntryDTO);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion

        #region DeleteBillingEntries Tests

        [Test]
        public async Task DeleteBillingEntries_ThrowsUnauthorizedAccessException_Returns403()
        {
            // Arrange
            var billingEntryIds = new List<int> { 1, 2, 3 };
            _mockBillingEntryService.Setup(s => s.DeleteBillingEntriesAsync(billingEntryIds, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new UnauthorizedAccessException("Unauthorized access"));

            // Act
            var result = await _controller.DeleteBillingEntries(billingEntryIds);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(403, statusCodeResult.StatusCode);
        }

        [Test]
        public async Task DeleteBillingEntries_ThrowsException_Returns500()
        {
            // Arrange
            var billingEntryIds = new List<int> { 1, 2, 3 };
            _mockBillingEntryService.Setup(s => s.DeleteBillingEntriesAsync(billingEntryIds, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.DeleteBillingEntries(billingEntryIds);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion
        #region GetBillingEntryDetails Tests

        [Test]
        public async Task GetBillingEntryDetails_ThrowsUnauthorizedAccessException_Returns403()
        {
            // Arrange
            int tutorLearnerSubjectId = 1;
            _mockBillingEntryService.Setup(s => s.GetBillingEntryDetailsAsync(tutorLearnerSubjectId))
                .ThrowsAsync(new UnauthorizedAccessException("Unauthorized access"));

            // Act
            var result = await _controller.GetBillingEntryDetails(tutorLearnerSubjectId);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(403, statusCodeResult.StatusCode);
            var response = statusCodeResult.Value as ApiResponse<object>;
            Assert.NotNull(response);
            Assert.AreEqual("Unauthorized access", response.Message);
        }

        [Test]
        public async Task GetBillingEntryDetails_ThrowsKeyNotFoundException_Returns404()
        {
            // Arrange
            int tutorLearnerSubjectId = 1;
            _mockBillingEntryService.Setup(s => s.GetBillingEntryDetailsAsync(tutorLearnerSubjectId))
                .ThrowsAsync(new KeyNotFoundException("Billing entry details not found"));

            // Act
            var result = await _controller.GetBillingEntryDetails(tutorLearnerSubjectId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            var response = notFoundResult.Value as ApiResponse<object>;
            Assert.NotNull(response);
            Assert.AreEqual("Billing entry details not found", response.Message);
        }

        [Test]
        public async Task GetBillingEntryDetails_ThrowsException_Returns500()
        {
            // Arrange
            int tutorLearnerSubjectId = 1;
            _mockBillingEntryService.Setup(s => s.GetBillingEntryDetailsAsync(tutorLearnerSubjectId))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.GetBillingEntryDetails(tutorLearnerSubjectId);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            var response = statusCodeResult.Value as ApiResponse<object>;
        }

        [Test]
        public async Task GetBillingEntryDetails_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            int tutorLearnerSubjectId = 1;
            var billingEntryDetails = new BillingEntryDetailsDTO(); // Mock data
            _mockBillingEntryService.Setup(s => s.GetBillingEntryDetailsAsync(tutorLearnerSubjectId))
                .ReturnsAsync(billingEntryDetails);

            // Act
            var result = await _controller.GetBillingEntryDetails(tutorLearnerSubjectId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var response = okResult.Value as ApiResponse<BillingEntryDetailsDTO>;
            Assert.NotNull(response);
            Assert.AreEqual(billingEntryDetails, response.Data);
        }

        #endregion

        #region AddBillingEntry Tests

        [Test]
        public async Task AddBillingEntry_Returns201_WhenSuccessful()
        {
            // Arrange
            var billingEntryDTO = new AdddBillingEntryDTO();
            _mockBillingEntryService.Setup(s => s.AddBillingEntryAsync(billingEntryDTO, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddBillingEntry(billingEntryDTO);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(201, statusCodeResult.StatusCode);
        }

        #endregion

        #region AddDraftBillingEntry Tests

        [Test]
        public async Task AddDraftBillingEntry_Returns201_WhenSuccessful()
        {
            // Arrange
            var billingEntryDTO = new AdddBillingEntryDTO();
            _mockBillingEntryService.Setup(s => s.AddDraftBillingEntryAsync(billingEntryDTO, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddDraftBillingEntry(billingEntryDTO);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(201, statusCodeResult.StatusCode);
        }

        #endregion

        #region UpdateBillingEntry Tests

        [Test]
        public async Task UpdateBillingEntry_Returns200_WhenSuccessful()
        {
            // Arrange
            var billingEntryDTO = new UpdateBillingEntryDTO();
            _mockBillingEntryService.Setup(s => s.UpdateBillingEntryAsync(It.IsAny<int>(), billingEntryDTO, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateBillingEntry(1, billingEntryDTO);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        #endregion

        #region DeleteBillingEntries Tests

        [Test]
        public async Task DeleteBillingEntries_Returns200_WhenSuccessful()
        {
            // Arrange
            var billingEntryIds = new List<int> { 1, 2, 3 };
            _mockBillingEntryService.Setup(s => s.DeleteBillingEntriesAsync(billingEntryIds, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteBillingEntries(billingEntryIds);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        #endregion

        #region GetAllBillingEntriesByTutorLearnerSubjectId Tests

        [Test]
        public async Task GetBillingEntriesByTutorLearnerSubjectId_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            int tutorLearnerSubjectId = 1;
            var billingEntries = new BillingEntryDTOS();
            _mockBillingEntryService.Setup(s => s.GetAllBillingEntriesByTutorLearnerSubjectIdAsync(tutorLearnerSubjectId, It.IsAny<System.Security.Claims.ClaimsPrincipal>(), 0, 20))
                .ReturnsAsync(billingEntries);

            // Act
            var result = await _controller.GetBillingEntriesByTutorLearnerSubjectId(tutorLearnerSubjectId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var response = okResult.Value as ApiResponse<BillingEntryDTOS>;
            Assert.NotNull(response);
            Assert.AreEqual(billingEntries, response.Data);
        }

        #endregion
    }
}

