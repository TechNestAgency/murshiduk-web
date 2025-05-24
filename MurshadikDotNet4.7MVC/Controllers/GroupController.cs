using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace MurshadikCP.Controllers
{
    public class GroupController : BaseController
    {
        // GET: Group
        // Index view shows groups
        public ActionResult Index()
        {
            if (currentPageID("Group") > 0)
            {
                if (!CurrentUser.canView(currentPageID("Group")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            var groups = db.groups.Include(g => g.region);
            return View(groups.ToList());
        }

        // GET: Group/Details/5
        public ActionResult Details(long? id)
        {
            if (currentPageID("Group") > 0)
            {
                if (!CurrentUser.canView(currentPageID("Group")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            group group = db.groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        // GET: Group/Create
        public ActionResult Create()
        {
            if (currentPageID("Group") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("Group")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            ViewBag.region_id = new SelectList(db.regions, "id", "name");
            return View();
        }

        // POST: Group/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,region_id,name,name_ar,active,created_at,updated_at")] group group)
        {
            if (currentPageID("Group") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("Group")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (ModelState.IsValid)
            {
                db.groups.Add(group);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.region_id = new SelectList(db.regions, "id", "name", group.region_id);
            return View(group);
        }

        // GET: Group/Edit/5
        public ActionResult Edit(long? id)
        {
            if (currentPageID("Group") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("Group")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            group group = db.groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            ViewBag.region_id = new SelectList(db.regions, "id", "name", group.region_id);
            return View(group);
        }

        // POST: Group/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,region_id,name,name_ar,active,created_at,updated_at")] group group)
        {
            if (currentPageID("Group") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("Group")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (ModelState.IsValid)
            {
                db.Entry(group).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.region_id = new SelectList(db.regions, "id", "name", group.region_id);
            return View(group);
        }

        // GET: Group/Delete/5
        public ActionResult Delete(long? id)
        {
            if (currentPageID("Group") > 0)
            {
                if (!CurrentUser.canDelete(currentPageID("Group")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            group group = db.groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        // POST: Group/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            if (currentPageID("Group") > 0)
            {
                if (!CurrentUser.canDelete(currentPageID("Group")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            group group = db.groups.Find(id);
            db.groups.Remove(group);
            db.SaveChanges();
            return RedirectToAction("Index");
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