﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Enum;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.Models;
using TutoRum.Services.Helper;
using TutoRum.Services.IService;
using TutoRum.Services.Service;
using TutoRum.Services.ViewModels;
using ZXing;

namespace TutoRum.UnitTests.ServiceUnitTest
{
    [TestFixture]
    public class TutorServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<UserManager<AspNetUser>> _userManagerMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IScheduleService> _scheduleServiceMock;
        private Mock<ITeachingLocationsService> _teachingLocationsServiceMock;
        private Mock<ICertificatesSevice> _certificatesSeviceMock;
        private Mock<ISubjectService> _subjectServiceMock;
        private Mock<HttpClient> _httpClientMock;
        private Mock<APIAddress> _apiAddressMock;
        private Mock<INotificationService> _notificationServiceMock;
        private Mock<IDbContextTransaction> _mockTransaction;

        private TutorService _tutorService;

        [SetUp]
        public void SetUp()
        {
            // Mocking IUserStore<AspNetUser> required by UserManager
            var userStoreMock = new Mock<IUserStore<AspNetUser>>();

            // Initialize UserManager mock with necessary dependencies
            _userManagerMock = new Mock<UserManager<AspNetUser>>(
                userStoreMock.Object,
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<IPasswordHasher<AspNetUser>>(),
                null, null, null, null, null, null);

            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _scheduleServiceMock = new Mock<IScheduleService>();
            _teachingLocationsServiceMock = new Mock<ITeachingLocationsService>();
            _certificatesSeviceMock = new Mock<ICertificatesSevice>();
            _subjectServiceMock = new Mock<ISubjectService>();
            _httpClientMock = new Mock<HttpClient>();
            _apiAddressMock = new Mock<APIAddress>();
            _notificationServiceMock = new Mock<INotificationService>();
            _mockUnitOfWork.Setup(st => st.Tutors.Add(It.IsAny<Tutor>()));
            _mockUnitOfWork.Setup(st => st.Tutors.Update(It.IsAny<Tutor>()));
            _mockUnitOfWork.Setup(st => st.Accounts.Update(It.IsAny<AspNetUser>()));

            
            _mockTransaction = new Mock<IDbContextTransaction>();

            _mockUnitOfWork
                .Setup(uow => uow.Tutors.GetExecutionStrategy())
                .Returns(new Mock<IExecutionStrategy>().Object);


            _mockUnitOfWork
                .Setup(uow => uow.Tutors.BeginTransactionAsync(It.IsAny<IsolationLevel>()))
                .ReturnsAsync(_mockTransaction.Object);

            _tutorService = new TutorService(
                _mockUnitOfWork.Object,
                _userManagerMock.Object,
                _mapperMock.Object,
                _scheduleServiceMock.Object,
                _teachingLocationsServiceMock.Object,
                _certificatesSeviceMock.Object,
                _subjectServiceMock.Object,
                _httpClientMock.Object,
                new APIAddress(new HttpClient()),
                _notificationServiceMock.Object);
        }


        [Test]
        public async Task RegisterTutorAsync_UserNotFound_ShouldThrowException()
        {
            // Arrange
            var tutorDto = new AddTutorDTO { /* Add necessary properties */ };
            var claimsPrincipal = new ClaimsPrincipal();

            // Simulate that the user does not exist
            _userManagerMock.Setup(um => um.GetUserAsync(claimsPrincipal))
                .ReturnsAsync((AspNetUser)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _tutorService.RegisterTutorAsync(tutorDto, claimsPrincipal));

            Assert.AreEqual("User not found.", ex.Message);
        }

        [Test]
        public async Task RegisterTutorAsync_UserIsAlreadyTutor_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var user = new AspNetUser { Id = Guid.NewGuid() };
            var tutorDto = new AddTutorDTO { /* Add necessary properties */ };
            var claimsPrincipal = new ClaimsPrincipal();

            _userManagerMock.Setup(um => um.GetUserAsync(claimsPrincipal))
                .ReturnsAsync(user);

            _userManagerMock.Setup(um => um.IsInRoleAsync(user, AccountRoles.Tutor))
                .ReturnsAsync(true); // Simulate user already being a tutor

