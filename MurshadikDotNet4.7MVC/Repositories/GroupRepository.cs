using MurshadikCP.Controllers;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public class GroupRepository : GenericRepository<group>, IGroupRepository
    {
        public Data GetGroups()
        {
            Data d = new Data();
            var groups = _db.groups.Select(p => new { p.id, p.region_id, p.region.name, p.name_ar }).ToList();
            d.status = true;
            d.message = "successfully fetch the records";
            d.data = groups;
            return d;
        }
    }
}