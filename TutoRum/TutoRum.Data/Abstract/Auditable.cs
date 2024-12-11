using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutoRum.Data.Abstract
{
    public class Auditable
    {
        public DateTime? CreatedDate { set; get; }

        public Guid? CreatedBy { set; get; }

        public DateTime? UpdatedDate { set; get; }

        public Guid? UpdatedBy { set; get; }

        public string? reasonDesc { get; set; }

        public bool IsDelete { get; set; }

    }
}
