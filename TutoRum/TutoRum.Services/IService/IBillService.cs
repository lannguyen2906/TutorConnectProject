using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Services.ViewModels;

namespace TutoRum.Services.IService
{
    public interface IBillService
    {
        Task<BillDTOS> GetAllBillsAsync(ClaimsPrincipal user, int pageIndex = 0, int pageSize = 20);
        Task<int> GenerateBillFromBillingEntriesAsync(List<int> billingEntryIds, ClaimsPrincipal user);
        Task DeleteBillAsync(int billId, ClaimsPrincipal user);
        Task<byte[]> GenerateBillPdfAsync(int billId);
        Task<BillDetailsDTO> GetBillDetailsByIdAsync(int billId);
        Task<string> ViewBillHtmlAsync(int billId);
        Task<bool> ApproveBillByIdAsync(int billId, ClaimsPrincipal user);
        Task SendBillEmailAsync(int billId);
        Task<PagedResult<BillDetailsDTO>> GetBillByTutorLearnerSubjectIdAsync(int tutorLearnerSubjectId, int pageIndex = 0, int pageSize = 10);
        Task<PagedResult<BillDetailsDTO>> GetBillByTutorAsync(ClaimsPrincipal user, int pageIndex = 0, int pageSize = 10);

    }
}
