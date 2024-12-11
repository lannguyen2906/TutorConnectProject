using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutoRum.Services.ViewModels
{
    public class PaymentDetailsDTO
    {
        public int PaymentId { get; set; } // Payment ID
        public int? BillId { get; set; } // Associated Bill ID
        public decimal? AmountPaid { get; set; } // Amount Paid
        public string PaymentMethod { get; set; } // Payment Method (e.g., "VnPay")
        public DateTime? PaymentDate { get; set; } // Date of Payment
        public string TransactionId { get; set; } // Transaction ID
        public string PaymentStatus { get; set; } // Payment Status (e.g., "Success", "Failed")
        public string OrderId { get; set; } // Order ID from the payment gateway
        public string Currency { get; set; } // Currency used in payment (e.g., "VND")
        public string BillStatus { get; set; } // Status of the associated Bill (e.g., "Paid")
        public decimal? BillTotalAmount { get; set; } // Total amount of the associated Bill
    }
}
