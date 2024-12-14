using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.FE.Common;
using TutoRum.FE.Controllers;
using TutoRum.FE.VNPay;
using TutoRum.Services.IService;
using TutoRum.Services.ViewModels;

namespace TutoRum.UnitTests.TutoRum.FE.UnitTest.Controller
{
    [TestFixture]
    public class PaymentControllerTests
    {
        private Mock<IVnPayService> _mockVnPayService;
        private Mock<IBillService> _mockBillService;
        private Mock<IPaymentService> _mockPaymentService;
        private PaymentController _controller;

        [SetUp]
        public void Setup()
        {
            _mockVnPayService = new Mock<IVnPayService>();
            _mockBillService = new Mock<IBillService>();
            _mockPaymentService = new Mock<IPaymentService>();
            _controller = new PaymentController(_mockVnPayService.Object, _mockBillService.Object, _mockPaymentService.Object);
        }

        #region CreatePaymentUrl Tests

        [Test]
        public async Task CreatePaymentUrl_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var billId = 1;
            var billDetails = new BillDetailsDTO { TotalBill = 1000 };
            var paymentUrl = "http://example.com/payment";

            _mockBillService.Setup(s => s.GetBillDetailsByIdAsync(billId)).ReturnsAsync(billDetails);
            _mockVnPayService.Setup(s => s.CreatePaymentUrl(It.IsAny<PaymentInformationModel>(), It.IsAny<HttpContext>()))
                .Returns(paymentUrl);

            // Act
            var result = await _controller.CreatePaymentUrl(billId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as ApiResponse<string>;
            Assert.NotNull(response);
            Assert.AreEqual(paymentUrl, response.Data);
        }

