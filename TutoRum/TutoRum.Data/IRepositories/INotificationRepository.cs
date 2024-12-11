using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Models;

namespace TutoRum.Data.IRepositories
{
    public interface INotificationRepository : IRepository<Notification>
    {
        Task<List<Notification>> GetByUserIdAsync(Guid userId);
        Task MarkNotificationsAsReadAsync(List<int> ids);
    }
}
