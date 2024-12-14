using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Models;

namespace TutoRum.Data.Infrastructure
{
    public class DbFactory : Disposable, IDbFactory
    {
        private readonly ApplicationDbContext _dbContext;

        public DbFactory(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ApplicationDbContext Init()
        {
            return _dbContext;
        }
    }
}
