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
using TutoRum.Data.Models;
using TutoRum.Services.IService;
using TutoRum.Services.Service;
using TutoRum.Services.ViewModels;
using static TutoRum.FE.Common.Url;

namespace TutoRum.UnitTests.ServiceUnitTest
{
    [TestFixture]
    public class PaymentRequestServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IEmailService> _mockEmailService;
        private Mock<UserManager<AspNetUser>> _mockUserManager;
        private Mock<INotificationService> _mockNotificationService;
        private PaymentRequestService _paymentRequestService;

        [SetUp]
        public void SetUp()
        {
            // Mocking dependencies
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockEmailService = new Mock<IEmailService>();
            _mockNotificationService = new Mock<INotificationService>();

            _mockUserManager = new Mock<UserManager<AspNetUser>>(
                Mock.Of<IUserStore<AspNetUser>>(),
                null, null, null, null, null, null, null, null);

            var user = new AspNetUser { Id = Guid.NewGuid(), Fullname = "Admin User" };

            _mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockEmailService.Setup(em => em.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            // Create the PaymentRequestService instance, injecting mocks
            _paymentRequestService = new PaymentRequestService(
                _mockUnitOfWork.Object,
                _mockEmailService.Object,
                _mockUserManager.Object,
                _mockNotificationService.Object
            );
        }

        [Test]
        public async Task CreatePaymentRequestAsync_ShouldThrowArgumentException_WhenRequestDtoIsNull()
        {
            // Act & Assert
            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                _paymentRequestService.CreatePaymentRequestAsync(null, Mock.Of<ClaimsPrincipal>())
            );
            Assert.AreEqual("Invalid payment request data.", exception.Message);
        }

