using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Abstract;

namespace TutoRum.Data.Models
{
    public class Faq : Auditable
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Admin")]
        public Guid AdminId { get; set; }
        public string? Question { get; set; }
        public string? Answer { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual Admin Admin { get; set; }
    }
}
