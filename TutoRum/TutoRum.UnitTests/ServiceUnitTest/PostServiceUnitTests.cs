using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Enum;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.IRepositories;
using TutoRum.Data.Models;
using TutoRum.Services.Service;
using TutoRum.Services.ViewModels;

namespace TutoRum.UnitTests.ServiceUnitTest
{
    [TestFixture]
    public class PostServiceUnitTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<UserManager<AspNetUser>> _userManagerMock;
        private PostService _postService;
        private Mock<IPostCategoryRepository> _mockPostCategoryRepository;



        [SetUp]
        public void SetUp()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _userManagerMock = new Mock<UserManager<AspNetUser>>(
                Mock.Of<IUserStore<AspNetUser>>(), null, null, null, null, null, null, null, null);
            _mockPostCategoryRepository = new Mock<IPostCategoryRepository>();
            _postService = new PostService(_unitOfWorkMock.Object, _userManagerMock.Object);
        }

        [Test]
        public void GetAllPostCategoryAsync_ShouldThrowException_WhenUserIsNull()
        {
            // Arrange
            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((AspNetUser)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _postService.GetAllPostCategoryAsync(new ClaimsPrincipal()));
            Assert.That(ex.Message, Is.EqualTo("User not found."));
        }

        [Test]
        public void GetAllPostCategoryAsync_ShouldThrowUnauthorizedAccessException_WhenUserIsNotAdmin()
        {
            // Arrange
            var user = new AspNetUser();
            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.IsInRoleAsync(user, AccountRoles.Admin)).ReturnsAsync(false);

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(() => _postService.GetAllPostCategoryAsync(new ClaimsPrincipal()));
        }

        [Test]
        public void GetAllPostCategoryAsync_ShouldThrowException_WhenNoPostCategoriesFound()
        {
            // Arrange
            var user = new AspNetUser();
            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.IsInRoleAsync(user, AccountRoles.Admin)).ReturnsAsync(true);

            _mockPostCategoryRepository.Setup(repo => repo.GetAll(It.IsAny<string[]>())).Returns((IEnumerable<PostCategory>)null);
            _unitOfWorkMock.Setup(uow => uow.PostCategories).Returns(_mockPostCategoryRepository.Object);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _postService.GetAllPostCategoryAsync(new ClaimsPrincipal()));
            Assert.That(ex.Message, Is.EqualTo("No post categories found."));
        }

        [Test]
        public async Task GetAllPostCategoryAsync_ShouldReturnPostCategories_WhenPostCategoriesExist()
        {
            // Arrange
            var user = new AspNetUser();
            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.IsInRoleAsync(user, AccountRoles.Admin)).ReturnsAsync(true);

            var postCategories = new List<PostCategory>
        {
            new PostCategory { PostType = 1, PostName = "Category1" },
            new PostCategory { PostType = 2, PostName = "Category2" }
        };

            _mockPostCategoryRepository.Setup(repo => repo.GetAll(It.IsAny<string[]>())).Returns(postCategories);

            _unitOfWorkMock.Setup(uow => uow.PostCategories).Returns(_mockPostCategoryRepository.Object);

            // Act
            var result = await _postService.GetAllPostCategoryAsync(new ClaimsPrincipal());

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Category1", result.First().PostName);
        }


        [Test]
        public async Task GetPostByPostIdAsync_ShouldReturnPost_WhenPostExists()
        {
            // Arrange
            var postId = 1;
            var post = new Post
            {
                PostId = postId,
                Title = "Test Post",
                Thumbnail = "test-thumbnail.png",
                Content = "Test Content",
                Subcontent = "Test Subcontent",
                Status = "Published",
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
                PostCategory = new PostCategory { PostName = "Test Category" },
                Admin = new Admin { AspNetUser = new AspNetUser { Fullname = "Admin Name", AvatarUrl = "admin-avatar.png" } }
            };

            _unitOfWorkMock.Setup(uow => uow.Posts.GetPostByIdAsync(postId)).ReturnsAsync(post);

            // Act
            var result = await _postService.GetPostByPostIdAsync(postId, new ClaimsPrincipal());

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(postId, result.PostId);
            Assert.AreEqual("Test Post", result.Title);
            Assert.AreEqual("Test Category", result.PostCategoryName);
        }

        [Test]
        public void GetPostByPostIdAsync_ShouldThrowException_WhenPostNotFound()
        {
            // Arrange
            var postId = 1;
            _unitOfWorkMock.Setup(uow => uow.Posts.GetPostByIdAsync(postId)).ReturnsAsync((Post)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _postService.GetPostByPostIdAsync(postId, new ClaimsPrincipal()));
            Assert.That(ex.Message, Is.EqualTo("Post not found."));
        }


        [Test]
        public async Task AddPostAsync_ShouldAddPost_WhenUserIsAdmin()
        {
            // Arrange
            var currentUser = new AspNetUser { Id = Guid.NewGuid(), Fullname = "Admin" };
            var addPostDto = new AddPostsDTO { Title = "New Post", Content = "Content", SubContent = "Subcontent", Thumbnail = "thumbnail.png", PostType = 1 };

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);
            _userManagerMock.Setup(um => um.IsInRoleAsync(currentUser, AccountRoles.Admin)).ReturnsAsync(true);
            _unitOfWorkMock.Setup(uow => uow.Posts.Add(It.IsAny<Post>()));
            // Act
            await _postService.AddPostAsync(addPostDto, new ClaimsPrincipal());

            // Assert
            _unitOfWorkMock.Verify(uow => uow.Posts.Add(It.IsAny<Post>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.CommitAsync(), Times.Once);
        }

        [Test]
        public void AddPostAsync_ShouldThrowException_WhenUserIsNull()
        {
            // Arrange
            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((AspNetUser)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _postService.AddPostAsync(new AddPostsDTO(), new ClaimsPrincipal()));
            Assert.That(ex.Message, Is.EqualTo("User not found."));
        }

        [Test]
        public void AddPostAsync_ShouldThrowUnauthorizedAccessException_WhenUserIsNotAdmin()
        {
            // Arrange
            var currentUser = new AspNetUser { Id = Guid.NewGuid(), Fullname = "User" };
            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);
            _userManagerMock.Setup(um => um.IsInRoleAsync(currentUser, AccountRoles.Admin)).ReturnsAsync(false);

            // Act & Assert
            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(() => _postService.AddPostAsync(new AddPostsDTO(), new ClaimsPrincipal()));
            Assert.That(ex.Message, Is.EqualTo("User does not have the Admin role."));
        }

        [Test]
        public async Task UpdatePostAsync_ShouldUpdatePost_WhenPostExists()
        {
            // Arrange
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };
            var postDto = new UpdatePostDTO { PostId = 1, Title = "Updated Title", Content = "Updated Content", Status = "Published" };
            var post = new Post { PostId = postDto.PostId };

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);
            _userManagerMock.Setup(um => um.IsInRoleAsync(currentUser, "Admin")).ReturnsAsync(true);
            _unitOfWorkMock.Setup(uow => uow.Posts.GetSingleById(postDto.PostId)).Returns(post);

            // Act
            await _postService.UpdatePostAsync(postDto, new ClaimsPrincipal());

            // Assert
            _unitOfWorkMock.Verify(uow => uow.CommitAsync(), Times.Once);
        }

        [Test]
        public void UpdatePostAsync_ShouldThrowException_WhenPostNotFound()
        {
            // Arrange
            var currentUser = new AspNetUser { Id = Guid.NewGuid() };
            var postDto = new UpdatePostDTO { PostId = 1, Title = "Updated Title", Content = "Updated Content", Status = "Published" };
            var post = new Post { PostId = postDto.PostId };

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);
            _userManagerMock.Setup(um => um.IsInRoleAsync(currentUser, "Admin")).ReturnsAsync(true);
            _unitOfWorkMock.Setup(uow => uow.Posts.GetSingleById(postDto.PostId)).Returns((Post)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _postService.UpdatePostAsync(postDto, new ClaimsPrincipal()));
            Assert.That(ex.Message, Is.EqualTo("Post not found."));
        }

        [Test]
        public async Task DeletePostAsync_ShouldDeletePost_WhenPostExistsAndUserIsAdmin()
        {
            // Arrange
            var postId = 1;
            var currentUser = new AspNetUser { Id = Guid.NewGuid(), Fullname = "Admin" };
            var post = new Post { PostId = postId, Status = "Published" };

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);
            _userManagerMock.Setup(um => um.IsInRoleAsync(currentUser, "Admin")).ReturnsAsync(true);
            _unitOfWorkMock.Setup(uow => uow.Posts.GetSingleById(postId)).Returns(post);

            // Act
            await _postService.DeletePostAsync(postId, new ClaimsPrincipal());

            // Assert
            Assert.AreEqual(PostStatus.Hidden.ToString(), post.Status);
            _unitOfWorkMock.Verify(uow => uow.Posts.Update(post), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.CommitAsync(), Times.Once);
        }

        [Test]
        public void DeletePostAsync_ShouldThrowUnauthorizedAccessException_WhenUserIsNotAdmin()
        {
            // Arrange
            var currentUser = new AspNetUser { Id = Guid.NewGuid(), Fullname = "User" };

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);
            _userManagerMock.Setup(um => um.IsInRoleAsync(currentUser, "Admin")).ReturnsAsync(false);

            // Act & Assert
            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(() => _postService.DeletePostAsync(1, new ClaimsPrincipal()));
            Assert.That(ex.Message, Is.EqualTo("User does not have the Admin role."));
        }

        [Test]
        public void DeletePostAsync_ShouldThrowKeyNotFoundException_WhenPostNotFound()
        {
            // Arrange
            var currentUser = new AspNetUser { Id = Guid.NewGuid(), Fullname = "Admin" };

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);
            _userManagerMock.Setup(um => um.IsInRoleAsync(currentUser, "Admin")).ReturnsAsync(true);
            _unitOfWorkMock.Setup(uow => uow.Posts.GetSingleById(It.IsAny<int>())).Returns((Post)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _postService.DeletePostAsync(1, new ClaimsPrincipal()));
            Assert.That(ex.Message, Is.EqualTo("Post not found."));
        }

       

        [Test]
        public void GetAllPostsAsync_ShouldThrowUnauthorizedAccessException_WhenUserIsNotAdmin()
        {
            // Arrange
            var currentUser = new AspNetUser { Id = Guid.NewGuid(), Fullname = "User" };

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);
            _userManagerMock.Setup(um => um.IsInRoleAsync(currentUser, AccountRoles.Admin)).ReturnsAsync(false);

            // Act & Assert
            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(() => _postService.GetAllPostsAsync(new ClaimsPrincipal()));
            Assert.That(ex.Message, Is.EqualTo("User does not have the Admin role."));
        }

      

    }
}
