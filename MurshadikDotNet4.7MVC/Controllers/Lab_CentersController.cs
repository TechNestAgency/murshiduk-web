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
    public class Lab_CentersController : BaseController
    {
        // GET: Lab_Centers
        // Index view show lab centers name
        public ActionResult Index(int lab_id)
        {
            if (currentPageID("Lab_Centers") > 0)
            {
                if (!CurrentUser.canView(currentPageID("Lab_Centers")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            return View(db.LabCollecctionPoints.Where(x => x.lab_id == lab_id).ToList());
        }

        // Add view show the form to add lab center there
        public ActionResult Add(int lab_id)
        {
            if (currentPageID("Lab_Centers") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("Lab_Centers")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            ViewBag.lab_id = new SelectList(db.labs.Where(x => x.id == lab_id).ToList(), "id", "Name");
            ViewBag.labid = lab_id;
            return View();
        }

        // here you can add lab center and also add working days.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(LabCollecctionPoint l, string open_at, string close_at, string[] working_days)
        {
            if (currentPageID("Lab_Centers") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("Lab_Centers")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            LabCollecctionPoint lab = new LabCollecctionPoint();
            if (l.id > 0)
            {
                lab = db.LabCollecctionPoints.Where(x => x.id == l.id).FirstOrDefault();
            }
            lab.working_days = string.Join(",", working_days);
            TimeSpan fromtime = DateTime.Parse(open_at).TimeOfDay;
            TimeSpan toTime = DateTime.Parse(close_at).TimeOfDay;
            lab.open_at = fromtime;
            lab.close_at = toTime;
            lab.latlang = "15,34";
            lab.Address = l.Address;//"Abdullah Road, Malaz, Riyadh!";
            lab.active = l.active;
            lab.Name = l.Name;
            lab.Number = l.Number;
            lab.email = l.email;
            lab.phone = l.phone;
            lab.contactperson = l.contactperson;
            lab.lab_id = l.lab_id;

            if (l.id > 0)
            {
                db.Entry(lab).State = EntityState.Modified;
            }
            else
            {
                db.LabCollecctionPoints.Add(lab);
            }
            db.SaveChanges();

            return RedirectToAction("Index", new { lab_id = l.lab_id });
        }

        // edit form, just edit the record of the lab center
        public ActionResult Edit(int id)
        {
            if (currentPageID("Lab_Centers") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("Lab_Centers")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            ViewBag.role = CurrentUser.RoleName;
            LabCollecctionPoint l = db.LabCollecctionPoints.Where(x => x.id == id).FirstOrDefault();
            if (l != null)
            {
                string day = "'" + l.working_days.Replace(",", "','") + "'";
                ViewBag.day = day;
                ViewBag.lab_id = new SelectList(db.labs.Where(x => x.id == l.lab_id).ToList(), "id", "Name");
                ViewBag.labid = l.lab_id;
            }
            return View(l);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            if (currentPageID("Lab_Centers") > 0)
            {
                if (!CurrentUser.canDelete(currentPageID("Lab_Centers")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            LabCollecctionPoint lab = db.LabCollecctionPoints.Find(id);

            db.LabCollecctionPoints.Remove(lab);
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