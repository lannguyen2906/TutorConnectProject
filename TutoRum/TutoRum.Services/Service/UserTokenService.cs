using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.Models;
using TutoRum.Services.IService;

namespace TutoRum.Services.Service
{
    public class UserTokenService : IUserTokenService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserTokenService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<string>> GetUserTokensByUserIdAsync(Guid userId)
        {
            return await _unitOfWork.UserTokens.GetUserTokensByUserIdAsync(userId);
        }

        public async Task SaveTokenAsync(Guid userId, string fcmToken, string deviceType)
        {
            // Kiểm tra xem user đã có token nào chưa
            var existingToken = await _unitOfWork.UserTokens.FindAsync(ut => ut.UserId == userId);

            if (existingToken != null)
            {
                // Nếu đã tồn tại, cập nhật token và device type
                existingToken.FcmToken = fcmToken;
                existingToken.DeviceType = deviceType;
                existingToken.UpdatedDate = DateTime.Now; // Thêm field này nếu cần để biết thời điểm cập nhật
            }
            else
            {
                // Nếu chưa tồn tại, thêm token mới
                var newUserToken = new UserToken
                {
                    UserId = userId,
                    FcmToken = fcmToken,
                    DeviceType = deviceType,
                    CreatedDate = DateTime.Now,
                };
                _unitOfWork.UserTokens.Add(newUserToken);
            }

            // Lưu thay đổi
            await _unitOfWork.CommitAsync();
        }


        public async Task RemoveTokensAsync(List<string> invalidTokens)
        {
           await _unitOfWork.UserTokens.RemoveTokensAsync(invalidTokens);
        }

    }
}
