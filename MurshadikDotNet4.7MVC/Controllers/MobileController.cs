using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MurshadikCP.Controllers
{
    public class MobileController : Controller
    {
        private mlaraEntities db = new mlaraEntities();
        UserInfo ui = new UserInfo();
        // GET: Mobile
        // this view for using the mobile application just pass the article id
        public ActionResult Index(long id)
        {
            article a = db.articles.Where(x => x.id == id).FirstOrDefault();
            if (a.category != null && a.category.name == "امراض البن")
            {
                return View("Bun", a);
            }
            else
            {
                return View(a);
            }
        }

        public ActionResult Cbsios()
        {
            return View();
        }

        public ActionResult Cbs()
        {
            return View();
        }

        public ActionResult AboutUs()
        {
            app_settings result = db.app_settings.Where(x => x.ap_key == SettingKeys.AboutUs).FirstOrDefault();
            ViewBag.Data = result.ap_value;
            return View("Generic");
        }

        public ActionResult Terms()
        {
            app_settings result = db.app_settings.Where(x => x.ap_key == SettingKeys.TermAndCondition).FirstOrDefault();
            ViewBag.Data = result.ap_value;
            return View("Generic");
        }

        [HttpGet]
        public ActionResult reportabug(string uID = "0")
        {
            ViewBag.uID = uID != "0" ? db.users.Where(x => x.phone == uID).FirstOrDefault().id : 0;
            return View();  
        }

        [HttpGet]
        public ActionResult Faqs()
        {
            var result = db.faqs.Where(x => x.active == true).ToList();
            return View(result);
        }

        [HttpPost]
        public ActionResult reportabug(string msg, HttpPostedFileBase image, string page, long uID)
        {
            bug_tracker bt = new bug_tracker();
            bt.msg = msg;
            bt.page = page;
            bt.type = 2;
            bt.created_by = uID == 0 ? 3 : uID;// create here a new user for bug tracker from mobile application
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
            string phone = uID != 0 ? db.users.Find(uID).phone : "0";
            return RedirectToAction("ReportBugMessage", new { insert = true, uID = phone });
        }

        public ActionResult ReportBugMessage(string uID, bool insert = false)
        {
            if (insert)
            {
                ViewBag.showmessage = "تمت الإضافة بنجاح";
            }
            ViewBag.uID = uID;
            return View();
        }
    }
}