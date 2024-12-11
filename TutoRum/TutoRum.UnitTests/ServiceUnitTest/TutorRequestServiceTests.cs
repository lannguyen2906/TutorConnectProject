using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Enum;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.IRepositories;
using TutoRum.Data.Models;
using TutoRum.Services.Helper;
using TutoRum.Services.IService;
using TutoRum.Services.Service;
using TutoRum.Services.ViewModels;

namespace TutoRum.UnitTests.ServiceUnitTest
{
    [TestFixture]
    public class TutorRequestServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<UserManager<AspNetUser>> _mockUserManager;
        private Mock<IEmailService> _mockEmailService;
        private Mock<INotificationService> _mockNotificationService;
        private Mock<ISubjectService> _mockSubjectService;
        private Mock<APIAddress> _mockApiAddress;
        private TutorRequestService _tutorRequestService;
        private Mock<IAccountRepository> _mockAccountRepository;
        private Guid tutorId;
        [SetUp]
        public void SetUp()
        {
            // Mocking dependencies
            tutorId = Guid.NewGuid();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUserManager = new Mock<UserManager<AspNetUser>>(
                new Mock<IUserStore<AspNetUser>>().Object,
                null, null, null, null, null, null, null, null
            );
            _mockEmailService = new Mock<IEmailService>();
            _mockNotificationService = new Mock<INotificationService>();
            _mockSubjectService = new Mock<ISubjectService>();
            _mockApiAddress = new Mock<APIAddress>();
            _mockAccountRepository = new Mock<IAccountRepository>();
            _mockUnitOfWork.Setup(st => st.TutorRequest.Add(It.IsAny<TutorRequest>()));
            _mockUnitOfWork.Setup(st => st.TutorRequest.Update(It.IsAny<TutorRequest>()));
            _mockUnitOfWork.Setup(u => u.TutorRequest.DeleteMulti(It.IsAny<Expression<Func<TutorRequest, bool>>>()));
            _mockUnitOfWork.Setup(uow => uow.Accounts).Returns(_mockAccountRepository.Object); // Use the correct property


            var user = new AspNetUser
            {
                Id = tutorId,
                Fullname = "Test User"
            };
            var TutorRequestTutor = new TutorRequestTutor
            {
                TutorId = tutorId,
                TutorRequestId = 1,
                Ischoose = true
            };
            var qualification = new QualificationLevel { Id = 4, Level = "Bachelor's Degree" };

            _mockUnitOfWork.Setup(uow => uow.Accounts.GetMultiAsQueryable(It.IsAny<Expression<Func<AspNetUser, bool>>>(), It.IsAny<string[]>()))
                .Returns(new List<AspNetUser> { user }.AsQueryable());
            _mockUnitOfWork.Setup(uow => uow.TutorRequestTutor.GetMultiAsQueryable(It.IsAny<Expression<Func<TutorRequestTutor, bool>>>(), It.IsAny<string[]>()))
                .Returns(new List<TutorRequestTutor> { TutorRequestTutor }.AsQueryable());
            

            var tutorRequests = new List<TutorRequest>
    {
        new TutorRequest
        {
            Id = 1,
            CityId = "1",
            DistrictId = "1",
            WardId = "1",
            RequestSummary = "Math Request",
            Fee = 200,
            TutorQualificationId = 4,
            TeachingLocation = "Location A",
            AspNetUserId = Guid.NewGuid(),
            TutorQualification = new QualificationLevel { Id = 4, Level = "Bachelor's Degree" }
        }
    };

            int totalRecords = 1; // Out parameter

            _mockUnitOfWork.Setup(uow => uow.TutorRequest.GetMultiPaging(
                It.IsAny<Expression<Func<TutorRequest, bool>>>(),
                out totalRecords, // Assign the out parameter value
                It.IsAny<int>(), // Index
                It.IsAny<int>(), // Size
                It.IsAny<string[]>(), // Includes
                It.IsAny<Func<IQueryable<TutorRequest>, IOrderedQueryable<TutorRequest>>>() // OrderBy
            )).Returns(tutorRequests);



            // Creating the service instance
            _tutorRequestService = new TutorRequestService(
                _mockUnitOfWork.Object,
                _mockUserManager.Object,
                _mockEmailService.Object,
                new APIAddress(new HttpClient()),
                _mockNotificationService.Object,
                _mockSubjectService.Object
            );
        }

