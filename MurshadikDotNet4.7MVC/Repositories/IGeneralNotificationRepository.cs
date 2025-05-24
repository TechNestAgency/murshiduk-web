using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public interface IGeneralNotificationRepository : IRepository<general_notifications>
    {
        general_notifications GetGeneralNotificationByID(long id);
    }
}