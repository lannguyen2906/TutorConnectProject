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
    public class PaymentRepository : RepositoryBase<Payment>, IPaymentRepository
    {
        public PaymentRepository(IDbFactory dbFactory)
            : base(dbFactory) { }

        public decimal GetTotalPaidAmountByTutorLearnerSubjectIdAsync(int tutorLearnerSubjectId)
        {
            return DbContext.Payments
                .Include(p => p.Bill)
                .ThenInclude(b => b.BillingEntries)
                .Where(p => p.Bill.BillingEntries.Any(be => be.TutorLearnerSubjectId == tutorLearnerSubjectId))
                .Sum(p => p.AmountPaid ?? 0); // Đảm bảo xử lý giá trị null
        }

        public decimal GetTotalEarningsThisMonth(Guid tutorId, DateTime startOfMonth, DateTime endOfMonth)
        {
            return DbContext.Payments
                .Include(p => p.Bill)
                .ThenInclude(b => b.BillingEntries)
                .ThenInclude(be => be.TutorLearnerSubject)
                .Where(p => p.Bill.BillingEntries.Any(be => be.TutorLearnerSubject.TutorSubject.TutorId == tutorId)
                            && p.PaymentDate >= startOfMonth
                            && p.PaymentDate <= endOfMonth)
                .Sum(p => p.AmountPaid ?? 0);  // Tính tổng tiền thanh toán       
        }
    }
}
