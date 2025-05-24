using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MurshadikCP.Controllers
{
    public class App_StatsController : BaseController
    {
        // GET: App_Stats
        // Index method refer to list of the app_Stats
        // this view is using for the mobile application
        // just show the statistics or count of the particular section
        public ActionResult Index(int id = 0)
        {
            if (currentPageID("App_Stats") > 0)
            {
                if (!CurrentUser.canView(currentPageID("App_Stats")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            Parent_App_Stats ps = new Parent_App_Stats();
            if (id > 0)
            {
                ps.List = db.app_stats.OrderBy(x => x.enabled == false).ToList();
                ps.stats = db.app_stats.Where(x => x.id == id).FirstOrDefault();
            }
            else
            {
                ps.List = db.app_stats.OrderBy(x => x.enabled == false).ToList();
                ps.stats = new app_stats();
            }
            
            return View(ps);
        }

        // image, background color, text color define here
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Parent_App_Stats ps, HttpPostedFileBase imgIcon, string bg_color, string text_color)
        {
            if (currentPageID("App_Stats") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("App_Stats")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (ModelState.IsValid)
            {
                Guid iconGuid = Guid.NewGuid();
                if (imgIcon.ContentLength > 0)
                {
                    var img = iconGuid.ToString() + Path.GetExtension(imgIcon.FileName);
                    var path = Path.Combine(Server.MapPath("~/Media/Images/"), img);
                    imgIcon.SaveAs(path);
                    ps.stats.icon = "/Media/Images/" + iconGuid.ToString() + Path.GetExtension(imgIcon.FileName);
                }
                ps.stats.bg_color = bg_color;
                ps.stats.text_color = text_color;
                db.app_stats.Add(ps.stats);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // edit for record
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Parent_App_Stats ps, int sid, HttpPostedFileBase imgIcon, string bg_color, string text_color)
        {
            if (currentPageID("App_Stats") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("App_Stats")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (ModelState.IsValid)
            {
                app_stats mainstats = db.app_stats.Find(sid);

                Guid iconGuid = Guid.NewGuid();
                
                if (imgIcon != null && imgIcon.ContentLength > 0)
                {
                    var img = iconGuid.ToString() + Path.GetExtension(imgIcon.FileName);

                    var path = Path.Combine(Server.MapPath("~/Media/Images/"), img);
                    imgIcon.SaveAs(path);
                    mainstats.icon = "/Media/Images/" + iconGuid.ToString() + Path.GetExtension(imgIcon.FileName);
                }

                mainstats.text_color = text_color;
                mainstats.bg_color = bg_color;
                mainstats.id = sid;
                mainstats.title = ps.stats.title;
                mainstats.enabled = ps.stats.enabled;
                mainstats.stats = ps.stats.stats;
                db.Entry(mainstats).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Successfully Updated!";
                
            }
            return RedirectToAction("Index");
        }
    }

    public class Parent_App_Stats
    {
        public List<app_stats> List { get; set; }
        public app_stats stats { get; set; }
    }
}