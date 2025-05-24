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
    public class msgsController : BaseController
    {
        // GET: msgs
        // Index view shows list of msgs 
        public ActionResult Index(int? Page_No)
        {
            if (currentPageID("msgs") > 0)
            {
                if (!CurrentUser.canView(currentPageID("msgs")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            int pagesize = 10;
            int No_Of_Page = (Page_No ?? 1);
            return View(db.msgs.Include(m => m.attention).Include(m => m.region).Include(m => m.skill).OrderByDescending(x => x.id).ToPagedList(No_Of_Page, pagesize));
        }

        // GET: msgs/Create
        public ActionResult Create()
        {
            if (currentPageID("msgs") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("msgs")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            ViewBag.attention_id = new SelectList(db.attentions, "id", "name_en");
            ViewBag.region_id = new SelectList(db.regions, "id", "name");
            ViewBag.skill_id = new SelectList(db.skills, "id", "name");
            return View();
        }

        // POST: msgs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        // create message
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,attention_id,region_id,skill_id,msg1")] msg msg)
        {
            if (currentPageID("msgs") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("msgs")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (ModelState.IsValid)
            {
                db.msgs.Add(msg);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.attention_id = new SelectList(db.attentions, "id", "name_en", msg.attention_id);
            ViewBag.region_id = new SelectList(db.regions, "id", "name", msg.region_id);
            ViewBag.skill_id = new SelectList(db.skills, "id", "name", msg.skill_id);
            return View(msg);
        }

        // GET: msgs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (currentPageID("msgs") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("msgs")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            msg msg = db.msgs.Find(id);
            if (msg == null)
            {
                return HttpNotFound();
            }
            ViewBag.attention_id = new SelectList(db.attentions, "id", "name_en", msg.attention_id);
            ViewBag.region_id = new SelectList(db.regions, "id", "name", msg.region_id);
            ViewBag.skill_id = new SelectList(db.skills, "id", "name", msg.skill_id);
            return View(msg);
        }

        // POST: msgs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        // edit message
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,attention_id,region_id,skill_id,msg1")] msg msg)
        {
            if (currentPageID("msgs") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("msgs")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (ModelState.IsValid)
            {
                db.Entry(msg).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.attention_id = new SelectList(db.attentions, "id", "name_en", msg.attention_id);
            ViewBag.region_id = new SelectList(db.regions, "id", "name", msg.region_id);
            ViewBag.skill_id = new SelectList(db.skills, "id", "name", msg.skill_id);
            return View(msg);
        }

        // POST: msgs/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (currentPageID("msgs") > 0)
            {
                if (!CurrentUser.canDelete(currentPageID("msgs")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            msg msg = db.msgs.Find(id);
            db.msgs.Remove(msg);
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
