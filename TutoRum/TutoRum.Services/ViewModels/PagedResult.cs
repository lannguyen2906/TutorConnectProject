using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutoRum.Services.ViewModels
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalRecords { get; set; }

        public static implicit operator PagedResult<T>(PagedResult<ListTutorRequestDTO> v)
        {
            throw new NotImplementedException();
        }
    }

}
