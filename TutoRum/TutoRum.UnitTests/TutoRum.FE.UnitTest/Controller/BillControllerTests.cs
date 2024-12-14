using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.FE.Common;
using TutoRum.FE.Controllers;
using TutoRum.Services.IService;
using TutoRum.Services.ViewModels;
using static TutoRum.FE.Common.Url;

namespace TutoRum.UnitTests.TutoRum.FE.UnitTest.Controller
{
    [TestFixture]
    public class BillControllerTests
    {
        private Mock<IBillService> _mockBillService;
        private BillController _controller;
        private ClaimsPrincipal _user;

        [SetUp]
        public void Setup()
        {
            _mockBillService = new Mock<IBillService>();

            _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.Name, "testuser@example.com"),
            new Claim(ClaimTypes.NameIdentifier, "1")
            }, "mock"));

            _controller = new BillController(_mockBillService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = _user }
                }
            };
        }

        #region GetAllBills Tests

        [Test]
        public async Task GetAllBills_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var bills = new BillDTOS(); // Mock data
            _mockBillService.Setup(s => s.GetAllBillsAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>(), 0, 20))
                .ReturnsAsync(bills);

            // Act
            var result = await _controller.GetAllBills();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var response = okResult.Value as ApiResponse<BillDTOS>;
            Assert.NotNull(response);
            Assert.AreEqual(bills, response.Data);
        }

        [Test]
        public async Task GetAllBills_ThrowsUnauthorizedAccessException_Returns403()
        {
            // Arrange
            _mockBillService.Setup(s => s.GetAllBillsAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>(), 0, 20))
                .ThrowsAsync(new UnauthorizedAccessException("Unauthorized access"));

            // Act
            var result = await _controller.GetAllBills();

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(403, statusCodeResult.StatusCode);
            var response = statusCodeResult.Value as ApiResponse<object>;
            Assert.AreEqual("Unauthorized access", response.Message);
        }

        [Test]
        public async Task GetAllBills_ThrowsException_Returns500()
        {
            // Arrange
            _mockBillService.Setup(s => s.GetAllBillsAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>(), 0, 20))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.GetAllBills();

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion

        #region GenerateBillFromBillingEntries Tests

        [Test]
        public async Task GenerateBillFromBillingEntries_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var billingEntryIds = new List<int> { 1, 2, 3 };
            var billId = 123; // Mock data
            _mockBillService.Setup(s => s.GenerateBillFromBillingEntriesAsync(billingEntryIds, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(billId);

            // Act
            var result = await _controller.GenerateBillFromBillingEntries(billingEntryIds);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var response = okResult.Value as ApiResponse<int>;
            Assert.NotNull(response);
            Assert.AreEqual(billId, response.Data);
        }

        [Test]
        public async Task GenerateBillFromBillingEntries_ThrowsUnauthorizedAccessException_Returns403()
        {
            // Arrange
            var billingEntryIds = new List<int> { 1, 2, 3 };
            _mockBillService.Setup(s => s.GenerateBillFromBillingEntriesAsync(billingEntryIds, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new UnauthorizedAccessException("Unauthorized access"));

            // Act
            var result = await _controller.GenerateBillFromBillingEntries(billingEntryIds);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(403, statusCodeResult.StatusCode);
            var response = statusCodeResult.Value as ApiResponse<object>;
            Assert.AreEqual("Unauthorized access", response.Message);
        }

        [Test]
        public async Task GenerateBillFromBillingEntries_ThrowsException_Returns500()
        {
            // Arrange
            var billingEntryIds = new List<int> { 1, 2, 3 };
            _mockBillService.Setup(s => s.GenerateBillFromBillingEntriesAsync(billingEntryIds, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.GenerateBillFromBillingEntries(billingEntryIds);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            var response = statusCodeResult.Value as ApiResponse<object>;
        }

        #endregion

        #region DeleteBill Tests

        [Test]
        public async Task DeleteBill_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var billId = 123;
            _mockBillService.Setup(s => s.DeleteBillAsync(billId, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteBill(billId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var response = okResult.Value as ApiResponse<string>;
            Assert.NotNull(response);
            Assert.AreEqual("Bill deleted successfully.", response.Data);
        }

        [Test]
        public async Task DeleteBill_ThrowsUnauthorizedAccessException_Returns403()
        {
            // Arrange
            var billId = 123;
            _mockBillService.Setup(s => s.DeleteBillAsync(billId, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new UnauthorizedAccessException("Unauthorized access"));

            // Act
            var result = await _controller.DeleteBill(billId);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(403, statusCodeResult.StatusCode);
            var response = statusCodeResult.Value as ApiResponse<object>;
            Assert.AreEqual("Unauthorized access", response.Message);
        }

        [Test]
        public async Task DeleteBill_ThrowsKeyNotFoundException_Returns404()
        {
            // Arrange
            var billId = 123;
            _mockBillService.Setup(s => s.DeleteBillAsync(billId, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new KeyNotFoundException("Bill not found"));

            // Act
            var result = await _controller.DeleteBill(billId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            var response = notFoundResult.Value as ApiResponse<object>;
            Assert.AreEqual("Bill not found", response.Message);
        }

        [Test]
        public async Task DeleteBill_ThrowsException_Returns500()
        {
            // Arrange
            var billId = 123;
            _mockBillService.Setup(s => s.DeleteBillAsync(billId, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.DeleteBill(billId);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            var response = statusCodeResult.Value as ApiResponse<object>;
        }

        #endregion

        [Test]
        public async Task GenerateBillPdf_ReturnsFile_WhenSuccessful()
        {
            // Arrange
            int billId = 123;
            var pdfContent = new byte[] { 1, 2, 3 }; // Mock PDF content
            _mockBillService.Setup(s => s.GenerateBillPdfAsync(billId)).ReturnsAsync(pdfContent);

            // Act
            var result = await _controller.GenerateBillPdf(billId);

            // Assert
            var fileResult = result as FileContentResult;
            Assert.NotNull(fileResult);
            Assert.AreEqual("application/pdf", fileResult.ContentType);
            Assert.AreEqual($"Bill_{billId}.pdf", fileResult.FileDownloadName);
            Assert.AreEqual(pdfContent, fileResult.FileContents);
        }

        [Test]
        public async Task GenerateBillPdf_ThrowsException_Returns500()
        {
            // Arrange
            int billId = 123;
            _mockBillService.Setup(s => s.GenerateBillPdfAsync(billId)).ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.GenerateBillPdf(billId);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            Assert.AreEqual("Lỗi hệ thống: Internal server error", statusCodeResult.Value);
        }


        #region ViewBillHtml Tests

        [Test]
        public async Task ViewBillHtml_ReturnsContent_WhenSuccessful()
        {
            // Arrange
            int billId = 123;
            string htmlContent = "<html><body>Bill</body></html>"; // Mock HTML content
            _mockBillService.Setup(s => s.ViewBillHtmlAsync(billId)).ReturnsAsync(htmlContent);

            // Act
            var result = await _controller.ViewBillHtml(billId);

            // Assert
            var contentResult = result as ContentResult;
            Assert.NotNull(contentResult);
            Assert.AreEqual("text/html", contentResult.ContentType);
            Assert.AreEqual(htmlContent, contentResult.Content);
        }

        [Test]
        public async Task ViewBillHtml_ThrowsException_Returns500()
        {
            // Arrange
            int billId = 123;
            _mockBillService.Setup(s => s.ViewBillHtmlAsync(billId)).ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.ViewBillHtml(billId);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            Assert.AreEqual("Lỗi hệ thống: Internal server error", statusCodeResult.Value);
        }

        #endregion

        #region ApproveBill Tests

        [Test]
        public async Task ApproveBill_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            int billId = 123;
            _mockBillService.Setup(s => s.ApproveBillByIdAsync(billId, It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(true);

            // Act
            var result = await _controller.ApproveBill(billId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var response = okResult.Value as ApiResponse<bool>;
            Assert.NotNull(response);
            Assert.IsTrue(response.Data);
        }

        [Test]
        public async Task ApproveBill_ThrowsUnauthorizedAccessException_Returns403()
        {
            // Arrange
            int billId = 123;
            _mockBillService.Setup(s => s.ApproveBillByIdAsync(billId, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new UnauthorizedAccessException("Unauthorized access"));

            // Act
            var result = await _controller.ApproveBill(billId);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(403, statusCodeResult.StatusCode);
        }

        [Test]
        public async Task ApproveBill_ThrowsException_Returns500()
        {
            // Arrange
            int billId = 123;
            _mockBillService.Setup(s => s.ApproveBillByIdAsync(billId, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.ApproveBill(billId);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion

        #region SendBillEmail Tests

        [Test]
        public async Task SendBillEmail_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            int billId = 123;
            string parentEmail = "parent@example.com";
            _mockBillService.Setup(s => s.SendBillEmailAsync(billId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.SendBillEmail(billId, parentEmail);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public async Task SendBillEmail_ThrowsUnauthorizedAccessException_Returns403()
        {
            // Arrange
            int billId = 123;
            string parentEmail = "parent@example.com";
            _mockBillService.Setup(s => s.SendBillEmailAsync(billId)).ThrowsAsync(new UnauthorizedAccessException("Unauthorized access"));

            // Act
            var result = await _controller.SendBillEmail(billId, parentEmail);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(403, statusCodeResult.StatusCode);
        }

        [Test]
        public async Task SendBillEmail_ThrowsException_Returns500()
        {
            // Arrange
            int billId = 123;
            string parentEmail = "parent@example.com";
            _mockBillService.Setup(s => s.SendBillEmailAsync(billId)).ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.SendBillEmail(billId, parentEmail);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion

        #region GetBillByTutorLearnerSubjectId Tests

        [Test]
        public async Task GetBillByTutorLearnerSubjectId_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            int tutorLearnerSubjectId = 123;
            var pagedResult = new PagedResult<BillDetailsDTO>(); // Mock data
            _mockBillService.Setup(s => s.GetBillByTutorLearnerSubjectIdAsync(tutorLearnerSubjectId, 0, 10))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _controller.GetBillByTutorLearnerSubjectId(tutorLearnerSubjectId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var response = okResult.Value as ApiResponse<PagedResult<BillDetailsDTO>>;
            Assert.NotNull(response);
            Assert.AreEqual(pagedResult, response.Data);
        }

        [Test]
        public async Task GetBillByTutorLearnerSubjectId_ThrowsException_Returns500()
        {
            // Arrange
            int tutorLearnerSubjectId = 123;
            _mockBillService.Setup(s => s.GetBillByTutorLearnerSubjectIdAsync(tutorLearnerSubjectId, 0, 10))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.GetBillByTutorLearnerSubjectId(tutorLearnerSubjectId);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion

        [Test]
        public async Task GetBillByTutor_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var pagedResult = new PagedResult<BillDetailsDTO>(); // Mock data
            _mockBillService.Setup(s => s.GetBillByTutorAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>(), 0, 10))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _controller.GetBillByTutor(0, 10);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var response = okResult.Value as ApiResponse<PagedResult<BillDetailsDTO>>;
            Assert.NotNull(response);
            Assert.AreEqual(pagedResult, response.Data);
        }

        [Test]
        public async Task GetBillByTutor_ThrowsException_Returns500()
        {
            // Arrange
            _mockBillService.Setup(s => s.GetBillByTutorAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>(), 0, 10))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.GetBillByTutor(0, 10);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            var response = statusCodeResult.Value as ApiResponse<string>;
            Assert.NotNull(response);
            Assert.AreEqual("Internal server error", response.Message);
        }



        [Test]
        public async Task GetBillDetailById_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            int billId = 123;
            var billDetails = new BillDetailsDTO(); // Mock data
            _mockBillService.Setup(s => s.GetBillDetailsByIdAsync(billId)).ReturnsAsync(billDetails);

            // Act
            var result = await _controller.GetBillDetailById(billId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var response = okResult.Value as ApiResponse<BillDetailsDTO>;
            Assert.NotNull(response);
            Assert.AreEqual(billDetails, response.Data);
        }

        [Test]
        public async Task GetBillDetailById_ThrowsException_Returns500()
        {
            // Arrange
            int billId = 123;
            _mockBillService.Setup(s => s.GetBillDetailsByIdAsync(billId)).ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.GetBillDetailById(billId);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            var response = statusCodeResult.Value as ApiResponse<string>;
            Assert.NotNull(response);
            Assert.AreEqual("Internal server error", response.Message);
        }
    }
}

