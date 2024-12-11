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
using TutoRum.Services.IService;
using TutoRum.Services.Service;
namespace TutoRum.UnitTests.ServiceUnitTest
{
    [TestFixture]
    public class BillServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<UserManager<AspNetUser>> _userManagerMock;
        private Mock<IEmailService> _emailServiceMock;
        private BillService _billService;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _userManagerMock = new Mock<UserManager<AspNetUser>>(
                new Mock<IUserStore<AspNetUser>>().Object,
                null, null, null, null, null, null, null, null
            );
            _emailServiceMock = new Mock<IEmailService>();

            var billingEntries = new List<BillingEntry>
        {
            new BillingEntry { BillingEntryId = 1, TotalAmount = 500 },
            new BillingEntry { BillingEntryId = 2, TotalAmount = 300 }
        };

            var newBill = new Bill
            {
                BillId = 1,
                TotalBill = 800,
                Deduction = 40,
                Status = "Pending",
            };

            _unitOfWorkMock.Setup(uow => uow.BillingEntry.GetMultiAsQueryable(It.IsAny<Expression<Func<BillingEntry, bool>>>(), It.IsAny<string[]>()))
                .Returns(billingEntries.AsQueryable);

            var newBills = new List<Bill>
            {
                new Bill
                {
                    BillId = 1,
                TotalBill = 800,
                Deduction = 40,
                Status = "Pending",
                }
            };

            var totalRecords = 1;
            _unitOfWorkMock.Setup(uow => uow.Bill.GetMultiPaging(
                It.IsAny<Expression<Func<Bill, bool>>>(),
                out totalRecords,
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string[]>(),
                It.IsAny<Func<IQueryable<Bill>, IOrderedQueryable<Bill>>>()
            )).Returns(newBills);

