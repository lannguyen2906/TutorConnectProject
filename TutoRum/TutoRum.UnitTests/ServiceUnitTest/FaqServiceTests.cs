using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
using TutoRum.Services.Service;
using TutoRum.Services.ViewModels;

namespace TutoRum.UnitTests.ServiceUnitTest
{
    [TestFixture]
    public class FaqServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IMapper> _mapperMock;
        private Mock<UserManager<AspNetUser>> _userManagerMock;
        private FaqService _faqService;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();

            _unitOfWorkMock.Setup(st => st.Faq.Add(It.IsAny<Faq>()));
            _unitOfWorkMock.Setup(st => st.Faq.Update(It.IsAny<Faq>()));



            _userManagerMock = new Mock<UserManager<AspNetUser>>(
              new Mock<IUserStore<AspNetUser>>().Object,
              null, null, null, null, null, null, null, null
          );

            _faqService = new FaqService(_unitOfWorkMock.Object, _mapperMock.Object, _userManagerMock.Object);
        }

        // Unit Test for GetAllFAQsAsync
        [Test]
        public async Task GetAllFAQsAsync_ShouldReturnListOfFaqDtos()
        {
            // Arrange
            var faqs = new List<Faq>
        {
            new Faq { Id = 1, Question = "Test Question 1", Answer = "Test Answer 1", IsActive = true },
            new Faq { Id = 2, Question = "Test Question 2", Answer = "Test Answer 2", IsActive = true }
        };

            _unitOfWorkMock.Setup(uow => uow.Faq.GetAllFAQsAsync()).ReturnsAsync(faqs);
            _mapperMock.Setup(m => m.Map<IEnumerable<FaqDto>>(faqs)).Returns(new List<FaqDto>
        {
            new FaqDto { Id = 1, Question = "Test Question 1", Answer = "Test Answer 1" },
            new FaqDto { Id = 2, Question = "Test Question 2", Answer = "Test Answer 2" }
        });

            // Act
            var result = await _faqService.GetAllFAQsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        // Unit Test for GetFAQByIdAsync
        [Test]
        public async Task GetFAQByIdAsync_ShouldReturnFaqDto_WhenFaqExists()
        {
            // Arrange
            var faq = new Faq { Id = 1, Question = "Test Question", Answer = "Test Answer" };
            _unitOfWorkMock.Setup(uow => uow.Faq.GetFAQByIdAsync(1)).ReturnsAsync(faq);
            _mapperMock.Setup(m => m.Map<FaqDto>(faq)).Returns(new FaqDto { Id = 1, Question = "Test Question", Answer = "Test Answer" });

            // Act
            var result = await _faqService.GetFAQByIdAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Test Question", result.Question);
        }

        [Test]
        public async Task GetFAQByIdAsync_ShouldThrowException_WhenFaqNotFound()
        {
            // Arrange
            _unitOfWorkMock.Setup(uow => uow.Faq.GetFAQByIdAsync(It.IsAny<int>())).ReturnsAsync((Faq)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _faqService.GetFAQByIdAsync(999));
            Assert.AreEqual("FAQ not found", ex.Message);
        }

        // Unit Test for CreateFAQAsync
        [Test]
        public async Task CreateFAQAsync_ShouldReturnFaqDto_WhenCreatedSuccessfully()
        {
            // Arrange
            var faqCreateDto = new FaqCreateDto { Question = "New Question", Answer = "New Answer" };
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            }));

            var admin = new Admin { AdminId = Guid.NewGuid() };

            _unitOfWorkMock.Setup(uow => uow.Admins.FindAsync(It.IsAny<Expression<Func<Admin, bool>>>())).ReturnsAsync(admin);
            _unitOfWorkMock.Setup(uow => uow.Faq.Add(It.IsAny<Faq>()));
            _unitOfWorkMock.Setup(uow => uow.CommitAsync()).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<Faq>(faqCreateDto)).Returns(new Faq());
            _mapperMock.Setup(m => m.Map<FaqDto>(It.IsAny<Faq>())).Returns(new FaqDto { Question = "New Question", Answer = "New Answer" });

            // Act
            var result = await _faqService.CreateFAQAsync(faqCreateDto, user);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("New Question", result.Question);
            Assert.AreEqual("New Answer", result.Answer);
        }

        // Unit Test for UpdateFAQAsync
        [Test]
        public async Task DeleteFAQAsync_ShouldDeleteFaq_WhenAdminRole()
        {
            // Arrange
            var user = new AspNetUser { Id = Guid.NewGuid() };
            var currentUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            }));

            var faq = new Faq { Id = 1, IsActive = true };
            _unitOfWorkMock.Setup(uow => uow.Faq.GetFAQByIdAsync(1)).ReturnsAsync(faq);
            _unitOfWorkMock.Setup(uow => uow.Faq.Update(It.IsAny<Faq>()));
            _unitOfWorkMock.Setup(uow => uow.CommitAsync()).Returns(Task.CompletedTask);

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.IsInRoleAsync(user, "Admin")).ReturnsAsync(true);

            // Act
            await _faqService.DeleteFAQAsync(1, currentUser);

            // Assert
            Assert.IsFalse(faq.IsActive);
        }

        [Test]
        public async Task DeleteFAQAsync_ShouldThrowUnauthorizedAccessException_WhenNotAdmin()
        {
            // Arrange
            var user = new AspNetUser { Id = Guid.NewGuid() };
            var currentUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            }));

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.IsInRoleAsync(user, "Admin")).ReturnsAsync(false);

            // Act & Assert
            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _faqService.DeleteFAQAsync(1, currentUser));
            Assert.AreEqual("User does not have the Admin role.", ex.Message);
        }
    }

}
