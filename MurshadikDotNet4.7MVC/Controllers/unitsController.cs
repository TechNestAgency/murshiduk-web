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
    public class unitsController : BaseController
    {
        // GET: units
        public ActionResult Index(int? Page_No)
        {
            if (currentPageID("units") > 0)
            {
                if (!CurrentUser.canView(currentPageID("units")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            int pagesize = 10;
            int No_Of_Page = (Page_No ?? 1);
            return View(db.units.OrderByDescending(x => x.id).ToPagedList(No_Of_Page, pagesize));
        }

        // GET: units/Create
        public ActionResult Create()
        {
            if (currentPageID("units") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("units")))
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

        // POST: units/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(unit unit)
        {
            if (currentPageID("units") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("units")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (CurrentUser.RoleId == (long)Role.MarketAdmin || CurrentUser.RoleId == (long)Role.SuperAdmin)
            {
                if (ModelState.IsValid)
                {
                    db.units.Add(unit);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(unit);
            }
            else
            {
                return RedirectToAction("NotAllow", "Custom");
            }
        }

        // GET: units/Edit/5
        public ActionResult Edit(int? id)
        {
            if (currentPageID("units") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("units")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (CurrentUser.RoleId == (long)Role.MarketAdmin || CurrentUser.RoleId == (long)Role.SuperAdmin)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                unit unit = db.units.Find(id);
                if (unit == null)
                {
                    return HttpNotFound();
                }
                return View(unit);
            }
            else
            {
                return RedirectToAction("NotAllow", "Custom");
            }
        }

        // POST: units/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(unit unit)
        {
            if (currentPageID("units") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("units")))
                    return RedirectToAction("NotAllow", "Custom");
            }
            
            if (CurrentUser.RoleId == (long)Role.MarketAdmin || CurrentUser.RoleId == (long)Role.SuperAdmin)
            {
                if (ModelState.IsValid)
                {
                    db.Entry(unit).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(unit);
            }
            else
            {
                return RedirectToAction("NotAllow", "Custom");
            }
        }

        // POST: units/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (currentPageID("units") > 0)
            {
                if (!CurrentUser.canDelete(currentPageID("units")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (CurrentUser.RoleId == (long)Role.MarketAdmin || CurrentUser.RoleId == (long)Role.SuperAdmin)
            {
                if (id == 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                unit unit = db.units.Find(id);
                db.units.Remove(unit);
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
