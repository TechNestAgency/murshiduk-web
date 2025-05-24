using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public interface IMarketRepository : IRepository<market>
    {
        market GetMarketByID(long id);
    }
}