using MurshadikCP.Controllers;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public class CallDetailRepository : GenericRepository<calldetail>, ICallDetailRepository
    {
        public Data UpdateCallStatus(int status, int type, Int32 to_user, string call_id, long User_Id)
        {
            Data d = new Data();
            calldetail cd = new calldetail();
            cd.status = status;
            cd.type = type;
            cd.user_to = to_user;
            cd.user_from = User_Id;
            cd.call_id = call_id;
            cd.created_at = DateTime.Now;
            Insert(cd);
            Save();
            d.status = true;
            d.message = "successfully";
            d.data = cd.id;
            return d;
        }
    }
}