        [Test]
        public async Task CreatePaymentUrl_ReturnsNotFound_WhenBillNotFound()
        {
            // Arrange
            var billId = 1;
            _mockBillService.Setup(s => s.GetBillDetailsByIdAsync(billId)).ReturnsAsync((BillDetailsDTO)null);

            // Act
            var result = await _controller.CreatePaymentUrl(billId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task CreatePaymentUrl_ThrowsException_Returns500()
        {
            // Arrange
            var billId = 1;
            _mockBillService.Setup(s => s.GetBillDetailsByIdAsync(billId)).ThrowsAsync(new Exception("Internal error"));

            // Act
            var result = await _controller.CreatePaymentUrl(billId);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion

        #region PaymentExecute Tests

        [Test]
        public async Task PaymentExecute_ReturnsOk_WhenPaymentProcessedSuccessfully()
        {
            // Arrange
            var responseModel = new PaymentResponseModel
            {
                TransactionId = "12345",
            };

            // Create a mock query collection with query string parameters
            var queryDictionary = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
    {
        { "vnp_TransactionNo", "12345" },
        { "vnp_Amount", "1000" }
    };
            var queryCollection = new QueryCollection(queryDictionary);

            // Set up the HttpContext with the mocked query
            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.Request.Query = queryCollection;

            // Set the controller context to use the mock HttpContext
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext
            };

            // Mock dependencies
            _mockVnPayService
                .Setup(s => s.PaymentExecute(It.IsAny<IQueryCollection>()))
                .Returns(responseModel);

            _mockPaymentService
                .Setup(s => s.ProcessPaymentAsync(responseModel, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.PaymentExecute();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as ApiResponse<PaymentResponseModel>;
            Assert.NotNull(response);
            Assert.AreEqual("Payment processed successfully", response.Message);
            Assert.AreEqual(responseModel, response.Data);
        }

        [Test]
        public async Task PaymentExecute_ReturnsBadRequest_WhenPaymentFails()
        {
            // Arrange
            var responseModel = new PaymentResponseModel
            {
                TransactionId = "12345",
            };

            // Create a mock query collection with query string parameters
            var queryDictionary = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
    {
        { "vnp_TransactionNo", "12345" },
        { "vnp_Amount", "1000" }
    };
            var queryCollection = new QueryCollection(queryDictionary);

            // Set up the HttpContext with the mocked query
            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.Request.Query = queryCollection;

            // Set the controller context to use the mock HttpContext
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext
            };

            // Mock dependencies
            _mockVnPayService
                .Setup(s => s.PaymentExecute(It.IsAny<IQueryCollection>()))
                .Returns(responseModel);

            _mockPaymentService
                .Setup(s => s.ProcessPaymentAsync(responseModel, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.PaymentExecute();

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);

            var apiResponse = badRequestResult.Value as ApiResponse<PaymentResponseModel>;
            Assert.NotNull(apiResponse);
            Assert.AreEqual("Failed to process payment", apiResponse.Message);
            Assert.AreEqual(responseModel, apiResponse.Data);
        }


        [Test]
        public async Task PaymentExecute_ThrowsException_Returns500()
        {
            // Arrange
            _mockVnPayService.Setup(s => s.PaymentExecute(It.IsAny<IQueryCollection>()))
                .Throws(new Exception("Internal error"));

            // Act
            var result = await _controller.PaymentExecute();

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion

        #region GetPaymentById Tests

        [Test]
        public async Task GetPaymentById_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var paymentId = 1;
            var paymentDetails = new PaymentDetailsDTO();

            _mockPaymentService.Setup(s => s.GetPaymentByIdAsync(paymentId)).ReturnsAsync(paymentDetails);

            // Act
            var result = await _controller.GetPaymentById(paymentId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as ApiResponse<PaymentDetailsDTO>;
            Assert.NotNull(response);
            Assert.AreEqual(paymentDetails, response.Data);
        }

        [Test]
        public async Task GetPaymentById_ThrowsException_Returns500()
        {
            // Arrange
            var paymentId = 1;
            _mockPaymentService.Setup(s => s.GetPaymentByIdAsync(paymentId)).ThrowsAsync(new Exception("Internal error"));

            // Act
            var result = await _controller.GetPaymentById(paymentId);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion

        #region GetListPayments Tests

        [Test]
        public async Task GetListPayments_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var resultList = new PagedResult<PaymentDetailsDTO> { Items = new List<PaymentDetailsDTO>(), TotalRecords = 0 };
            _mockPaymentService.Setup(s => s.GetListPaymentAsync(0, 20, null, null)).ReturnsAsync(resultList);

            // Act
            var result = await _controller.GetListPayments();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as ApiResponse<PagedResult<PaymentDetailsDTO>>;
            Assert.NotNull(response);
            Assert.AreEqual(resultList, response.Data);
        }

        [Test]
        public async Task GetListPayments_ThrowsException_Returns500()
        {
            // Arrange
            _mockPaymentService.Setup(s => s.GetListPaymentAsync(0, 20, null, null))
                .ThrowsAsync(new Exception("Internal error"));

            // Act
            var result = await _controller.GetListPayments();

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion

        #region GetPaymentsByTutorLearnerSubjectId Tests

        [Test]
        public async Task GetPaymentsByTutorLearnerSubjectId_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var resultList = new PagedResult<PaymentDetailsDTO> { Items = new List<PaymentDetailsDTO>(), TotalRecords = 0 };
            _mockPaymentService.Setup(s => s.GetPaymentsByTutorLearnerSubjectIdAsync(1, 0, 20)).ReturnsAsync(resultList);

            // Act
            var result = await _controller.GetPaymentsByTutorLearnerSubjectId(1);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as ApiResponse<PagedResult<PaymentDetailsDTO>>;
            Assert.NotNull(response);
            Assert.AreEqual(resultList, response.Data);
        }

        [Test]
        public async Task GetPaymentsByTutorLearnerSubjectId_ThrowsException_Returns500()
        {
            // Arrange
            _mockPaymentService.Setup(s => s.GetPaymentsByTutorLearnerSubjectIdAsync(1, 0, 20))
                .ThrowsAsync(new Exception("Internal error"));

            // Act
            var result = await _controller.GetPaymentsByTutorLearnerSubjectId(1);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion
    }
}
