using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace TutoRum.Services.IService
{
    public interface IContractService
    {
        public Task<string> GenerateContractAsync(int tutorLearnerSubjectID);
    }
}
