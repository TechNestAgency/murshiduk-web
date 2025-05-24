using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MurshadikCP.Controllers
{
    public class AlertHazardController : BaseController
    {
        //private mlaraEntities db = new mlaraEntities();
        //UserInfo ui = new UserInfo();
        // refer to Index view
        //  main purpose for sending the weather notification
        // setting here for the message and then automatically send to the user
        // GET: AlertHazard
        public ActionResult Index(int Id = 0)
        {
            if (currentPageID("AlertHazard") > 0)
            {
                if (!CurrentUser.canView(currentPageID("AlertHazard")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            ParentAlertHazard pmt = new ParentAlertHazard();
            pmt.azList = db.AlertHazards.OrderByDescending(x => x.Id).ToList();
            if (Id > 0)
            {
                pmt.az = db.AlertHazards.Where(x => x.Id == Id).FirstOrDefault();
            }
            else
            {
                pmt.az = new AlertHazard();
            }

            return View(pmt);
        }


        // creating the alert hazard message
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ParentAlertHazard msg)
        {
            if (currentPageID("AlertHazard") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("AlertHazard")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (ModelState.IsValid)
            {
                msg.az.created_at = DateTime.Now;
                db.AlertHazards.Add(msg.az);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(msg);
        }

        // edit the alert hazard message
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ParentAlertHazard msg, int id)
        {
            if (currentPageID("AlertHazard") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("AlertHazard")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (ModelState.IsValid)
            {
                msg.az.Id = id;
                msg.az.created_at = DateTime.Now;
                db.Entry(msg.az).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Successfully Updated!";
            }
            return RedirectToAction("Index");
        }

        // POST: Skill/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            if (currentPageID("AlertHazard") > 0)
            {
                if (!CurrentUser.canDelete(currentPageID("AlertHazard")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AlertHazard msg = db.AlertHazards.Find(id);
            db.AlertHazards.Remove(msg);
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

    public class ParentAlertHazard
    {
        public List<AlertHazard> azList { get; set; }
        public AlertHazard az { get; set; }
    }

}