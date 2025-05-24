using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using MurshadikCP.Models;
using MurshadikCP.Models.DB;

namespace MurshadikCP.Controllers
{
    [Authorize]
    public class marketsController : BaseController
    {
        //UserInfo ui = new UserInfo();

        // GET: markets
        // Index views shows markets list
        public ActionResult Index(int? Page_No)
        {
            if (currentPageID("markets") > 0)
            {
                if (!CurrentUser.canView(currentPageID("markets")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            int pagesize = 10;
            int No_Of_Page = (Page_No ?? 1);
            int marketCount = 0;
            List<market> markets = new List<market>();
            ViewBag.role = CurrentUser.RoleName;
            if (CurrentUser.multiplemarketids != null)
            {
                marketCount = CurrentUser.multiplemarketids.Split(',').Count();
                if (marketCount > 1)
                {
                    long[] mid = CurrentUser.multiplemarketids.Split(',').Select(long.Parse).ToArray();
                    markets = db.markets.Where(x => mid.Contains(x.id)).Include(m => m.region).Include(m => m.user).OrderByDescending(x => x.id).ToList();
                }
                else
                {
                    return RedirectToAction("View");
                }
            }
            else if (CurrentUser.RoleId == (int)Role.MarketAdmin || CurrentUser.RoleId == (int)Role.SuperAdmin || CurrentUser.RoleId == (int)Role.Manager)
            {
                markets = db.markets.Include(m => m.region).Include(m => m.user).OrderByDescending(x => x.id).ToList();
            }

            ViewBag.region_id = new SelectList(db.regions.ToList(), "id", "name");

            return View(markets.ToPagedList(No_Of_Page, pagesize));
        }

        // GET: markets/Create
        public ActionResult Create()
        {
            if (currentPageID("markets") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("markets")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (CurrentUser.RoleId == (int)Role.SuperAdmin || CurrentUser.RoleId == (int)Role.MarketAdmin || CurrentUser.RoleId == (int)Role.Manager)
            {
                ViewBag.region_id = new SelectList(db.regions, "id", "name");
                return View();
            }
            else
            {
                return RedirectToAction("NotAllow", "Custom");
            }
        }

        // POST: markets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(market market, string open_at, string close_at, HttpPostedFileBase image)
        {
            if (currentPageID("markets") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("markets")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            TimeSpan fromtime = DateTime.Parse(open_at).TimeOfDay;
            TimeSpan toTime = DateTime.Parse(close_at).TimeOfDay;

            Guid imgGuid = Guid.NewGuid();
            
            if (image != null && image.ContentLength > 0)
            {
                var img = imgGuid.ToString() + Path.GetExtension(image.FileName);
                var path = Path.Combine(Server.MapPath("~/Media/Images/"), img);
                image.SaveAs(path);
                market.market_image = "/Media/Images/" + img;
            }

            market.open_at = fromtime;
            if (market.location == null && market.location == "")
            {
                market.location = "24.579345,46.720413";
            }
            
            market.close_at = toTime;
            market.created_by = CurrentUser.Id;
            market.created_at = DateTime.Now;
            db.markets.Add(market);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: markets/View/5
        public ActionResult View(long? id, string search = null)
        {
            if (currentPageID("markets") > 0)
            {
                if (!CurrentUser.canView(currentPageID("markets")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (id == null)
            {
                if(CurrentUser.marketid > 0)
                    id = CurrentUser.marketid;
                else
                    return RedirectToAction("Index");
            }

            var mp = db.market_products.Where(x => x.market_id == id).Select(p => p.product_id).Distinct().ToList();
            if (mp != null)
            {
                var p = db.products.Where(x => !mp.Contains(x.id)).Select(x => new { id = x.id, product_name = x.product_name + " - " + x.product_origin + " - " + x.unit.name }).ToList();
                ViewBag.products = new SelectList(p, "id", "product_name");
            }
            ViewBag.role = CurrentUser.RoleName;
            market market = new market();
            if (CurrentUser.multiplemarketids != null)
            {
                if (CurrentUser.multiplemarketids.Contains(id.ToString()))
                {
                    market = db.markets.Find(id);//(CurrentUser.marketid);
                    if (search != null && search != "")
                    {
                        market.market_products = market.market_products.Where(x => x.product.product_name.ToLower().Contains(search.ToLower()) ||
                        x.product.product_categories.name.Contains(search)).ToList();
                        ViewBag.searching = search;
                    }
                }
                else
                {
                    ViewBag.role = CurrentUser.RoleName;
                    return RedirectToAction("NotAllow", "Custom");
                }
            }
            else
            {
                market = db.markets.Find(id);
                if (search != null && search != "")
                {
                    market.market_products = market.market_products.Where(x => x.product.product_name.ToLower().Contains(search.ToLower()) ||
                    x.product.product_categories.name.Contains(search)).ToList();
                    ViewBag.searching = search;
                }
            }
            if (market == null)
            {
                return HttpNotFound();
            }

            return View(market);
        }

        public PartialViewResult RefillMarketProductChart(long marketid, long productid)
        {
            Dictionary<string, decimal> marketProductChartsData = new Dictionary<string, decimal>();
            TimeSpan daysToSub = new System.TimeSpan(6, 0, 0, 0);
            var startDate = DateTime.Today.Subtract(daysToSub).StartOfDay();
            var dt = startDate;
            var endDate = DateTime.Today.EndOfDay();

            var products = db.market_products
                        .Where(cd => cd.market_id == marketid && cd.product_id == productid && cd.created_at >= startDate && cd.created_at <= endDate)
                        .GroupBy(cd => DbFunctions.TruncateTime(cd.created_at)).ToArray();

            //decimal? productLastPrice = db.market_products
            //    .Where(cd => cd.market_id == marketid && cd.product_id == productid && cd.created_at <= startDate).OrderByDescending(x => x.id).Select(x => x.price).FirstOrDefault();
            //decimal plp = productLastPrice != null ? productLastPrice.Value : 0;

            var abcproducts = products.Select(cd => new
            {
                Value = cd.LastOrDefault().price.Value,
                Date = (DateTime)cd.Key
            }).ToArray();

            while (dt.StartOfDay().CompareTo(endDate.EndOfDay()) <= 0)
            {
                var productDetails = abcproducts.Where(c => c.Date == dt.Date).FirstOrDefault();
                var lastproductDetails = db.market_products.Where(x => x.market_id == marketid && x.product_id == productid && x.created_at < dt.Date).OrderByDescending(x => x.id).FirstOrDefault();
                decimal plp = lastproductDetails != null ? lastproductDetails.price.Value : 0;
                marketProductChartsData.Add(dt.Date.ToString("MMM-dd"), productDetails != null ? productDetails.Value : plp);

                dt = dt.AddDays(1);
            }

            ViewBag.marketProductDataForChart = marketProductChartsData;
            ViewBag.ProductName = db.products.Find(productid).product_name;
            return PartialView("_ProductPriceChart");
        }


        // GET: markets/Edit/5
        public ActionResult Edit(long? id)
        {
            if (currentPageID("markets") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("markets")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            ViewBag.role = CurrentUser.RoleName;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (CurrentUser.multiplemarketids != null && CurrentUser.multiplemarketids.Contains(id.ToString()))
            {
                market market = db.markets.Find(id);
                if (market == null)
                {
                    return HttpNotFound();
                }
                ViewBag.region_id = new SelectList(db.regions, "id", "name", market.region_id);
                return View(market);
            }
            else if (CurrentUser.multiplemarketids == null)
            {
                market market = db.markets.Find(id);
                if (market == null)
                {
                    return HttpNotFound();
                }
                ViewBag.region_id = new SelectList(db.regions, "id", "name", market.region_id);
                return View(market);
            }
            else
            {
                return RedirectToAction("NotAllow", "Custom");
            }
        }

        // POST: markets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        // modify the market record
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(market market, string open_at, string close_at, HttpPostedFileBase image)
        {
            if (currentPageID("markets") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("markets")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            TimeSpan fromtime = DateTime.Parse(open_at).TimeOfDay;
            TimeSpan toTime = DateTime.Parse(close_at).TimeOfDay;

            Guid imgGuid = Guid.NewGuid();

            if (image != null && image.ContentLength > 0)
            {
                var img = imgGuid.ToString() + Path.GetExtension(image.FileName);
                var path = Path.Combine(Server.MapPath("~/Media/Images/"), img);
                image.SaveAs(path);
                market.market_image = "/Media/Images/" + img;
            }

            market.open_at = fromtime;
            market.close_at = toTime;
            market.updated_at = DateTime.Now;
            db.Entry(market).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");

        }

        // delete from table based on marketid, productid
        [ActionName("DeleteMarketProduct")]
        public ActionResult DeleteMarketProduct(long marketid, long productid)
        {
            if (currentPageID("markets") > 0)
            {
                if (!CurrentUser.canDelete(currentPageID("markets")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            List<market_products> market = db.market_products.Where(x => x.market_id == marketid && x.product_id == productid).ToList();
            db.market_products.RemoveRange(market);
            db.SaveChanges();
            return Json("Success");
        }

        // POST: markets/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            if (currentPageID("markets") > 0)
            {
                if (!CurrentUser.canDelete(currentPageID("markets")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            market market = db.markets.Find(id);
            db.markets.Remove(market);
            db.SaveChanges();
            return Json("Success");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