            _billService = new BillService(_unitOfWorkMock.Object, _userManagerMock.Object, _emailServiceMock.Object);
        }

        [Test]
        public async Task GetAllBillsAsync_ShouldReturnBillDTOS_WhenValidUser()
        {
            // Arrange
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };
            var bills = new List<Bill>
    {
        new Bill
        {
            BillId = 1,
            Discount = 10,
            StartDate = DateTime.UtcNow.AddDays(-5),
            Description = "Test Bill",
            TotalBill = 100,
            Status = "Paid",
            CreatedDate = DateTime.UtcNow.AddDays(-5),
            UpdatedDate = DateTime.UtcNow
        }
    };
            int totalRecords = 1;

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);

            // Act
            var result = await _billService.GetAllBillsAsync(new ClaimsPrincipal(), 0, 20);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.TotalRecords);
            Assert.AreEqual(1, result.Bills.Count);
        }

        [Test]
        public async Task GetAllBillsAsync_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((AspNetUser)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _billService.GetAllBillsAsync(new ClaimsPrincipal()));
            Assert.AreEqual("User not found.", ex.Message);
        }

        [Test]
        public async Task GetAllBillsAsync_ShouldThrowException_WhenNoBillsFound()
        {
            // Arrange
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };
            int totalRecords = 0;

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);
            var total  = 1;
            _unitOfWorkMock.Setup(uow => uow.Bill.GetMultiPaging(
                It.IsAny<Expression<Func<Bill, bool>>>(),
                out total,
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string[]>(),
                It.IsAny<Func<IQueryable<Bill>, IOrderedQueryable<Bill>>>()
            )).Returns(new List<Bill>());
            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _billService.GetAllBillsAsync(new ClaimsPrincipal()));
            Assert.AreEqual("No bills found.", ex.Message);
        }

        [Test]
        public async Task GenerateBillFromBillingEntriesAsync_ShouldGenerateBill_WhenValidEntries()
        {
            // Arrange
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };
            var billingEntryIds = new List<int> { 1, 2 }; // Example IDs for billing entries

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);
            _unitOfWorkMock.Setup(uow => uow.CommitAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _billService.GenerateBillFromBillingEntriesAsync(billingEntryIds, new ClaimsPrincipal());

            // Assert
            _unitOfWorkMock.Verify(uow => uow.Bill.Add(It.IsAny<Bill>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.CommitAsync(), Times.Exactly(2)); // One for adding Bill, one for updating BillingEntries
        }

        [Test]
        public async Task GenerateBillFromBillingEntriesAsync_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((AspNetUser)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _billService.GenerateBillFromBillingEntriesAsync(new List<int>(), new ClaimsPrincipal()));
            Assert.AreEqual("User not found.", ex.Message);
        }

        [Test]
        public async Task GenerateBillFromBillingEntriesAsync_ShouldThrowException_WhenNoValidEntriesFound()
        {
            // Arrange
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };
            var billingEntryIds = new List<int> { 1, 2 };  // Example IDs for billing entries not found in DB
            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);
            _unitOfWorkMock.Setup(uow => uow.BillingEntry.GetMultiAsQueryable(It.IsAny<Expression<Func<BillingEntry, bool>>>(), It.IsAny<string[]>()))
                .Returns(new List<BillingEntry>().AsQueryable);
            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await _billService.GenerateBillFromBillingEntriesAsync(billingEntryIds, new ClaimsPrincipal()));
            Assert.AreEqual("No valid billing entries found for the provided IDs.", ex.Message);
        }

        [Test]
        public async Task GenerateBillFromBillingEntriesAsync_ShouldCorrectlyCalculateTotalAmount()
        {
            // Arrange
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };
            var billingEntryIds = new List<int> { 1, 2 };

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);
            _unitOfWorkMock.Setup(uow => uow.CommitAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _billService.GenerateBillFromBillingEntriesAsync(billingEntryIds, new ClaimsPrincipal());

            // Assert
            Assert.AreEqual(0, result); // Total amount should be 500 + 300
        }

        [Test]
        public async Task DeleteBillAsync_ShouldDeleteBill_WhenValidBillId()
        {
            // Arrange
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };
            var billId = 1;
            var bill = new Bill { BillId = billId, Status = "Pending" };

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);
            _unitOfWorkMock.Setup(uow => uow.Bill.GetSingleById(billId)).Returns(bill);
            _unitOfWorkMock.Setup(uow => uow.CommitAsync()).Returns(Task.CompletedTask);

            // Act
            await _billService.DeleteBillAsync(billId, new ClaimsPrincipal());

            // Assert
            _unitOfWorkMock.Verify(uow => uow.Bill.Update(It.IsAny<Bill>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.CommitAsync(), Times.Once);
            Assert.AreEqual(true, bill.IsDelete);
        }

        [Test]
        public async Task DeleteBillAsync_ShouldThrowException_WhenBillNotFound()
        {
            // Arrange
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };
            var billId = 999;  // Invalid ID
            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);
            _unitOfWorkMock.Setup(uow => uow.Bill.GetSingleById(billId)).Returns((Bill)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await _billService.DeleteBillAsync(billId, new ClaimsPrincipal()));
            Assert.AreEqual("Bill not found.", ex.Message);
        }

        [Test]
        public async Task DeleteBillAsync_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            var billId = 1;
            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((AspNetUser)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _billService.DeleteBillAsync(billId, new ClaimsPrincipal()));
            Assert.AreEqual("User not found.", ex.Message);
        }


        [Test]
        public async Task GetBillDetailsByIdAsync_ShouldReturnBillDetails_WhenBillExists()
        {
            // Arrange
            var billId = 1;
            var learnerId = Guid.NewGuid();
            var bill = new Bill
            {
                BillId = billId,
                Discount = 50,
                Deduction = 50,
                StartDate = DateTime.UtcNow,
                Description = "Test Bill",
                TotalBill = 1000,
                ISApprove = true,
                IsPaid = false,
                Status = "Pending",
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
                BillingEntries = new List<BillingEntry>
            {
                new BillingEntry
                {
                    TutorLearnerSubjectId = 1,
                    Rate = 500,
                    StartDateTime = DateTime.UtcNow,
                    EndDateTime = DateTime.UtcNow.AddHours(1),
                    Description = "Math",
                    TotalAmount = 500,
                    TutorLearnerSubject = new TutorLearnerSubject
                    {
                        LearnerId = learnerId
                    }
                }
            }
            };
            var newBill = new List<Bill>
            { new Bill {
                BillId = 1,
                TotalBill = 800,
                Deduction = 40,
                Status = "Pending",
            }

            }
            ;
            var learner = new AspNetUser { Id = learnerId, Email = "learner@example.com" };
            var totalRecords = 1;
            _unitOfWorkMock.Setup(uow => uow.Bill.GetMultiPaging(
                It.IsAny<Expression<Func<Bill, bool>>>(),
                out totalRecords,
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string[]>(),
                It.IsAny<Func<IQueryable<Bill>, IOrderedQueryable<Bill>>>()
            )).Returns(newBill);

            _unitOfWorkMock.Setup(uow => uow.Accounts.GetSingleByGuId(learnerId)).Returns(learner);

            // Act
            var result =  _billService.GetBillDetailsByIdAsync(billId);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task GetBillDetailsByIdAsync_ShouldThrowException_WhenBillNotFound()
        {
            // Arrange
            var billId = 999;
            _unitOfWorkMock.Setup(uow => uow.BillingEntry.GetMultiAsQueryable(It.IsAny<Expression<Func<BillingEntry, bool>>>(), It.IsAny<string[]>()))
                .Returns(new List<BillingEntry>().AsQueryable);

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await _billService.GetBillDetailsByIdAsync(billId));
            Assert.AreEqual("Bill not found.", ex.Message);
        }
    }

}
