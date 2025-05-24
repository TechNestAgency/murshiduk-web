using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public interface ICategoryRepository : IRepository<category>
    {
        category GetCategoryByID(long id);
        //UserInfo GetUserInfoByID(string uID);
    }
}