using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutoRum.Services.ViewModels
{
   public class FaqDto
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? AdminPosition { get; set; }  
        public string? AdminFullname { get; set; }
    }

    public class FaqCreateDto
    {
        public string Question { get; set; }
        public string Answer { get; set; }
      
        public bool IsActive { get; set; } = true; 
    }
    public class FaqUpdateDto
    {
        public int Id { get; set; } 
        public string Question { get; set; }
        public string Answer { get; set; } 
        public bool IsActive { get; set; } = true; 
    }
    public class FaqHomePageDTO
    {
        public List<FaqDto> FAQs { get; set; }
        public int TotalRecordCount { get; set; } 
    }
}
