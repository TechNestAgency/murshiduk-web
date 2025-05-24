using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public class AlertHazardRepository : GenericRepository<AlertHazard>, IAlertHazardRepository
    {
        public AlertHazard GetAlertHazardByID(long id)
        {
            return table.Where(u => u.Id == id).SingleOrDefault();
        }
    }
}