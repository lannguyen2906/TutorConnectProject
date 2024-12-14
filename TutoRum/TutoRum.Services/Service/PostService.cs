using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Enum;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.Models;
using TutoRum.Services.IService;
using TutoRum.Services.ViewModels;
using static TutoRum.Services.ViewModels.PostsHomePageDTO;

namespace TutoRum.Services.Service
{
    public class PostService : IPostService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AspNetUser> _userManager;

        public PostService(IUnitOfWork unitOfWork, UserManager<AspNetUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }


        public async Task<IEnumerable<PostCategory>> GetAllPostCategoryAsync(ClaimsPrincipal user)
        {
            // Get the current user
            var currentUser = await _userManager.GetUserAsync(user);

            if (currentUser == null)
            {
                throw new Exception("User not found.");
            }

            // Check if the current user is an admin
            if (!await _userManager.IsInRoleAsync(currentUser, AccountRoles.Admin))
            {
                throw new UnauthorizedAccessException("User does not have permission to view this post.");
            }

            // Retrieve all post categories from the database
            var postCategories = _unitOfWork.PostCategories.GetAll();

            if (postCategories == null || !postCategories.Any())
            {
                throw new Exception("No post categories found.");
            }

            return postCategories;
        }



        public async Task<PostsDTO> GetPostByPostIdAsync(int postId, ClaimsPrincipal user)
        {
            var post = await _unitOfWork.Posts.GetPostByIdAsync(postId);

            if (post == null)
            {
                throw new Exception("Post not found.");
            }

            return new PostsDTO
            {
                PostId = post.PostId,
                Title = post.Title,
                Thumbnail = post.Thumbnail,
                Content = post.Content,
                SubContent = post.Subcontent,
                Status = post.Status,
                CreatedDate = post.CreatedDate,
                UpdatedDate = post.UpdatedDate,
                PostCategoryName = post.PostCategory?.PostName,
                PostType = post.PostType,
                Author = post.Admin != null ? new AuthorDTO
                {
                    AuthorName = post.Admin.AspNetUser.Fullname,
                    Avatar = post.Admin.AspNetUser.AvatarUrl
                } : null
            };
        }




        public async Task AddPostAsync(AddPostsDTO post, ClaimsPrincipal user)
        {
            var currentUser = await _userManager.GetUserAsync(user);

            if (currentUser == null)
            {
                throw new Exception("User not found.");
            }
            else
            {

                if (!await _userManager.IsInRoleAsync(currentUser, AccountRoles.Admin))
                {
                    throw new UnauthorizedAccessException("User does not have the Admin role.");
                }

                var newPost = new Post
                {
                    AdminId = currentUser.Id,
                    Title = post.Title,
                    Content = post.Content,
                    Subcontent = post.SubContent,
                    Thumbnail = post.Thumbnail,
                    Status = PostStatus.Hidden.ToString(),
                    PostType = post.PostType,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = currentUser.Id

                };

                _unitOfWork.Posts.Add(newPost);
                await _unitOfWork.CommitAsync();

            }
        }

        public async Task UpdatePostAsync(UpdatePostDTO postDto, ClaimsPrincipal user)
        {
            var currentUser = await _userManager.GetUserAsync(user);
            if (currentUser == null || !await _userManager.IsInRoleAsync(currentUser, "Admin"))
                throw new UnauthorizedAccessException("User does not have the Admin role.");

            var post = _unitOfWork.Posts.GetSingleById(postDto.PostId);
            if (post == null)
                throw new KeyNotFoundException("Post not found.");

            post.Title = postDto.Title;
            post.Content = postDto.Content;
            post.Subcontent = postDto.Subcontent;
            post.Thumbnail = postDto.Thumbnail;
            post.Status = postDto.Status;
            post.PostType = postDto.PostType;
            post.UpdatedDate = DateTime.UtcNow;
            post.UpdatedBy = currentUser.Id;
            await _unitOfWork.CommitAsync();
        }

        public async Task DeletePostAsync(int postId, ClaimsPrincipal user)
        {
            var currentUser = await _userManager.GetUserAsync(user);
            if (currentUser == null || !await _userManager.IsInRoleAsync(currentUser, "Admin"))
                throw new UnauthorizedAccessException("User does not have the Admin role.");

            var post = _unitOfWork.Posts.GetSingleById(postId);
            if (post == null)
            {
                throw new KeyNotFoundException("Post not found.");
            }
            else
            {
                post.Status = PostStatus.Hidden.ToString();
                post.UpdatedDate = DateTime.UtcNow;
                post.UpdatedBy = currentUser.Id;
            }

            _unitOfWork.Posts.Update(post);
            await _unitOfWork.CommitAsync();
        }

        public async Task<PostsHomePageDTO> GetAllPostsAsync(ClaimsPrincipal user, int index = 0, int size = 20)
        {

            var currentUser = await _userManager.GetUserAsync(user);

            if (currentUser == null || !await _userManager.IsInRoleAsync(currentUser, "Admin"))
                throw new UnauthorizedAccessException("User does not have the Admin role.");

            int total;
            var posts = _unitOfWork.Posts.GetMultiPaging(
                filter: post => true,
                total: out total,
                index: index,
                size: size,
                includes: new[] { "Admin", "Admin.AspNetUser", "PostCategory" }
            );
            if (posts == null || !posts.Any())
            {
                throw new KeyNotFoundException("No posts found for the specified Admin ID.");
            }
            return new PostsHomePageDTO
            {
                Posts = posts.Select(post => new PostsDTO
                {
                    PostId = post.PostId,
                    Title = post.Title,
                    Thumbnail = post.Thumbnail,
                    Content = post.Content,
                    SubContent = post.Subcontent,
                    Status = post.Status,
                    CreatedDate = post.CreatedDate,
                    UpdatedDate = post.UpdatedDate,
                    PostCategoryName = post.PostCategory?.PostName,
                    Author = post.Admin != null ? new AuthorDTO
                    {
                        AuthorName = post.Admin.AspNetUser.Fullname,
                        Avatar = post.Admin.AspNetUser.AvatarUrl
                    } : null
                }).ToList(),
                TotalRecordCount = total
            };
        }


        public async Task<PostsHomePageDTO> GetPostsHomePage(int index = 0, int size = 20)
        {
            int total;

            var posts = _unitOfWork.Posts.GetMultiPaging(
                filter: post => true,
                total: out total,
                index: index,
                size: size,
                includes: new[] { "Admin", "Admin.AspNetUser", "PostCategory" }
            );

            var totalRecordCount = _unitOfWork.Posts.Count(post => true);

            if (posts == null || !posts.Any())
            {
                throw new KeyNotFoundException("No posts found.");
            }

            return new PostsHomePageDTO
            {
                Posts = posts.Select(post => new PostsDTO
                {
                    PostId = post.PostId,
                    Title = post.Title,
                    Thumbnail = post.Thumbnail,
                    Content = post.Content,
                    SubContent = post.Subcontent,
                    Status = post.Status,
                    CreatedDate = post.CreatedDate,
                    UpdatedDate = post.UpdatedDate,
                    PostCategoryName = post.PostCategory?.PostName,
                    Author = post.Admin != null ? new AuthorDTO
                    {
                        AuthorName = post.Admin.AspNetUser.Fullname,
                        Avatar = post.Admin.AspNetUser.AvatarUrl
                    } : null,
                    PostType = post.PostType,
                }).ToList(),
                TotalRecordCount = totalRecordCount
            };
        }



    }
}
