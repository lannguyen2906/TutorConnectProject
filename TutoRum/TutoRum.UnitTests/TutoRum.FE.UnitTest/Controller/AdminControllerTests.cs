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

namespace TutoRum.UnitTests.TutoRum.FE.UnitTest.Controller
{
    [TestFixture]
    public class AdminControllerTests
    {
        private Mock<IAdminSevice> _mockAdminService;
        private AdminController _controller;

        [SetUp]
        public void Setup()
        {
            _mockAdminService = new Mock<IAdminSevice>();
            _controller = new AdminController(_mockAdminService.Object);
        }

        #region AssignRoleAdmin Tests

        [Test]
        public async Task AssignRoleAdmin_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var dto = new AssignRoleAdminDto();
            _mockAdminService.Setup(s => s.AssignRoleAdmin(dto, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AssignRoleAdmin(dto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public async Task AssignRoleAdmin_ThrowsKeyNotFoundException_Returns404()
        {
            // Arrange
            var dto = new AssignRoleAdminDto();
            _mockAdminService.Setup(s => s.AssignRoleAdmin(dto, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new KeyNotFoundException("Role not found"));

            // Act
            var result = await _controller.AssignRoleAdmin(dto);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task AssignRoleAdmin_ThrowsException_Returns500()
        {
            // Arrange
            var dto = new AssignRoleAdminDto();
            _mockAdminService.Setup(s => s.AssignRoleAdmin(dto, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new Exception("Internal error"));

            // Act
            var result = await _controller.AssignRoleAdmin(dto);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion

        #region GetAdminListByTutorHomePage Tests

        [Test]
        public async Task GetAdminListByTutorHomePage_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var filterDto = new FilterDto();
            var admin = new AdminHomePageDTO
            {
                // Populate the properties of AdminHomePageDTO as needed for the test
            };

            _mockAdminService.Setup(s => s.GetAllTutors(
                It.IsAny<System.Security.Claims.ClaimsPrincipal>(),
                filterDto,
                0,
                20))
                .ReturnsAsync(admin);

            // Act
            var result = await _controller.GetAdminListByTutorHomePage(filterDto, 0, 20);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as ApiResponse<AdminHomePageDTO>;
            Assert.NotNull(response);
        }

        [Test]
        public async Task GetAdminListByTutorHomePage_ThrowsKeyNotFoundException_Returns404()
        {
            // Arrange
            var filterDto = new FilterDto();
            _mockAdminService.Setup(s => s.GetAllTutors(It.IsAny<System.Security.Claims.ClaimsPrincipal>(), filterDto, 0, 20))
                .ThrowsAsync(new KeyNotFoundException("No tutors found"));

            // Act
            var result = await _controller.GetAdminListByTutorHomePage(filterDto, 0, 20);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task GetAdminListByTutorHomePage_ThrowsUnauthorizedAccessException_Returns403()
        {
            // Arrange
            var filterDto = new FilterDto();
            _mockAdminService.Setup(s => s.GetAllTutors(It.IsAny<System.Security.Claims.ClaimsPrincipal>(), filterDto, 0, 20))
                .ThrowsAsync(new UnauthorizedAccessException("Unauthorized"));

            // Act
            var result = await _controller.GetAdminListByTutorHomePage(filterDto, 0, 20);

            // Assert
            var forbidResult = result as ForbidResult;
            Assert.NotNull(forbidResult);
        }

        [Test]
        public async Task GetAdminListByTutorHomePage_ThrowsException_Returns500()
        {
            // Arrange
            var filterDto = new FilterDto();
            _mockAdminService.Setup(s => s.GetAllTutors(It.IsAny<System.Security.Claims.ClaimsPrincipal>(), filterDto, 0, 20))
                .ThrowsAsync(new Exception("Internal error"));

            // Act
            var result = await _controller.GetAdminListByTutorHomePage(filterDto, 0, 20);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion

        #region GetAdminMenuAction Tests

        [Test]
        public async Task GetAdminMenuAction_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var menuActions = new List<AdminMenuAction>();
            _mockAdminService.Setup(s => s.GetAdminMenuActionAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(menuActions);

            // Act
            var result = await _controller.GetAdminMenuAction();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public async Task GetAdminMenuAction_ThrowsKeyNotFoundException_Returns404()
        {
            // Arrange
            _mockAdminService.Setup(s => s.GetAdminMenuActionAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new KeyNotFoundException("Menu actions not found"));

            // Act
            var result = await _controller.GetAdminMenuAction();

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task GetAdminMenuAction_ThrowsUnauthorizedAccessException_Returns403()
        {
            // Arrange
            _mockAdminService.Setup(s => s.GetAdminMenuActionAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new UnauthorizedAccessException("Unauthorized"));

            // Act
            var result = await _controller.GetAdminMenuAction();

            // Assert
            var forbidResult = result as ForbidResult;
            Assert.NotNull(forbidResult);
        }

        [Test]
        public async Task GetAdminMenuAction_ThrowsException_Returns500()
        {
            // Arrange
            _mockAdminService.Setup(s => s.GetAdminMenuActionAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new Exception("Internal error"));

            // Act
            var result = await _controller.GetAdminMenuAction();

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion

        #region SetVerificationStatusAsync Tests

        [Test]
        public async Task SetVerificationStatusAsync_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var dto = new VerificationStatusDto();
            _mockAdminService.Setup(s => s.SetVerificationStatusAsync(dto.EntityType, dto.GuidId, dto.Id, dto.IsVerified, dto.Reason, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.SetVerificationStatusAsync(dto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public async Task SetVerificationStatusAsync_ThrowsKeyNotFoundException_Returns404()
        {
            // Arrange
            var dto = new VerificationStatusDto();
            _mockAdminService.Setup(s => s.SetVerificationStatusAsync(dto.EntityType, dto.GuidId, dto.Id, dto.IsVerified, dto.Reason, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new KeyNotFoundException("Entity not found"));

            // Act
            var result = await _controller.SetVerificationStatusAsync(dto);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task SetVerificationStatusAsync_ThrowsArgumentException_Returns400()
        {
            // Arrange
            var dto = new VerificationStatusDto();
            _mockAdminService.Setup(s => s.SetVerificationStatusAsync(dto.EntityType, dto.GuidId, dto.Id, dto.IsVerified, dto.Reason, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new ArgumentException("Invalid data"));

            // Act
            var result = await _controller.SetVerificationStatusAsync(dto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [Test]
        public async Task SetVerificationStatusAsync_ThrowsException_Returns500()
        {
            // Arrange
            var dto = new VerificationStatusDto();
            _mockAdminService.Setup(s => s.SetVerificationStatusAsync(dto.EntityType, dto.GuidId, dto.Id, dto.IsVerified, dto.Reason, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new Exception("Internal error"));

            // Act
            var result = await _controller.SetVerificationStatusAsync(dto);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion

        #region GetDashboardKeyMetrics Tests

        [Test]
        public async Task GetDashboardKeyMetrics_ShouldReturnOk_WhenDataIsRetrieved()
        {
            // Arrange
            var metricsDto = new DashboardKeyMetricsDto
            {
                NumberOfTutors = 1

            };

            _mockAdminService.Setup(service => service.GetDashboardKeyMetrics(It.IsAny<ClaimsPrincipal>()))
                             .ReturnsAsync(metricsDto);

            // Act
            var result = await _controller.GetDashboardKeyMetrics();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var response = okResult.Value as ApiResponse<DashboardKeyMetricsDto>;
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.AreEqual(metricsDto, response.Data);
        }

        [Test]
        public async Task GetDashboardKeyMetrics_ShouldReturnForbid_WhenUnauthorizedAccessExceptionIsThrown()
        {
            // Arrange
            _mockAdminService.Setup(service => service.GetDashboardKeyMetrics(It.IsAny<ClaimsPrincipal>()))
                             .ThrowsAsync(new UnauthorizedAccessException());

            // Act
            var result = await _controller.GetDashboardKeyMetrics();

            // Assert
            Assert.IsInstanceOf<ForbidResult>(result);
        }

        [Test]
        public async Task GetDashboardKeyMetrics_ShouldReturnServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _mockAdminService.Setup(service => service.GetDashboardKeyMetrics(It.IsAny<ClaimsPrincipal>()))
                             .ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _controller.GetDashboardKeyMetrics();

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);

            var response = objectResult.Value as ApiResponse<object>;
            Assert.IsNotNull(response);
            Assert.IsFalse(response.Success);
        }

        #endregion

        #region GetDashboardChartsData Tests

        [Test]
        public async Task GetDashboardChartsData_ShouldReturnOk_WhenDataIsRetrieved()
        {
            // Arrange
            var chartData = new DashboardChartData
            {
                MonthlyRevenues = new List<MonthlyRevenueData> { }
            };

            _mockAdminService.Setup(service => service.GetDashboardChartsData(It.IsAny<ClaimsPrincipal>()))
                             .ReturnsAsync(chartData);

            // Act
            var result = await _controller.GetDashboardChartsData();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var response = okResult.Value as ApiResponse<DashboardChartData>;
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.AreEqual(chartData, response.Data);
        }

        [Test]
        public async Task GetDashboardChartsData_ShouldReturnForbid_WhenUnauthorizedAccessExceptionIsThrown()
        {
            // Arrange
            _mockAdminService.Setup(service => service.GetDashboardChartsData(It.IsAny<ClaimsPrincipal>()))
                             .ThrowsAsync(new UnauthorizedAccessException());

            // Act
            var result = await _controller.GetDashboardChartsData();

            // Assert
            Assert.IsInstanceOf<ForbidResult>(result);
        }

        [Test]
        public async Task GetDashboardChartsData_ShouldReturnServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _mockAdminService.Setup(service => service.GetDashboardChartsData(It.IsAny<ClaimsPrincipal>()))
                             .ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _controller.GetDashboardChartsData();

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);

            var response = objectResult.Value as ApiResponse<object>;
            Assert.IsNotNull(response);
            Assert.IsFalse(response.Success);
        }

        #endregion
    }
}
