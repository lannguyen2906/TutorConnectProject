using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.IRepositories;
using TutoRum.Data.Models;

namespace TutoRum.Data.Repositories
{
    public class BillRepository : RepositoryBase<Bill>, IBillRepository
    {
        public BillRepository(IDbFactory dbFactory)
           : base(dbFactory) { }
    }
}
