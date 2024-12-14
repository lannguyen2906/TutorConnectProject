using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Models;

namespace TutoRum.Data.IRepositories
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        decimal GetTotalPaidAmountByTutorLearnerSubjectIdAsync(int tutorLearnerSubjectId);
        decimal GetTotalEarningsThisMonth(Guid tutorId, DateTime startOfMonth, DateTime endOfMonth);
    }
}
