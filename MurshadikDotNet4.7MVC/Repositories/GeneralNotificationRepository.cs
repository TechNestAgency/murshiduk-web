using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public class GeneralNotificationRepository : GenericRepository<general_notifications>, IGeneralNotificationRepository
    {
        public general_notifications GetGeneralNotificationByID(long id)
        {
            return table.Where(u => u.id == id).SingleOrDefault();
        }
    }
}