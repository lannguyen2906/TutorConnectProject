using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Models;

namespace TutoRum.Services.ViewModels
{
    public class PostsHomePageDTO
    {
        public IEnumerable<PostsDTO> Posts { get; set; }
        public int? TotalRecordCount { get; set; }
        public int? Page_number { get; set; }
        public int? Page_size { get; set; }
    }

    public class PostsDTO
    {
        public int PostId { get; set; }
        public string? Title { get; set; }
        public string? Thumbnail { get; set; }
        public string? Content { get; set; }
        public string? SubContent { get; set; }
        public string? Status { get; set; }
        public string? PostCategoryName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public AuthorDTO? Author { get; set; }
        public int? PostType { get; set; }
    }

    public class AuthorDTO
    {
        public string? AuthorName { get; set; }
        public string? Avatar { get; set; }

    }


    public class AddPostsDTO
    {
        public string? Title { get; set; }
        public string? Thumbnail { get; set; }
        public string? Content { get; set; }
        public string? SubContent { get; set; }
        public string? Status { get; set; }
        public int? PostType { get; set; }

    }

    public class UpdatePostDTO
    {
        public int PostId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? Subcontent { get; set; }
        public string? Thumbnail { get; set; }
        public string? Status { get; set; }
        public int? PostType { get; set; }
    }
}
