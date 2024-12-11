using Microsoft.AspNetCore.Identity;
using Moq;
using System.Threading.Tasks;
using NUnit.Framework;
using TutoRum.Services.Service;
using TutoRum.Services.ViewModels;
using TutoRum.Data.Models;
using TutoRum.Data.Infrastructure;
using Microsoft.Extensions.Configuration; // Thêm namespace này để sử dụng IConfiguration
using System.Linq;
using Microsoft.AspNetCore.Http;
using TutoRum.Services.IService;
using TutoRum.Data.Enum;
using Microsoft.EntityFrameworkCore;


namespace TutoRum.UnitTests.ServiceUnitTest
{
    [TestFixture]
    public class AccountServiceUnitTest
    {
        private Mock<UserManager<AspNetUser>> _mockUserManager;
        private Mock<SignInManager<AspNetUser>> _signInManagerMock;
        private Mock<IConfiguration> _configurationMock;
        private Mock<INotificationService> _notificationMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private AccountService _adminService;

        [SetUp]
        public void Setup()
        {
            var store = new Mock<IUserStore<AspNetUser>>();
            _mockUserManager = new Mock<UserManager<AspNetUser>>(store.Object, null, null, null, null, null, null, null, null);
            _signInManagerMock = new Mock<SignInManager<AspNetUser>>(
                _mockUserManager.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<AspNetUser>>(),
                null, null, null, null);
            _configurationMock = new Mock<IConfiguration>();
            _notificationMock = new Mock<INotificationService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _adminService = new AccountService(
                _mockUserManager.Object,
                _signInManagerMock.Object,
                _configurationMock.Object,
                _notificationMock.Object,
                _unitOfWorkMock.Object);
        }

        [Test]
        public async Task SignInAsync_ShouldReturnNull_WhenCredentialsAreInvalid()
        {
            // Arrange
            var model = new SignInModel { Email = "invalid@example.com", Password = "WrongPassword" };
            _signInManagerMock
                .Setup(s => s.PasswordSignInAsync(model.Email, model.Password, false, false))
                .ReturnsAsync(SignInResult.Failed);

            // Act
            var result = await _adminService.SignInAsync(model);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task SignInAsync_ShouldReturnSignInResponseDto_WhenCredentialsAreValid()
        {
            // Arrange
            var model = new SignInModel { Email = "valid@example.com", Password = "CorrectPassword" };
            var user = new AspNetUser
            {
                Id = Guid.NewGuid(),
                Email = model.Email,
                Fullname = "Valid User"
            };
            var roles = new List<string> { "Admin", "User" };

            _signInManagerMock
                .Setup(s => s.PasswordSignInAsync(model.Email, model.Password, false, false))
                .ReturnsAsync(SignInResult.Success);
            _mockUserManager
                .Setup(u => u.FindByEmailAsync(model.Email))
                .ReturnsAsync(user);
            _mockUserManager
                .Setup(u => u.GetRolesAsync(user))
                .ReturnsAsync(roles);

            // Act
            var result = await _adminService.SignInAsync(model);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(user.Id, result.ID);
            Assert.AreEqual(user.Email, result.Email);
            Assert.AreEqual(user.Fullname, result.Fullname);
            Assert.AreEqual(roles, result.Roles);
        }

        [Test]
        public async Task SignInAsync_ShouldHandleUserNotFound()
        {
            // Arrange
            var model = new SignInModel { Email = "unknown@example.com", Password = "CorrectPassword" };
            _signInManagerMock
                .Setup(s => s.PasswordSignInAsync(model.Email, model.Password, false, false))
                .ReturnsAsync(SignInResult.Success);
            _mockUserManager
                .Setup(u => u.FindByEmailAsync(model.Email))
                .ReturnsAsync((AspNetUser)null);

            // Act
            var result = await _adminService.SignInAsync(model);

            // Assert
            Assert.IsNull(result);
        }


        [Test]
        public async Task SignInAsync_ShouldHandleNoRolesForUser()
        {
            // Arrange
            var model = new SignInModel { Email = "norole@example.com", Password = "CorrectPassword" };
            var user = new AspNetUser
            {
                Id = Guid.NewGuid(),
                Email = model.Email,
                Fullname = "No Role User"
            };

            _signInManagerMock
                .Setup(s => s.PasswordSignInAsync(model.Email, model.Password, false, false))
                .ReturnsAsync(SignInResult.Success);
            _mockUserManager
                .Setup(u => u.FindByEmailAsync(model.Email))
                .ReturnsAsync(user);
            _mockUserManager
                .Setup(u => u.GetRolesAsync(user))
                .ReturnsAsync(new List<string>());

            // Act
            var result = await _adminService.SignInAsync(model);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result.Roles);
        }

