using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Services.ViewModels;

namespace TutoRum.Services.IService
{
    public interface IBillingEntryService
    {
        Task<BillingEntryDTOS> GetAllBillingEntriesAsync(ClaimsPrincipal user, int pageIndex = 0, int pageSize = 20);
        Task<BillingEntryDTO> GetBillingEntryByIdAsync(int billingEntryId, ClaimsPrincipal user);
        Task<BillingEntryDTOS> GetAllBillingEntriesByTutorLearnerSubjectIdAsync(int tutorLearnerSubjectId, ClaimsPrincipal user, int pageIndex = 0, int pageSize = 20);
        Task AddBillingEntryAsync(AdddBillingEntryDTO billingEntryDTO, ClaimsPrincipal user);
        Task AddDraftBillingEntryAsync(AdddBillingEntryDTO billingEntryDTO, ClaimsPrincipal user);
        Task UpdateBillingEntryAsync(int billingEntryId, UpdateBillingEntryDTO billingEntryDTO, ClaimsPrincipal user);
        Task DeleteBillingEntriesAsync(List<int> billingEntryIds, ClaimsPrincipal user);
        Task<BillingEntryDetailsDTO> GetBillingEntryDetailsAsync(int tutorLearnerSubjectId);
        decimal CalculateTotalAmount(DateTime startDateTime, DateTime endDateTime, decimal rate);
    }
}
