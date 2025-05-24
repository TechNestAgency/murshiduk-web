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
    public class AlertTypeController : BaseController
    {
        //private mlaraEntities db = new mlaraEntities();
        //UserInfo ui = new UserInfo();
        // GET: AlertType
        // Refer to Index page just show the list of the alert types
        public ActionResult Index(int Id = 0)
        {
            if (currentPageID("AlertType") > 0)
            {
                if (!CurrentUser.canView(currentPageID("AlertType")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            ParentAlertType pmt = new ParentAlertType();
            pmt.atList = db.AlertTypes.OrderByDescending(x => x.Id).ToList();
            if (Id > 0)
            {
                pmt.aty = db.AlertTypes.Where(x => x.Id == Id).FirstOrDefault();
            }
            else
            {
                pmt.aty = new AlertType();
            }

            return View(pmt);
        }


        // create alert type
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ParentAlertType msg)
        {
            if (currentPageID("AlertType") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("AlertType")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (ModelState.IsValid)
            {
                msg.aty.created_at = DateTime.Now;
                db.AlertTypes.Add(msg.aty);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(msg);
        }


        // update or modify the alert type
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ParentAlertType msg, int id)
        {
            if (currentPageID("AlertType") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("AlertType")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (ModelState.IsValid)
            {
                msg.aty.Id = id;
                msg.aty.created_at = DateTime.Now;
                db.Entry(msg.aty).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Successfully Updated!";
                //return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        // POST: AlertType/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            if (currentPageID("AlertType") > 0)
            {
                if (!CurrentUser.canDelete(currentPageID("AlertType")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AlertType msg = db.AlertTypes.Find(id);
            db.AlertTypes.Remove(msg);
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

    public class ParentAlertType
    {
        public List<AlertType> atList { get; set; }
        public AlertType aty { get; set; }
    }

}