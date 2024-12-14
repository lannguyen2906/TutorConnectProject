using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace TutoRum.Data.Models
{
    public partial class AspNetUser : IdentityUser<Guid>
    {
        public AspNetUser()
        {
            TutorRequest = new HashSet<TutorRequest?>();
        }

        public string? Fullname { get; set; }
        public DateTime? Dob { get; set; }
        public bool? Gender { get; set; }
        public string? AvatarUrl { get; set; }
        public string? AddressId { get; set; }
        public string? DistrictId { get; set; }
        public string? WardId { get; set; }
        public string? AddressDetail { get; set; }
        public string? Status { get; set; }


        public virtual Admin? Admin { get; set; }
        public virtual Tutor? Tutor { get; set; }
        public virtual ICollection<TutorRequest> TutorRequest { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<UserToken> UserTokens { get; set; }



    }
}