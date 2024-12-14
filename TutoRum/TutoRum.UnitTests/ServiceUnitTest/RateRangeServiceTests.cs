using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.Models;
using TutoRum.Services.Service;

namespace TutoRum.UnitTests.ServiceUnitTest
{
    [TestFixture]
    public class RateRangeServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private RateRangeService _rateRangeService;

        [SetUp]
        public void SetUp()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _rateRangeService = new RateRangeService(_mockUnitOfWork.Object);
        }

        [Test]
        public async Task CreateRateRangeAsync_ShouldAddRateRange_WhenValid()
        {
            // Arrange
            var rateRange = new RateRange
            {
                MinRate = 100,
                MaxRate = 200,
                Level = "Basic",
                Description = "Basic Level"
            };

            _mockUnitOfWork.Setup(uow => uow.RateRange.Add(It.IsAny<RateRange>()));
            _mockUnitOfWork.Setup(uow => uow.CommitAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _rateRangeService.CreateRateRangeAsync(rateRange);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Basic", result.Level);
            _mockUnitOfWork.Verify(uow => uow.RateRange.Add(rateRange), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
        }

        [Test]
        public void CreateRateRangeAsync_ShouldThrowException_WhenInvalidRates()
        {
            // Arrange
            var rateRange = new RateRange
            {
                MinRate = 300,
                MaxRate = 200,
                Level = "Invalid",
                Description = "Invalid Range"
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _rateRangeService.CreateRateRangeAsync(rateRange));
            Assert.AreEqual("Invalid rate range values.", ex.Message);
        }

        [Test]
        public async Task GetAllRateRangesAsync_ShouldReturnAllRateRanges()
        {
            // Arrange
            var rateRanges = new List<RateRange>
            {
                new RateRange { Id = 1, MinRate = 100, MaxRate = 200, Level = "Basic" },
                new RateRange { Id = 2, MinRate = 200, MaxRate = 300, Level = "Advanced" }
            };

            _mockUnitOfWork.Setup(uow => uow.RateRange.GetAll(It.IsAny<string[]>())).Returns(rateRanges);

            // Act
            var result = await _rateRangeService.GetAllRateRangesAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Basic", result.First().Level);
        }

        [Test]
        public async Task GetRateRangeByIdAsync_ShouldReturnRateRange_WhenFound()
        {
            // Arrange
            var rateRange = new RateRange { Id = 1, MinRate = 100, MaxRate = 200, Level = "Basic" };

            _mockUnitOfWork.Setup(uow => uow.RateRange.GetSingleById(1)).Returns(rateRange);

            // Act
            var result = await _rateRangeService.GetRateRangeByIdAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Basic", result.Level);
        }

        [Test]
        public void GetRateRangeByIdAsync_ShouldThrowException_WhenNotFound()
        {
            // Arrange
            _mockUnitOfWork.Setup(uow => uow.RateRange.GetSingleById(1)).Returns((RateRange)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _rateRangeService.GetRateRangeByIdAsync(1));
            Assert.AreEqual("Rate range not found.", ex.Message);
        }

        [Test]
        public async Task UpdateRateRangeAsync_ShouldUpdateRateRange_WhenValid()
        {
            // Arrange
            var existingRateRange = new RateRange { Id = 1, MinRate = 100, MaxRate = 200, Level = "Basic" };
            var updatedRateRange = new RateRange { MinRate = 150, MaxRate = 250, Level = "Advanced", Description = "Updated" };

            _mockUnitOfWork.Setup(uow => uow.RateRange.GetSingleById(1)).Returns(existingRateRange);
            _mockUnitOfWork.Setup(uow => uow.CommitAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _rateRangeService.UpdateRateRangeAsync(1, updatedRateRange);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Advanced", result.Level);
            Assert.AreEqual(150, result.MinRate);
            Assert.AreEqual(250, result.MaxRate);
            _mockUnitOfWork.Verify(uow => uow.RateRange.Update(existingRateRange), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
        }

        [Test]
        public void UpdateRateRangeAsync_ShouldThrowException_WhenInvalidRates()
        {
            // Arrange
            var existingRateRange = new RateRange { Id = 1, MinRate = 100, MaxRate = 200, Level = "Basic" };
            var updatedRateRange = new RateRange { MinRate = 300, MaxRate = 200, Level = "Invalid" };

            _mockUnitOfWork.Setup(uow => uow.RateRange.GetSingleById(1)).Returns(existingRateRange);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _rateRangeService.UpdateRateRangeAsync(1, updatedRateRange));
            Assert.AreEqual("Invalid rate range values.", ex.Message);
        }

        [Test]
        public void UpdateRateRangeAsync_ShouldThrowException_WhenNotFound()
        {
            // Arrange
            var updatedRateRange = new RateRange { MinRate = 150, MaxRate = 250, Level = "Advanced" };

            _mockUnitOfWork.Setup(uow => uow.RateRange.GetSingleById(1)).Returns((RateRange)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _rateRangeService.UpdateRateRangeAsync(1, updatedRateRange));
            Assert.AreEqual("Rate range not found.", ex.Message);
        }

        [Test]
        public async Task DeleteRateRangeAsync_ShouldDeleteRateRange_WhenFound()
        {
            // Arrange
            var rateRange = new RateRange { Id = 1, MinRate = 100, MaxRate = 200, Level = "Basic" };

            _mockUnitOfWork.Setup(uow => uow.RateRange.GetSingleById(1)).Returns(rateRange);
            _mockUnitOfWork.Setup(uow => uow.CommitAsync()).Returns(Task.CompletedTask);

            // Act
            await _rateRangeService.DeleteRateRangeAsync(1);

            // Assert
            _mockUnitOfWork.Verify(uow => uow.RateRange.Delete(1), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
        }

        [Test]
        public void DeleteRateRangeAsync_ShouldThrowException_WhenNotFound()
        {
            // Arrange
            _mockUnitOfWork.Setup(uow => uow.RateRange.GetSingleById(1)).Returns((RateRange)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _rateRangeService.DeleteRateRangeAsync(1));
            Assert.AreEqual("Rate range not found.", ex.Message);
        }
    }
}
