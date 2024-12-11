using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Services.ViewModels;

namespace TutoRum.Services.IService
{
    public interface ITutorService
    {
        Task RegisterTutorAsync(AddTutorDTO tutorDto, ClaimsPrincipal user);
        Task<TutorDto> GetTutorByIdAsync(Guid id);
        Task<TutorHomePageDTO> GetTutorHomePage(TutorFilterDto? tutorFilterDto, int index = 0, int size = 20 );
        Task DeleteTutorAsync(Guid tutorId, ClaimsPrincipal user);
        //Task UpdateTutorAsync(UpdateTutorDTO tutorDto, ClaimsPrincipal user);

        Task<List<TutorMajorDto>> GetAllTutorsWithMajorsAndMinorsAsync();
        Task<List<TutorRatingDto>> GetAllTutorsWithFeedbackAsync();

        Task<WalletOverviewDto> GetWalletOverviewDtoAsync(ClaimsPrincipal user);

        Task<List<TutorSummaryDto>> GetTopTutorAsync(int size);
        Task UpdateTutorInfoAsync(UpdateTutorInforDTO tutorDto, ClaimsPrincipal user);
    }
}
