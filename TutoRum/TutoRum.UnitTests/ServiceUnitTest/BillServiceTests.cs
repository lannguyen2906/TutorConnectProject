using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.Models;
using TutoRum.Services.Service;
using TutoRum.Services.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;
using TutoRum.Services.IService;

[TestFixture]
public class BillServiceTests
{
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<UserManager<AspNetUser>> _userManagerMock;
    private Mock<IEmailService> _emailServiceMock;
    private BillService _billService;

    [SetUp]
    public void Setup()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userManagerMock = new Mock<UserManager<AspNetUser>>(
            Mock.Of<IUserStore<AspNetUser>>(), null, null, null, null, null, null, null, null);
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

        var billingEntries = new List<BillingEntry>
        {
            new BillingEntry { BillingEntryId = 1, TotalAmount = 500 },
            new BillingEntry { BillingEntryId = 2, TotalAmount = 300 }
        };

        var userId = Guid.NewGuid();
        var currentUser = new AspNetUser { Id = Guid.NewGuid() };

        // Mock UserManager's methods
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);

        _unitOfWorkMock.Setup(uow => uow.BillingEntry.GetMultiAsQueryable(It.IsAny<Expression<Func<BillingEntry, bool>>>(), It.IsAny<string[]>()))
            .Returns(billingEntries.AsQueryable);

        _unitOfWorkMock.Setup(uow => uow.Bill.GetMultiAsQueryable(It.IsAny<Expression<Func<Bill, bool>>>(), It.IsAny<string[]>()))
            .Returns(newBills.AsQueryable);

        _emailServiceMock = new Mock<IEmailService>();
        _billService = new BillService(_unitOfWorkMock.Object, _userManagerMock.Object, _emailServiceMock.Object);
    }

    [Test]
    public async Task GetAllBillsAsync_UserNotFound_ThrowsException()
    {
        // Arrange
        var claimsPrincipal = new ClaimsPrincipal();
        _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((AspNetUser)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(async () =>
            await _billService.GetAllBillsAsync(claimsPrincipal));
        Assert.AreEqual("User not found.", ex.Message);
    }

   
    [Test]
    public async Task GetBillDetailsByIdAsync_ValidBill_ReturnsBillDetails()
    {
        // Arrange

        var billDetailsDTO = new BillDetailsDTO
        {
            BillId = 1,
            Discount = 10,
            Deduction = 5,
            TotalBill = 100,
            Status = "Pending"
        };


        // Act
        var result = await _billService.GetBillDetailsByIdAsync(1);

        // Assert
        Assert.AreEqual(billDetailsDTO.BillId, result.BillId);
    }

    // Test for ApproveBillByIdAsync method
    [Test]
    public async Task ApproveBillByIdAsync_BillNotFound_ThrowsException()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.Bill.GetSingleById(It.IsAny<int>())).Returns((Bill)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(async () =>
            await _billService.ApproveBillByIdAsync(1, new ClaimsPrincipal()));
        Assert.AreEqual("Bill not found.", ex.Message);
    }

    [Test]
    public async Task ApproveBillByIdAsync_AlreadyApproved_ThrowsException()
    {
        // Arrange
        var bill = new Bill { BillId = 1, ISApprove = true };
        _unitOfWorkMock.Setup(u => u.Bill.GetSingleById(It.IsAny<int>())).Returns(bill);

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(async () =>
            await _billService.ApproveBillByIdAsync(1, new ClaimsPrincipal()));
        Assert.AreEqual("Bill is already approved.", ex.Message);
    }

    [Test]
    public async Task ApproveBillByIdAsync_ValidApproval_ReturnsTrue()
    {
        // Arrange
        var bill = new Bill { BillId = 1, ISApprove = false };
        _unitOfWorkMock.Setup(u => u.Bill.GetSingleById(It.IsAny<int>())).Returns(bill);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _billService.ApproveBillByIdAsync(1, new ClaimsPrincipal());

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("Đã chấp nhận", bill.Status);
    }

    // Test for SendBillEmailAsync method
    [Test]
    public async Task SendBillEmailAsync_BillNotFound_ThrowsException()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.Bill.GetSingleById(It.IsAny<int>())).Returns((Bill)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _billService.SendBillEmailAsync(1));
    }

    // Test for DeleteBillAsync method
    [Test]
    public async Task DeleteBillAsync_BillNotFound_ThrowsException()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.Bill.GetSingleById(It.IsAny<int>())).Returns((Bill)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _billService.DeleteBillAsync(1, new ClaimsPrincipal()));
        Assert.AreEqual("Bill not found.", ex.Message);
    }

    [Test]
    public async Task GenerateBillFromBillingEntriesAsync_ValidData_ReturnsBillId()
    {
        // Arrange
        var claimsPrincipal = new ClaimsPrincipal();
        var mockUser = new AspNetUser { Id = Guid.NewGuid() };

        _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(mockUser);
        _unitOfWorkMock.Setup(u => u.Bill.Add(It.IsAny<Bill>())).Callback<Bill>(bill => bill.BillId = 1);

        // Act
        var result = await _billService.GenerateBillFromBillingEntriesAsync(new List<int> { 1, 2 }, claimsPrincipal);

        // Assert
        Assert.AreEqual(1, result);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Exactly(2));
    }

    [Test]
    public async Task DeleteBillAsync_ValidBillId_MarksAsDeleted()
    {
        // Arrange
        var claimsPrincipal = new ClaimsPrincipal();
        var mockUser = new AspNetUser { Id = Guid.NewGuid() };
        var mockBill = new Bill { BillId = 1, IsDelete = false };

        _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(mockUser);
        _unitOfWorkMock.Setup(u => u.Bill.GetSingleById(It.IsAny<int>())).Returns(mockBill);

        // Act
        await _billService.DeleteBillAsync(1, claimsPrincipal);

        // Assert
        Assert.IsTrue(mockBill.IsDelete);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
    }
}
