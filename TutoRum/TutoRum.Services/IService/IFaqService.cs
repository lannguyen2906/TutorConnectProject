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
    public interface IFaqService
    {
        Task<IEnumerable<FaqDto>> GetAllFAQsAsync(); 
        Task<FaqDto> GetFAQByIdAsync(int id);
       
        Task<FaqDto> CreateFAQAsync(FaqCreateDto faqCreateDto, ClaimsPrincipal user);
        Task<FaqDto> UpdateFAQAsync(FaqUpdateDto faqUpdateDto, ClaimsPrincipal user);
        Task DeleteFAQAsync(int id, ClaimsPrincipal user);
        Task<FaqHomePageDTO> GetFaqHomePage(int index = 0, int size = 20);
    }
}
