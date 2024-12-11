using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using TutoRum.Data.Enum;
using TutoRum.Data.Models;
using TutoRum.FE.Common;
using TutoRum.FE.Controllers;
using TutoRum.Services.IService;
using TutoRum.Services.ViewModels;

namespace TutoRum.UnitTests.TutoRum.FE.UnitTest.Controller
{
    [TestFixture]
    public class AccountsControllerTests
    {
        private Mock<IAccountService> _accountServiceMock;
        private Mock<IEmailService> _emailServiceMock;
        private Mock<UserManager<AspNetUser>> _userManagerMock;
        private AccountsController _controller;

        [SetUp]
        public void SetUp()
        {
            _accountServiceMock = new Mock<IAccountService>();
            _emailServiceMock = new Mock<IEmailService>();

            // Mock the UserManager
            var storeMock = new Mock<IUserStore<AspNetUser>>();
            var optionsMock = new Mock<IOptions<IdentityOptions>>();
            var passwordHasherMock = new Mock<IPasswordHasher<AspNetUser>>();
            var userValidators = new List<IUserValidator<AspNetUser>>();
            var passwordValidators = new List<IPasswordValidator<AspNetUser>>();
            var keyNormalizerMock = new Mock<ILookupNormalizer>();
            var errorsMock = new Mock<IdentityErrorDescriber>();
            var servicesMock = new Mock<IServiceProvider>();
            var loggerMock = new Mock<ILogger<UserManager<AspNetUser>>>();

            _userManagerMock = new Mock<UserManager<AspNetUser>>(
                storeMock.Object, optionsMock.Object, passwordHasherMock.Object,
                userValidators, passwordValidators, keyNormalizerMock.Object,
                errorsMock.Object, servicesMock.Object, loggerMock.Object);

            // Mock HttpContext and Request
            var httpContextMock = new Mock<HttpContext>();
            var httpRequestMock = new Mock<HttpRequest>();
            httpRequestMock.Setup(r => r.Scheme).Returns("https");
            httpContextMock.Setup(c => c.Request).Returns(httpRequestMock.Object);

            // Mock the required services for TestAuthHandler
            var optionsMonitorMock = new Mock<IOptionsMonitor<AuthenticationSchemeOptions>>();
            var loggerFactoryMock = new Mock<ILoggerFactory>();
            var urlEncoderMock = new Mock<UrlEncoder>();
            var systemClockMock = new Mock<ISystemClock>();

            // Mock the authentication service
            var authenticationServiceMock = new Mock<IAuthenticationService>();

            // Add the mocked services to the HttpContext
            httpContextMock.Setup(c => c.RequestServices.GetService(typeof(IAuthenticationService)))
                .Returns(authenticationServiceMock.Object);
            httpContextMock.Setup(c => c.RequestServices.GetService(typeof(IOptionsMonitor<AuthenticationSchemeOptions>)))
                .Returns(optionsMonitorMock.Object);
            httpContextMock.Setup(c => c.RequestServices.GetService(typeof(ILoggerFactory)))
                .Returns(loggerFactoryMock.Object);
            httpContextMock.Setup(c => c.RequestServices.GetService(typeof(UrlEncoder)))
                .Returns(urlEncoderMock.Object);
            httpContextMock.Setup(c => c.RequestServices.GetService(typeof(ISystemClock)))
                .Returns(systemClockMock.Object);

            // Instantiate the controller with mocked dependencies
            _controller = new AccountsController(_accountServiceMock.Object, _emailServiceMock.Object, _userManagerMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContextMock.Object
                }
            };
        }

