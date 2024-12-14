using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Models;
using TutoRum.FE.Common;
using TutoRum.FE.Controllers;
using TutoRum.Services.IService;
using TutoRum.Services.ViewModels;

namespace TutoRum.UnitTests.TutoRum.FE.UnitTest.Controller
{
    [TestFixture]
    public class PostControllerTests
    {
        private Mock<IPostService> _postServiceMock;
        private PostController _controller;

        [SetUp]
        public void SetUp()
        {
            _postServiceMock = new Mock<IPostService>();
            _controller = new PostController(_postServiceMock.Object);
        }

       

        [Test]
        public async Task GetAllPost_ShouldReturnUnauthorized_WhenUnauthorizedAccessExceptionThrown()
        {
            // Arrange
            _postServiceMock
                .Setup(s => s.GetAllPostsAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws(new UnauthorizedAccessException("Unauthorized"));

            // Act
            var result = await _controller.GetAllPost();

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(403, objectResult.StatusCode);

            var apiResponse = objectResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("Unauthorized", apiResponse.Message);
        }

        [Test]
        public async Task GetAllPost_ShouldReturnNotFound_WhenKeyNotFoundExceptionThrown()
        {
            // Arrange
            _postServiceMock
                .Setup(s => s.GetAllPostsAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws(new KeyNotFoundException("Posts not found"));

            // Act
            var result = await _controller.GetAllPost();

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult.Value);

            var apiResponse = notFoundResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("Posts not found", apiResponse.Message);
        }


        [Test]
        public async Task GetPostsHomePage_ShouldReturnOk_WhenPostsExist()
        {
            // Arrange
            var postsHomePage = new PostsHomePageDTO
            {
                Page_number = 0,
                Page_size = 10,
                TotalRecordCount = 2,
                Posts = new List<PostsDTO>
        {
            new PostsDTO { PostId = 1, Title = "Post 1" },
            new PostsDTO { PostId = 2, Title = "Post 2" }
        }
            };

            _postServiceMock.Setup(s => s.GetPostsHomePage(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(postsHomePage);

            // Act
            var result = await _controller.GetPostsHomePage(0, 10);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult.Value);
            Assert.IsInstanceOf<ApiResponse<PostsHomePageDTO>>(okResult.Value);
        }

        [Test]
        public async Task GetPostsHomePage_ShouldReturnServerError_WhenExceptionThrown()
        {
            // Arrange
            _postServiceMock.Setup(s => s.GetPostsHomePage(It.IsAny<int>(), It.IsAny<int>())).Throws(new System.Exception("Some error"));

            // Act
            var result = await _controller.GetPostsHomePage(0, 10);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
        }

        [Test]
        public async Task AddPost_ShouldReturnOk_WhenPostIsAdded()
        {
            // Arrange
            var postDto = new AddPostsDTO { Title = "New Post", Content = "Content of new post" };

            _postServiceMock.Setup(s => s.AddPostAsync(postDto, It.IsAny<ClaimsPrincipal>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddPost(postDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult.Value);
        }


        [Test]
        public async Task UpdatePost_ShouldReturnOk_WhenPostIsUpdated()
        {
            // Arrange
            var postDto = new UpdatePostDTO { PostId = 1, Title = "Updated Post", Content = "Updated content" };

            _postServiceMock.Setup(s => s.UpdatePostAsync(postDto, It.IsAny<ClaimsPrincipal>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdatePost(postDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult.Value);
        }

        [Test]
        public async Task UpdatePost_ShouldReturnUnauthorized_WhenUnauthorizedAccessExceptionThrown()
        {
            // Arrange
            var postDto = new UpdatePostDTO { PostId = 1, Title = "Updated Post", Content = "Updated content" };

            _postServiceMock.Setup(s => s.UpdatePostAsync(postDto, It.IsAny<ClaimsPrincipal>())).Throws(new UnauthorizedAccessException("Unauthorized"));

            // Act
            var result = await _controller.UpdatePost(postDto);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(403, objectResult.StatusCode);
            Assert.IsInstanceOf<ApiResponse<object>>(objectResult.Value);
        }

        [Test]
        public async Task UpdatePost_ShouldReturnNotFound_WhenKeyNotFoundExceptionThrown()
        {
            // Arrange
            var postDto = new UpdatePostDTO { PostId = 1, Title = "Updated Post", Content = "Updated content" };

            _postServiceMock.Setup(s => s.UpdatePostAsync(postDto, It.IsAny<ClaimsPrincipal>())).Throws(new KeyNotFoundException("Post not found"));

            // Act
            var result = await _controller.UpdatePost(postDto);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsInstanceOf<ApiResponse<object>>(notFoundResult.Value);
        }

        [Test]
        public async Task UpdatePost_ShouldReturnServerError_WhenExceptionThrown()
        {
            // Arrange
            var postDto = new UpdatePostDTO { PostId = 1, Title = "Updated Post", Content = "Updated content" };

            _postServiceMock.Setup(s => s.UpdatePostAsync(postDto, It.IsAny<ClaimsPrincipal>())).Throws(new System.Exception("Some error"));

            // Act
            var result = await _controller.UpdatePost(postDto);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
        }


        [Test]
        public async Task DeletePost_ShouldReturnOk_WhenPostIsDeleted()
        {
            // Arrange
            int postId = 1;

            _postServiceMock.Setup(s => s.DeletePostAsync(postId, It.IsAny<ClaimsPrincipal>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeletePost(postId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult.Value);
            var response = okResult.Value as ApiResponse<int>;
            Assert.AreEqual(postId, response.Data);
        }

        [Test]
        public async Task DeletePost_ShouldReturnUnauthorized_WhenUnauthorizedAccessExceptionThrown()
        {
            // Arrange
            int postId = 1;

            _postServiceMock.Setup(s => s.DeletePostAsync(postId, It.IsAny<ClaimsPrincipal>())).Throws(new UnauthorizedAccessException("Unauthorized"));

            // Act
            var result = await _controller.DeletePost(postId);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(403, objectResult.StatusCode);
            Assert.IsInstanceOf<ApiResponse<object>>(objectResult.Value);
        }

        [Test]
        public async Task DeletePost_ShouldReturnNotFound_WhenKeyNotFoundExceptionThrown()
        {
            // Arrange
            int postId = 1;

            _postServiceMock.Setup(s => s.DeletePostAsync(postId, It.IsAny<ClaimsPrincipal>())).Throws(new KeyNotFoundException("Post not found"));

            // Act
            var result = await _controller.DeletePost(postId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsInstanceOf<ApiResponse<object>>(notFoundResult.Value);
        }

        [Test]
        public async Task DeletePost_ShouldReturnServerError_WhenExceptionThrown()
        {
            // Arrange
            int postId = 1;

            _postServiceMock.Setup(s => s.DeletePostAsync(postId, It.IsAny<ClaimsPrincipal>())).Throws(new System.Exception("Some error"));

            // Act
            var result = await _controller.DeletePost(postId);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
        }

        [Test]
        public async Task AddPost_ShouldReturnUnauthorized_WhenUnauthorizedAccessExceptionThrown()
        {
            // Arrange
            var postDto = new AddPostsDTO { Title = "New Post", Content = "Content of new post" };

            _postServiceMock.Setup(s => s.AddPostAsync(postDto, It.IsAny<ClaimsPrincipal>())).Throws(new UnauthorizedAccessException("Unauthorized"));

            // Act
            var result = await _controller.AddPost(postDto);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(403, objectResult.StatusCode);
        }

        [Test]
        public async Task AddPost_ShouldReturnServerError_WhenExceptionThrown()
        {
            // Arrange
            var postDto = new AddPostsDTO { Title = "New Post", Content = "Content of new post" };

            _postServiceMock.Setup(s => s.AddPostAsync(postDto, It.IsAny<ClaimsPrincipal>())).Throws(new System.Exception("Some error"));

            // Act
            var result = await _controller.AddPost(postDto);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
        }

        [Test]
        public async Task GetPostById_ShouldReturnOk_WhenPostExists()
        {
            // Arrange
            var post = new PostsDTO { PostId = 1, Title = "Post 1", Content = "Content of Post 1" };
            _postServiceMock.Setup(s => s.GetPostByPostIdAsync(1, It.IsAny<ClaimsPrincipal>())).ReturnsAsync(post);

            // Act
            var result = await _controller.GetPostById(1);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult.Value);
            Assert.IsInstanceOf<ApiResponse<PostsDTO>>(okResult.Value);
        }

        [Test]
        public async Task GetPostById_ShouldReturnUnauthorized_WhenUnauthorizedAccessExceptionThrown()
        {
            // Arrange
            _postServiceMock.Setup(s => s.GetPostByPostIdAsync(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>())).Throws(new UnauthorizedAccessException("Unauthorized"));

            // Act
            var result = await _controller.GetPostById(1);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(403, objectResult.StatusCode);
        }

        [Test]
        public async Task GetPostById_ShouldReturnServerError_WhenExceptionThrown()
        {
            // Arrange
            _postServiceMock.Setup(s => s.GetPostByPostIdAsync(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>())).Throws(new System.Exception("Some error"));

            // Act
            var result = await _controller.GetPostById(1);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
        }


        [Test]
        public async Task GetAllPostCategories_ShouldReturnOk_WhenCategoriesExist()
        {
            // Arrange
            var postCategories = new List<PostCategory>
    {
        new PostCategory { PostType = 1, PostName = "Category 1" },
        new PostCategory { PostType = 2, PostName = "Category 2" }
    };

            _postServiceMock.Setup(s => s.GetAllPostCategoryAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(postCategories);

            // Act
            var result = await _controller.GetAllPostCategories();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult.Value);
            Assert.IsInstanceOf<ApiResponse<IEnumerable<PostCategory>>>(okResult.Value);
        }

        [Test]
        public async Task GetAllPostCategories_ShouldReturnUnauthorized_WhenUnauthorizedAccessExceptionThrown()
        {
            // Arrange
            _postServiceMock.Setup(s => s.GetAllPostCategoryAsync(It.IsAny<ClaimsPrincipal>())).Throws(new UnauthorizedAccessException("Unauthorized"));

            // Act
            var result = await _controller.GetAllPostCategories();

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(403, objectResult.StatusCode);
        }

        [Test]
        public async Task GetAllPostCategories_ShouldReturnServerError_WhenExceptionThrown()
        {
            // Arrange
            _postServiceMock.Setup(s => s.GetAllPostCategoryAsync(It.IsAny<ClaimsPrincipal>())).Throws(new System.Exception("Some error"));

            // Act
            var result = await _controller.GetAllPostCategories();

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
        }
      

        [Test]
        public async Task GetPostsHomePage_ShouldReturnUnauthorized_WhenUnauthorizedAccessExceptionThrown()
        {
            // Arrange
            _postServiceMock.Setup(s => s.GetPostsHomePage(It.IsAny<int>(), It.IsAny<int>()))
                .Throws(new UnauthorizedAccessException("Unauthorized"));

            // Act
            var result = await _controller.GetPostsHomePage(0, 10);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(403, objectResult.StatusCode);
            Assert.IsInstanceOf<ApiResponse<object>>(objectResult.Value);
        }

        [Test]
        public async Task GetPostById_ShouldReturnNotFound_WhenKeyNotFoundExceptionThrown()
        {
            // Arrange
            int postId = 1; // ID của bài viết cần kiểm tra
            _postServiceMock.Setup(s => s.GetPostByPostIdAsync(postId, It.IsAny<ClaimsPrincipal>()))
                .Throws(new KeyNotFoundException("Post not found"));

            // Act
            var result = await _controller.GetPostById(postId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsInstanceOf<ApiResponse<object>>(notFoundResult.Value);
        }

        [Test]
        public async Task GetAllPostCategories_ShouldReturnNotFound_WhenKeyNotFoundExceptionThrown()
        {
            // Arrange
            _postServiceMock.Setup(s => s.GetAllPostCategoryAsync(It.IsAny<ClaimsPrincipal>()))
                .Throws(new KeyNotFoundException("No categories found"));

            // Act
            var result = await _controller.GetAllPostCategories();

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsInstanceOf<ApiResponse<object>>(notFoundResult.Value);
        }

        [Test]
        public async Task GetPostsHomePage_ShouldReturnNotFound_WhenKeyNotFoundExceptionThrown()
        {
            // Arrange
            _postServiceMock.Setup(s => s.GetPostsHomePage(It.IsAny<int>(), It.IsAny<int>()))
                .Throws(new KeyNotFoundException("No posts found"));

            // Act
            var result = await _controller.GetPostsHomePage();

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsInstanceOf<ApiResponse<object>>(notFoundResult.Value);
        }

        [Test]
        public async Task AddPost_ShouldReturnNotFound_WhenKeyNotFoundExceptionThrown()
        {
            // Arrange
            var postDto = new AddPostsDTO { Title = "New Post", Content = "Content of new post" };

            _postServiceMock.Setup(s => s.AddPostAsync(postDto, It.IsAny<ClaimsPrincipal>()))
                .Throws(new KeyNotFoundException("Post category not found"));

            // Act
            var result = await _controller.AddPost(postDto);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsInstanceOf<ApiResponse<object>>(notFoundResult.Value);
        }

    }
}
