using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Models;

namespace TutoRum.Data.Models
{
    public class TutorTeachingLocations
    {
        public Guid TutorId { get; set; }
        public Tutor Tutor { get; set; } 

        // Khóa ngoại liên kết với TeachingLocation
        public int TeachingLocationId { get; set; }
        public TeachingLocation TeachingLocation { get; set; }  



    }
}
