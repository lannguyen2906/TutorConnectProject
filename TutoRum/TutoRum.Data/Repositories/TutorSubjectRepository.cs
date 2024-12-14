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
    public class TutorSubjectRepository : RepositoryBase<TutorSubject>, ITutorSubjectRepository
    {
        public TutorSubjectRepository(IDbFactory dbFactory)
            : base(dbFactory) { }

        
    }
}
