using System;
using System.Collections.Generic;
using TutoRum.Data.Abstract;

namespace TutoRum.Data.Models
{
    public partial class BillingEntry : Auditable
    {
        public int BillingEntryId { get; set; }
        public int? TutorLearnerSubjectId { get; set; }
        public decimal? Rate { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string? Description { get; set; }
        public decimal? TotalAmount { get; set; }
        public bool IsDraft { get; set; }

        // Thêm khóa ngoại BillId
        public int? BillId { get; set; }
        public virtual Bill? Bill { get; set; } 

        public virtual TutorLearnerSubject? TutorLearnerSubject { get; set; }
    }
}
