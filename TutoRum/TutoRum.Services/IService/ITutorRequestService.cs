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
    public interface ITutorRequestService
    {
        Task<int> CreateTutorRequestAsync(TutorRequestDTO tutorRequestDto, ClaimsPrincipal user);
        Task<TutorRequestDTO> GetTutorRequestByIdAsync(int tutorRequestId);
        Task<PagedResult<TutorRequestDTO>> GetAllTutorRequestsAsync(TutorRequestHomepageFilterDto filter, int pageIndex = 0, int pageSize = 20);
        Task<bool> UpdateTutorRequestAsync(int tutorRequestId, TutorRequestDTO tutorRequestDto, ClaimsPrincipal user);
        Task<bool> ChooseTutorForTutorRequestAsync(int tutorRequestId, Guid tutorId, ClaimsPrincipal user);
        Task<bool> AddTutorToRequestAsync(int tutorRequestId, Guid tutorId);
        Task<TutorRequestWithTutorsDTO> GetListTutorsByTutorRequestAsync(int tutorRequestId);
        Task<PagedResult<ListTutorRequestDTO>> GetTutorRequestsByLearnerIdAsync(Guid learnerId, int pageIndex = 0, int pageSize = 20);
        Task SendTutorRequestEmailAsync(int tutorRequestId, Guid tutorID);
        Task<PagedResult<ListTutorRequestDTO>> GetTutorRequestsAdmin(TutorRequestFilterDto filter, ClaimsPrincipal user, int pageIndex = 0, int pageSize = 20);
        Task<PagedResult<ListTutorRequestForTutorDto>> GetTutorRequestsByTutorIdAsync(Guid tutorId, int pageIndex = 0, int pageSize = 20);
        Task<TutorLearnerSubjectDetailDto> GetTutorLearnerSubjectInfoByTutorRequestId(ClaimsPrincipal user, int tutorRequestId);
        Task<bool> CloseTutorRequestAsync(int tutorRequestId, ClaimsPrincipal user);

    }
}
