using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.IRepositories;
using TutoRum.Data.Models;
using TutoRum.Data.Repositories;

namespace TutoRum.Data.Repositories
{
    public class RateRangeRepository : RepositoryBase<RateRange>, IRateRangeRepository
    {
        public RateRangeRepository(IDbFactory dbFactory)
            : base(dbFactory) { }
    }
}
