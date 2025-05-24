using MurshadikCP.Controllers;
using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public class MarketRepository : GenericRepository<market>, IMarketRepository
    {
        public market GetMarketByID(long id)
        {
            return table.Where(u => u.id == id).SingleOrDefault();
        }

        public Data GetAllMarkets(int regionId = 0)
        {
            Data d = new Data();
            if (regionId > 0)
            {
                var markets = _db.markets.Include("region").Where(x => x.region_id == regionId && x.is_active == true).ToList();

                List<MarketData> md = new List<MarketData>();
                if (markets != null)
                {
                    for (int i = 0; i < markets.Count; i++)
                    {
                        MarketData m = new MarketData();
                        m.id = markets[i].id;
                        m.marketname = markets[i].marketname;
                        m.location = markets[i].location;
                        m.address = markets[i].address;
                        m.open_at = markets[i].open_at;
                        m.close_at = markets[i].close_at;
                        m.contact_person = markets[i].contact_person;
                        m.phone = markets[i].phone;
                        m.is_active = markets[i].is_active;
                        m.region_Name = markets[i].region.name;
                        m.region_id = markets[i].region_id.Value;
                        m.market_image = markets[i].market_image;
                        md.Add(m);
                    }
                }
                d.status = true;
                d.message = "successfully fetch single record!";
                d.data = md;
                return d;
            }
            else
            {
                var markets = _db.markets.Include("region").Where(x => x.is_active == true).ToList();

                List<MarketData> md = new List<MarketData>();
                if (markets != null)
                {
                    for (int i = 0; i < markets.Count; i++)
                    {
                        MarketData m = new MarketData();
                        m.id = markets[i].id;
                        m.marketname = markets[i].marketname;
                        m.location = markets[i].location;
                        m.address = markets[i].address;
                        m.open_at = markets[i].open_at;
                        m.close_at = markets[i].close_at;
                        m.contact_person = markets[i].contact_person;
                        m.phone = markets[i].phone;
                        m.is_active = markets[i].is_active;
                        m.region_Name = markets[i].region.name;
                        m.region_id = markets[i].region_id.Value;
                        m.market_image = markets[i].market_image;
                        md.Add(m);
                    }
                }
                d.status = true;
                d.message = "successfully fetch single record!";
                d.data = md;
                return d;
            }
        }
    }
}