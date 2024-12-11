using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Enum;
using TutoRum.Services.ViewModels;

namespace TutoRum.Services.IService
{
    public interface IAdminSevice
    {
        Task AssignRoleAdmin(AssignRoleAdminDto dto, ClaimsPrincipal user);
        Task<AdminHomePageDTO> GetAllTutors(
            ClaimsPrincipal user,
            FilterDto filterDto,
            int index = 0,
            int size = 20);
        Task SetVerificationStatusAsync(EntityTypeName entityType, Guid guidId, int id, bool isVerified, string reason, ClaimsPrincipal user);

        Task<List<AdminMenuAction>> GetAdminMenuActionAsync(ClaimsPrincipal user);
    }
}
