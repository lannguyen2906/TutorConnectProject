using System;
using System.Collections.Generic;
using TutoRum.Data.Abstract;
using TutoRum.Data.Enum;

namespace TutoRum.Data.Models
{
    public partial class Certificate : Auditable
    {
        public int CertificateId { get; set; }
        public Guid? TutorId { get; set; }
        public string? ImgUrl { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public DateTime? IssueDate { get; set; } // Ngày cấp chứng chỉ
        public DateTime? ExpiryDate { get; set; } // Ngày hết hạn chứng chỉ
        public bool? IsVerified { get; set; } // Đã được xác thực hay chưa
        public string? EntityType { get; set; } = EntityTypeName.Certificate.ToString();
        public virtual Tutor? Tutor { get; set; }
    }
}