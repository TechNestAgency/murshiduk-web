using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System.Globalization;
using System.Data.Entity;

namespace MurshadikCP.Controllers
{
    [Authorize]
    public class DashboardController : BaseController
    {

        // GET: Dashboard
        UserInfo u = new UserInfo();
        //mlaraEntities db = new mlaraEntities();

        // it contains cid = City ID
        // and check the data in weather data table and set the data according the city 
        public List<WeatherDataForDashboard> weatherList(string cid = "OERK")
        {
            List<WeatherDataForDashboard> list = new List<WeatherDataForDashboard>();
            var wde = db.WeatherDatas.Where(wd => wd.city_identifier == cid).OrderByDescending(x => x.id).Select(w => w.weather).FirstOrDefault();
            if (wde != null)
            {
                WeatherJson weatherJson = Newtonsoft.Json.JsonConvert.DeserializeObject<WeatherJson>(wde);

                WeatherDataForDashboard wd = new WeatherDataForDashboard();
                wd.Name = "اليوم";
                wd.icon = weatherJson.currentIcon == "" ? "wi-day-sunny" : weatherJson.currentIcon.Split(' ')[0];
                wd.Max = weatherJson.currentTemperature.Trim() + "°";

                list.Add(wd);

                wd = new WeatherDataForDashboard();
                string day = Convert.ToDateTime(weatherJson.firstDate).DayOfWeek.ToString();
                wd.Name = GetArabicDaysName(day);
                wd.icon = weatherJson.firstdayIcon.Split(' ')[0];
                wd.Max = weatherJson.firstdayMaxTemperature + "°";
                wd.Min = weatherJson.firstdayMinTemperature + "°";
                list.Add(wd);

                wd = new WeatherDataForDashboard();
                day = Convert.ToDateTime(weatherJson.secondDate).DayOfWeek.ToString();
                wd.Name = GetArabicDaysName(day);
                wd.icon = weatherJson.seconddayIcon.Split(' ')[0];
                wd.Max = weatherJson.seconddayMaxTemperature + "°";
                wd.Min = weatherJson.seconddayMinTemperature + "°";
                list.Add(wd);

                wd = new WeatherDataForDashboard();
                day = Convert.ToDateTime(weatherJson.thirdDate).DayOfWeek.ToString();
                wd.Name = GetArabicDaysName(day);
                wd.icon = weatherJson.thirddayIcon.Split(' ')[0];
                wd.Max = weatherJson.thirddayMaxTemperature + "°";
                wd.Min = weatherJson.thirddayMinTemperature + "°";
                list.Add(wd);

                wd = new WeatherDataForDashboard();
                day = Convert.ToDateTime(weatherJson.fourthDate).DayOfWeek.ToString();
                wd.Name = GetArabicDaysName(day);
                wd.icon = weatherJson.fourthdayIcon.Split(' ')[0];
                wd.Max = weatherJson.fourthdayMaxTemperature + "°";
                wd.Min = weatherJson.fourthdayMinTemperature + "°";
                list.Add(wd);

                wd = new WeatherDataForDashboard();
                day = Convert.ToDateTime(weatherJson.fifthDate).DayOfWeek.ToString();
                wd.Name = GetArabicDaysName(day);
                wd.icon = weatherJson.fifthdayIcon.Split(' ')[0];
                wd.Max = weatherJson.fifthdayMaxTemperature + "°";
                wd.Min = weatherJson.fifthdayMinTemperature + "°";
                list.Add(wd);

                //foreach (var item in w)
                //{
                //    WeatherDataForDashboard wd = new WeatherDataForDashboard();
                //    wd.Name = item.ToString();

                //}
                //JsonConvert.DeserializeObject<WeatherJson>(wde);
            }
            return list;
        }

        // pass the arabic name and return english name of days
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

