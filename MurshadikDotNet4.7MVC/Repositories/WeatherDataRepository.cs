using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public class WeatherDataRepository : GenericRepository<WeatherData>, IWeatherDataRepository
    {
        public Object GetWeatherDataForCity(string cid)
        {
            
            var wde = GetAll().Where(wd => wd.city_identifier == cid).OrderByDescending(x => x.id).Select(w => w.weather).FirstOrDefault();
            if (wde != null)
            {
                return JsonConvert.DeserializeObject<WeatherJson>(wde);
            }

            return null;
        }
    }
}