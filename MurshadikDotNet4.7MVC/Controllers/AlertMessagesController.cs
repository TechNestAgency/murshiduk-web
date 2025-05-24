using PagedList;
using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace MurshadikCP.Controllers
{
    public class AlertMessagesController : BaseController
    {
        // GET: AlertMessages
        // list of alert message set by admin user
        public ActionResult Index(int? Page_No)
        {
            if (currentPageID("AlertMessages") > 0)
            {
                if (!CurrentUser.canView(currentPageID("AlertMessages")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            int pagesize = 10;
            int No_Of_Page = (Page_No ?? 1);
            return View(db.AlertMessages.OrderByDescending(x => x.Id).ToPagedList(No_Of_Page, pagesize));
        }


        // create view for alert messages
        public ActionResult Create()
        {
            if (currentPageID("AlertMessages") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("AlertMessages")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            ViewBag.attention_id = new SelectList(db.attentions, "id", "name_en");
            ViewBag.alertActionId = new SelectList(db.AlertTypes, "id", "name_ar");
            ViewBag.alertHazardId = new SelectList(db.AlertHazards, "id", "name_ar");
            return View();
        }

        // creating for alert messages
        // multiseason for adding more than one season at a time
        // city also adding more than one city
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AlertMessage am)
        {
            if (currentPageID("AlertMessages") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("AlertMessages")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (ModelState.IsValid)
            {
                am.created_at = DateTime.Now;
                am.created_by = CurrentUser.Id;
                db.AlertMessages.Add(am);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.attention_id = new SelectList(db.attentions, "id", "name_en");
            ViewBag.alertActionId = new SelectList(db.AlertTypes, "id", "name_ar");
            ViewBag.alertHazardId = new SelectList(db.AlertHazards, "id", "name_ar");
           
            return View(am);
        }

        // edit view for alert messages
        // just show the cshtml page to edit the record
        public ActionResult Edit(int? id)
        {
            if (currentPageID("AlertMessages") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("AlertMessages")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AlertMessage msg = db.AlertMessages.Find(id);
            if (msg == null)
            {
                return HttpNotFound();
            }

            ViewBag.alertActionId = new SelectList(db.AlertTypes, "id", "name_ar", msg.alertActionId);
            ViewBag.alertHazardId = new SelectList(db.AlertHazards, "id", "name_ar", msg.alertHazardId);
            msg.season = msg.season != null ? msg.season.Replace(",", "','") : "";
            return View(msg);
        }

        // POST: msgs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        // editing the alert messages
        // multiseason is for using more than seasons
        // city also more than one selecting by the end user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AlertMessage am)
        {
            if (currentPageID("AlertMessages") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("AlertMessages")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (ModelState.IsValid)
            {
                am.created_by = CurrentUser.Id;
                am.created_at = DateTime.Now;
                db.Entry(am).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.alertActionId = new SelectList(db.AlertTypes, "id", "name_ar", am.alertActionId);
            ViewBag.alertHazardId = new SelectList(db.AlertHazards, "id", "name_ar", am.alertHazardId);
            return View(am);
        }

        // POST: msgs/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (currentPageID("AlertMessages") > 0)
            {
                if (!CurrentUser.canDelete(currentPageID("AlertMessages")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            AlertMessage msg = db.AlertMessages.Find(id);
            db.AlertMessages.Remove(msg);
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