        [Test]
        public async Task SignInAsync_ShouldReturnNull_WhenSignInManagerThrowsException()
        {
            // Arrange
            var model = new SignInModel { Email = "error@example.com", Password = "CorrectPassword" };
            _signInManagerMock
                .Setup(s => s.PasswordSignInAsync(model.Email, model.Password, false, false))
                .ThrowsAsync(new Exception("Sign-in error"));

            // Act
            var result = await _adminService.SignInAsync(model);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task SignUpAsync_ShouldReturnSuccess_WhenUserCreatedSuccessfully()
        {
            // Arrange
            var model = new SignUpModel { Email = "test@example.com", Password = "CorrectPassword", Fullname = "Test User" };
            var user = new AspNetUser { Email = model.Email, UserName = model.Email, Fullname = model.Fullname };

            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<AspNetUser>(), model.Password))
                .ReturnsAsync(IdentityResult.Success);
            _notificationMock.Setup(ns => ns.SendNotificationAsync(It.IsAny<NotificationRequestDto>(), false))
                .Returns(Task.CompletedTask);
            _mockUserManager.Setup(um => um.AddToRoleAsync(It.IsAny<AspNetUser>(), AccountRoles.Learner))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _adminService.SignUpAsync(model);

            // Assert
            Assert.IsTrue(result.Succeeded);
        }

