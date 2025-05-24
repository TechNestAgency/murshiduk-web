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
    public class productsController : BaseController
    {
        //private mlaraEntities db = new mlaraEntities();
        UserInfo ui = new UserInfo();

        // GET: products
        // index view shows all products
        // pagination also there
        public ActionResult Index(int? Page_No)
        {
            if (currentPageID("products") > 0)
            {
                if (!CurrentUser.canView(currentPageID("products")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            int pagesize = 30;
            int No_Of_Page = (Page_No ?? 1);
            if (CurrentUser.RoleId == (int)Role.Market)
            {
                return RedirectToAction("View", "markets");
            }

            var products = db.products.Include(p => p.product_categories).Include(p => p.product_type).Include(p => p.unit).Include(p => p.user).OrderByDescending(p => p.created_at);
            return View(products.OrderByDescending(x => x.id).ToPagedList(No_Of_Page, pagesize));
        }


        // GET: products/Create
        public ActionResult Create(int marketid = 0)
        {
            if (currentPageID("products") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("products")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            ViewBag.product_category_id = new SelectList(db.product_categories, "id", "name");
            ViewBag.product_type_id = new SelectList(db.product_type, "id", "name");
            ViewBag.unit_id = new SelectList(db.units, "id", "name");
            ViewBag.role = ui.RoleName;
            if (marketid > 0)
                ViewBag.marketid = marketid;
            return View();
        }

        // POST: products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(product product, HttpPostedFileBase product_image, int marketid = 0)
        {
            if (currentPageID("products") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("products")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (ModelState.IsValid)
            {
                Guid imgGuid = Guid.NewGuid();
                if (product_image != null && product_image.ContentLength > 0)
                {
                    var img = imgGuid.ToString() + Path.GetExtension(product_image.FileName);
                    var path = Path.Combine(Server.MapPath("~/Media/Images/Products/"), img);
                    product_image.SaveAs(path);
                    product.product_image = img;
                }

                product.slug = imgGuid.ToString();
                product.created_at = DateTime.Now;
                product.created_by = CurrentUser.Id;
                product.is_active = true;
                db.products.Add(product);
                db.SaveChanges();

                if (marketid > 0)
                {
                    return RedirectToAction("View", "Markets", new { id = marketid });
                }

                return RedirectToAction("Index");
            }

            ViewBag.product_category_id = new SelectList(db.product_categories, "id", "name", product.product_category_id);
            ViewBag.product_type_id = new SelectList(db.product_type, "id", "name", product.product_type_id);
            ViewBag.unit_id = new SelectList(db.units, "id", "name", product.unit_id);
            return View(product);
        }

        // GET: products/Details/5
        public ActionResult Details(long? id)
        {
            if (currentPageID("products") > 0)
            {
                if (!CurrentUser.canView(currentPageID("products")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            product product = db.products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        // GET: products/Edit/5
        public ActionResult Edit(long? id)
        {
            if (currentPageID("products") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("products")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (CurrentUser.RoleId == (long)Role.MarketAdmin || CurrentUser.RoleId == (long)Role.SuperAdmin)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                product product = db.products.Find(id);
                if (product == null)
                {
                    return HttpNotFound();
                }
                ViewBag.product_category_id = new SelectList(db.product_categories, "id", "name", product.product_category_id);
                ViewBag.product_type_id = new SelectList(db.product_type, "id", "name", product.product_type_id);
                ViewBag.unit_id = new SelectList(db.units, "id", "name", product.unit_id);
                ViewBag.role = CurrentUser.RoleName;
                return View(product);
            }
            else
            {
                return RedirectToAction("NotAllow", "Custom");
            }
        }

        // POST: products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(product product, HttpPostedFileBase product_image)
        {
            if (currentPageID("products") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("products")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (CurrentUser.RoleId == (long)Role.MarketAdmin || CurrentUser.RoleId == (long)Role.SuperAdmin)
            {
                if (ModelState.IsValid)
                {
                    Guid imgGuid = Guid.NewGuid();
                    if (product_image != null && product_image.ContentLength > 0)
                    {
                        var img = imgGuid.ToString() + Path.GetExtension(product_image.FileName);
                        var path = Path.Combine(Server.MapPath("~/Media/Images/Products/"), img);
                        product_image.SaveAs(path);
                        product.product_image = img;
                    }
                    product.updated_at = DateTime.Now;
                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.product_category_id = new SelectList(db.product_categories, "id", "name", product.product_category_id);
                ViewBag.product_type_id = new SelectList(db.product_type, "id", "name", product.product_type_id);
                ViewBag.unit_id = new SelectList(db.units, "id", "name", product.unit_id);
                return View(product);
            }
            else
            {
                return RedirectToAction("NotAllow", "Custom");
            }
            
        }

        // POST: products/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            if (currentPageID("products") > 0)
            {
                if (!CurrentUser.canDelete(currentPageID("products")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (CurrentUser.RoleId == (long)Role.MarketAdmin || CurrentUser.RoleId == (long)Role.SuperAdmin)
            {
                if (id == 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                product product = db.products.Find(id);
                db.products.Remove(product);
                db.SaveChanges();
                return Json("Success");
            }
            else
            {
                return RedirectToAction("NotAllow", "Custom");
            }
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
