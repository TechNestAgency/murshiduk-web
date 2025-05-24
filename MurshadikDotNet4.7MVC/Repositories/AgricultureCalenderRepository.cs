using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public class AgricultureCalenderRepository : GenericRepository<Agriculture_Calender>, IAgricultureCalenderRepository
    {
        public object GetCalenderArticles()
        {
            return GetAll().Select(p => new { p.title, p.description, p.file_name }).ToList();
        }


    }
}