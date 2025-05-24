using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public class AlertTypeRepository : GenericRepository<AlertType>, IAlertTypeRepository
    {
        public AlertType GetAlertTypeByID(long id)
        {
            return table.Where(u => u.Id == id).SingleOrDefault();
        }


    }
}