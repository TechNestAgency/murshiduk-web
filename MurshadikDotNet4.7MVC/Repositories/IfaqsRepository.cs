using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public interface IfaqsRepository : IRepository<faq>
    {
        faq GetFaqByID(long id);
        IQueryable<faq> GetFaqsBySearch(int? Page_No, string Search_Data, string Filter_Value);
    }
}