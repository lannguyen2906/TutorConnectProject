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
    public class NotificationRepository : RepositoryBase<Notification>, INotificationRepository
    {
        public NotificationRepository(IDbFactory dbFactory)
            : base(dbFactory) { }

        public async Task<List<Notification>> GetByUserIdAsync(Guid userId)
        {
            return await DbContext.Notifications
               .Where(n => n.UserId == userId)
               .OrderByDescending(n => n.CreatedDate)
               .ToListAsync();
        }
        public async Task MarkNotificationsAsReadAsync(List<int> ids)
        {
            await DbContext.Notifications
                .Where(n => ids.Contains(n.NotificationId))
                .ForEachAsync(notification => notification.IsRead = true);

            await DbContext.SaveChangesAsync();
        }
    }
}
