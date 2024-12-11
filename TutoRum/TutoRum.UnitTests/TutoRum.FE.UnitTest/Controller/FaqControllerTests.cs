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

namespace TutoRum.UnitTests.TutoRum.FE.UnitTest.Controller
{
    [TestFixture]
    public class FaqControllerTests
    {
        private Mock<IFaqService> _faqServiceMock;
        private FaqController _controller;

        [SetUp]
        public void SetUp()
        {
            _faqServiceMock = new Mock<IFaqService>();
            _controller = new FaqController(_faqServiceMock.Object);
        }

        [Test]
        public async Task GetAllFAQs_ShouldReturnOk_WhenFAQsExist()
        {
            // Arrange
            var faqs = new List<FaqDto>
    {
        new FaqDto { Id = 1, Question = "What is this?", Answer = "It is a FAQ." },
        new FaqDto { Id = 2, Question = "How does it work?", Answer = "It works like this." }
    };
            _faqServiceMock.Setup(s => s.GetAllFAQsAsync()).ReturnsAsync(faqs);

            // Act
            var result = await _controller.GetAllFAQs();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult.Value);
            Assert.IsInstanceOf<ApiResponse<IEnumerable<FaqDto>>>(okResult.Value);
        }

        [Test]
        public async Task GetAllFAQs_ShouldReturnServerError_WhenExceptionThrown()
        {
            // Arrange
            _faqServiceMock.Setup(s => s.GetAllFAQsAsync()).Throws(new Exception("Some error"));

            // Act
            var result = await _controller.GetAllFAQs();

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
        }