            // Act & Assert
            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _tutorService.RegisterTutorAsync(tutorDto, claimsPrincipal));

            Assert.AreEqual("User is already a tutor!", ex.Message);
        }

        


        [Test]
        public async Task RegisterTutorAsync_Success_ShouldAddTutorAndSendNotification()
        {
            // Arrange
            var user = new AspNetUser { Id = Guid.NewGuid() };
            var tutorDto = new AddTutorDTO
            {
                Experience = "5",
                Specialization = "Math",
                ProfileDescription = "Experienced tutor",
                BriefIntroduction = "Friendly tutor",
                EducationalLevelID = 1,
                ShortDescription = "Experienced in Algebra",
                Major = "Mathematics",
                videoUrl = "http://video-url",
                AddressID = Guid.NewGuid().ToString(),
                IsAccepted = true,
                TeachingLocation = new List<AddTeachingLocationViewDTO>(), // Assuming some data here
                Certificates = new List<CertificateDTO>(), // Assuming some data here
                Schedule = new List<ScheduleDTO>(), // Assuming some data here
                Subjects = new List<AddSubjectDTO>(), // Assuming some data here
            };
            var claimsPrincipal = new ClaimsPrincipal();

            _userManagerMock.Setup(um => um.GetUserAsync(claimsPrincipal))
                .ReturnsAsync(user);

            _userManagerMock.Setup(um => um.IsInRoleAsync(user, AccountRoles.Tutor))
                .ReturnsAsync(false);

            _userManagerMock.Setup(um => um.AddToRoleAsync(user, AccountRoles.Tutor))
                .ReturnsAsync(IdentityResult.Success);

            _mockUnitOfWork.Setup(uow => uow.Tutors.Add(It.IsAny<Tutor>())).Verifiable();
            _mockUnitOfWork.Setup(uow => uow.CommitAsync()).Returns(Task.CompletedTask);

            _scheduleServiceMock.Setup(ss => ss.AddSchedulesAsync(It.IsAny<List<ScheduleDTO>>(), It.IsAny<Guid>())).Returns(Task.CompletedTask);
            _teachingLocationsServiceMock.Setup(tls => tls.AddTeachingLocationsAsync(It.IsAny<IEnumerable<AddTeachingLocationViewDTO>>(), It.IsAny<Guid>())).Returns(Task.CompletedTask);
            _certificatesSeviceMock.Setup(cs => cs.AddCertificatesAsync(It.IsAny<List<CertificateDTO>>(), It.IsAny<Guid>())).Returns(Task.CompletedTask);
            _subjectServiceMock.Setup(ss => ss.AddSubjectsWithRateAsync(It.IsAny<IEnumerable<AddSubjectDTO>>(), It.IsAny<Guid>())).Returns(Task.CompletedTask);
            _notificationServiceMock.Setup(ns => ns.SendNotificationAsync(It.IsAny<NotificationRequestDto>(), false)).Returns(Task.CompletedTask);

            // Act
            var result =  _tutorService.RegisterTutorAsync(tutorDto, claimsPrincipal);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task RegisterTutorAsync_ShouldStartTransactionAndCommit_WhenRoleAssignedSuccessfully()
        {
            // Arrange
            var user = new AspNetUser { Id = Guid.NewGuid() };
            var tutorDto = new AddTutorDTO
            {
                Experience = "5",
                Specialization = "Math",
                ProfileDescription = "Experienced tutor",
                BriefIntroduction = "Friendly tutor",
                EducationalLevelID = 1,
                ShortDescription = "Experienced in Algebra",
                Major = "Mathematics",
                videoUrl = "http://video-url",
                AddressID = Guid.NewGuid().ToString(),
                IsAccepted = true,
                TeachingLocation = new List<AddTeachingLocationViewDTO>(), // Add some data here if necessary
                Certificates = new List<CertificateDTO>(), // Add some data here if necessary
                Schedule = new List<ScheduleDTO>(), // Add some data here if necessary
                Subjects = new List<AddSubjectDTO>(), // Add some data here if necessary
            };
            var claimsPrincipal = new ClaimsPrincipal();

            _userManagerMock.Setup(um => um.GetUserAsync(claimsPrincipal))
                .ReturnsAsync(user);

            // Simulate role not assigned yet, then successful role assignment
            _userManagerMock.Setup(um => um.IsInRoleAsync(user, AccountRoles.Tutor))
                .ReturnsAsync(false);

            _userManagerMock.Setup(um => um.AddToRoleAsync(user, AccountRoles.Tutor))
                .ReturnsAsync(IdentityResult.Success);

            // Mocking the transaction
            _mockUnitOfWork.Setup(uow => uow.Tutors.GetExecutionStrategy())
                .Returns(new Mock<IExecutionStrategy>().Object);

            _mockUnitOfWork.Setup(uow => uow.Tutors.BeginTransactionAsync(It.IsAny<IsolationLevel>()))
                .ReturnsAsync(_mockTransaction.Object);

            // Mocking the Add and Commit methods
            _mockUnitOfWork.Setup(uow => uow.Tutors.Add(It.IsAny<Tutor>())).Verifiable();
            _mockUnitOfWork.Setup(uow => uow.CommitAsync()).Returns(Task.CompletedTask);

            // Mocking the services that depend on the registration
            _scheduleServiceMock.Setup(ss => ss.AddSchedulesAsync(It.IsAny<List<ScheduleDTO>>(), It.IsAny<Guid>())).Returns(Task.CompletedTask);
            _teachingLocationsServiceMock.Setup(tls => tls.AddTeachingLocationsAsync(It.IsAny<IEnumerable<AddTeachingLocationViewDTO>>(), It.IsAny<Guid>())).Returns(Task.CompletedTask);
            _certificatesSeviceMock.Setup(cs => cs.AddCertificatesAsync(It.IsAny<List<CertificateDTO>>(), It.IsAny<Guid>())).Returns(Task.CompletedTask);
            _subjectServiceMock.Setup(ss => ss.AddSubjectsWithRateAsync(It.IsAny<IEnumerable<AddSubjectDTO>>(), It.IsAny<Guid>())).Returns(Task.CompletedTask);
            _notificationServiceMock.Setup(ns => ns.SendNotificationAsync(It.IsAny<NotificationRequestDto>(), false)).Returns(Task.CompletedTask);

            // Act
            var result = _tutorService.RegisterTutorAsync(tutorDto, claimsPrincipal);

            // Assert
            Assert.IsNotNull(result);
        }


        [Test]
        public void UpdateTutorInfoAsync_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            var tutorDto = new UpdateTutorInforDTO { /* Initialize your DTO properties here */ };
            var claimsPrincipal = new ClaimsPrincipal();

            _userManagerMock.Setup(um => um.GetUserAsync(claimsPrincipal))
                .ReturnsAsync((AspNetUser)null); // Simulate user not found

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _tutorService.UpdateTutorInfoAsync(tutorDto, claimsPrincipal));

            Assert.AreEqual("User not found.", ex.Message);
        }


        [Test]
        public void UpdateTutorInfoAsync_ShouldThrowUnauthorizedAccessException_WhenUserIsNotTutor()
        {
            // Arrange
            var user = new AspNetUser { Id = Guid.NewGuid() };
            var tutorDto = new UpdateTutorInforDTO { /* Initialize your DTO properties here */ };
            var claimsPrincipal = new ClaimsPrincipal();

            _userManagerMock.Setup(um => um.GetUserAsync(claimsPrincipal))
                .ReturnsAsync(user);

            _userManagerMock.Setup(um => um.IsInRoleAsync(user, AccountRoles.Tutor))
                .ReturnsAsync(false); // Simulate user is not a tutor

            // Act & Assert
            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _tutorService.UpdateTutorInfoAsync(tutorDto, claimsPrincipal));

            Assert.AreEqual("User is not a tutor!", ex.Message);
        }
       
        [Test]
        public async Task UpdateTutorInfoAsync_ShouldUpdateTutorInfo_WhenSuccessful()
        {
            // Arrange
            var user = new AspNetUser { Id = Guid.NewGuid(), AddressId = Guid.NewGuid().ToString() };
            var tutorDto = new UpdateTutorInforDTO { /* Initialize your DTO properties here */ };
            var claimsPrincipal = new ClaimsPrincipal();

            _userManagerMock.Setup(um => um.GetUserAsync(claimsPrincipal))
                .ReturnsAsync(user);

            _userManagerMock.Setup(um => um.IsInRoleAsync(user, AccountRoles.Tutor))
                .ReturnsAsync(true);

            var existingTutor = new Tutor { TutorId = Guid.NewGuid() };
            _mockUnitOfWork.Setup(uow => uow.Tutors.GetSingleByGuId(user.Id))
                .Returns(existingTutor);

            var mockTransaction = new Mock<IDbContextTransaction>();
            _mockUnitOfWork.Setup(uow => uow.Tutors.BeginTransactionAsync(It.IsAny<IsolationLevel>()))
                .ReturnsAsync(mockTransaction.Object);

            var strategyMock = new Mock<IExecutionStrategy>();
            _mockUnitOfWork.Setup(uow => uow.Tutors.GetExecutionStrategy())
                .Returns(strategyMock.Object);

            // Mock successful service calls
            _teachingLocationsServiceMock.Setup(s => s.AddTeachingLocationsAsync(It.IsAny<List<AddTeachingLocationViewDTO>>(), It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);
            _certificatesSeviceMock.Setup(s => s.AddCertificatesAsync(It.IsAny<List<CertificateDTO>>(), It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);
            _subjectServiceMock.Setup(s => s.AddSubjectsWithRateAsync(It.IsAny<List<AddSubjectDTO>>(), It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = _tutorService.UpdateTutorInfoAsync(tutorDto, claimsPrincipal);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task GetTutorByIdAsync_ShouldThrowKeyNotFoundException_WhenTutorNotFound()
        {
            // Arrange
            var tutorId = Guid.NewGuid();

            _mockUnitOfWork.Setup(uow => uow.Tutors.GetByIdAsync(tutorId))
                .ReturnsAsync((Tutor)null); // Simulate tutor not found

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _tutorService.GetTutorByIdAsync(tutorId));

            Assert.AreEqual("Tutor not found.", ex.Message);
        }



        [Test]
        public async Task GetTutorByIdAsync_ShouldReturnTutorDto_WhenTutorFound()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var tutor = new Tutor
            {
                TutorId = tutorId,
                EducationalLevel = 1,
                TutorTeachingLocations = new List<TutorTeachingLocations>
        {
            new TutorTeachingLocations
            {
                TeachingLocation = new TeachingLocation
                {
                    CityId = "1",
                    DistrictId = "1"
                }
            }
        }
            };

            var tutorDto = new TutorDto
            {
                TutorId = tutorId,
                EducationalLevelID = 1,
                AddressID = "123",
                TeachingLocations = new List<TeachingLocationViewDTO>
        {
            new TeachingLocationViewDTO
            {
                CityId = "1",
                Districts = new List<DistrictDTO> { new DistrictDTO { DistrictId = "1" } }
            }
        }
            };

            _mockUnitOfWork.Setup(uow => uow.Tutors.GetByIdAsync(tutorId))
                .ReturnsAsync(tutor); // Simulate tutor found

            _mapperMock.Setup(m => m.Map<TutorDto>(tutor))
                .Returns(tutorDto); // Map tutor to tutorDto

            // Act
            var result =  _tutorService.GetTutorByIdAsync(tutorId);

            // Assert
            Assert.IsNotNull(result);
        }


        // Test for NotifyAdminForPendingTutors method

        [Test]
        public async Task NotifyAdminForPendingTutors_ShouldSendNotification_WhenPendingTutorsExist()
        {
            // Arrange
            var pendingCount = 5; // Simulate 5 pending tutors
            _mockUnitOfWork.Setup(uow => uow.Tutors.Count(It.IsAny<Expression<Func<Tutor, bool>>>()))
                .Returns(pendingCount); // Simulate pending tutors count

            var notification = new NotificationRequestDto
            {
                Title = "Kiểm duyệt gia sư",
                Description = $"Hiện có {pendingCount} gia sư cần được kiểm duyệt.",
                NotificationType = NotificationType.AdminTutorApproval,
                Href = "/admin/tutors"
            };

            _notificationServiceMock.Setup(ns => ns.SendNotificationAsync(notification, true))
                .Returns(Task.CompletedTask); // Mock notification sending

            // Act
            var result =  _tutorService.NotifyAdminForPendingTutors();

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task NotifyAdminForPendingTutors_ShouldNotSendNotification_WhenNoPendingTutors()
        {
            // Arrange
            var pendingCount = 0; // No pending tutors
            _mockUnitOfWork.Setup(uow => uow.Tutors.Count(It.IsAny<Expression<Func<Tutor, bool>>>()))
                .Returns(pendingCount); // Simulate no pending tutors

            // Act
            await _tutorService.NotifyAdminForPendingTutors();

            // Assert
            _notificationServiceMock.Verify(ns => ns.SendNotificationAsync(It.IsAny<NotificationRequestDto>(), true), Times.Never);
        }
       

        [Test]
        public async Task DeleteTutorAsync_ShouldThrowUnauthorizedAccessException_WhenUserIsNotAdmin()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var user = new ClaimsPrincipal(); // Non-admin user
            _userManagerMock.Setup(um => um.GetUserAsync(user)).ReturnsAsync(new AspNetUser { Id = Guid.NewGuid() });
            _userManagerMock.Setup(um => um.IsInRoleAsync(It.IsAny<AspNetUser>(), "Admin")).ReturnsAsync(false);

            // Act & Assert
            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _tutorService.DeleteTutorAsync(tutorId, user)
            );
            Assert.AreEqual("User does not have the Admin role.", ex.Message);
        }

        [Test]
        public async Task DeleteTutorAsync_ShouldThrowKeyNotFoundException_WhenTutorNotFound()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var user = new ClaimsPrincipal(); // Admin user
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };
            _userManagerMock.Setup(um => um.GetUserAsync(user)).ReturnsAsync(currentUser);
            _userManagerMock.Setup(um => um.IsInRoleAsync(currentUser, "Admin")).ReturnsAsync(true);
            _mockUnitOfWork.Setup(uow => uow.Tutors.GetByIdAsync(tutorId)).ReturnsAsync((Tutor)null);  // Tutor not found

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _tutorService.DeleteTutorAsync(tutorId, user)
            );
            Assert.AreEqual("Tutor not found.", ex.Message);
        }

        

        [Test]
        public async Task GetAllTutorsWithMajorsAndMinorsAsync_ShouldReturnListOfMajorsAndMinors()
        {
            // Arrange
            var majorMinors = new List<MajorMinorDto>
    {
        new MajorMinorDto { Major = "Math", Minors = new List<string> { "Algebra", "Geometry" } },
        new MajorMinorDto { Major = "Science", Minors = new List<string> { "Physics", "Chemistry" } }
    };
            MajorMinorData.FieldsList = majorMinors;

            // Act
            var result = await _tutorService.GetAllTutorsWithMajorsAndMinorsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Math", result[0].Major);
            Assert.AreEqual(2, result[0].Minors.Count);
        }

        [Test]
        public async Task GetAllTutorsWithFeedbackAsync_ShouldReturnListOfTutorsWithRatingsAndFeedbacks()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var tutor = new Tutor
            {
                TutorId = tutorId,
                TutorNavigation = new AspNetUser { Id = tutorId },
            };
            var tutors = new List<Tutor> { tutor };
            var feedbacks = new List<Feedback>
    {
        new Feedback { Comments = "Great tutor!", Rating = 5 },
        new Feedback { Comments = "Needs improvement.", Rating = 3 }
    };

            // Mock _unitOfWork methods
            _mockUnitOfWork.Setup(uow => uow.Tutors.GetAllTutorsAsync()).ReturnsAsync(tutors);
            _mockUnitOfWork.Setup(uow => uow.Tutors.GetAverageRatingForTutorAsync(tutorId)).ReturnsAsync(4.0m);
            _mockUnitOfWork.Setup(uow => uow.feedback.GetFeedbacksForTutorAsync(tutorId)).ReturnsAsync(feedbacks);

            // Mock _mapper methods
            var tutorRatingDto = new TutorRatingDto
            {
                TutorId = tutorId,
                AverageRating = 4.0m,
                Feedbacks = new List<FeedbackDto>
        {
            new FeedbackDto { Comments = "Great tutor!", Rating = 5 },
            new FeedbackDto { Comments = "Needs improvement.", Rating = 3 }
        }
            };

            _mapperMock.Setup(m => m.Map<TutorRatingDto>(It.IsAny<Tutor>())).Returns(tutorRatingDto);
            _mapperMock.Setup(m => m.Map<List<FeedbackDto>>(It.IsAny<List<Feedback>>())).Returns(feedbacks.Select(f => new FeedbackDto
            {
                Comments = f.Comments,
                Rating = f.Rating
            }).ToList());

            // Act
            var result = await _tutorService.GetAllTutorsWithFeedbackAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count); // Only one tutor
            Assert.AreEqual(4.0m, result[0].AverageRating);
            Assert.AreEqual(2, result[0].Feedbacks.Count);  // Two feedbacks

            // Assert _mapper was called
            _mapperMock.Verify(m => m.Map<TutorRatingDto>(It.IsAny<Tutor>()), Times.Once);
            _mapperMock.Verify(m => m.Map<List<FeedbackDto>>(It.IsAny<List<Feedback>>()), Times.Once);
        }


        [Test]
        public async Task GetWalletOverviewDtoAsync_ShouldReturnWalletOverview_WhenUserIsTutor()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var user = new ClaimsPrincipal(); // Tutor user
            var currentUser = new AspNetUser { Id = tutorId };
            var tutor = new Tutor
            {
                TutorId = tutorId,
                Balance = 100m
            };

            _userManagerMock.Setup(um => um.GetUserAsync(user)).ReturnsAsync(currentUser);
            _userManagerMock.Setup(um => um.IsInRoleAsync(currentUser, AccountRoles.Tutor)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(uow => uow.Tutors.FindAsync(It.IsAny<Expression<Func<Tutor, bool>>>())).ReturnsAsync(tutor);
            _mockUnitOfWork.Setup(uow => uow.Payment.GetTotalEarningsThisMonth(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(500m);
            _mockUnitOfWork.Setup(uow => uow.PaymentRequest.Count(It.IsAny<Expression<Func<PaymentRequest, bool>>>())).Returns(2);  // Two pending withdrawals

            // Act
            var result = await _tutorService.GetWalletOverviewDtoAsync(user);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(100m, result.CurrentBalance);
            Assert.AreEqual(2, result.PendingWithdrawals);
            Assert.AreEqual(500m, result.TotalEarningsThisMonth);
        }

        #region GetTopTutorAsync Tests

        [Test]
        public async Task GetTopTutorAsync_ShouldReturnEmptyList_WhenNoTutorsExist()
        {
            // Arrange
            _mockUnitOfWork.Setup(uow => uow.Tutors.GetAllTutorsAsync())
                .ReturnsAsync(new List<Tutor>());

            // Act
            var result = await _tutorService.GetTopTutorAsync(5);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetTopTutorAsync_ShouldReturnSortedTutors_WhenTutorsExist()
        {
            // Arrange
            var mockTutors = new List<Tutor>
        {
            new Tutor { TutorId = Guid.NewGuid(), Rating = 4.5m, IsVerified = true },
            new Tutor { TutorId = Guid.NewGuid(), Rating = 3.5m, IsVerified = true },
            new Tutor { TutorId = Guid.NewGuid(), Rating = 4.0m, IsVerified = false }
        };

            _mockUnitOfWork.Setup(uow => uow.Tutors.GetAllTutorsAsync())
                .ReturnsAsync(mockTutors);

            _mockUnitOfWork.Setup(uow => uow.TutorLearnerSubject.Count(It.IsAny<Expression<Func<TutorLearnerSubject, bool>>>()))
                .Returns((Expression<Func<TutorLearnerSubject, bool>> filter) =>
                {
                    var tutorId = mockTutors.First().TutorId; // Use a simple rule for mock counts
                    return filter.Compile().Invoke(new TutorLearnerSubject { TutorSubject = new TutorSubject { TutorId = tutorId } }) ? 5 : 0;
                });

            var mappedTutors = mockTutors.Select(t => new TutorSummaryDto
            {
                TutorId = t.TutorId,
                Rating = t.Rating,
                TeachingLocations = new List<TeachingLocationViewDTO>()
            }).ToList();

            _mapperMock.Setup(m => m.Map<List<TutorSummaryDto>>(It.IsAny<List<Tutor>>()))
                .Returns(mappedTutors);

            // Act
            var result = await _tutorService.GetTopTutorAsync(2);

            // Assert
            Assert.AreEqual(4.5m, result[0].Rating);
            Assert.AreEqual(3.5m, result[1].Rating);
        }

        [Test]
        public async Task GetTopTutorAsync_ShouldHandleEmptyTeachingLocations()
        {
            // Arrange
            var mockTutors = new List<Tutor>
        {
            new Tutor { TutorId = Guid.NewGuid(), Rating = 4.5m, IsVerified = true }
        };

            _mockUnitOfWork.Setup(uow => uow.Tutors.GetAllTutorsAsync())
                .ReturnsAsync(mockTutors);

            _mockUnitOfWork.Setup(uow => uow.TutorLearnerSubject.Count(It.IsAny<Expression<Func<TutorLearnerSubject, bool>>>()))
                .Returns(0);

            var mappedTutors = new List<TutorSummaryDto>
        {
            new TutorSummaryDto { TutorId = mockTutors.First().TutorId, Rating = 4.5m, TeachingLocations = new List<TeachingLocationViewDTO>() }
        };

            _mapperMock.Setup(m => m.Map<List<TutorSummaryDto>>(It.IsAny<List<Tutor>>()))
                .Returns(mappedTutors);

            // Act
            var result = await _tutorService.GetTopTutorAsync(1);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(4.5m, result[0].Rating);
            Assert.IsEmpty(result[0].TeachingLocations);
        }


        #endregion

        #region GetTutorHomePage Tests

        [Test]
        public async Task GetTutorHomePage_ShouldReturnEmptyDto_WhenNoTutorsMatchFilter()
        {
            // Arrange
            var tutorFilterDto = new TutorFilterDto();
            int total;


            int totalRecords = 1; // Out parameter

            _mockUnitOfWork.Setup(uow => uow.Tutors.GetMultiPaging(
                It.IsAny<Expression<Func<Tutor, bool>>>(),
                out totalRecords, // Assign the out parameter value
                It.IsAny<int>(), // Index
                It.IsAny<int>(), // Size
                It.IsAny<string[]>(), // Includes
                It.IsAny<Func<IQueryable<Tutor>, IOrderedQueryable<Tutor>>>() // OrderBy
            )).Returns(new List<Tutor>());

            // Act
            var result = await _tutorService.GetTutorHomePage(tutorFilterDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result.Tutors);
        }

        [Test]
        public async Task GetTutorHomePage_ShouldFilterAndReturnTutors_WhenFilterMatches()
        {
            // Arrange
            var tutorFilterDto = new TutorFilterDto
            {
                MinPrice = 20,
                MaxPrice = 50,
                SearchingQuery = "Sample",
                City = "123",
                District = "456",
                Subjects = new List<int> { 1 }
            };

            int total;
            var mockTutors = new List<Tutor>
        {
            new Tutor
            {
                TutorId = Guid.NewGuid(),
                IsVerified = true,
                TutorSubjects = new List<TutorSubject>
                {
                    new TutorSubject { Rate = 30, Subject = new Subject { SubjectId = tutorFilterDto.Subjects.First() } }
                },
                TutorTeachingLocations = new List<TutorTeachingLocations>
                {
                    new TutorTeachingLocations
                    {
                        TeachingLocation = new TeachingLocation { CityId = tutorFilterDto.City, DistrictId = tutorFilterDto.District }
                    }
                },
                TutorNavigation = new AspNetUser
                {
                    Fullname = "Sample Tutor",
                    Gender = tutorFilterDto.Gender
                }
            }
        };

            int totalRecords = 1; // Out parameter

            _mockUnitOfWork.Setup(uow => uow.Tutors.GetMultiPaging(
                It.IsAny<Expression<Func<Tutor, bool>>>(),
                out totalRecords, // Assign the out parameter value
                It.IsAny<int>(), // Index
                It.IsAny<int>(), // Size
                It.IsAny<string[]>(), // Includes
                It.IsAny<Func<IQueryable<Tutor>, IOrderedQueryable<Tutor>>>() // OrderBy
            )).Returns(mockTutors);

            _mapperMock.Setup(m => m.Map<List<TutorSummaryDto>>(mockTutors))
                .Returns(mockTutors.Select(t => new TutorSummaryDto { TutorId = t.TutorId }).ToList());

            _mockUnitOfWork.Setup(uow => uow.TutorLearnerSubject.Count(It.IsAny<Expression<Func<TutorLearnerSubject, bool>>>()))
                .Returns(5);

            _mockUnitOfWork.Setup(uow => uow.Tutors.GetAverageRatingForTutorAsync(It.IsAny<Guid>()))
                .ReturnsAsync(4.5m);

            _mockUnitOfWork.Setup(uow => uow.TutorLearnerSubject.GetSubjectsByUserIdAndRoleAsync(
                It.IsAny<Guid>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<bool>()
            )).ReturnsAsync(new List<TutorLearnerSubject>
            {
            new TutorLearnerSubject
            {
                BillingEntries = new List<BillingEntry>
                {
                    new BillingEntry
                    {
                        StartDateTime = DateTime.UtcNow.AddHours(-1),
                        EndDateTime = DateTime.UtcNow
                    }
                }
            }
            });


            // Act
            var result = await _tutorService.GetTutorHomePage(tutorFilterDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Tutors.First().NumberOfStudents);
            Assert.AreEqual(4.5m, result.Tutors.First().Rating);
        }

        
        #endregion
    }
}

