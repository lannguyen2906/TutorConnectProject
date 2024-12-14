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
    public interface IPostService
    {
        Task<PostsDTO> GetPostByPostIdAsync(int postId, ClaimsPrincipal user);
        Task<PostsHomePageDTO> GetAllPostsAsync(ClaimsPrincipal user, int index = 0, int size = 20);
        Task AddPostAsync(AddPostsDTO postDto, ClaimsPrincipal user);
        Task UpdatePostAsync(UpdatePostDTO postDto, ClaimsPrincipal user);
        Task DeletePostAsync(int postId, ClaimsPrincipal user);
        Task<IEnumerable<PostCategory>> GetAllPostCategoryAsync(ClaimsPrincipal user);
        Task<PostsHomePageDTO> GetPostsHomePage(int index = 0, int size = 20);
    }
}
