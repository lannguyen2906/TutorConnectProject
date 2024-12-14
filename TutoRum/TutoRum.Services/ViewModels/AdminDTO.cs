using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutoRum.Services.ViewModels
{
    public class AdminDTO
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public string RoleId { get; set; }
        public string Status { get; set; }
    }

    public class AssignRoleAdminDto
    {
        public Guid UserId { get; set; }
        public string? Position { get; set; }
        public DateTime? HireDate { get; set; }
        public decimal? Salary { get; set; }
        public int? SupervisorId { get; set; }
    }

    public class ViewAccount
    {
        public Guid UserId { get; set; }
        public string? Fullname { get; set; }
        public DateTime? Dob { get; set; }
        public bool? Gender { get; set; }
        public string? AddressId { get; set; }
        public string? AddressDetail { get; set; }
        public string? Status { get; set; }
        public bool? LockoutEnabled { get; set; }
        public List<string> Roles { get; set; } = new();
    }

    public class AdminMenuAction
    {
        public string? Href { get; set; }

        public int? NumberOfAction { get; set; }
    }

    public class FilterDto
    {
        public string? Search { get; set; } // Tìm kiếm theo ID, tên, email
        public string? Status { get; set; } // Trạng thái: "Pending", "Approved", "Rejected"
        public DateTime? StartDate { get; set; } // Lọc từ ngày đăng ký
        public DateTime? EndDate { get; set; } // Lọc đến ngày đăng ký
    }

}
