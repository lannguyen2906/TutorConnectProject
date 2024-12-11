using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.Models;
using TutoRum.Services.IService;
using TutoRum.Services.ViewModels;

namespace TutoRum.Services.Service
{
    public class FaqService : IFaqService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<AspNetUser> _userManager;
      
        public FaqService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<AspNetUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<IEnumerable<FaqDto>> GetAllFAQsAsync()
        {
            var faqs = await _unitOfWork.Faq.GetAllFAQsAsync();
            return _mapper.Map<IEnumerable<FaqDto>>(faqs);
        }

        public async Task<FaqDto> GetFAQByIdAsync(int id)
        {
            var faq = await _unitOfWork.Faq.GetFAQByIdAsync(id);
            if (faq == null)
            {
                throw new Exception("FAQ not found"); 
            }
            return _mapper.Map<FaqDto>(faq);
        }
        public async Task<FaqDto> CreateFAQAsync(FaqCreateDto faqCreateDto, ClaimsPrincipal user)
        {
            if (faqCreateDto == null)
    {
        throw new ArgumentNullException(nameof(faqCreateDto), "FAQ data cannot be null.");
    }

    if (string.IsNullOrEmpty(faqCreateDto.Question))
    {
        throw new ArgumentNullException(nameof(faqCreateDto.Question), "Question cannot be null or empty.");
    }

    if (string.IsNullOrEmpty(faqCreateDto.Answer))
    {
        throw new ArgumentNullException(nameof(faqCreateDto.Answer), "Answer cannot be null or empty.");
    }
            var adminIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (adminIdClaim == null)
            {
                throw new UnauthorizedAccessException("Admin ID is required.");
            }

            if (!Guid.TryParse(adminIdClaim.Value, out var adminId))
            {
                throw new ArgumentException("Invalid Admin ID format.");
            }

           
            var admin = await _unitOfWork.Admins.FindAsync(a => a.AdminId == adminId);
            if (admin == null)
            {
               
                throw new KeyNotFoundException("Admin not found. Make sure the user is assigned an Admin role.");
            }

            
            var faq = _mapper.Map<Faq>(faqCreateDto);
            faq.AdminId = adminId;
            faq.CreatedDate = DateTime.UtcNow;

          
            _unitOfWork.Faq.Add(faq);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<FaqDto>(faq);
        }

        public async Task<FaqDto> UpdateFAQAsync(FaqUpdateDto faqUpdateDto, ClaimsPrincipal user)
        {
            
            var adminIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (adminIdClaim == null)
            {
                throw new UnauthorizedAccessException("Admin ID is required.");
            }

            if (!Guid.TryParse(adminIdClaim.Value, out var adminId))
            {
                throw new ArgumentException("Invalid Admin ID format.");
            }

           
            var existingFaq = await _unitOfWork.Faq.GetFAQByIdAsync(faqUpdateDto.Id);
            if (existingFaq == null)
            {
                throw new KeyNotFoundException("FAQ not found.");
            }

            _mapper.Map(faqUpdateDto, existingFaq);

            existingFaq.UpdatedDate = DateTime.UtcNow;

          
            await _unitOfWork.CommitAsync();

           
            return _mapper.Map<FaqDto>(existingFaq);
        }
        public async Task DeleteFAQAsync(int id, ClaimsPrincipal user)
        {
            
            var currentUser = await _userManager.GetUserAsync(user);

           
            if (currentUser == null || !await _userManager.IsInRoleAsync(currentUser, "Admin"))
            {
                throw new UnauthorizedAccessException("User does not have the Admin role.");
            }

           
            var existingFaq = await _unitOfWork.Faq.GetFAQByIdAsync(id);
            if (existingFaq == null)
            {
                throw new KeyNotFoundException("FAQ not found.");
            }

           
            existingFaq.IsActive = false; 
            existingFaq.UpdatedBy = currentUser.Id; 
            existingFaq.UpdatedDate = DateTime.UtcNow; 

            try
            {
                
                _unitOfWork.Faq.Update(existingFaq);
                await _unitOfWork.CommitAsync(); 
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception("Error while deleting the FAQ in the database", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while deleting the FAQ", ex);
            }
        }

        public async Task<FaqHomePageDTO> GetFaqHomePage(int index = 0, int size = 20)
        {
           
            int total;

            var faqs = _unitOfWork.Faq.GetMultiPaging(
                filter: faq => faq.IsActive,
                total: out total,
                index: index, 
                size: size,
                includes: new[] { "Admin", "Admin.AspNetUser" } 
            );

          
            var totalRecordCount = _unitOfWork.Faq.Count(faq => faq.IsActive);

          
            if (faqs == null || !faqs.Any())
            {
                throw new KeyNotFoundException("No FAQs found.");
            }

          
            var faqDtos = _mapper.Map<IEnumerable<FaqDto>>(faqs);

           
            return new FaqHomePageDTO
            {
                FAQs = faqDtos.ToList(), 
                TotalRecordCount = totalRecordCount 
            };
        }

    }
}
