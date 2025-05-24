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
using MurshadikCP.Repositories;

namespace MurshadikCP.Controllers
{
    [Authorize]
    public class citiesController : BaseController
    {
        // GET: cities
        // Index View showing the cities
        // pagination and search criteria
        // cities shows based on region_id
        CityRepository cr = new CityRepository();
        public ActionResult Index(int? Page_No, string Search_Data, string Filter_Value, int region_id)
        {
            if (currentPageID("cities") > 0)
            {
                if (!CurrentUser.canView(currentPageID("cities")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            int pagesize = Constants.defaultPageCount;
          
            ViewBag.Filter_Value = Search_Data;
          
            ViewBag.region_id = new SelectList(db.regions.ToList(), "id", "name_ar");
            
            return View(cr.GetCitiesBySearch(Page_No, Search_Data, Filter_Value, region_id).ToPagedList(Page_No ?? 1, pagesize));
        }

        // GET: cities/Create
        public ActionResult Create()
        {
            if (currentPageID("cities") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("cities")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            ViewBag.region_id = new SelectList(db.regions.ToList(), "id", "name_ar");
            return View();
        }

        // POST: cities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(city city, string city_id)
        {
            if (currentPageID("cities") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("cities")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (ModelState.IsValid)
            {

                if (city_id == "")
                {
                    cr.AddCity(city);
                }
                else
                {
                    city.id = Convert.ToInt16(city_id);
                    cr.UpdateCity(city);
                }
                cr.Save();
                int page = 0;
                if (Request.UrlReferrer.Query.Contains("Page_No"))
                {
                    page = Convert.ToInt16(HttpUtility.ParseQueryString(Request.UrlReferrer.Query)["Page_No"]);
                }
                else
                {
                    page = 1;
                }
                
                return RedirectToAction("Index", new { region_id = city.region_id, Page_No = page });
            }

            return RedirectToAction("Index", new { region_id = city.region_id });
        }

        // GET: cities/Edit/5
        public ActionResult Edit(int? id)
        {
            if (currentPageID("cities") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("cities")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //city city = db.cities.Find(id);
            city city = cr.GetById(id ?? 0);
            if (city == null)
            {
                return HttpNotFound();
            }
            ViewBag.region_id = new SelectList(db.regions.ToList(), "id", "name_ar", city.region_id);
            return View(city);
        }

        // POST: cities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(city city)
        {
            if (currentPageID("cities") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("cities")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (ModelState.IsValid)
            {
                cr.Update(city);
                cr.Save();
                return RedirectToAction("Index", new { region_id = city.region_id });
            }
            return View(city);
        }

        // POST: cities/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (currentPageID("cities") > 0)
            {
                if (!CurrentUser.canDelete(currentPageID("cities")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            cr.Delete(id);
            cr.Save();
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
