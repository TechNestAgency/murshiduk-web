using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public interface IUserRole: IRepository<roles_permission>
    {
        //roles_permission myRoles();
        bool canView(int page_id);
        bool canUpdate(int page_id);
        bool canDelete(int page_id);
        bool canInsert(int page_id);
    }
}