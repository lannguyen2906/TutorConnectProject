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
using TutoRum.Services.Service;
using TutoRum.Services.ViewModels;

namespace TutoRum.UnitTests.TutoRum.FE.UnitTest.Controller
{
    [TestFixture]
    public class TutorRequestControllerTests
    {
        private Mock<ITutorRequestService> _mockTutorRequestService;
        private TutorRequestController _controller;
        private ClaimsPrincipal _user;

        [SetUp]
        public void Setup()
        {
            _mockTutorRequestService = new Mock<ITutorRequestService>();
            _controller = new TutorRequestController(_mockTutorRequestService.Object);

            // Setup a mock user (if needed for authorization)

            _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.Name, "testuser@example.com"),
            new Claim(ClaimTypes.NameIdentifier, "1")
            }, "mock"));

            _controller = new TutorRequestController(_mockTutorRequestService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = _user }
                }
            };
        }

        [Test]
        public async Task GetAllTutorRequests_ReturnsOkResult_WhenSuccessful()
        {
            // Arrange
            var filter = new TutorRequestHomepageFilterDto();
            var pageIndex = 0;
            var pageSize = 20;
            var tutorRequests = new PagedResult<TutorRequestDTO>
            {
                Items = new List<TutorRequestDTO> { new TutorRequestDTO() },
                TotalRecords = 1
            };

            _mockTutorRequestService.Setup(service => service.GetAllTutorRequestsAsync(filter, pageIndex, pageSize))
                .ReturnsAsync(tutorRequests);

            // Act
            var result = await _controller.GetAllTutorRequests(filter, pageIndex, pageSize);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public async Task GetTutorRequestById_ReturnsNotFound_WhenTutorRequestDoesNotExist()
        {
            // Arrange
            var tutorRequestId = 1;
            _mockTutorRequestService.Setup(service => service.GetTutorRequestByIdAsync(tutorRequestId))
                .ReturnsAsync((TutorRequestDTO)null);

            // Act
            var result = await _controller.GetTutorRequestById(tutorRequestId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task CreateTutorRequest_ReturnsCreatedAtAction_WhenSuccessful()
        {
            // Arrange
            var tutorRequestDto = new TutorRequestDTO { /* Set properties */ };
            var tutorRequestId = 1;
            _mockTutorRequestService.Setup(service => service.CreateTutorRequestAsync(tutorRequestDto, _user))
                .ReturnsAsync(tutorRequestId);

            // Act
            var result = await _controller.CreateTutorRequestAsync(tutorRequestDto);

            // Assert
            var createdAtActionResult = result as CreatedAtActionResult;
            Assert.NotNull(createdAtActionResult);
            Assert.AreEqual(201, createdAtActionResult.StatusCode);
        }

        [Test]
        public async Task UpdateTutorRequest_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var tutorRequestId = 1;
            var tutorRequestDto = tutorRequestDTO();
            _mockTutorRequestService.Setup(service => service.UpdateTutorRequestAsync(tutorRequestId, tutorRequestDto, _user))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateTutorRequestAsync(tutorRequestId, tutorRequestDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public async Task ChooseTutorForTutorRequestAsync_ReturnsNotFound_WhenTutorRequestDoesNotExist()
        {
            // Arrange
            var tutorRequestId = 1;
            var tutorId = Guid.NewGuid();
            _mockTutorRequestService.Setup(service => service.ChooseTutorForTutorRequestAsync(tutorRequestId, tutorId, _user))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.ChooseTutorForTutorRequestAsync(tutorRequestId, tutorId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task CloseTutorRequest_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var tutorRequestId = 1;
            _mockTutorRequestService.Setup(service => service.CloseTutorRequestAsync(tutorRequestId, _user))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.CloseTutorRequest(tutorRequestId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }


        [Test]
        public async Task AddTutorToRequestAsync_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            int tutorRequestId = 1;
            Guid tutorId = Guid.NewGuid();
            _mockTutorRequestService.Setup(service => service.AddTutorToRequestAsync(tutorRequestId, tutorId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.AddTutorToRequestAsync(tutorRequestId, tutorId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public async Task AddTutorToRequestAsync_ReturnsNotFound_WhenFailed()
        {
            // Arrange
            int tutorRequestId = 1;
            Guid tutorId = Guid.NewGuid();
            _mockTutorRequestService.Setup(service => service.AddTutorToRequestAsync(tutorRequestId, tutorId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.AddTutorToRequestAsync(tutorRequestId, tutorId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task GetListTutorsByTutorRequestAsync_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            int tutorRequestId = 1;
            var tutorRequestWithTutors = new TutorRequestWithTutorsDTO();
            _mockTutorRequestService.Setup(service => service.GetListTutorsByTutorRequestAsync(tutorRequestId))
                .ReturnsAsync(tutorRequestWithTutors);

            // Act
            var result = await _controller.GetListTutorsByTutorRequestAsync(tutorRequestId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public async Task GetTutorRequestsByLearnerId_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            Guid learnerId = Guid.NewGuid();
            var pagedResult = new PagedResult<ListTutorRequestDTO>();
            _mockTutorRequestService.Setup(service => service.GetTutorRequestsByLearnerIdAsync(learnerId, 0, 20))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _controller.GetTutorRequestsByLearnerId(learnerId, 0, 20);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task GetListTutorRequestsByTutorID_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            Guid tutorId = Guid.NewGuid();
            var pagedResult = new PagedResult<ListTutorRequestForTutorDto>();
            _mockTutorRequestService.Setup(service => service.GetTutorRequestsByTutorIdAsync(tutorId, 0, 20))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _controller.GetListTutorRequestsByTutorID(tutorId, 0, 20);

            // Assert
            Assert.IsNotNull(result);

        }

        [Test]
        public async Task GetTutorRequestsAdmin_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var filter = new TutorRequestFilterDto();
            var pagedResult = new PagedResult<ListTutorRequestDTO>();
            _mockTutorRequestService.Setup(service => service.GetTutorRequestsAdmin(filter, null, 0, 20))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _controller.GetTutorRequestsAdmin(filter, 0, 20);

            // Assert
            Assert.IsNotNull(result);

        }

        [Test]
        public async Task SendTutorRequestEmailAsync_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            int tutorRequestId = 1;
            Guid tutorId = Guid.NewGuid();
            _mockTutorRequestService.Setup(service => service.SendTutorRequestEmailAsync(tutorRequestId, tutorId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.SendTutorRequestEmailAsync(tutorRequestId, tutorId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public async Task SendTutorRequestEmailAsync_ReturnsBadRequest_OnException()
        {
            // Arrange
            int tutorRequestId = 1;
            Guid tutorId = Guid.NewGuid();
            _mockTutorRequestService.Setup(service => service.SendTutorRequestEmailAsync(tutorRequestId, tutorId))
                .ThrowsAsync(new Exception("Email sending failed."));

            // Act
            var result = await _controller.SendTutorRequestEmailAsync(tutorRequestId, tutorId);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }
        [Test]
        public async Task GetTutorLearnerSubjectInfoByTutorRequestId_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            int tutorRequestId = 1;
            var expectedResult = new TutorLearnerSubjectDetailDto { /* Populate with mock data */ };
            _mockTutorRequestService.Setup(service => service.GetTutorLearnerSubjectInfoByTutorRequestId(_user, tutorRequestId))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.GetTutorLearnerSubjectInfoByTutorRequestId(tutorRequestId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var response = okResult.Value as ApiResponse<TutorLearnerSubjectDetailDto>;
            Assert.NotNull(response);
            Assert.AreEqual(expectedResult, response.Data);
        }

        [Test]
        public async Task GetTutorLearnerSubjectInfoByTutorRequestId_ReturnsServerError_OnException()
        {
            // Arrange
            int tutorRequestId = 1;
            _mockTutorRequestService.Setup(service => service.GetTutorLearnerSubjectInfoByTutorRequestId(_user, tutorRequestId))
                .ThrowsAsync(new Exception("Error fetching tutor info."));

            // Act
            var result = await _controller.GetTutorLearnerSubjectInfoByTutorRequestId(tutorRequestId);

            // Assert
            var statusCodeResult = result.Result as ObjectResult;
            Assert.NotNull(statusCodeResult);
        }


        #region CloseTutorRequest Tests


        [Test]
        public async Task CloseTutorRequest_ReturnsNotFound_WhenRequestDoesNotExist()
        {
            // Arrange
            int tutorRequestId = 1;
            _mockTutorRequestService.Setup(service => service.CloseTutorRequestAsync(tutorRequestId, _user))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.CloseTutorRequest(tutorRequestId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            var response = notFoundResult.Value as ApiResponse<object>;
            Assert.NotNull(response);
            Assert.AreEqual("Tutor request not found.", response.Message);
        }

        [Test]
        public async Task CloseTutorRequest_ReturnsServerError_OnException()
        {
            // Arrange
            int tutorRequestId = 1;
            _mockTutorRequestService.Setup(service => service.CloseTutorRequestAsync(tutorRequestId, _user))
                .ThrowsAsync(new Exception("Error closing request."));

            // Act
            var result = await _controller.CloseTutorRequest(tutorRequestId);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            var response = statusCodeResult.Value as ApiResponse<object>;
            Assert.NotNull(response);
        }

        #endregion

        [Test]
        public async Task GetAllTutorRequests_ThrowsException_ReturnsServerError()
        {
            // Arrange
            var filter = new TutorRequestHomepageFilterDto();
            _mockTutorRequestService.Setup(service => service.GetAllTutorRequestsAsync(filter, 0, 20))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.GetAllTutorRequests(filter, 0, 20);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        [Test]
        public async Task GetTutorRequestById_ThrowsException_ReturnsServerError()
        {
            // Arrange
            int tutorRequestId = 1;
            _mockTutorRequestService.Setup(service => service.GetTutorRequestByIdAsync(tutorRequestId))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.GetTutorRequestById(tutorRequestId);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        [Test]
        public async Task CreateTutorRequestAsync_ThrowsException_ReturnsServerError()
        {
            // Arrange
            var tutorRequestDto = new TutorRequestDTO();
            _mockTutorRequestService.Setup(service => service.CreateTutorRequestAsync(tutorRequestDto, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.CreateTutorRequestAsync(tutorRequestDto);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        [Test]
        public async Task UpdateTutorRequestAsync_ThrowsException_ReturnsServerError()
        {
            // Arrange
            int tutorRequestId = 1;
            var tutorRequestDto = new TutorRequestDTO();
            _mockTutorRequestService.Setup(service => service.UpdateTutorRequestAsync(tutorRequestId, tutorRequestDto, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.UpdateTutorRequestAsync(tutorRequestId, tutorRequestDto);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }


        [Test]
        public async Task AddTutorToRequestAsync_ThrowsException_ReturnsServerError()
        {
            // Arrange
            int tutorRequestId = 1;
            Guid tutorId = Guid.NewGuid();
            _mockTutorRequestService.Setup(service => service.AddTutorToRequestAsync(tutorRequestId, tutorId))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.AddTutorToRequestAsync(tutorRequestId, tutorId);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            var response = statusCodeResult.Value as ApiResponse<object>;
        }

        [Test]
        public async Task GetListTutorsByTutorRequestAsync_ThrowsException_ReturnsServerError()
        {
            // Arrange
            int tutorRequestId = 1;
            _mockTutorRequestService.Setup(service => service.GetListTutorsByTutorRequestAsync(tutorRequestId))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.GetListTutorsByTutorRequestAsync(tutorRequestId);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            var response = statusCodeResult.Value as ApiResponse<object>;
        }

        [Test]
        public async Task GetTutorRequestsByLearnerId_ThrowsException_ReturnsServerError()
        {
            // Arrange
            Guid learnerId = Guid.NewGuid();
            _mockTutorRequestService.Setup(service => service.GetTutorRequestsByLearnerIdAsync(learnerId, 0, 20))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.GetTutorRequestsByLearnerId(learnerId, 0, 20);

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        public async Task GetListTutorRequestsByTutorID_ThrowsException_ReturnsServerError()
        {
            // Arrange
            Guid tutorId = Guid.NewGuid();
            _mockTutorRequestService.Setup(service => service.GetTutorRequestsByTutorIdAsync(tutorId, 0, 20))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.GetListTutorRequestsByTutorID(tutorId, 0, 20);

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        public async Task GetTutorRequestsAdmin_ThrowsException_ReturnsServerError()
        {
            // Arrange
            var filter = new TutorRequestFilterDto();
            _mockTutorRequestService.Setup(service => service.GetTutorRequestsAdmin(filter, It.IsAny<System.Security.Claims.ClaimsPrincipal>(), 0, 20))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.GetTutorRequestsAdmin(filter, 0, 20);

            // Assert
            Assert.NotNull(result);

        }
        private TutorRequestDTO tutorRequestDTO() 
        {
            return  new TutorRequestDTO
            {
                Id = 1,
                PhoneNumber = "123-456-7890",
                RequestSummary = "Looking for a Math tutor for high school.",
                CityId = "001",
                DistrictId = "002",
                WardId = "003",
                TeachingLocation = "123 Main Street, City Center",
                NumberOfStudents = 1,
                StartDate = new DateTime(2024, 1, 15),
                PreferredScheduleType = "Evening",
                TimePerSession = TimeSpan.FromHours(2),
                Subject = "Mathematics",
                StudentGender = "Male",
                TutorGender = "Female",
                Fee = 500000, // Assume this is in VND or another currency
                LearnerName = "John Doe",
                SessionsPerWeek = 3,
                DetailedDescription = "Student is in grade 10 and requires help with Algebra and Geometry.",
                TutorQualificationId = 5,
                TutorQualificationName = "Certified Math Teacher",
                Status = "Pending",
                FreeSchedules = "Monday, Wednesday, Friday",
                rateRangeId = 2,
                CreatedUserId = Guid.NewGuid(),
                RegisteredTutorIds = new List<Guid>
                        {
                            Guid.NewGuid(),
                            Guid.NewGuid()
                        }
            };
            
        }
    }
}
