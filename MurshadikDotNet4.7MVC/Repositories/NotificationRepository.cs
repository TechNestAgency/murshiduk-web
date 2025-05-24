using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public class NotificationRepository : GenericRepository<notification>, INotificationRepository
    {
        public notification GetNotificationByID(long id)
        {
            return table.Where(u => u.id == id).SingleOrDefault();
        }

        public void DeleteAllNotification(long uid)
        {
            List<notification> notifications = _db.notifications.Where(x => x.user_id == uid).ToList();
            table.RemoveRange(notifications);
            _db.SaveChanges();
        }

        public List<notification> GetAllNotification(long uid, DateTime dt)
        {
            return _db.notifications.Where(x => x.user_id == uid && x.created_at > dt).OrderByDescending(x => x.created_at).ToList();
        }
    }
}