        // get top 10 keywords from keywords table
        public ActionResult GetTop10Keywords()
        {
            List<string> qa = db.qa_questions.Select(p => p.keywords).ToList();
            if (qa.Count > 0)
            {
                db.sp_UpdateKeywordCountZero();
                foreach (var item in qa)
                {
                    string[] keywords = item.Split(',');
                    foreach (var kitem in keywords)
                    {
                        keyword k = db.keywords.Where(x => x.keyword1 == kitem).FirstOrDefault();
                        if (k != null)
                        {
                            k.keyword_count = k.keyword_count + 1;
                            db.Entry(k).State = System.Data.Entity.EntityState.Modified;
                        }
                    }
                }
                db.SaveChanges();
            }

            return View();
        }

        // insert skills in user table where role id = 6
        // role_id = 6 for consultant
        // we have one to many table for skills of the users { tbl name (skill_user) }
        public ActionResult InsertskillsForNewconsultants(int id)
        {
            return Content("");
            var allSkills = db.skills.ToList();
            var u = db.users.Where(ux => ux.device_token == "1" && ux.skills != null && ux.active == true).Select(ux => new { ux.id, ux.name, ux.phone, ux.skills }).ToList();
            List<Object> users = new List<object>();
            u.ForEach(uu =>
            {
                var userSkills = uu.skills.Split(',').ToList();

                List<Object> uSkillsWithIds = new List<object>();

                userSkills.ForEach(us => {
                    var thisSkill = allSkills.Where(ass => ass.name.Equals(us.Trim())).FirstOrDefault();
                    if (thisSkill != null)
                    {

                        skill_user skill_User = new skill_user();
                        skill_User.user_id = uu.id;
                        skill_User.skill_id = thisSkill.id;
                        skill_User.is_approved = true;
                        skill_User.created_at = DateTime.Now;
                        skill_User.updated_at = DateTime.Now;
                        db.skill_user.Add(skill_User);

                        uSkillsWithIds.Add(new { Skill = us, SkillID = thisSkill.id });
                    };
                });

                if (uSkillsWithIds.Any())
                {
                    users.Add(new { Id = uu.id, Name = uu.name, Phone = uu.phone, OldSkills = uu.skills, Skills = uSkillsWithIds.ToArray() });
                }

            }
            );

            db.SaveChanges();

            return Content(JsonConvert.SerializeObject(users));


            List<user> user = db.users.Where(x => x.device_token == "1" && x.id > id).ToList();
            foreach (var item in user)
            {
                string[] ski = item.skills.Split(',');
                if (ski != null)
                {
                    for (int i = 0; i < ski.Count(); i++)
                    {
                        string sk_user = ski[i].ToString();
                        skill sk = db.skills.Where(x => x.name == sk_user).FirstOrDefault();
                        if (sk != null)
                        {
                            skill_user skill_User = new skill_user();
                            skill_User.user_id = item.id;
                            skill_User.skill_id = sk.id;
                            skill_User.is_approved = true;
                            skill_User.created_at = DateTime.Now;
                            skill_User.updated_at = DateTime.Now;
                            db.skill_user.Add(skill_User);
                            db.SaveChanges();
                        }
                    }
                }
            }

            return RedirectToAction("Index");
        }

        // this is for our easyness so we generate appointments manually for using this method
        // that method is not using in code, this is just for our personal use only
        public ActionResult GenerateAppointments()
        {
            //just creating appointments testing purpose
            List<lab> listofLab = db.labs.Where(x => x.active == true).ToList();

            for (int i = 0; i < listofLab.Count(); i++)
            {
                for (int y = 0; y < 30; y++)
                {
                    DateTime dt = DateTime.Now.AddDays(y);
                    string[] days = listofLab[i].working_days.Split(',');
                    string NameofDay = "";
                    foreach (var item in days)
                    {
                        NameofDay = NameofDay + getEnglishDayName(item) + ",";
                    }

                    if (NameofDay.Contains(dt.DayOfWeek.ToString()))
                    {
                        db.GenerateAppointments(Convert.ToInt16(listofLab[i].id), dt);
                    }
                }
            }
            return RedirectToAction("Index");
        }

