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
    public class municipalitiesController : BaseController
    {
        // GET: municipalities
        // Index View show the municipalities data in list form
        // pagination and search crieteria is also there
        public ActionResult Index(int? Page_No, int city_id, string Search_Data, string Filter_Value)
        {
            if (currentPageID("municipalities") > 0)
            {
                if (!CurrentUser.canView(currentPageID("municipalities")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            int pagesize = 10;
            int No_Of_Page = (Page_No ?? 1);

            if (Search_Data != null)
            {
                Page_No = 1;
            }
            else
            {
                Search_Data = Filter_Value;
            }

            ViewBag.Filter_Value = Search_Data;
            var municipalities = from c in db.municipalities
                                 where c.city_id == city_id
                                 select c;
            if (!String.IsNullOrEmpty(Search_Data))
            {
                municipalities = municipalities.Where(s => s.name_ar.Contains(Search_Data)
                                       || s.name.Contains(Search_Data));
            }
            ViewBag.city_id = new SelectList(db.cities, "id", "name_en");
            return View(municipalities.OrderByDescending(x => x.id).ToPagedList(No_Of_Page, pagesize));
        }

        // GET: municipalities/Create
        public ActionResult Create()
        {
            if (currentPageID("municipalities") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("municipalities")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            ViewBag.city_id = new SelectList(db.cities, "id", "name_en");
            return View();
        }

        // POST: municipalities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        // Create municipalities
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(municipality municipality, string municipality_id)
        {
            if (currentPageID("municipalities") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("municipalities")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (ModelState.IsValid)
            {
                if (municipality_id == "")
                {
                    db.municipalities.Add(municipality);
                }
                else
                {
                    municipality.id = Convert.ToInt32(municipality_id);
                    db.Entry(municipality).State = EntityState.Modified;
                }
                db.SaveChanges();
                return RedirectToAction("Index", new { city_id  = municipality.city_id });
            }

            ViewBag.city_id = new SelectList(db.cities, "id", "name_en", municipality.city_id);
            return View(municipality);
        }

        // GET: municipalities/Edit/5
        public ActionResult Edit(int? id)
        {
            if (currentPageID("municipalities") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("municipalities")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            municipality municipality = db.municipalities.Find(id);
            if (municipality == null)
            {
                return HttpNotFound();
            }
            ViewBag.city_id = new SelectList(db.cities, "id", "name_en", municipality.city_id);
            return View(municipality);
        }

        // POST: municipalities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        // modify municipalities
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(municipality municipality)
        {
            if (currentPageID("municipalities") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("municipalities")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (ModelState.IsValid)
            {
                db.Entry(municipality).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { city_id = municipality.city_id });
            }
            ViewBag.city_id = new SelectList(db.cities, "id", "name_en", municipality.city_id);
            return View(municipality);
        }

        // POST: municipalities/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (currentPageID("municipalities") > 0)
            {
                if (!CurrentUser.canDelete(currentPageID("municipalities")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            municipality municipality = db.municipalities.Find(id);
            db.municipalities.Remove(municipality);
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
