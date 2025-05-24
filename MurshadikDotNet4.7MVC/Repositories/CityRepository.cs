using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public class CityRepository : GenericRepository<city>, ICityRepository
    {
        public city GetCityByID(long id)
        {
            return table.Where(u => u.id == id).SingleOrDefault();
        }

        public IQueryable<city> GetCitiesBySearch(int? Page_No, string Search_Data, string Filter_Value, int region_id)
        {
            int No_Of_Page = (Page_No ?? 1);

            if (Search_Data != null && Search_Data != "")
            {
                Page_No = 1;
            }
            else
            {
                Search_Data = Filter_Value;
            }

            var region = _db.regions.Where(x => x.name_ar.Contains(Search_Data)).FirstOrDefault();
            var cities = from c in _db.cities
                         where c.region_id == region_id
                         select c;
            if (!String.IsNullOrEmpty(Search_Data))
            {
                cities = cities.Where(s => s.name_ar.Contains(Search_Data)
                                       || s.name_en.Contains(Search_Data));
            }
            if (region != null)
            {
                cities = _db.cities.Where(s => s.region_id == region.id);
            }
            
            return cities.OrderByDescending(x => x.id);
        }

        public void AddCity(city c)
        {
            Insert(c);
        }

        public void UpdateCity(city c)
        {
            Update(c);
        }

        public object GetAllCities()
        {
            return _db.cities.ToList();
        }

    }
}