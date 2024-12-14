using Microsoft.AspNetCore.Identity;
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
    public interface IAccountService
    {
        public Task<SignInResponseDto?> GetCurrentUser(ClaimsPrincipal user);
        public Task<IdentityResult> SignUpAsync(SignUpModel model);
        public Task<AspNetUser> GetUserByIdAsync(string userId);
        public Task<SignInResponseDto> SignInAsync(SignInModel model);
        Task<string> GenerateEmailConfirmationTokenAsync(AspNetUser user);
        Task<IdentityResult> ConfirmEmailAsync(AspNetUser user, string token);
        Task<AspNetUser> GetUserByEmailAsync(string email);
        Task<string> GeneratePasswordResetTokenAsync(AspNetUser user);
        Task<IdentityResult> ResetPasswordAsync(AspNetUser user, string token, string newPassword);
        Task<IEnumerable<ViewAccount>> GetAllAccountsAsync();
        Task BlockUserAsync(Guid userId);
        Task UnblockUserAsync(Guid userId);
    }
}