        [Test]
        public async Task CreateTutorRequestAsync_ShouldCreateNewTutorRequest_WithNewSubject()
        {
            // Arrange
            var tutorRequestDto = new TutorRequestDTO
            {
                Subject = "Math",
                PhoneNumber = "123456789",
                RequestSummary = "Math tutor request",
                CityId = "1",
                DistrictId = "1",
                WardId = "1",
                TeachingLocation = "HCMC",
                NumberOfStudents = 1,
                StartDate = DateTime.UtcNow.AddDays(1),
                PreferredScheduleType = "Weekdays",
                StudentGender = "Male",
                TutorGender = "Female",
                Fee = 200000,
                SessionsPerWeek = 2,
                DetailedDescription = "Detailed description",
                TutorQualificationId = 1,
                FreeSchedules = "2 3 4",
                rateRangeId = 1
            };

            var currentUser = new AspNetUser { Id = Guid.NewGuid() };

            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);

            var existingSubjects = new List<Subject>();
            _mockUnitOfWork.Setup(uow => uow.Subjects.GetAll(It.IsAny<string[]>())).Returns(existingSubjects.AsQueryable());

            _mockUnitOfWork.Setup(uow => uow.Subjects.Add(It.IsAny<Subject>())).Verifiable();
            _mockUnitOfWork.Setup(uow => uow.CommitAsync()).Verifiable();

            // Act
            var result = await _tutorRequestService.CreateTutorRequestAsync(tutorRequestDto, new ClaimsPrincipal());

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task CreateTutorRequestAsync_ShouldCreateTutorRequest_WithExistingSubject()
        {
            // Arrange
            var tutorRequestDto = new TutorRequestDTO
            {
                Subject = "Math",
                PhoneNumber = "123456789",
                RequestSummary = "Math tutor request",
                CityId = "1",
                DistrictId = "1",
                WardId = "1",
                TeachingLocation = "HCMC",
                NumberOfStudents = 1,
                StartDate = DateTime.UtcNow.AddDays(1),
                PreferredScheduleType = "Weekdays",
                StudentGender = "Male",
                TutorGender = "Female",
                Fee = 200000,
                SessionsPerWeek = 2,
                DetailedDescription = "Detailed description",
                TutorQualificationId = 1,
                FreeSchedules = "2 3 4",
                rateRangeId = 1
            };

            var currentUser = new AspNetUser { Id = Guid.NewGuid() };

            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);

            var existingSubjects = new List<Subject>
        {
            new Subject { SubjectName = "Math", SubjectId = 1 }
        };

            _mockUnitOfWork.Setup(uow => uow.Subjects.GetAll(It.IsAny<string[]>())).Returns(existingSubjects.AsQueryable());

            _mockUnitOfWork.Setup(uow => uow.Subjects.Add(It.IsAny<Subject>())).Verifiable();
            _mockUnitOfWork.Setup(uow => uow.CommitAsync()).Verifiable();

            // Act
            var result = await _tutorRequestService.CreateTutorRequestAsync(tutorRequestDto, new ClaimsPrincipal());

