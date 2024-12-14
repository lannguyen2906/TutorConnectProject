

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Services.ViewModels;

namespace TutoRum.Services.IService
{
    public interface ICertificatesSevice
    {
        Task AddCertificatesAsync(IEnumerable<CertificateDTO> certificates, Guid tutorId);
        Task UpdateCertificatesAsync(IEnumerable<CertificateDTO> certificates, Guid tutorId);
        Task DeleteCertificatesAsync(int certificateIds, Guid tutorId);
    }
}
