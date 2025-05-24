using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public interface IUserRepository : IRepository<user>
    {
        user GetUserByID(string uID);
        UserInfo GetUserInfoByID(string uID);

        user GetUserByID(long Id);
    }


}