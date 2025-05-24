using PagedList;
using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MurshadikCP.Controllers
{
    [Authorize]
    public class LabController : BaseController
    {
        // GET: Lab
        // Index Method shows list of labs in the view
        public ActionResult Index(int? Page_No)
        {
            if (currentPageID("Lab") > 0)
            {
                if (!CurrentUser.canView(currentPageID("Lab")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            ViewBag.role = CurrentUser.RoleId;
            int pagesize = 10;
            int No_Of_Page = (Page_No ?? 1);
            ViewBag.completed = db.appointments.Where(x => x.is_completed == true).Count();
            ViewBag.totalappointment = db.appointments.Where(x => x.is_booked == true).Count();
            ViewBag.pending = db.appointments.Where(x => x.is_sample_collected == true && x.is_completed == false).Count();
            if (CurrentUser.labid > 0)
            {
                return View(db.labs.Where(x => x.id == CurrentUser.labid).OrderByDescending(x => x.id).ToPagedList(No_Of_Page, pagesize));
            }
            else
            {
                return View(db.labs.OrderByDescending(x => x.id).ToPagedList(No_Of_Page, pagesize));
            }
        }

        // Add views show the form to add the lab information
        public ActionResult Add()
        {
            if (currentPageID("Lab") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("Lab")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            ViewBag.role = CurrentUser.RoleName;
            ViewBag.city_id = new SelectList(db.cities.ToList(), "id", "name_ar");
            return View();
        }

        // edit view show the form to modify the lab
        public ActionResult Edit(int id)
        {
            if (currentPageID("Lab") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("Lab")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (CurrentUser.RoleId != (int)Role.SuperAdmin)
            {
                if (id != CurrentUser.labid)
                {
                    return RedirectToAction("NotAllow", "Custom");
                }
            }

            ViewBag.role = CurrentUser.RoleName;
            lab l = db.labs.Where(x => x.id == id).FirstOrDefault();
            if (l != null)
            {
                string day = "'" + l.working_days.Replace(",", "','") + "'";
                ViewBag.day = day;
                ViewBag.city_id = new SelectList(db.cities.ToList(), "id", "name_ar", l.city_id);
            }
            return View(l);
        }

        // add lab here
        // data comes from the form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(lab l, string open_at, string close_at, string[] working_days)
        {
            if (currentPageID("Lab") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("Lab")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            lab lab = new lab();
            if (l.id > 0)
            {
                lab = db.labs.Where(x => x.id == l.id).FirstOrDefault();
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
            lab.city_id = l.city_id;
            lab.max_appointment_per_day = l.max_appointment_per_day;
            lab.appointment_interval = l.appointment_interval;
            
            if (l.id == 0)
            {
                lab.created_at = DateTime.Now;
                db.labs.Add(lab);
                db.SaveChanges();
                for (int y = 0; y < 30; y++)
                {
                    DateTime dt = DateTime.Now.AddDays(y);
                    string[] days = lab.working_days.Split(',');
                    string NameofDay = "";
                    foreach (var item in days)
                    {
                        NameofDay = NameofDay + getEnglishDayName(item) + ",";
                    }

                    if (NameofDay.Contains(dt.DayOfWeek.ToString()))
                    {
                        db.GenerateAppointments(Convert.ToInt16(lab.id), dt);
                    }

                    db.GenerateAppointments(Convert.ToInt16(lab.id), dt);
                }

            }
            else
            {
                lab.updated_at = DateTime.Now;
                db.Entry(lab).State = EntityState.Modified;
                db.SaveChanges();
            }
            
            return RedirectToAction("Index");
        }

        public string getEnglishDayName(string day)
        {
            string dayName = "";
            if (day == "الأحد")
                dayName = "Sunday";
            else if (day == "الاثنين")
                dayName = "Monday";
            else if (day == "الثلاثاء")
                dayName = "Tuesday";
            else if (day == "الأربعاء")
                dayName = "Wednesday";
            else if (day == "الخميس")
                dayName = "Thursday";
            else if (day == "الجمعة")
                dayName = "Friday";
            else if (day == "السبت")
                dayName = "Saturday";
            return dayName;
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            if (currentPageID("Lab") > 0)
            {
                if (!CurrentUser.canDelete(currentPageID("Lab")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            lab lab = db.labs.Find(id);
            appointment app = db.appointments.Where(x => x.lab_id == id && x.is_booked == true).FirstOrDefault();
            if (app == null)
            {
                db.labs.Remove(lab);
                db.SaveChanges();
                return Json("Success");
            }
            else
            {
                return Json("Appointment Exists & booking also exits! so can't delete lab");
            }
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