        // this method is also for our use only
        // in this method delete user directly and check first if child data exists then also delete first child records then delete user
        // we just pass user_id here
        public ActionResult DeleteUser(long id)
        {
            if (Session["User"] != null)
            {
                u = (UserInfo)Session["User"];
                if (u.RoleId == 1)
                {
                    db.DeleteUserByID(id);
                    return RedirectToAction("Index");
                }

            }

            return RedirectToAction("Login", "Account");

        }

        // same method is using for our personal use only
        // but in this method we pass user_id array so at a time we delete more than 1 user
        public ActionResult DeleteUserbyArray(string id)
        {
            if (Session["User"] != null)
            {
                u = (UserInfo)Session["User"];
                if (u.RoleId == 1)
                {
                    string[] ids = id.Split(',');
                    foreach (var item in ids)
                    {
                        long user_id = Convert.ToInt32(item);
                        db.DeleteUserByID(user_id);
                    }

                    return RedirectToAction("Index");
                }

            }

            return RedirectToAction("Login", "Account");

        }

        public ActionResult NotificationTest(string phoneNo)
        {
            Generic g = new Generic();
            //g.sendNotificationOnChats("سؤال", "تمت الموافقة على سؤالك!", _Questions.user.phone);
            var result = g.sendNotificationOnChats("مكالمة فائتة", "لديك مكالمة فائتة", phoneNo, "question_approved", "1");
            g.InsertNotificationData(112, "لديك مكالمة فائتة", (Int16)AppConstants.Type.Question, 10, "");
            return Content("Done");
        }

