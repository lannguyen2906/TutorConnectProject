using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Enum;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.Models;
using TutoRum.Services.IService;
using TutoRum.Services.Service;
using TutoRum.Services.ViewModels;

namespace TutoRum.UnitTests.ServiceUnitTest
{
    public class AdminServiceUnitTest
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<UserManager<AspNetUser>> _mockUserManager;
        private Mock<IMapper> _mockMapper;
        private Mock<IScheduleService> _mockScheduleService;
        private Mock<INotificationService> _notificationMock; // Thêm mock cho INotificationService
        private AdminSevice _adminService;

        [SetUp]
        public void SetUp()
        {
            // Mocking dependencies
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mockUserManager = new Mock<UserManager<AspNetUser>>(
                new Mock<IUserStore<AspNetUser>>().Object, null, null, null, null, null, null, null, null);
            _mockMapper = new Mock<IMapper>();
            _mockScheduleService = new Mock<IScheduleService>();
            _notificationMock = new Mock<INotificationService>(); // Khởi tạo mock cho INotificationService

            var mockUser = new AspNetUser { Id = Guid.NewGuid(), UserName = "TestUser" };
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(mockUser);
            // Creating AdminService instance with all dependencies
            _adminService = new AdminSevice(
                _unitOfWorkMock.Object,
                _mockUserManager.Object,
                _mockMapper.Object,
                _mockScheduleService.Object,
                _notificationMock.Object // Truyền thêm dependency này
            );
        }

        [Test]
        public async Task AssignRoleAdmin_ShouldThrowException_WhenCurrentUserNotFound()
        {
            // Arrange
            var dto = new AssignRoleAdminDto { UserId = Guid.NewGuid(), Position = "Admin", Salary = 50000 };
            var claimsPrincipal = new ClaimsPrincipal();
            _mockUserManager.Setup(x => x.GetUserAsync(claimsPrincipal)).ReturnsAsync((AspNetUser)null);

            // Act & Assert
            var exception = Assert.ThrowsAsync<Exception>(async () => await _adminService.AssignRoleAdmin(dto, claimsPrincipal));
            Assert.AreEqual("User not found.", exception.Message);
        }

        [Test]
        public async Task AssignRoleAdmin_ShouldThrowException_WhenTargetUserNotFound()
        {
            // Arrange
            var dto = new AssignRoleAdminDto { UserId = Guid.NewGuid(), Position = "Admin", Salary = 50000 };
            var claimsPrincipal = new ClaimsPrincipal();
            _mockUserManager.Setup(x => x.GetUserAsync(claimsPrincipal)).ReturnsAsync(new AspNetUser());
            _mockUserManager.Setup(x => x.FindByIdAsync(dto.UserId.ToString())).ReturnsAsync((AspNetUser)null);

            // Act & Assert
            var exception = Assert.ThrowsAsync<Exception>(async () => await _adminService.AssignRoleAdmin(dto, claimsPrincipal));
            Assert.AreEqual("Target user not found.", exception.Message);
        }

        [Test]
        public async Task AssignRoleAdmin_ShouldThrowException_WhenUserAlreadyAdmin()
        {
            // Arrange
            var dto = new AssignRoleAdminDto { UserId = Guid.NewGuid(), Position = "Admin", Salary = 50000 };
            var claimsPrincipal = new ClaimsPrincipal();
            var existingUser = new AspNetUser();
            _mockUserManager.Setup(x => x.GetUserAsync(claimsPrincipal)).ReturnsAsync(existingUser);
            _mockUserManager.Setup(x => x.FindByIdAsync(dto.UserId.ToString())).ReturnsAsync(existingUser);
            _mockUserManager.Setup(x => x.IsInRoleAsync(existingUser, AccountRoles.Admin)).ReturnsAsync(true);

            // Act & Assert
            var exception = Assert.ThrowsAsync<Exception>(async () => await _adminService.AssignRoleAdmin(dto, claimsPrincipal));
            Assert.AreEqual("User is already an admin.", exception.Message);
        }

        [Test]
        public async Task AssignRoleAdmin_ShouldThrowException_WhenUserAreAdmin()
        {
            // Arrange
            var dto = new AssignRoleAdminDto { UserId = Guid.NewGuid(), Position = "Admin", Salary = 50000 };
            var claimsPrincipal = new ClaimsPrincipal();
            var existingUser = new AspNetUser();
            _mockUserManager.Setup(x => x.GetUserAsync(claimsPrincipal)).ReturnsAsync(existingUser);
            _mockUserManager.Setup(x => x.FindByIdAsync(dto.UserId.ToString())).ReturnsAsync(existingUser);
            _mockUserManager.Setup(x => x.IsInRoleAsync(existingUser, AccountRoles.Admin)).ReturnsAsync(true); // Người dùng đã là Admin

            // Act & Assert
            var exception =  Assert.ThrowsAsync<Exception>(async () => await _adminService.AssignRoleAdmin(dto, claimsPrincipal));
            Assert.AreEqual("User is already an admin.", exception.Message); // Kiểm tra thông báo ngoại lệ
        }