        [Test]
        public async Task CreatePaymentRequestAsync_ShouldThrowArgumentException_WhenAmountIsZeroOrNegative()
        {
            var requestDto = new CreatePaymentRequestDTO
            {
                Amount = 0,
                BankCode = "BankCode",
                AccountNumber = "123456789",
                FullName = "Test User"
            };

            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                _paymentRequestService.CreatePaymentRequestAsync(requestDto, Mock.Of<ClaimsPrincipal>())
            );
            Assert.AreEqual("Invalid payment request data.", exception.Message);
        }

        [Test]
        public async Task CreatePaymentRequestAsync_ShouldThrowException_WhenUserNotFound()
        {
            var requestDto = new CreatePaymentRequestDTO
            {
                Amount = 100,
                BankCode = "BankCode",
                AccountNumber = "123456789",
                FullName = "Test User"
            };

            _mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((AspNetUser)null);

            var exception = Assert.ThrowsAsync<Exception>(() =>
                _paymentRequestService.CreatePaymentRequestAsync(requestDto, Mock.Of<ClaimsPrincipal>())
            );
            Assert.AreEqual("User not found.", exception.Message);
        }

        [Test]
        public async Task CreatePaymentRequestAsync_ShouldThrowException_WhenTutorNotFound()
        {
            var requestDto = new CreatePaymentRequestDTO
            {
                Amount = 100,
                BankCode = "BankCode",
                AccountNumber = "123456789",
                FullName = "Test User"
            };

            var user = new ClaimsPrincipal();
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };
            _mockUserManager.Setup(m => m.GetUserAsync(user)).ReturnsAsync(currentUser);

            _mockUnitOfWork.Setup(u => u.Tutors.GetSingleByGuId(currentUser.Id)).Returns((Tutor)null);

            var exception = Assert.ThrowsAsync<Exception>(() =>
                _paymentRequestService.CreatePaymentRequestAsync(requestDto, user)
            );
            Assert.AreEqual("Tutor not found.", exception.Message);
        }

        [Test]
        public async Task CreatePaymentRequestAsync_ShouldThrowException_WhenInsufficientBalance()
        {
            var requestDto = new CreatePaymentRequestDTO
            {
                Amount = 100,
                BankCode = "BankCode",
                AccountNumber = "123456789",
                FullName = "Test User"
            };

            var user = new ClaimsPrincipal();
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };
            _mockUserManager.Setup(m => m.GetUserAsync(user)).ReturnsAsync(currentUser);

            var tutor = new Tutor { TutorId = Guid.NewGuid(), Balance = 50 }; // Insufficient balance
            _mockUnitOfWork.Setup(u => u.Tutors.GetSingleByGuId(currentUser.Id)).Returns(tutor);

            var exception = Assert.ThrowsAsync<Exception>(() =>
                _paymentRequestService.CreatePaymentRequestAsync(requestDto, user)
            );
            Assert.AreEqual("Insufficient balance.", exception.Message);
        }

        [Test]
        public async Task CreatePaymentRequestAsync_ShouldCreatePaymentRequest_WhenValidData()
        {
            // Arrange
            var requestDto = new CreatePaymentRequestDTO
            {
                Amount = 100,
                BankCode = "BankCode",
                AccountNumber = "123456789",
                FullName = "Test User"
            };

            var user = new ClaimsPrincipal();
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };

            _mockUserManager.Setup(m => m.GetUserAsync(user)).ReturnsAsync(currentUser);

            var tutor = new Tutor { TutorId = Guid.NewGuid(), Balance = 200 }; // Sufficient balance
            _mockUnitOfWork.Setup(u => u.Tutors.GetSingleByGuId(currentUser.Id)).Returns(tutor);
            _mockUnitOfWork.Setup(u => u.PaymentRequest.Add(It.IsAny<PaymentRequest>()));
            _mockNotificationService.Setup(n => n.SendNotificationAsync(It.IsAny<NotificationRequestDto>(), It.IsAny<bool>()));

            // Act
            var result = await _paymentRequestService.CreatePaymentRequestAsync(requestDto, user);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(100, result.Amount);
            Assert.AreEqual("Pending", result.Status);
        }

        [Test]
        public async Task SendPaymentRequestConfirmationEmailAsync_UserNotFound_ThrowsException()
        {
            // Arrange
            ClaimsPrincipal user = new ClaimsPrincipal(); // Mocked user
            PaymentRequest paymentRequest = new PaymentRequest { Amount = 1000000, BankCode = "123", AccountNumber = "456" };
            string confirmationUrl = "http://example.com";

            _mockUserManager
                .Setup(x => x.GetUserAsync(user))
                .ReturnsAsync((AspNetUser)null); // Simulate user not found

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() =>
                _paymentRequestService.SendPaymentRequestConfirmationEmailAsync(user, paymentRequest, confirmationUrl));
            Assert.AreEqual("User not found.", ex.Message);
        }

        [Test]
        public async Task ConfirmPaymentRequest_InvalidRequestOrToken_ThrowsException()
        {
            // Arrange
            int requestId = 1;
            Guid token = Guid.NewGuid();

            _mockUnitOfWork
                .Setup(x => x.PaymentRequest.FindAsync(It.IsAny<Expression<Func<PaymentRequest, bool>>>()))
                .ReturnsAsync((PaymentRequest)null); // Simulate payment request not found

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() =>
                _paymentRequestService.ConfirmPaymentRequest(requestId, token));
            Assert.AreEqual("Invalid request or token.", ex.Message);
        }

        [Test]
        public async Task ConfirmPaymentRequest_ValidRequest_UpdatesStatusAndCommits()
        {
            // Arrange
            int requestId = 1;
            Guid token = Guid.NewGuid();
            var paymentRequest = new PaymentRequest { PaymentRequestId = requestId, Token = token, VerificationStatus = "Pending" };

            _mockUnitOfWork
                .Setup(x => x.PaymentRequest.FindAsync(It.IsAny<Expression<Func<PaymentRequest, bool>>>()))
                .ReturnsAsync(paymentRequest);

            // Act
            var result = await _paymentRequestService.ConfirmPaymentRequest(requestId, token);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual("Confirmed", paymentRequest.VerificationStatus);

            _mockUnitOfWork.Verify(x => x.PaymentRequest.Update(paymentRequest), Times.Once);
            _mockUnitOfWork.Verify(x => x.CommitAsync(), Times.Once);
        }

        [Test]
        public async Task GetListPaymentRequestsByTutorAsync_UserNotFound_ThrowsException()
        {
            // Arrange
            ClaimsPrincipal user = new ClaimsPrincipal();

            _mockUserManager
                .Setup(x => x.GetUserAsync(user))
                .ReturnsAsync((AspNetUser)null); // Simulate user not found

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() =>
                _paymentRequestService.GetListPaymentRequestsByTutorAsync(user));
            Assert.AreEqual("User not found.", ex.Message);
        }

        [Test]
        public void GetMultiPaging_ShouldReturnPagedData()
        {
            // Arrange
            var paymentRequests = new List<PaymentRequest>
    {
        new PaymentRequest { PaymentRequestId = 1, Amount = 100000 },
        new PaymentRequest { PaymentRequestId = 2, Amount = 200000 },
        new PaymentRequest { PaymentRequestId = 3, Amount = 300000 }
    };

            int totalItems = paymentRequests.Count; // Total items in the result

            _mockUnitOfWork
                .Setup(x => x.PaymentRequest.GetMultiPaging(
                    It.IsAny<Expression<Func<PaymentRequest, bool>>>(),
                    out totalItems,
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<string[]>(),
                    It.IsAny<Func<IQueryable<PaymentRequest>, IOrderedQueryable<PaymentRequest>>>()))
                .Returns((Expression<Func<PaymentRequest, bool>> filter,
                          out int total,
                          int index,
                          int size,
                          string[] includes,
                          Func<IQueryable<PaymentRequest>, IOrderedQueryable<PaymentRequest>> orderBy) =>
                {
                    total = totalItems;
                    return paymentRequests.Skip(index * size).Take(size);
                });

            // Act
            int total = 0;
            var result = _mockUnitOfWork.Object.PaymentRequest.GetMultiPaging(
                x => x.Amount > 0, // Example filter
                out total,
                index: 0,
                size: 2,
                includes: null,
                orderBy: null);

            // Assert
            Assert.AreEqual(2, result.Count()); // Page size is 2
            Assert.AreEqual(totalItems, total); // Total items in dataset
            Assert.AreEqual(1, result.First().PaymentRequestId);
        }

        [Test]
        public async Task GetListPaymentRequestsAsync_WithEmptyFilter_ShouldReturnAllRecords()
        {
            // Arrange
            var filter = new PaymentRequestFilterDTO();
            var paymentRequests = new List<PaymentRequest>
        {
            new PaymentRequest { PaymentRequestId = 1, TutorId = Guid.NewGuid(), Amount = 1000 },
            new PaymentRequest { PaymentRequestId = 2, TutorId = Guid.NewGuid(), Amount = 2000 }
        };
            int totalRecords = paymentRequests.Count;

            _mockUnitOfWork.Setup(uow => uow.PaymentRequest.GetMultiPaging(
                It.IsAny<Expression<Func<PaymentRequest, bool>>>(),
                out totalRecords,
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string[]>(),
                It.IsAny<Func<IQueryable<PaymentRequest>, IOrderedQueryable<PaymentRequest>>>()
            )).Returns(paymentRequests);

            // Act
            var result = await _paymentRequestService.GetListPaymentRequestsAsync(filter);

            // Assert
            Assert.AreEqual(2, result.Items.Count);
            Assert.AreEqual(totalRecords, result.TotalRecords);
        }

        [Test]
        public async Task GetListPaymentRequestsAsync_WithTutorNameFilter_ShouldReturnFilteredResults()
        {
            // Arrange
            var filter = new PaymentRequestFilterDTO { TutorName = "Tutor A" };
            var paymentRequests = new List<PaymentRequest>
        {
            new PaymentRequest { PaymentRequestId = 1, TutorId = Guid.NewGuid(), Amount = 1000, Tutor = new Tutor { TutorNavigation = new AspNetUser { Fullname = "Tutor A" } } },
            new PaymentRequest { PaymentRequestId = 2, TutorId = Guid.NewGuid(), Amount = 2000, Tutor = new Tutor { TutorNavigation = new AspNetUser { Fullname = "Tutor B" } } }
        };
            int totalRecords = 1;

            _mockUnitOfWork.Setup(uow => uow.PaymentRequest.GetMultiPaging(
                It.IsAny<Expression<Func<PaymentRequest, bool>>>(),
                out totalRecords,
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string[]>(),
                It.IsAny<Func<IQueryable<PaymentRequest>, IOrderedQueryable<PaymentRequest>>>()
            )).Returns(paymentRequests.Where(pr => pr.Tutor.TutorNavigation.Fullname == filter.TutorName).ToList());

            // Act
            var result = await _paymentRequestService.GetListPaymentRequestsAsync(filter);

            // Assert
            Assert.AreEqual(1, result.Items.Count);
            Assert.AreEqual(totalRecords, result.TotalRecords);
            Assert.AreEqual("Tutor A", result.Items.First().TutorName);
        }

        [Test]
        public async Task GetListPaymentRequestsAsync_WithPagination_ShouldReturnPaginatedResults()
        {
            // Arrange
            var filter = new PaymentRequestFilterDTO();
            var paymentRequests = new List<PaymentRequest>
        {
            new PaymentRequest { PaymentRequestId = 1, TutorId = Guid.NewGuid(), Amount = 1000 },
            new PaymentRequest { PaymentRequestId = 2, TutorId = Guid.NewGuid(), Amount = 2000 },
            new PaymentRequest { PaymentRequestId = 3, TutorId = Guid.NewGuid(), Amount = 3000 }
        };
            int totalRecords = paymentRequests.Count;

            _mockUnitOfWork.Setup(uow => uow.PaymentRequest.GetMultiPaging(
                It.IsAny<Expression<Func<PaymentRequest, bool>>>(),
                out totalRecords,
                1, // Page index 1
                2, // Page size 2
                It.IsAny<string[]>(),
                It.IsAny<Func<IQueryable<PaymentRequest>, IOrderedQueryable<PaymentRequest>>>()
            )).Returns(paymentRequests.Skip(2).Take(2).ToList());

            // Act
            var result = await _paymentRequestService.GetListPaymentRequestsAsync(filter, 1, 2);

            // Assert
            Assert.AreEqual(1, result.Items.Count);
            Assert.AreEqual(totalRecords, result.TotalRecords);
            Assert.AreEqual(3, result.Items.First().PaymentRequestId);
        }

        [Test]
        public async Task GetListPaymentRequestsAsync_WithNoResults_ShouldReturnEmptyList()
        {
            // Arrange
            var filter = new PaymentRequestFilterDTO { TutorName = "Non Existing Tutor" };
            var paymentRequests = new List<PaymentRequest>();
            int totalRecords = 0;

            _mockUnitOfWork.Setup(uow => uow.PaymentRequest.GetMultiPaging(
                It.IsAny<Expression<Func<PaymentRequest, bool>>>(),
                out totalRecords,
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string[]>(),
                It.IsAny<Func<IQueryable<PaymentRequest>, IOrderedQueryable<PaymentRequest>>>()
            )).Returns(paymentRequests);

            // Act
            var result = await _paymentRequestService.GetListPaymentRequestsAsync(filter);

            // Assert
            Assert.AreEqual(0, result.Items.Count);
            Assert.AreEqual(0, result.TotalRecords);
        }

        [Test]
        public async Task DeletePaymentRequest_WhenUserNotFound_ShouldThrowException()
        {
            // Arrange
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((AspNetUser)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _paymentRequestService.DeletePaymentRequest(1, new ClaimsPrincipal())
            );
            Assert.AreEqual("User not found.", ex.Message);
        }

        [Test]
        public async Task DeletePaymentRequest_WhenPaymentRequestNotFound_ShouldThrowException()
        {
            // Arrange
            var user = new AspNetUser { Id = Guid.NewGuid() };
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _mockUnitOfWork.Setup(uow => uow.PaymentRequest.FindAsync(It.IsAny<Expression<Func<PaymentRequest, bool>>>())).ReturnsAsync((PaymentRequest)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _paymentRequestService.DeletePaymentRequest(1, new ClaimsPrincipal())
            );
            Assert.AreEqual("PaymentRequest not found.", ex.Message);
        }

        [Test]
        public async Task DeletePaymentRequest_WhenUserIsNotTutorForRequest_ShouldThrowException()
        {
            // Arrange
            var user = new AspNetUser { Id = Guid.NewGuid() };
            var paymentRequest = new PaymentRequest { PaymentRequestId = 1, TutorId = Guid.NewGuid(), Amount = 1000 };

            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _mockUnitOfWork.Setup(uow => uow.PaymentRequest.FindAsync(It.IsAny<Expression<Func<PaymentRequest, bool>>>())).ReturnsAsync(paymentRequest);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _paymentRequestService.DeletePaymentRequest(1, new ClaimsPrincipal())
            );
            Assert.AreEqual("User do not have role for this payment request", ex.Message);
        }

        [Test]
        public async Task DeletePaymentRequest_WhenPaymentRequestIsNotPending_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var TutorId = Guid.NewGuid();
            var user = new AspNetUser { Id = TutorId };
            var paymentRequest = new PaymentRequest { PaymentRequestId = 1, TutorId = TutorId, Amount = 1000, Status = "Completed" };

            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _mockUnitOfWork.Setup(uow => uow.PaymentRequest.FindAsync(It.IsAny<Expression<Func<PaymentRequest, bool>>>())).ReturnsAsync(paymentRequest);

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _paymentRequestService.DeletePaymentRequest(1, new ClaimsPrincipal())
            );
            Assert.AreEqual("Only pending payment requests can be deleted.", ex.Message);
        }

        [Test]
        public async Task DeletePaymentRequest_WhenRequestCanBeDeleted_ShouldReturnTrue()
        {
            // Arrange
            var TutorId = Guid.NewGuid();

            var user = new AspNetUser { Id = TutorId };
            var paymentRequest = new PaymentRequest { PaymentRequestId = 1, TutorId = TutorId, Amount = 1000, Status = "Pending" };
            var tutor = new Data.Models.Tutor { TutorId = TutorId, Balance = 5000 };

            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _mockUnitOfWork.Setup(uow => uow.PaymentRequest.FindAsync(It.IsAny<Expression<Func<PaymentRequest, bool>>>())).ReturnsAsync(paymentRequest);
            _mockUnitOfWork.Setup(uow => uow.Tutors.GetSingleByGuId(It.IsAny<Guid>())).Returns(tutor);

            // Act
            var result = await _paymentRequestService.DeletePaymentRequest(1, new ClaimsPrincipal());

            // Assert
            Assert.IsTrue(result);
            _mockUnitOfWork.Verify(uow => uow.Tutors.Update(It.IsAny<Tutor>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.PaymentRequest.Update(It.IsAny<PaymentRequest>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
        }

        [Test]
        public async Task GetListPaymentRequestsByTutorAsync_WhenUserNotFound_ShouldThrowException()
        {
            // Arrange
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((AspNetUser)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _paymentRequestService.GetListPaymentRequestsByTutorAsync(new ClaimsPrincipal(),  0, 20)
            );
            Assert.AreEqual("User not found.", ex.Message);
        }

        [Test]
        public async Task GetListPaymentRequestsByTutorAsync_WhenUserIsNotTutor_ShouldThrowException()
        {
            // Arrange
            var user = new AspNetUser { Id = Guid.NewGuid() };
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _mockUserManager.Setup(um => um.IsInRoleAsync(user, AccountRoles.Tutor)).ReturnsAsync(false);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                 await _paymentRequestService.GetListPaymentRequestsByTutorAsync(new ClaimsPrincipal(), 0, 20)

            );
            Assert.AreEqual("User is not tutor", ex.Message);
        }

        [Test]
        public async Task GetListPaymentRequestsByTutorAsync_WhenPaymentRequestsExist_ShouldReturnPagedResult()
        {
            // Arrange
            var user = new AspNetUser { Id = Guid.NewGuid() };
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _mockUserManager.Setup(um => um.IsInRoleAsync(user, AccountRoles.Tutor)).ReturnsAsync(true);

            var paymentRequests = new List<PaymentRequest>
        {
            new PaymentRequest { PaymentRequestId = 1, TutorId = user.Id, Amount = 1000, Status = "Pending" }
        };

            _mockUnitOfWork.Setup(uow => uow.PaymentRequest.GetMultiPaging(
                    It.IsAny<Expression<Func<PaymentRequest, bool>>>(),
                    out It.Ref<int>.IsAny, // You can mock the 'out' parameter separately
                    It.IsAny<int>(), // Provide an explicit value for the 'index'
                    It.IsAny<int>(), // Provide an explicit value for the 'size'
                    It.IsAny<string[]>(), // Provide an explicit value for the 'includes'
               It.IsAny<Func<IQueryable<PaymentRequest>, IOrderedQueryable<PaymentRequest>>>() // The 'orderBy' parameter
                )).Returns(paymentRequests.AsQueryable());

            // Act
            var result = await _paymentRequestService.GetListPaymentRequestsByTutorAsync(new ClaimsPrincipal(), 0, 20);

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        public async Task GetListPaymentRequestsByTutorAsync_WhenNoPaymentRequestsExist_ShouldReturnEmptyResult()
        {
            // Arrange
            var user = new AspNetUser { Id = Guid.NewGuid() };
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _mockUserManager.Setup(um => um.IsInRoleAsync(user, AccountRoles.Tutor)).ReturnsAsync(true);

            var paymentRequests = new List<PaymentRequest>();
            _mockUnitOfWork.Setup(uow => uow.PaymentRequest.GetMultiPaging(
                    It.IsAny<Expression<Func<PaymentRequest, bool>>>(),
                    out It.Ref<int>.IsAny, // You can mock the 'out' parameter separately
                    It.IsAny<int>(), // Provide an explicit value for the 'index'
                    It.IsAny<int>(), // Provide an explicit value for the 'size'
                    It.IsAny<string[]>(), // Provide an explicit value for the 'includes'
               It.IsAny<Func<IQueryable<PaymentRequest>, IOrderedQueryable<PaymentRequest>>>() // The 'orderBy' parameter
                )).Returns(paymentRequests.AsQueryable());

            // Act
            var result = await _paymentRequestService.GetListPaymentRequestsByTutorAsync(new ClaimsPrincipal(), 0, 20);

            // Assert
            Assert.AreEqual(0, result.Items.Count);
            Assert.AreEqual(0, result.TotalRecords);
        }

        [Test]
        public async Task ApprovePaymentRequestAsync_WhenPaymentRequestNotFound_ShouldThrowException()
        {
            // Arrange
            _mockUnitOfWork.Setup(uow => uow.PaymentRequest.GetSingleById(It.IsAny<int>())).Returns((PaymentRequest)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _paymentRequestService.ApprovePaymentRequestAsync(1, new ClaimsPrincipal())
            );
            Assert.AreEqual("Payment request not found.", ex.Message);
        }

        [Test]
        public async Task ApprovePaymentRequestAsync_WhenPaymentRequestAlreadyProcessed_ShouldThrowException()
        {
            // Arrange
            var paymentRequest = new PaymentRequest { PaymentRequestId = 1, IsPaid = true, Status = "Approved" };
            _mockUnitOfWork.Setup(uow => uow.PaymentRequest.GetSingleById(It.IsAny<int>())).Returns(paymentRequest);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _paymentRequestService.ApprovePaymentRequestAsync(1, new ClaimsPrincipal())
            );
            Assert.AreEqual("Payment request has already been processed.", ex.Message);
        }

        [Test]
        public async Task ApprovePaymentRequestAsync_WhenUserNotFound_ShouldThrowException()
        {
            // Arrange
            var paymentRequest = new PaymentRequest { PaymentRequestId = 1, Status = "Pending" };
            _mockUnitOfWork.Setup(uow => uow.PaymentRequest.GetSingleById(It.IsAny<int>())).Returns(paymentRequest);
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((AspNetUser)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _paymentRequestService.ApprovePaymentRequestAsync(1, new ClaimsPrincipal())
            );
            Assert.AreEqual("User not found.", ex.Message);
        }

        [Test]
        public async Task ApprovePaymentRequestAsync_WhenPaymentRequestApproved_ShouldReturnTrue()
        {
            // Arrange
            var user = new AspNetUser { Id = Guid.NewGuid(), Fullname = "Admin User" };
            var paymentRequest = new PaymentRequest { PaymentRequestId = 1, TutorId = Guid.NewGuid(), Status = "Pending", Amount = 1000 };
            var tutor = new Tutor { TutorId = Guid.NewGuid() };

            _mockUnitOfWork.Setup(uow => uow.PaymentRequest.GetSingleById(It.IsAny<int>())).Returns(paymentRequest);
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _mockUnitOfWork.Setup(uow => uow.Tutors.GetSingleByGuId(It.IsAny<Guid>())).Returns(tutor);
            _mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockEmailService.Setup(em => em.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            // Act
            var result = await _paymentRequestService.ApprovePaymentRequestAsync(1, new ClaimsPrincipal());

            // Assert
            Assert.IsTrue(result);
            _mockUnitOfWork.Verify(uow => uow.PaymentRequest.Update(It.IsAny<PaymentRequest>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
        }

        [Test]
        public async Task ApprovePaymentRequestAsync_WhenPaymentRequestIsNotPaid_ShouldUpdateCorrectly()
        {
            // Arrange
            var user = new AspNetUser { Id = Guid.NewGuid(), Fullname = "Admin User" , Email = "tets123@gmail.com"};
            var paymentRequest = new PaymentRequest { PaymentRequestId = 1, TutorId = Guid.NewGuid(), Status = "Pending", IsPaid = false };
            var tutor = new Tutor { TutorId = paymentRequest.TutorId };

            _mockUnitOfWork.Setup(uow => uow.PaymentRequest.GetSingleById(It.IsAny<int>())).Returns(paymentRequest);
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _mockUnitOfWork.Setup(uow => uow.Tutors.GetSingleByGuId(It.IsAny<Guid>())).Returns(tutor);
            _mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockEmailService.Setup(em => em.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));


            // Act
            var result = await _paymentRequestService.ApprovePaymentRequestAsync(1, new ClaimsPrincipal());

            // Assert
            Assert.IsTrue(result);
            _mockUnitOfWork.Verify(uow => uow.PaymentRequest.Update(It.IsAny<PaymentRequest>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
        }
        [Test]
        public async Task RejectPaymentRequestAsync_WhenPaymentRequestNotFound_ShouldThrowException()
        {
            // Arrange
            _mockUnitOfWork.Setup(uow => uow.PaymentRequest.GetSingleById(It.IsAny<int>()))
                           .Returns((PaymentRequest)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _paymentRequestService.RejectPaymentRequestAsync(1, "Reason", new ClaimsPrincipal())
            );
            Assert.AreEqual("Payment request not found.", ex.Message);
        }

        [Test]
        public async Task RejectPaymentRequestAsync_WhenPaymentRequestAlreadyProcessed_ShouldThrowException()
        {
            // Arrange
            var paymentRequest = new PaymentRequest { PaymentRequestId = 1, Status = "Approved" };
            _mockUnitOfWork.Setup(uow => uow.PaymentRequest.GetSingleById(It.IsAny<int>()))
                           .Returns(paymentRequest);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _paymentRequestService.RejectPaymentRequestAsync(1, "Reason", new ClaimsPrincipal())
            );
            Assert.AreEqual("Payment request has already been processed.", ex.Message);
        }

        [Test]
        public async Task RejectPaymentRequestAsync_WhenUserNotFound_ShouldThrowException()
        {
            // Arrange
            var paymentRequest = new PaymentRequest { PaymentRequestId = 1, Status = "Pending" };
            _mockUnitOfWork.Setup(uow => uow.PaymentRequest.GetSingleById(It.IsAny<int>()))
                           .Returns(paymentRequest);
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                            .ReturnsAsync((AspNetUser)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _paymentRequestService.RejectPaymentRequestAsync(1, "Reason", new ClaimsPrincipal())
            );
            Assert.AreEqual("User not found.", ex.Message);
        }

        [Test]
        public async Task RejectPaymentRequestAsync_WhenValid_ShouldRejectPaymentRequest()
        {
            // Arrange
            var adminUser = new AspNetUser { Id = Guid.NewGuid(), Fullname = "Admin" };
            var tutor = new Tutor {  TutorId = Guid.NewGuid(), Balance = 1000 };
            var paymentRequest = new PaymentRequest
            {
                PaymentRequestId = 1,
                TutorId = tutor.TutorId,
                Status = "Pending",
                Amount = 500
            };

            _mockUnitOfWork.Setup(uow => uow.PaymentRequest.GetSingleById(It.IsAny<int>())).Returns(paymentRequest);
            _mockUnitOfWork.Setup(uow => uow.Tutors.GetSingleByGuId(paymentRequest.TutorId)).Returns(tutor);
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(adminUser);

            // Act
            var result = await _paymentRequestService.RejectPaymentRequestAsync(1, "Invalid details", new ClaimsPrincipal());

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual("Rejected", paymentRequest.Status);
            Assert.AreEqual(1500, tutor.Balance);
            _mockUnitOfWork.Verify(uow => uow.PaymentRequest.Update(paymentRequest), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
            _mockEmailService.Verify(es => es.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task UpdatePaymentRequestByIdAsync_WhenUpdateDtoIsNull_ShouldThrowArgumentException()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _paymentRequestService.UpdatePaymentRequestByIdAsync(1, null, new ClaimsPrincipal())
            );
            Assert.AreEqual("Update data cannot be null.", ex.Message);
        }

        [Test]
        public async Task UpdatePaymentRequestByIdAsync_WhenPaymentRequestNotFound_ShouldThrowException()
        {
            // Arrange
            _mockUnitOfWork.Setup(uow => uow.PaymentRequest.GetSingleById(It.IsAny<int>()))
                           .Returns((PaymentRequest)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _paymentRequestService.UpdatePaymentRequestByIdAsync(1, new UpdatePaymentRequestDTO(), new ClaimsPrincipal())
            );
            Assert.AreEqual("Payment request not found.", ex.Message);
        }

        [Test]
        public async Task UpdatePaymentRequestByIdAsync_WhenUserNotFound_ShouldThrowException()
        {
            // Arrange
            var paymentRequest = new PaymentRequest { PaymentRequestId = 1 };
            _mockUnitOfWork.Setup(uow => uow.PaymentRequest.GetSingleById(It.IsAny<int>())).Returns(paymentRequest);
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((AspNetUser)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _paymentRequestService.UpdatePaymentRequestByIdAsync(1, new UpdatePaymentRequestDTO(), new ClaimsPrincipal())
            );
            Assert.AreEqual("User not found.", ex.Message);
        }

        [Test]
        public async Task UpdatePaymentRequestByIdAsync_WhenValid_ShouldUpdatePaymentRequest()
        {
            // Arrange
            var adminUser = new AspNetUser { Id = Guid.NewGuid(), Fullname = "Admin" };
            var paymentRequest = new PaymentRequest { PaymentRequestId = 1 };
            var updateDto = new UpdatePaymentRequestDTO
            {
                BankCode = "XYZ",
                AccountNumber = "123456789",
                FullName = "Updated User",
                Amount = 1000
            };

            _mockUnitOfWork.Setup(uow => uow.PaymentRequest.GetSingleById(It.IsAny<int>())).Returns(paymentRequest);
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(adminUser);

            // Act
            var result = await _paymentRequestService.UpdatePaymentRequestByIdAsync(1, updateDto, new ClaimsPrincipal());

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual("XYZ", paymentRequest.BankCode);
            Assert.AreEqual("123456789", paymentRequest.AccountNumber);
            Assert.AreEqual("Updated User", paymentRequest.FullName);
            Assert.AreEqual(1000, paymentRequest.Amount);
            Assert.AreEqual(adminUser.Id, paymentRequest.UpdatedBy);
            _mockUnitOfWork.Verify(uow => uow.PaymentRequest.Update(paymentRequest), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
        }

        [Test]
        public void GeneratePaymentRequestTokenAsync_WhenPaymentRequestNotFound_ShouldThrowException()
        {
            // Arrange
            _mockUnitOfWork.Setup(uow => uow.PaymentRequest.FindAsync(It.IsAny<Expression<Func<PaymentRequest, bool>>>()))
                .ReturnsAsync((PaymentRequest)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _paymentRequestService.GeneratePaymentRequestTokenAsync(1)
            );

            Assert.AreEqual("Payment request not found.", ex.Message);
        }

        [Test]
        public async Task GeneratePaymentRequestTokenAsync_WhenPaymentRequestExists_ShouldGenerateTokenAndSave()
        {
            // Arrange
            var paymentRequest = new PaymentRequest { PaymentRequestId = 1 };
            _mockUnitOfWork.Setup(uow => uow.PaymentRequest.FindAsync(It.IsAny<Expression<Func<PaymentRequest, bool>>>()))
                .ReturnsAsync(paymentRequest);

            // Act
            var token = await _paymentRequestService.GeneratePaymentRequestTokenAsync(1);

            // Assert
            Assert.AreNotEqual(Guid.Empty, token);
            Assert.AreEqual(token, paymentRequest.Token);
            _mockUnitOfWork.Verify(uow => uow.PaymentRequest.Update(paymentRequest), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
        }


        [Test]
        public void SendPaymentRequestConfirmationEmailAsync_WhenUserNotFound_ShouldThrowException()
        {
            // Arrange
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                            .ReturnsAsync((AspNetUser)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _paymentRequestService.SendPaymentRequestConfirmationEmailAsync(new ClaimsPrincipal(), new PaymentRequest(), "http://example.com")
            );

            Assert.AreEqual("User not found.", ex.Message);
        }

        [Test]
        public void SendPaymentRequestConfirmationEmailAsync_WhenTutorNotFound_ShouldThrowException()
        {
            // Arrange
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                            .ReturnsAsync(currentUser);

            _mockUnitOfWork.Setup(uow => uow.Tutors.GetSingleByGuId(currentUser.Id))
                           .Returns((Tutor)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _paymentRequestService.SendPaymentRequestConfirmationEmailAsync(new ClaimsPrincipal(), new PaymentRequest(), "http://example.com")
            );

            Assert.AreEqual("Tutor not found.", ex.Message);
        }

        [Test]
        public async Task SendPaymentRequestConfirmationEmailAsync_WhenValid_ShouldSendEmail()
        {
            // Arrange
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };
            var tutor = new Tutor
            {
                TutorNavigation = new AspNetUser
                {
                    Fullname = "Tutor Name",
                    Email = "tutor@example.com"
                }
            };
            var paymentRequest = new PaymentRequest
            {
                Amount = 1000,
                BankCode = "XYZ",
                AccountNumber = "123456789"
            };
            var confirmationUrl = "http://example.com";

            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                            .ReturnsAsync(currentUser);
            _mockUnitOfWork.Setup(uow => uow.Tutors.GetSingleByGuId(currentUser.Id))
                           .Returns(tutor);

            // Act
            var result =  _paymentRequestService.SendPaymentRequestConfirmationEmailAsync(new ClaimsPrincipal(), paymentRequest, confirmationUrl);

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