        // this method refers to Index View
        // you can check View folder on root and go to Dashboard then you can find Index.cshtml file
        // first we check the user in session after login
        // the check on role_id then redirect to the controllers.
        public ActionResult Index(string start_date = null, string end_date = null, int Region_id = 0)
        {

            if (currentPageID("Dashboard") > 0)
            {
                if (!CurrentUser.canView(currentPageID("Dashboard")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            start_date = start_date == string.Empty ? null : start_date;
            end_date = end_date == string.Empty ? null : end_date;


            if (CurrentUser.RoleId == (int)Role.LabManager)
            {
                return RedirectToAction("Index", "Appointments", new { @id = CurrentUser.labid });
            }
            else if (CurrentUser.RoleId == (int)Role.Market)
            {
                if (CurrentUser.multiplemarketids != null)
                {
                    return RedirectToAction("Index", "Markets");
                }
                else
                {
                    return RedirectToAction("View", "Markets", new { @id = CurrentUser.marketid });
                }

            }
            else if (CurrentUser.RoleId == (int)Role.WeatherAdmin)
            {
                return RedirectToAction("Index", "AlertMessages");
            }
            else if (CurrentUser.RoleId == (int)Role.RegionManager)
            {
                return RedirectToAction("Index", "Workerextension");
            }

            string strName = Contants.LAYOUT_VERTICAL;
            ViewBag.ModeName = strName;
           
            user user = db.users.Where(x => x.id == CurrentUser.Id).FirstOrDefault();
            ViewData["Avatar"] = user.avatar != null ? user.avatar : "avatar.png";
            ViewData["UserName"] = user.name;
            ViewData["Phone"] = user.phone;

            DateTime sdt = DateTime.Now.AddYears(-2); DateTime edt = DateTime.Now;

            TimeSpan daysToSub = new System.TimeSpan(6, 0, 0, 0);
            var startDate = DateTime.Today.Subtract(daysToSub).StartOfDay();
            var endDate = DateTime.Today.EndOfDay();
            var todaysStartDate = DateTime.Today.StartOfDay();
            var todaysEndDate = DateTime.Today.EndOfDay();


            if (CurrentUser.roles_Permissions.Exists(x => x.Page.PageName == "Dashboard"))
            {
                if (!string.IsNullOrEmpty(start_date))
                {
                    ViewBag.Start_Date = start_date;
                    sdt = DateTime.Parse(start_date, null, System.Globalization.DateTimeStyles.RoundtripKind);
                    startDate = sdt;
                }
                if (!string.IsNullOrEmpty(end_date))
                {
                    ViewBag.End_Date = end_date;
                    edt = DateTime.Parse(end_date, null, System.Globalization.DateTimeStyles.RoundtripKind);
                    endDate = edt;
                }

                ViewBag.TopMarket = db.markets.Where(x => x.is_active == true && x.created_at >= sdt && x.created_at <= edt).OrderByDescending(x => x.id).Take(5).ToList();
                ViewBag.TopQuestion = db.qa_questions.Where(x => x.is_approved == false && x.is_verified == false && x.created_at >= sdt && x.created_at <= edt).OrderByDescending(x => x.id).Take(10).ToList();
                ViewBag.TopAnswer = db.qa_answers.Where(x => x.is_approved == false && x.is_verified == false && x.created_at >= sdt && x.created_at <= edt).OrderByDescending(x => x.id).Take(10).ToList();
                ViewBag.TopLabReport = db.appointments.Where(x => x.is_completed == false && x.is_sample_collected == true && x.created_at >= sdt && x.created_at <= edt).OrderByDescending(x => x.id).Take(10).ToList();
                ViewBag.TopReport_Completed = db.appointments.Where(x => x.is_completed == true && x.created_at >= sdt && x.created_at <= edt).OrderByDescending(x => x.id).Take(10).ToList();
                ViewBag.TopConsultant = db.users.Where(x => x.role_id == (int)Role.Consultants && x.created_at >= sdt && x.created_at <= edt).Count();
                ViewBag.TopFarmers = db.users.Where(x => x.role_id == (int)Role.Farmers && x.created_at >= sdt && x.created_at <= edt).Count();
                ViewBag.msgs = db.chatmessages.Where(x => x.created_at >= sdt && x.created_at <= edt).Count();

                ViewBag.Top5Consultant = db.GetTop5ConsultantRating(Region_id).ToList();
                ViewBag.topRatingByCalling = db.GetTop5ConsultantCalling(Region_id).ToList();
                ViewBag.Region_id = Region_id;
                ViewData["Product"] = db.products.Where(x => x.created_at >= sdt && x.created_at <= edt).ToList().Count();
                ViewData["Market"] = db.markets.Where(x => x.created_at >= sdt && x.created_at <= edt).ToList().Count();
                ViewData["Article"] = db.articles.Where(x => x.created_at >= sdt && x.created_at <= edt).ToList().Count();
                ViewData["Lab"] = db.labs.Where(x => x.created_at >= sdt && x.created_at <= edt).ToList().Count();
                ViewData["Question"] = db.qa_questions.Where(x => x.created_at >= sdt && x.created_at <= edt).ToList().Count();
                ViewData["Answer"] = db.qa_answers.Where(x => x.created_at >= sdt && x.created_at <= edt).ToList().Count();
                ViewData["LabReport"] = db.lab_reports.Where(x => x.created_at >= sdt && x.created_at <= edt).ToList().Count();
                ViewBag.NotShow = false;
            }
            else
            {
                ViewBag.NotShow = true;
            }

            var messages = db.chatmessages
                            .Where(cm => cm.created_at >= startDate && cm.created_at <= endDate)
                            .GroupBy(cd => DbFunctions.TruncateTime(cd.created_at))
                            .Select(cd => new { Value = cd.Count(), Date = (DateTime)cd.Key }).ToArray();

            var calls = db.calldetails
                        .Where(cd => cd.status == 2 && cd.created_at >= startDate && cd.created_at <= endDate)
                        .GroupBy(cd => DbFunctions.TruncateTime(cd.created_at))
                        .Select(cd => new { Value = cd.Count(), Date = (DateTime)cd.Key }).ToArray();

            var Android = db.users
                         .Where(xs => xs.app_type == 2 && xs.created_at >= startDate && xs.created_at <= endDate)
                        .GroupBy(xs => DbFunctions.TruncateTime(xs.created_at))
                        .Select(xs => new { Value = xs.Count(), Date = (DateTime)xs.Key }).ToArray();

            var IOS = db.users
                         .Where(xs => xs.app_type == 1 && xs.created_at >= startDate && xs.created_at <= endDate)
                        .GroupBy(xs => DbFunctions.TruncateTime(xs.created_at))
                        .Select(xs => new { Value = xs.Count(), Date = (DateTime)xs.Key }).ToArray();

            var AllUsers = db.users
                            .Where(x => x.created_at >= startDate && x.created_at <= endDate)
                            .GroupBy(xs => DbFunctions.TruncateTime(xs.created_at))
                            .Select(xs => new { Value = xs.Count(), Date = (DateTime)xs.Key }).ToArray();

            //var todaysEndDate = endDate.AddDays(1);
            var callsTodayDTO = db.calldetails
                                .Where(cd => cd.status == 2 && cd.created_at >= todaysStartDate && cd.created_at < todaysEndDate)
                                .ToList();
            var callsToday = callsTodayDTO
                             .GroupBy(ctd => ctd.created_at.Hour)
                             .Select(ctd => new { Value = ctd.Count(), Key = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, ctd.Key, 0, 0).ChartTimeFormat() })
                             .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            //new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 0, 0).ToString("hh tt");
            var messagesTodayDto = db.chatmessages
                                .Where(cm => cm.created_at >= todaysStartDate && cm.created_at < todaysEndDate)
                                .ToList();
            var messagesToday = messagesTodayDto
                                 .GroupBy(cmt => cmt.created_at.Hour)
                                 .Select(ctd => new { Value = ctd.Count(), Key = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, ctd.Key, 0, 0).ChartTimeFormat() })
                                 .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            Dictionary<string, int> callChartsData = new Dictionary<string, int>();
            Dictionary<string, int> messagesChartsData = new Dictionary<string, int>();
            Dictionary<string, int> AndroidChartsData = new Dictionary<string, int>();
            Dictionary<string, int> IOSChartsData = new Dictionary<string, int>();
            Dictionary<string, int> AllUsersData = new Dictionary<string, int>();

            var dt = startDate;
            while (dt.StartOfDay().CompareTo(endDate.EndOfDay()) <= 0)
            {
                var callDetails = calls.Where(c => c.Date == dt.Date).FirstOrDefault();
                var messageDetails = messages.Where(m => m.Date == dt.Date).FirstOrDefault();
                var AndroidDetails = Android.Where(a => a.Date == dt.Date).FirstOrDefault();
                var IOSDetails = IOS.Where(a => a.Date == dt.Date).FirstOrDefault();
                var AllUserDetails = AllUsers.Where(a => a.Date == dt.Date).FirstOrDefault();

                callChartsData.Add(dt.Date.ToString("MMM-dd"), callDetails != null ? callDetails.Value : 0);
                messagesChartsData.Add(dt.Date.ToString("MMM-dd"), messageDetails != null ? messageDetails.Value : 0);

                AndroidChartsData.Add(dt.Date.ToString("MMM-dd"), AndroidDetails != null ? AndroidDetails.Value : 0);
                IOSChartsData.Add(dt.Date.ToString("MMM-dd"), IOSDetails != null ? IOSDetails.Value : 0);

                AllUsersData.Add(dt.Date.ToString("MMM-dd"), AllUserDetails != null ? AllUserDetails.Value : 0);

                dt = dt.AddDays(1);
            }

            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;
            ViewBag.callsDataForChart = callChartsData;
            ViewBag.messagesDataForChart = messagesChartsData;
            ViewBag.callsToday = callsToday;
            ViewBag.messagesToday = messagesToday;
            ViewBag.Android = AndroidChartsData;
            ViewBag.AllUsersData = AllUsersData;
            ViewBag.Iphone = IOSChartsData;
         
            List<region> regionddl = db.regions.Where(x => x.active == true).ToList();
            regionddl.Add(new region { id = 0, name_ar = Resources.Resources.All });
            ViewBag.Region = new SelectList(regionddl.OrderBy(x => x.id), "id", "name_ar");
            return View(CurrentUser.roles_Permissions);
        }

        // pass Day name in English and get Arabic name of the day
        public string GetArabicDaysName(string Name)
        {
            string value = "";
            switch (Name)
            {
                case "Monday":
                    value = "الإثنين";
                    break;
                case "Tuesday":
                    value = "الثلاثاء";
                    break;
                case "Wednesday":
                    value = "الأربعاء";
                    break;
                case "Thursday":
                    value = "الخميس";
                    break;
                case "Friday":
                    value = "الجمعة";
                    break;
                case "Saturday":
                    value = "السبت";
                    break;
                case "Sunday":
                    value = "الأحد";
                    break;
            }
            return value;
        }

        public class WeatherNotification
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string regionID { get; set; }
            public string Category { get; set; }
        }

