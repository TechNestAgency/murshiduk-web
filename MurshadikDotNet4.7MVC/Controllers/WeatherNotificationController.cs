using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using PagedList;
using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MurshadikCP.Controllers
{
    public class WeatherNotificationController : BaseController
    {
        //private mlaraEntities db = new mlaraEntities();
        UserInfo ui = new UserInfo();
        // GET: WeatherNotification
        public ActionResult Index(int? Page_No, string Search_Data, string Filter_Value)
        {
            ui = (UserInfo)Session["User"];
            if (ui != null)
            {
                List<roles_permission> roles = ui.roles_Permissions.ToList();
                if (roles != null)
                {
                    if (roles.Exists(x => x.Page.PageName == "WeatherNotification"))
                    {
                        if (roles.Find(x => x.Page.PageName == "WeatherNotification").can_view)
                        {
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

                            ViewBag.Filter_Value = Search_Data;

                            if (!String.IsNullOrEmpty(Search_Data))
                            {
                                return View(db.weather_notification.Where(x => x.title_ar.Contains(Search_Data) || x.title_en.Contains(Search_Data) 
                                || x.region.name_ar.Contains(Search_Data) || x.season.Contains(Search_Data) || x.notification_class.notification_class1.Contains(Search_Data)
                                || x.city.Contains(Search_Data) || x.message.Contains(Search_Data) || x.link.Contains(Search_Data) || x.link_title.Contains(Search_Data)
                                ).OrderByDescending(x => x.id).ToPagedList(No_Of_Page, pagesize));
                            }

                            return View(db.weather_notification.OrderByDescending(x => x.id).ToPagedList(No_Of_Page, pagesize));
                        }
                    }
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            return RedirectToAction("NotAllow", "Custom");
        }

        // GET: WeatherNotification/Create
        public ActionResult Create()
        {
            ui = (UserInfo)Session["User"];
            if (ui != null)
            {
                List<roles_permission> roles = ui.roles_Permissions.ToList();
                if (roles != null)
                {
                    if (roles.Exists(x => x.Page.PageName == "WeatherNotification"))
                    {
                        if (roles.Find(x => x.Page.PageName == "WeatherNotification").can_insert)
                        {
                            ViewBag.region_id = new SelectList(db.regions.ToList(), "id", "name_ar");
                            //ViewBag.city = new SelectList(db.cities.Where(x => x.region_id == 1).ToList(), "name_ar", "name_ar");
                            ViewBag.notification_class_id = new SelectList(db.notification_class.ToList(), "id", "notification_class1");
                            return View();
                        }
                    }
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            return RedirectToAction("NotAllow", "Custom");
        }

        public ActionResult bindCityForAlertMessages(int id)
        {
            ViewBag.city = new SelectList(db.cities.Where(x => x.region_id == id).ToList(), "name_ar", "name_ar");

            return PartialView("_MultiCity");
        }

        public ActionResult bindCity(int id)
        {
            ViewBag.city = new SelectList(db.cities.Where(x => x.region_id == id).ToList(), "name_ar", "name_ar");

            return PartialView("_MultiCity");
        }

        // POST: cities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(weather_notification wn, string allseason, string allcity)
        {
            if (ModelState.IsValid)
            {
                wn.season = allseason;
                wn.city = allcity;
                db.weather_notification.Add(wn);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(wn);
        }

        [HttpPost]
        public ActionResult AddClass(string className)
        {
            if (className != "")
            {
                notification_class nc = new notification_class();
                nc.notification_class1 = className;
                db.notification_class.Add(nc);
                db.SaveChanges();
                return Json("Success");
            }
            else
            {
                return Json("Error");
            }
        }

        // GET: WeatherNotification/Edit/5
        public ActionResult Edit(int? id)
        {
            ui = (UserInfo)Session["User"];
            if (ui != null)
            {
                List<roles_permission> roles = ui.roles_Permissions.ToList();
                if (roles != null)
                {
                    if (roles.Exists(x => x.Page.PageName == "WeatherNotification"))
                    {
                        if (roles.Find(x => x.Page.PageName == "WeatherNotification").can_update)
                        {
                            if (id == null)
                            {
                                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                            }
                            weather_notification wn = db.weather_notification.Find(id);
                            if (wn == null)
                            {
                                return HttpNotFound();
                            }
                            string allCities = "'" + wn.city.Replace(",", "','") + "'";
                            string allSeason = "'" + wn.season.Replace(",", "','") + "'";
                            ViewBag.region_id = new SelectList(db.regions.ToList(), "id", "name_ar", wn.region_id);
                            ViewBag.city = new SelectList(db.cities.ToList(), "name_ar", "name_ar", wn.city);
                            ViewBag.allcity = allCities;
                            ViewBag.season = allSeason;
                            ViewBag.notification_class_id = new SelectList(db.notification_class.ToList(), "id", "notification_class1");
                            return View(wn);
                        }
                    }
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            return RedirectToAction("NotAllow", "Custom");
        }

        // POST: cities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(weather_notification wn, string allseason, string allcity)
        {
            if (ModelState.IsValid)
            {
                //int a = wn.message.Count();
                wn.season = allseason;
                wn.city = allcity;
                db.Entry(wn).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(wn);
        }

        // POST: WeatherNotification/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ui = (UserInfo)Session["User"];
            if (ui != null)
            {
                List<roles_permission> roles = ui.roles_Permissions.ToList();
                if (roles != null)
                {
                    if (roles.Exists(x => x.Page.PageName == "WeatherNotification"))
                    {
                        if (roles.Find(x => x.Page.PageName == "WeatherNotification").can_delete)
                        {
                            if (id == 0)
                            {
                                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                            }
                            weather_notification wn = db.weather_notification.Find(id);
                            db.weather_notification.Remove(wn);
                            db.SaveChanges();
                            return Json("Success");
                            //return RedirectToAction("Index");
                        }
                    }
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            return RedirectToAction("NotAllow", "Custom");
        }

        public ActionResult getWeatherDataAsync()
        {
            try
            {
                HttpClient clinet = new HttpClient();
                string weatherURL = System.Configuration.ConfigurationManager.AppSettings["WeatherAPIURL"] + "GetWeatherInfo";
                var result = clinet.GetAsync(weatherURL);
                result.Wait();

                var result1 = result.Result;
                var test = result1.Content.ReadAsStringAsync();
                test.Wait();

                List<WeatherJson> weatherOfCities = JsonConvert.DeserializeObject<List<WeatherJson>>(test.Result);

                var cities = db.cities.Where(c => c.weather_identifier != null).ToList();
                List<string> weather_id = new List<string>();
                foreach (var wcity in weatherOfCities)
                {
                    var thisCity = cities.Where(c => c.weather_identifier == wcity.cityCode).FirstOrDefault();

                    if (thisCity == null)
                    {
                        continue;
                    }

                    DateTime dt = DateTime.ParseExact(wcity.todayDate, "M/d/yyyy h:m:s tt", CultureInfo.InvariantCulture);
                    DateTime dateTime = Convert.ToDateTime(wcity.todayDate);
                    WeatherData weather = db.WeatherDatas.Where(x => x.city_id == thisCity.id && x.region_id == thisCity.region_id && x.updated_at == dateTime).OrderByDescending(x => x.id).FirstOrDefault();

                    if (weather == null)
                    {
                        WeatherData wd = new WeatherData()
                        {
                            region_id = thisCity.region_id,
                            city_id = thisCity.id,
                            weather = JsonConvert.SerializeObject(wcity),
                            city_identifier = thisCity.weather_identifier,
                            created_at = DateTime.Now,
                            updated_at = dt
                        };
                        weather_id.Add(wcity.weather_PhenomenonEn + "," + thisCity.weather_identifier);
                        db.WeatherDatas.Add(wd);
                    }
                }

                // Saving all at once drastically improves response time. 
                db.SaveChanges();

                if (weather_id.Count() > 0)
                {
                    var currentlist = db.currentweathernotificationforCities.ToList();
                    db.currentweathernotificationforCities.RemoveRange(currentlist);
                    db.SaveChanges();

                    List<NotificationDTO> message = new List<NotificationDTO>();
                    foreach (var item in weather_id)
                    {
                        string[] datassz = item.Split(',');
                        NotificationDTO m = GetWeatherNotificationMessage(datassz[0], datassz[1]);
                        if (m != null)
                        {
                            message.Add(m);
                        }
                    }

                    Generic gn = new Generic();
                    var response = gn.sendMultipleNotification(message);
                }

                

                var wdata = db.WeatherDatas.ToList();
                return View(); //return wdata.Select(wds => JsonConvert.DeserializeObject<WeatherData>(wds.weather)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }


            return null;
        }

        public string SendWeatherNotification(string Weather_icon, string city_identifier)
        {
            notification_class nc = db.notification_class.Where(x => x.notification_class1 == Weather_icon).FirstOrDefault();
            if (nc != null)
            {
                long wi = nc.id;
                weather_notification wn = db.weather_notification.Where(x => x.notification_class_id == wi).FirstOrDefault();
                if (wn != null)
                {
                    string title = wn.title_ar;
                    string desc = wn.message;
                    string topic = city_identifier;
                    return title + "`" + desc + "`" + topic;
                }
            }

            return null;
        }

        public NotificationDTO GetWeatherNotificationMessage(string alert, string city_identifier)
        {
            var n = (from am in db.AlertMessages
                    join ah in db.AlertHazards on am.alertHazardId equals ah.Id
                    where ah.name == alert
                    select new NotificationDTO()
                    {
                        title = ah.name_ar,
                        body = am.message,
                        topic = city_identifier
                    }).FirstOrDefault();

            NotificationLog nl = new NotificationLog();
            nl.created_at = DateTime.Now;
            nl.log_type = (int)LogType.Weather;
            if (n == null)
            {

                nl.is_processed = false;
                nl.log_message = alert + " or message not found for " + city_identifier;
            }
            else
            {
                List<city> cities = db.cities.Where(x => x.weather_identifier == city_identifier).ToList();
                if (cities != null)
                {
                    foreach (var item in cities)
                    {
                        currentweathernotificationforCity cwnc = new currentweathernotificationforCity();
                        cwnc.city_id = item.id;
                        cwnc.message = n.body;
                        cwnc.region_id = item.region_id ?? 0;
                        cwnc.title = n.title; 
                        cwnc.alert_hazard_id = 1; // later changed
                        cwnc.alert_type_id = 1; // later changed
                        cwnc.service_id = item.id;
                        db.currentweathernotificationforCities.Add(cwnc);
                    }
                }

                //currentweathernotificationforCity wc = new currentweathernotificationforCity();
                //wc.city_id = //get all cities from this city_identifier and insert all notifications
                nl.is_processed = true;
                nl.log_message = alert + " | " + n.body + " | " + city_identifier;
            }
            db.NotificationLogs.Add(nl);
            db.SaveChanges();
            return n;
        }

        public async Task<object> sendMultipleNotification(List<NotificationDTO> message)
        {
            try
            {
                var messaging = FirebaseMessaging.DefaultInstance;
                if (messaging == null)
                {
                    var pathToKey = @"D:\fbadmin.json";//Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fbadmin.json");
                                                       //Path.Combine(HttpContext.Current.Server.MapPath("fbadmin.json"));
                    FirebaseApp.Create(new AppOptions
                    {
                        Credential = GoogleCredential.FromFile(pathToKey)
                    });
                    messaging = FirebaseMessaging.DefaultInstance;
                }

                List<Message> m = new List<Message>();

                foreach (var item in message)
                {
                    var alert = new Message()
                    {
                        Notification = new Notification
                        {
                            Title = item.title,
                            Body = item.body
                        },
                        Topic = "966591794404"//item.topic
                    };

                    m.Add(alert);
                }

                var response = await messaging.SendAllAsync(m);
                return response;
            }
            catch (Exception ex)
            {
                return ex;
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