using NUnit.Framework;
using Moq;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TutoRum.Data.Models;
using TutoRum.Data.Infrastructure;
using TutoRum.Services.Service;
using TutoRum.Services.ViewModels;
using System;
using System.Security.Claims;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace TutoRum.UnitTests.ServiceUnitTest
{
    [TestFixture]
    public class FaqServiceUnitTest
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IMapper> _mockMapper;
        private Mock<UserManager<AspNetUser>> _mockUserManager;
        private FaqService _faqService;

        [SetUp]
        public void SetUp()
        {
            // Mocking dependencies
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockUserManager = new Mock<UserManager<AspNetUser>>(
                Mock.Of<IUserStore<AspNetUser>>(),
                null, null, null, null, null, null, null, null);

            // Creating FaqService instance
            _faqService = new FaqService(_mockUnitOfWork.Object, _mockMapper.Object, _mockUserManager.Object);
        }

        [Test]
        public async Task GetAllFAQsAsync_ReturnsFAQs()
        {
            // Arrange
            var faqs = new List<Faq>
            {
                new Faq { Id = 1, Question = "What is TutoRum?", Answer = "TutoRum is a platform for learning." },
                new Faq { Id = 2, Question = "How can I sign up?", Answer = "You can sign up via the website." }
            };

            var faqDtos = new List<FaqDto>
            {
                new FaqDto { Id = 1, Question = "What is TutoRum?", Answer = "TutoRum is a platform for learning." },
                new FaqDto { Id = 2, Question = "How can I sign up?", Answer = "You can sign up via the website." }
            };

            // Setup mock behavior
            _mockUnitOfWork.Setup(uow => uow.Faq.GetAllFAQsAsync()).ReturnsAsync(faqs);
            _mockMapper.Setup(m => m.Map<IEnumerable<FaqDto>>(It.IsAny<IEnumerable<Faq>>())).Returns(faqDtos);

            // Act
            var result = await _faqService.GetAllFAQsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("What is TutoRum?", result.First().Question);
            Assert.AreEqual("You can sign up via the website.", result.Last().Answer);
        }

        [Test]
        public async Task GetAllFAQsAsync_ReturnsEmpty_WhenNoFaqs()
        {
            // Arrange
            var faqs = new List<Faq>();  // No FAQs
            var faqDtos = new List<FaqDto>(); // No DTOs

            // Setup mock behavior
            _mockUnitOfWork.Setup(uow => uow.Faq.GetAllFAQsAsync()).ReturnsAsync(faqs);
            _mockMapper.Setup(m => m.Map<IEnumerable<FaqDto>>(It.IsAny<IEnumerable<Faq>>())).Returns(faqDtos);

            // Act
            var result = await _faqService.GetAllFAQsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());  // No FAQs returned
        }

        [Test]
        public async Task GetFAQByIdAsync_ValidId_ReturnsFaq()
        {
            // Arrange
            var faq = new Faq { Id = 1, Question = "What is TutoRum?", Answer = "TutoRum is a platform for learning." };
            var faqDto = new FaqDto { Id = 1, Question = "What is TutoRum?", Answer = "TutoRum is a platform for learning." };

            // Setup mock behavior
            _mockUnitOfWork.Setup(uow => uow.Faq.GetFAQByIdAsync(1)).ReturnsAsync(faq);
            _mockMapper.Setup(m => m.Map<FaqDto>(It.IsAny<Faq>())).Returns(faqDto);

            // Act
            var result = await _faqService.GetFAQByIdAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("What is TutoRum?", result.Question);
            Assert.AreEqual("TutoRum is a platform for learning.", result.Answer);
        }

     

        [Test]
        public async Task GetFAQByIdAsync_ThrowsException_WhenDatabaseFails()
        {
            // Arrange
            _mockUnitOfWork.Setup(uow => uow.Faq.GetFAQByIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _faqService.GetFAQByIdAsync(1));
            Assert.AreEqual("Database error", ex.Message);
        }

        [Test]
        public async Task GetAllFAQsAsync_ThrowsException_WhenDatabaseFails()
        {
            // Arrange
            _mockUnitOfWork.Setup(uow => uow.Faq.GetAllFAQsAsync()).ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _faqService.GetAllFAQsAsync());
            Assert.AreEqual("Database error", ex.Message);
        }

        // New Test - Test when the mapping fails
        [Test]
        public async Task GetAllFAQsAsync_ThrowsException_WhenMappingFails()
        {
            // Arrange
            var faqs = new List<Faq>
            {
                new Faq { Id = 1, Question = "What is TutoRum?", Answer = "TutoRum is a platform for learning." }
            };

            // Setup mock behavior
            _mockUnitOfWork.Setup(uow => uow.Faq.GetAllFAQsAsync()).ReturnsAsync(faqs);
            _mockMapper.Setup(m => m.Map<IEnumerable<FaqDto>>(It.IsAny<IEnumerable<Faq>>())).Throws(new AutoMapperMappingException("Mapping failed"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<AutoMapperMappingException>(async () => await _faqService.GetAllFAQsAsync());
            Assert.AreEqual("Mapping failed", ex.Message);
        }

        // New Test - Test for valid FAQ ID but FAQ is not found in DB
        [Test]
        public async Task GetFAQByIdAsync_ThrowsException_WhenFaqNotFoundInDb()
        {
            // Arrange
            _mockUnitOfWork.Setup(uow => uow.Faq.GetFAQByIdAsync(1)).ReturnsAsync((Faq)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _faqService.GetFAQByIdAsync(1));
            Assert.AreEqual("FAQ not found", ex.Message);
        }

        [Test]
        public async Task GetFAQByIdAsync_ReturnsFaq_WhenFaqFound()
        {
            var faqInDb = new Faq { Id = 1, Question = "What is Unit Testing?", Answer = "Unit testing is a software testing method where individual units of code are tested." };
            _mockUnitOfWork.Setup(uow => uow.Faq.GetFAQByIdAsync(1)).ReturnsAsync(faqInDb);
            var faqDto = new FaqDto { Id = 1, Question = faqInDb.Question, Answer = faqInDb.Answer };
            _mockMapper.Setup(m => m.Map<FaqDto>(It.IsAny<Faq>())).Returns(faqDto);

            var result = await _faqService.GetFAQByIdAsync(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(faqInDb.Question, result.Question);
            Assert.AreEqual(faqInDb.Answer, result.Answer);
        }

        [Test]
        public async Task GetFAQByIdAsync_ThrowsException_WhenDatabaseErrorOccurs()
        {
            _mockUnitOfWork.Setup(uow => uow.Faq.GetFAQByIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception("Database error"));

            var ex = Assert.ThrowsAsync<Exception>(async () => await _faqService.GetFAQByIdAsync(1));
            Assert.AreEqual("Database error", ex.Message);
        }

       

        [Test]
        public async Task GetAllFAQsAsync_ReturnsFaqList_WhenFaqsExist()
        {
            var faqsInDb = new List<Faq>
    {
        new Faq { Id = 1, Question = "What is Unit Testing?", Answer = "Unit testing is a software testing method." },
        new Faq { Id = 2, Question = "What is Dependency Injection?", Answer = "Dependency Injection is a software design pattern." }
    };

            _mockUnitOfWork.Setup(uow => uow.Faq.GetAllFAQsAsync()).ReturnsAsync(faqsInDb);
            _mockMapper.Setup(m => m.Map<IEnumerable<FaqDto>>(It.IsAny<IEnumerable<Faq>>()))
                       .Returns(faqsInDb.Select(f => new FaqDto { Id = f.Id, Question = f.Question, Answer = f.Answer }));

            var result = await _faqService.GetAllFAQsAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(faqsInDb.Count, result.Count());
            Assert.AreEqual(faqsInDb[0].Question, result.First().Question);
            Assert.AreEqual(faqsInDb[1].Answer, result.Last().Answer);
        }
        [Test]
        public void CreateFAQAsync_ThrowsUnauthorizedAccessException_WhenAdminIdIsMissing()
        {
            // Arrange
            var claims = new List<Claim>(); // No AdminId claim
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

            var faqCreateDto = new FaqCreateDto
            {
                Question = "Sample question",
                Answer = "Sample answer"
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _faqService.CreateFAQAsync(faqCreateDto, user));
            Assert.AreEqual("Admin ID is required.", ex.Message);
        }

        [Test]
        public void CreateFAQAsync_ThrowsArgumentException_WhenAdminIdFormatIsInvalid()
        {
            // Arrange
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "invalid-guid")
        };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

            var faqCreateDto = new FaqCreateDto
            {
                Question = "Sample question",
                Answer = "Sample answer"
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _faqService.CreateFAQAsync(faqCreateDto, user));
            Assert.AreEqual("Invalid Admin ID format.", ex.Message);
        }

        [Test]
        public void CreateFAQAsync_ThrowsKeyNotFoundException_WhenAdminNotFound()
        {
            // Arrange
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
    };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

            var faqCreateDto = new FaqCreateDto
            {
                Question = "Sample question",
                Answer = "Sample answer"
            };

            // Mock Admin lookup returning null (not found)
            _mockUnitOfWork.Setup(uow => uow.Admins.FindAsync(It.IsAny<Expression<Func<Admin, bool>>>()))
                .ReturnsAsync((Admin)null);  // Admin not found

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _faqService.CreateFAQAsync(faqCreateDto, user));
            Assert.AreEqual("Admin not found. Make sure the user is assigned an Admin role.", ex.Message);
        }


       



        [Test]
        public async Task CreateFAQAsync_ThrowsException_WhenQuestionIsNull()
        {
            // Arrange
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
    };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

            var faqCreateDto = new FaqCreateDto
            {
                Question = null,  // Null question
                Answer = "Sample answer"
            };

            // Act & Assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _faqService.CreateFAQAsync(faqCreateDto, user);
            });

            // Assert
            Assert.That(exception.ParamName, Is.EqualTo("Question"));
            Assert.That(exception.Message, Is.EqualTo("Question cannot be null or empty. (Parameter 'Question')"));
        }



        [Test]
        public async Task CreateFAQAsync_ThrowsException_WhenAnswerIsNull()
        {
            // Arrange
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
    };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

            var faqCreateDto = new FaqCreateDto
            {
                Question = "Sample question",
                Answer = null  // Invalid answer
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () => await _faqService.CreateFAQAsync(faqCreateDto, user));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("Answer"));
            Assert.That(ex.Message, Is.EqualTo("Answer cannot be null or empty. (Parameter 'Answer')"));
        }


        [Test]
        public void UpdateFAQAsync_ThrowsUnauthorizedAccessException_WhenAdminIdIsNull()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity()); // No claims

            var faqUpdateDto = new FaqUpdateDto
            {
                Id = 1,
                Question = "Updated question",
                Answer = "Updated answer"
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _faqService.UpdateFAQAsync(faqUpdateDto, user));
            Assert.AreEqual("Admin ID is required.", ex.Message);
        }

        [Test]
        public async Task UpdateFAQAsync_SuccessfullyUpdatesFAQ_WhenValidDtoAndAdmin()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var adminId = Guid.NewGuid();

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userId),
        new Claim("AdminId", adminId.ToString()) // Add AdminId claim
    };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

            var faqUpdateDto = new FaqUpdateDto
            {
                Id = 1, // The ID of the FAQ to update
                Question = "Updated question",
                Answer = "Updated answer"
            };

            var existingFaq = new Faq
            {
                Id = faqUpdateDto.Id,
                Question = "Old question",
                Answer = "Old answer",
                AdminId = adminId,
                CreatedDate = DateTime.UtcNow
            };

            // Mock the GetFAQByIdAsync to return an existing FAQ
            _mockUnitOfWork.Setup(uow => uow.Faq.GetFAQByIdAsync(faqUpdateDto.Id)).ReturnsAsync(existingFaq);

            // Setup CommitAsync to return Task.CompletedTask (mimic save)
            _mockUnitOfWork.Setup(uow => uow.CommitAsync()).Returns(Task.CompletedTask);

            // Setup the mapper to return the updated FAQ DTO
            _mockMapper.Setup(m => m.Map<FaqDto>(It.IsAny<Faq>())).Returns(new FaqDto
            {
                Question = faqUpdateDto.Question,
                Answer = faqUpdateDto.Answer
            });

            // Act
            var result = await _faqService.UpdateFAQAsync(faqUpdateDto, user);

            // Assert
            Assert.IsNotNull(result, "The result should not be null.");
            Assert.AreEqual(faqUpdateDto.Question, result.Question, "The question should be updated.");
            Assert.AreEqual(faqUpdateDto.Answer, result.Answer, "The answer should be updated.");

            // Verify that the FAQ was fetched and updated
            _mockUnitOfWork.Verify(uow => uow.Faq.GetFAQByIdAsync(faqUpdateDto.Id), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
            _mockMapper.Verify(m => m.Map<FaqDto>(It.IsAny<Faq>()), Times.Once);
        }



        [Test]
        public void UpdateFAQAsync_ThrowsKeyNotFoundException_WhenFAQNotFound()
        {
            // Arrange
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) // Valid admin ID
    };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

            var faqUpdateDto = new FaqUpdateDto
            {
                Id = 999, // This ID will not exist in the database
                Question = "Updated question",
                Answer = "Updated answer"
            };

            // Mock the GetFAQByIdAsync method to return null (FAQ not found)
            _mockUnitOfWork.Setup(uow => uow.Faq.GetFAQByIdAsync(It.IsAny<int>())).ReturnsAsync((Faq)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _faqService.UpdateFAQAsync(faqUpdateDto, user));
            Assert.AreEqual("FAQ not found.", ex.Message);
        }
        [Test]
        public async Task UpdateFAQAsync_ThrowsException_WhenCommitFails()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var adminId = Guid.NewGuid();

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userId),
        new Claim("AdminId", adminId.ToString())
    };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

            var faqUpdateDto = new FaqUpdateDto
            {
                Id = 1,
                Question = "Updated question",
                Answer = "Updated answer"
            };

            var existingFaq = new Faq
            {
                Id = faqUpdateDto.Id,
                Question = "Old question",
                Answer = "Old answer",
                AdminId = adminId,
                CreatedDate = DateTime.UtcNow
            };

            // Mock the GetFAQByIdAsync to return an existing FAQ
            _mockUnitOfWork.Setup(uow => uow.Faq.GetFAQByIdAsync(faqUpdateDto.Id)).ReturnsAsync(existingFaq);

            // Mock CommitAsync to throw an exception
            _mockUnitOfWork.Setup(uow => uow.CommitAsync()).ThrowsAsync(new Exception("Database commit failed"));

            // Act & Assert
            var exception =  Assert.ThrowsAsync<Exception>(() => _faqService.UpdateFAQAsync(faqUpdateDto, user));
            Assert.AreEqual("Database commit failed", exception.Message);
        }
        [Test]
        public async Task UpdateFAQAsync_RetainsOldValues_WhenMapperDoesNotUpdate()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var adminId = Guid.NewGuid();

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userId),
        new Claim("AdminId", adminId.ToString())
    };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

            var faqUpdateDto = new FaqUpdateDto
            {
                Id = 1,
                Question = "Updated question",
                Answer = "Updated answer"
            };

            var existingFaq = new Faq
            {
                Id = faqUpdateDto.Id,
                Question = "Old question",
                Answer = "Old answer",
                AdminId = adminId,
                CreatedDate = DateTime.UtcNow
            };

            // Mock the GetFAQByIdAsync to return an existing FAQ
            _mockUnitOfWork.Setup(uow => uow.Faq.GetFAQByIdAsync(faqUpdateDto.Id)).ReturnsAsync(existingFaq);

            // Mock the Mapper to return the updated FAQ (simulate successful mapping)
            _mockMapper.Setup(m => m.Map<FaqDto>(It.IsAny<Faq>())).Returns(new FaqDto
            {
                Question = "Updated question",
                Answer = "Updated answer"
            });

            // Act
            var result = await _faqService.UpdateFAQAsync(faqUpdateDto, user);

            // Assert
            Assert.AreEqual(faqUpdateDto.Question, result.Question);
            Assert.AreEqual(faqUpdateDto.Answer, result.Answer);
        }

        [Test]
        public async Task UpdateFAQAsync_ReturnsNull_WhenMapperReturnsNull()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var adminId = Guid.NewGuid();

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userId),
        new Claim("AdminId", adminId.ToString())
    };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

            var faqUpdateDto = new FaqUpdateDto
            {
                Id = 1,
                Question = "Updated question",
                Answer = "Updated answer"
            };

            var existingFaq = new Faq
            {
                Id = faqUpdateDto.Id,
                Question = "Old question",
                Answer = "Old answer",
                AdminId = adminId,
                CreatedDate = DateTime.UtcNow
            };

            // Mock the GetFAQByIdAsync to return an existing FAQ
            _mockUnitOfWork.Setup(uow => uow.Faq.GetFAQByIdAsync(faqUpdateDto.Id)).ReturnsAsync(existingFaq);

            // Mock the Mapper to return null
            _mockMapper.Setup(m => m.Map<FaqDto>(It.IsAny<Faq>())).Returns<FaqDto>(null);

            // Act
            var result = await _faqService.UpdateFAQAsync(faqUpdateDto, user);

            // Assert
            Assert.IsNull(result, "The result should be null when the Mapper returns null.");
        }

        [Test]
        public async Task DeleteFAQAsync_ThrowsUnauthorizedAccessException_WhenUserIsNotAdmin()
        {
            // Arrange
            var userId = Guid.NewGuid();  // Use Guid for the userId as expected by AspNetUser
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString()) // Convert userId to string here for the claim
    };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

            var faqId = 1; // Assume this FAQ exists

            // Mock GetUserAsync and IsInRoleAsync to return a non-admin user
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new AspNetUser { Id = userId });
            _mockUserManager.Setup(um => um.IsInRoleAsync(It.IsAny<AspNetUser>(), "Admin")).ReturnsAsync(false);

            // Act & Assert
            var exception =  Assert.ThrowsAsync<UnauthorizedAccessException>(() => _faqService.DeleteFAQAsync(faqId, user));
            Assert.AreEqual("User does not have the Admin role.", exception.Message);
        }

        [Test]
        public async Task DeleteFAQAsync_ThrowsKeyNotFoundException_WhenFAQDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString())
    };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

            var faqId = 1; // Assume this FAQ does not exist

            // Mock GetUserAsync and IsInRoleAsync to return an admin user
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new AspNetUser { Id = userId });
            _mockUserManager.Setup(um => um.IsInRoleAsync(It.IsAny<AspNetUser>(), "Admin")).ReturnsAsync(true);

            // Mock GetFAQByIdAsync to return null (FAQ not found)
            _mockUnitOfWork.Setup(uow => uow.Faq.GetFAQByIdAsync(faqId)).ReturnsAsync((Faq)null);

            // Act & Assert
            var exception =  Assert.ThrowsAsync<KeyNotFoundException>(() => _faqService.DeleteFAQAsync(faqId, user));
            Assert.AreEqual("FAQ not found.", exception.Message);
        }

        [Test]
        public async Task DeleteFAQAsync_SuccessfullyDeletesFAQ_WhenValidAdminUserAndFAQExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString())
    };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

            var faqId = 1; // Assume this FAQ exists
            var existingFaq = new Faq
            {
                Id = faqId,
                Question = "Sample question",
                Answer = "Sample answer",
                IsActive = true
            };

            // Mock GetUserAsync and IsInRoleAsync to return an admin user
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new AspNetUser { Id = userId });
            _mockUserManager.Setup(um => um.IsInRoleAsync(It.IsAny<AspNetUser>(), "Admin")).ReturnsAsync(true);

            // Mock GetFAQByIdAsync to return the existing FAQ
            _mockUnitOfWork.Setup(uow => uow.Faq.GetFAQByIdAsync(faqId)).ReturnsAsync(existingFaq);

            // Mock Update and CommitAsync to ensure they are called
            _mockUnitOfWork.Setup(uow => uow.Faq.Update(It.IsAny<Faq>()));
            _mockUnitOfWork.Setup(uow => uow.CommitAsync()).Returns(Task.CompletedTask);

            // Act
            await _faqService.DeleteFAQAsync(faqId, user);

            // Assert
            Assert.IsFalse(existingFaq.IsActive);  // Verify the FAQ is marked as inactive
            _mockUnitOfWork.Verify(uow => uow.Faq.Update(It.IsAny<Faq>()), Times.Once); // Verify Update is called
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once); // Verify CommitAsync is called
        }

        [Test]
        public async Task DeleteFAQAsync_ThrowsUnauthorizedAccessException_WhenUserDoesNotExist()
        {
            // Arrange
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
    };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

            var faqId = 1;

            // Mock GetUserAsync to return null (simulating an unauthenticated user)
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((AspNetUser)null);

            // Act & Assert
            var exception =  Assert.ThrowsAsync<UnauthorizedAccessException>(() => _faqService.DeleteFAQAsync(faqId, user));
            Assert.AreEqual("User does not have the Admin role.", exception.Message);
        }
        [Test]
        public async Task DeleteFAQAsync_ThrowsDbUpdateException_WhenDatabaseUpdateFails()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString())
    };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

            var faqId = 1;
            var existingFaq = new Faq
            {
                Id = faqId,
                Question = "Sample question",
                Answer = "Sample answer",
                IsActive = true
            };

            // Mock GetUserAsync and IsInRoleAsync to return an admin user
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new AspNetUser { Id = userId });
            _mockUserManager.Setup(um => um.IsInRoleAsync(It.IsAny<AspNetUser>(), "Admin")).ReturnsAsync(true);

            // Mock GetFAQByIdAsync to return the existing FAQ
            _mockUnitOfWork.Setup(uow => uow.Faq.GetFAQByIdAsync(faqId)).ReturnsAsync(existingFaq);

            // Simulate a database update failure
            _mockUnitOfWork.Setup(uow => uow.Faq.Update(It.IsAny<Faq>())).Throws(new DbUpdateException("Database update failed"));

            // Act & Assert
            var exception =  Assert.ThrowsAsync<Exception>(() => _faqService.DeleteFAQAsync(faqId, user));
            Assert.AreEqual("Error while deleting the FAQ in the database", exception.Message);
        }
        [Test]
        public async Task DeleteFAQAsync_ThrowsException_WhenUnexpectedErrorOccurs()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString())
    };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

            var faqId = 1;
            var existingFaq = new Faq
            {
                Id = faqId,
                Question = "Sample question",
                Answer = "Sample answer",
                IsActive = true
            };

            // Mock GetUserAsync and IsInRoleAsync to return an admin user
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new AspNetUser { Id = userId });
            _mockUserManager.Setup(um => um.IsInRoleAsync(It.IsAny<AspNetUser>(), "Admin")).ReturnsAsync(true);

            // Mock GetFAQByIdAsync to return the existing FAQ
            _mockUnitOfWork.Setup(uow => uow.Faq.GetFAQByIdAsync(faqId)).ReturnsAsync(existingFaq);

            // Simulate an unexpected error during the delete operation
            _mockUnitOfWork.Setup(uow => uow.Faq.Update(It.IsAny<Faq>())).Throws(new Exception("Unexpected error"));

            // Act & Assert
            var exception =  Assert.ThrowsAsync<Exception>(() => _faqService.DeleteFAQAsync(faqId, user));
            Assert.AreEqual("An unexpected error occurred while deleting the FAQ", exception.Message);
        }
        [Test]
        public async Task DeleteFAQAsync_SuccessfullySetsIsActiveToFalse_WhenFAQIsDeleted()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString())
    };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

            var faqId = 1;
            var existingFaq = new Faq
            {
                Id = faqId,
                Question = "Sample question",
                Answer = "Sample answer",
                IsActive = true
            };

            // Mock GetUserAsync and IsInRoleAsync to return an admin user
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new AspNetUser { Id = userId });
            _mockUserManager.Setup(um => um.IsInRoleAsync(It.IsAny<AspNetUser>(), "Admin")).ReturnsAsync(true);

            // Mock GetFAQByIdAsync to return the existing FAQ
            _mockUnitOfWork.Setup(uow => uow.Faq.GetFAQByIdAsync(faqId)).ReturnsAsync(existingFaq);

            // Mock Update and CommitAsync to ensure they are called
            _mockUnitOfWork.Setup(uow => uow.Faq.Update(It.IsAny<Faq>()));
            _mockUnitOfWork.Setup(uow => uow.CommitAsync()).Returns(Task.CompletedTask);

            // Act
            await _faqService.DeleteFAQAsync(faqId, user);

            // Assert
            Assert.IsFalse(existingFaq.IsActive);  // Verify that IsActive is set to false after deletion
            _mockUnitOfWork.Verify(uow => uow.Faq.Update(It.IsAny<Faq>()), Times.Once); // Verify Update is called
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once); // Verify CommitAsync is called
        }
        [Test]
        public async Task DeleteFAQAsync_SuccessfullySetsUpdatedBy_WhenFAQIsDeleted()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString())
    };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

            var faqId = 1;
            var existingFaq = new Faq
            {
                Id = faqId,
                Question = "Sample question",
                Answer = "Sample answer",
                IsActive = true
            };

            // Mock GetUserAsync and IsInRoleAsync to return an admin user
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new AspNetUser { Id = userId });
            _mockUserManager.Setup(um => um.IsInRoleAsync(It.IsAny<AspNetUser>(), "Admin")).ReturnsAsync(true);

            // Mock GetFAQByIdAsync to return the existing FAQ
            _mockUnitOfWork.Setup(uow => uow.Faq.GetFAQByIdAsync(faqId)).ReturnsAsync(existingFaq);

            // Mock Update and CommitAsync to ensure they are called
            _mockUnitOfWork.Setup(uow => uow.Faq.Update(It.IsAny<Faq>()));
            _mockUnitOfWork.Setup(uow => uow.CommitAsync()).Returns(Task.CompletedTask);

            // Act
            await _faqService.DeleteFAQAsync(faqId, user);

            // Assert
            Assert.AreEqual(userId, existingFaq.UpdatedBy);  // Verify that UpdatedBy is set to current user's Id
        }
      
    }
}