        [Test]
        public async Task GetFAQById_ShouldReturnOk_WhenFAQExists()
        {
            // Arrange
            var faq = new FaqDto { Id = 1, Question = "What is this?", Answer = "It is a FAQ." };
            _faqServiceMock.Setup(s => s.GetFAQByIdAsync(1)).ReturnsAsync(faq);

            // Act
            var result = await _controller.GetFAQById(1);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult.Value);
            Assert.IsInstanceOf<ApiResponse<FaqDto>>(okResult.Value);
        }

        [Test]
        public async Task GetFAQById_ShouldReturnNotFound_WhenFAQNotExists()
        {
            // Arrange
            _faqServiceMock.Setup(s => s.GetFAQByIdAsync(It.IsAny<int>())).ReturnsAsync((FaqDto)null);

            // Act
            var result = await _controller.GetFAQById(1);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("FAQ with ID 1 not found.", ((ApiResponse<FaqDto>)notFoundResult.Value).Message);
        }

        [Test]
        public async Task GetFAQById_ShouldReturnServerError_WhenExceptionThrown()
        {
            // Arrange
            _faqServiceMock.Setup(s => s.GetFAQByIdAsync(It.IsAny<int>())).Throws(new Exception("Some error"));

            // Act
            var result = await _controller.GetFAQById(1);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
        }

        [Test]
        public async Task CreateFAQ_ShouldReturnCreated_WhenFAQIsCreated()
        {
            // Arrange
            var faqCreateDto = new FaqCreateDto { Question = "What is this?", Answer = "It is a FAQ." };
            var faqDto = new FaqDto { Id = 1, Question = "What is this?", Answer = "It is a FAQ." };

            _faqServiceMock.Setup(s => s.CreateFAQAsync(It.IsAny<FaqCreateDto>(), It.IsAny<ClaimsPrincipal>())).ReturnsAsync(faqDto);

            // Act
            var result = await _controller.CreateFAQ(faqCreateDto);

            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(result);
            var createdResult = result as CreatedAtActionResult;
            Assert.AreEqual(nameof(_controller.GetFAQById), createdResult.ActionName);
            Assert.AreEqual(1, ((FaqDto)((ApiResponse<FaqDto>)createdResult.Value).Data).Id);
        }

        [Test]
        public async Task CreateFAQ_ShouldReturnBadRequest_WhenFAQDtoIsNull()
        {
            // Act
            var result = await _controller.CreateFAQ(null);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Question and Answer must not be empty.", ((ApiResponse<object>)badRequestResult.Value).Message);
        }

        [Test]
        public async Task CreateFAQ_ShouldReturnServerError_WhenExceptionThrown()
        {
            // Arrange
            var faqCreateDto = new FaqCreateDto { Question = "What is this?", Answer = "It is a FAQ." };

            _faqServiceMock.Setup(s => s.CreateFAQAsync(It.IsAny<FaqCreateDto>(), It.IsAny<ClaimsPrincipal>())).Throws(new Exception("Some error"));

            // Act
            var result = await _controller.CreateFAQ(faqCreateDto);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
        }

        [Test]
        public async Task UpdateFAQ_ShouldReturnOk_WhenFAQIsUpdated()
        {
            // Arrange
            var faqUpdateDto = new FaqUpdateDto { Question = "What is this?", Answer = "Updated FAQ" };
            var faqDto = new FaqDto { Id = 1, Question = "What is this?", Answer = "Updated FAQ" };

            _faqServiceMock.Setup(s => s.UpdateFAQAsync(It.IsAny<FaqUpdateDto>(), It.IsAny<ClaimsPrincipal>())).ReturnsAsync(faqDto);

            // Act
            var result = await _controller.UpdateFAQ(1, faqUpdateDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult.Value);
            Assert.AreEqual(1, ((FaqDto)((ApiResponse<FaqDto>)okResult.Value).Data).Id);
        }

        [Test]
        public async Task UpdateFAQ_ShouldReturnNotFound_WhenFAQNotExists()
        {
            // Arrange
            var faqUpdateDto = new FaqUpdateDto { Question = "What is this?", Answer = "Updated FAQ" };

            _faqServiceMock.Setup(s => s.UpdateFAQAsync(It.IsAny<FaqUpdateDto>(), It.IsAny<ClaimsPrincipal>())).Throws(new KeyNotFoundException());

            // Act
            var result = await _controller.UpdateFAQ(1, faqUpdateDto);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("FAQ with ID 1 not found.", ((ApiResponse<FaqDto>)notFoundResult.Value).Message);
        }

        [Test]
        public async Task UpdateFAQ_ShouldReturnServerError_WhenExceptionThrown()
        {
            // Arrange
            var faqUpdateDto = new FaqUpdateDto { Question = "What is this?", Answer = "Updated FAQ" };

            _faqServiceMock.Setup(s => s.UpdateFAQAsync(It.IsAny<FaqUpdateDto>(), It.IsAny<ClaimsPrincipal>())).Throws(new Exception("Some error"));

            // Act
            var result = await _controller.UpdateFAQ(1, faqUpdateDto);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
        }

        [Test]
        public async Task DeleteFAQ_ShouldReturnNotFound_WhenFAQNotExists()
        {
            // Arrange
            _faqServiceMock.Setup(s => s.DeleteFAQAsync(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>())).Throws(new KeyNotFoundException());

            // Act
            var result = await _controller.DeleteFAQ(1);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("FAQ with ID 1 not found.", ((ApiResponse<object>)notFoundResult.Value).Message);
        }

        [Test]
        public async Task DeleteFAQ_ShouldReturnUnauthorized_WhenUnauthorizedAccessExceptionThrown()
        {
            // Arrange
            _faqServiceMock.Setup(s => s.DeleteFAQAsync(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>())).Throws(new UnauthorizedAccessException());

            // Act
            var result = await _controller.DeleteFAQ(1);

            // Assert
            Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.IsNotNull(unauthorizedResult.Value);
        }

        [Test]
        public async Task DeleteFAQ_ShouldReturnServerError_WhenExceptionThrown()
        {
            // Arrange
            _faqServiceMock.Setup(s => s.DeleteFAQAsync(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>())).Throws(new Exception("Some error"));

            // Act
            var result = await _controller.DeleteFAQ(1);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.AreEqual("An internal server error occurred.", ((ApiResponse<object>)objectResult.Value).Message);
        }

        [Test]
        public async Task GetHomepageFAQs_ShouldReturnOk_WhenFAQsExist()
        {
            // Arrange
            var faqs = new FaqHomePageDTO
            {
                FAQs = new List<FaqDto>
        {
            new FaqDto { Id = 1, Question = "What is this?", Answer = "It is a FAQ." },
            new FaqDto { Id = 2, Question = "How does it work?", Answer = "It works like this." }
        }
            };

            _faqServiceMock.Setup(s => s.GetFaqHomePage(It.IsAny<int>(), It.IsAny<int>()))
                           .ReturnsAsync(faqs);

            // Act
            var result = await _controller.GetHomepageFAQs(0, 20);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult.Value);
            Assert.IsInstanceOf<ApiResponse<FaqHomePageDTO>>(okResult.Value);
        }


        [Test]
        public async Task GetHomepageFAQs_ShouldReturnServerError_WhenExceptionThrown()
        {
            // Arrange
            _faqServiceMock.Setup(s => s.GetFaqHomePage(It.IsAny<int>(), It.IsAny<int>())).Throws(new Exception("Some error"));

            // Act
            var result = await _controller.GetHomepageFAQs(0, 20);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
        }

        [Test]
        public async Task UpdateFAQ_ShouldReturnBadRequest_WhenDtoIsInvalid()
        {
            // Act
            var result = await _controller.UpdateFAQ(1, null);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Question and Answer must not be empty.", ((ApiResponse<object>)badRequestResult.Value).Message);
        }

        [Test]
        public async Task DeleteFAQ_ShouldReturnOk_WhenFAQIsDeleted()
        {
            // Act
            var result = await _controller.DeleteFAQ(1);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual("FAQ deleted successfully.", ((ApiResponse<string>)okResult.Value).Data);
        }


    }
}
