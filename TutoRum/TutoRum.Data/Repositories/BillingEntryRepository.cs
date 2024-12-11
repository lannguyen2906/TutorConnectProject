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
    public class BillingEntryRepository : RepositoryBase<BillingEntry>, IBillingEntryRepository
    {
        public BillingEntryRepository(IDbFactory dbFactory)
            : base(dbFactory) { }

        public async Task<List<BillingEntry>> GetBillingEntriesByTutorLearnerSubjectIdAsync(int tutorLearnerSubjectId)
        {

            return await DbContext.BillingEntries.Where(be => be.TutorLearnerSubjectId == tutorLearnerSubjectId).ToListAsync();
        }
    }
}
