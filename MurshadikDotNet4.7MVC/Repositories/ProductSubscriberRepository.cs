using MurshadikCP.Controllers;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public class ProductSubscriberRepository : GenericRepository<product_subscribers>, IProductSubscriberRepository
    {
        public Data DeleteProductSubscriber(long marketid, long productid, long uid)
        {
            Data d = new Data();
            product_subscribers ps = table.Where(x => x.market_id == marketid && x.product_id == productid && x.user_id == uid).FirstOrDefault();
            if (ps != null)
            {
                Delete(ps.id);
                Save();
                d.message = "successfully deleted product subscriber";
            }
            else
            {
                d.message = "record not exists against this market and product";
            }

            d.status = true;
            d.data = null;
            return d;
        }

        public Data ProductSubscriber(long marketid, long productid, int onincrease, long uid)
        {
            Data d = new Data();
            product_subscribers ps = table.Where(x => x.market_id == marketid && x.product_id == productid && x.user_id == uid).FirstOrDefault();
            if (ps == null)
            {
                try
                {
                    ps = new product_subscribers();
                    ps.created_at = DateTime.Now;
                    ps.is_active = true;
                    ps.market_id = marketid;
                    ps.product_id = productid;
                    ps.on_increase = onincrease;
                    ps.user_id = uid;
                    Insert(ps);
                    d.message = "successfully insert product subscriber";
                }
                catch (Exception ex)
                {
                    d.message = ex.Message.ToString();
                    d.data = ex.InnerException.InnerException.Message.ToString();
                }
            }
            else
            {
                ps.updated_at = DateTime.Now;
                ps.on_increase = onincrease;
                Update(ps);
                d.message = "successfully updated product subscriber";
            }

            Save();
            d.status = true;
            return d;
        }
    }
}