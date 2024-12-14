using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Models;

namespace TutoRum.Services.IService
{
    public interface IUserTokenService
    {
        Task SaveTokenAsync(Guid userId, string token, string DeviceType);
        Task<IEnumerable<string>> GetUserTokensByUserIdAsync (Guid userId);
        Task RemoveTokensAsync(List<string> invalidTokens);

    }
}
