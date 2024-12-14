using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Models;

namespace TutoRum.Data.IRepositories
{
    public interface ISubjectRepository : IRepository<Subject>
    {
        Task<Subject> GetSubjectByIdAsync(int id);
    }
}
