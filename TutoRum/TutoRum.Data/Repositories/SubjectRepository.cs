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
    public class SubjectRepository : RepositoryBase<Subject>, ISubjectRepository
    {
        public SubjectRepository(IDbFactory dbFactory)
            : base(dbFactory) { }

        public async Task<Subject> GetSubjectByIdAsync(int id)
        {
            return await DbContext.Subjects 
                .Where(subject => subject.SubjectId == id) 
                .FirstOrDefaultAsync(); 
        }
    }
}
