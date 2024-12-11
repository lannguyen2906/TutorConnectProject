using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Models;
using TutoRum.Services.ViewModels;

namespace TutoRum.Services.IService
{
    public interface IPaymentRequestService
    {
        Task<PaymentRequest> CreatePaymentRequestAsync(CreatePaymentRequestDTO requestDto, ClaimsPrincipal user);
        Task<PagedResult<PaymentRequestDTO>> GetListPaymentRequestsAsync(
     PaymentRequestFilterDTO filter,
     int pageIndex = 0,
     int pageSize = 20);
        Task<PagedResult<PaymentRequestDTO>> GetListPaymentRequestsByTutorAsync(
            ClaimsPrincipal user,
            int pageIndex = 0,
            int pageSize = 20);
        
        Task<bool> ApprovePaymentRequestAsync(int paymentRequestId, ClaimsPrincipal adminUser);
        Task<bool> RejectPaymentRequestAsync(int paymentRequestId, string rejectionReason, ClaimsPrincipal adminUser);
        Task<bool> UpdatePaymentRequestByIdAsync(int paymentRequestId, UpdatePaymentRequestDTO updateDto, ClaimsPrincipal adminUser);
        Task<bool> ConfirmPaymentRequest(int requestId, Guid token);
        Task<Guid> GeneratePaymentRequestTokenAsync(int paymentRequestId);
        Task SendPaymentRequestConfirmationEmailAsync(ClaimsPrincipal user, PaymentRequest paymentRequest, string confirmationUrl);
        Task<bool> DeletePaymentRequest(int paymentRequestId, ClaimsPrincipal user);
    }
}
