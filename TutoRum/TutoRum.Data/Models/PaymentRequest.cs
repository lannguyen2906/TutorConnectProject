using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Abstract;

namespace TutoRum.Data.Models
{
    public class PaymentRequest : Auditable
    {
        public int PaymentRequestId { get; set; }
        public Guid TutorId { get; set; } // Foreign key to Tutor
        public string BankCode { get; set; } = string.Empty; // Bank code
        public string AccountNumber { get; set; } = string.Empty; // Bank account number
        public string FullName { get; set; } = string.Empty; // Account holder's name
        public decimal Amount { get; set; } // Requested withdrawal amount
        public string Status { get; set; } = "Pending"; // Payment request status (Pending, Approved, Rejected)
        public string VerificationStatus { get; set; } = "Pending"; // Trạng thái xác minh (Pending, Approved, Rejected)
        public Guid Token {  get; set; }
        // Các trường bổ sung cho admin
        public bool IsPaid { get; set; } = false; // Đã thanh toán hay chưa
        public DateTime? PaidDate { get; set; } // Ngày thanh toán
        public string? AdminNote { get; set; } // Ghi chú từ admin (nếu cần)

        // Navigation property
        public virtual Tutor Tutor { get; set; } = null!;
    }
}
