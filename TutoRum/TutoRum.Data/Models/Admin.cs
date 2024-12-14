using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TutoRum.Data.Abstract;

namespace TutoRum.Data.Models
{
    public partial class Admin : Auditable
    {
        public Admin()
        {
            Posts = new HashSet<Post>();
            Faqs = new HashSet<Faq>();
        }

        [Key, ForeignKey("AspNetUser")]
        public Guid AdminId { get; set; }

        public string? Position { get; set; }
        public DateTime? HireDate { get; set; }
        public decimal? Salary { get; set; }
        public int? SupervisorId { get; set; }

        public virtual AspNetUser AspNetUser { get; set; } 
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Faq> Faqs { get; set; }
    }
}