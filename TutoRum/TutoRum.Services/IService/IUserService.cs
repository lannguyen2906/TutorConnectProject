using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Services.ViewModels;

namespace TutoRum.Services.IService
{
    public interface IUserService
    {
        Task UpdateUserProfileAsync(UpdateUserDTO userDto, ClaimsPrincipal user);
    }
}