        public class Repository
        {
            [JsonPropertyName("title")]
            public string title { get; set; }
        }

        // same for the above first we check here for weather service
        // here is another api weather service
        // so we check the data first here and then put this piece of code into seperate windows service
        // first we insert the data into our table then send the notification
        // before inserting the data first check the date and time is same then we didn't put data again.
        public async Task<CurrentWeatherNotificationJson> getcurrentWeatherDataAsync()
        {
            try
            {
                HttpClient clinet = new HttpClient();
                string weatherURL = System.Configuration.ConfigurationManager.AppSettings["WeatherAPIURL"] + "GetNotifications?alertHazards";
                var result = clinet.GetAsync(weatherURL);
                result.Wait();

                var result1 = result.Result;
                var test = result1.Content.ReadAsStringAsync();
                test.Wait();

                List<CurrentWeatherNotificationJson> weatherOfCities = JsonConvert.DeserializeObject<List<CurrentWeatherNotificationJson>>(test.Result);

                foreach (var wcity in weatherOfCities)
                {
                    currentweathernotificationforCity wd = new currentweathernotificationforCity();

                    wd.service_id = Convert.ToInt32(wcity.id);
                    wd.region_id = Convert.ToInt16(wcity.regiondID);
                    wd.alert_type_id = Convert.ToInt16(wcity.alertType);
                    wd.title = wcity.title;
                    wd.from_date = Convert.ToDateTime(wcity.fromdate);
                    wd.to_date = Convert.ToDateTime(wcity.todate);
                    wd.notification_at = Convert.ToDateTime(wcity.lastModified);
                    for (int i = 0; i < wcity.governorates.Count(); i++)
                    {
                        wd.city_id = Convert.ToInt16(wcity.governorates[i].id);
                        for (int y = 0; y < wcity.alertHazards.Count(); y++)
                        {
                            wd.alert_hazard_id = Convert.ToInt16(wcity.alertHazards[y].id);
                            AlertMessage am = db.AlertMessages.Where(x => x.city_id == wd.city_id && x.alertHazardId == wd.alert_hazard_id).FirstOrDefault();
                            if (am != null)
                            {
                                wd.message = am.message;
                                wd.link = am.link;
                                db.currentweathernotificationforCities.Add(wd);
                                db.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

            CurrentWeatherNotificationJson json = new CurrentWeatherNotificationJson();
            return json;
        }
    }

    public class WeatherDataForDashboard
    {
        public string Name { get; set; }
        public string Min { get; set; }
        public string Max { get; set; }
        public string icon { get; set; }

    }

}