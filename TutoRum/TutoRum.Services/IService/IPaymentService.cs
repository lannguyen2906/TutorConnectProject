using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Services.ViewModels;

namespace TutoRum.Services.IService
{
    public interface IPaymentService
    {
        Task<bool> ProcessPaymentAsync(PaymentResponseModel response, ClaimsPrincipal user);
        Task<PaymentDetailsDTO> GetPaymentByIdAsync(int paymentId);
        Task<PagedResult<PaymentDetailsDTO>> GetListPaymentAsync(int pageIndex = 0, int pageSize = 20,string searchKeyword = null, string paymentStatus = null);
        Task<PagedResult<PaymentDetailsDTO>> GetPaymentsByTutorLearnerSubjectIdAsync(int tutorLearnerSubjectId, int pageIndex = 0, int pageSize = 20);
        //Task<TutorDto> GetTutorByBillIdAsync(int billId);
    }
}
