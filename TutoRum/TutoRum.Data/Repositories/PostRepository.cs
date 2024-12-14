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
    internal class PostRepository : RepositoryBase<Post>, IPostRepository
    {
        public PostRepository(IDbFactory dbFactory)
            : base(dbFactory) { }


        public async Task<Post> GetPostByIdAsync(int id)
        {
            return await DbContext.Posts
                .Include(p => p.PostCategory)
                .Include(p => p.Admin.AspNetUser)
                .FirstOrDefaultAsync(p => p.PostId == id);
        }

        public async Task<IEnumerable<Post>> GetPostsByAdminIdAsync(Guid adminId)
        {
            return await DbContext.Posts
                                 .Include(p => p.PostCategory)
                                 .Where(post => post.AdminId == adminId)
                                 .ToListAsync();
        }
    }
}
