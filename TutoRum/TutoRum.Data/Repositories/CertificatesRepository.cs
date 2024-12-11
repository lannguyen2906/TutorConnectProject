using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.IRepositories;
using TutoRum.Data.Models;

namespace TutoRum.Data.Repositories
{
    public class CertificatesRepository : RepositoryBase<Certificate>, ICertificatesRepository
    {
        public CertificatesRepository(Infrastructure.IDbFactory dbFactory)
            : base(dbFactory)
        {
        }

        public async Task<IEnumerable<Certificate>> GetAllByTutorIdAsync(Guid tutorId)
        {
            return await Task.FromResult(GetMulti(t => t.TutorId == tutorId));
        }
    }
}