        [Test]
        public async Task BlockUserAsync_ShouldReturnOk_WhenUserIsBlockedSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };

            // Mock UserManager's methods
            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);
            _userManagerMock.Setup(um => um.IsInRoleAsync(currentUser, AccountRoles.Admin)).ReturnsAsync(true);

            // Mock the account service to block the user
            _accountServiceMock.Setup(a => a.BlockUserAsync(userId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.BlockUserAsync(userId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult.Value);
        }

        [Test]
        public async Task BlockUserAsync_ShouldReturnUnauthorized_WhenCurrentUserIsNotAdmin()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };

            // Mock UserManager's methods
            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);
            _userManagerMock.Setup(um => um.IsInRoleAsync(currentUser, AccountRoles.Admin)).ReturnsAsync(false);

            // Act
            var result = await _controller.BlockUserAsync(userId);

            // Assert
            Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.IsNotNull(unauthorizedResult.Value);
        }


        [Test]
        public async Task SignUp_ShouldReturnUnauthorized_WhenRegistrationFails()
        {
            // Arrange
            var signUpModel = new SignUpModel { Email = "test@example.com", Password = "Password123" };
            _accountServiceMock.Setup(a => a.SignUpAsync(signUpModel))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Registration failed" }));

            // Act
            var result = await _controller.SignUp(signUpModel);

            // Assert
            Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.IsNotNull(unauthorizedResult.Value);
        }

        [Test]
        public async Task SignIn_ShouldReturnOk_WhenCredentialsAreValid()
        {
            // Arrange
            var signInModel = new SignInModel { Email = "test@example.com", Password = "Password123" };
            var user = new AspNetUser { Email = signInModel.Email, LockoutEnabled = true };
            _userManagerMock.Setup(um => um.FindByEmailAsync(signInModel.Email)).ReturnsAsync(user);
            _accountServiceMock.Setup(a => a.SignInAsync(signInModel)).ReturnsAsync(new SignInResponseDto());

            // Act
            var result = await _controller.SignIn(signInModel);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult.Value);
        }

        [Test]
        public async Task SignIn_ShouldReturnBadRequest_WhenAccountIsBlocked()
        {
            // Arrange
            var signInModel = new SignInModel { Email = "test@example.com", Password = "Password123" };
            var user = new AspNetUser { Email = signInModel.Email, LockoutEnabled = false }; // Account is blocked
            _userManagerMock.Setup(um => um.FindByEmailAsync(signInModel.Email)).ReturnsAsync(user);

            // Act
            var result = await _controller.SignIn(signInModel);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Tài khoản của bạn đã bị block.", badRequestResult.Value);
        }
        [Test]
        public async Task GetCurrentUser_ShouldReturnOk_WhenUserIsFound()
        {
            // Arrange
            var user = new SignInResponseDto
            {
                Fullname = "John Doe",
                Dob = new DateTime(1990, 1, 1),
                Gender = true,
                AvatarUrl = "https://example.com/avatar.jpg",
                Email = "test@example.com",
                PhoneNumber = "123456789",
                CityId = "Address123",
                AddressDetail = "123 Main St",
                Roles = new List<string> { "Admin", "User" }
            };

            // Mocking the account service's GetCurrentUser to return the SignInResponseDto
            _accountServiceMock.Setup(a => a.GetCurrentUser(It.IsAny<ClaimsPrincipal>()))
                               .ReturnsAsync(user);

            // Act
            var result = await _controller.GetCurrentUser();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsNotNull(okResult.Value);

            var response = okResult.Value as ApiResponse<SignInResponseDto>;
            Assert.IsNotNull(response);
            Assert.AreEqual(user.Email, response.Data.Email);
        }

        [Test]
        public async Task GetCurrentUser_ShouldReturnNotFound_WhenUserIsNotFound()
        {
            // Arrange
            _accountServiceMock.Setup(a => a.GetCurrentUser(It.IsAny<ClaimsPrincipal>()))
                               .ReturnsAsync((SignInResponseDto)null);  // Return null when user is not found

            // Act
            var result = await _controller.GetCurrentUser();

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("{ message = User not found }", notFoundResult.Value.ToString());
        }

        [Test]
        public async Task SignUp_ShouldReturnOk_WhenSignUpSucceeds()
        {
            // Arrange
            var signUpModel = new SignUpModel { Email = "test@example.com", Password = "Password123" };
            var mockUser = new AspNetUser { Id = Guid.NewGuid(), Email = signUpModel.Email };

            // Mock SignUpAsync to succeed
            _accountServiceMock.Setup(a => a.SignUpAsync(signUpModel))
                .ReturnsAsync(IdentityResult.Success);

            // Mock GetUserByEmailAsync to return the registered user
            _accountServiceMock.Setup(a => a.GetUserByEmailAsync(signUpModel.Email))
                .ReturnsAsync(mockUser);

            // Mock GenerateEmailConfirmationTokenAsync to return a token
            _accountServiceMock.Setup(a => a.GenerateEmailConfirmationTokenAsync(mockUser))
                .ReturnsAsync("MockConfirmationToken");

            // Mock sending the confirmation email
            _emailServiceMock.Setup(e => e.SendEmailAsync(mockUser.Email, It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Mock Url.Action to return a confirmation link
            _controller.Url = new Mock<IUrlHelper>().Object;
            var urlHelperMock = new Mock<IUrlHelper>(MockBehavior.Strict);
            urlHelperMock.Setup(u => u.Action(It.IsAny<UrlActionContext>()))
                         .Returns("https://example.com/confirm-email?userId=" + mockUser.Id + "&token=MockConfirmationToken");
            _controller.Url = urlHelperMock.Object;

            // Act
            var result = await _controller.SignUp(signUpModel);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual("Đăng ký thành công. Đã gửi email xác nhận.", ((ApiResponse<object>)okResult.Value).Message);
        }

        [Test]
        public async Task SignUp_ShouldReturnNotFound_WhenUserIsNotFoundAfterSignUp()
        {
            // Arrange
            var signUpModel = new SignUpModel { Email = "test@example.com", Password = "Password123" };

            // Mock SignUpAsync to succeed
            _accountServiceMock.Setup(a => a.SignUpAsync(signUpModel))
                .ReturnsAsync(IdentityResult.Success);

            // Mock GetUserByEmailAsync to return null (user not found)
            _accountServiceMock.Setup(a => a.GetUserByEmailAsync(signUpModel.Email))
                .ReturnsAsync((AspNetUser)null);

            // Act
            var result = await _controller.SignUp(signUpModel);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual("Không tìm thấy người dùng sau khi đăng ký.", ((ApiResponse<object>)notFoundResult.Value).Message);
        }

        [Test]
        public async Task SignUp_ShouldReturnUnauthorized_WhenSignUpFails()
        {
            // Arrange
            var signUpModel = new SignUpModel { Email = "test@example.com", Password = "Password123" };

            // Mock SignUpAsync to fail
            _accountServiceMock.Setup(a => a.SignUpAsync(signUpModel))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Registration failed" }));

            // Act
            var result = await _controller.SignUp(signUpModel);

            // Assert
            Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual("Đăng ký không thành công.", ((ApiResponse<object>)unauthorizedResult.Value).Message);
        }

        [Test]
        public async Task SignUp_ShouldReturnServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var signUpModel = new SignUpModel { Email = "test@example.com", Password = "Password123" };

            // Mock SignUpAsync to throw an exception
            _accountServiceMock.Setup(a => a.SignUpAsync(signUpModel))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.SignUp(signUpModel);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var serverErrorResult = result as ObjectResult;
            Assert.AreEqual(500, serverErrorResult.StatusCode);
        }

        [Test]
        public async Task SendEmailConfirmation_ShouldReturnOk_WhenUserIsFound()
        {
            // Arrange
            var userId = Guid.NewGuid(); // Ensure it's a string since your method expects a string
            var mockUser = new AspNetUser { Id = userId, Email = "test@example.com" };

            _accountServiceMock.Setup(a => a.GetUserByIdAsync(userId.ToString())).ReturnsAsync(mockUser);
            _accountServiceMock.Setup(a => a.GenerateEmailConfirmationTokenAsync(mockUser)).ReturnsAsync("MockToken");
            _emailServiceMock.Setup(e => e.SendEmailAsync(mockUser.Email, It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Mock the Url.Action to return a confirmation link
            var confirmationLink = "https://localhost:5001/Accounts/ConfirmEmail?userId=" + userId + "&token=MockToken";
            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock.Setup(u => u.Action(It.IsAny<UrlActionContext>()))
                .Returns(confirmationLink);

            _controller.Url = urlHelperMock.Object;

            // Act
            var result = await _controller.SendEmailConfirmation(userId.ToString());

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
        }

        [Test]
        public async Task SendEmailConfirmation_ShouldReturnNotFound_WhenUserIsNotFound()
        {
            // Arrange
            var userId = "nonExistentUserId";
            _accountServiceMock.Setup(a => a.GetUserByIdAsync(userId)).ReturnsAsync((AspNetUser)null);

            // Act
            var result = await _controller.SendEmailConfirmation(userId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("User not found.", ((ApiResponse<object>)notFoundResult.Value).Message);
        }

        [Test]
        public async Task ConfirmEmail_ShouldRedirect_WhenConfirmationIsSuccessful()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var token = "MockToken";
            var mockUser = new AspNetUser { Id = userId };

            _accountServiceMock.Setup(a => a.GetUserByIdAsync(userId.ToString())).ReturnsAsync(mockUser);
            _accountServiceMock.Setup(a => a.ConfirmEmailAsync(mockUser, token)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.ConfirmEmail(userId.ToString(), token);

            // Assert
            Assert.IsInstanceOf<RedirectResult>(result);
            var redirectResult = result as RedirectResult;
            Assert.AreEqual("https://tutor-rum-project.vercel.app/login", redirectResult.Url);
        }

        [Test]
        public async Task ConfirmEmail_ShouldReturnBadRequest_WhenUserIsNotFound()
        {
            // Arrange
            var userId = "nonExistentUserId";
            var token = "MockToken";
            _accountServiceMock.Setup(a => a.GetUserByIdAsync(userId)).ReturnsAsync((AspNetUser)null);

            // Act
            var result = await _controller.ConfirmEmail(userId, token);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            Assert.AreEqual("User not found.", (result as BadRequestObjectResult).Value);
        }

        [Test]
        public async Task ForgotPassword_ShouldReturnOk_WhenUserIsFound()
        {
            // Arrange
            var model = new ForgotPasswordModel { Email = "test@example.com" };
            var mockUser = new AspNetUser { Email = model.Email };

            _accountServiceMock.Setup(a => a.GetUserByEmailAsync(model.Email)).ReturnsAsync(mockUser);
            _accountServiceMock.Setup(a => a.GeneratePasswordResetTokenAsync(mockUser)).ReturnsAsync("MockToken");
            _emailServiceMock.Setup(e => e.SendEmailAsync(mockUser.Email, It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ForgotPassword(model);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual("Đường link đặt lại mật khẩu đã được gửi qua email.", ((ApiResponse<object>)okResult.Value).Message);
        }

        [Test]
        public async Task ForgotPassword_ShouldReturnBadRequest_WhenUserIsNotFound()
        {
            // Arrange
            var model = new ForgotPasswordModel { Email = "nonExistent@example.com" };
            _accountServiceMock.Setup(a => a.GetUserByEmailAsync(model.Email)).ReturnsAsync((AspNetUser)null);

            // Act
            var result = await _controller.ForgotPassword(model);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            Assert.AreEqual("Không tìm thấy người dùng với email này.", (result as BadRequestObjectResult).Value);
        }

        [Test]
        public async Task ResetPassword_ShouldReturnOk_WhenResetIsSuccessful()
        {
            // Arrange
            var model = new ResetPasswordModel { Email = "test@example.com", Token = "MockToken", NewPassword = "NewPassword123" };
            var mockUser = new AspNetUser { Email = model.Email };

            _accountServiceMock.Setup(a => a.GetUserByEmailAsync(model.Email)).ReturnsAsync(mockUser);
            _accountServiceMock.Setup(a => a.ResetPasswordAsync(mockUser, model.Token, model.NewPassword)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.ResetPassword(model);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual("Đổi mật khẩu thành công", ((ApiResponse<object>)okResult.Value).Message);
        }

        [Test]
        public async Task ResetPassword_ShouldReturnBadRequest_WhenUserIsNotFound()
        {
            // Arrange
            var model = new ResetPasswordModel { Email = "nonExistent@example.com", Token = "MockToken", NewPassword = "NewPassword123" };
            _accountServiceMock.Setup(a => a.GetUserByEmailAsync(model.Email)).ReturnsAsync((AspNetUser)null);

            // Act
            var result = await _controller.ResetPassword(model);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            Assert.AreEqual("User not found.", (result as BadRequestObjectResult).Value);
        }

        [Test]
        public async Task ResetPassword_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var model = new ResetPasswordModel { Email = "test@example.com", Token = "MockToken", NewPassword = "" }; // Invalid state due to empty password
            _controller.ModelState.AddModelError("NewPassword", "The New Password field is required.");

            // Act
            var result = await _controller.ResetPassword(model);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
        }

        [Test]
        public async Task ResetPassword_ShouldReturnBadRequest_WhenResetFails()
        {
            // Arrange
            var model = new ResetPasswordModel { Email = "test@example.com", Token = "MockToken", NewPassword = "NewPassword123" };
            var mockUser = new AspNetUser { Email = model.Email };

            _accountServiceMock.Setup(a => a.GetUserByEmailAsync(model.Email)).ReturnsAsync(mockUser);
            _accountServiceMock.Setup(a => a.ResetPasswordAsync(mockUser, model.Token, model.NewPassword)).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Reset failed" }));

            // Act
            var result = await _controller.ResetPassword(model);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            Assert.AreEqual("Password reset failed.", (result as BadRequestObjectResult).Value);
        }


        [Test]
        public async Task SignOut_ShouldReturnOk_WhenSignedOutSuccessfully()
        {
            // Act
            var result = await _controller.SignOut();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual("You have been signed out.", okResult.Value);
        }

        [Test]
        public async Task ViewAllAccounts_ShouldReturnOk_WhenUserIsAdmin()
        {
            // Arrange
            var mockUser = new AspNetUser { Id = Guid.NewGuid(), Email = "admin@example.com" };
            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(mockUser);
            _userManagerMock.Setup(um => um.IsInRoleAsync(mockUser, AccountRoles.Admin)).ReturnsAsync(true);

            var mockAccounts = new List<ViewAccount>
        {
            new ViewAccount {  },
            new ViewAccount {  }
        };
            _accountServiceMock.Setup(a => a.GetAllAccountsAsync()).ReturnsAsync(mockAccounts);

            // Act
            var result = await _controller.ViewAllAccounts();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsInstanceOf<ApiResponse<IEnumerable<ViewAccount>>>(okResult.Value);
            var response = okResult.Value as ApiResponse<IEnumerable<ViewAccount>>;
            Assert.AreEqual(mockAccounts.Count, response.Data.Count());
        }

        [Test]
        public async Task ViewAllAccounts_ShouldReturnUnauthorized_WhenUserIsNotAdmin()
        {
            // Arrange
            var mockUser = new AspNetUser { Id = Guid.NewGuid(), Email = "user@example.com" };
            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(mockUser);
            _userManagerMock.Setup(um => um.IsInRoleAsync(mockUser, AccountRoles.Admin)).ReturnsAsync(false);

            // Act
            var result = await _controller.ViewAllAccounts();

            // Assert
            Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.AreEqual("Bạn không có quyền Admin.", ((ApiResponse<IEnumerable<ViewAccount>>)unauthorizedResult.Value).Message);
        }

        [Test]
        public async Task ViewAllAccounts_ShouldReturnNotFound_WhenNoAccountsExist()
        {
            // Arrange
            var mockUser = new AspNetUser { Id = Guid.NewGuid(), Email = "admin@example.com" };
            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(mockUser);
            _userManagerMock.Setup(um => um.IsInRoleAsync(mockUser, AccountRoles.Admin)).ReturnsAsync(true);
            _accountServiceMock.Setup(a => a.GetAllAccountsAsync()).ReturnsAsync(new List<ViewAccount>());

            // Act
            var result = await _controller.ViewAllAccounts();

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("Không có tài khoản nào trong hệ thống.", ((ApiResponse<IEnumerable<ViewAccount>>)notFoundResult.Value).Message);
        }

        [Test]
        public async Task ViewAllAccounts_ShouldReturnServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var mockUser = new AspNetUser { Id = Guid.NewGuid(), Email = "admin@example.com" };
            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(mockUser);
            _userManagerMock.Setup(um => um.IsInRoleAsync(mockUser, AccountRoles.Admin)).ReturnsAsync(true);
            _accountServiceMock.Setup(a => a.GetAllAccountsAsync()).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.ViewAllAccounts();

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
        }

        [Test]
        public async Task SendEmailConfirmation_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            _accountServiceMock.Setup(x => x.GetUserByIdAsync(It.IsAny<string>())).ReturnsAsync((AspNetUser)null);

            // Act
            var result = await _controller.SendEmailConfirmation("invalidUserId");

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task SendEmailConfirmation_ShouldReturnServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _accountServiceMock.Setup(x => x.GetUserByIdAsync(It.IsAny<string>())).ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _controller.SendEmailConfirmation("validUserId");

            // Assert
            var statusResult = result as ObjectResult;
            Assert.AreEqual(500, statusResult?.StatusCode);
        }

        [Test]
        public async Task SignIn_ShouldReturnUnauthorized_WhenUserDoesNotExist()
        {
            // Arrange
            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((AspNetUser)null);

            // Act
            var result = await _controller.SignIn(new SignInModel { Email = "nonexistent@example.com" });

            // Assert
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }

        [Test]
        public async Task SignIn_ShouldReturnBadRequest_WhenAccountIsLocked()
        {
            // Arrange
            var user = new AspNetUser { LockoutEnabled = false };
            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);

            // Act
            var result = await _controller.SignIn(new SignInModel { Email = "locked@example.com" });

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task SignIn_ShouldReturnOk_WhenSignInIsSuccessful()
        {
            // Arrange
            var user = new AspNetUser { LockoutEnabled = true };
            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            _accountServiceMock.Setup(x => x.SignInAsync(It.IsAny<SignInModel>())).ReturnsAsync(new SignInResponseDto());

            // Act
            var result = await _controller.SignIn(new SignInModel { Email = "test@example.com" });

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task BlockUserAsync_ShouldReturnUnauthorized_WhenUserIsNotAdmin()
        {
            // Arrange
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((AspNetUser)null);

            // Act
            var result = await _controller.BlockUserAsync(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            Assert.AreEqual(401, (result as ObjectResult)?.StatusCode); // Unauthorized
        }

        [Test]
        public async Task BlockUserAsync_ShouldReturnBadRequest_WhenAdminBlocksSelf()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var mockUser = new AspNetUser { Id = userId, Email = "admin@example.com" };
            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(mockUser);
            _userManagerMock.Setup(um => um.IsInRoleAsync(mockUser, AccountRoles.Admin)).ReturnsAsync(true);

            // Act
            var result = await _controller.BlockUserAsync(userId);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task BlockUserAsync_ShouldReturnOk_WhenUserBlockedSuccessfully()
        {
            // Arrange
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);
            _userManagerMock.Setup(x => x.IsInRoleAsync(It.IsAny<AspNetUser>(), AccountRoles.Admin)).ReturnsAsync(true);

            // Act
            var result = await _controller.BlockUserAsync(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task BlockUserAsync_ShouldReturnServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);
            _userManagerMock.Setup(x => x.IsInRoleAsync(It.IsAny<AspNetUser>(), AccountRoles.Admin)).ReturnsAsync(true);
            _accountServiceMock.Setup(x => x.BlockUserAsync(It.IsAny<Guid>())).ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _controller.BlockUserAsync(Guid.NewGuid());

            // Assert
            var statusResult = result as ObjectResult;
            Assert.AreEqual(500, statusResult?.StatusCode); // Server error
        }

       

    }
}
