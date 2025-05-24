using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public class UnitRepository : GenericRepository<unit>, IUnitRepository
    {
        public unit GetUnitByID(long id)
        {
            return table.Where(u => u.id == id).SingleOrDefault();
        }
    }
}