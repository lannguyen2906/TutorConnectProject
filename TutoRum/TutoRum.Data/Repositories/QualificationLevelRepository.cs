using Microsoft.EntityFrameworkCore;
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
    public class QualificationLevelRepository : RepositoryBase<QualificationLevel>, IQualificationLevelRepository
    {
        public QualificationLevelRepository(IDbFactory dbFactory)
            : base(dbFactory) { }

        public async Task<IEnumerable<QualificationLevel>> GetAllQualificationLevelsAsync()
        {
            return await DbContext.QualificationLevel.ToListAsync();
        }
    }
}
