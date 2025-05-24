using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using MurshadikCP.Models;
using System.Data.Entity;
using System.IO;
using System.Net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace MurshadikCP.Controllers
{
    [Authorize]
    public class AppointmentsController : BaseController
    {
        // GET: Appointments
        // appointments list show in this view
        // startdate, enddate shows if use put any date for search then the result shows according to that
        public ActionResult Index(string startdate, string enddate, int id = 0)
        {
            if (currentPageID("Appointments") > 0)
            {
                if (!CurrentUser.canView(currentPageID("Appointments")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            bool hasAdminAccess = new long[] { (long)Role.SuperAdmin, (long)Role.Manager }.Contains(CurrentUser.RoleId);
            ViewBag.role = CurrentUser.RoleName;
            if (id > 0)
            {
                Models.DB.UserAccessTypeIdentifier uati = db.UserAccessTypeIdentifiers.Where(x => x.user_id == CurrentUser.Id).FirstOrDefault();
                //user_access lab_user = db.user_access.Where(x => x.userid == CurrentUser.Id).FirstOrDefault();
                if (uati == null && !hasAdminAccess)
                {
                    return RedirectToAction("pages-labpermission", "Pages");
                }
                else
                {
                    if (id == CurrentUser.labid || hasAdminAccess)
                    {
                        if (startdate != "" && startdate != null)
                        {
                            DateTime sd = Convert.ToDateTime(startdate);
                            DateTime ed = Convert.ToDateTime(enddate);
                            ViewBag.startdate = sd;
                            ViewBag.enddate = ed;
                            AppointmentLabViewModel result = GetAppointmentsByDates(id, sd, ed);
                            return View(result);
                        }
                        else
                        {
                            AppointmentLabViewModel result = GetAppointments(id);
                            return View(result);
                        }
                    }
                    else
                    {
                        return RedirectToAction("pages-labpermission", "Pages");
                    }
                }
            }
            else
            {
                AppointmentLabViewModel result = GetAppointments(Convert.ToInt32(CurrentUser.labid));
                return View(result);
            }

            return RedirectToAction("pages-labpermission", "Pages");
        }

        // this method takes labid, start date, end date and return lab appointments, report pending and report completed.
        public AppointmentLabViewModel GetAppointmentByDates(int labid, string start, string end)
        {
            DateTime sd = Convert.ToDateTime(start);
            DateTime ed = Convert.ToDateTime(end);
            var tables = new AppointmentLabViewModel
            {
                appointments = db.appointments.Where(x => x.appointment_date == DateTime.Today && x.lab_id == labid).ToList(),
                lab = db.labs.Where(x => x.id == labid).FirstOrDefault(),
                lab_Reports_pending = db.lab_reports.Where(x => x.reported_by == null && x.lab_id == labid && (x.collected_date >= sd || x.collected_date <= ed)).ToList(),
                lab_Reports_completed = db.lab_reports.Where(x => x.reported_by != null && x.lab_id == labid && (x.collected_date >= sd || x.collected_date <= ed)).ToList(),
            };

            return tables;
        }

        // this method takes labid, start date, end date and return lab appointments, report pending and report completed.
        public AppointmentLabViewModel GetAppointmentsByDates(int labid, DateTime sd, DateTime ed)
        {
            var tables = new AppointmentLabViewModel
            {
                appointments = db.appointments.Where(x => x.appointment_date >= sd.Date && x.appointment_date <= ed.Date && x.lab_id == labid).ToList(),
                lab = db.labs.Where(x => x.id == labid).FirstOrDefault(),
                lab_Reports_pending = db.lab_reports.Where(x => x.reported_by == null && x.lab_id == labid).OrderByDescending(x => x.created_at).ToList(),
                lab_Reports_completed = db.lab_reports.Where(x => x.reported_by != null && x.lab_id == labid).OrderByDescending(x => x.created_at).ToList(),
            };

            return tables;
        }

        // this method takes lab id and show that data according it.
        public AppointmentLabViewModel GetAppointments(int labid)
        {
            var tables = new AppointmentLabViewModel
            {
                appointments = db.appointments.Where(x => x.appointment_date == DateTime.Today && x.lab_id == labid).ToList(),
                lab = db.labs.Where(x => x.id == labid).FirstOrDefault(),
                lab_Reports_pending = db.lab_reports.Where(x => x.reported_by == null && x.lab_id == labid).OrderByDescending(x => x.created_at).ToList(),
                lab_Reports_completed = db.lab_reports.Where(x => x.reported_by != null && x.lab_id == labid).OrderByDescending(x => x.created_at).ToList(),
            };

            return tables;
        }

        // this method refer View for Display Appointment
        // takes appointment ID
        public ActionResult DisplayAppointment(int id)
        {
            if (currentPageID("Appointments") > 0)
            {
                if (!CurrentUser.canView(currentPageID("Appointments")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            ViewBag.role = CurrentUser.RoleName;
            appointment a = db.appointments.Where(x => x.id == id).FirstOrDefault();
            return View(a);
        }

        // Cancel appointment if that appointment is not completed
        [HttpPost]
        public JsonResult CancelAppointment(long id)
        {
            appointment a = db.appointments.Where(x => x.id == id).FirstOrDefault();

            user u = db.users.Where(x => x.id == a.user_id).FirstOrDefault();

            a.user_id = null;
            a.is_booked = false;
            db.Entry(a).State = EntityState.Modified;
            db.SaveChanges();

            Generic g = new Generic();
            g.sendNotificationOnChats(u.name + " " + u.last_name, "تم إلغاء موعدك", u.phone, "lab_appointment_cancelled", a.lab_id.ToString());

            return Json("Cancel Successfully");
        }


        // this method shows lab report and takes the lab_report id
        public ActionResult DisplayLabReport(int id)
        {
            if (currentPageID("Appointments") > 0)
            {
                if (!CurrentUser.canView(currentPageID("Appointments")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            ViewBag.role = CurrentUser.RoleName;
            lab_reports lr = db.lab_reports.Where(x => x.id == id).FirstOrDefault();
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "تربة و الماء",
                Value = "1"
            });
            items.Add(new SelectListItem
            {
                Text = "التربة",
                Value = "2"
            });
            items.Add(new SelectListItem
            {
                Text = "ماء",
                Value = "3"
            });
            ViewBag.reporttype = new SelectList(items, "Value", "Text", lr.reporttype);
            //ViewBag.reporttype = items;
            return View(lr);
        }

        // after analysis the sample which provided by the farmer so it will be create lab report based on the parameters
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(lab_reports lr, HttpPostedFileBase labreportpdf)
        {
            if (currentPageID("Appointments") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("Appointments")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            lab_reports report = db.lab_reports.Where(x => x.id == lr.id).FirstOrDefault();
            Guid imgGuid = Guid.NewGuid();
            if (labreportpdf != null && labreportpdf.ContentLength > 0)
            {
                var pdf = imgGuid.ToString() + Path.GetExtension(labreportpdf.FileName);
                var path = Path.Combine(Server.MapPath("~/Media/PDF/Lab_Reports/"), pdf);
                labreportpdf.SaveAs(path);
                lr.extra1 = pdf;
                report.extra1 = lr.extra1;
            }
            report.soil_calcium_carbonate = lr.soil_calcium_carbonate;
            report.soil_clay = lr.soil_clay;
            report.soil_electric_conductivity = lr.soil_electric_conductivity;
            report.soil_notes = lr.soil_notes;
            report.soil_ph = lr.soil_ph;
            report.soil_phosphorus_ppm = lr.soil_phosphorus_ppm;
            report.soil_potasium_ppm = lr.soil_potasium_ppm;
            report.soil_recommendations = lr.soil_recommendations;
            report.soil_salt = lr.soil_salt;
            report.soil_sand = lr.soil_sand;
            report.soil_texture_type = lr.soil_texture_type;
            report.water_carbonate = lr.water_carbonate;
            report.water_chloride = lr.water_chloride;
            report.water_electric_conductivity = lr.water_electric_conductivity;
            report.water_notes = lr.water_notes;
            report.water_ph = lr.water_ph;
            report.water_recommendation = lr.water_recommendation;
            report.water_salts = lr.water_salts;
            report.reported_by = CurrentUser.Id.ToString();
            report.reporttype = lr.reporttype;
            
            

            db.Entry(report).State = EntityState.Modified;
            db.SaveChanges();

            int lab_repot = db.lab_reports.Where(x => x.appointment_id == lr.appointment_id).Count();
            int lab_report = db.lab_reports.Where(x => x.appointment_id == lr.appointment_id && x.reported_by != null).Count();

            if (lab_repot == lab_report)
            {
                appointment appointment = db.appointments.Find(lr.appointment_id);
                appointment.is_completed = true;
                db.Entry(appointment).State = EntityState.Modified;
                db.SaveChanges();
                string sampleNo = String.Join(",", db.lab_reports.Where(x => x.appointment_id == lr.appointment_id).Select(x => x.sample_no).ToList());
                Generic g = new Generic();
                g.sendNotificationOnChats("تم إنشاء التقرير", "تقرير المختبر الخاص بك جاهز الان", report.user.phone, "lab_report_available", report.lab_id.ToString());


                g.InsertNotificationData(report.user_id, "تقرير المعمل الخاص بك للعينة رقم [" + sampleNo + "] متاح الآن", (Int16)AppConstants.Type.Lab, report.lab_id, "");
            }
            return RedirectToAction("Index", new { id = report.lab_id });
        }

        // creating appointment for new user if he has not the appointment
        // check also the user if he exists then no need to create first
        // else create user first then create appointment
        // and also collecting the no of samples
        [HttpPost]
        public JsonResult CreateAppointmentForNewUser(string name, string mobile, int noofsample, int labid)
        {
            string[] sampleNos = new string[noofsample];

            user user = db.users.Where(x => x.phone == mobile).FirstOrDefault();
            if (user == null)
            {
                MurshadikAPIController murshadikAPI = new MurshadikAPIController();
                murshadikAPI.RegisterUser_ForLabNewUser(name, "المملكة العربية السعودية", 5, mobile);
            }

            user = db.users.Where(x => x.phone == mobile).FirstOrDefault();
            if (user != null) // if user exists then normal workflow going on
            {
                appointment a = new appointment();
                a.lab_id = labid;
                a.user_id = user.id;
                a.appointment_date = DateTime.Now;
                DateTime dt = a.appointment_date.Value.Date;
                appointment existsapp = db.appointments.Where(lab => lab.lab_id == labid && lab.appointment_date == dt).FirstOrDefault();

                TimeSpan time1 = TimeSpan.FromHours(1);
                a.appointment_time = existsapp != null ? existsapp.appointment_time.Value.Add(time1) : DateTime.Now.TimeOfDay.Add(time1);
                a.is_sample_collected = true;
                a.is_booked = true;
                a.no_samples = noofsample;
                a.created_at = DateTime.Now;
                a.updated_at = DateTime.Now;
                db.appointments.Add(a);
                db.SaveChanges();

                for (int i = 0; i < noofsample; i++)
                {
                    lab_reports lr = new lab_reports();
                    lr.lab_id = Convert.ToInt16(a.lab_id);
                    lr.appointment_id = Convert.ToInt16(a.id);
                    lr.user_id = (long)a.user_id;
                    lr.collected_date = DateTime.Now;
                    lr.collected_by = CurrentUser.Id.ToString();
                    Random generator = new Random();
                    String r = generator.Next(0, 999999).ToString("D6");
                    string geneateSampleNo = a.lab_id.ToString() + "-" + CurrentUser.Id.ToString() + "-" + i.ToString() + "-" + r;
                    sampleNos[i] = geneateSampleNo;
                    lr.sample_no = geneateSampleNo;
                    lr.created_at = DateTime.Now;
                    lr.reported_date = DateTime.Now;
                    db.lab_reports.Add(lr);
                    db.SaveChanges();
                }

                return Json(" noofSample : " + string.Join(",", sampleNos));
            }

            return Json(" Error : appointment doesn't exists!");
        }

        // after appointment lab manager takes samples from farmer and update here.
        [HttpPost]
        public JsonResult CreateAppointment(int id,int noofsample)//, string appointmentTime, int labid, int region_id)
        {
            if (id > 0)
            {
                string[] sampleNos = new string[noofsample];

                appointment a = db.appointments.Where(x => x.id == id).FirstOrDefault();
                a.is_sample_collected = true;
                a.no_samples = noofsample;
                db.Entry(a).State = EntityState.Modified;
                db.SaveChanges();

                for (int i = 0; i < noofsample; i++)
                {
                    lab_reports lr = new lab_reports();
                    lr.lab_id = Convert.ToInt16(a.lab_id);
                    lr.appointment_id = a.id;
                    lr.user_id = (long)a.user_id;
                    lr.collected_date = DateTime.Now;
                    lr.collected_by = CurrentUser.Id.ToString();
                    Random generator = new Random();
                    String r = generator.Next(0, 999999).ToString("D6");
                    string geneateSampleNo = a.lab_id.ToString() + "-" + CurrentUser.Id.ToString() + "-" + i.ToString() + "-" + r;
                    sampleNos[i] = geneateSampleNo;
                    lr.sample_no = geneateSampleNo;
                    lr.created_at = DateTime.Now;
                    lr.reported_date = DateTime.Now;
                    db.lab_reports.Add(lr);
                    db.SaveChanges();
                }

                return Json(" noofSample : " + string.Join(",", sampleNos));
            }
            else
            {
                return Json(" Error : appointment doesn't exists!");
            }
        }
    }
}