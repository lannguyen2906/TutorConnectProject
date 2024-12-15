using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Models;
using TutoRum.FE.Common;
using TutoRum.FE.Controllers;
using TutoRum.Services.IService;
using TutoRum.Services.ViewModels;
using static TutoRum.FE.Common.Url;

namespace TutoRum.UnitTests.TutoRum.FE.UnitTest.Controller
{
    [TestFixture]
    public class PaymentRequestControllerTests
    {
        private Mock<IPaymentRequestService> _paymentRequestServiceMock;
        private PaymentRequestController _controller;
        private ClaimsPrincipal _user;

        [SetUp]
        public void Setup()
        {
            _paymentRequestServiceMock = new Mock<IPaymentRequestService>();

            _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.Name, "testuser@example.com"),
            new Claim(ClaimTypes.NameIdentifier, "1")
            }, "mock"));

            _controller = new PaymentRequestController(_paymentRequestServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = _user }
                }
            };
        }

        #region CreatePaymentRequest Tests


        [Test]
        public async Task CreatePaymentRequest_ThrowsException_Returns500()
        {
            // Arrange
            var requestDto = new CreatePaymentRequestDTO();
            _paymentRequestServiceMock.Setup(s => s.CreatePaymentRequestAsync(requestDto, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.CreatePaymentRequest(requestDto);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            var response = statusCodeResult.Value as ApiResponse<string>;
            Assert.NotNull(response);
            Assert.AreEqual("Internal server error", response.Message);
        }

        #endregion

        #region GetListPaymentRequests Tests

        [Test]
        public async Task GetListPaymentRequests_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var filterDto = new PaymentRequestFilterDTO();
            var pagedResult = new PagedResult<PaymentRequestDTO>();
            _paymentRequestServiceMock.Setup(s => s.GetListPaymentRequestsAsync(filterDto, 0, 20))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _controller.GetListPaymentRequests(filterDto, 0, 20);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var response = okResult.Value as ApiResponse<PagedResult<PaymentRequestDTO>>;
            Assert.NotNull(response);
            Assert.AreEqual(pagedResult, response.Data);
        }

        [Test]
        public async Task GetListPaymentRequests_ThrowsException_Returns500()
        {
            // Arrange
            var filterDto = new PaymentRequestFilterDTO();
            _paymentRequestServiceMock.Setup(s => s.GetListPaymentRequestsAsync(filterDto, 0, 20))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.GetListPaymentRequests(filterDto, 0, 20);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion

        #region GetListPaymentRequestsByTutor Tests

        [Test]
        public async Task GetListPaymentRequestsByTutor_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var pagedResult = new PagedResult<PaymentRequestDTO>();
            _paymentRequestServiceMock.Setup(s => s.GetListPaymentRequestsByTutorAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>(), 0, 20))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _controller.GetListPaymentRequestsByTutor(0, 20);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var response = okResult.Value as ApiResponse<PagedResult<PaymentRequestDTO>>;
            Assert.NotNull(response);
            Assert.AreEqual(pagedResult, response.Data);
        }

        [Test]
        public async Task GetListPaymentRequestsByTutor_ThrowsException_Returns500()
        {
            // Arrange
            _paymentRequestServiceMock.Setup(s => s.GetListPaymentRequestsByTutorAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>(), 0, 20))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.GetListPaymentRequestsByTutor(0, 20);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion

        #region ApprovePaymentRequest Tests

        [Test]
        public async Task ApprovePaymentRequest_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            int paymentRequestId = 123;
            _paymentRequestServiceMock.Setup(s => s.ApprovePaymentRequestAsync(paymentRequestId, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.ApprovePaymentRequest(paymentRequestId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var response = okResult.Value as ApiResponse<string>;
            Assert.NotNull(response);
            Assert.AreEqual("Payment request approved successfully.", response.Message);
        }

        [Test]
        public async Task ApprovePaymentRequest_ThrowsException_Returns500()
        {
            // Arrange
            int paymentRequestId = 123;
            _paymentRequestServiceMock.Setup(s => s.ApprovePaymentRequestAsync(paymentRequestId, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.ApprovePaymentRequest(paymentRequestId);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        #endregion


        [Test]
        public async Task RejectPaymentRequest_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            int paymentRequestId = 123;
            var rejectionDto = new RejectPaymentRequestDTO { RejectionReason = "Invalid data" };
            _paymentRequestServiceMock.Setup(s => s.RejectPaymentRequestAsync(paymentRequestId, rejectionDto.RejectionReason, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.RejectPaymentRequest(paymentRequestId, rejectionDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var response = okResult.Value as ApiResponse<string>;
            Assert.NotNull(response);
            Assert.AreEqual("Payment request rejected successfully.", response.Message);
        }

        [Test]
        public async Task RejectPaymentRequest_ThrowsException_Returns500()
        {
            // Arrange
            int paymentRequestId = 123;
            var rejectionDto = new RejectPaymentRequestDTO { RejectionReason = "Invalid data" };
            _paymentRequestServiceMock.Setup(s => s.RejectPaymentRequestAsync(paymentRequestId, rejectionDto.RejectionReason, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.RejectPaymentRequest(paymentRequestId, rejectionDto);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }


        [Test]
        public async Task UpdatePaymentRequest_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            int id = 1;
            var updateDto = new UpdatePaymentRequestDTO();
            _paymentRequestServiceMock.Setup(s => s.UpdatePaymentRequestByIdAsync(id, updateDto, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdatePaymentRequest(id, updateDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual("Payment request updated successfully.", okResult.Value);
        }

        [Test]
        public async Task UpdatePaymentRequest_ReturnsNotFound_WhenRequestNotUpdated()
        {
            // Arrange
            int id = 1;
            var updateDto = new UpdatePaymentRequestDTO();
            _paymentRequestServiceMock.Setup(s => s.UpdatePaymentRequestByIdAsync(id, updateDto, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.UpdatePaymentRequest(id, updateDto);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual("Payment request not found or could not be updated.", notFoundResult.Value);
        }

        [Test]
        public async Task UpdatePaymentRequest_ReturnsBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            int id = 1;
            var updateDto = new UpdatePaymentRequestDTO();
            _paymentRequestServiceMock.Setup(s => s.UpdatePaymentRequestByIdAsync(id, updateDto, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new Exception("Internal error"));

            // Act
            var result = await _controller.UpdatePaymentRequest(id, updateDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Internal error", badRequestResult.Value);
        }

        [Test]
        public async Task UpdatePaymentRequest_ReturnsBadRequest_WhenModelStateInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Error", "Invalid model state");
            var updateDto = new UpdatePaymentRequestDTO();

            // Act
            var result = await _controller.UpdatePaymentRequest(1, updateDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }


        [Test]
        public async Task ConfirmPaymentRequest_ReturnsRedirect_WhenSuccessful()
        {
            // Arrange
            int requestId = 1;
            Guid token = Guid.NewGuid();
            _paymentRequestServiceMock.Setup(s => s.ConfirmPaymentRequest(requestId, token)).ReturnsAsync(true);

            // Act
            var result = await _controller.ConfirmPaymentRequest(requestId, token);

            // Assert
            var redirectResult = result as RedirectResult;
            Assert.NotNull(redirectResult);
        }

        [Test]
        public async Task ConfirmPaymentRequest_ReturnsBadRequest_WhenRequestFails()
        {
            // Arrange
            int requestId = 1;
            Guid token = Guid.NewGuid();
            _paymentRequestServiceMock.Setup(s => s.ConfirmPaymentRequest(requestId, token)).ReturnsAsync(false);

            // Act
            var result = await _controller.ConfirmPaymentRequest(requestId, token);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Email confirmation failed.", badRequestResult.Value);
        }

        [Test]
        public async Task ConfirmPaymentRequest_ReturnsBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            int requestId = 1;
            Guid token = Guid.NewGuid();
            _paymentRequestServiceMock.Setup(s => s.ConfirmPaymentRequest(requestId, token))
                .ThrowsAsync(new Exception("Internal error"));

            // Act
            var result = await _controller.ConfirmPaymentRequest(requestId, token);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Internal error", badRequestResult.Value);
        }


        [Test]
        public async Task DeletePaymentRequestById_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            int id = 1;
            _paymentRequestServiceMock.Setup(s => s.DeletePaymentRequest(id, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeletePaymentRequestById(id);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual("Payment request deleted successfully.", okResult.Value);
        }

        [Test]
        public async Task DeletePaymentRequestById_ReturnsNotFound_WhenRequestNotDeleted()
        {
            // Arrange
            int id = 1;
            _paymentRequestServiceMock.Setup(s => s.DeletePaymentRequest(id, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeletePaymentRequestById(id);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual("Payment request not found or could not be deleted.", notFoundResult.Value);
        }

        [Test]
        public async Task DeletePaymentRequestById_ReturnsBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            int id = 1;
            _paymentRequestServiceMock.Setup(s => s.DeletePaymentRequest(id, It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ThrowsAsync(new Exception("Internal error"));

            // Act
            var result = await _controller.DeletePaymentRequestById(id);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Internal error", badRequestResult.Value);
        }

        [Test]
        public void DownloadPaymentRequests_ShouldReturnFile_WhenPaymentRequestsExist()
        {
            // Arrange
            var paymentRequests = new List<PaymentRequest>
        {
            new PaymentRequest
            {
                PaymentRequestId = 1,
                BankCode = "B001",
                AccountNumber = "12345678",
                FullName = "John Doe",
                Amount = 1000,
                IsPaid = false,
                reasonDesc = "Test reason"
            },
            new PaymentRequest
            {
                PaymentRequestId = 2,
                BankCode = "B002",
                AccountNumber = "87654321",
                FullName = "Jane Smith",
                Amount = 2000,
                IsPaid = true,
                reasonDesc = "Another test reason"
            }
        };

            _paymentRequestServiceMock.Setup(s => s.GetPendingPaymentRequests())
                                      .Returns(paymentRequests);

            // Act
            var result = _controller.DownloadPaymentRequests() as FileContentResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.ContentType);
            Assert.AreEqual("PaymentRequests.xlsx", result.FileDownloadName);

            // Verify Excel content
            using (var stream = new MemoryStream(result.FileContents))
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets["PaymentRequests"];
                Assert.NotNull(worksheet);

                // Verify headers
                Assert.AreEqual("Mã yêu cầu", worksheet.Cells[1, 1].Value);
                Assert.AreEqual("Mã ngân hàng", worksheet.Cells[1, 2].Value);
                Assert.AreEqual("Số tài khoản", worksheet.Cells[1, 3].Value);
                Assert.AreEqual("Tên tài khoản", worksheet.Cells[1, 4].Value);
                Assert.AreEqual("Số tiền cần chuyển", worksheet.Cells[1, 5].Value);
                Assert.AreEqual("Đã chuyển tiền", worksheet.Cells[1, 6].Value);
                Assert.AreEqual("Ghi chú", worksheet.Cells[1, 7].Value);

                // Verify data rows
                Assert.AreEqual(1, worksheet.Cells[2, 1].Value);
                Assert.AreEqual("B001", worksheet.Cells[2, 2].Value);
                Assert.AreEqual("12345678", worksheet.Cells[2, 3].Value);
                Assert.AreEqual("John Doe", worksheet.Cells[2, 4].Value);
                Assert.AreEqual(1000, worksheet.Cells[2, 5].Value);
                Assert.AreEqual(false, worksheet.Cells[2, 6].Value);
                Assert.AreEqual("Test reason", worksheet.Cells[2, 7].Value);

                Assert.AreEqual(2, worksheet.Cells[3, 1].Value);
                Assert.AreEqual("B002", worksheet.Cells[3, 2].Value);
                Assert.AreEqual("87654321", worksheet.Cells[3, 3].Value);
                Assert.AreEqual("Jane Smith", worksheet.Cells[3, 4].Value);
                Assert.AreEqual(2000, worksheet.Cells[3, 5].Value);
                Assert.AreEqual(true, worksheet.Cells[3, 6].Value);
                Assert.AreEqual("Another test reason", worksheet.Cells[3, 7].Value);
            }
        }

        [Test]
        public void DownloadPaymentRequests_ShouldReturnEmptyFile_WhenNoPaymentRequestsExist()
        {
            // Arrange
            _paymentRequestServiceMock.Setup(s => s.GetPendingPaymentRequests())
                                      .Returns(new List<PaymentRequest>());

            // Act
            var result = _controller.DownloadPaymentRequests() as FileContentResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.ContentType);
            Assert.AreEqual("PaymentRequests.xlsx", result.FileDownloadName);

            // Verify Excel content is valid but contains only headers
            using (var stream = new MemoryStream(result.FileContents))
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets["PaymentRequests"];
                Assert.NotNull(worksheet);

                // Verify headers exist
                Assert.AreEqual("Mã yêu cầu", worksheet.Cells[1, 1].Value);
                Assert.AreEqual("Mã ngân hàng", worksheet.Cells[1, 2].Value);
                Assert.AreEqual("Số tài khoản", worksheet.Cells[1, 3].Value);
                Assert.AreEqual("Tên tài khoản", worksheet.Cells[1, 4].Value);
                Assert.AreEqual("Số tiền cần chuyển", worksheet.Cells[1, 5].Value);
                Assert.AreEqual("Đã chuyển tiền", worksheet.Cells[1, 6].Value);
                Assert.AreEqual("Ghi chú", worksheet.Cells[1, 7].Value);

                // Verify no data rows
                Assert.IsNull(worksheet.Cells[2, 1].Value);
            }
        }

        [Test]
        public void DownloadPaymentRequests_ShouldHandleException_WhenServiceFails()
        {
            // Arrange
            _paymentRequestServiceMock.Setup(s => s.GetPendingPaymentRequests())
                                      .Throws(new Exception("Unexpected error"));

            // Act & Assert
            Assert.Throws<Exception>(() => _controller.DownloadPaymentRequests());
        }
    }
}

