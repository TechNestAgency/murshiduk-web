using MurshadikCP.Controllers;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public class CurrentWeatherNotificationForCityRepository : GenericRepository<currentweathernotificationforCity>, ICurrentWeatherNotificationForCityRepository
    {
        public Data GetCurrentWeatherNoficationByCityId(int city_id)
        {
            Data d = new Data();
            List<currentweathernotificationforCity> wdata = GetAll().Where(x => x.city_id == city_id).ToList();
            d.data = wdata;
            d.status = true;
            d.message = wdata.Count() > 0 ? "successfully fetch records!" : "no record found!";
            return d;
        }
    }
}