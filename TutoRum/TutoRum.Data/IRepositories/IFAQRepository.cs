using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Models;

namespace TutoRum.Data.IRepositories
{
    public interface  IFAQRepository : IRepository<Faq>
    {
        Task<IEnumerable<Faq>> GetAllFAQsAsync();
        Task<Faq> GetFAQByIdAsync(int id);
    }
}