        [Test]
        public async Task AssignRoleAdmin_ShouldThrowException_WhenPositionIsInvalid()
        {
            // Arrange
            var dto = new AssignRoleAdminDto { UserId = Guid.NewGuid(), Position = "Manager", Salary = 50000 }; // Position không hợp lệ
            var claimsIdentity = new ClaimsIdentity(new[]
            {
        new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
    });
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var existingUser = new AspNetUser();
            _mockUserManager.Setup(x => x.GetUserAsync(claimsPrincipal)).ReturnsAsync(existingUser);
            _mockUserManager.Setup(x => x.FindByIdAsync(dto.UserId.ToString())).ReturnsAsync(new AspNetUser());

            // Act & Assert
            var exception = Assert.ThrowsAsync<NullReferenceException>(async () => await _adminService.AssignRoleAdmin(dto, claimsPrincipal));
        }


        [Test]
        public async Task AssignRoleAdmin_ShouldThrowException_WhenRemovingRolesFails()
        {
            // Arrange
            var dto = new AssignRoleAdminDto { UserId = Guid.NewGuid(), Position = "Admin", Salary = 50000 };
            var claimsPrincipal = new ClaimsPrincipal();
            var existingUser = new AspNetUser();
            _mockUserManager.Setup(x => x.GetUserAsync(claimsPrincipal)).ReturnsAsync(existingUser);
            _mockUserManager.Setup(x => x.FindByIdAsync(dto.UserId.ToString())).ReturnsAsync(existingUser);
            _mockUserManager.Setup(x => x.IsInRoleAsync(existingUser, AccountRoles.Admin)).ReturnsAsync(false);
            _mockUserManager.Setup(x => x.GetRolesAsync(existingUser)).ReturnsAsync(new List<string> { "User" });
            _mockUserManager.Setup(x => x.RemoveFromRolesAsync(existingUser, It.IsAny<IEnumerable<string>>()))
                            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Failed to remove roles" }));

            // Act & Assert
            var exception = Assert.ThrowsAsync<Exception>(async () => await _adminService.AssignRoleAdmin(dto, claimsPrincipal));
            Assert.AreEqual("Failed to remove user's current roles.", exception.Message);
        }

        [Test]
        public async Task AssignRoleAdmin_ShouldThrowException_WhenAddingRoleFails()
        {
            // Arrange
            var dto = new AssignRoleAdminDto { UserId = Guid.NewGuid(), Position = "Admin", Salary = 50000 };
            var claimsPrincipal = new ClaimsPrincipal();
            var existingUser = new AspNetUser();
            _mockUserManager.Setup(x => x.GetUserAsync(claimsPrincipal)).ReturnsAsync(existingUser);
            _mockUserManager.Setup(x => x.FindByIdAsync(dto.UserId.ToString())).ReturnsAsync(existingUser);
            _mockUserManager.Setup(x => x.IsInRoleAsync(existingUser, AccountRoles.Admin)).ReturnsAsync(false);
            _mockUserManager.Setup(x => x.GetRolesAsync(existingUser)).ReturnsAsync(new List<string> { "User" });
            _mockUserManager.Setup(x => x.RemoveFromRolesAsync(existingUser, It.IsAny<IEnumerable<string>>()))
                            .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.AddToRoleAsync(existingUser, AccountRoles.Admin))
                            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Failed to add to Admin role" }));

