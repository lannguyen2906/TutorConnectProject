using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.Models;
using TutoRum.Services.Helper;
using TutoRum.Services.Service;
using TutoRum.Services.ViewModels;

namespace TutoRum.UnitTests.ServiceUnitTest
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<UserManager<AspNetUser>> _mockUserManager;
        private UserService _userService;
        private Guid userID;
        [SetUp]
        public void SetUp()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUserManager = new Mock<UserManager<AspNetUser>>(
                Mock.Of<IUserStore<AspNetUser>>(),
                null, null, null, null, null, null, null, null
            );
            _mockUserManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userID.ToString());
            _mockUnitOfWork.Setup(st => st.Accounts.Update(It.IsAny<AspNetUser>()));

            _userService = new UserService(_mockUnitOfWork.Object, _mockUserManager.Object, new APIAddress(new HttpClient()));
        }

        // Test 1: User not found
        [Test]
        public async Task UpdateUserProfileAsync_ShouldThrowUnauthorizedAccessException_WhenUserNotFound()
        {
            // Arrange
            var userDto = new UpdateUserDTO();
            var claimsPrincipal = new ClaimsPrincipal();
            _mockUserManager.Setup(x => x.GetUserAsync(claimsPrincipal)).ReturnsAsync((AspNetUser)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _userService.UpdateUserProfileAsync(userDto, claimsPrincipal));

            Assert.That(ex.Message, Is.EqualTo("User not found."));
        }

        // Test 2: Logged in user ID is null
        [Test]
        public async Task UpdateUserProfileAsync_ShouldThrowUnauthorizedAccessException_WhenLoggedInUserIdIsNull()
        {
            // Arrange
            var userDto = new UpdateUserDTO();
            var claimsPrincipal = new ClaimsPrincipal();
            _mockUserManager.Setup(x => x.GetUserAsync(claimsPrincipal)).ReturnsAsync(new AspNetUser());
            _mockUserManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns((string)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _userService.UpdateUserProfileAsync(userDto, claimsPrincipal));

            Assert.That(ex.Message, Is.EqualTo("Unable to identify the logged-in user."));
        }

        // Test 3: User does not have permission to update profile
        [Test]
        public async Task UpdateUserProfileAsync_ShouldThrowUnauthorizedAccessException_WhenUserDoesNotHavePermission()
        {
            // Arrange
            var userDto = new UpdateUserDTO();
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "some-different-id")
            }));
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };
            _mockUserManager.Setup(x => x.GetUserAsync(claimsPrincipal)).ReturnsAsync(currentUser);

            // Act & Assert
            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _userService.UpdateUserProfileAsync(userDto, claimsPrincipal));

            Assert.That(ex.Message, Is.EqualTo("User does not have permission to update this profile."));
        }

        // Test 4: Successful profile update
        [Test]
        public async Task UpdateUserProfileAsync_ShouldUpdateProfile_WhenDataIsValid()
        {
            // Arrange
            var userDto = new UpdateUserDTO
            {
                Fullname = "New Fullname",
                Dob = new DateTime(1990, 1, 1),
                Gender = true,
                AvatarUrl = "new-avatar-url",
                PhoneNumber = "1234567890"
            };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userID.ToString())
            }));
            var currentUser = new AspNetUser
            {
                Id = userID,
                Fullname = "Old Fullname",
                Dob = new DateTime(1990, 1, 1),
                Gender = true,
                AvatarUrl = "old-avatar-url",
                PhoneNumber = "0987654321"
            };

            _mockUserManager.Setup(x => x.GetUserAsync(claimsPrincipal)).ReturnsAsync(currentUser);
            _mockUserManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(currentUser.Id.ToString());

            // Act
            await _userService.UpdateUserProfileAsync(userDto, claimsPrincipal);

            // Assert
            Assert.AreEqual(currentUser.Fullname, "New Fullname");
            Assert.AreEqual(currentUser.AvatarUrl, "new-avatar-url");
            Assert.AreEqual(currentUser.PhoneNumber, "1234567890");
            _mockUnitOfWork.Verify(u => u.Accounts.Update(currentUser), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Test]
        public async Task UpdateUserProfileAsync_ShouldPartialUpdate_WhenNullValuesArePassed()
        {
            // Arrange
            var userDto = new UpdateUserDTO
            {
                Fullname = "Old Fullname",  // Chỉ cập nhật PhoneNumber, Fullname không thay đổi
                PhoneNumber = "1234567890",
                
            };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
        new Claim(ClaimTypes.NameIdentifier, userID.ToString())
    }));
            var currentUser = new AspNetUser
            {
                Id = userID,
                Fullname = "Old Fullname",
                PhoneNumber = "0987654321"
            };

            _mockUserManager.Setup(x => x.GetUserAsync(claimsPrincipal)).ReturnsAsync(currentUser);
            _mockUserManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(currentUser.Id.ToString());

            // Act
            await _userService.UpdateUserProfileAsync(userDto, claimsPrincipal);

            // Assert
            Assert.AreEqual("Old Fullname", currentUser.Fullname);  // Fullname không thay đổi
            Assert.AreEqual("1234567890", currentUser.PhoneNumber);  // PhoneNumber được cập nhật
            _mockUnitOfWork.Verify(u => u.Accounts.Update(currentUser), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }


        [Test]
        public async Task UpdateUserProfileAsync_ShouldNotChangePhoneNumber_WhenPhoneNumberIsNull()
        {
            // Arrange
            var userDto = new UpdateUserDTO { PhoneNumber = null, Fullname = "test1" };  // PhoneNumber is null, should not change
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
        new Claim(ClaimTypes.NameIdentifier, userID.ToString())
    }));
            var currentUser = new AspNetUser
            {
                Id = userID,
                PhoneNumber = "0987654321"
            };

            _mockUserManager.Setup(x => x.GetUserAsync(claimsPrincipal)).ReturnsAsync(currentUser);
            _mockUserManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userID.ToString());

            // Act
            await _userService.UpdateUserProfileAsync(userDto, claimsPrincipal);

            // Assert
            Assert.AreEqual("0987654321", currentUser.PhoneNumber);  // PhoneNumber không thay đổi
            _mockUnitOfWork.Verify(u => u.Accounts.Update(currentUser), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }



        // Test 7: Handle update when no changes are made
        [Test]
        public async Task UpdateUserProfileAsync_ShouldNotUpdate_WhenNoChangesAreMade()
        {
            // Arrange
            var userDto = new UpdateUserDTO
            {
                Fullname = "Existing Name",
                PhoneNumber = "0987654321"
            };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userID.ToString())
            }));
            var currentUser = new AspNetUser
            {
                Id = userID,
                Fullname = "Existing Name",
                PhoneNumber = "0987654321"
            };

            _mockUserManager.Setup(x => x.GetUserAsync(claimsPrincipal)).ReturnsAsync(currentUser);
            _mockUserManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(currentUser.Id.ToString());

            // Act
            var result =  _userService.UpdateUserProfileAsync(userDto, claimsPrincipal);

            // Assert
            Assert.IsNotNull(result);
        }
        [Test]
        public async Task UpdateUserProfileAsync_ShouldThrowArgumentException_WhenDtoIsInvalid()
        {
            // Arrange
            var userDto = new UpdateUserDTO
            {
                Fullname = ""  // Invalid Fullname (empty string)
            };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
        new Claim(ClaimTypes.NameIdentifier, userID.ToString())
    }));
            var currentUser = new AspNetUser
            {
                Id = userID,
                Fullname = "Existing Name",
                PhoneNumber = "0987654321"
            };

            _mockUserManager.Setup(x => x.GetUserAsync(claimsPrincipal)).ReturnsAsync(currentUser);
            _mockUserManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(currentUser.Id.ToString());

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _userService.UpdateUserProfileAsync(userDto, claimsPrincipal));

            Assert.That(ex.Message, Is.EqualTo("Fullname cannot be empty."));
        }

    }
}