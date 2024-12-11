using System;
using System.Collections.Generic;

namespace TutoRum.Data.Models
{
    public partial class AspNetUserLogin
    {
        public string LoginProvider { get; set; } = null!;
        public string ProviderKey { get; set; } = null!;
        public Guid UserId { get; set; }
        public string? ProviderDisplayName { get; set; }

        public virtual AspNetUser User { get; set; } = null!;
    }
}
