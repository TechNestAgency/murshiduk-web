using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public interface ICityRepository : IRepository<city>
    {
        city GetCityByID(long id);
        IQueryable<city> GetCitiesBySearch(int? Page_No, string Search_Data, string Filter_Value, int region_id);
    }
}