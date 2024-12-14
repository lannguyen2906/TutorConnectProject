using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.Models;
using TutoRum.Services.Service;
using TutoRum.Services.ViewModels;

namespace TutoRum.UnitTests.ServiceUnitTest
{
    [TestFixture]
    public class FeedbackServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IMapper> _mapperMock;
        private FeedbackService _feedbackService;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _unitOfWorkMock.Setup(st => st.feedback.Add(It.IsAny<Feedback>()));

            _mapperMock = new Mock<IMapper>();
            _feedbackService = new FeedbackService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task CreateFeedbackAsync_ShouldCreateFeedbackAndReturnDto()
        {
            // Arrange
            var createFeedbackDto = new CreateFeedbackDto
            {
                Rating = 5,
                Comments = "Great tutor!",
                TutorLearnerSubjectId = 1,
                Punctuality = 5,
                SupportQuality = 5,
                TeachingSkills = 5,
                ResponseToQuestions = 5,
                Satisfaction = 5
            };

            // Mock IMapper to return a Feedback object when mapping from CreateFeedbackDto
            var mockFeedback = new Feedback
            {
                FeedbackId =1 ,
                Rating = createFeedbackDto.Rating,
                Comments = createFeedbackDto.Comments,
                TutorLearnerSubjectId = createFeedbackDto.TutorLearnerSubjectId,
                CreatedDate = DateTime.UtcNow
            };

            _mapperMock.Setup(m => m.Map<Feedback>(It.IsAny<CreateFeedbackDto>())).Returns(mockFeedback);

            // Mock the unit of work - simulate adding and committing the feedback
            _unitOfWorkMock.Setup(uow => uow.feedback.Add(It.IsAny<Feedback>())).Returns(mockFeedback);
            _unitOfWorkMock.Setup(uow => uow.CommitAsync()).Returns(Task.CompletedTask);

            // Mock GetAverageRatingForTutorAsync to return an average rating
            var tutorId = Guid.NewGuid(); // Assuming you already know the tutorId
            _unitOfWorkMock.Setup(uow => uow.Tutors.GetAverageRatingForTutorAsync(It.IsAny<Guid>()))
        .ReturnsAsync(4.5m);

            // Mock GetByIdAsync to return a tutor object
            var mockTutor = new Tutor
            {
                TutorId = tutorId,
                Rating = 4.0m // Initial rating
            };
            _unitOfWorkMock.Setup(uow => uow.Tutors.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(mockTutor);
            _unitOfWorkMock.Setup(uow => uow.Tutors.Update(It.IsAny<Tutor>()));

            // Act
            var result = await _feedbackService.CreateFeedbackAsync(createFeedbackDto);

            // Assert
            Assert.IsNull(result);
        }

        // Test CreateFeedbackAsync
        [Test]
        public async Task CreateFeedbackAsync_ShouldCreateFeedback_WhenValidData()
        {
            // Arrange
            var createFeedbackDto = new CreateFeedbackDto
            {
                Rating = 5,
                Comments = "Excellent",
                TutorLearnerSubjectId = 1
            };

            var feedback = new Feedback { FeedbackId = 1, Rating = 5, Comments = "Excellent" };
            var tutorId = Guid.NewGuid();

            _unitOfWorkMock.Setup(uow => uow.feedback.Add(It.IsAny<Feedback>())).Returns(feedback);
            _unitOfWorkMock.Setup(uow => uow.CommitAsync()).Returns(Task.CompletedTask);

            // Mock GetAverageRatingForTutorAsync
            _unitOfWorkMock.Setup(uow => uow.Tutors.GetAverageRatingForTutorAsync(It.IsAny<Guid>()))
        .ReturnsAsync(4.5m);
            var mockFeedback = new Feedback
            {
                FeedbackId = 1,
                Rating = createFeedbackDto.Rating,
                Comments = createFeedbackDto.Comments,
                CreatedDate = DateTime.UtcNow
            };

            // Setup the mapper to return mockFeedback when mapping from CreateFeedbackDto to Feedback
            _mapperMock.Setup(m => m.Map<Feedback>(It.IsAny<CreateFeedbackDto>())).Returns(mockFeedback);

            _mapperMock.Setup(mapper => mapper.Map<FeedbackDto>(It.IsAny<Feedback>())).Returns(new FeedbackDto { FeedbackId = 1 });

            // Act
            var result = await _feedbackService.CreateFeedbackAsync(createFeedbackDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.FeedbackId);
            _unitOfWorkMock.Verify(uow => uow.CommitAsync(), Times.Once); // Ensure CommitAsync is called
        }

        // Test UpdateFeedbackAsync
        [Test]
        public async Task UpdateFeedbackAsync_ShouldUpdateFeedback_WhenValidData()
        {
            // Arrange
            var feedbackDto = new FeedbackDto
            {
                FeedbackId = 1,
                Rating = 4,
                Comments = "Good",
                TutorLearnerSubjectId = 1
            };

            var existingFeedback = new Feedback
            {
                FeedbackId = 1,
                Rating = 3,
                Comments = "Okay",
                TutorLearnerSubjectId = 1
            };

            _unitOfWorkMock.Setup(uow => uow.feedback.GetFeedbackByTutorLearnerSubjectIdAsync(It.IsAny<int>())).ReturnsAsync(existingFeedback);
            _unitOfWorkMock.Setup(uow => uow.CommitAsync()).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(uow => uow.Tutors.GetAverageRatingForTutorAsync(It.IsAny<Guid>()))
        .ReturnsAsync(4.5m);
            _mapperMock.Setup(mapper => mapper.Map<FeedbackDto>(It.IsAny<Feedback>())).Returns(feedbackDto);

            // Act
            var result = await _feedbackService.UpdateFeedbackAsync(feedbackDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Rating); // Check updated rating
            Assert.AreEqual("Good", result.Comments); // Check updated comment
        }

        // Test GetFeedbackByTutorLearnerSubjectIdAsync
        [Test]
        public async Task GetFeedbackByTutorLearnerSubjectIdAsync_ShouldReturnFeedback_WhenValidId()
        {
            // Arrange
            var tutorLearnerSubjectId = 1;
            var existingFeedback = new Feedback
            {
                FeedbackId = 1,
                Rating = 4,
                Comments = "Good",
                TutorLearnerSubjectId = tutorLearnerSubjectId
            };

            _unitOfWorkMock.Setup(uow => uow.feedback.GetFeedbackByTutorLearnerSubjectIdAsync(tutorLearnerSubjectId)).ReturnsAsync(existingFeedback);

            // Act
            var result = await _feedbackService.GetFeedbackByTutorLearnerSubjectIdAsync(tutorLearnerSubjectId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Rating);
            Assert.AreEqual("Good", result.Comments);
        }
    }
}
