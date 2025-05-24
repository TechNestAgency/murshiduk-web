using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public interface IAlertTypeRepository : IRepository<AlertType>
    {
        AlertType GetAlertTypeByID(long id);
    }
}