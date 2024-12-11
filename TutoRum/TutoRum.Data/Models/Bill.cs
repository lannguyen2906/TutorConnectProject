using System;
using System.Collections.Generic;
using TutoRum.Data.Abstract;

namespace TutoRum.Data.Models
{
    public partial class Bill : Auditable
    {
        public Bill()
        {
            Payments = new HashSet<Payment>();
            BillingEntries = new HashSet<BillingEntry>();
        }

        public int BillId { get; set; }
        public decimal? Discount { get; set; }
        public decimal? Deduction { get; set; }
        public DateTime? StartDate { get; set; }
        public string? Description { get; set; }
        public decimal? TotalBill { get; set; }
        public string? Status { get; set; }
        public bool? ISApprove { get; set; }
        public bool IsPaid { get; set; } // Đã thanh toán thành công hay chưa
        public DateTime? PaymentDate { get; set; } // Ngày thanh toán thành công

        // Thêm thuộc tính BillingEntries để liên kết nhiều BillingEntry
        public virtual ICollection<BillingEntry> BillingEntries { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
