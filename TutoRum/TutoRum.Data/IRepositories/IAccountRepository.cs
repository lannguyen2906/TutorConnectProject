using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Models;


namespace TutoRum.Data.IRepositories
{
    public interface IAccountRepository : IRepository<AspNetUser>
    {

    }
}
