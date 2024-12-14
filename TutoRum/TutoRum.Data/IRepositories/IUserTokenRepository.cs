using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Models;

namespace TutoRum.Data.IRepositories
{
    public interface IUserTokenRepository : IRepository<UserToken>
    {
        Task<IEnumerable<string>> GetUserTokensByUserIdAsync(Guid userId);
        Task<UserToken?> GetByTokenAndUserIdAsync(string token, Guid userId);
        Task RemoveTokensAsync(List<string> invalidTokens);

    }
}
