using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public interface ILabRepository : IRepository<lab>
    {
        lab GetLabByID(long id);
    }
}