            // Act & Assert
            var exception = Assert.ThrowsAsync<Exception>(async () => await _adminService.AssignRoleAdmin(dto, claimsPrincipal));
            Assert.AreEqual("Failed to add user to the Admin role.", exception.Message);
        }

      

        [Test]
        public async Task AssignRoleAdmin_ShouldThrowException_WhenCurrentUserIsNotFound()
        {
            // Arrange
            var dto = new AssignRoleAdminDto { UserId = Guid.NewGuid(), Position = "Admin", Salary = 50000, SupervisorId = 2 };
            var claimsPrincipal = new ClaimsPrincipal(); // ClaimsPrincipal không có thông tin về người dùng hiện tại

            _mockUserManager.Setup(x => x.GetUserAsync(claimsPrincipal)).ReturnsAsync((AspNetUser)null);

            // Act & Assert
            var exception = Assert.ThrowsAsync<Exception>(async () => await _adminService.AssignRoleAdmin(dto, claimsPrincipal));
            Assert.AreEqual("User not found.", exception.Message);
        }
        [Test]
        public async Task AssignRoleAdmin_ShouldThrowException_WhenTargetUserIsNotFound()
        {
            // Arrange
            var dto = new AssignRoleAdminDto { UserId = Guid.NewGuid(), Position = "Admin", Salary = 50000, SupervisorId = 1 };
            var claimsPrincipal = new ClaimsPrincipal();
            var existingUser = new AspNetUser { Id = dto.UserId };

            _mockUserManager.Setup(x => x.GetUserAsync(claimsPrincipal)).ReturnsAsync(existingUser);
            _mockUserManager.Setup(x => x.FindByIdAsync(dto.UserId.ToString())).ReturnsAsync((AspNetUser)null); // Không tìm thấy user

            // Act & Assert
            var exception = Assert.ThrowsAsync<Exception>(async () => await _adminService.AssignRoleAdmin(dto, claimsPrincipal));
            Assert.AreEqual("Target user not found.", exception.Message);
        }
        [Test]
        public async Task SetVerificationStatusAsync_ShouldThrowException_WhenUserNotFound()
        {
            var model = EntityTypeName.Tutor;
            var guidId = Guid.NewGuid();
            var userClaims = new ClaimsPrincipal();

            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync((AspNetUser)null);  // Simulate user not found

            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _adminService.SetVerificationStatusAsync(model, guidId, 0, true, "Test reason", userClaims));

            Assert.AreEqual("User not found.", ex.Message);
        }

        // 2. Test trường hợp Tutor không tồn tại
        [Test]
        public async Task SetVerificationStatusAsync_ShouldThrowException_WhenTutorNotFound()
        {
            var model = EntityTypeName.Tutor;
            var guidId = Guid.NewGuid();
            var userClaims = new ClaimsPrincipal();
            var mockUser = new AspNetUser { Id = guidId };

            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(mockUser);

            _unitOfWorkMock.Setup(uow => uow.Tutors.GetSingleByGuId(It.IsAny<Guid>()))
                .Returns((Tutor)null);  // Simulate tutor not found

            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _adminService.SetVerificationStatusAsync(model, guidId, 0, true, "Test reason", userClaims));

            Assert.AreEqual("Tutor not found.", ex.Message);
        }

        // 3. Test trường hợp SetVerificationStatus thành công cho Tutor
        [Test]
        public async Task SetVerificationStatusAsync_ShouldUpdateTutorStatus_WhenValidTutor()
        {
            var model = EntityTypeName.Tutor;
            var guidId = Guid.NewGuid();
            var userClaims = new ClaimsPrincipal();
            var mockUser = new AspNetUser { Id = guidId };
            var tutor = new Tutor { TutorId = guidId, IsVerified = false };

            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(mockUser);

            _unitOfWorkMock.Setup(uow => uow.Tutors.GetSingleByGuId(It.IsAny<Guid>()))
                .Returns(tutor);

            _unitOfWorkMock.Setup(uow => uow.Tutors.Update(It.IsAny<Tutor>())).Verifiable();
            _notificationMock.Setup(ns => ns.SendNotificationAsync(It.IsAny<NotificationRequestDto>(), false)).Returns(Task.CompletedTask);

            await _adminService.SetVerificationStatusAsync(model, guidId, 0, true, "Test reason", userClaims);

            Assert.IsTrue(tutor.IsVerified);
            Assert.AreEqual("Đã xác thực", tutor.Status);
            _unitOfWorkMock.Verify(uow => uow.Tutors.Update(It.IsAny<Tutor>()), Times.Once);
        }

        // 4. Test trường hợp TutorSubject không tồn tại
        [Test]
        public async Task SetVerificationStatusAsync_ShouldThrowException_WhenTutorSubjectNotFound()
        {
            var model = EntityTypeName.TutorSubject;
            var id = 1;
            var userClaims = new ClaimsPrincipal();
            var mockUser = new AspNetUser { Id = Guid.NewGuid() };

            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(mockUser);

            _unitOfWorkMock.Setup(uow => uow.TutorSubjects.GetSingleById(It.IsAny<int>()))
                .Returns((TutorSubject)null);  // Simulate tutor subject not found

            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _adminService.SetVerificationStatusAsync(model, Guid.NewGuid(), id, true, "Test reason", userClaims));

            Assert.AreEqual("Tutor subject with ID 1 not found.", ex.Message);
        }

        // 5. Test trường hợp SetVerificationStatus thành công cho TutorSubject
        [Test]
        public async Task SetVerificationStatusAsync_ShouldUpdateTutorSubjectStatus_WhenValidTutorSubject()
        {
            var model = EntityTypeName.TutorSubject;
            var id = 1;
            var userClaims = new ClaimsPrincipal();
            var mockUser = new AspNetUser { Id = Guid.NewGuid() };
            var tutorSubject = new TutorSubject { TutorId = Guid.NewGuid(), IsVerified = false };
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(mockUser);

            _unitOfWorkMock.Setup(uow => uow.TutorSubjects.GetSingleById(It.IsAny<int>()))
                .Returns(tutorSubject);

            _unitOfWorkMock.Setup(uow => uow.TutorSubjects.Update(It.IsAny<TutorSubject>())).Verifiable();

            await _adminService.SetVerificationStatusAsync(model, Guid.NewGuid(), id, true, "Test reason", userClaims);

            Assert.IsTrue(tutorSubject.IsVerified);
            Assert.AreEqual("Đã xác thực", tutorSubject.Status);
            _unitOfWorkMock.Verify(uow => uow.TutorSubjects.Update(It.IsAny<TutorSubject>()), Times.Once);
        }

        // 6. Test trường hợp TutorRequest không tồn tại
        [Test]
        public async Task SetVerificationStatusAsync_ShouldThrowException_WhenTutorRequestNotFound()
        {
            var model = EntityTypeName.TutorRequest;
            var id = 1;
            var userClaims = new ClaimsPrincipal();
            var mockUser = new AspNetUser { Id = Guid.NewGuid() };

            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(mockUser);

            _unitOfWorkMock.Setup(uow => uow.TutorRequest.GetSingleById(It.IsAny<int>()))
                .Returns((TutorRequest)null);  // Simulate tutor request not found

            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _adminService.SetVerificationStatusAsync(model, Guid.NewGuid(), id, true, "Test reason", userClaims));

            Assert.AreEqual("Tutor request with ID 1 not found.", ex.Message);
        }

        // 7. Test trường hợp SetVerificationStatus thành công cho TutorRequest
        [Test]
        public async Task SetVerificationStatusAsync_ShouldUpdateTutorRequestStatus_WhenValidTutorRequest()
        {
            var model = EntityTypeName.TutorRequest;
            var id = 1;
            var userClaims = new ClaimsPrincipal();
            var mockUser = new AspNetUser { Id = Guid.NewGuid() };
            var tutorRequest = new TutorRequest { Id = id, IsVerified = false };

            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(mockUser);

            _unitOfWorkMock.Setup(uow => uow.TutorRequest.GetSingleById(It.IsAny<int>()))
                .Returns(tutorRequest);

            _unitOfWorkMock.Setup(uow => uow.TutorRequest.Update(It.IsAny<TutorRequest>())).Verifiable();

            await _adminService.SetVerificationStatusAsync(model, Guid.NewGuid(), id, true, "Test reason", userClaims);

            Assert.IsTrue(tutorRequest.IsVerified);
            Assert.AreEqual("Đã xác thực", tutorRequest.Status);
            _unitOfWorkMock.Verify(uow => uow.TutorRequest.Update(It.IsAny<TutorRequest>()), Times.Once);
        }

        [Test]
        public void SetVerificationStatusAsync_ShouldThrowArgumentException_WhenInvalidEntityType()
        {
            var model = (EntityTypeName)999; // Invalid enum value
            var guidId = Guid.NewGuid();
            var userClaims = new ClaimsPrincipal();

            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _adminService.SetVerificationStatusAsync(model, guidId, 0, true, "Test reason", userClaims));

            Assert.AreEqual("Invalid entity type specified.", ex.Message);
        }

        [Test]
        public async Task SetVerificationStatusAsync_ShouldUpdateContractStatus_WhenValidContract()
        {
            var model = EntityTypeName.Contract;
            var id = 1;
            var userClaims = new ClaimsPrincipal();
            var mockUser = new AspNetUser { Id = Guid.NewGuid() };
            var contract = new TutorLearnerSubject { TutorLearnerSubjectId = id, IsContractVerified = false };

            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(mockUser);

            _unitOfWorkMock.Setup(uow => uow.TutorLearnerSubject.GetSingleById(It.IsAny<int>()))
                .Returns(contract);

            _unitOfWorkMock.Setup(uow => uow.TutorLearnerSubject.Update(It.IsAny<TutorLearnerSubject>())).Verifiable();

            await _adminService.SetVerificationStatusAsync(model, Guid.NewGuid(), id, true, "Test reason", userClaims);

            Assert.IsTrue(contract.IsContractVerified);
        }



    }
}
