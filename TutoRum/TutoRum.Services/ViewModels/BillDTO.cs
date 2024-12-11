using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutoRum.Services.ViewModels
{
    public class BillDTO
    {
        public int BillId { get; set; }
        public decimal? Discount { get; set; }
        public DateTime? StartDate { get; set; }
        public string? Description { get; set; }
        public decimal? TotalBill { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class BillDTOS
    {
        public int TotalRecords { get; set; }
        public List<BillDTO> Bills { get; set; }
    }

    public class BillingEntryInBIllDTO
    {
        public int BillingEntryId { get; set; }
        public int? TutorLearnerSubjectId { get; set; }
        public decimal? Rate { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string? Description { get; set; }
        public decimal? TotalAmount { get; set; }
    }

    public class BillDetailsDTO
    {
        public int BillId { get; set; }
        public decimal? Discount { get; set; }
        public decimal? Deduction { get; set; }
        public DateTime? StartDate { get; set; }
        public string? Description { get; set; }
        public decimal? TotalBill { get; set; }
        public bool? IsApprove { get; set; }
        public bool? IsPaid { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? LearnerEmail { get; set; }

        public List<BillingEntryDTO> BillingEntries { get; set; }
    }
}
