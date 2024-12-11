using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutoRum.Services.ViewModels
{

    public class BillingEntryDTOS
    {
        public int totalRecords { get; set; }
        public List<BillingEntryDTO> BillingEntries { get; set; }   
    }


    public class BillingEntryDTO
    {
        public int? BillingEntryID { get; set; }
        public int? BillId { get; set; }
        public int? TutorLearnerSubjectId { get; set; }
        public decimal? Rate { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string? Description { get; set; }
        public decimal? TotalAmount { get; set; }
    }

    public class AdddBillingEntryDTO
    {
        public int TutorLearnerSubjectId { get; set; }
        public decimal? Rate { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string? Description { get; set; }
        public decimal? TotalAmount { get; set; }
    }


    public class UpdateBillingEntryDTO
    {
        public decimal? Rate { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string? Description { get; set; }
        public decimal? TotalAmount { get; set; }
    }

    public class BillingEntryDetailsDTO
    {
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public decimal Rate { get; set; }
    }

    public class CalculateTotalAmountRequest
    {
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public decimal Rate { get; set; }
    }

}
