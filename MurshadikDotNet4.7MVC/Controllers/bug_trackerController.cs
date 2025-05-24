using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MurshadikCP.Controllers
{
    public class bug_trackerController : BaseController
    {
        // GET: bug_tracker
        public ActionResult Index(int? Page_No, string Search_Data, string Filter_Value, string Status = null, string Page = null, string start_date = null, string end_date = null)
        {
            if (currentPageID("bug_tracker") > 0)
            {
                if (!CurrentUser.canView(currentPageID("bug_tracker")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            int pagesize = 10;
            int No_Of_Page = (Page_No ?? 1);

            if (Search_Data != null)
            {
                Page_No = 1;
            }
            else
            {
                Search_Data = Filter_Value;
            }

            ViewBag.Filter_status = Status == null ? "0" : Status;
            ViewBag.Filter_page = Page == null ? "0" : Page;
            ViewBag.Filter_Value = Search_Data;
            IQueryable<bug_tracker> bug = from bt in db.bug_tracker select bt;

            TimeSpan daysToSub = new System.TimeSpan(6, 0, 0, 0);
            var startDate = DateTime.Today.Subtract(daysToSub).StartOfDay();
            var endDate = DateTime.Today.EndOfDay();

            if (Status != null && Status != "0")
            {
                bug = bug.Where(x => x.status == Status);
            }

            if (Page != null && Page != "0")
            {
                bug = bug.Where(x => x.page.Contains(Page));
            }

            if (!string.IsNullOrEmpty(start_date))
            {
                ViewBag.Start_Date = start_date;
                var sdt = DateTime.Parse(start_date, null, System.Globalization.DateTimeStyles.RoundtripKind);
                bug = bug.Where(art => art.created_at >= sdt);
            }
            else
            {
                bug = bug.Where(art => art.created_at >= startDate);
            }

            if (!string.IsNullOrEmpty(end_date))
            {
                ViewBag.End_Date = end_date;
                var edt = DateTime.Parse(end_date, null, System.Globalization.DateTimeStyles.RoundtripKind);
                bug = bug.Where(art => art.created_at <= edt);
            }
            else
            {
                bug = bug.Where(art => art.created_at <= endDate);
            }

            StatsCardModel total = new StatsCardModel(Resources.Resources.Total, bug.Count().ToString(), "bx bx-question-mark", bgColor: BgColors.Danger);
            StatsCardModel pending = new StatsCardModel(Resources.Resources.Pending, bug.Where(x => x.status == "Pending").Count().ToString(), "bx bx-timer", bgColor: BgColors.Success);
            //StatsCardModel completed  = new StatsCardModel(Resources.Resources.Completed, bug.Where(x => x.status == "مكتمل").Count().ToString(), "bx bx-check-shield", bgColor: BgColors.Info);
            StatsCardModel closed = new StatsCardModel("مغلق", bug.Where(x => x.status == "مغلق").Count().ToString(), "bx bx-check-shield", bgColor: BgColors.Info);

            List<StatsCardModel> statsCards = new List<StatsCardModel>();
            statsCards.Add(total);
            statsCards.Add(pending);
            //statsCards.Add(completed);
            statsCards.Add(closed);

            ViewBag.StatsCards = statsCards;

            if (!String.IsNullOrEmpty(Search_Data))
            {
                return View(bug.OrderByDescending(x => x.id).ToPagedList(No_Of_Page, pagesize));
            }

            return View(bug.OrderByDescending(x => x.id).ToPagedList(No_Of_Page, pagesize));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(bug_tracker bt, string notes)
        {
            if (currentPageID("bug_tracker") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("bug_tracker")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            bug_tracker bug = db.bug_tracker.Find(bt.id);
            if (bug.status != bt.status)
            {
                bug.status = bt.status;
                db.Entry(bug).State = EntityState.Modified;
                db.SaveChanges();

                bug_history bh = new bug_history();
                bh.bug_id = bug.id;
                bh.status = bt.status;
                bh.msg = notes;
                bh.created_at = DateTime.Now;
                bh.created_by = CurrentUser.Id;
                db.bug_history.Add(bh);
                db.SaveChanges();

                // for closed status then send notification to user which role is farmer or consultant
                if (bt.status == "مغلق")
                {
                    user u = db.users.Find(bug.created_by);
                    if (u != null && (u.role_id == 6 || u.role_id == 5))
                    {
                        Generic g = new Generic();
                        g.sendNotificationOnPriceUpdate("تم حل المشكله", "نشكر مشاركتك ويسعدنا ابلاغك بالإنتهاء من حل المشكلة", u.phone);
                    }
                }

                return RedirectToAction("Index");
            }

            return RedirectToAction("NotAllow", "Custom");
        }

        [HttpGet]
        public ActionResult View(int id)
        {
            if (currentPageID("bug_tracker") > 0)
            {
                if (!CurrentUser.canView(currentPageID("bug_tracker")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            bug_tracker bt = db.bug_tracker.Find(id);
            string filecheck = bt.image != null ? bt.image.Split('.').Last() : "";
            ViewBag.isPhoto = GeneralExtensions.isPicture(filecheck);
            ViewBag.bug_history = db.bug_history.Where(x => x.bug_id == bt.id).ToList();
            return View(bt);
        }

        public ActionResult Create()
        {
            if (currentPageID("bug_tracker") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("bug_tracker")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            return View();
        }

        // for end user creating this view
        // just for keep the bug tracker
        // it takes msg, image if there, and page name automatically comes from the view
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string msg, HttpPostedFileBase image, string page)
        {
            if (currentPageID("bug_tracker") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("bug_tracker")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            bug_tracker bt = new bug_tracker();
            bt.msg = msg;
            bt.page = page;
            bt.type = 1; // 1 for web and 2 for mobile
            bt.created_by = CurrentUser.Id;
            Guid imageGuid = Guid.NewGuid();

            if (image != null && image.ContentLength > 0)
            {
                var img = imageGuid.ToString() + Path.GetExtension(image.FileName);

                var path = Path.Combine(Server.MapPath("~/Media/bugs/"), img);
                image.SaveAs(path);
                bt.image = "/Media/bugs/" + imageGuid.ToString() + Path.GetExtension(image.FileName);
            }

            bt.created_at = DateTime.Now;
            bt.status = "Pending";
            db.bug_tracker.Add(bt);
            db.SaveChanges();

            return RedirectToAction("Index", "Dashboard");
        }
        
    }
}