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
    public class QualificationLevelControllerTests
    {
        private Mock<IQualificationLevelService> _mockQualificationLevelService;
        private QualificationLevelController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockQualificationLevelService = new Mock<IQualificationLevelService>();
            _controller = new QualificationLevelController(_mockQualificationLevelService.Object);
        }

        [Test]
        public async Task GetAllQualificationLevels_ShouldReturnOk_WithQualificationLevels()
        {
            // Arrange
            var qualificationLevels = new List<QualificationLevelDto>
            {
                new QualificationLevelDto { Id = 1, Level = "Bachelor's Degree" },
                new QualificationLevelDto { Id = 2, Level = "Master's Degree" }
            };
            _mockQualificationLevelService.Setup(service => service.GetAllQualificationLevelsAsync())
                                          .ReturnsAsync(qualificationLevels);

            // Act
            var result = await _controller.GetAllQualificationLevels();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var response = okResult.Value as ApiResponse<IEnumerable<QualificationLevelDto>>;
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }

        [Test]
        public async Task GetAllQualificationLevels_ShouldReturnServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockQualificationLevelService.Setup(service => service.GetAllQualificationLevelsAsync())
                                          .ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _controller.GetAllQualificationLevels();

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            var response = objectResult.Value as ApiResponse<object>;
            Assert.IsNotNull(response);
            Assert.IsFalse(response.Success);
        }
    }
}
