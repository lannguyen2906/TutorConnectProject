using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Models;
using TutoRum.FE.Common;
using TutoRum.FE.Controllers;
using TutoRum.Services.IService;

namespace TutoRum.UnitTests.TutoRum.FE.UnitTest.Controller
{
    [TestFixture]
    public class RateRangeControllerTests
    {
        private Mock<IRateRangeService> _mockRateRangeService;
        private RateRangeController _controller;

        [SetUp]
        public void Setup()
        {
            _mockRateRangeService = new Mock<IRateRangeService>();
            _controller = new RateRangeController(_mockRateRangeService.Object);
        }

        #region CreateRateRange Tests

        [Test]
        public async Task CreateRateRange_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var rateRange = new RateRange();
            _mockRateRangeService.Setup(s => s.CreateRateRangeAsync(rateRange)).ReturnsAsync(rateRange);

            // Act
            var result = await _controller.CreateRateRange(rateRange);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as ApiResponse<RateRange>;
            Assert.NotNull(response);
            Assert.AreEqual(rateRange, response.Data);
        }

        [Test]
        public async Task CreateRateRange_ThrowsException_Returns500()
        {
            // Arrange
            var rateRange = new RateRange();
            _mockRateRangeService.Setup(s => s.CreateRateRangeAsync(rateRange))
                .ThrowsAsync(new Exception("Internal error"));

            // Act
            var result = await _controller.CreateRateRange(rateRange);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion

        #region GetAllRateRanges Tests

        [Test]
        public async Task GetAllRateRanges_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var rateRanges = new List<RateRange> { new RateRange() };
            _mockRateRangeService.Setup(s => s.GetAllRateRangesAsync()).ReturnsAsync(rateRanges);

            // Act
            var result = await _controller.GetAllRateRanges();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as ApiResponse<IEnumerable<RateRange>>;
            Assert.NotNull(response);
            Assert.AreEqual(rateRanges, response.Data);
        }

        [Test]
        public async Task GetAllRateRanges_ThrowsException_Returns500()
        {
            // Arrange
            _mockRateRangeService.Setup(s => s.GetAllRateRangesAsync())
                .ThrowsAsync(new Exception("Internal error"));

            // Act
            var result = await _controller.GetAllRateRanges();

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion

        #region GetRateRangeById Tests

        [Test]
        public async Task GetRateRangeById_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var rateRange = new RateRange();
            _mockRateRangeService.Setup(s => s.GetRateRangeByIdAsync(1)).ReturnsAsync(rateRange);

            // Act
            var result = await _controller.GetRateRangeById(1);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as ApiResponse<RateRange>;
            Assert.NotNull(response);
            Assert.AreEqual(rateRange, response.Data);
        }

        [Test]
        public async Task GetRateRangeById_ThrowsKeyNotFoundException_Returns404()
        {
            // Arrange
            _mockRateRangeService.Setup(s => s.GetRateRangeByIdAsync(1))
                .ThrowsAsync(new KeyNotFoundException("Rate range not found"));

            // Act
            var result = await _controller.GetRateRangeById(1);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task GetRateRangeById_ThrowsException_Returns500()
        {
            // Arrange
            _mockRateRangeService.Setup(s => s.GetRateRangeByIdAsync(1))
                .ThrowsAsync(new Exception("Internal error"));

            // Act
            var result = await _controller.GetRateRangeById(1);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion

        #region UpdateRateRange Tests

        [Test]
        public async Task UpdateRateRange_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var rateRange = new RateRange();
            _mockRateRangeService.Setup(s => s.UpdateRateRangeAsync(1, rateRange)).ReturnsAsync(rateRange);

            // Act
            var result = await _controller.UpdateRateRange(1, rateRange);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as ApiResponse<RateRange>;
            Assert.NotNull(response);
            Assert.AreEqual(rateRange, response.Data);
        }

        [Test]
        public async Task UpdateRateRange_ThrowsKeyNotFoundException_Returns404()
        {
            // Arrange
            var rateRange = new RateRange();
            _mockRateRangeService.Setup(s => s.UpdateRateRangeAsync(1, rateRange))
                .ThrowsAsync(new KeyNotFoundException("Rate range not found"));

            // Act
            var result = await _controller.UpdateRateRange(1, rateRange);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task UpdateRateRange_ThrowsException_Returns500()
        {
            // Arrange
            var rateRange = new RateRange();
            _mockRateRangeService.Setup(s => s.UpdateRateRangeAsync(1, rateRange))
                .ThrowsAsync(new Exception("Internal error"));

            // Act
            var result = await _controller.UpdateRateRange(1, rateRange);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion

        #region DeleteRateRange Tests

        [Test]
        public async Task DeleteRateRange_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            _mockRateRangeService.Setup(s => s.DeleteRateRangeAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteRateRange(1);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as ApiResponse<object>;
            Assert.NotNull(response);
            Assert.IsNull(response.Data);
        }

        [Test]
        public async Task DeleteRateRange_ThrowsKeyNotFoundException_Returns404()
        {
            // Arrange
            _mockRateRangeService.Setup(s => s.DeleteRateRangeAsync(1))
                .ThrowsAsync(new KeyNotFoundException("Rate range not found"));

            // Act
            var result = await _controller.DeleteRateRange(1);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task DeleteRateRange_ThrowsException_Returns500()
        {
            // Arrange
            _mockRateRangeService.Setup(s => s.DeleteRateRangeAsync(1))
                .ThrowsAsync(new Exception("Internal error"));

            // Act
            var result = await _controller.DeleteRateRange(1);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion
    }
}
