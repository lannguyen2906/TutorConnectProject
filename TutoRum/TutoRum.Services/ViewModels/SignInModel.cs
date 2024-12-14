using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutoRum.Services.ViewModels
{
    public class SignInModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
    }

    public class SignInResponseDto
    {
        public Guid? ID { get; set; }
        public string? Fullname { get; set; }
        public DateTime? Dob { get; set; }
        public bool? Gender { get; set; }
        public string? AvatarUrl { get; set; }

        public string Email { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public string? CityId { get; set; }
        public string? DistrictId { get; set; }
        public string? WardId { get; set; }
        public string? AddressDetail { get; set; }
        public decimal? Balance { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}
