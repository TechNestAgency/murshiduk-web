using PagedList;
using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MurshadikCP.Controllers
{
    [Authorize]
    public class FarmersController : BaseController
    {
        // GET: Farmers
        // Index view shows the users which role_id = 5
        // pagination and search criteria also there
        // data comes from the user table based on role id
        public ActionResult Index(int? Page_No, string Search_Data, string Filter_Value, string skill, int Region = 0, int active = 0, string start_date = null, string end_date = null)
        {
            if (currentPageID("Farmers") > 0)
            {
                if (!CurrentUser.canView(currentPageID("Farmers")))
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

            ViewBag.Region = new SelectList(db.regions, "id", "name_ar");
            ViewBag.Filter_Value = Search_Data;
            ViewBag.Filter_region = Region;
            ViewBag.Filter_active = active;
            ViewBag.Filter_skill = skill;

            ViewBag.totalFarmers = db.users.Where(x => x.role_id == 5).Count();
            ViewBag.totalActive = db.users.Where(x => x.role_id == 5 && x.is_profile_completed == true).Count();
            ViewBag.totalInactive = db.users.Where(x => x.role_id == 5 && x.is_profile_completed == false).Count();

            IQueryable<user> users = from user in db.users where user.role_id == (long)UserType.Farmer select user;

            if (!string.IsNullOrEmpty(start_date))
            {
                ViewBag.Start_Date = start_date;
                var sdt = DateTime.Parse(start_date, null, System.Globalization.DateTimeStyles.RoundtripKind);
                users = users.Where(art => art.created_at >= sdt);
            }
            if (!string.IsNullOrEmpty(end_date))
            {
                ViewBag.End_Date = end_date;
                var edt = DateTime.Parse(end_date, null, System.Globalization.DateTimeStyles.RoundtripKind);
                users = users.Where(art => art.created_at <= edt);
            }

            if (!String.IsNullOrEmpty(Search_Data))
            {
                users = users.Where(x => x.name.Contains(Search_Data) || x.last_name.Contains(Search_Data) || x.name + " " + x.last_name == Search_Data.Trim()
                || x.phone.Contains(Search_Data));

            }

            if (Region > 0)
            {
                users = users.Where(x => x.region_id == Region);
            }
            if (active > 0)
                users = users.Where(x => x.is_profile_completed == (active == 1) ? true : false);

            if (skill != "" && skill != null && skill != "0")
            {
                users = users.Where(x => x.skills != null && x.skills.Contains(skill));
            }

            return View(users.OrderByDescending(x => x.created_at).ToPagedList(No_Of_Page, pagesize));
        }

        // block farmer from here
        [HttpPost, ActionName("Block")]
        //[ValidateAntiForgeryToken]
        public ActionResult block(long id, bool active)
        {
            if (currentPageID("Farmers") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("Farmers")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            user u = db.users.Find(id);
            u.active = !active ? true : false;
            u.updated_at = DateTime.Now;
            db.Entry(u).State = EntityState.Modified;
            db.SaveChanges();
            string message = active ? "تم الحظر" : "تم رفع الحظر";

            return Json(new ErrorModel(message, true));
        }

        [HttpPost, ActionName("ChangeRole")]
        public ActionResult ChangeRole(long id)
        {
            if (currentPageID("Farmers") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("Farmers")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            user u = db.users.Find(id);
            u.active = true;
            u.role_id = 6;
            u.updated_at = DateTime.Now;
            db.Entry(u).State = EntityState.Modified;
            db.SaveChanges();
            return Json("Success");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchFarmers(string q)
        {
            if (q.Length <= 3)
            {
                return Json(null);
            }

            var u = db.users
                            .Where(
                                user => user.role_id == 5 && (user.name.Contains(q) || user.last_name.Contains(q) || user.phone.Contains(q))
                                )
                            .Select(user => new { id = user.id, name = user.name + " - " + user.last_name + " - " + user.phone + " - " + user.region1.name_ar })
                            .ToList();
            return Json(u);
        }
    }
}