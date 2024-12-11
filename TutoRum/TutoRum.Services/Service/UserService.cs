using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Enum;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.Models;
using TutoRum.Services.Helper;
using TutoRum.Services.IService;
using TutoRum.Services.ViewModels;

namespace TutoRum.Services.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly APIAddress _apiAddress;
        public UserService(IUnitOfWork unitOfWork, UserManager<AspNetUser> userManager, APIAddress apiAddress)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _apiAddress = apiAddress;

        }

        public async Task UpdateUserProfileAsync(UpdateUserDTO userDto, ClaimsPrincipal user)
        {
            var currentUser = await _userManager.GetUserAsync(user);
            if (currentUser == null)
            {
                throw new UnauthorizedAccessException("User not found.");
            }

            var loggedInUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (loggedInUserId == null)
            {
                throw new UnauthorizedAccessException("Unable to identify the logged-in user.");
            }

            if (currentUser.Id.ToString() != loggedInUserId)
            {
                throw new UnauthorizedAccessException("User does not have permission to update this profile.");
            }

            // Kiểm tra xem Fullname có hợp lệ không
            if (string.IsNullOrEmpty(userDto.Fullname))
            {
                throw new ArgumentException("Fullname cannot be empty.");
            }

            // Cập nhật thông tin cá nhân
            currentUser.Fullname = userDto.Fullname ?? currentUser.Fullname;
            currentUser.Dob = userDto.Dob ?? currentUser.Dob;
            currentUser.Gender = userDto.Gender ?? currentUser.Gender;
            currentUser.AvatarUrl = userDto.AvatarUrl ?? currentUser.AvatarUrl;
            currentUser.AddressId = userDto.CityId ?? currentUser.AddressId;
            currentUser.DistrictId = userDto.DistrictId ?? currentUser.DistrictId;
            currentUser.WardId = userDto.WardId ?? currentUser.WardId;
            currentUser.AddressDetail = userDto.AddressDetail ?? currentUser.AddressDetail;
            currentUser.PhoneNumber = userDto.PhoneNumber ?? currentUser.PhoneNumber;

            _unitOfWork.Accounts.Update(currentUser);
            await _unitOfWork.CommitAsync();
        }




    }


}
