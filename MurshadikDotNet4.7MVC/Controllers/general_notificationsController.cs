using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MurshadikCP.Controllers
{
    public class general_notificationsController : BaseController
    {
        // GET: general_notifications
        public ActionResult Index()
        {
            if (currentPageID("general_notifications") > 0)
            {
                if (!CurrentUser.canView(currentPageID("general_notifications")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            general_notifications gn = new general_notifications();
            return View(gn);
        }

        [HttpPost]
        public ActionResult Send(general_notifications gn)
        {
            if (currentPageID("general_notifications") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("general_notifications")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            gn.created_at = DateTime.Now;
            gn.created_by = CurrentUser.Id;
            db.general_notifications.Add(gn);
            db.SaveChanges();

            Generic g = new Generic();
            string[] city_code = db.cities.Where(x => x.weather_identifier != null).Select(x => x.weather_identifier).Distinct().ToArray();

            g.sendGeneralNotification(gn.title, gn.message.Length > 1024 ? gn.message.Substring(0, 1024) : gn.message, city_code);

            return RedirectToAction("Index");
        }
    }
}