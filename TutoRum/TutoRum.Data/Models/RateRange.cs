using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutoRum.Data.Models
{
    public class RateRange
    {
        public int Id { get; set; }
        public string Level { get; set; } // Tiểu học, Trung học, Đại học, v.v.
        public decimal MinRate { get; set; } // Giá thấp nhất
        public decimal MaxRate { get; set; } // Giá cao nhất
        public string Description { get; set; } // Mô tả cấp độ
    }
}