        [Test]
        public async Task SignUpAsync_ShouldReturnFailure_WhenUserCreationFails()
        {
            // Arrange
            var model = new SignUpModel { Email = "test@example.com", Password = "CorrectPassword", Fullname = "Test User" };

            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<AspNetUser>(), model.Password))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Error" }));

            // Act
            var result = await _adminService.SignUpAsync(model);

            // Assert
            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual("Error", result.Errors.First().Description);
        }

        [Test]
        public async Task SignUpAsync_ShouldSendNotification_WhenUserCreatedSuccessfully()
        {
            // Arrange
            var model = new SignUpModel { Email = "test@example.com", Password = "CorrectPassword", Fullname = "Test User" };

            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<AspNetUser>(), model.Password))
                .ReturnsAsync(IdentityResult.Success);
            _notificationMock.Setup(ns => ns.SendNotificationAsync(It.IsAny<NotificationRequestDto>(), false))
                .Returns(Task.CompletedTask);
            _mockUserManager.Setup(um => um.AddToRoleAsync(It.IsAny<AspNetUser>(), AccountRoles.Learner))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _adminService.SignUpAsync(model);

            // Assert
            _notificationMock.Verify(ns => ns.SendNotificationAsync(It.IsAny<NotificationRequestDto>(), false), Times.Once);
        }

        [Test]
        public async Task SignUpAsync_ShouldFail_WhenRoleAssignmentFails()
        {
            // Arrange
            var model = new SignUpModel { Email = "test@example.com", Password = "CorrectPassword", Fullname = "Test User" };
            var user = new AspNetUser { Email = model.Email, UserName = model.Email, Fullname = model.Fullname };

            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<AspNetUser>(), model.Password))
                .ReturnsAsync(IdentityResult.Success);
            _notificationMock.Setup(ns => ns.SendNotificationAsync(It.IsAny<NotificationRequestDto>(), false))
                .Returns(Task.CompletedTask);
            _mockUserManager.Setup(um => um.AddToRoleAsync(It.IsAny<AspNetUser>(), AccountRoles.Learner))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Role assignment failed" }));

            // Act
            var result = await _adminService.SignUpAsync(model);

            // Assert
            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual("Role assignment failed", result.Errors.First().Description);
        }

        [Test]
        public async Task SignUpAsync_ShouldReturnFailure_WhenNotificationServiceFails()
        {
            // Arrange
            var model = new SignUpModel { Email = "test@example.com", Password = "CorrectPassword", Fullname = "Test User" };
            var user = new AspNetUser { Email = model.Email, UserName = model.Email, Fullname = model.Fullname };

            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<AspNetUser>(), model.Password))
                .ReturnsAsync(IdentityResult.Success);
            _notificationMock.Setup(ns => ns.SendNotificationAsync(It.IsAny<NotificationRequestDto>(), false))
                .ThrowsAsync(new Exception("Notification service error"));
            _mockUserManager.Setup(um => um.AddToRoleAsync(It.IsAny<AspNetUser>(), AccountRoles.Learner))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _adminService.SignUpAsync(model);

            // Assert
            Assert.IsFalse(result.Succeeded);
        }

        [Test]
        public async Task SignUpAsync_ShouldUseDefaultAvatarUrl_WhenAvatarUrlIsNotProvided()
        {
            // Arrange
            var model = new SignUpModel { Email = "test@example.com", Password = "CorrectPassword", Fullname = "Test User" };
            var user = new AspNetUser { Email = model.Email, UserName = model.Email, Fullname = model.Fullname };

            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<AspNetUser>(), model.Password))
                .ReturnsAsync(IdentityResult.Success);
            _notificationMock.Setup(ns => ns.SendNotificationAsync(It.IsAny<NotificationRequestDto>(), false))
                .Returns(Task.CompletedTask);
            _mockUserManager.Setup(um => um.AddToRoleAsync(It.IsAny<AspNetUser>(), AccountRoles.Learner))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _adminService.SignUpAsync(model);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task SignUpAsync_ShouldReturnFailure_WhenEmailIsAlreadyTaken()
        {
            // Arrange
            var model = new SignUpModel { Email = "test@example.com", Password = "CorrectPassword", Fullname = "Test User" };
            var user = new AspNetUser { Email = model.Email, UserName = model.Email, Fullname = model.Fullname };

            _mockUserManager.Setup(um => um.FindByEmailAsync(model.Email))
                .ReturnsAsync(user);
            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<AspNetUser>(), model.Password))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Email already taken" }));

            // Act
            var result = await _adminService.SignUpAsync(model);

            // Assert
            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual("Email already taken", result.Errors.First().Description);
        }

        [Test]
        public async Task SignUpAsync_ShouldUseEmailAsUsername_WhenFullnameIsNotProvided()
        {
            // Arrange
            var model = new SignUpModel { Email = "test@example.com", Password = "CorrectPassword" };

            // Mock the CreateAsync method to return success
            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<AspNetUser>(), model.Password))
                .ReturnsAsync(IdentityResult.Success);

            // Mock role assignment success
            _mockUserManager.Setup(um => um.AddToRoleAsync(It.IsAny<AspNetUser>(), AccountRoles.Learner))
                .ReturnsAsync(IdentityResult.Success);

            // Mock SendNotificationAsync method
            _notificationMock.Setup(ns => ns.SendNotificationAsync(It.IsAny<NotificationRequestDto>(), false))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _adminService.SignUpAsync(model);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task GetAllAccountsAsync_ShouldReturnEmptyList_WhenNoUsersExist()
        {
            // Arrange
            var emptyUserList = new List<AspNetUser>().AsQueryable();  // IQueryable from the empty list

            // Mock the Users property to return the IQueryable of the empty list
            _mockUserManager.Setup(x => x.Users).Returns(emptyUserList); // Return IQueryable

            // Act
            var result = await _adminService.GetAllAccountsAsync();

            // Assert
            Assert.IsEmpty(result);  // Verify the result is an empty list
        }






        [Test]
        public async Task GetAllAccountsAsync_ShouldReturnUsersWithEmptyRoles_WhenUsersHaveNoRoles()
        {
            // Arrange
            var user = new AspNetUser { Id = Guid.NewGuid(), Fullname = "Test User" };
            var users = new List<AspNetUser> { user }.AsQueryable();  // Convert to IQueryable
            _mockUserManager.Setup(x => x.Users).Returns(users);  // Mock Users property
            _mockUserManager.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string>());  // Mock GetRolesAsync

            // Act
            var result = await _adminService.GetAllAccountsAsync();

            // Assert
            Assert.AreEqual(1, result.Count());  // Verify one user is returned
            Assert.AreEqual("Test User", result.First().Fullname);  // Verify user fullname
            Assert.IsEmpty(result.First().Roles);  // Verify user has no roles
        }
        [Test]
        public async Task GetAllAccountsAsync_ShouldReturnUsersWithRoles_WhenUsersHaveRoles()
        {
            // Arrange
            var user = new AspNetUser { Id = Guid.NewGuid(), Fullname = "Test User" };
            var roles = new List<string> { "Admin", "User" };
            var users = new List<AspNetUser> { user }.AsQueryable();  // Convert to IQueryable
            _mockUserManager.Setup(x => x.Users).Returns(users);  // Mock Users property
            _mockUserManager.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roles);  // Mock GetRolesAsync

            // Act
            var result = await _adminService.GetAllAccountsAsync();

            // Assert
            Assert.AreEqual(1, result.Count());  // Verify one user is returned
            Assert.AreEqual("Test User", result.First().Fullname);  // Verify user fullname
            Assert.AreEqual(2, result.First().Roles.Count);  // Verify user has two roles
            Assert.Contains("Admin", result.First().Roles);  // Verify "Admin" role is included
            Assert.Contains("User", result.First().Roles);  // Verify "User" role is included
        }
        [Test]
        public async Task GetAllAccountsAsync_ShouldThrowException_WhenErrorOccursWhileFetchingRoles()
        {
            // Arrange
            var user = new AspNetUser { Id = Guid.NewGuid(), Fullname = "Test User" };
            var users = new List<AspNetUser> { user }.AsQueryable();  // Convert to IQueryable
            _mockUserManager.Setup(x => x.Users).Returns(users);  // Mock Users property
            _mockUserManager.Setup(x => x.GetRolesAsync(user)).ThrowsAsync(new Exception("Error fetching roles"));  // Mock error in GetRolesAsync

            // Act & Assert
            var exception =  Assert.ThrowsAsync<Exception>(async () => await _adminService.GetAllAccountsAsync());  // Verify exception is thrown
        }

        [Test]
        public async Task BlockUserAsync_ShouldBlockUser_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new AspNetUser { Id = userId, LockoutEnabled = true };
            _mockUserManager.Setup(x => x.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            // Act
            await _adminService.BlockUserAsync(userId);

            // Assert
            Assert.IsFalse(user.LockoutEnabled); // Kiểm tra nếu LockoutEnabled đã được set thành false
            _mockUserManager.Verify(x => x.UpdateAsync(user), Times.Once); // Kiểm tra xem UpdateAsync có được gọi
        }

        [Test]
        public async Task BlockUserAsync_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockUserManager.Setup(x => x.FindByIdAsync(userId.ToString())).ReturnsAsync((AspNetUser)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _adminService.BlockUserAsync(userId));
            Assert.AreEqual("User not found.", ex.Message);
        }

        [Test]
        public async Task BlockUserAsync_ShouldThrowException_WhenUpdateFails()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new AspNetUser { Id = userId, LockoutEnabled = true };
            _mockUserManager.Setup(x => x.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Error blocking user." }));

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _adminService.BlockUserAsync(userId));
            Assert.AreEqual("Error blocking user.", ex.Message);
        }

        [Test]
        public async Task UnblockUserAsync_ShouldUnblockUser_WhenUserIsLockedOut()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new AspNetUser { Id = userId, LockoutEnabled = false };
            _mockUserManager.Setup(x => x.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            // Act
            await _adminService.UnblockUserAsync(userId);

            // Assert
            Assert.IsTrue(user.LockoutEnabled); // Kiểm tra nếu LockoutEnabled đã được set thành true
            _mockUserManager.Verify(x => x.UpdateAsync(user), Times.Once); // Kiểm tra xem UpdateAsync có được gọi
        }

        [Test]
        public async Task UnblockUserAsync_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockUserManager.Setup(x => x.FindByIdAsync(userId.ToString())).ReturnsAsync((AspNetUser)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _adminService.UnblockUserAsync(userId));
            Assert.AreEqual("User not found.", ex.Message);
        }

        [Test]
        public async Task UnblockUserAsync_ShouldThrowException_WhenUserIsNotLockedOut()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new AspNetUser { Id = userId, LockoutEnabled = true }; // Người dùng không bị lock
            _mockUserManager.Setup(x => x.FindByIdAsync(userId.ToString())).ReturnsAsync(user);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _adminService.UnblockUserAsync(userId));
            Assert.AreEqual("User is not locked out.", ex.Message);
        }

        [Test]
        public async Task BlockUserAsync_ShouldThrowException_WhenUserIsAlreadyBlocked()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new AspNetUser { Id = userId, LockoutEnabled = false }; // Người dùng đã bị block (LockoutEnabled = false)
            _mockUserManager.Setup(x => x.FindByIdAsync(userId.ToString())).ReturnsAsync(user);

            // Act & Assert
            var ex = Assert.ThrowsAsync<NullReferenceException>(async () => await _adminService.BlockUserAsync(userId));
        }
        [Test]
        public async Task UnblockUserAsync_ShouldThrowException_WhenUserIsNotBlocked()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new AspNetUser { Id = userId, LockoutEnabled = true }; // Người dùng chưa bị lock (LockoutEnabled = true)
            _mockUserManager.Setup(x => x.FindByIdAsync(userId.ToString())).ReturnsAsync(user);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _adminService.UnblockUserAsync(userId));
            Assert.AreEqual("User is not locked out.", ex.Message); // Kiểm tra thông báo lỗi
        }

    }
}








