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
    public class FAQRepository : RepositoryBase<Faq>, IFAQRepository
    {
        public FAQRepository(IDbFactory dbFactory)
            : base(dbFactory) { }

        public async Task<IEnumerable<Faq>> GetAllFAQsAsync()
        {

            return await DbContext.Faq
                .Include(f => f.Admin)
                .ThenInclude(a => a.AspNetUser)
                .Where(f => f.IsActive)
                .ToListAsync();
           
        }

        public async Task<Faq> GetFAQByIdAsync(int id)
        {

            return await DbContext.Faq
                .Include(f => f.Admin)
                    .ThenInclude(a => a.AspNetUser)
                .FirstOrDefaultAsync(f => f.Id == id && f.IsActive);
        }
    }
}
