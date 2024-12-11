using System;
using System.Collections.Generic;
using TutoRum.Data.Abstract;

namespace TutoRum.Data.Models
{
    public partial class TeachingLocation : Auditable
    {
        public TeachingLocation()
        {
            Tutors = new HashSet<TutorTeachingLocations>();
        }

        public int TeachingLocationId { get; set; }
        public string? CityId { get; set; }
        public string? DistrictId { get; set; }


        public ICollection<TutorTeachingLocations> Tutors { get; set; }
    }
}