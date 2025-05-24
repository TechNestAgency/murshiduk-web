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
    public class attentionsController : BaseController
    {
        // GET: attentions
        // Index View for showing the list of attentions.
        public ActionResult Index(int? Page_No)
        {
            if (currentPageID("attentions") > 0)
            {
                if (!CurrentUser.canView(currentPageID("attentions")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            int pagesize = 10;
            int No_Of_Page = (Page_No ?? 1);
            return View(db.attentions.OrderByDescending(x => x.id).ToPagedList(No_Of_Page, pagesize));
        }

        // GET: attentions/Create
        public ActionResult Create()
        {
            if (currentPageID("attentions") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("attentions")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            return View();
        }

        // POST: attentions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        // creating the attentions here
        // below are the fields which are pass from the view
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name_en,name_ar")] attention attention)
        {
            if (currentPageID("attentions") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("attentions")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (ModelState.IsValid)
            {
                db.attentions.Add(attention);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(attention);
        }

        // GET: attentions/Edit/5
        // edit attentions View for modify the record
        public ActionResult Edit(int? id)
        {
            if (currentPageID("attentions") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("attentions")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            var a = db.attentions.Where(x => x.id == id).FirstOrDefault();
            return View(a);
        }

        // POST: attentions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name_en,name_ar")] attention attention)
        {
            if (currentPageID("attentions") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("attentions")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (ModelState.IsValid)
            {
                db.Entry(attention).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(attention);
        }

        // POST: attentions/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (currentPageID("attentions") > 0)
            {
                if (!CurrentUser.canDelete(currentPageID("attentions")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            attention attention = db.attentions.Find(id);
            db.attentions.Remove(attention);
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
