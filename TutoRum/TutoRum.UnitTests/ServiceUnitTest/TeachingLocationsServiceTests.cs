using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.Models;
using TutoRum.Services.Service;
using TutoRum.Services.ViewModels;

namespace TutoRum.UnitTests.ServiceUnitTest
{
    [TestFixture]
    public class TeachingLocationsServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<UserManager<AspNetUser>> _userManagerMock;
        private TeachingLocationsService _teachingLocationsService;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _userManagerMock = new Mock<UserManager<AspNetUser>>(
                Mock.Of<IUserStore<AspNetUser>>(), null, null, null, null, null, null, null, null);

            _teachingLocationsService = new TeachingLocationsService(
                _unitOfWorkMock.Object,
                _userManagerMock.Object
            );
        }

        // Test for DeleteTeachingLocationAsync method
        [Test]
        public async Task DeleteTeachingLocationAsync_TeachingLocationNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var locationIds = new[] { 1 };

            _unitOfWorkMock.Setup(u => u.tutorTeachingLocations.GetSingleById(It.IsAny<int>()))
                .Returns((TutorTeachingLocations)null); // Simulate location not found

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _teachingLocationsService.DeleteTeachingLocationAsync(tutorId, locationIds));
            Assert.AreEqual("Teaching location not found.", ex.Message);
        }

        [Test]
        public async Task DeleteTeachingLocationAsync_UnauthorizedAccess_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var locationIds = new[] { 1 };
            var location = new TutorTeachingLocations
            {
                TutorId = Guid.NewGuid() // Different tutorId for unauthorized access
            };

            _unitOfWorkMock.Setup(u => u.tutorTeachingLocations.GetSingleById(It.IsAny<int>()))
                .Returns(location); // Simulate unauthorized access

            // Act & Assert
            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _teachingLocationsService.DeleteTeachingLocationAsync(tutorId, locationIds));
            Assert.AreEqual("You do not have permission to delete this teaching location.", ex.Message);
        }

        [Test]
        public async Task DeleteTeachingLocationAsync_ValidRequest_DeletesLocation()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var locationIds = new[] { 1 };
            var location = new TutorTeachingLocations
            {
                TutorId = tutorId,
                TeachingLocationId = 1
            };

            _unitOfWorkMock.Setup(u => u.tutorTeachingLocations.GetSingleById(It.IsAny<int>()))
                .Returns(location); // Simulate location found and belonging to the tutor

            // Act
            await _teachingLocationsService.DeleteTeachingLocationAsync(tutorId, locationIds);

            // Assert
            _unitOfWorkMock.Verify(u => u.tutorTeachingLocations.Delete(It.IsAny<int>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        // Test for AddTeachingLocationsAsync method
        [Test]
        public async Task AddTeachingLocationsAsync_ValidRequest_AddsTeachingLocations()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var locationDto = new AddTeachingLocationViewDTO
            {
                CityId = "1",
                Districts = new List<AddDistrictDTO>
            {
                new AddDistrictDTO { DistrictId = "1" }
            }
            };

            var teachingLocations = new List<AddTeachingLocationViewDTO> { locationDto };

            var location = new TeachingLocation
            {
                TeachingLocationId = 1,
                CityId = "1",
                DistrictId = "1",
                CreatedDate = DateTime.UtcNow
            };

            _unitOfWorkMock.Setup(u => u.teachingLocation.FindAsync(It.IsAny<string?>(), It.IsAny<string?>()))
                .ReturnsAsync(location); // Simulate location found

            _unitOfWorkMock.Setup(u => u.tutorTeachingLocations.ExistsAsync(It.IsAny<Guid>(), It.IsAny<int>()))
                .ReturnsAsync(false); // Simulate tutor does not already have this location

            // Act
            var result =  _teachingLocationsService.AddTeachingLocationsAsync(teachingLocations, tutorId);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task AddTeachingLocationsAsync_AlreadyExists_DoesNotAddLocation()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var locationDto = new AddTeachingLocationViewDTO
            {
                CityId = "1",
                Districts = new List<AddDistrictDTO>
            {
                new AddDistrictDTO { DistrictId = "1" }
            }
            };

            var teachingLocations = new List<AddTeachingLocationViewDTO> { locationDto };

            var location = new TeachingLocation
            {
                TeachingLocationId = 1,
                CityId = "1",
                DistrictId = "1",
                CreatedDate = DateTime.UtcNow
            };

            _unitOfWorkMock.Setup(u => u.teachingLocation.FindAsync(It.IsAny<string?>(), It.IsAny<string?>()))
                .ReturnsAsync(location); // Simulate location found

            _unitOfWorkMock.Setup(u => u.tutorTeachingLocations.ExistsAsync(It.IsAny<Guid>(), It.IsAny<int>()))
                .ReturnsAsync(true); // Simulate tutor already has this location

            // Act
            await _teachingLocationsService.AddTeachingLocationsAsync(teachingLocations, tutorId);

            // Assert
            _unitOfWorkMock.Verify(u => u.teachingLocation.Add(It.IsAny<TeachingLocation>()), Times.Never); // Location should not be added
            _unitOfWorkMock.Verify(u => u.tutorTeachingLocations.Add(It.IsAny<TutorTeachingLocations>()), Times.Never); // TutorTeachingLocations should not be added
        }
    }
}