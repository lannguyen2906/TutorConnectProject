using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutoRum.Services.ViewModels
{
    public class UserDTO
    {
    }

    public class UpdateUserDTO
    {
        public string? Fullname { get; set; }
        public DateTime? Dob { get; set; }
        public bool? Gender { get; set; }
        public string? AvatarUrl { get; set; }
        public string? AddressDetail { get; set; }
        public string? CityId { get; set; }
        public string? DistrictId { get; set; }
        public string? WardId { get; set; }
        public string? PhoneNumber { get; set; }
    }


}
