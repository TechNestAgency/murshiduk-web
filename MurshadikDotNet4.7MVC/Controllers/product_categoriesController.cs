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
    public class product_categoriesController : BaseController
    {
        // GET: product_categories
        // list of product categories shows on index view page
        public ActionResult Index(int? Page_No)
        {
            if (currentPageID("product_categories") > 0)
            {
                if (!CurrentUser.canView(currentPageID("product_categories")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            int pagesize = 10;
            int No_Of_Page = (Page_No ?? 1);
            var product_categories = db.product_categories.Include(p => p.user);
            return View(db.product_categories.OrderByDescending(x => x.id).ToPagedList(No_Of_Page, pagesize));

        }


        // GET: product_categories/Create
        public ActionResult Create()
        {
            if (currentPageID("product_categories") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("product_categories")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (CurrentUser.RoleId == (long)Role.MarketAdmin || CurrentUser.RoleId == (long)Role.SuperAdmin)
            {
                return View();
            }
            else
            {
                return RedirectToAction("NotAllow", "Custom");
            }

        }

        // POST: product_categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(product_categories product_categories)
        {
            if (currentPageID("product_categories") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("product_categories")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (CurrentUser.RoleId == (long)Role.MarketAdmin || CurrentUser.RoleId == (long)Role.SuperAdmin)
            {
                if (ModelState.IsValid)
                {
                    product_categories.created_by = CurrentUser.Id;
                    product_categories.created_at = DateTime.Now;
                    db.product_categories.Add(product_categories);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.created_by = new SelectList(db.users, "id", "name", product_categories.created_by);
                return View(product_categories);
            }
            else
            {
                return RedirectToAction("NotAllow", "Custom");
            }
        }

        // GET: product_categories/Edit/5
        public ActionResult Edit(long? id)
        {
            if (currentPageID("product_categories") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("product_categories")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            product_categories product_categories = db.product_categories.Find(id);
            if (product_categories == null)
            {
                return HttpNotFound();
            }
            return View(product_categories);

        }

        // POST: product_categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(product_categories product_categories)
        {
            if (currentPageID("product_categories") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("product_categories")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (CurrentUser.RoleId == (long)Role.MarketAdmin || CurrentUser.RoleId == (long)Role.SuperAdmin)
            {
                if (ModelState.IsValid)
                {
                    product_categories.updated_at = DateTime.Now;
                    db.Entry(product_categories).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.created_by = new SelectList(db.users, "id", "name", product_categories.created_by);
                return View(product_categories);
            }
            else
            {
                return RedirectToAction("NotAllow", "Custom");
            }
        }

        // POST: product_categories/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            if (currentPageID("product_categories") > 0)
            {
                if (!CurrentUser.canDelete(currentPageID("product_categories")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (CurrentUser.RoleId == (long)Role.MarketAdmin || CurrentUser.RoleId == (long)Role.SuperAdmin)
            {
                if (id == 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                product_categories product_categories = db.product_categories.Find(id);
                db.product_categories.Remove(product_categories);
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
