using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public class BugTrackerRepository : GenericRepository<bug_tracker>, IBugTrackerRepository
    {
        public bug_tracker GetBugTrackerByID(long id)
        {
            return table.Where(u => u.id == id).SingleOrDefault();
        }
    }
}