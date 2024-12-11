using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.IRepositories;
using TutoRum.Data.Models;

namespace TutoRum.Data.Repositories
{
    public class UserTokenRepository : RepositoryBase<UserToken>, IUserTokenRepository
    {
        public UserTokenRepository(IDbFactory dbFactory)
            : base(dbFactory) { }

        public async Task<IEnumerable<string>> GetUserTokensByUserIdAsync(Guid userId)
        {
            return await DbContext.UserTokens
                .Where(ut => ut.UserId == userId)
                .Select(ut => ut.FcmToken)
                .ToListAsync();
        }
        public async Task<UserToken?> GetByTokenAndUserIdAsync(string token, Guid userId)
        {
            return await DbContext.UserTokens
                .FirstOrDefaultAsync(ut => ut.FcmToken == token && ut.UserId == userId);
        }

        public async Task RemoveTokensAsync(List<string> invalidTokens)
        {
            if (invalidTokens == null || !invalidTokens.Any())
                return;

            var tokensToRemove = await DbContext.UserTokens
                .Where(t => invalidTokens.Contains(t.FcmToken))
                .ToListAsync();

            if (tokensToRemove.Any())
            {
                // Xóa các token không hợp lệ
                DbContext.UserTokens.RemoveRange(tokensToRemove);
                await DbContext.SaveChangesAsync();
            }
        }
    }
}
