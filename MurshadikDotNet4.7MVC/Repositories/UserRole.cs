using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace MurshadikCP.Repositories
{
    public class UserRole : GenericRepository<roles_permission>, IUserRole
    {
        public bool canDelete(int page_id)
        {
            return false;
        }

        public bool canInsert(int page_id)
        {
            return false;
        }

        public bool canUpdate(int page_id)
        {
            return false;
        }

        public bool canView(int page_id)
        {
            return false;
        }
    }
}