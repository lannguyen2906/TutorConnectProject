using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.Models;
using TutoRum.Services.Service;
using TutoRum.Services.ViewModels;

namespace TutoRum.UnitTests.ServiceUnitTest
{
    [TestFixture]
    public class SubjectServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<UserManager<AspNetUser>> _mockUserManager;
        private SubjectService _subjectService;

        [SetUp]
        public void SetUp()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUserManager = new Mock<UserManager<AspNetUser>>(
                new Mock<IUserStore<AspNetUser>>().Object,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
            );

            _subjectService = new SubjectService(_mockUnitOfWork.Object, _mockUserManager.Object);
        }

        [Test]
        public async Task GetAllSubjectAsync_ShouldReturnSubjects_WhenSubjectsExist()
        {
            // Arrange
            var mockSubjects = new List<Subject>
        {
            new Subject { SubjectId = 1, SubjectName = "Math" },
            new Subject { SubjectId =2, SubjectName = "English" }
        };

            var mockTutorSubjects = new List<TutorSubject>
        {
            new TutorSubject { Tutor = new Tutor { IsVerified = true }, Subject = mockSubjects[0] },
            new TutorSubject { Tutor = new Tutor { IsVerified = true }, Subject = mockSubjects[1] }
        };

            _mockUnitOfWork.Setup(u => u.Subjects.GetAll(It.IsAny<string[]>())).Returns(mockSubjects);
            _mockUnitOfWork.Setup(u => u.TutorSubjects.GetAll(It.IsAny<string[]>())).Returns(mockTutorSubjects);

            // Act
            var result = await _subjectService.GetAllSubjectAsync();

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Math", result.First().SubjectName);
        }

        [Test]
        public void GetAllSubjectAsync_ShouldThrowException_WhenNoSubjectsExist()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.Subjects.GetAll(It.IsAny<string[]>())).Returns(new List<Subject>());

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _subjectService.GetAllSubjectAsync());
        }

        [Test]
        public async Task AddSubjectsWithRateAsync_ShouldAddNewSubjects_WhenSubjectDoesNotExist()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var subjectDto = new List<AddSubjectDTO>
        {
            new AddSubjectDTO
            {
                SubjectName = "Physics",
                Rate = 200,
                Description = "Physics Subject",
                SubjectType = "Science",
                RateRangeId = 1
            }
        };

            var mockSubjects = new List<Subject>(); // No subjects exist
            _mockUnitOfWork.Setup(u => u.Subjects.GetAll(It.IsAny<string[]>())).Returns(mockSubjects);

            // Mock adding the new subject and tutor subject
            _mockUnitOfWork.Setup(u => u.Subjects.Add(It.IsAny<Subject>()));
            _mockUnitOfWork.Setup(u => u.TutorSubjects.Add(It.IsAny<TutorSubject>()));
            _mockUnitOfWork.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

            // Act
            await _subjectService.AddSubjectsWithRateAsync(subjectDto, tutorId);

            // Assert
            _mockUnitOfWork.Verify(u => u.Subjects.Add(It.IsAny<Subject>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.TutorSubjects.Add(It.IsAny<TutorSubject>()), Times.Once);
        }

        [Test]
        public async Task AddSubjectsWithRateAsync_ShouldNotAddSubject_WhenSubjectExists()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var existingSubject = new Subject { SubjectId = 1, SubjectName = "Math" };
            var subjectDto = new List<AddSubjectDTO>
        {
            new AddSubjectDTO
            {
                SubjectName = "Math",
                Rate = 200,
                Description = "Math Subject",
                SubjectType = "Science",
                RateRangeId = 1
            }
        };

            var mockSubjects = new List<Subject> { existingSubject }; // Subject exists
            _mockUnitOfWork.Setup(u => u.Subjects.GetAll(It.IsAny<string[]>())).Returns(mockSubjects);
            _mockUnitOfWork.Setup(u => u.TutorSubjects.Add(It.IsAny<TutorSubject>()));
            _mockUnitOfWork.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

            // Act
            await _subjectService.AddSubjectsWithRateAsync(subjectDto, tutorId);

            // Assert
            _mockUnitOfWork.Verify(u => u.Subjects.Add(It.IsAny<Subject>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.TutorSubjects.Add(It.IsAny<TutorSubject>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Test]
        public async Task DeleteTutorSubjectAsync_ShouldDeleteTutorSubject_WhenSubjectExists()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var subjectId = 1;
            var subject = new Subject { SubjectId = subjectId, SubjectName = "Math" };

            // Mock các phương thức của UnitOfWork
            _mockUnitOfWork.Setup(u => u.Subjects.GetSingleById(subjectId)).Returns(subject);
            _mockUnitOfWork.Setup(u => u.TutorSubjects.Delete(subjectId));
            _mockUnitOfWork.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

            // Act
            await _subjectService.DeleteTutorSubjectAsync(tutorId, subjectId);

            // Assert
            _mockUnitOfWork.Verify(u => u.TutorSubjects.Delete(subjectId), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Test]
        public void DeleteTutorSubjectAsync_ShouldThrowException_WhenSubjectNotFound()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var subjectId = 1;

            // Mock phương thức GetSingleById trả về null
            _mockUnitOfWork.Setup(u => u.Subjects.GetSingleById(subjectId)).Returns((Subject)null);

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await _subjectService.DeleteTutorSubjectAsync(tutorId, subjectId));
        }

        [Test]
        public async Task UpdateSubjectsAsync_ShouldUpdateSubject_WhenSubjectExists()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var subjectDto = new List<UpdateSubjectDTO>
        {
            new UpdateSubjectDTO
            {
                SubjectId = 1,
                SubjectName = "Physics",
                Rate = 200,
                RateRangeId = 1,
                Description = "Physics subject description"
            }
        };
            var existingSubject = new Subject { SubjectId = 1, SubjectName = "Math" };
            var tutorSubject = new TutorSubject { TutorId = tutorId, SubjectId = 1 };

            _mockUnitOfWork.Setup(u => u.Subjects.FindAsync(It.IsAny<Expression<Func<Subject, bool>>>()))
                .ReturnsAsync(existingSubject);
            _mockUnitOfWork.Setup(u => u.TutorSubjects.FindAsync(It.IsAny<Expression<Func<TutorSubject, bool>>>()))
                .ReturnsAsync(tutorSubject);
            _mockUnitOfWork.Setup(u => u.Subjects.Update(It.IsAny<Subject>()));
            _mockUnitOfWork.Setup(u => u.TutorSubjects.Update(It.IsAny<TutorSubject>()));
            _mockUnitOfWork.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

            // Act
            await _subjectService.UpdateSubjectsAsync(subjectDto, tutorId);

            // Assert
            _mockUnitOfWork.Verify(u => u.Subjects.Update(It.IsAny<Subject>()), Times.Once);
        }

        [Test]
        public void UpdateSubjectsAsync_ShouldThrowException_WhenSubjectNotFound()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var subjectDto = new List<UpdateSubjectDTO>
        {
            new UpdateSubjectDTO
            {
                SubjectId = 999, // SubjectId không tồn tại
                SubjectName = "Biology"
            }
        };

            _mockUnitOfWork.Setup(u => u.Subjects.FindAsync(It.IsAny<Expression<Func<Subject, bool>>>()))
                .ReturnsAsync((Subject)null);

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _subjectService.UpdateSubjectsAsync(subjectDto, tutorId));
        }

        [Test]
        public async Task UpdateSubjectsAsync_ShouldUpdateRate_WhenRateIsProvided()
        {
            // Arrange
            var tutorId = Guid.NewGuid();
            var subjectDto = new List<UpdateSubjectDTO>
        {
            new UpdateSubjectDTO
            {
                SubjectId = 1,
                Rate = 250,
                RateRangeId = 2
            }
        };

            var existingSubject = new Subject { SubjectId = 1, SubjectName = "Math" };
            var tutorSubject = new TutorSubject { TutorId = tutorId, SubjectId = 1, Rate = 200 };

            _mockUnitOfWork.Setup(u => u.Subjects.FindAsync(It.IsAny<Expression<Func<Subject, bool>>>()))
                .ReturnsAsync(existingSubject);
            _mockUnitOfWork.Setup(u => u.TutorSubjects.FindAsync(It.IsAny<Expression<Func<TutorSubject, bool>>>()))
                .ReturnsAsync(tutorSubject);
            _mockUnitOfWork.Setup(u => u.Subjects.Update(It.IsAny<Subject>()));
            _mockUnitOfWork.Setup(u => u.TutorSubjects.Update(It.IsAny<TutorSubject>()));
            _mockUnitOfWork.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

            // Act
            await _subjectService.UpdateSubjectsAsync(subjectDto, tutorId);

            // Assert
            Assert.AreEqual(250, tutorSubject.Rate); // Kiểm tra tỷ lệ đã được cập nhật
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Test]
        public async Task GetTopSubjectsAsync_ShouldReturnTopSubjects_WhenDataExists()
        {
            // Arrange
            int size = 3;
            var classrooms = new List<TutorLearnerSubject>
        {
            new TutorLearnerSubject { TutorSubject = new TutorSubject { SubjectId = 1 } },
            new TutorLearnerSubject { TutorSubject = new TutorSubject { SubjectId = 1 } },
            new TutorLearnerSubject { TutorSubject = new TutorSubject { SubjectId = 2 } },
            new TutorLearnerSubject { TutorSubject = new TutorSubject { SubjectId = 3 } },
        };

            _mockUnitOfWork.Setup(u => u.TutorLearnerSubject.GetAllTutorLearnerSubjectAsync()).ReturnsAsync(classrooms);
            _mockUnitOfWork.Setup(u => u.Subjects.GetSingleById(It.IsAny<int>())).Returns(new Subject { SubjectName = "Subject" });

            // Act
            var result = await _subjectService.GetTopSubjectsAsync(size);

            // Assert
            Assert.AreEqual(size, result.Count);
            Assert.AreEqual(1, result[0].SubjectId); // Subject with most classes should be first
        }

        [Test]
        public async Task GetTopSubjectsAsync_ShouldReturnEmpty_WhenNoClassrooms()
        {
            // Arrange
            int size = 3;
            _mockUnitOfWork.Setup(u => u.TutorLearnerSubject.GetAllTutorLearnerSubjectAsync()).ReturnsAsync(new List<TutorLearnerSubject>());

            // Act
            var result = await _subjectService.GetTopSubjectsAsync(size);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task CreateSubjectAsync_ShouldCreateSubject_WhenValid()
        {
            // Arrange
            var subjectDto = new SubjectDTO { SubjectName = "Math" };
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, "userId") }));
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };

            _mockUserManager.Setup(um => um.GetUserAsync(user)).ReturnsAsync(currentUser);
            _mockUnitOfWork.Setup(u => u.Subjects.FindAsync(It.IsAny<Expression<Func<Subject, bool>>>())).ReturnsAsync((Subject)null);
            _mockUnitOfWork.Setup(u => u.Subjects.Add(It.IsAny<Subject>()));
            _mockUnitOfWork.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _subjectService.CreateSubjectAsync(subjectDto, user);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(subjectDto.SubjectName, result.SubjectName);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Test]
        public void CreateSubjectAsync_ShouldThrowException_WhenSubjectExists()
        {
            // Arrange
            var subjectDto = new SubjectDTO { SubjectName = "Math" };
            var existingSubject = new Subject { SubjectName = "Math" };

            _mockUnitOfWork.Setup(u => u.Subjects.FindAsync(It.IsAny<Expression<Func<Subject, bool>>>())).ReturnsAsync(existingSubject);

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _subjectService.CreateSubjectAsync(subjectDto, null));
        }
        [Test]
        public async Task GetSubjectByIdAsync_ShouldReturnSubject_WhenExists()
        {
            // Arrange
            int subjectId = 1;
            var subject = new Subject { SubjectId = subjectId, SubjectName = "Math" };

            _mockUnitOfWork.Setup(u => u.Subjects.FindAsync(It.IsAny<Expression<Func<Subject, bool>>>())).ReturnsAsync(subject);

            // Act
            var result = await _subjectService.GetSubjectByIdAsync(subjectId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(subjectId, result.SubjectId);
        }

        [Test]
        public void GetSubjectByIdAsync_ShouldThrowException_WhenNotFound()
        {
            // Arrange
            int subjectId = 1;

            _mockUnitOfWork.Setup(u => u.Subjects.FindAsync(It.IsAny<Expression<Func<Subject, bool>>>())).ReturnsAsync((Subject)null);

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _subjectService.GetSubjectByIdAsync(subjectId));
        }
        [Test]
        public async Task GetAllSubjectsAsync_ShouldReturnPaginatedSubjects_WhenDataExists()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 2;
            var subjects = new List<Subject>
        {
            new Subject { SubjectId = 1, SubjectName = "Math" },
            new Subject { SubjectId = 2, SubjectName = "Physics" }
        };
            int total = 1;

            var totalRecords = 1;
            _mockUnitOfWork.Setup(uow => uow.Subjects.GetMultiPaging(
                It.IsAny<Expression<Func<Subject, bool>>>(),
                out total,
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string[]>(),
                It.IsAny<Func<IQueryable<Subject>, IOrderedQueryable<Subject>>>()
            )).Returns(subjects);

            // Act
            var result = await _subjectService.GetAllSubjectsAsync(pageNumber, pageSize);

            // Assert
            Assert.AreEqual(subjects.Count, result.Item1.Count());
            Assert.AreEqual(total, result.Item2);
        }

        [Test]
        public void GetAllSubjectsAsync_ShouldThrowException_WhenInvalidPageParameters()
        {
            // Arrange
            int pageNumber = -1;
            int pageSize = 0;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _subjectService.GetAllSubjectsAsync(pageNumber, pageSize));
        }

        [Test]
        public async Task UpdateSubjectAsync_ShouldUpdateSubject_WhenValid()
        {
            // Arrange
            var subjectId = 1;
            var subjectDto = new SubjectDTO { SubjectName = "New Subject Name" };
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, "userId") }));
            var existingSubject = new Subject { SubjectId = subjectId, SubjectName = "Old Name" };
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };

            _mockUnitOfWork.Setup(u => u.Subjects.FindAsync(It.IsAny<Expression<Func<Subject, bool>>>())).ReturnsAsync(existingSubject);
            _mockUserManager.Setup(um => um.GetUserAsync(user)).ReturnsAsync(currentUser);
            _mockUnitOfWork.Setup(u => u.Subjects.Update(It.IsAny<Subject>()));
            _mockUnitOfWork.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

            // Act
            await _subjectService.UpdateSubjectAsync(subjectId, subjectDto, user);

            // Assert
            Assert.AreEqual(subjectDto.SubjectName, existingSubject.SubjectName); // Assert that the subject name was updated
            Assert.AreEqual(currentUser.Id, existingSubject.UpdatedBy); // Assert that the current user ID is set as the "UpdatedBy" field
            Assert.AreEqual(DateTime.UtcNow.Date, existingSubject.UpdatedDate?.Date); // Assert that the updated date was set to current UTC date
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once); // Ensure CommitAsync was called once
        }

        [Test]
        public void UpdateSubjectAsync_ShouldThrowException_WhenSubjectNotFound()
        {
            // Arrange
            var subjectId = 1;
            var subjectDto = new SubjectDTO { SubjectName = "New Subject Name" };
            _mockUnitOfWork.Setup(u => u.Subjects.FindAsync(It.IsAny<Expression<Func<Subject, bool>>>())).ReturnsAsync((Subject)null); // Subject not found

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _subjectService.UpdateSubjectAsync(subjectId, subjectDto, null)); // Assert that an exception is thrown
        }

        [Test]
        public void UpdateSubjectAsync_ShouldThrowException_WhenSubjectDataIsNull()
        {
            // Arrange
            var subjectId = 1;
            SubjectDTO subjectDto = null;
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, "userId") }));

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _subjectService.UpdateSubjectAsync(subjectId, subjectDto, user)); // Assert that an ArgumentException is thrown for null subjectDto
        }


        #region DeleteSubjectAsync

        [Test]
        public async Task DeleteSubjectAsync_ShouldDeleteSubject_WhenValid()
        {
            // Arrange
            var subjectId = 1;
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, "userId") }));
            var existingSubject = new Subject { SubjectId = subjectId, IsDelete = false };
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };

            _mockUnitOfWork.Setup(u => u.Subjects.FindAsync(It.IsAny<Expression<Func<Subject, bool>>>())).ReturnsAsync(existingSubject);
            _mockUserManager.Setup(um => um.GetUserAsync(user)).ReturnsAsync(currentUser);
            _mockUnitOfWork.Setup(u => u.Subjects.Update(It.IsAny<Subject>()));
            _mockUnitOfWork.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

            // Act
            await _subjectService.DeleteSubjectAsync(subjectId, user);

            // Assert
            Assert.IsTrue(existingSubject.IsDelete); // Assert that the subject is marked as deleted
            Assert.AreEqual(currentUser.Id, existingSubject.UpdatedBy); // Assert that the current user ID is set as the "UpdatedBy" field
            Assert.AreEqual(DateTime.UtcNow.Date, existingSubject.UpdatedDate?.Date); // Assert that the updated date was set to current UTC date
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once); // Ensure CommitAsync was called once
        }

        [Test]
        public void DeleteSubjectAsync_ShouldThrowException_WhenSubjectNotFound()
        {
            // Arrange
            var subjectId = 1;
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, "userId") }));

            _mockUnitOfWork.Setup(u => u.Subjects.FindAsync(It.IsAny<Expression<Func<Subject, bool>>>())).ReturnsAsync((Subject)null); // Subject not found

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _subjectService.DeleteSubjectAsync(subjectId, user)); // Assert that an exception is thrown
        }

        [Test]
        public void DeleteSubjectAsync_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            var subjectId = 1;
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, "userId") }));
            var existingSubject = new Subject { SubjectId = subjectId, IsDelete = false };

            _mockUnitOfWork.Setup(u => u.Subjects.FindAsync(It.IsAny<Expression<Func<Subject, bool>>>())).ReturnsAsync(existingSubject);
            _mockUserManager.Setup(um => um.GetUserAsync(user)).ReturnsAsync((AspNetUser)null); // User not found

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _subjectService.DeleteSubjectAsync(subjectId, user)); // Assert that an exception is thrown
        }
        #endregion
    }
}