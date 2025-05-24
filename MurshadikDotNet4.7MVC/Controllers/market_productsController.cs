using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
    public class market_productsController : BaseController
    {
        // GET: market_products
        // Index method shows list of market_products based on marketid and productid
        // pagination also there
        public ActionResult Index(int? Page_No, long MarketID, long ProductID)
        {
            if (currentPageID("market_products") > 0)
            {
                if (!CurrentUser.canView(currentPageID("market_products")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            int pagesize = 10;
            int No_Of_Page = (Page_No ?? 1);
            var market_products = db.market_products.Where(x => x.market_id == MarketID && x.product_id == ProductID).Include(m => m.market).Include(m => m.product).OrderByDescending(m => m.price_date);
            ViewBag.role = CurrentUser.RoleName;
            return View(market_products.ToPagedList(No_Of_Page, pagesize));
        }

        // GET: market_products/Create
        public ActionResult Create(long MarketID, long ProductID)
        {
            if (currentPageID("market_products") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("market_products")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            market_products mp = new market_products();
            mp.product_id = ProductID;
            mp.market_id = MarketID;
            ViewBag.role = CurrentUser.RoleName;
            ViewBag.market_id = new SelectList(db.markets.ToList(), "id", "marketname", MarketID);
            ViewBag.product_id = new SelectList(db.products.ToList(), "id", "product_name", ProductID);
            return View(mp);
        }

        // add market product based on marketid, productid
        [HttpPost]
        public ActionResult CreateMarketProduct(long MarketID, long ProductID, decimal unitPrice = 0, decimal Price = 0, decimal OldPrice = 0)
        {
            if (currentPageID("market_products") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("market_products")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            market_products mp = new market_products();
            mp.product_id = ProductID;
            mp.market_id = MarketID;
            mp.created_at = DateTime.Now;
            mp.price_date = DateTime.Now;
            mp.unit_value = unitPrice != 0 ? unitPrice.ToString() : db.market_products.Where(x => x.market_id == MarketID && x.product_id == ProductID).FirstOrDefault().unit_value;
            if (Price > 0)
                mp.price = Price;
            else
                mp.price = 0;
            db.market_products.Add(mp);
            db.SaveChanges();

            if (Price > 0 && OldPrice > 0)
            {
                CheckProductSubscribers(MarketID, ProductID, Price, OldPrice);
            }

            return Json("Success");
        }

        // POST: market_products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(market_products market_products)
        {
            if (currentPageID("market_products") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("market_products")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (ModelState.IsValid)
            {
                market_products.created_at = DateTime.Now;
                market_products.price_date = DateTime.Now;
                db.market_products.Add(market_products);
                db.SaveChanges();

                if (CurrentUser.marketid == market_products.market_id || CurrentUser.RoleId == (int)Role.Market)
                {
                    return RedirectToAction("View", "markets", new { id = market_products.market_id });
                }

                return RedirectToAction("View", "markets", new { id = market_products.market_id });
            }

            ViewBag.market_id = new SelectList(db.markets, "id", "marketname", market_products.market_id);
            ViewBag.product_id = new SelectList(db.products, "id", "product_name", market_products.product_id);
            return RedirectToAction("Create", new { MarketID = market_products.market_id, ProductID = market_products.product_id });
        }

        // GET: market_products/Edit/5
        public ActionResult Edit(long? id)
        {
            if (currentPageID("market_products") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("market_products")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            ViewBag.role = CurrentUser.RoleName;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            market_products market_products = db.market_products.Find(id);
            if (market_products == null)
            {
                return HttpNotFound();
            }
            ViewBag.market_id = new SelectList(db.markets, "id", "marketname", market_products.market_id);
            ViewBag.product_id = new SelectList(db.products, "id", "product_name", market_products.product_id);
            return View(market_products);
        }

        // POST: market_products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        // edit the record here for market_product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(market_products market_products)
        {
            if (currentPageID("market_products") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("market_products")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (ModelState.IsValid)
            {
                market_products.updated_at = DateTime.Now;
                db.Entry(market_products).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { MarketID = market_products.market_id, ProductID = market_products.product_id });
            }
            ViewBag.market_id = new SelectList(db.markets, "id", "marketname", market_products.market_id);
            ViewBag.product_id = new SelectList(db.products, "id", "product_name", market_products.product_id);
            return View(market_products);
        }

        // this method check the users if he or she subscribe the product of that market then we send notification
        public void CheckProductSubscribers(long market_id, long product_id, decimal currentPrice, decimal oldPrice)
        {
            List<product_subscribers> ps = db.product_subscribers.Where(x => x.product_id == product_id && x.market_id == market_id).ToList();
            if (ps != null)
            {
                int increase = 0;
                if (currentPrice > oldPrice)
                {
                    increase = 1;
                }
                else if (oldPrice > currentPrice)
                {
                    increase = 0;
                }

                List<product_subscribers> ps_increase = db.product_subscribers.Where(x => x.market_id == market_id && x.product_id == product_id && x.on_increase == increase).ToList();
                List<product_subscribers> ps_final = ps.Where(x => currentPrice >= x.on_increase && x.on_increase != 1 && x.on_increase != 0).ToList();
                ps_final.AddRange(ps_increase);
                string[] phoneNos = new string[ps_final.Count()];
                for (int i = 0; i < ps_final.Count(); i++)
                {
                    phoneNos[i] = ps_final[i].user.phone + "-market";
                }

                Generic g = new Generic();
                if (phoneNos.Count() > 0)
                {
                    //g.Message("المنتج", "سعر " + ps[0].product.product_name + " هو " + currentPrice.ToString(), phoneNos, "", phoneNos)
                    //g.SendGeneralNotification("تم تغيير سعر المنتج", "تم تغيير سعر المنتج " + ps[0].product.product_name + " : " + currentPrice.ToString() + "ريال","marekt price",phoneNos,market_id)
                    g.sendNotificationOnChats("المنتج", "تم تغيير سعر المنتج " + ps[0].product.product_name + " : " + currentPrice.ToString() + "ريال", phoneNos, "market_price_change", market_id.ToString());
                }

                foreach (var item in ps_final)
                {
                    g.InsertNotificationData(item.user_id.Value, "تم تغيير سعر المنتج " + item.product.product_name + " : " + currentPrice.ToString() + "ريال", (Int16)AppConstants.Type.Market, market_id, "");
                }
            }
        }

        

        // POST: market_products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            if (currentPageID("market_products") > 0)
            {
                if (!CurrentUser.canDelete(currentPageID("market_products")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            market_products market_products = db.market_products.Find(id);
            db.market_products.Remove(market_products);
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
