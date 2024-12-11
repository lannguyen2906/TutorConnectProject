using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Enum;

namespace TutoRum.Services.ViewModels
{
    public class VerificationStatusDto
    {
        public EntityTypeName EntityType { get; set; }
        public Guid GuidId { get; set; }
        public int Id { get; set; }
        public bool IsVerified { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
