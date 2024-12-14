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
using TutoRum.Services.Service;
using TutoRum.Services.ViewModels;
using static TutoRum.FE.Common.Url;

namespace TutoRum.UnitTests.ServiceUnitTest
{
    [TestFixture]
    public class CertificatesServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<UserManager<AspNetUser>> _mockUserManager;
        private CertificatesSevice _service;
        private ClaimsPrincipal _user;

        [SetUp]
        public void SetUp()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUserManager = new Mock<UserManager<AspNetUser>>(
                Mock.Of<IUserStore<AspNetUser>>(),
                null, null, null, null, null, null, null, null
            );
            _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.Name, "testuser@example.com"),
            new Claim(ClaimTypes.NameIdentifier, "1")
            }, "mock"));
            _service = new CertificatesSevice(_mockUnitOfWork.Object, _mockUserManager.Object);
        }

        // Test AddCertificatesAsync
        [Test]
        public async Task AddCertificatesAsync_Should_Add_Certificates_When_Valid()
        {
            var certificates = new List<CertificateDTO>
            {
                new CertificateDTO { ImgUrl = "image1.png", Description = "Cert 1", IssueDate = DateTime.UtcNow, ExpiryDate = DateTime.UtcNow.AddYears(1) }
            };

            var tutorId = Guid.NewGuid();

            _mockUnitOfWork.Setup(u => u.Certificates.Add(It.IsAny<Certificate>()));

            await _service.AddCertificatesAsync(certificates, tutorId);

            _mockUnitOfWork.Verify(u => u.Certificates.Add(It.IsAny<Certificate>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        // Test UpdateCertificatesAsync
        [Test]
        public async Task UpdateCertificatesAsync_Should_Update_Certificate_When_Valid()
        {
            var certificates = new List<CertificateDTO>
            {
                new CertificateDTO { CertificateId = 1, ImgUrl = "image2.png", Description = "Updated Cert", IssueDate = DateTime.UtcNow, ExpiryDate = DateTime.UtcNow.AddYears(1) }
            };

            var tutorId = Guid.NewGuid();

            var existingCertificate = new Certificate
            {
                CertificateId = 1,
                TutorId = tutorId,
                ImgUrl = "image1.png",
                Description = "Old Cert",
                IssueDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddYears(1),
                IsVerified = false,
                Status = "Chưa xác thực"
            };

            _mockUnitOfWork.Setup(u => u.Certificates.GetSingleById(It.IsAny<int>())).Returns(existingCertificate);
            _mockUnitOfWork.Setup(u => u.Certificates.Update(It.IsAny<Certificate>()));

            await _service.UpdateCertificatesAsync(certificates, tutorId);

            _mockUnitOfWork.Verify(u => u.Certificates.Update(It.IsAny<Certificate>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Test]
        public void UpdateCertificatesAsync_Should_Throw_Exception_When_Certificate_Not_Found()
        {
            var certificates = new List<CertificateDTO>
            {
                new CertificateDTO { CertificateId = 1, ImgUrl = "image2.png", Description = "Updated Cert", IssueDate = DateTime.UtcNow, ExpiryDate = DateTime.UtcNow.AddYears(1) }
            };

            var tutorId = Guid.NewGuid();

            _mockUnitOfWork.Setup(u => u.Certificates.GetSingleById(It.IsAny<int>())).Returns<Certificate>(null);

            Assert.ThrowsAsync<Exception>(async () => await _service.UpdateCertificatesAsync(certificates, tutorId));
        }

        // Test VerifyCertificatesAsync
        [Test]
        public async Task VerifyCertificatesAsync_Should_Verify_Certificates_When_Valid()
        {
            var certificateIds = new List<int> { 1, 2 };
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };

            var certificates = new List<Certificate>
            {
                new Certificate { CertificateId = 1, IsVerified = false, Status = "Chưa xác thực" },
                new Certificate { CertificateId = 2, IsVerified = false, Status = "Chưa xác thực" }
            };

            _mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);
            _mockUnitOfWork.Setup(u => u.Certificates.GetMulti(It.IsAny<Expression<Func<Certificate, bool>>>(), It.IsAny<string[]>())).Returns(certificates);
            _mockUnitOfWork.Setup(u => u.Certificates.Update(It.IsAny<Certificate>()));

            await _service.VerifyCertificatesAsync(certificateIds, _user);

            _mockUnitOfWork.Verify(u => u.Certificates.Update(It.IsAny<Certificate>()), Times.Exactly(2));
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        // Test DeleteCertificatesAsync
        [Test]
        public async Task DeleteCertificatesAsync_Should_Delete_Certificate_When_Valid()
        {
            var certificateIds = 1;
            var tutorId = Guid.NewGuid();

            var certificateToDelete = new Certificate
            {
                CertificateId = 1,
                TutorId = tutorId,
                ImgUrl = "image1.png",
                Description = "Cert to delete",
                IssueDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddYears(1),
                IsVerified = false,
                Status = "Chưa xác thực"
            };

            _mockUnitOfWork.Setup(u => u.Certificates.GetMulti(It.IsAny<Expression<Func<Certificate, bool>>>(), It.IsAny<string[]>())).Returns(new List<Certificate> { certificateToDelete });

            _mockUnitOfWork.Setup(u => u.Certificates.Delete(It.IsAny<int>()));
            _mockUnitOfWork.Setup(u => u.Certificates.GetSingleById(It.IsAny<int>())).Returns( certificateToDelete );
           


            await _service.DeleteCertificatesAsync(certificateIds, tutorId);

            _mockUnitOfWork.Verify(u => u.Certificates.Delete(It.IsAny<int>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Test]
        public void DeleteCertificatesAsync_Should_Throw_Exception_When_Certificate_Not_Found()
        {
            var certificateIds = 1;
            var tutorId = Guid.NewGuid();

            _mockUnitOfWork.Setup(u => u.Certificates.GetMulti(It.IsAny<Expression<Func<Certificate, bool>>>(), It.IsAny<string[]>())).Returns(new List<Certificate>());

            Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.DeleteCertificatesAsync(certificateIds, tutorId));
        }
    }
}