            // Assert
            _mockUnitOfWork.Verify(uow => uow.Subjects.Add(It.IsAny<Subject>()), Times.Never); // No new subject should be added
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once); // Commit should still be called
        }

        [Test]
        public async Task CreateTutorRequestAsync_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            var tutorRequestDto = new TutorRequestDTO
            {
                Subject = "Math",
                PhoneNumber = "123456789",
                RequestSummary = "Math tutor request",
                CityId = "1",
                DistrictId = "1",
                WardId = "1",
                TeachingLocation = "HCMC",
                NumberOfStudents = 1,
                StartDate = DateTime.UtcNow.AddDays(1),
                PreferredScheduleType = "Weekdays",
                StudentGender = "Male",
                TutorGender = "Female",
                Fee = 200000,
                SessionsPerWeek = 2,
                DetailedDescription = "Detailed description",
                TutorQualificationId = 1,
                FreeSchedules = "2 3 4",
                rateRangeId = 1
            };

            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((AspNetUser)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _tutorRequestService.CreateTutorRequestAsync(tutorRequestDto, new ClaimsPrincipal())
            );

            Assert.AreEqual(ex.Message, "User not found.");
        }


        [Test]
        public async Task GetTutorRequestByIdAsync_ShouldReturnTutorRequestDTO_WhenValidIdProvided()
        {
            // Arrange
            var tutorRequestId = 1;
            var tutorRequest = new TutorRequest
            {
                Id = tutorRequestId,
                AspNetUserId = Guid.NewGuid(),
                CityId = "1",
                DistrictId = "1",
                WardId = "1",
                TutorQualificationId = 4,
                TeachingLocation = "Test Location",
                PhoneNumber = "123456789",
                RequestSummary = "Request Summary",
                NumberOfStudents = 2,
                StartDate = DateTime.Now,
                PreferredScheduleType = "Weekly",
                Subject = "Math",
                StudentGender = "Male",
                TutorGender = "Female",
                Fee = 200,
                SessionsPerWeek = 3,
                DetailedDescription = "Detailed Description",
                Status = "Pending",
                FreeSchedules = "Free Schedule",
                RateRangeId = 5
            };

            var user = new AspNetUser
            {
                Id = Guid.NewGuid(),
                Fullname = "Test User"
            };

            var qualification = new QualificationLevel { Id = 4, Level = "Bachelor's Degree" };

            _mockUnitOfWork.Setup(uow => uow.TutorRequest.GetSingleById(tutorRequestId))
                .Returns(tutorRequest);

            _mockUnitOfWork.Setup(uow => uow.Accounts.GetMultiAsQueryable(It.IsAny<Expression<Func<AspNetUser, bool>>>(), It.IsAny<string[]>()))
                .Returns(new List<AspNetUser> { user }.AsQueryable());

            _mockUnitOfWork.Setup(uow => uow.QualificationLevel.FindAsync(It.IsAny<Expression<Func<QualificationLevel, bool>>>()))
                .ReturnsAsync(qualification);

            // Act
            var result = await _tutorRequestService.GetTutorRequestByIdAsync(tutorRequestId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(tutorRequestId, result.Id);
            Assert.AreEqual("Test User", result.LearnerName);
            Assert.AreEqual("Bachelor's Degree", result.TutorQualificationName);
        }

        [Test]
        public async Task GetAllTutorRequestsAsync_ShouldReturnPagedResult_WhenFilterIsValid()
        {
            // Arrange
            var filter = new TutorRequestHomepageFilterDto
            {
                CityId = "1",
                DistrictId = "2",
                Search = "Math",
                MinFee = 100,
                MaxFee = 500
            };

            var tutorRequests = new List<TutorRequest>
    {
        new TutorRequest
        {
            Id = 1,
            CityId = "1",
            DistrictId = "1",
            WardId = "1",
            RequestSummary = "Math Request",
            Fee = 200,
            TutorQualificationId = 4,
            TeachingLocation = "Location A",
            AspNetUserId = Guid.NewGuid(),
            TutorQualification = new QualificationLevel { Id = 4, Level = "Bachelor's Degree" }
        }
    };

            int totalRecords = 1; // Out parameter

            _mockUnitOfWork.Setup(uow => uow.TutorRequest.GetMultiPaging(
                It.IsAny<Expression<Func<TutorRequest, bool>>>(),
                out totalRecords, // Assign the out parameter value
                It.IsAny<int>(), // Index
                It.IsAny<int>(), // Size
                It.IsAny<string[]>(), // Includes
                It.IsAny<Func<IQueryable<TutorRequest>, IOrderedQueryable<TutorRequest>>>() // OrderBy
            )).Returns(tutorRequests);


            var qualification = new QualificationLevel { Id = 4, Level = "Bachelor's Degree" };
            _mockUnitOfWork.Setup(uow => uow.QualificationLevel.FindAsync(It.IsAny<Expression<Func<QualificationLevel, bool>>>()))
                .ReturnsAsync(qualification); // Use ReturnsAsync for asynchronous methods

            // Act
            var result = await _tutorRequestService.GetAllTutorRequestsAsync(filter);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Items.Count);
            Assert.AreEqual(1, result.Items.First().Id);
            Assert.AreEqual("Math Request", result.Items.First().RequestSummary);
        }

        [Test]
        public void UpdateTutorRequestAsync_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((AspNetUser)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _tutorRequestService.UpdateTutorRequestAsync(1, new TutorRequestDTO(), new ClaimsPrincipal())
            );
            Assert.AreEqual("User not found.", ex.Message);
        }

        [Test]
        public void UpdateTutorRequestAsync_ShouldThrowException_WhenTutorRequestNotFound()
        {
            // Arrange
            var user = new AspNetUser { Id = Guid.NewGuid() };
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _mockUnitOfWork.Setup(x => x.TutorRequest.GetSingleById(It.IsAny<int>())).Returns((TutorRequest)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _tutorRequestService.UpdateTutorRequestAsync(1, new TutorRequestDTO(), new ClaimsPrincipal())
            );
            Assert.AreEqual("Tutor request not found.", ex.Message);
        }

        [Test]
        public async Task UpdateTutorRequestAsync_ShouldUpdateTutorRequest_WhenValid()
        {
            // Arrange
            var user = new AspNetUser { Id = Guid.NewGuid() };
            var tutorRequest = new TutorRequest();
            var dto = new TutorRequestDTO { Fee = 500 };

            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _mockUnitOfWork.Setup(x => x.TutorRequest.GetSingleById(It.IsAny<int>())).Returns(tutorRequest);

            // Act
            var result = await _tutorRequestService.UpdateTutorRequestAsync(1, dto, new ClaimsPrincipal());

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(500, tutorRequest.Fee);
            _mockUnitOfWork.Verify(x => x.CommitAsync(), Times.Once);
        }

        [Test]
        public void ChooseTutorForTutorRequestAsync_ShouldThrowException_WhenTutorNotFound()
        {
            // Arrange
            var user = new AspNetUser { Id = Guid.NewGuid() };
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _mockUnitOfWork.Setup(x => x.TutorRequest.GetSingleById(It.IsAny<int>())).Returns(new TutorRequest());
            _mockUnitOfWork.Setup(x => x.TutorRequestTutor.GetSingleByCondition(It.IsAny<Expression<Func<TutorRequestTutor, bool>>>(), It.IsAny<string[]>()))
                .Returns((TutorRequestTutor)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _tutorRequestService.ChooseTutorForTutorRequestAsync(1, Guid.NewGuid(), new ClaimsPrincipal())
            );
            Assert.AreEqual("Tutor not found for this tutor request.", ex.Message);
        }

        [Test]
        public async Task ChooseTutorForTutorRequestAsync_ShouldChooseTutor_WhenValid()
        {
            // Arrange
            var user = new AspNetUser { Id = Guid.NewGuid() };
            var tutorRequest = new TutorRequest();
            var tutorRequestTutor = new TutorRequestTutor();

            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _mockUnitOfWork.Setup(x => x.TutorRequest.GetSingleById(It.IsAny<int>())).Returns(tutorRequest);
            _mockUnitOfWork.Setup(x => x.TutorRequestTutor.GetSingleByCondition(It.IsAny<Expression<Func<TutorRequestTutor, bool>>>(), It.IsAny<string[]>()))
    .Returns(tutorRequestTutor);

            // Act
            var result = await _tutorRequestService.ChooseTutorForTutorRequestAsync(1, Guid.NewGuid(), new ClaimsPrincipal());

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(tutorRequestTutor.Ischoose);
            _mockUnitOfWork.Verify(x => x.CommitAsync(), Times.Once);
        }

        [Test]
        public void AddTutorToRequestAsync_ShouldThrowException_WhenTutorRequestNotFound()
        {
            // Arrange
            _mockUnitOfWork.Setup(x => x.TutorRequest.GetSingleById(It.IsAny<int>())).Returns((TutorRequest)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _tutorRequestService.AddTutorToRequestAsync(1, Guid.NewGuid())
            );
            Assert.AreEqual("Tutor request not found.", ex.Message);
        }

        [Test]
        public void AddTutorToRequestAsync_ShouldThrowException_WhenUserRegistersOwnRequest()
        {
            // Arrange
            var tutorID = Guid.NewGuid();
            var tutorRequest = new TutorRequest { AspNetUserId = tutorID };
            _mockUnitOfWork.Setup(x => x.TutorRequest.GetSingleById(It.IsAny<int>())).Returns(tutorRequest);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _tutorRequestService.AddTutorToRequestAsync(1, tutorID)
            );
            Assert.AreEqual("You cannot register your tutor request", ex.Message);
        }

        [Test]
        public void AddTutorToRequestAsync_ShouldThrowException_WhenTutorNotFound()
        {
            // Arrange
            _mockUnitOfWork.Setup(x => x.TutorRequest.GetSingleById(It.IsAny<int>())).Returns(new TutorRequest());
            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((AspNetUser)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _tutorRequestService.AddTutorToRequestAsync(1, Guid.NewGuid())
            );
            Assert.AreEqual("Tutor not found.", ex.Message);
        }

        [Test]
        public void AddTutorToRequestAsync_ShouldThrowException_WhenUserNotTutor()
        {
            // Arrange
            var tutor = new AspNetUser();
            _mockUnitOfWork.Setup(x => x.TutorRequest.GetSingleById(It.IsAny<int>())).Returns(new TutorRequest());
            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(tutor);
            _mockUserManager.Setup(x => x.IsInRoleAsync(tutor, AccountRoles.Tutor)).ReturnsAsync(false);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _tutorRequestService.AddTutorToRequestAsync(1, Guid.NewGuid())
            );
            Assert.AreEqual("User does not have Tutor role", ex.Message);
        }

        [Test]
        public async Task AddTutorToRequestAsync_ShouldAddTutorToRequest_WhenValid()
        {
            // Arrange
            var tutorRequest = new TutorRequest { AspNetUserId = Guid.NewGuid() };
            var tutor = new AspNetUser();
            _mockUnitOfWork.Setup(x => x.TutorRequest.GetSingleById(It.IsAny<int>())).Returns(tutorRequest);
            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(tutor);
            _mockUserManager.Setup(x => x.IsInRoleAsync(tutor, AccountRoles.Tutor)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(x => x.TutorRequestTutor.GetSingleByCondition(It.IsAny<Expression<Func<TutorRequestTutor, bool>>>(), It.IsAny<string[]>()))
                .Returns((TutorRequestTutor)null);

            // Act
            var result = await _tutorRequestService.AddTutorToRequestAsync(1, Guid.NewGuid());

            // Assert
            Assert.IsTrue(result);
            _mockUnitOfWork.Verify(x => x.CommitAsync(), Times.Once);
            _mockNotificationService.Verify(x => x.SendNotificationAsync(It.IsAny<NotificationRequestDto>(), false), Times.Once);
        }

        [Test]
        public void GetListTutorsByTutorRequestAsync_ShouldThrowException_WhenTutorRequestNotFound()
        {
            // Arrange
            _mockUnitOfWork.Setup(x => x.TutorRequest.GetSingleById(It.IsAny<int>())).Returns((TutorRequest)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _tutorRequestService.GetListTutorsByTutorRequestAsync(1)
            );
            Assert.AreEqual("Tutor request not found.", ex.Message);
        }

        [Test]
        public void GetListTutorsByTutorRequestAsync_ShouldThrowException_WhenRequestNotVerified()
        {
            // Arrange
            var tutorRequest = new TutorRequest { IsVerified = false };
            _mockUnitOfWork.Setup(x => x.TutorRequest.GetSingleById(It.IsAny<int>())).Returns(tutorRequest);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _tutorRequestService.GetListTutorsByTutorRequestAsync(1)
            );
            Assert.AreEqual("The tutor request is not verified.", ex.Message);
        }

        [Test]
        public async Task GetListTutorsByTutorRequestAsync_ShouldReturnTutors_WhenValid()
        {
            // Arrange
            var tutorRequest = new TutorRequest { Id = 1, IsVerified = true, Subject = "Math", StartDate = DateTime.UtcNow };
            var tutorRequestTutors = new List<TutorRequestTutor>
        {
            new TutorRequestTutor { TutorId = Guid.NewGuid(), IsVerified = true },
            new TutorRequestTutor { TutorId = Guid.NewGuid(), IsVerified = false }
        };
            var tutors = tutorRequestTutors.Select(tr => new Tutor
            {
                TutorId = tr.TutorId,
                Specialization = "Specialization",
                TutorNavigation = new AspNetUser { Fullname = "TutorName", Email = "tutor@example.com" }
            }).ToList();

            _mockUnitOfWork.Setup(x => x.TutorRequest.GetSingleById(It.IsAny<int>())).Returns(tutorRequest);
            _mockUnitOfWork.Setup(x => x.TutorRequestTutor.GetMultiAsQueryable(It.IsAny<Expression<Func<TutorRequestTutor, bool>>>(), It.IsAny<string[]>()))
                .Returns(tutorRequestTutors.AsQueryable());
            _mockUnitOfWork.Setup(x => x.Tutors.GetMultiAsQueryable(It.IsAny<Expression<Func<Tutor, bool>>>(), It.IsAny<string[]>()))
                .Returns(tutors.AsQueryable());

            // Act
            var result = await _tutorRequestService.GetListTutorsByTutorRequestAsync(1);

            // Assert
            Assert.AreEqual(2, result.Tutors.Count);
            Assert.AreEqual("Math", result.Subject);
        }


        [Test]
        public async Task GetTutorRequestsAdmin_ShouldReturnPagedResult_WhenValidData()
        {
            // Arrange
            var filter = new TutorRequestFilterDto
            {
                Search = "Math",
                IsVerified = true,
                StartDate = DateTime.UtcNow,
                Subject = "Math"
            };
            var user = new ClaimsPrincipal() { };
            var pageIndex = 0;
            var pageSize = 20;
            var fakeTutorRequests = new List<TutorRequest>
            {
                new TutorRequest
                {
                    Id = 1,
                    RequestSummary = "Math tutor",
                    DetailedDescription = "Math request",
                    IsVerified = true,
                    StartDate = DateTime.UtcNow,
                    Subject = "Math",
                    Fee = 20
                }
            };

            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new AspNetUser() { Id = tutorId });
            _mockUserManager.Setup(x => x.IsInRoleAsync(It.IsAny<AspNetUser>(), It.IsAny<string>())).ReturnsAsync(true);


            // Act
            var result = await _tutorRequestService.GetTutorRequestsAdmin(filter, user, pageIndex, pageSize);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.TotalRecords);
            Assert.AreEqual(1, result.Items.Count);
        }

        [Test]
        public async Task GetTutorRequestsAdmin_ShouldThrowException_WhenNoTutorRequestsFound()
        {
            // Arrange
            var filter = new TutorRequestFilterDto();
            var user = new ClaimsPrincipal();
            var pageIndex = 0;
            var pageSize = 20;
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new AspNetUser());

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _tutorRequestService.GetTutorRequestsAdmin(filter, user, pageIndex, pageSize));
        }

        [Test]
        public async Task GetTutorRequestsByLearnerIdAsync_ShouldReturnPagedResult_WhenValidData()
        {
            // Arrange
            var learnerId = Guid.NewGuid();
            var pageIndex = 0;
            var pageSize = 20;
            var fakeTutorRequests = new List<TutorRequest>
            {
                new TutorRequest
                {
                    Id = 1,
                    Subject = "Math",
                    Fee = 20,
                    StartDate = DateTime.UtcNow,
                    Status = "Pending",
                    IsVerified = true
                }
            };

            // Act
            var result = await _tutorRequestService.GetTutorRequestsByLearnerIdAsync(learnerId, pageIndex, pageSize);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.TotalRecords);
            Assert.AreEqual(1, result.Items.Count);
        }

       

        [Test]
        public async Task GetTutorRequestsByTutorIdAsync_ShouldThrowException_WhenNoTutorRequestsFound()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var pageIndex = 0;
            var pageSize = 20;

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _tutorRequestService.GetTutorRequestsByTutorIdAsync(tutorId, pageIndex, pageSize));
            Assert.That(ex.Message, Is.EqualTo("No tutor requests found for the given learner."));
        }


        
        [Test]
        public void SendTutorRequestEmailAsync_ShouldThrowException_WhenTutorRequestNotFound()
        {
            // Arrange
            var tutorRequestId = 1;
            var tutorID = Guid.NewGuid();

            _mockUnitOfWork.Setup(x => x.TutorRequestTutor.GetMultiAsQueryable(It.IsAny<Expression<Func<TutorRequestTutor, bool>>>(), It.IsAny<string[]>()))
                .Returns(Enumerable.Empty<TutorRequestTutor>().AsQueryable());

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _tutorRequestService.SendTutorRequestEmailAsync(tutorRequestId, tutorID));
            Assert.That(ex.Message, Is.EqualTo("Tutor request not found."));
        }

        [Test]
        public void SendTutorRequestEmailAsync_ShouldThrowException_WhenTutorNotFoundForRequest()
        {
            // Arrange
            var tutorRequestId = 1;
            var tutorID = Guid.NewGuid();
            var tutorRequest = new TutorRequestDTO
            {
                Id = tutorRequestId,
                LearnerName = "John Doe",
                Subject = "Math"
            };

            var tutorRequestTutors = new TutorRequestTutor
            {
                TutorRequestId = tutorRequestId,
                TutorId = tutorID,
                IsVerified = false
            };

            // Mock data
            _mockUnitOfWork.Setup(x => x.TutorRequestTutor.GetMultiAsQueryable(It.IsAny<Expression<Func<TutorRequestTutor, bool>>>(), It.IsAny<string[]>()))
                .Returns(new List<TutorRequestTutor> { tutorRequestTutors }.AsQueryable());

            _mockUnitOfWork.Setup(x => x.TutorRequestTutor.Update(It.IsAny<TutorRequestTutor>()));
            _mockUnitOfWork.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);

            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((AspNetUser)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _tutorRequestService.SendTutorRequestEmailAsync(tutorRequestId, tutorID));
            
        }

       



    }



}
