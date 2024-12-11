using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Models;
using TutoRum.Services.Service;
using TutoRum.Services.ViewModels;

namespace TutoRum.Services.IService
{
    public interface ITutorLearnerSubjectService
    {
        Task RegisterLearnerForTutorAsync(RegisterLearnerDTO learnerDto, ClaimsPrincipal user);
        Task<List<SubjectDetailDto>> GetSubjectDetailsByUserIdAsync(Guid userId,string viewType);
        Task<List<SubjectDetailDto>> GetClassroomsByUserIdAsync(Guid userId, string viewType);
        Task<TutorLearnerSubjectSummaryDetailDto> GetTutorLearnerSubjectSummaryDetailByIdAsync(int tutorLearnerSubjectId, ClaimsPrincipal user);
        Task UpdateClassroom(int tutorLearnerSubjectId, RegisterLearnerDTO learnerDto, ClaimsPrincipal user);
        Task<bool> VerifyTutorLearnerContractAsync(int tutorLearnerSubjectId, ClaimsPrincipal user, bool isVerified, string? reason);
        Task<PagedResult<ContractDto>> GetListContractAsync(ContractFilterDto filter, int pageIndex = 0, int pageSize = 20);
        Task<bool> CreateClassForLearnerAsync(CreateClassDTO classDto, int tutorRequestId, ClaimsPrincipal user);
        Task<bool> CloseClassAsync(int tutorLearnerSubjectId, ClaimsPrincipal user);
        Task HandleContractUploadAndNotifyAsync(Guid tutorId, int tutorLearnerSubjectId, string contractUrl);
    }
}
