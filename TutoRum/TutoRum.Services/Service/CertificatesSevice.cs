using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Enum;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.Models;
using TutoRum.Services.IService;
using TutoRum.Services.ViewModels;

namespace TutoRum.Services.Service
{
    public class CertificatesSevice : ICertificatesSevice
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AspNetUser> _userManager;


        public CertificatesSevice(IUnitOfWork unitOfWork, UserManager<AspNetUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task AddCertificatesAsync(IEnumerable<CertificateDTO> certificates, Guid tutorId)
        {
            if (certificates != null && certificates.Any())
            {
                foreach (var certificateDto in certificates)
                {
                    var certificate = new Certificate
                    {
                        TutorId = tutorId,
                        ImgUrl = certificateDto.ImgUrl,
                        Description = certificateDto.Description,
                        CreatedDate = DateTime.UtcNow,
                        IssueDate = certificateDto.IssueDate,
                        ExpiryDate = certificateDto.ExpiryDate,
                        IsVerified = false,
                        Status = "Chưa xác thực"
                    };

                    _unitOfWork.Certificates.Add(certificate);
                }

                await _unitOfWork.CommitAsync();
            }
        }

        public async Task UpdateCertificatesAsync(IEnumerable<CertificateDTO> certificates, Guid tutorId)
        {
            if (certificates != null && certificates.Any())
            {
                foreach (var certificateDto in certificates)
                {
                    
                    var certificate =  _unitOfWork.Certificates.GetSingleById(certificateDto.CertificateId);

                    if (certificate == null || certificate.TutorId != tutorId)
                    {
                        throw new Exception($"Chứng chỉ không tồn tại hoặc không thuộc về giáo viên này. ID: {certificateDto.CertificateId}");
                    }

                   
                    certificate.ImgUrl = certificateDto.ImgUrl;
                    certificate.Description = certificateDto.Description;
                    certificate.IssueDate = certificateDto.IssueDate;
                    certificate.ExpiryDate = certificateDto.ExpiryDate;
                    certificate.UpdatedDate = DateTime.UtcNow;
                    certificate.IsVerified = false;
                    certificate.Status = "Chưa xác thực";
                    _unitOfWork.Certificates.Update(certificate);
                }

                // Lưu các thay đổi vào database
                await _unitOfWork.CommitAsync();
            }
        }

        public async Task VerifyCertificatesAsync(List<int> certificateIds, ClaimsPrincipal user)
        {

            var currentUser = await _userManager.GetUserAsync(user);
            if (currentUser == null)
            {
                throw new Exception("User not found.");
            }

            var certificates = _unitOfWork.Certificates.GetMulti(c => certificateIds.Contains(c.CertificateId)).ToList();

            if (certificates != null && certificates.Any())
            {
                foreach (var certificate in certificates)
                {
                    certificate.IsVerified = true;
                    certificate.Status = "Đã xác thực";

                    certificate.CreatedBy = currentUser.Id; 

                    _unitOfWork.Certificates.Update(certificate);
                }

                await _unitOfWork.CommitAsync();
            }
            else
            {
                throw new Exception("Không tìm thấy chứng chỉ nào.");
            }
        }

        public async Task DeleteCertificatesAsync(int certificateIds, Guid tutorId)
        {
            if (certificateIds != null)
            {
                // Lấy tất cả chứng chỉ theo danh sách ID
                var certificatesToDelete = _unitOfWork.Certificates.GetSingleById(certificateIds);

                if (certificatesToDelete == null)
                {
                    throw new KeyNotFoundException("No certificates found for the provided tutor and certificate IDs.");
                }

                _unitOfWork.Certificates.Delete(certificatesToDelete.CertificateId);

                // Lưu thay đổi
                await _unitOfWork.CommitAsync();
            }
        }


    }
}
