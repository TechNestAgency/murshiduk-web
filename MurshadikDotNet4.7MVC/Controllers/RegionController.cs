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
    public class RegionController : BaseController
    {
        // GET: Region
        RegionRepository rr = new RegionRepository();
        public ActionResult Index(int? Page_No, string Search_Data, string Filter_Value)
        {
            if (currentPageID("Region") > 0)
            {
                if (!CurrentUser.canView(currentPageID("Region")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            int pagesize = 10;

            ViewBag.Filter_Value = Search_Data;

            return View(rr.GetAllRegions(Page_No, Search_Data, Filter_Value).ToPagedList(Page_No ?? 1, pagesize));
        }

        // POST: Region/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(region region, string region_id, string active)
        {
            if (currentPageID("Region") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("Region")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            region.created_at = DateTime.Now;
            region.updated_at = DateTime.Now;
            if (active == "on")
            {
                region.active = true;
            }
            
            if (region_id != "")
            {
                region.id = Convert.ToInt32(region_id);
                rr.UpdateRegion(region);
            }
            else
            {
                rr.CreateRegion(region);
            }

            return RedirectToAction("Index");
        }

        // POST: Region/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(region region)
        {
            if (currentPageID("Region") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("Region")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (ModelState.IsValid)
            {
                region.created_at = DateTime.Now;
                region.updated_at = DateTime.Now;
                rr.UpdateRegion(region);
                rr.Save();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        

        // POST: Region/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            if (currentPageID("Region") > 0)
            {
                if (!CurrentUser.canDelete(currentPageID("Region")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            region region = rr.GetById(Convert.ToInt16(id));
            rr.DeleteRegion(region);
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
