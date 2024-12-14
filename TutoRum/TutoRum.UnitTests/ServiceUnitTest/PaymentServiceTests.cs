using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Moq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.Models;
using TutoRum.Services.IService;
using TutoRum.Services.Service;
using TutoRum.Services.ViewModels;

namespace TutoRum.UnitTests.ServiceUnitTest
{
    [TestFixture]
    public class PaymentServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IBillService> _billServiceMock;
        private Mock<IEmailService> _emailServiceMock;
        private Mock<UserManager<AspNetUser>> _userManagerMock;
        private PaymentService _paymentService;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _billServiceMock = new Mock<IBillService>();
            _emailServiceMock = new Mock<IEmailService>();
            _userManagerMock = new Mock<UserManager<AspNetUser>>(
                Mock.Of<IUserStore<AspNetUser>>(), null, null, null, null, null, null, null, null);
            _unitOfWorkMock.Setup(st => st.Payment.Add(It.IsAny<Payment>()));
            _emailServiceMock.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            _paymentService = new PaymentService(
                _unitOfWorkMock.Object,
                _billServiceMock.Object,
                _emailServiceMock.Object,
                _userManagerMock.Object);
        }

        // Test ProcessPaymentAsync method
        [Test]
        public async Task ProcessPaymentAsync_InvalidResponse_ThrowsException()
        {
            // Arrange
            var response = new PaymentResponseModel { VnPayResponseCode = "01" }; // Invalid response code
            var claimsPrincipal = new ClaimsPrincipal();

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _paymentService.ProcessPaymentAsync(response, claimsPrincipal));
            Assert.AreEqual("Payment failed or invalid response.", ex.Message);
        }

        [Test]
        public async Task ProcessPaymentAsync_BillNotFound_ThrowsException()
        {
            // Arrange
            var response = new PaymentResponseModel { VnPayResponseCode = "00", OrderId = "1" };
            _unitOfWorkMock.Setup(u => u.Bill.GetSingleById(It.IsAny<int>())).Returns((Bill)null);
            var claimsPrincipal = new ClaimsPrincipal();

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _paymentService.ProcessPaymentAsync(response, claimsPrincipal));
            Assert.AreEqual("Bill not found.", ex.Message);
        }

        [Test]
        public async Task ProcessPaymentAsync_BillAlreadyPaid_ThrowsException()
        {
            // Arrange
            var bill = new Bill { BillId = 1, IsPaid = true };
            _unitOfWorkMock.Setup(u => u.Bill.GetSingleById(It.IsAny<int>())).Returns(bill);
            var response = new PaymentResponseModel { VnPayResponseCode = "00", OrderId = "1" };
            var claimsPrincipal = new ClaimsPrincipal();

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _paymentService.ProcessPaymentAsync(response, claimsPrincipal));
            Assert.AreEqual("Bill is already paid.", ex.Message);
        }

        [Test]
        public async Task ProcessPaymentAsync_SuccessfulPayment_UpdatesBillAndSendsEmail()
        {
            // Arrange
            var tutor = new Tutor { TutorId = Guid.NewGuid(), Balance = 500 };

            var tutorSubject = new TutorSubject { TutorSubjectId = 1, Tutor = tutor };

            var tutorLearnerSubject = new TutorLearnerSubject
            {
                TutorLearnerSubjectId = 1,
                TutorSubject = tutorSubject
            };

            var billingEntry = new BillingEntry
            {
                BillingEntryId = 1,
                TutorLearnerSubject = tutorLearnerSubject
            };

            var bill = new Bill
            {
                BillId = 1,
                TotalBill = 1000,
                IsPaid = false,
                BillingEntries = new HashSet<BillingEntry> { billingEntry }
            };
            var response = new PaymentResponseModel { VnPayResponseCode = "00", OrderId = "1", Success = true, PaymentMethod = "VnPay", TransactionId = "txn123" };
            var claimsPrincipal = new ClaimsPrincipal();
            var newBills = new List<Bill>
            {
                new Bill
                {
                    BillId = 1,
                TotalBill = 800,
                Deduction = 40,
                Status = "Pending",
                BillingEntries = new HashSet<BillingEntry> { billingEntry }

                }
            };

            var currentUser = new AspNetUser { Id = Guid.NewGuid() , Email ="test@gmail.com"};
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);
            _unitOfWorkMock.Setup(u => u.Bill.GetSingleById(It.IsAny<int>())).Returns(bill);
            _unitOfWorkMock.Setup(u => u.Tutors.GetSingleById(It.IsAny<int>())).Returns(tutor);
            _unitOfWorkMock.Setup(u => u.Bill.Update(It.IsAny<Bill>())).Verifiable();
            _unitOfWorkMock.Setup(u => u.Tutors.Update(It.IsAny<Tutor>())).Verifiable();
            _unitOfWorkMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(uow => uow.Bill.GetMultiAsQueryable(It.IsAny<Expression<Func<Bill, bool>>>(), It.IsAny<string[]>()))
            .Returns(newBills.AsQueryable);
            _billServiceMock.Setup(b => b.GetBillDetailsByIdAsync(It.IsAny<int>())).ReturnsAsync( new BillDetailsDTO { BillId = 1, TotalBill = 123 });
            
            // Act
            var result = await _paymentService.ProcessPaymentAsync(response, claimsPrincipal);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual("Paid", bill.Status);
        }

        // Test SendPaymentSuccessEmailAsync method
        [Test]
        public async Task SendPaymentSuccessEmailAsync_BillDetailsNotFound_ThrowsException()
        {
            // Arrange
            var bill = new Bill { BillId = 1 };
            _billServiceMock.Setup(b => b.GetBillDetailsByIdAsync(It.IsAny<int>())).ReturnsAsync((BillDetailsDTO)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _paymentService.SendPaymentSuccessEmailAsync(bill, "test@example.com"));
            Assert.AreEqual("Bill details not found for email.", ex.Message);
        }

        [Test]
        public async Task SendPaymentSuccessEmailAsync_SuccessfulEmailSend()
        {
            // Arrange
            var bill = new Bill { BillId = 1 };
            var billDetailsDTO = new BillDetailsDTO { BillId = 1, TotalBill = 100 };
            _billServiceMock.Setup(b => b.GetBillDetailsByIdAsync(It.IsAny<int>())).ReturnsAsync(billDetailsDTO);
            _emailServiceMock.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            // Act
            await _paymentService.SendPaymentSuccessEmailAsync(bill, "test@example.com");

            // Assert
            _emailServiceMock.Verify(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        // Test GetPaymentByIdAsync method
        [Test]
        public async Task GetPaymentByIdAsync_PaymentNotFound_ThrowsException()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.Payment.GetSingleById(It.IsAny<int>())).Returns((Payment)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _paymentService.GetPaymentByIdAsync(1));
            Assert.AreEqual("Payment with ID 1 not found.", ex.Message);
        }

        [Test]
        public async Task GetPaymentByIdAsync_ValidPayment_ReturnsPaymentDetailsDTO()
        {
            // Arrange
            var payment = new Payment { PaymentId = 1, AmountPaid = 100, PaymentMethod = "VnPay", PaymentStatus = "Success", BillId = 1 };
            var bill = new Bill { BillId = 1, TotalBill = 100 };
            _unitOfWorkMock.Setup(u => u.Payment.GetSingleById(It.IsAny<int>())).Returns(payment);
            _unitOfWorkMock.Setup(u => u.Bill.GetSingleById(It.IsAny<int>())).Returns(bill);

            // Act
            var result = await _paymentService.GetPaymentByIdAsync(1);

            // Assert
            Assert.AreEqual(payment.PaymentId, result.PaymentId);
            Assert.AreEqual(payment.AmountPaid, result.AmountPaid);
            Assert.AreEqual(payment.PaymentMethod, result.PaymentMethod);
            Assert.AreEqual(payment.PaymentStatus, result.PaymentStatus);
            Assert.AreEqual(payment.BillId, result.BillId);
        }

        [Test]
        public async Task GetListPaymentAsync_ReturnsPagedPayments()
        {
            // Arrange
            var payments = new List<Payment>
        {
            new Payment { PaymentId = 1, AmountPaid = 100, PaymentMethod = "VnPay", PaymentStatus = "Success", BillId = 1 },
            new Payment { PaymentId = 2, AmountPaid = 50, PaymentMethod = "PayPal", PaymentStatus = "Success", BillId = 2 }
        };
            int total = 2;
            _unitOfWorkMock.Setup(uow => uow.Payment.GetMultiPaging(
                It.IsAny<Expression<Func<Payment, bool>>>(),
                out total, // Assign the out parameter value
                It.IsAny<int>(), // Index
                It.IsAny<int>(), // Size
                It.IsAny<string[]>(), // Includes
                It.IsAny<Func<IQueryable<Payment>, IOrderedQueryable<Payment>>>() // OrderBy
            )).Returns(payments);
            // Act
            var result = await _paymentService.GetListPaymentAsync(pageIndex: 0, pageSize: 2);

            // Assert
            Assert.AreEqual(2, result.Items.Count);
            Assert.AreEqual(2, result.TotalRecords);
        }

        // Test GetPaymentsByTutorLearnerSubjectIdAsync method
        [Test]
        public async Task GetPaymentsByTutorLearnerSubjectIdAsync_ReturnsPagedPaymentsForTutorLearnerSubjectId()
        {
            // Arrange
            var payments = new List<Payment>
        {
            new Payment { PaymentId = 1, AmountPaid = 100, PaymentMethod = "VnPay", PaymentStatus = "Success", BillId = 1 },
            new Payment { PaymentId = 2, AmountPaid = 50, PaymentMethod = "PayPal", PaymentStatus = "Success", BillId = 2 }
        };
            int total = 1;
            // Mock the predicate for the tutor/learner subject filter
            _unitOfWorkMock.Setup(uow => uow.Payment.GetMultiPaging(
                It.IsAny<Expression<Func<Payment, bool>>>(),
                out total, // Assign the out parameter value
                It.IsAny<int>(), // Index
                It.IsAny<int>(), // Size
                It.IsAny<string[]>(), // Includes
                It.IsAny<Func<IQueryable<Payment>, IOrderedQueryable<Payment>>>() // OrderBy
            )).Returns(payments);
            // Act
            var result = await _paymentService.GetPaymentsByTutorLearnerSubjectIdAsync(tutorLearnerSubjectId: 1, pageIndex: 0, pageSize: 2);

            // Assert
            Assert.AreEqual(2, result.Items.Count);
        }
    }
}