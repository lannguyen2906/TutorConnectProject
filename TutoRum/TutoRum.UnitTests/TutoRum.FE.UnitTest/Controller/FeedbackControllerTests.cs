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
    public class FeedbackControllerTests
    {
        private Mock<IFeedbackService> _mockFeedbackService;
        private FeedbackController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockFeedbackService = new Mock<IFeedbackService>();
            _controller = new FeedbackController(_mockFeedbackService.Object);
        }

        [Test]
        public async Task GetFeedbackDetailByTutorId_ReturnsOk_WhenFeedbackExists()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var feedbackDetail = new FeedbackDetail();
            _mockFeedbackService
                .Setup(s => s.GetFeedbackDetailByTutorIdAsync(tutorId, false))
                .ReturnsAsync(feedbackDetail);

            // Act
            var result = await _controller.GetFeedbackDetailByTutorId(tutorId, false);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var apiResponse = okResult.Value as ApiResponse<FeedbackDetail>;
            Assert.NotNull(apiResponse);
            Assert.AreEqual(feedbackDetail, apiResponse.Data);
        }

        [Test]
        public async Task UpdateFeedback_ShouldReturnNotFound_WhenFeedbackDoesNotExist()
        {
            // Arrange
            var feedbackDto = new FeedbackDto { FeedbackId = 1, Rating = 5 };
            _mockFeedbackService.Setup(service => service.UpdateFeedbackAsync(It.IsAny<FeedbackDto>()))
                .ThrowsAsync(new KeyNotFoundException("Feedback not found"));

            // Act
            var result = await _controller.UpdateFeedback(feedbackDto);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task CreateFeedback_ShouldReturnServerError_WhenExceptionOccurs()
        {
            // Arrange
            var createFeedbackDto = new CreateFeedbackDto { Rating = 5, Comments = "Great tutor!" };
            _mockFeedbackService.Setup(service => service.CreateFeedbackAsync(It.IsAny<CreateFeedbackDto>()))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.CreateFeedback(createFeedbackDto);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }


        [Test]
        public async Task GetFeedbackDetailByTutorId_ReturnsServerError_WhenExceptionThrown()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            _mockFeedbackService
                .Setup(s => s.GetFeedbackDetailByTutorIdAsync(tutorId, false))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetFeedbackDetailByTutorId(tutorId, false);

            // Assert
            var serverErrorResult = result as ObjectResult;
            Assert.NotNull(serverErrorResult);
            Assert.AreEqual(500, serverErrorResult.StatusCode);
        }

        [Test]
        public async Task GetByFeedbackId_ReturnsOk_WhenFeedbackExists()
        {
            // Arrange
            var tutorLearnerSubjectId = 1;
            var feedback = new FeedbackDto();
            _mockFeedbackService
                .Setup(s => s.GetFeedbackByTutorLearnerSubjectIdAsync(tutorLearnerSubjectId))
                .ReturnsAsync(feedback);

            // Act
            var result = await _controller.GetByFeedbackId(tutorLearnerSubjectId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var apiResponse = okResult.Value as ApiResponse<FeedbackDto>;
            Assert.NotNull(apiResponse);
            Assert.AreEqual(feedback, apiResponse.Data);
        }

        [Test]
        public async Task CreateFeedback_ReturnsOk_WhenFeedbackCreatedSuccessfully()
        {
            // Arrange
            var createFeedbackDto = new CreateFeedbackDto();
            var createdFeedback = new FeedbackDto();
            _mockFeedbackService
                .Setup(s => s.CreateFeedbackAsync(createFeedbackDto))
                .ReturnsAsync(createdFeedback);

            // Act
            var result = await _controller.CreateFeedback(createFeedbackDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var apiResponse = okResult.Value as ApiResponse<FeedbackDto>;
            Assert.NotNull(apiResponse);
            Assert.AreEqual(createdFeedback, apiResponse.Data);
        }

        [Test]
        public async Task UpdateFeedback_ReturnsOk_WhenFeedbackUpdatedSuccessfully()
        {
            // Arrange
            var feedbackDto = new FeedbackDto();
            var updatedFeedback = new FeedbackDto();
            _mockFeedbackService
                .Setup(s => s.UpdateFeedbackAsync(feedbackDto))
                .ReturnsAsync(updatedFeedback);

            // Act
            var result = await _controller.UpdateFeedback(feedbackDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var apiResponse = okResult.Value as ApiResponse<FeedbackDto>;
            Assert.NotNull(apiResponse);
            Assert.AreEqual(updatedFeedback, apiResponse.Data);
        }

        [Test]
        public async Task GetFeedbackStatistics_ReturnsOk_WhenStatisticsRetrievedSuccessfully()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            _mockFeedbackService
                .Setup(s => s.GetFeedbackStatisticsForTutorAsync(It.IsAny<Guid>()))
    .ReturnsAsync((
        new List<QuestionStatistics>
        {
            new QuestionStatistics { QuestionType = "1", TotalAnswerCount = "3.4" },
            new QuestionStatistics { QuestionType = "2", TotalAnswerCount = "3.8" }
        },
        new List<string>
        {
            "Great tutor!",
            "Very helpful and patient."
        }
    ));

            // Act
            var result = await _controller.GetFeedbackStatistics(tutorId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var apiResponse = okResult.Value as ApiResponse<FeedbackStatisticsResponse>;
            Assert.NotNull(apiResponse);
        }

        [Test]
        public async Task GetFeedbackStatistics_ReturnsNotFound_WhenTutorIdDoesNotExist()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            _mockFeedbackService
                .Setup(s => s.GetFeedbackStatisticsForTutorAsync(tutorId))
                .ThrowsAsync(new KeyNotFoundException("Tutor not found"));

            // Act
            var result = await _controller.GetFeedbackStatistics(tutorId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }
    }

}
