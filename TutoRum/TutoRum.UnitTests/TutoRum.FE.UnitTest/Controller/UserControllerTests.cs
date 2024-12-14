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
    public class UserControllerTests
    {
        private Mock<IUserService> _mockUserService;
        private Mock<IScheduleService> _mockScheduleService;
        private UserController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockUserService = new Mock<IUserService>();
            _mockScheduleService = new Mock<IScheduleService>();
            _controller = new UserController(_mockUserService.Object, _mockScheduleService.Object);
        }

        [Test]
        public async Task UpdateProfile_ReturnsOk_WhenProfileUpdatedSuccessfully()
        {
            // Arrange
            var userDto = new UpdateUserDTO
            {
                Fullname = "John Doe",
            };

            _mockUserService
                .Setup(s => s.UpdateUserProfileAsync(userDto, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateProfile(userDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var apiResponse = okResult.Value as ApiResponse<UpdateUserDTO>;
            Assert.NotNull(apiResponse);
            Assert.AreEqual(userDto, apiResponse.Data);
        }

        [Test]
        public async Task UpdateProfile_ReturnsUnauthorized_WhenAccessIsDenied()
        {
            // Arrange
            var userDto = new UpdateUserDTO();
            _mockUserService
                .Setup(s => s.UpdateUserProfileAsync(userDto, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new UnauthorizedAccessException("Access denied"));

            // Act
            var result = await _controller.UpdateProfile(userDto);

            // Assert
            var unauthorizedResult = result as ObjectResult;
            Assert.NotNull(unauthorizedResult);
            Assert.AreEqual(403, unauthorizedResult.StatusCode);

            var apiResponse = unauthorizedResult.Value as ApiResponse<object>;
            Assert.NotNull(apiResponse);
            Assert.AreEqual("Access denied", apiResponse.Message);
        }

        [Test]
        public async Task UpdateProfile_ReturnsNotFound_WhenUserNotFound()
        {
            // Arrange
            var userDto = new UpdateUserDTO();
            _mockUserService
                .Setup(s => s.UpdateUserProfileAsync(userDto, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new KeyNotFoundException("User not found"));

            // Act
            var result = await _controller.UpdateProfile(userDto);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);

            var apiResponse = notFoundResult.Value as ApiResponse<object>;
            Assert.NotNull(apiResponse);
            Assert.AreEqual("User not found", apiResponse.Message);
        }

    }

}
