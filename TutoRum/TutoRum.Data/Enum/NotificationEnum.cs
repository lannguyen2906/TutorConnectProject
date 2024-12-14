using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutoRum.Data.Enum
{
    public enum NotificationType
    {
        GeneralUser, // Thông báo chung cho user
        AdminTutorApproval, // Yêu cầu admin kiểm duyệt gia sư
        AdminContractApproval, // Yêu cầu admin kiểm duyệt hợp đồng
        TutorStudentRequest, // Gia sư nhận yêu cầu kết nối từ học viên
        TutorInvoiceConfirmation, // Gia sư nhận xác nhận hóa đơn từ học viên
        StudentInvoiceApproval, // Học viên nhận xác nhận hóa đơn từ gia sư
        TutorRegistrationPending, // Chờ xét duyệt
        TutorRegistrationResult, // Kết quả xét duyệt
        TutorLearnerSubjectPending,
        TutorLearnerSubjectResult,
        TutorLearnerSubjectContractPending,
        TutorLearnerSubjectContractResult,
        BillingEntryAdd,
        TutorInfoUpdated,
        AdminTutorInfoUpdated
    }
}
