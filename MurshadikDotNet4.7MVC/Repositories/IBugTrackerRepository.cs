﻿using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public interface IBugTrackerRepository : IRepository<bug_tracker>
    {
        bug_tracker GetBugTrackerByID(long id);
    }
}