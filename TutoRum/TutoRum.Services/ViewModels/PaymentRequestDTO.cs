using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutoRum.Services.ViewModels
{
    public class CreatePaymentRequestDTO
    {
        public string BankCode { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    public class PaymentRequestDTO
    {
        public int PaymentRequestId { get; set; }
        public Guid TutorId { get; set; }
        public string BankCode { get; set; }
        public string AccountNumber { get; set; }
        public bool IsPaid { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string VerificationStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? PaidDate { get; set; }
        public string? AdminNote { get; set; }
        public string? TutorName { get; set; } // Tutor's name for display
    }

    public class RejectPaymentRequestDTO
    {
        public string RejectionReason { get; set; }
    }

    public class UpdatePaymentRequestDTO
    {
        public string BankCode { get; set; }
        public string AccountNumber { get; set; }
        public string FullName { get; set; }
        public decimal Amount { get; set; }
    }

    public class PaymentRequestFilterDTO
    {
        public int? PaymentRequestId { get; set; }
        public string? TutorName { get; set; } // Tên gia sư (tìm kiếm)
        public bool? IsPaid { get; set; } // Trạng thái thanh toán
        public string? VerificationStatus { get; set; } // Trạng thái kiểm duyệt
        public DateTime? FromDate { get; set; } // Ngày bắt đầu
        public DateTime? ToDate { get; set; } // Ngày kết thúc
    }

}
