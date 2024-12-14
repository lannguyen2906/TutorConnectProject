using System;
using System.Collections.Generic;
using TutoRum.Data.Abstract;

namespace TutoRum.Data.Models
{
    public partial class Payment : Auditable
    {
        public int PaymentId { get; set; }
        public int? BillId { get; set; }
        public decimal? AmountPaid { get; set; }
        public string? PaymentMethod { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? Currency { get; set; }
        public string? PaymentStatus { get; set; }
        public string? TransactionId { get; set; } // Mã giao dịch từ cổng VnPay
        public string? ResponseCode { get; set; } // Mã phản hồi từ cổng thanh toán
        public string? OrderId { get; set; } // Mã đơn hàng từ VnPay


        public virtual Bill? Bill { get; set; }
    }
}
