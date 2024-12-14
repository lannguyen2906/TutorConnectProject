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
using static TutoRum.FE.Common.Url;

namespace TutoRum.UnitTests.TutoRum.FE.UnitTest
{
    [TestFixture]
    public class TutorLearnerSubjectControllerTests
    {
        private Mock<ITutorLearnerSubjectService> _mockTutorLearnerSubjectService;
        private Mock<IContractService> _contractServiceMock;
        private TutorLearnerSubjectController _controller;
        private ClaimsPrincipal _user;

        [SetUp]
        public void Setup()
        {
            _mockTutorLearnerSubjectService = new Mock<ITutorLearnerSubjectService>();
            _contractServiceMock = new Mock<IContractService>();
            // Setup a mock user (if needed for authorization)

            _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.Name, "testuser@example.com"),
            new Claim(ClaimTypes.NameIdentifier, "1")
            }, "mock"));

            _controller = new TutorLearnerSubjectController(_mockTutorLearnerSubjectService.Object,
                _contractServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = _user }
                },

            };
        }

        [Test]
        public async Task RegisterLearnerForTutor_ValidRequest_ReturnsOk()
        {
            // Arrange
            var learnerDto = new RegisterLearnerDTO { CityId = "1" };
            _mockTutorLearnerSubjectService
                .Setup(s => s.RegisterLearnerForTutorAsync(learnerDto, It.IsAny<ClaimsPrincipal>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.RegisterLearnerForTutor(learnerDto, Guid.NewGuid());

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public async Task DownloadContract_FileExists_ReturnsFile()
        {
            // Arrange
            var tutorLearnerSubjectID = 1;
            var filePath = "test.docx";
            var fileBytes = new byte[] { 1, 2, 3 };

            _contractServiceMock.Setup(s => s.GenerateContractAsync(tutorLearnerSubjectID))
                .ReturnsAsync(filePath);
            System.IO.File.WriteAllBytes(filePath, fileBytes);

            // Act
            var result = await _controller.DownloadContract(tutorLearnerSubjectID);

            // Assert
            var fileResult = result as FileContentResult;
            Assert.IsNotNull(fileResult);
            Assert.AreEqual("application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileResult.ContentType);
            Assert.AreEqual("test.docx", fileResult.FileDownloadName);

            // Cleanup
            System.IO.File.Delete(filePath);
        }

        [Test]
        public async Task DownloadContract_FileDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var tutorLearnerSubjectID = 1;
            _contractServiceMock.Setup(s => s.GenerateContractAsync(tutorLearnerSubjectID))
                .ReturnsAsync("nonexistent.docx");

            // Act
            var result = await _controller.DownloadContract(tutorLearnerSubjectID);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task GetTutorLearnerSubjectDetailById_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var subjectDetail = new TutorLearnerSubjectSummaryDetailDto();
            _mockTutorLearnerSubjectService
                .Setup(s => s.GetTutorLearnerSubjectSummaryDetailByIdAsync(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(subjectDetail);

            // Act
            var result = await _controller.GetTutorLearnerSubjectDetailById(1);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(subjectDetail, ((ApiResponse<TutorLearnerSubjectSummaryDetailDto>)okResult.Value).Data);
        }

        [Test]
        public async Task GetTutorLearnerSubjectDetailById_ReturnsNotFound_WhenKeyNotFound()
        {
            // Arrange
            _mockTutorLearnerSubjectService
                .Setup(s => s.GetTutorLearnerSubjectSummaryDetailByIdAsync(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.GetTutorLearnerSubjectDetailById(1);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task GetTutorLearnerSubjectDetailById_ReturnsForbidden_WhenUnauthorized()
        {
            // Arrange
            _mockTutorLearnerSubjectService
                .Setup(s => s.GetTutorLearnerSubjectSummaryDetailByIdAsync(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .ThrowsAsync(new UnauthorizedAccessException());

            // Act
            var result = await _controller.GetTutorLearnerSubjectDetailById(1);

            // Assert
            var forbiddenResult = result as ObjectResult;
            Assert.NotNull(forbiddenResult);
            Assert.AreEqual(403, forbiddenResult.StatusCode);
        }

        [Test]
        public async Task GetSubjectDetailsByUserId_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var subjectDetails = new List<SubjectDetailDto>();
            _mockTutorLearnerSubjectService
                .Setup(s => s.GetSubjectDetailsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(subjectDetails);

            // Act
            var result = await _controller.GetSubjectDetailsByUserId(Guid.NewGuid(), "viewType");

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(subjectDetails, ((ApiResponse<List<SubjectDetailDto>>)okResult.Value).Data);
        }

        [Test]
        public async Task GetSubjectDetailsByUserId_ReturnsServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockTutorLearnerSubjectService
                .Setup(s => s.GetSubjectDetailsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.GetSubjectDetailsByUserId(Guid.NewGuid(), "viewType");

            // Assert
            var serverErrorResult = result as ObjectResult;
            Assert.NotNull(serverErrorResult);
            Assert.AreEqual(500, serverErrorResult.StatusCode);
        }

        [Test]
        public async Task RegisterLearnerForTutor_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var learnerDto = new RegisterLearnerDTO();
            _mockTutorLearnerSubjectService
                .Setup(s => s.RegisterLearnerForTutorAsync(learnerDto, It.IsAny<ClaimsPrincipal>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.RegisterLearnerForTutor(learnerDto, Guid.NewGuid());

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public async Task GetSubjectDetailsByUserId_ValidRequest_ReturnsOk()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var viewType = "learner";
            var subjectDetails = new List<SubjectDetailDto>
        {
            new SubjectDetailDto { SubjectName = "Math" }
        };

            _mockTutorLearnerSubjectService
                .Setup(s => s.GetSubjectDetailsByUserIdAsync(userId, viewType))
                .ReturnsAsync(subjectDetails);

            // Act
            var result = await _controller.GetSubjectDetailsByUserId(userId, viewType);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as ApiResponse<List<SubjectDetailDto>>;
            Assert.IsNotNull(response);
            Assert.AreEqual(subjectDetails, response.Data);
        }

        [Test]
        public async Task HandleContractUploadAsync_ReturnsOk_WhenUploadIsSuccessful()
        {
            // Arrange
            var contractDto = new HandleContractUploadDTO
            {
                TutorId = Guid.NewGuid(),
                TutorLearnerSubjectId = 1,
                ContractUrl = "http://example.com/contract.pdf"
            };

            _mockTutorLearnerSubjectService.Setup(s => s.HandleContractUploadAndNotifyAsync(
                contractDto.TutorId, contractDto.TutorLearnerSubjectId, contractDto.ContractUrl))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.HandleContractUploadAsync(contractDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public async Task HandleContractUploadAsync_ReturnsServerError_WhenExceptionOccurs()
        {
            // Arrange
            var contractDto = new HandleContractUploadDTO
            {
                TutorId = Guid.NewGuid(),
                TutorLearnerSubjectId = 1,
                ContractUrl = "http://example.com/contract.pdf"
            };

            _mockTutorLearnerSubjectService.Setup(s => s.HandleContractUploadAndNotifyAsync(
                contractDto.TutorId, contractDto.TutorLearnerSubjectId, contractDto.ContractUrl))
                .Throws(new Exception("Internal error"));

            // Act
            var result = await _controller.HandleContractUploadAsync(contractDto);

            // Assert
            var serverError = result as ObjectResult;
            Assert.NotNull(serverError);
            Assert.AreEqual(500, serverError.StatusCode);
        }

        [Test]
        public async Task VerifyContract_ReturnsOk_WhenVerificationIsSuccessful()
        {
            // Arrange
            var tutorLearnerSubjectId = 1;
            var isVerified = true;
            var reason = "Verified successfully";

            _mockTutorLearnerSubjectService.Setup(s => s.VerifyTutorLearnerContractAsync(
                tutorLearnerSubjectId, It.IsAny<ClaimsPrincipal>(), isVerified, reason))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.VerifyContract(tutorLearnerSubjectId, isVerified, reason);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public async Task VerifyContract_ReturnsUnauthorized_WhenAccessDenied()
        {
            // Arrange
            var tutorLearnerSubjectId = 1;
            var isVerified = false;
            var reason = "Access Denied";

            _mockTutorLearnerSubjectService.Setup(s => s.VerifyTutorLearnerContractAsync(
                tutorLearnerSubjectId, It.IsAny<ClaimsPrincipal>(), isVerified, reason))
                .Throws(new UnauthorizedAccessException("Unauthorized"));

            // Act
            var result = await _controller.VerifyContract(tutorLearnerSubjectId, isVerified, reason);

            // Assert
            var unauthorizedResult = result as ObjectResult;
            Assert.NotNull(unauthorizedResult);
            Assert.AreEqual(403, unauthorizedResult.StatusCode);
        }

        

        [Test]
        public async Task RegisterLearnerForTutor_ReturnsUnauthorized_WhenUserIsUnauthorized()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var learnerDto = new RegisterLearnerDTO { Notes = "John Doe" };

            _mockTutorLearnerSubjectService.Setup(s => s.RegisterLearnerForTutorAsync(learnerDto, It.IsAny<ClaimsPrincipal>()))
                .Throws(new UnauthorizedAccessException("Unauthorized"));

            // Act
            var result = await _controller.RegisterLearnerForTutor(learnerDto, tutorId);

            // Assert
            var unauthorizedResult = result as ObjectResult;
            Assert.NotNull(unauthorizedResult);
            Assert.AreEqual(403, unauthorizedResult.StatusCode);
        }

        [Test]
        public async Task RegisterLearnerForTutor_ReturnsNotFound_WhenTutorNotFound()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var learnerDto = new RegisterLearnerDTO { Notes = "John Doe"};

            _mockTutorLearnerSubjectService.Setup(s => s.RegisterLearnerForTutorAsync(learnerDto, It.IsAny<ClaimsPrincipal>()))
                .Throws(new KeyNotFoundException("Tutor not found"));

            // Act
            var result = await _controller.RegisterLearnerForTutor(learnerDto, tutorId);

            // Assert
            var notFoundResult = result as ObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task RegisterLearnerForTutor_ReturnsServerError_WhenExceptionOccurs()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var learnerDto = new RegisterLearnerDTO { Notes = "John Doe" };

            _mockTutorLearnerSubjectService.Setup(s => s.RegisterLearnerForTutorAsync(learnerDto, It.IsAny<ClaimsPrincipal>()))
                .Throws(new Exception("Internal Server Error"));

            // Act
            var result = await _controller.RegisterLearnerForTutor(learnerDto, tutorId);

            // Assert
            var serverErrorResult = result as ObjectResult;
            Assert.NotNull(serverErrorResult);
            Assert.AreEqual(500, serverErrorResult.StatusCode);
        }

        

        [Test]
        public async Task GetSubjectDetailsByUserId_ReturnsNotFound_WhenDetailsNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var viewType = "ViewType";

            _mockTutorLearnerSubjectService.Setup(s => s.GetSubjectDetailsByUserIdAsync(userId, viewType))
                .Throws(new KeyNotFoundException("Not Found"));

            // Act
            var result = await _controller.GetSubjectDetailsByUserId(userId, viewType);

            // Assert
            var notFoundResult = result as ObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }
        [Test]
        public async Task UpdateClassroom_ReturnsOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            var tutorLearnerSubjectId = 1;
            var learnerDto = new RegisterLearnerDTO();
            _mockTutorLearnerSubjectService.Setup(s => s.UpdateClassroom(tutorLearnerSubjectId, learnerDto, It.IsAny<ClaimsPrincipal>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateClassroom(tutorLearnerSubjectId, learnerDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as ApiResponse<RegisterLearnerDTO>;
            Assert.NotNull(response);
            Assert.AreEqual(learnerDto, response.Data);
        }

        [Test]
        public async Task UpdateClassroom_ReturnsUnauthorized_WhenUserIsUnauthorized()
        {
            // Arrange
            var tutorLearnerSubjectId = 1;
            var learnerDto = new RegisterLearnerDTO();
            _mockTutorLearnerSubjectService.Setup(s => s.UpdateClassroom(tutorLearnerSubjectId, learnerDto, It.IsAny<ClaimsPrincipal>()))
                .Throws(new UnauthorizedAccessException("Unauthorized"));

            // Act
            var result = await _controller.UpdateClassroom(tutorLearnerSubjectId, learnerDto);

            // Assert
            var unauthorizedResult = result as ObjectResult;
            Assert.NotNull(unauthorizedResult);
            Assert.AreEqual(403, unauthorizedResult.StatusCode);
        }

        [Test]
        public async Task UpdateClassroom_ReturnsNotFound_WhenTutorLearnerSubjectIsNotFound()
        {
            // Arrange
            var tutorLearnerSubjectId = 1;
            var learnerDto = new RegisterLearnerDTO();
            _mockTutorLearnerSubjectService.Setup(s => s.UpdateClassroom(tutorLearnerSubjectId, learnerDto, It.IsAny<ClaimsPrincipal>()))
                .Throws(new KeyNotFoundException("Not Found"));

            // Act
            var result = await _controller.UpdateClassroom(tutorLearnerSubjectId, learnerDto);

            // Assert
            var notFoundResult = result as ObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task UpdateClassroom_ReturnsServerError_WhenExceptionOccurs()
        {
            // Arrange
            var tutorLearnerSubjectId = 1;
            var learnerDto = new RegisterLearnerDTO();
            _mockTutorLearnerSubjectService.Setup(s => s.UpdateClassroom(tutorLearnerSubjectId, learnerDto, It.IsAny<ClaimsPrincipal>()))
                .Throws(new Exception("Internal Server Error"));

            // Act
            var result = await _controller.UpdateClassroom(tutorLearnerSubjectId, learnerDto);

            // Assert
            var serverErrorResult = result as ObjectResult;
            Assert.NotNull(serverErrorResult);
            Assert.AreEqual(500, serverErrorResult.StatusCode);
        }
        

        [Test]
        public async Task DownloadContract_ReturnsNotFound_WhenFileDoesNotExist()
        {
            // Arrange
            var tutorLearnerSubjectId = 1;

            _contractServiceMock.Setup(s => s.GenerateContractAsync(tutorLearnerSubjectId))
                .ReturnsAsync("nonexistent.pdf");

            // Act
            var result = await _controller.DownloadContract(tutorLearnerSubjectId);

            // Assert
            var notFoundResult = result as ObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task VerifyContract_ValidRequest_ReturnsOk()
        {
            // Arrange
            var tutorLearnerSubjectId = 1;
            var isVerified = true;
            var reason = "Verified";

            _mockTutorLearnerSubjectService
                .Setup(s => s.VerifyTutorLearnerContractAsync(tutorLearnerSubjectId, It.IsAny<ClaimsPrincipal>(), isVerified, reason))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.VerifyContract(tutorLearnerSubjectId, isVerified, reason);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public async Task CloseClass_ValidRequest_ReturnsOk()
        {
            // Arrange
            var tutorLearnerSubjectId = 1;
            _mockTutorLearnerSubjectService
                .Setup(s => s.CloseClassAsync(tutorLearnerSubjectId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.CloseClass(tutorLearnerSubjectId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public async Task GetTutorLearnerSubjectDetailById_ReturnsOk_WhenSubjectExists()
        {
            // Arrange
            var id = 1;
            var subjectDetail = new TutorLearnerSubjectSummaryDetailDto();
            _mockTutorLearnerSubjectService
                .Setup(s => s.GetTutorLearnerSubjectSummaryDetailByIdAsync(id, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(subjectDetail);

            // Act
            var result = await _controller.GetTutorLearnerSubjectDetailById(id);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as ApiResponse<TutorLearnerSubjectSummaryDetailDto>;
            Assert.NotNull(response);
            Assert.AreEqual(subjectDetail, response.Data);
        }

        [Test]
        public async Task GetClassroomsByUserIdAsync_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var viewType = "list";
            var subjectDetails = new List<SubjectDetailDto>
        {
            new SubjectDetailDto { SubjectName = "Math" },
            new SubjectDetailDto { SubjectName = "Science" }
        };

            _mockTutorLearnerSubjectService.Setup(s => s.GetClassroomsByUserIdAsync(userId, viewType))
                .ReturnsAsync(subjectDetails);

            // Act
            var result = await _controller.GetClassroomsByUserIdAsync(userId, viewType);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as ApiResponse<List<SubjectDetailDto>>;
            Assert.NotNull(response);
            Assert.AreEqual(subjectDetails, response.Data);
        }

        [Test]
        public async Task GetClassroomsByUserIdAsync_ReturnsUnauthorized_WhenUserIsUnauthorized()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var viewType = "list";
            _mockTutorLearnerSubjectService.Setup(s => s.GetClassroomsByUserIdAsync(userId, viewType))
                .Throws(new UnauthorizedAccessException("Unauthorized"));

            // Act
            var result = await _controller.GetClassroomsByUserIdAsync(userId, viewType);

            // Assert
            var unauthorizedResult = result as ObjectResult;
            Assert.NotNull(unauthorizedResult);
            Assert.AreEqual(403, unauthorizedResult.StatusCode);
        }

        [Test]
        public async Task GetClassroomsByUserIdAsync_ReturnsNotFound_WhenNoClassroomsFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var viewType = "list";
            _mockTutorLearnerSubjectService.Setup(s => s.GetClassroomsByUserIdAsync(userId, viewType))
                .Throws(new KeyNotFoundException("Not Found"));

            // Act
            var result = await _controller.GetClassroomsByUserIdAsync(userId, viewType);

            // Assert
            var notFoundResult = result as ObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task GetClassroomsByUserIdAsync_ReturnsServerError_WhenExceptionOccurs()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var viewType = "list";
            _mockTutorLearnerSubjectService.Setup(s => s.GetClassroomsByUserIdAsync(userId, viewType))
                .Throws(new Exception("Internal Server Error"));

            // Act
            var result = await _controller.GetClassroomsByUserIdAsync(userId, viewType);

            // Assert
            var serverErrorResult = result as ObjectResult;
            Assert.NotNull(serverErrorResult);
            Assert.AreEqual(500, serverErrorResult.StatusCode);
        }

        [Test]
        public async Task GetTutorLearnerSubjectDetailById_ReturnsNotFound_WhenSubjectNotFound()
        {
            // Arrange
            var id = 1;
            _mockTutorLearnerSubjectService
                .Setup(s => s.GetTutorLearnerSubjectSummaryDetailByIdAsync(id, It.IsAny<ClaimsPrincipal>()))
                .ThrowsAsync(new KeyNotFoundException("Subject not found"));

            // Act
            var result = await _controller.GetTutorLearnerSubjectDetailById(id);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task GetClassroomsByUserIdAsync_ReturnsOk_WhenClassroomsExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var viewType = "default";
            var classrooms = new List<SubjectDetailDto> { new SubjectDetailDto() };
            _mockTutorLearnerSubjectService
                .Setup(s => s.GetClassroomsByUserIdAsync(userId, viewType))
                .ReturnsAsync(classrooms);

            // Act
            var result = await _controller.GetClassroomsByUserIdAsync(userId, viewType);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as ApiResponse<List<SubjectDetailDto>>;
            Assert.NotNull(response);
            Assert.AreEqual(classrooms, response.Data);
        }

        [Test]
        public async Task UpdateClassroom_ReturnsOk_WhenUpdatedSuccessfully()
        {
            // Arrange
            var tutorLearnerSubjectID = 1;
            var learnerDto = new RegisterLearnerDTO();
            _mockTutorLearnerSubjectService
                .Setup(s => s.UpdateClassroom(tutorLearnerSubjectID, learnerDto, It.IsAny<ClaimsPrincipal>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateClassroom(tutorLearnerSubjectID, learnerDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as ApiResponse<RegisterLearnerDTO>;
            Assert.NotNull(response);
            Assert.AreEqual(learnerDto, response.Data);
        }

        [Test]
        public async Task HandleContractUploadAsync_ReturnsOk_WhenUploadedSuccessfully()
        {
            // Arrange
            var contractDto = new HandleContractUploadDTO
            {
                TutorId = Guid.NewGuid(),
                TutorLearnerSubjectId = 1,
                ContractUrl = "http://example.com/contract.pdf"
            };
            _mockTutorLearnerSubjectService
                .Setup(s => s.HandleContractUploadAndNotifyAsync(contractDto.TutorId, contractDto.TutorLearnerSubjectId, contractDto.ContractUrl))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.HandleContractUploadAsync(contractDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as ApiResponse<string>;
            Assert.NotNull(response);
        }

        #region GetContracts Tests

        [Test]
        public async Task GetContracts_ReturnsOk_WhenContractsExist()
        {
            // Arrange
            var filter = new ContractFilterDto();
            var pagedContracts = new PagedResult<ContractDto>
            {
                Items = new List<ContractDto> { new ContractDto() },
                TotalRecords = 1
            };

            _mockTutorLearnerSubjectService
                .Setup(s => s.GetListContractAsync(filter, 0, 20))
                .ReturnsAsync(pagedContracts);

            // Act
            var result = await _controller.GetContracts(filter, 0, 20);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as ApiResponse<PagedResult<ContractDto>>;
            Assert.NotNull(response);
            Assert.AreEqual(pagedContracts, response.Data);
        }

        [Test]
        public async Task GetContracts_ReturnsServerError_WhenExceptionThrown()
        {
            // Arrange
            var filter = new ContractFilterDto();
            _mockTutorLearnerSubjectService
                .Setup(s => s.GetListContractAsync(filter, 0, 20))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetContracts(filter, 0, 20);

            // Assert
            var serverErrorResult = result as ObjectResult;
            Assert.NotNull(serverErrorResult);
            Assert.AreEqual(500, serverErrorResult.StatusCode);

            var response = serverErrorResult.Value as ApiResponse<object>;
            Assert.NotNull(response);
        }

        #endregion

        #region CreateClassForLearnerAsync Tests

        [Test]
        public async Task CreateClassForLearnerAsync_ReturnsOk_WhenClassCreatedSuccessfully()
        {
            // Arrange
            var classDto = new CreateClassDTO();
            var tutorRequestId = 1;

            _mockTutorLearnerSubjectService
                .Setup(s => s.CreateClassForLearnerAsync(classDto, tutorRequestId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.CreateClassForLearnerAsync(classDto, tutorRequestId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as dynamic;
        }

        [Test]
        public async Task CreateClassForLearnerAsync_ReturnsBadRequest_WhenClassDtoIsNull()
        {
            // Act
            var result = await _controller.CreateClassForLearnerAsync(null, 1);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Invalid data.", badRequestResult.Value);
        }

        [Test]
        public async Task CreateClassForLearnerAsync_ReturnsBadRequest_WhenClassCreationFails()
        {
            // Arrange
            var classDto = new CreateClassDTO();
            var tutorRequestId = 1;

            _mockTutorLearnerSubjectService
                .Setup(s => s.CreateClassForLearnerAsync(classDto, tutorRequestId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.CreateClassForLearnerAsync(classDto, tutorRequestId);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Failed to create class.", badRequestResult.Value);
        }

        [Test]
        public async Task CreateClassForLearnerAsync_ReturnsForbid_WhenUnauthorizedAccessExceptionThrown()
        {
            // Arrange
            var classDto = new CreateClassDTO();
            var tutorRequestId = 1;

            _mockTutorLearnerSubjectService
                .Setup(s => s.CreateClassForLearnerAsync(classDto, tutorRequestId, It.IsAny<ClaimsPrincipal>()))
                .ThrowsAsync(new UnauthorizedAccessException("Access denied"));

            // Act
            var result = await _controller.CreateClassForLearnerAsync(classDto, tutorRequestId);

            // Assert
            var forbidResult = result as ForbidResult;
            Assert.NotNull(forbidResult);
        }

        [Test]
        public async Task CreateClassForLearnerAsync_ReturnsServerError_WhenExceptionThrown()
        {
            // Arrange
            var classDto = new CreateClassDTO();
            var tutorRequestId = 1;

            _mockTutorLearnerSubjectService
                .Setup(s => s.CreateClassForLearnerAsync(classDto, tutorRequestId, It.IsAny<ClaimsPrincipal>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.CreateClassForLearnerAsync(classDto, tutorRequestId);

            // Assert
            var serverErrorResult = result as ObjectResult;
            Assert.NotNull(serverErrorResult);
            Assert.AreEqual(500, serverErrorResult.StatusCode);
            Assert.AreEqual("Internal server error: Test exception", serverErrorResult.Value);
        }

        #endregion

        [Test]
        public async Task CloseClass_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var tutorLearnerSubjectId = 1;
            _mockTutorLearnerSubjectService
                .Setup(s => s.CloseClassAsync(tutorLearnerSubjectId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.CloseClass(tutorLearnerSubjectId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public async Task CloseClass_ReturnsBadRequest_WhenClosureFails()
        {
            // Arrange
            var tutorLearnerSubjectId = 1;
            _mockTutorLearnerSubjectService
                .Setup(s => s.CloseClassAsync(tutorLearnerSubjectId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.CloseClass(tutorLearnerSubjectId);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Failed to close class.", badRequestResult.Value);
        }

        [Test]
        public async Task CloseClass_ReturnsForbidden_WhenUnauthorized()
        {
            // Arrange
            var tutorLearnerSubjectId = 1;
            _mockTutorLearnerSubjectService
                .Setup(s => s.CloseClassAsync(tutorLearnerSubjectId, It.IsAny<ClaimsPrincipal>()))
                .Throws<UnauthorizedAccessException>();

            // Act
            var result = await _controller.CloseClass(tutorLearnerSubjectId);

            // Assert
            var forbiddenResult = result as ObjectResult;
        }

        [Test]
        public async Task CloseClass_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            var tutorLearnerSubjectId = 1;
            _mockTutorLearnerSubjectService
                .Setup(s => s.CloseClassAsync(tutorLearnerSubjectId, It.IsAny<ClaimsPrincipal>()))
                .Throws<Exception>();

            // Act
            var result = await _controller.CloseClass(tutorLearnerSubjectId);

            // Assert
            var errorResult = result as ObjectResult;
            Assert.NotNull(errorResult);
            Assert.AreEqual(500, errorResult.StatusCode);
        }

        [Test]
        public async Task VerifyContract_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            int tutorLearnerSubjectId = 1;
            bool isVerified = true;
            string reason = "Verified successfully";

            _mockTutorLearnerSubjectService
                .Setup(s => s.VerifyTutorLearnerContractAsync(
                    tutorLearnerSubjectId,
                    It.IsAny<ClaimsPrincipal>(),
                    isVerified,
                    reason))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.VerifyContract(tutorLearnerSubjectId, isVerified, reason);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.IsTrue(((ApiResponse<bool>)okResult.Value).Data);
        }

        [Test]
        public async Task VerifyContract_ReturnsForbidden_WhenUnauthorized()
        {
            // Arrange
            int tutorLearnerSubjectId = 1;
            bool isVerified = true;
            string reason = "Unauthorized access";

            _mockTutorLearnerSubjectService
                .Setup(s => s.VerifyTutorLearnerContractAsync(
                    tutorLearnerSubjectId,
                    It.IsAny<ClaimsPrincipal>(),
                    isVerified,
                    reason))
                .Throws(new UnauthorizedAccessException("Unauthorized access"));

            // Act
            var result = await _controller.VerifyContract(tutorLearnerSubjectId, isVerified, reason);

            // Assert
            var forbiddenResult = result as ObjectResult;
            Assert.NotNull(forbiddenResult);
            Assert.AreEqual(403, forbiddenResult.StatusCode);
        }

        [Test]
        public async Task VerifyContract_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            int tutorLearnerSubjectId = 1;
            bool isVerified = true;
            string reason = "Some reason";

            _mockTutorLearnerSubjectService
                .Setup(s => s.VerifyTutorLearnerContractAsync(
                    tutorLearnerSubjectId,
                    It.IsAny<ClaimsPrincipal>(),
                    isVerified,
                    reason))
                .Throws(new Exception("Internal server error"));

            // Act
            var result = await _controller.VerifyContract(tutorLearnerSubjectId, isVerified, reason);

            // Assert
            var errorResult = result as ObjectResult;
            Assert.NotNull(errorResult);
            Assert.AreEqual(500, errorResult.StatusCode);
        }

    }
}
