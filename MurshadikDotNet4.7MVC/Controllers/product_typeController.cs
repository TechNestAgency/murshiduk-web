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
    public class product_typeController : BaseController
    {
        // GET: product_type
        // shows product type on index view page
        public ActionResult Index(int? Page_No)
        {
            if (!CurrentUser.canView(currentPageID("product_type")))
            {
                return RedirectToAction("NotAllow", "Custom");
            }

            int pagesize = 10;
            int No_Of_Page = (Page_No ?? 1);
            return View(db.product_type.OrderByDescending(x => x.id).ToPagedList(No_Of_Page, pagesize));

        }

        // GET: product_type/Create
        public ActionResult Create()
        {
            if (!CurrentUser.canInsert(currentPageID("product_type")))
            {
                return RedirectToAction("NotAllow", "Custom");
            }

            return View();
        }

        // POST: product_type/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(product_type product_type)
        {
            if (!CurrentUser.canInsert(currentPageID("product_type")))
            {
                return RedirectToAction("NotAllow", "Custom");
            }

            if (CurrentUser.RoleId == (long)Role.MarketAdmin || CurrentUser.RoleId == (long)Role.SuperAdmin)
            {
                if (ModelState.IsValid)
                {
                    product_type.created_at = DateTime.Now;
                    product_type.is_active = true;
                    db.product_type.Add(product_type);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("NotAllow", "Custom");
            }

            return View(product_type);
        }

        // GET: product_type/Edit/5
        public ActionResult Edit(long? id)
        {
            if (!CurrentUser.canUpdate(currentPageID("product_type")))
            {
                return RedirectToAction("NotAllow", "Custom");
            }

            if (CurrentUser.RoleId == (long)Role.MarketAdmin || CurrentUser.RoleId == (long)Role.SuperAdmin)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                product_type product_type = db.product_type.Find(id);
                if (product_type == null)
                {
                    return HttpNotFound();
                }
                return View(product_type);
            }
            else
            {
                return RedirectToAction("NotAllow", "Custom");
            }

        }

        // POST: product_type/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(product_type product_type)
        {
            if (!CurrentUser.canUpdate(currentPageID("product_type")))
            {
                return RedirectToAction("NotAllow", "Custom");
            }

            if (CurrentUser.RoleId == (long)Role.MarketAdmin || CurrentUser.RoleId == (long)Role.SuperAdmin)
            {
                if (ModelState.IsValid)
                {
                    product_type.updated_at = DateTime.Now;
                    db.Entry(product_type).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(product_type);
            }
            else
            {
                return RedirectToAction("NotAllow", "Custom");
            }
        }

        // POST: product_type/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            if (!CurrentUser.canDelete(currentPageID("product_type")))
            {
                return RedirectToAction("NotAllow", "Custom");
            }

            if (CurrentUser.RoleId == (long)Role.MarketAdmin || CurrentUser.RoleId == (long)Role.SuperAdmin)
            {
                if (id == 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                product_type product_type = db.product_type.Find(id);
                db.product_type.Remove(product_type);
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
