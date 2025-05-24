using MurshadikCP.Controllers;
using MurshadikCP.Extensions;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public class MarketProductRepository : GenericRepository<market_products>, IMarketProductRepository
    {
        public object GetMarketProducts()
        {
            var product_categories = _db.product_categories
                           .Select(p => new { p.id, p.name });
            return product_categories;
        }

        public object GetSubscribersProduct(long userId)
        {
            List<product_subscribers> ps = _db.product_subscribers.Where(x => x.user_id == userId).ToList();
            List<fun_GetLatestProductPricesByUser_Result> final = new List<fun_GetLatestProductPricesByUser_Result>();
            foreach (var item in ps)
            {
                int mid = Convert.ToInt32(item.market_id);
                int pid = Convert.ToInt32(item.product_id);
                var result = _db.fun_GetLatestProductPricesByUser(mid, pid).FirstOrDefault();
                if (result != null)
                {
                    final.Add(result);
                }
            }

            return final;
        }

        public List<ProductPrice> GetMarketProduct(long product_id, long market_id, int filter = 0)
        {
            List<ProductPrice> pp = new List<ProductPrice>();
            if (filter == 360)
            {
                int mid = Convert.ToInt16(market_id);
                List<fun_GetLatestProductPricesbyYear_Result> price_product = _db.fun_GetLatestProductPricesbyYear(mid, product_id).ToList();
                if (price_product.Count() > 0)
                {
                    for (int i = 0; i < price_product.Count; i++)
                    {
                        ProductPrice price = new ProductPrice();
                        price.product_id = product_id;
                        price.price = price_product[i].price;
                        price.price_date = price_product[i].price_date.Value.FormattedDate();
                        pp.Add(price);
                    }
                }
            }
            else
            {
                var price_product = _db.market_products.Where(x => x.product_id == product_id && x.market_id == market_id).OrderByDescending(x => x.id).Take(filter).ToList();
                {
                    for (int i = 0; i < price_product.Count; i++)
                    {
                        ProductPrice price = new ProductPrice();
                        price.product_id = product_id;
                        price.price = price_product[i].price;
                        price.price_date = price_product[i].price_date.Value.FormattedDate();
                        pp.Add(price);
                    }
                }
            }

            return pp;
        }

        public Data GetMarketDetails(int market_id)
        {
            Data d = new Data();

            var markets = _db.markets.Include("region").Where(x => x.id == market_id).FirstOrDefault();
            MarketProduct _marketproduct = new MarketProduct();
            MarketData m = new MarketData();
            if (markets != null)
            {
                m.id = markets.id;
                m.marketname = markets.marketname;
                m.location = markets.location;
                m.address = markets.address;
                m.open_at = markets.open_at;
                m.close_at = markets.close_at;
                m.contact_person = markets.contact_person;
                m.phone = markets.phone;
                m.is_active = markets.is_active;
                m.region_Name = markets.region.name;
                m.market_image = markets.market_image;

                List<market_products> mp = new List<market_products>();
                List<ProductPrice> productPrices = new List<ProductPrice>();

                List<fun_GetLatestProductPrices_Result> productprices = _db.fun_GetLatestProductPrices(market_id).ToList();
                if (productprices.Count() <= 0)
                {
                    _marketproduct.product_price = null;
                    _marketproduct.products = null;
                    _marketproduct.market = m;

                    d.status = true;
                    d.message = "Successfully fetch the records!";
                    d.data = _marketproduct;
                    return d;
                }
                long pid = productprices.First().id;
                mp = _db.market_products.Where(x => x.product_id == pid).OrderByDescending(x => x.id).Take(7).ToList();

                foreach (var item in mp)
                {
                    ProductPrice pp = new ProductPrice();
                    pp.product_id = pid;
                    pp.price = item.price;
                    pp.price_date = item.price_date.Value.FormattedDate();
                    productPrices.Add(pp);
                }

                _marketproduct.product_price = productPrices;
                _marketproduct.products = productprices;
                _marketproduct.market = m;
            }

            d.status = true;
            d.message = "Successfully fetch the records!";
            d.data = _marketproduct;
            return d;
        }
    }
}