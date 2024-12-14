using System;
using System.Collections.Generic;
using TutoRum.Data.Abstract;

namespace TutoRum.Data.Models
{
    public partial class Post : Auditable
    {
        public int PostId { get; set; }
        public Guid? AdminId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? Subcontent { get; set; }
        public string? Thumbnail { get; set; }
        public string? Status { get; set; }
        public int? PostType { get; set; }

        public virtual Admin? Admin { get; set; }
        public virtual PostCategory? PostCategory { get; set; }

    }
}