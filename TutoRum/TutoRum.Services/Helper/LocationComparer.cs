using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutoRum.Services.Helper
{
    public class LocationComparer : IEqualityComparer<object>
    {
        public bool Equals(object x, object y)
        {
            if (x == null || y == null) return false;

            var loc1 = x.GetType().GetProperty("CityName")?.GetValue(x, null)?.ToString();
            var dist1 = x.GetType().GetProperty("DistrictName")?.GetValue(x, null)?.ToString();
            var loc2 = y.GetType().GetProperty("CityName")?.GetValue(y, null)?.ToString();
            var dist2 = y.GetType().GetProperty("DistrictName")?.GetValue(y, null)?.ToString();

            return string.Equals(loc1, loc2, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(dist1, dist2, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(object obj)
        {
            if (obj == null) return 0;

            var city = obj.GetType().GetProperty("CityName")?.GetValue(obj, null)?.ToString() ?? string.Empty;
            var district = obj.GetType().GetProperty("DistrictName")?.GetValue(obj, null)?.ToString() ?? string.Empty;

            return city.ToLower().GetHashCode() ^ district.ToLower().GetHashCode();
        }
    }
}
