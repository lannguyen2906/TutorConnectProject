using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TutoRum.Data.Enum;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.Models;
using TutoRum.Services.IService;
using TutoRum.Services.ViewModels;

namespace TutoRum.Services.Service
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<AspNetUser> userManager;
        private readonly SignInManager<AspNetUser> signInManager;
        private readonly IConfiguration configuration;
        private readonly IUrlHelper _urlHelper;
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;

        public AccountService(UserManager<AspNetUser> userManager, SignInManager<AspNetUser> signInManager, IConfiguration configuration, INotificationService notificationService, IUnitOfWork unitOfWork)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
        }

        public async Task<SignInResponseDto> SignInAsync(SignInModel model)
        {
            try
            {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

                if (!result.Succeeded)
                {
                    return null;
                }

                var user = await userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return null;
                }

                var roles = await userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, model.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, model.Email)
        };

                return new SignInResponseDto
                {
                    ID = user.Id,
                    AvatarUrl = user.AvatarUrl ?? "",
                    Fullname = user.Fullname ?? "",
                    Dob = user.Dob ?? null,
                    Gender = user.Gender ?? null,
                    AddressDetail = user.AddressDetail ?? null,
                    CityId = user.AddressId ?? string.Empty,
                    DistrictId = user.DistrictId ?? string.Empty,
                    WardId = user.WardId ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    Roles = roles.ToList()
                };
            }
            catch (Exception)
            {
                // Log the exception if necessary
                return null;
            }
        }




        public async Task<IdentityResult> SignUpAsync(SignUpModel model)
        {
            var user = new AspNetUser
            {
                Fullname = string.IsNullOrEmpty(model.Fullname) ? model.Email.Split('@')[0] : model.Fullname,
                Email = model.Email,
                UserName = model.Email,  // Email được sử dụng làm UserName khi Fullname không có
                AvatarUrl = "https://firebasestorage.googleapis.com/v0/b/tutorconnect-27339.appspot.com/o/avatar%2Fdefault-avatar-icon-of-social-media-user-vector.jpg?alt=media&token=27853f0b-a90f-424d-b4ca-526f00993ce0",
            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var notificationDto = new NotificationRequestDto
                {
                    UserId = user.Id,
                    Title = "Chào mừng bạn đến với TutorConnect",
                    Description = "Bạn vui lòng hoàn thiện thông tin cá nhân của mình nhé",
                    NotificationType = NotificationType.GeneralUser,
                    Href = "/user/settings/user-profile",
                };

                try
                {
                    await _notificationService.SendNotificationAsync(notificationDto, false);
                }
                catch (Exception ex)
                {
                    return IdentityResult.Failed(new IdentityError { Description = $"Notification service error: {ex.Message}" });
                }

                var roleResult = await userManager.AddToRoleAsync(user, AccountRoles.Learner);

                if (!roleResult.Succeeded)
                {
                    return roleResult;
                }
            }

            return result;
        }



        public async Task<AspNetUser> GetUserByIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            var user = await userManager.FindByIdAsync(userId);
            return user;
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(AspNetUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            return token;
        }

        public async Task<IdentityResult> ConfirmEmailAsync(AspNetUser user, string token)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            var result = await userManager.ConfirmEmailAsync(user, token);
            return result;
        }

        public async Task<AspNetUser> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            var user = await userManager.FindByEmailAsync(email);
            return user;
        }

        public async Task<string> GeneratePasswordResetTokenAsync(AspNetUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            return token;
        }


        public async Task<IdentityResult> ResetPasswordAsync(AspNetUser user, string token, string newPassword)
        {
            return await userManager.ResetPasswordAsync(user, token, newPassword);
        }

        public async Task<SignInResponseDto?> GetCurrentUser(ClaimsPrincipal user)
        {
            var currentUser = await userManager.GetUserAsync(user);

            if (currentUser == null)
            {
                return null;
            }
            var roles = await userManager.GetRolesAsync(currentUser);
            var isTutor = await userManager.IsInRoleAsync(currentUser,AccountRoles.Tutor);
            var tutor = await _unitOfWork.Tutors.FindAsync(t => t.TutorId == currentUser.Id);

            return new SignInResponseDto
            {
                ID = currentUser.Id,
                PhoneNumber = currentUser.PhoneNumber,
                Email = currentUser.Email,
                AvatarUrl = currentUser.AvatarUrl,
                Dob = currentUser.Dob,
                Fullname = currentUser.Fullname.Split('@')[0],
                Gender = currentUser.Gender,
                AddressDetail = currentUser.AddressDetail,
                CityId = currentUser.AddressId,
                DistrictId = currentUser.DistrictId,
                WardId = currentUser.WardId,
                Roles = roles.ToList(),
                Balance = isTutor ? tutor.Balance : null,
            };
        }

        public async Task<IEnumerable<ViewAccount>> GetAllAccountsAsync()
        {
            // Get all users from the UserManager
            var users =  userManager.Users.ToList();

            var accountViewModels = new List<ViewAccount>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user); 
                accountViewModels.Add(new ViewAccount
                {
                    UserId = user.Id,
                    Fullname = user.Fullname,
                    Dob = user.Dob,
                    Gender = user.Gender,
                    AddressId = user.AddressId,
                    AddressDetail = user.AddressDetail,
                    Status = user.Status,
                    LockoutEnabled = user.LockoutEnabled,
                    Roles = roles.ToList() 
                });
            }

            return accountViewModels;
        }


        public async Task BlockUserAsync(Guid userId)
        {
            // Lấy thông tin người dùng cần block
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // Cập nhật thông tin LockoutEnabled
            user.LockoutEnabled = false;

            // Cập nhật vào database
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception("Error blocking user.");
            }

        }

        public async Task UnblockUserAsync(Guid userId)
        {
            
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new Exception("User not found.");
            }

          
            if (user.LockoutEnabled)
            {
                throw new Exception("User is not locked out.");
            }

            
            user.LockoutEnabled = true;

           
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception("Error unlocking user.");
            }

           
        }


    }
}
