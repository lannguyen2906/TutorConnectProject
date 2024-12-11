using Microsoft.AspNetCore.Mvc;
using TutoRum.Data.Models;
using TutoRum.FE.Common;
using TutoRum.Services.IService;
using TutoRum.Services.Service;
using TutoRum.Services.ViewModels;
using static TutoRum.Services.ViewModels.PostsHomePageDTO;

namespace TutoRum.FE.Controllers
{
    public class PostController : ApiControllerBase
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }


        [HttpGet]
        [Route(Common.Url.User.Post.GetAllPost)]
        [ProducesResponseType(typeof(ApiResponse<PostsHomePageDTO>), 200)]
        public async Task<IActionResult> GetAllPost(int index = 0, int size = 10)
        {
            try
            {
                var posts = await _postService.GetAllPostsAsync(User, index, size);

                var response = ApiResponseFactory.Success(posts);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                var response = ApiResponseFactory.Unauthorized<object>(ex.Message);
                return StatusCode(403, response);
            }
            catch (KeyNotFoundException ex)
            {
                var response = ApiResponseFactory.NotFound<object>(ex.Message);
                return NotFound(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }

        [HttpGet]
        [Route(Common.Url.User.Post.GetPostsHomePage)]
        [ProducesResponseType(typeof(ApiResponse<PostsHomePageDTO>), 200)]
        public async Task<IActionResult> GetPostsHomePage([FromQuery] int index = 0, int size = 10)
        {
            try
            {
                var postsHomePage = await _postService.GetPostsHomePage(index, size);

                var response = new PostsHomePageDTO
                {
                    Page_number = index,
                    Page_size = size,        
                    TotalRecordCount = postsHomePage.TotalRecordCount, 
                    Posts = postsHomePage.Posts  
                };

                var apiResponse = ApiResponseFactory.Success(response);
                return Ok(apiResponse);
            }
            catch (UnauthorizedAccessException ex)
            {
                var response = ApiResponseFactory.Unauthorized<object>(ex.Message);
                return StatusCode(403, response);
            }
            catch (KeyNotFoundException ex)
            {
                var response = ApiResponseFactory.NotFound<object>(ex.Message);
                return NotFound(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }



        [HttpGet]
        [Route(Common.Url.User.Post.GetAllPostCategories)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<PostCategory>>), 200)]
        public async Task<IActionResult> GetAllPostCategories()
        {
            try
            {
                // Call the service method to get all post categories
                var postCategories = await _postService.GetAllPostCategoryAsync(User);

                var response = ApiResponseFactory.Success(postCategories);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                var response = ApiResponseFactory.Unauthorized<object>(ex.Message);
                return StatusCode(403, response);
            }
            catch (KeyNotFoundException ex)
            {
                var response = ApiResponseFactory.NotFound<object>(ex.Message);
                return NotFound(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }



        [HttpGet]
        [Route(Common.Url.User.Post.GetPostById)]
        [ProducesResponseType(typeof(ApiResponse<PostsDTO>), 200)]
        public async Task<IActionResult> GetPostById(int postId)
        {
            try
            {
                // Call the service method to get the post by ID
                var post = await _postService.GetPostByPostIdAsync(postId, User);

                var response = ApiResponseFactory.Success(post);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                var response = ApiResponseFactory.Unauthorized<object>(ex.Message);
                return StatusCode(403, response);
            }
            catch (KeyNotFoundException ex)
            {
                var response = ApiResponseFactory.NotFound<object>(ex.Message);
                return NotFound(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }


        [HttpPost]
        [Route(Common.Url.User.Post.AddPost)]
        [ProducesResponseType(typeof(ApiResponse<AddPostsDTO>), 200)]
        public async Task<IActionResult> AddPost([FromBody] AddPostsDTO postDto)
        {
            try
            {
                await _postService.AddPostAsync(postDto, User);

                var response = ApiResponseFactory.Success( postDto );
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                var response = ApiResponseFactory.Unauthorized<object>(ex.Message);
                return StatusCode(403, response);
            }
            catch (KeyNotFoundException ex)
            {
                var response = ApiResponseFactory.NotFound<object>(ex.Message);
                return NotFound(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }


        [HttpPost]
        [Route(Common.Url.User.Post.UpdatePost)]
        [ProducesResponseType(typeof(ApiResponse<UpdatePostDTO>), 200)]
        public async Task<IActionResult> UpdatePost([FromBody] UpdatePostDTO postDto)
        {
            try
            {
                await _postService.UpdatePostAsync(postDto, User);

                var response = ApiResponseFactory.Success(new { postDto });
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                var response = ApiResponseFactory.Unauthorized<object>(ex.Message);
                return StatusCode(403, response);
            }
            catch (KeyNotFoundException ex)
            {
                var response = ApiResponseFactory.NotFound<object>(ex.Message);
                return NotFound(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }


        [HttpPost]
        [Route(Common.Url.User.Post.DeletePost)]
        [ProducesResponseType(typeof(ApiResponse<int>), 200)]
        public async Task<IActionResult> DeletePost(int postID)
        {
            try
            {
                await _postService.DeletePostAsync(postID, User);
                var response = ApiResponseFactory.Success(postID);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                var response = ApiResponseFactory.Unauthorized<object>(ex.Message);
                return StatusCode(403, response);
            }
            catch (KeyNotFoundException ex)
            {
                var response = ApiResponseFactory.NotFound<object>(ex.Message);
                return NotFound(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }
    }
}
