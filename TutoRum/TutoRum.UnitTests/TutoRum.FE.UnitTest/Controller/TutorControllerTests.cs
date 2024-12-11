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
    public class TutorControllerTests
    {
        private Mock<ITutorService> _tutorServiceMock;
        private Mock<ITeachingLocationsService> _teachingLocationsServiceMock;
        private Mock<ICertificatesSevice> _certificatesSeviceMock;
        private Mock<ISubjectService> _subjectService;
        private TutorController _controller;
        private ClaimsPrincipal _user;

        [SetUp]
        public void SetUp()
        {
            _tutorServiceMock = new Mock<ITutorService>();
            _certificatesSeviceMock = new Mock<ICertificatesSevice>();
            _subjectService = new Mock<ISubjectService>();
            _teachingLocationsServiceMock = new Mock<ITeachingLocationsService>();
            // Create a mock user
            _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.Name, "testuser@example.com"),
            new Claim(ClaimTypes.NameIdentifier, "1")
            }, "mock"));

            _controller = new TutorController(_tutorServiceMock.Object, _teachingLocationsServiceMock.Object, _certificatesSeviceMock.Object, _subjectService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = _user }
                }
            };
        }

        [Test]
        public async Task RegisterTutor_ShouldReturnOk_WhenTutorRegisteredSuccessfully()
        {
            // Arrange
            var tutorDto = new AddTutorDTO(); // Replace with appropriate mock data

            _tutorServiceMock.Setup(x => x.RegisterTutorAsync(It.IsAny<AddTutorDTO>(), It.IsAny<ClaimsPrincipal>()))
                             .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.RegisterTutor(tutorDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task RegisterTutor_ShouldReturnUnauthorized_WhenUnauthorizedAccessExceptionThrown()
        {
            // Arrange
            var tutorDto = new AddTutorDTO();
            _tutorServiceMock.Setup(x => x.RegisterTutorAsync(It.IsAny<AddTutorDTO>(), It.IsAny<ClaimsPrincipal>()))
                             .ThrowsAsync(new UnauthorizedAccessException("Unauthorized access"));

            // Act
            var result = await _controller.RegisterTutor(tutorDto);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(403, statusCodeResult.StatusCode);

            var apiResponse = statusCodeResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("Unauthorized access", apiResponse.Message);
        }

        [Test]
        public async Task RegisterTutor_ShouldReturnNotFound_WhenKeyNotFoundExceptionThrown()
        {
            // Arrange
            var tutorDto = new AddTutorDTO();
            _tutorServiceMock.Setup(x => x.RegisterTutorAsync(It.IsAny<AddTutorDTO>(), It.IsAny<ClaimsPrincipal>()))
                             .ThrowsAsync(new KeyNotFoundException("Tutor not found"));

            // Act
            var result = await _controller.RegisterTutor(tutorDto);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);

            var apiResponse = notFoundResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("Tutor not found", apiResponse.Message);

        }

     
        

        [Test]
        public async Task GetTutorById_ShouldReturnOk_WhenTutorExists()
        {
            // Arrange
            var tutorDto = new TutorDto(); // Mock data
            var tutorId = Guid.NewGuid();

            _tutorServiceMock.Setup(x => x.GetTutorByIdAsync(It.IsAny<Guid>())).ReturnsAsync(tutorDto);

            // Act
            var result = await _controller.GetTutorById(tutorId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task GetTutorById_ShouldReturnNotFound_WhenTutorDoesNotExist()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            _tutorServiceMock.Setup(x => x.GetTutorByIdAsync(It.IsAny<Guid>())).ThrowsAsync(new KeyNotFoundException("Tutor not found"));

            // Act
            var result = await _controller.GetTutorById(tutorId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);

            var apiResponse = notFoundResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("Tutor not found", apiResponse.Message);
        }


        [Test]
        public async Task DeleteTutor_ShouldReturnOk_WhenTutorDeletedSuccessfully()
        {
            // Arrange
            var tutorId = Guid.NewGuid();

            _tutorServiceMock.Setup(x => x.DeleteTutorAsync(It.IsAny<Guid>(), It.IsAny<ClaimsPrincipal>()))
                             .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteTutor(tutorId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task DeleteTutor_ShouldReturnUnauthorized_WhenUnauthorizedAccessExceptionThrown()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            _tutorServiceMock.Setup(x => x.DeleteTutorAsync(It.IsAny<Guid>(), It.IsAny<ClaimsPrincipal>()))
                             .ThrowsAsync(new UnauthorizedAccessException("Unauthorized access"));

            // Act
            var result = await _controller.DeleteTutor(tutorId);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(403, statusCodeResult.StatusCode);

            var apiResponse = statusCodeResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("Unauthorized access", apiResponse.Message);
        }


        [Test]
        public async Task DeleteTutor_ShouldReturnNotFound_WhenTutorDoesNotExist()
        {
            // Arrange
            var tutorId = Guid.NewGuid(); // Create a sample tutor ID
            var exceptionMessage = "Tutor not found"; // Custom message for exception

            // Setup the service to throw KeyNotFoundException when DeleteTutorAsync is called
            _tutorServiceMock.Setup(x => x.DeleteTutorAsync(It.IsAny<Guid>(), It.IsAny<ClaimsPrincipal>()))
                             .ThrowsAsync(new KeyNotFoundException(exceptionMessage));

            // Act
            var result = await _controller.DeleteTutor(tutorId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult; // Check if result is NotFoundObjectResult
            Assert.IsNotNull(notFoundResult); // Ensure it's not null
            Assert.AreEqual(404, notFoundResult.StatusCode); // Check if it returns 404 NotFound

            // Extract the ApiResponse from the result
            var apiResponse = notFoundResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse); 
            Assert.AreEqual(exceptionMessage, apiResponse.Message); 
        }

        [Test]
        public async Task RegisterTutor_ShouldReturnServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var tutorDto = new AddTutorDTO(); // Replace with appropriate test data
            var exceptionMessage = "An internal server error occurred."; // Custom exception message

            // Setup the service to throw a general Exception when RegisterTutorAsync is called
            _tutorServiceMock.Setup(x => x.RegisterTutorAsync(It.IsAny<AddTutorDTO>(), It.IsAny<ClaimsPrincipal>()))
                             .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.RegisterTutor(tutorDto);

            // Assert
            var statusCodeResult = result as ObjectResult; // Check if result is ObjectResult
            Assert.IsNotNull(statusCodeResult); // Ensure it's not null
            Assert.AreEqual(500, statusCodeResult.StatusCode); // Check if it returns 500 Server Error

            // Extract the ApiResponse from the result
            var apiResponse = statusCodeResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse); // Ensure the response is not null
            Assert.AreEqual(exceptionMessage, apiResponse.Message); // Ensure the message matches the exception message
        }


        [Test]
        public async Task GetTutorsHomePage_ShouldReturnOk_WhenTutorsFound()
        {
            // Arrange
            var tutorHomePageDto = new TutorHomePageDTO(); // Replace with mock data
            _tutorServiceMock.Setup(x => x.GetTutorHomePage(null,It.IsAny<int>(), It.IsAny<int>()))
                             .ReturnsAsync(tutorHomePageDto);

            // Act
            var result = await _controller.GetTutorsHomePage(null);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult); // Ensure it is OkObjectResult
            Assert.AreEqual(200, okResult.StatusCode); // Check for 200 status code

            var apiResponse = okResult.Value as ApiResponse<TutorHomePageDTO>;
            Assert.IsNotNull(apiResponse); // Ensure the ApiResponse is not null
            Assert.AreEqual(tutorHomePageDto, apiResponse.Data); // Check that the data is correct
        }

        [Test]
        public async Task GetTutorsHomePage_ShouldReturnNotFound_WhenNoTutorsFound()
        {
            // Arrange
            _tutorServiceMock.Setup(x => x.GetTutorHomePage(null, It.IsAny<int>(), It.IsAny<int>()))
                             .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.GetTutorsHomePage(null);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult); // Ensure it is NotFoundObjectResult
            Assert.AreEqual(404, notFoundResult.StatusCode); // Check for 404 status code

            var apiResponse = notFoundResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse); // Ensure the ApiResponse is not null
            Assert.AreEqual("No tutor requests found.", apiResponse.Message); // Ensure the error message is correct
        }

        [Test]
        public async Task GetTutorsHomePage_ShouldReturnServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var exceptionMessage = "An internal server error occurred.";
            _tutorServiceMock.Setup(x => x.GetTutorHomePage(null, It.IsAny<int>(), It.IsAny<int>()))
                             .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetTutorsHomePage(null);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult); // Ensure it is ObjectResult
            Assert.AreEqual(500, statusCodeResult.StatusCode); // Check for 500 status code

            var apiResponse = statusCodeResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse); // Ensure the ApiResponse is not null
            Assert.AreEqual(exceptionMessage, apiResponse.Message); // Ensure the error message matches
        }

       
        [Test]
        public async Task GetTutorById_ShouldReturnServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var tutorId = Guid.NewGuid(); // Replace with appropriate test data
            var exceptionMessage = "An internal server error occurred.";

            _tutorServiceMock.Setup(x => x.GetTutorByIdAsync(It.IsAny<Guid>()))
                             .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetTutorById(tutorId);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode); // Ensure it returns 500

            var apiResponse = statusCodeResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual(exceptionMessage, apiResponse.Message); // Ensure the error message matches
        }

        [Test]
        public async Task DeleteTutor_ShouldReturnServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var tutorId = Guid.NewGuid(); // Replace with appropriate test data
            var exceptionMessage = "An internal server error occurred.";

            _tutorServiceMock.Setup(x => x.DeleteTutorAsync(It.IsAny<Guid>(), It.IsAny<ClaimsPrincipal>()))
                             .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.DeleteTutor(tutorId);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode); // Ensure it returns 500

            var apiResponse = statusCodeResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual(exceptionMessage, apiResponse.Message); // Ensure the error message matches
        }

    }
}
