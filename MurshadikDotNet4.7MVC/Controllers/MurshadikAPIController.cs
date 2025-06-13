using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using MurshadikCP.Extensions;
using System.Threading.Tasks;
using System.Data.Entity.Migrations;
using Google.Api.Gax.ResourceNames;
using System.Web.Routing;
using Microsoft.Ajax.Utilities;

namespace MurshadikCP.Controllers
{

    // this controllers only for mobile application
    // API data get, post and delete methods
    // based on authentication JWT\
    
    [RoutePrefix("api/Murshadik")]
    public class MurshadikAPIController : ApiController
    {
        private mlaraEntities db = new mlaraEntities();
		

        private string SendOTP(string phoneNo)
        {
            string[] phoneNumbers = System.Configuration.ConfigurationManager.AppSettings["testingPhoneNumbers"].Split(',');
            if (phoneNumbers.Contains(phoneNo))
            {
                return "3658";
            }
            
            Random generator = new Random();
            String otp = generator.Next(0, 9999).ToString("D4");
            string msg = "فضلا استخدام رمز التفعيل " + otp + " لتسجيل الدخول في تطبيق مرشدك الزراعي";

            SendSMS sms = new SendSMS();
            sms.SMSSend(phoneNo, msg);

            //SendSMSYamamah smsy = new SendSMSYamamah();
            //smsy.sendSMSByYamamahAPI(phoneNo, msg);

            return otp;
        }
        [HttpGet]
        [Route("MarketProduct")]
        public Data MarketProduct()
        {
			Data respons = new Data();
			var Markets = db.markets.Select(s => new
            {
               Id= s.id,
               Name = s.marketname
            });
            var Products = db.products.Select(s => new
            {
                Id = s.id,
                Name = s.product_name
            });
            respons.data = new { Markets, Products };
            respons.status = true;
            respons.message = "Success";


			return respons;
        }

        //remove Account
        [Authorize]
        [HttpGet]
        [Route("removeAccount")]
        public Data removeAccount()
		{
            Data result = new Data();
            var identity = User.Identity as ClaimsIdentity;
            string user_id = identity.Claims.FirstOrDefault(x => x.Type.Equals("userid")).Value;
            if (!string.IsNullOrEmpty(user_id))
			{
                var UserId = long.Parse(user_id);
                var user= this.db.users.Find(UserId);
				var worker = this.db.Workers.Where(w=>w.UserId == UserId).FirstOrDefault();

                user.phone = "_" + user.phone;
                this.db.Entry(user).State = EntityState.Modified;
                this.db.SaveChanges();
              
                if (worker != null)
                { 
                   worker.IsDeleted = true;
					this.db.Entry(worker).State = EntityState.Modified;
					this.db.SaveChanges();
				}

				result.status = true;
                result.data = true;
                result.message = "Success";
                return result;
			}
			else
			{
                result.status = false;
                result.message = "Error";
                result.data = false;
            return result; 
			}

		}

        [Authorize]
        [HttpPost]
        [Route("changePhoneno")]
        public Data changePhoneno(string newPhoneNo)
        {
            Data d = new Data();
            var identity = User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity.Claims;
            string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;
            long uid = Convert.ToInt32(user_id);
            var user = db.users.Where(x => x.phone == newPhoneNo).FirstOrDefault();
            if (user != null)
            {
                d.status = false;
                d.message = "already exists this number";
                d.data = null;
            }
            else
            {
                string otpStr = this.SendOTP(newPhoneNo);

                otp otp = db.otps.Where(x => x.user_id == uid).FirstOrDefault();

                bool isNew = false;
                if (otp == null)
                {
                    otp = new otp();
                    isNew = true;
                }

                otp.user_id = uid;
                otp.created_at = DateTime.Now;
                otp.otp1 = otpStr;

                otp.attempts = otp != null ? otp.attempts + 1 : 1;
                if (!isNew)
                { db.Entry(otp).State = EntityState.Modified; }
                else { db.otps.Add(otp); }
                db.SaveChanges();

                d.status = true;
                d.data = "otp successfully sent! ";
                d.message = "Otp send successfully... Attempts No :" + otp.attempts.ToString();
            }

            return d;
        }

        [Authorize]
        [HttpPost]
        [Route("updatePhoneNoCheckingOTP")]
        public Data updatePhoneNoCheckingOTP(string newPhoneNo, string OTP)
        {
            Data d = new Data();
            var identity = User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity.Claims;
            string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;
            long uid = Convert.ToInt32(user_id);
            otp otp = db.otps.Where(x => x.user_id == uid).FirstOrDefault();
            if (otp != null)
            {
                if (otp.otp1 == OTP)
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    var user = db.users.Where(x => x.id == uid).FirstOrDefault();
                    if (user != null)
                    {
                        user.phone = newPhoneNo;
                        db.Entry(user).State = EntityState.Modified;
                        db.SaveChanges();
                        d.status = true;
                        d.message = "successfully updated phone no";
                        d.data = new { user = new { id = user.id,
                            name = user.name, last_name = user.last_name, fullname = user.name + " " + user.last_name, phone = user.phone, role_id = user.role_id,
                            last_login_date = user.last_login_date, app_type = user.app_type, app_ver = user.app_ver, phone_verified_at = user.phone_verified_at,
                            password = user.password, avatar = user.avatar, active = user.active, country = user.country, region = user.region, governorate = user.governorate,
                            can_post = user.can_post, is_group_admin = user.is_group_admin, chatid = user.chatId, linkedin = user.linked_in, is_online = user.is_online,
                            is_profile_completed = user.is_profile_completed, created_at = user.created_at, updated_at = user.updated_at, is_weather_notification_enabled = user.is_weather_notification_enabled,
                            is_market_notification_enabled = user.is_market_notification_enabled
                        }, skills = getUserSkills(user.id) };

                        // after checking otp then remove otp
                        db.otps.Remove(otp);
                        db.SaveChanges();

                    }

                }
                else
                {
                    d.status = false;
                    d.message = "user enter wrong OTP";
                }
            }
            else
            {
                d.status = false;
                d.message = "OTP is not exists!";
            }

            return d;
        }

        // Resend OTP to end user if he/she want
        [HttpGet]
        [Route("ResendOTP")]
        public Data ResendOTP(string username)
        {
            Data d = new Data();
            
            if (username == "" || username == null)
            {
                d.message = "phone number is not in proper format!";
                d.status = false;
                d.data = "otp failed";
                return d;
            }

            user u = db.users.Where(x => x.phone == username).FirstOrDefault();

            if (u != null)
            {
                string otpStr = this.SendOTP(username);

                otp otp = db.otps.Where(x => x.user_id == u.id).FirstOrDefault();

                bool isNew = false;
                if(otp == null)
                {
                    otp = new otp();
                    isNew = true;
                }

                otp.user_id = u.id;
                otp.created_at = DateTime.Now;
                otp.otp1 = otpStr;

                otp.attempts = otp != null ? otp.attempts + 1 : 1;
                if (!isNew)
                { db.Entry(otp).State = EntityState.Modified; }
                else { db.otps.Add(otp); }
                db.SaveChanges();

                d.status = true;
                d.data = "otp successfully sent! ";
                d.message = "Otp send successfully... Attempts No :" + otp.attempts.ToString();
            }
            else
            {
                d.status = false;
                d.data = "user not found";
                d.message = "user not exists";
                
            }

            return d;
        }

        // for our purpose just adding the consultant from csv file
        //[HttpPost]
        //[Route("CreateCustomUsers")]
        //public bool CreateCustomUsers()
        //{
        //    string name = HttpContext.Current.Request.Form["name"];
        //    string last_name = HttpContext.Current.Request.Form["lastname"];
        //    string phone = HttpContext.Current.Request.Form["phone"];
        //    int role_id = 6;
        //    int app_type = Convert.ToInt16(HttpContext.Current.Request.Form["app_type"]);
        //    int region_id = Convert.ToInt16(HttpContext.Current.Request.Form["region_id"]);
        //    string region = GetRegionName(Convert.ToInt16(HttpContext.Current.Request.Form["region_id"]));
        //    string country = "المملكة العربية السعودية";
        //    string skills = HttpContext.Current.Request.Form["skills"];
        //    string profile = HttpContext.Current.Request.Form["profile"];
        //    decimal rating = Convert.ToDecimal(HttpContext.Current.Request.Form["rating"]);
        //    bool active = Convert.ToBoolean(HttpContext.Current.Request.Form["active"]);
        //    string avatar = HttpContext.Current.Request.Form["avatar"];
        //    string city_code = "OERK";

        //    user u = db.users.Where(x => x.phone == phone).FirstOrDefault();
        //    if (u == null)
        //    {
        //        bool a = RegisterUser_Custom(phone, country, role_id, last_name, app_type, region_id, profile, skills, phone, rating, active, avatar, name);
        //    }

        //    return true;
        //}

        // check login method check username, country, role_id
        // if user is not exists then create user first then send OTP
        [HttpGet]
        [Route("CheckLogin")]
        public Data CheckLogin(string username, string country, long role_id, int app_type = 1)
        {
            Data d = new Data();

            if (username == null || username == "")
            {
                d.message = "phone number is not in proper format!";
                d.status = false;
                d.data = "otp failed";
                return d;
            }

            user u = db.users.Where(x => x.phone == username).FirstOrDefault();

            if (u != null)
            {
                d.message = "Successfully Done! User exists Generate Token";
            }
            else
            {
                u = RegisterUser(username, country, role_id, app_type);

                if (u != null)
                {
                    d.message = "Successfully Done! User not exists, create user first and Generate Token partially!";
                }
                else
                {
                    d.message = "Error!";
                    d.status = false;
                    d.data = null;
                    return d;
                }
            }

            string otp = this.SendOTP(username);

            otp checkOtp = db.otps.Where(x => x.user_id == u.id).FirstOrDefault();
            if (checkOtp == null)
            {
                checkOtp = new otp();
                checkOtp.user_id = u.id;
                checkOtp.created_at = DateTime.Now;
                checkOtp.otp1 = otp;
                checkOtp.attempts = 1;
                db.otps.Add(checkOtp);
            }
            else
            {
                checkOtp.created_at = DateTime.Now;
                checkOtp.otp1 = otp;
                checkOtp.attempts = 1;
                db.Entry(checkOtp).State = EntityState.Modified;
            }

            db.SaveChanges();

            d.status = true;
            d.data = "otp successfully sent!";
            return d;
        }

        // on or off enable notification for end user
        [Authorize]
        [HttpPost]
        [Route("EnableNotificationForType")]
        public Data EnableNotificationForType()
        {
            Data d = new Data();
            try
            {
                var identity = User.Identity as ClaimsIdentity;
                IEnumerable<Claim> claims = identity.Claims;
                string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;
                long uid = Convert.ToInt32(user_id);
                int type_id = Convert.ToInt32(HttpContext.Current.Request.Form["type_id"].ToString());
                bool is_enabled = Convert.ToBoolean(HttpContext.Current.Request.Form["is_enabled"].ToString());
                user u = db.users.Find(uid);
                if (type_id == 1)
                {
                    u.is_market_notification_enabled = is_enabled;
                }
                else
                {
                    u.is_weather_notification_enabled = is_enabled;
                }
                db.Entry(u).State = EntityState.Modified;
                db.SaveChanges();
                d.status = true;
                d.message = "success";
            }
            catch(Exception ex)
            {
                d.status = false;
                d.message = ex.Message.ToString();
            }
            return d;
        }

        //  check OTP from mobile app
        [HttpPost]
        [Route("CheckOTPNew")]
        public Data CheckOTPNew(string UserName, string OTP)
        {
            Data d = new Data();
            db.Configuration.ProxyCreationEnabled = false;
            user u = db.users.Where(x => x.phone == UserName).FirstOrDefault();
            if (u == null)
            {
                d.message = "User not found!";
                d.status = false;
                d.data = null;
                return d;
            }

            if (u.linked_in == "1")
            {
                if (OTP == "3658")
                {
                    d.message = GetToken(UserName).ToString();//"Successfully Done! User exists Generate Token";
                    d.status = true;
                    d.data = new { user = u, skills = getUserSkills(u.id) };
                }
                else
                {
                    d.message = "Invalid OTP!";
                    d.status = false;
                    d.data = null;
                }

            }
            else
            {

                otp otp = db.otps.Where(x => x.user_id == u.id).FirstOrDefault();
                string saveOTP = otp.otp1.ToString();
                if (saveOTP == OTP)
                {
                    d.message = GetToken(UserName).ToString();//"Successfully Done! User exists Generate Token";
                    d.status = true;
                    d.data = new { user = u, skills = getUserSkills(u.id) };


                    // after removing otp when successfully done
                    db.otps.Remove(otp);
                    db.SaveChanges();
                }
                else
                {
                    d.message = "Invalid OTP!";
                    d.status = false;
                    d.data = null;
                }
            }

            return d;
        }

        [HttpPost]
        [Route("CheckOTP")]
        public Data CheckOTP(string UserName, string OTP)
        {
            Data d = new Data();
            db.Configuration.ProxyCreationEnabled = false;
            user u = db.users.Where(x => x.phone == UserName).FirstOrDefault();
            if (u == null)
            {
                d.message = "User not found!";
                d.status = false;
                d.data = null;
                return d;
            }

            otp otp = db.otps.Where(x => x.user_id == u.id).FirstOrDefault();
            string saveOTP = otp != null ? otp.otp1.ToString() : "";

            if (u.linked_in == "1")
            {
                if (OTP == "3658")
                {
                    d.message = GetToken(UserName).ToString();//"Successfully Done! User exists Generate Token";
                    d.status = true;
                    d.data = new { user = u, skills = getUserSkills(u.id) };
                }
                else
                {
                    d.message = "Invalid OTP!";
                    d.status = false;
                    d.data = null;
                }

            }
            else
            {

                if (saveOTP == OTP)
                {
                    d.message = GetToken(UserName).ToString();//"Successfully Done! User exists Generate Token";
                    d.status = true;
                    user u1 = new user();
                    u1.id = u.id;
                    u1.name = u.name;
                    u1.phone = u.phone;
                    u1.role_id = u.role_id;
                    u1.user_id = u.user_id;
                    u1.app_type = u.app_type;
                    u1.region_id = u.region_id;
                    u1.avatar = u.avatar;
                    u1.firebase_token = u.firebase_token;

                    u1.active = u.active;
                    u1.country = u.country;
                    u1.region = u.region;
                    u1.skills = u.role_id == 6 ? GetSkillsByConsultantForProfile(u.id) : u.skills;
                    //u1.skill_user = u.role_id == 6 ? db.skill_user.Where(x => x.user_id == u.id).ToList() : null;
                    u1.governorate = u.governorate;
                    u1.municipality = u.municipality;
                    u1.linked_in = u.linked_in;
                    u1.last_name = u.last_name;
                    u1.city_code = u.city_code;
                    u1.is_group_admin = u.is_group_admin;
                    u1.can_post = u.can_post;
                    u1.is_profile_completed = u.is_profile_completed;
                    u1.is_approved = u.is_approved;
                    u1.is_market_notification_enabled = u.is_market_notification_enabled;
                    u1.is_weather_notification_enabled = u.is_weather_notification_enabled;

                    d.data = u;

                    // after removing otp when successfully done
                    db.otps.Remove(otp);
                    db.SaveChanges();
                }
                else
                {
                    d.message = "Invalid OTP!";
                    d.status = false;
                    d.data = null;
                }
            }
            return d;
        }

        // based on user id it return users skills
        public object getUserSkills(long id)
        {
            //var id = 1;
            var query =
               from su in db.skill_user
               join s in db.skills on su.skill_id equals s.id
               where su.user_id == id && s.active == true
               select new { s.id, s.name, su.is_approved };
            return query.ToList();
        }

        // based on user id it returns consultant skills for profile
        public string GetSkillsByConsultantForProfile(long id)
        {

            string Allskills = "";
            var skills = db.skill_user.Where(x => x.user_id == id).Include(x => x.skill).ToList().Select(p => p.skill.name);

            //var skills = db.fun_GetConsultantSkills(id).ToList();
            if (skills.Count() > 0)
            {
                Allskills = string.Join(",", skills);
            }
            return Allskills;
        }

        // based on user id it return consultant skills
        public string GetSkillsByConsultant(long id)
        {
            string Allskills = "";
            var skills = db.fun_GetConsultantSkills(id).ToList();
            if (skills.Count() > 0)
            {
                Allskills = string.Join(",", skills);
            }
            return Allskills;
        }

        // delete all notification based on user id and notification id
        [Authorize]
        [HttpDelete]
        [Route("DeleteAllNotification_byId")]
        public Data DeleteAllNotification(long id)
        {
            Data d = new Data();
            var identity = User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity.Claims;
            string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;
            long uid = Convert.ToInt32(user_id);

            notification n = db.notifications.Where(x => x.user_id == uid && x.id == id).FirstOrDefault();
            db.notifications.Remove(n);
            db.SaveChanges();

            d.status = true;
            d.message = "Delete All Notification by id Successfully!";
            d.data = null;
            return d;
        }

        // delete all notification based on user id
        [Authorize]
        [HttpDelete]
        [Route("DeleteAllNotification")]
        public Data DeleteAllNotification()
        {
            Data d = new Data();
            var identity = User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity.Claims;
            string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;
            long uid = Convert.ToInt32(user_id);

            List<notification> n = db.notifications.Where(x => x.user_id == uid).ToList();
            db.notifications.RemoveRange(n);
            db.SaveChanges();

            d.status = true;
            d.message = "Delete All Notification Successfully!";
            d.data = null;
            return d;
        }

        // based on datetime return all notification according to the user id
        [Authorize]
        [HttpGet]
        [Route("GetAllNotification")]
        public List<notification> GetAllNotification(string dateTime)
        {
            var identity = User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity.Claims;
            string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;
            long uid = Convert.ToInt32(user_id);           
            DateTime dt = Convert.ToDateTime(dateTime);
            return db.notifications.Where(x => x.user_id == uid && x.created_at > dt).OrderByDescending(x => x.created_at).ToList();
            
           
        }

        // chat data based on chatid according to the user
        [Authorize]
        [HttpGet]
        [Route("getUserByChatId")]
        public object getUserByChatId(long id)
        {
            string chatid = id.ToString();
            user u = db.users.Where(x => x.chatId == chatid).FirstOrDefault();
            if (u.role_id == 6)
            {
                u.skills = GetSkillsByConsultant(id);
            }

            var user = new
            {
                id = u.id,
                name = u.name + " " + u.last_name,
                phone = u.phone,
                region = db.regions.Where(x => x.id == u.region_id && x.active == true).FirstOrDefault().name_ar,
                skills = u.skills,
                profile = u.profile,
                avatar = u.avatar,
                chatId = u.chatId,
                is_online = u.is_online,
                rating = u.rating,
                role_id = u.role_id,
                country = u.country
            };

            if (user != null)
            {
                return Ok(user);
            }

            return NotFound();
        }

        // return user details by userid
        [Authorize]
        [HttpGet]
        [Route("getUserBy/{id}")]
        public object getUserByID(long id)
        {
            user u = db.users.Where(x => x.id == id).FirstOrDefault();
            if (u.role_id == 6)
            {
                u.skills = GetSkillsByConsultant(id);
            }

            var user = new
            {
                id = u.id,
                name = u.name + " " + u.last_name,
                phone = u.phone,
                region = db.regions.Where(x => x.id == u.region_id && x.active == true).FirstOrDefault().name_ar,
                skills = u.skills,
                profile = u.profile,
                avatar = u.avatar,
                chatId = u.chatId,
                is_online = u.is_online,
                rating = u.rating,
                role_id = u.role_id,
                country = u.country
            };

            if (user != null)
            {
                return Ok(user);
            }

            return NotFound();

        }

        // check username and return Token which is generated by JWT
        public string GetToken(string UserName)
        {
            string key = "my_secret_key_1231321";
            var issuer = "http://murshadik.com";
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            user user = db.users.Where(x => x.phone == UserName).Include(x => x.role).FirstOrDefault();
            if (user != null)
            {
                var paramClaims = new List<Claim>();
                paramClaims.Add(new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                paramClaims.Add(new Claim("valid", user.active.ToString()));
                paramClaims.Add(new Claim("userid", user.id.ToString()));
                paramClaims.Add(new Claim("fullname", user.name + " " + user.last_name));
                paramClaims.Add(new Claim("name", user.name));
                paramClaims.Add(new Claim("last_name", user.last_name == null ? "" : user.last_name));
                paramClaims.Add(new Claim("roleid", user.role_id.ToString()));
                paramClaims.Add(new Claim("rolename", user.role.name_en));
                paramClaims.Add(new Claim("is_profile_completed", user.is_profile_completed.ToString()));

                var token = new JwtSecurityToken(issuer,
                    issuer,
                    paramClaims,
                    expires: DateTime.Now.AddYears(1),
                    signingCredentials: credentials);

                var jwt_token = new JwtSecurityTokenHandler().WriteToken(token);

                return jwt_token.ToString();//data = jwt_token, isNew = user.is_profile_completed };
            }
            return "user not exists in our system!";
        }


        // based on lab_id and datetime and return the appointment data
        [Authorize]
        [HttpGet]
        [Route("GetAllAppointmentbyLabID")]
        public object GetAllAppointmentbyLabID(long lab_id, string date)
        {
            Array a = new Array[0];

            if (User.Identity.IsAuthenticated)
            {

                DateTime dt = Convert.ToDateTime(date);
                DateTime todayDate = DateTime.Now;
                if (dt.Date == todayDate.Date)
                {
                    if (DateTime.Now.TimeOfDay.Hours > 20)
                    {
                        return a;
                    }
                    else
                    {
                        DateTime time1 = DateTime.Now.AddHours(1);
                        var appointment = db.appointments.Where(x => x.lab_id == lab_id && x.appointment_date == dt && x.appointment_time > time1.TimeOfDay)
                        .Select(p => new AppointmentDTO
                        {
                            dt = p.appointment_time.Value,
                            is_booked = p.is_booked,
                            id = p.id
                        }).ToList();

                        return appointment;
                    }
                }
                else if (dt.Date > todayDate)
                {
                    var appointment = db.appointments.Where(x => x.lab_id == lab_id && x.appointment_date == dt)
                    .Select(p => new AppointmentDTO
                    {
                        dt = p.appointment_time.Value,
                        is_booked = p.is_booked,
                        id = p.id
                    }).ToList();

                    return appointment;
                }
            }

            return a;
        }

        // showing the details on application after open the application
        // its contains images and statistics data from tables
        // to just show the dashboard
        [HttpGet]
        [Route("New_Dashboard")]
        public object getDashboardData()
        {
            long[] sectionCategoriesArticles = { 14, 33 };
            long[] sectionCategories = { 1, 30 };

            var articles = db.categories
                                        .Where(cat => sectionCategoriesArticles.Contains(cat.id) && cat.active == true)
                                        .Select(cat => new
                                        {
                                            id = cat.id,
                                            title = cat.name,
                                            banner = cat.banner,
                                            icon = cat.icon,
                                            articles = cat.articles.Where(x => x.active == true)
                                                .Select(art => new Article()
                                                {
                                                    id = art.id,
                                                    title_ar = art.title,
                                                    desc_ar = art.short_description,
                                                    image = art.image,
                                                    created_at = art.created_at
                                                })
                                                .Take(10)
                                                .ToList()
                                        })
                                        .ToList();

            var categories = db.categories.Where(cat => cat.active == true && (sectionCategories
                                                        .Contains(cat.parent_category.Value) || sectionCategories.Contains(cat.id))
                                                )
                                                .Select(cat => new Category()
                                                {
                                                    id = cat.id,
                                                    name = cat.name,
                                                    description = cat.description,
                                                    parent_category = cat.parent_category,
                                                    icon = cat.icon,
                                                    bg_color = cat.bg_color,
                                                    text_color = cat.text_color,
                                                    banner = cat.banner,
                                                    active = cat.active,
                                                    created_at = cat.created_at,
                                                    updated_at = cat.updated_at,
                                                }
                                                )
                                                .ToList();
            
            string consultantCount = CustomFuntions.FormatNumber(db.users.Where(x => x.role_id == (int)Role.Consultants).Count());
            string farmerCount = CustomFuntions.FormatNumber(db.users.Where(x => x.role_id == (int)Role.Farmers).Count());
            string Risala = CustomFuntions.FormatNumber((db.chatmessages.Count() + db.qa_questions.Count() + db.qa_answers.Count()));

            List<StatsDTO> stats = db.app_stats.Where(x => x.enabled).Take(3).Select(p => new StatsDTO()
            {
                title = p.stats,//p.stats.Length > 3 ? p.stats.Substring(0,2)+"K" : p.stats,
                total = p.id == 1 ? db.msgs.Count() : 
                p.id == 2 ? db.users.Where(x => x.role_id == 5).Count() : 
                p.id == 3 ? db.users.Where(x => x.role_id == 7).Count() :
                p.id == 4 ? 10 :
                p.id == 5 ? db.msgs.Count() : 
                p.id == 6 ? db.qa_questions.Count() :
                p.id == 7 ? db.qa_answers.Count() :
                p.id == 8 ? db.lab_reports.Count() :
                p.id == 9 ? db.labs.Count() :
                p.id == 10 ? db.articles.Count() :
                p.id == 11 ? db.markets.Count() :
                p.id == 12 ? db.products.Count() :
                597,
                icon = p.icon,
                desc = p.title,
                bgColor = p.bg_color
            }).ToList();
            
            stats[0].title = consultantCount;
            stats[1].title = farmerCount;
            stats[2].title = Risala;

            var firstSection = new
            {
                ParentCat = categories.Where(cat => cat.id == sectionCategories.First()).Select(c => new
                {
                    id = c.id,
                    title = c.name,
                    banner = c.banner,
                    icon = c.icon
                }).First(),
                Categories = categories.Where(cat => cat.parent_category == sectionCategories.First()),
            };

            var secondSection = new
            {
                ParentCat = categories.Where(cat => cat.id == sectionCategories.Last()).Select(c => new
                {
                    id = c.id,
                    title = c.name,
                    banner = c.banner,
                    icon = c.icon
                }
                                            ).First(),
                Categories = categories.Where(cat => cat.parent_category == sectionCategories.Last()),
            };

            var dashboardData = new
            {
                liveTransmission = new LiveStream()
                {
                    type = "livestream",
                    title = db.app_settings.Where(x => x.ap_key == AppSettings.AP_STREAM_TITLE).FirstOrDefault().ap_value,
                    enabled = "on",
                    banner = db.app_settings.Where(x => x.ap_key == AppSettings.AP_STREAM_BANNER).FirstOrDefault().ap_value,
                    url = db.app_settings.Where(x => x.ap_key == AppSettings.AP_STREAM_URL).FirstOrDefault().ap_value
                    
                },
                adB = new LiveStream() 
                {
                    type = db.app_settings.Where(x => x.ap_key == AppSettings.AP_AD_CONTENT).FirstOrDefault().ap_value,
                    title = db.app_settings.Where(x => x.ap_key == AppSettings.AP_AD_TITLE).FirstOrDefault().ap_value,
                    enabled = "on",
                    banner = db.app_settings.Where(x => x.ap_key == AppSettings.AP_AD_ICON).FirstOrDefault().ap_value,
                    url = db.app_settings.Where(x => x.ap_key == AppSettings.AP_AD_LINK).FirstOrDefault().ap_value
                },
                sectionOne = new
                {
                    id = firstSection.ParentCat.id,
                    title = firstSection.ParentCat.title,
                    banner = firstSection.ParentCat.banner,
                    icon = firstSection.ParentCat.icon,
                    categories = firstSection.Categories
                },
                agCalendarCatID = 13,
                agGuideCatID = 12,
                sectionTwo = articles[0],
                sectionThree = articles[1],
                mediaLibraryCatID = 15,
                sectionFour = new
                {
                    id = secondSection.ParentCat.id,
                    title = secondSection.ParentCat.title,
                    banner = secondSection.ParentCat.banner,
                    icon = secondSection.ParentCat.icon,
                    categories = secondSection.Categories
                },
                contactUsSection = new 
                {
                    AP_SYSTEM_EMAIL = db.app_settings.Where(x => x.ap_key == "AP_SYSTEM_EMAIL").FirstOrDefault().ap_value,
                    AP_CUSTOMER_SUPPORT_WA_NUMBER = db.app_settings.Where(x => x.ap_key == "AP_CUSTOMER_SUPPORT_WA_NUMBER").FirstOrDefault().ap_value,
                    AP_CUSTOMER_SUPPORT_NUMBER = db.app_settings.Where(x => x.ap_key == "AP_CUSTOMER_SUPPORT_NUMBER").FirstOrDefault().ap_value,
                    AP_FACEBOOK_URL = db.app_settings.Where(x => x.ap_key == "AP_FACEBOOK_URL").FirstOrDefault().ap_value,
                    AP_TWITTER_URL = db.app_settings.Where(x => x.ap_key == "AP_TWITTER_URL").FirstOrDefault().ap_value,
                    AP_YOUTUBE_URL = db.app_settings.Where(x => x.ap_key == "AP_YOUTUBE_URL").FirstOrDefault().ap_value,
                    AP_INSTA_URL = db.app_settings.Where(x => x.ap_key == "AP_INSTA_URL").FirstOrDefault().ap_value,
                    AP_SNAP_URL = db.app_settings.Where(x => x.ap_key == "AP_SNAP_URL").FirstOrDefault().ap_value
                },
                stats = stats
            };

            //if (dashboardData.stats.Count() > 0)
            //{
                
            //}

            return dashboardData;
        }

        static string FormatNumber(string num1)
        {
            int num = Convert.ToInt16(num1);

            if (num >= 100000)
            { 
                return num / 1000 + "K"; 
            }
            if (num >= 10000)
            {
                return (num / 1000D).ToString("0.#") + "K";
            }
            return num.ToString("#,0");
        }

        public int CheckCountBytitle(long stats_id)
        {
            int count = 0;
            if (stats_id == 1)
            {
                count = db.msgs.Count();
            }
            else if (stats_id == 2)
            {
                count = db.users.Where(x => x.role_id == 5).Count();
            }
            else if (stats_id == 3)
            {
                count = db.users.Where(x => x.role_id == 7).Count();
            }
            else if (stats_id == 4)
            {
                count = 10;
            }
            else if (stats_id == 5)
            {
                count = db.msgs.Count();
            }
            else if (stats_id == 6)
            {
                count = db.qa_questions.Count();
            }
            else if (stats_id == 7)
            {
                count = db.qa_answers.Count();
            }
            else if (stats_id == 8)
            {
                count = db.lab_reports.Count();
            }
            else if (stats_id == 9)
            {
                count = db.labs.Count();
            }
            else if (stats_id == 10)
            {
                count = db.articles.Count();
            }
            else if (stats_id == 11)
            {
                count = db.markets.Count();
            }
            else if (stats_id == 12)
            {
                count = db.products.Count();
            }
            return count;
        }

        // get_message return list of messages from chat id
        [Authorize]
        [HttpGet]
        [Route("Get_Message")]
        public List<chatmessage> Get_Message(string receiver_id)
        {
            List<chatmessage> cm = db.chatmessages.Where(x => x.chat_ids.Contains(receiver_id)).OrderByDescending(x => x.id).ToList();

            return cm;
        }

        // return all chats based on user id
        [Authorize]
        [HttpGet]
        [Route("GetAllChats")]
        public object GetAllChats()
        {
            var identity = User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity.Claims;
            string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;
            List<chat> chats = db.chats.Where(x => x.chat_ids.Contains(user_id)).ToList();
            List<ChatDetails> cd_list = new List<ChatDetails>();
            if (chats.Count() > 0)
            {
                for (int i = 0; i < chats.Count(); i++)
                {
                    ChatDetails cd = new ChatDetails();
                    cd.last_message = chats[i].last_message;
                    cd.status = chats[i].status.Value;
                    cd.created_at = chats[i].created_at.Value;
                    cd.message_type = chats[i].message_type;
                    string[] ids = chats[i].chat_ids.Split(',');
                    long claim_id = Convert.ToInt16(user_id);
                    if (user_id != ids[0])
                    {
                        Int32 u_id = Convert.ToInt32(ids[0]);
                        user u = db.users.Where(x => x.id == u_id).FirstOrDefault();
                        if (u != null)
                        {
                            cd.avatar = u.avatar;
                            cd.is_online = u.is_online;
                            cd.name = u.name + " " + u.last_name;
                            cd.phone = u.phone;
                            cd.rating = u.rating;
                            cd.active = u.active;
                            cd.chatId = u.chatId;
                            cd.id = u.id;
                            cd.role_id = u.role_id;
                            cd_list.Add(cd);
                        }
                    }
                    else
                    {
                        Int32 u_id = Convert.ToInt32(ids[1]);
                        user u = db.users.Where(x => x.id == u_id).FirstOrDefault();
                        if (u != null)
                        {
                            cd.avatar = u.avatar;
                            cd.is_online = u.is_online;
                            cd.name = u.name + " " + u.last_name;
                            cd.phone = u.phone;
                            cd.rating = u.rating;
                            cd.active = u.active;
                            cd.chatId = u.chatId;
                            cd.id = u.id;
                            cd.role_id = u.role_id;
                            cd_list.Add(cd);
                        }
                    }

                }
            }
            return cd_list;
        }

        // update status based on status type and receiver id
        [Authorize]
        [HttpGet]
        [Route("Update_Status")]
        public Data Update_Status(string status_type, string receiver_id)
        {
            Data d = new Data();
            var identity = User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity.Claims;
            string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;

            string message = "";
            chatmessage cm = db.chatmessages.Where(x => x.chat_ids.Contains(receiver_id)).LastOrDefault();
            if (cm != null)
            {
                message = cm.message;
            }

            d.status = true;
            d.message = message;
            d.data = null;

            return d;
        }

        // return group messages
        [Authorize]
        [HttpGet]
        [Route("getgroupmessages")]
        public List<group_messages> getGroupMessages(int id = 0, string dt = null)
        {
            List<group_messages> gm = new List<group_messages>();
            if (id > 0)
            {
                gm = db.group_messages.Where(x => x.id > id).ToList();
            }
            if (dt != null && dt != "")
            {
                DateTime date = Convert.ToDateTime(dt);
                gm = db.group_messages.Where(x => x.created_at > date).OrderByDescending(x => x.id).Take(20).ToList();
            }
            return gm;
        }

        // return group messages
        [Authorize]
        [HttpGet]
        [Route("getGroupMessagesBy1")]
        public Data getGroupMessagesBy1(int group_id, int? Page_No = 0, int id = 0, int page_size = 20)
        {
            Data d = new Data();

            var skip = Page_No == 1 ? 0 : page_size * (Page_No - 1);
            var query = db.group_messages.Where(group => group.group_id == group_id);

            var results = query.OrderByDescending(message => message.id)
                   .Select(m => new
                   {
                       Messages = m,
                       TotalCount = query.Count()
                   })
                   .Skip(skip.Value).Take(page_size)
                   .ToList();

            var totalCount = results.First().TotalCount;
            var pages = Math.Ceiling(totalCount / (double)page_size);
            var messages = results.Select(r => r.Messages).ToList();

            // if CurrentPage is greater than 1 means it has previousPage  
            var previous_Page = Page_No > 1 ? true : false;
            // if TotalPages is greater than CurrentPage means it has nextPage  
            var next_Page = Page_No < pages ? true : false;

            d.status = true;
            d.message = "successfully fetch the records";
            d.data = results;
            d.page_info = new { total_count = totalCount, current_page = Page_No, total_page = pages, next_Page, previous_Page };
            return d;

        }

        // return group messages
        // pagination also there
        [Authorize]
        [HttpGet]
        [Route("getGroupMessagesBy_V2")]
        public Data getGroupMessagesBy(int group_id, int? Page_No = 0, int id = 0, int page_size = 20, bool withMembers = false)
        {
            Data d = new Data();
            List<group_messages> gm = new List<group_messages>();

            int pagesize = page_size;

            int No_Of_Page = Page_No ?? 1;
            if (Page_No == 0 || Page_No == null) { Page_No = 1; }

            if (group_id > 0)
            {
                gm = db.group_messages.Where(x => x.group_id == group_id).OrderByDescending(x => x.id).Skip((No_Of_Page - 1) * pagesize).Take(pagesize).ToList();
            }

            int gm_count = db.group_messages.Where(x => x.group_id == group_id).Count();
            int TotalPages = (int)Math.Ceiling(gm_count / (double)pagesize);

            // if CurrentPage is greater than 1 means it has previousPage  
            var previous_Page = Page_No > 1 ? true : false;
            // if TotalPages is greater than CurrentPage means it has nextPage  
            var next_Page = Page_No < TotalPages ? true : false;

            if (withMembers)
            {
                db.Configuration.ProxyCreationEnabled = false;
                List<user> users = db.users.Where(x => x.role_id == (int)Role.Consultants && x.is_approved == true && x.region_id == group_id).ToList();
                d.status = true;
                d.message = "successfully fetch the records";
                d.data = new { group_message = gm, consultant = users.Select(p => new { p.id, p.name, p.phone, p.avatar, p.chatId, p.is_group_admin, p.can_post, p.city_code }) };
                d.page_info = new { total_count = gm_count, current_page = Page_No, total_page = TotalPages, next_Page, previous_Page };
                return d;
            }

            d.status = true;
            d.message = "successfully fetch the records";
            d.data = new { group_message = gm, consultant = new string[0] };
            d.page_info = new { total_count = gm_count, current_page = Page_No, total_page = TotalPages, next_Page, previous_Page };
            return d;
        }

        [Authorize]
        [HttpGet]
        [Route("getGroupMessagesBy")]
        public Data getGroupMessagesBy(int group_id, int? Page_No = 0, int id = 0, int page_size = 20)
        {
            Data d = new Data();
            List<group_messages> gm = new List<group_messages>();
            
            int pagesize = page_size;

            int No_Of_Page = (Page_No ?? 1);
            if (Page_No == 0 || Page_No == null) { Page_No = 1; }
            
            if (group_id > 0)
            {
                gm = db.group_messages.Where(x => x.group_id == group_id).OrderByDescending(x => x.id).Skip((No_Of_Page - 1) * pagesize).Take(pagesize).ToList();
            }

            int gm_count = db.group_messages.Where(x => x.group_id == group_id).Count();
            int TotalPages = (int)Math.Ceiling(gm_count / (double)pagesize);

            // if CurrentPage is greater than 1 means it has previousPage  
            var previous_Page = Page_No > 1 ? true : false;
            // if TotalPages is greater than CurrentPage means it has nextPage  
            var next_Page = Page_No < TotalPages ? true : false;

            d.status = true;
            d.message = "successfully fetch the records";
            d.data = gm;
            d.page_info = new { total_count = gm_count, current_page = Page_No, total_page = TotalPages, next_Page, previous_Page };
            return d;

        }

        // insert group messages takes message, and type_id (image, file, audio etc)
        [Authorize]
        [HttpPost]
        [Route("groupmessage")]
        public group_messages group_Message()
        {
            Data d = new Data();
            var identity = User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity.Claims;
            string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;
            string fullname = claims.Where(p => p.Type == "fullname").FirstOrDefault()?.Value;
            long u_id = Convert.ToInt32(user_id);
            long region_id = db.users.Where(x => x.id == u_id).FirstOrDefault().region_id;
            int group_id = Convert.ToInt32(region_id);
            int type_id = Convert.ToInt32(HttpContext.Current.Request.Form["type_id"].ToString());
            string message = HttpContext.Current.Request.Form["message"];
            //int group_id = Convert.ToInt32(HttpContext.Current.Request.Form["group_id"].ToString());

            group_messages gm = new group_messages();
            gm.message_type = type_id;
            if (type_id != 1)
            {
                var httpContext = HttpContext.Current;
                HttpPostedFile httpPostedFile = httpContext.Request.Files[0];
                if (httpPostedFile != null)
                {
                    Guid guid = Guid.NewGuid();
                    var file = guid.ToString() + Path.GetExtension(httpPostedFile.FileName);
                    var path = Path.Combine(HostingEnvironment.MapPath("~/Media/Chat/"), file);
                    // Save the uploaded file  
                    httpPostedFile.SaveAs(path);
                    gm.message = "/Media/Chat/" + file;
                }
            }
            else
            {
                gm.message = message;
            }

            gm.message_from = Convert.ToInt16(user_id);
            gm.group_id = group_id;
            gm.created_at = DateTime.Now;
            db.group_messages.Add(gm);
            db.SaveChanges();

            string group_name = db.regions.Where(x => x.id == gm.group_id && x.active == true).FirstOrDefault().name_ar;

            Generic g = new Generic();
            string messsageStr = gm.message.Length > 255 ? gm.message.Substring(0, gm.message.Substring(0, 255).LastIndexOf(" ")) : gm.message;
            //g.sendNotificationOnChats(fullname, gm.message.Length > 20 ? gm.message.Substring(0, 20) : gm.message, group_name, "group_message", gm.group_id.ToString());
            g.sendNotificationOnChats(fullname, messsageStr, "group-" + group_id.ToString(), "group_message", group_id.ToString());
            return gm;
        }

        
        
        
        // return agriculture calender data
        [HttpGet]
        [Route("getallagriculture_calender")]
        public Data getallagriculture_calender()
        {
            Data d = new Data();
            d.data = db.Agriculture_Calender.Where(x => x.active == true).ToList();
            d.message = "successfully fetch records";
            d.status = true;
            return d;
        }

        // insert message data into table
        [Authorize]
        [HttpPost]
        [Route("message")]
        public object Save_Message()
        {
            Data d = new Data();
            var identity = User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity.Claims;
            string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;
            long _uid = Convert.ToInt64(user_id);
            string User_name = db.users.Where(x => x.id == _uid).FirstOrDefault().name;
            string fullname = claims.Where(p => p.Type == "fullname").FirstOrDefault()?.Value;
            int type_id = Convert.ToInt32(HttpContext.Current.Request.Form["type_id"].ToString());
            string message = HttpContext.Current.Request.Form["message"];
            string receiver_id = HttpContext.Current.Request.Form["receiver_id"].ToString();
            int status = Convert.ToInt16(HttpContext.Current.Request.Form["status"].ToString());

            Int64[] ids = new Int64[2];
            ids[0] = Convert.ToInt64(user_id);
            ids[1] = Convert.ToInt64(receiver_id);
            Array.Sort(ids);
            string chatIDs = ids[0] + "," + ids[1];

            string phoneno = "";
            long receiver_ID = Convert.ToInt64(receiver_id);
            phoneno = db.users.Where(x => x.id == receiver_ID).FirstOrDefault().phone;
            db.Configuration.ProxyCreationEnabled = false;
            chatmessage cm = new chatmessage();
            cm.message_type = type_id;
            if (type_id != 1 && type_id != 6)
            {
                var httpContext = HttpContext.Current;
                HttpPostedFile httpPostedFile = httpContext.Request.Files[0];
                if (httpPostedFile != null)
                {
                    Guid guid = Guid.NewGuid();
                    var file = guid.ToString() + Path.GetExtension(httpPostedFile.FileName);
                    var path = Path.Combine(HostingEnvironment.MapPath("~/Media/Chat/"), file);
                    // Save the uploaded file 
                    httpPostedFile.SaveAs(path);
                    cm.message = "/Media/Chat/" + file;
                }
            }
            else
            {
                cm.message = message;
            }

            cm.chat_ids = chatIDs;
            cm.status = status;
            cm.message_from = Convert.ToInt32(user_id);
            cm.message_to = Convert.ToInt32(receiver_ID);
            cm.created_at = DateTime.Now;
            db.chatmessages.Add(cm);
            db.SaveChanges();

            // for check chat table
            chat chat = new chat();
            chat = db.chats.Where(x => x.chat_ids == cm.chat_ids).FirstOrDefault();
            if (chat == null)
            {
                chat = new chat();
                chat.chat_ids = cm.chat_ids;
                chat.created_at = DateTime.Now;
                if (cm.message_type != 1)
                {
                    chat.last_message = null;
                }
                else
                {
                    chat.last_message = cm.message;
                }
                chat.status = cm.status;
                chat.message_type = cm.message_type;
                db.chats.Add(chat);
                db.SaveChanges();
            }
            else
            {
                chat.last_message = cm.message;
                chat.created_at = DateTime.Now;
                chat.message_type = cm.message_type;
                db.Entry(chat).State = EntityState.Modified;
                db.SaveChanges();
            }

            Generic g = new Generic();
            //g.sendNotificationOnChats(fullname, cm.message.Length > 20 ? cm.message.Substring(0, 20) : cm.message, phoneno);

            //if (type_id == 1)
            //{
            //    g.InsertNotificationData(receiver_ID, "new message from : " + User_name + " - " + message, 7, cm.id, "");
            //}
            //else
            //{
            //    string messaget_type = db.messagetypes.Where(x => x.id == chat.message_type).FirstOrDefault().name;
            //    g.InsertNotificationData(receiver_ID, "new message from : " + User_name + " - " + messaget_type, 7, cm.id, "");
            //}

            
            object mes = new { cm.id, cm.message, cm.message_from, cm.message_type, cm.status, cm.chat_ids, cm.created_at };

            ////cm.messagetype = new messagetype();
            //cm.messagetype.chatmessages = null;
            //cm.messagetype.chats = null;
            return mes;
        }

        // baesd on user id show the user profile information
        [Authorize]
        [HttpGet]
        [Route("GetUserProfileNew")]
        public Data GetUserProfileNew()
        {
            Data d = new Data();
            db.Configuration.ProxyCreationEnabled = false;
            user u = new user();
            if (User.Identity.IsAuthenticated)
            {
                var identity = User.Identity as ClaimsIdentity;
                IEnumerable<Claim> claims = identity.Claims;
                string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;
                long uid = Convert.ToInt32(user_id);
                u = db.users.Find(uid);
                u.is_approved = u.is_approved;
                //if (u.role_id == 6)
                //{
                //    u.skills = GetSkillsByConsultantForProfile(u.id);//GetSkillsByConsultant(u.id);
                //}
                
                d.status = true;
                d.message = "successfully fetch the data";
                d.data = new { user = u, skills = getUserSkills(u.id) };

                return d;
            }
            else
            {
                d.status = false;
                d.message = "User is not authorized";
                return d;
            }
        }

        // baesd on user id show the user profile information
        [Authorize]
        [HttpGet]
        [Route("GetUserProfile")]
        public user GetUserProfile()
        {
            db.Configuration.ProxyCreationEnabled = false;
            user u = new user();
            if (User.Identity.IsAuthenticated)
            {
                var identity = User.Identity as ClaimsIdentity;
                IEnumerable<Claim> claims = identity.Claims;
                string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;
                long uid = Convert.ToInt32(user_id);
                u = db.users.Find(uid);
                u.is_approved = u.is_approved;
                if (u.role_id == 6)
                {
                    u.skills = GetSkillsByConsultantForProfile(u.id);//GetSkillsByConsultant(u.id);
                }

                u.last_login_date = DateTime.Now;
                u.updated_at = DateTime.Now;
                db.Entry(u).State = EntityState.Modified;
                db.SaveChanges();

                return u;
            }
            else
            {
                return u;
            }
        }

        // insert comment to consultant and rating also
        [Authorize]
        [HttpPost]
        [Route("CommentByUser")]
        public Data CommentByUser(long consultantid, float rating, string message)
        {
            Data d = new Data();
            if (User.Identity.IsAuthenticated)
            {
                var identity = User.Identity as ClaimsIdentity;
                IEnumerable<Claim> claims = identity.Claims;
                string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;
                long uid = Convert.ToInt32(user_id);
                commentsByUser cbu = new commentsByUser();
                cbu.consultantId = consultantid;
                cbu.created_at = DateTime.Now;
                cbu.farmerId = uid;
                cbu.is_active = true;
                cbu.message = message;
                cbu.Rating = rating;
                cbu.updated_at = DateTime.Now;
                db.commentsByUsers.Add(cbu);
                db.SaveChanges();

                Generic g = new Generic();
                user u = db.users.Find(uid);
                g.sendNotificationOnPriceUpdate("تقييم", "تم تقييمك من قبل :" + u.name, u.phone);

                g.InsertNotificationData(consultantid, "تم تقييمك من قبل :" + g.GetFullName(uid), (Int16)AppConstants.Type.Rating, cbu.id, "");

                d.status = true;
                d.message = "successful";
                return d;
            }
            d.status = false;
            d.message = "unsuccessful";
            return d;
        }

        // list of market product categories
        //[Authorize]
        [HttpGet]
        [Route("GetMarketProductCategories")]
        public object GetMarketProductCategories()
        {
            var product_categories = db.product_categories
                       .Select(p => new { p.id, p.name });
            return product_categories;
        }

        // book for an appointment
        [Authorize]
        [HttpPost]
        [Route("AppointmentBook")]
        public Data AppointmentBook(long appointment_id)
        {
            Data d = new Data();
            if (User.Identity.IsAuthenticated)
            {
                var identity = User.Identity as ClaimsIdentity;
                IEnumerable<Claim> claims = identity.Claims;
                string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;
                appointment appointment = db.appointments.Where(x => x.id == appointment_id).FirstOrDefault();
                if (appointment != null)
                {
                    if (appointment.is_booked == false)
                    {
                        appointment.user_id = Convert.ToInt32(user_id);
                        appointment.is_booked = true;
                        appointment.created_at = DateTime.Now;
                        db.Entry(appointment).State = EntityState.Modified;
                        db.SaveChanges();

                        //InsertNotificationData(Convert.ToInt32(user_id), "Appointment has been booked", Convert.ToInt16(AppConstants.Type.Lab), appointment_id, "");

                        d.status = true;
                        d.message = "Successfully Created Appointment!";
                        d.data = "Your Appointment ID - : " + appointment_id.ToString();
                        return d;
                    }
                    else
                    {
                        d.status = false;
                        d.message = "appointment is already exists!";
                        d.data = "Error";
                        return d;
                    }
                }
                else
                {
                    d.status = false;
                    d.message = "appointment is not exists!";
                    d.data = "Error";
                    return d;
                }
            }
            d.status = false;
            d.message = "User is not authenticated";
            d.data = "Error";
            return d;
        }


        // get lab data
        [HttpGet]
        [Route("GetLabData")]
        public Object GetLabData()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var regions = db.regions.Where(x => x.active == true).Select(p => new { p.id, p.name_ar, p.name }).ToList();
            var labs = db.labs.Where(x => x.active == true)
                .Include(x => x.city)
                .Select(p => new
                {
                    p.id,
                    p.Name,
                    p.city
                })
                .ToList();

            return new { regions, labs };
        }

        [HttpGet]
        [Route("GetLab")]
        public Object GetLab()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var labs = db.labs.Where(x => x.active == true)
                .Include(x => x.city)
                .Select(p => new
                {
                    p.id,
                    p.Name,
                    p.city
                })
                .ToList();

            return labs;

        }

        // after login the JWT create token and claims read from here
        // so avoiding the database again and again to take info regarding the user
        [Authorize]
        [HttpPost]
        [Route("GetClaims")]
        public Object GetClaims()
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                return new
                {
                    name = claims.Where(p => p.Type == "name").FirstOrDefault()?.Value,
                    fullname = claims.Where(p => p.Type == "fullname").FirstOrDefault()?.Value,
                    last_name = claims.Where(p => p.Type == "last_name").FirstOrDefault()?.Value,
                    roleName = claims.Where(p => p.Type == "rolename").FirstOrDefault()?.Value,
                    roleid = claims.Where(p => p.Type == "roleid").FirstOrDefault()?.Value,
                    valid = claims.Where(p => p.Type == "valid").FirstOrDefault()?.Value,
                    userid = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value
                };

            }

            return null;
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public bool RegisterUser_Custom(string username, string country, long role_id, string last_name, int app_type, long region_id, string profile, string skills, string phone, decimal rating, bool active, string avatar,string name)
        {
            try
            {
                user user1 = new user();
                user1.user_id = Guid.NewGuid().ToString();
                user1.password = RandomString(15);
                user1.phone = username;
                user1.last_name = last_name;
                user1.app_type = app_type;
                user1.role_id = role_id;
                user1.name = name;
                user1.active = active;
                user1.region_id = region_id;
                user1.city_code = "OERK";
                user1.country = country;
                user1.rating = rating;
                user1.device_token = "1";
                user1.created_at = DateTime.Now;
                user1.profile = profile;
                user1.skills = skills;
                user1.is_market_notification_enabled = true;
                user1.is_weather_notification_enabled = true;
                user1.avatar = avatar;
                mlaraEntities db = new mlaraEntities();
                db.users.Add(user1);
                db.SaveChanges();
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("RegisterUser")]
        public user RegisterUser(string userName, string country, long role_id, int app_type)
        {
            try
            {
                user newUser = new user();
                newUser.user_id = Guid.NewGuid().ToString();
                newUser.password = RandomString(15);
                newUser.phone = userName;
                newUser.last_name = "";
                newUser.app_type = app_type;
                newUser.role_id = role_id;
                newUser.name = userName;
                newUser.active = true;
                newUser.region_id = 8;
                newUser.city_code = "OERK";
                newUser.country = country;
                newUser.rating = 0;
                newUser.is_approved = false;
                newUser.created_at = DateTime.Now;
                newUser.is_market_notification_enabled = true;
                newUser.is_weather_notification_enabled = true;
                newUser.last_login_date = DateTime.Now;
                mlaraEntities db = new mlaraEntities();
                db.users.Add(newUser);
                db.SaveChanges();
                return newUser;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool RegisterUser_ForLabNewUser(string userName, string country, long role_id, string mobile)
        {
            try
            {
                string UserName = mobile.CheckUser();
                if (UserName != "" && UserName.Length == 12)
                {
                    //userName = UserName;
                    user user1 = new user();
                    user1.user_id = Guid.NewGuid().ToString();
                    user1.password = RandomString(15);
                    user1.phone = mobile;
                    user1.last_name = "";
                    user1.app_type = 1;
                    user1.role_id = role_id;
                    user1.name = userName;
                    user1.active = true;
                    user1.region_id = 8;
                    user1.city_code = "OERK";
                    user1.country = country;
                    user1.rating = 0;
                    user1.created_at = DateTime.Now;
                    user1.updated_at = DateTime.Now;
                    user1.last_login_date = DateTime.Now;
                    mlaraEntities db = new mlaraEntities();
                    db.users.Add(user1);
                    db.SaveChanges();
                    return true;
                }

                return false;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool RegisterUser(string userName, string country, long role_id, string mobile)
        {
            try
            {
                string UserName = userName.CheckUser();
                if (UserName != "" && UserName.Length == 12)
                {
                    userName = UserName;
                    user user1 = new user();
                    user1.user_id = Guid.NewGuid().ToString();
                    user1.password = RandomString(15);
                    user1.phone = mobile;
                    user1.last_name = "";
                    user1.app_type = 1;
                    user1.role_id = role_id;
                    user1.name = userName;
                    user1.active = true;
                    user1.region_id = 8;
                    user1.city_code = "OERK";
                    user1.country = country;
                    user1.rating = 0;
                    user1.created_at = DateTime.Now;
                    mlaraEntities db = new mlaraEntities();
                    db.users.Add(user1);
                    db.SaveChanges();
                    return true;
                }

                return false;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string GetRegionName(int region_id)
        {
            string regionname = "";
            long rid = Convert.ToInt32(region_id);
            var region = db.regions.Where(x => x.id == rid && x.active == true).FirstOrDefault();
            if (region != null)
            {
                regionname = region.name_ar;
            }

            return regionname;
        }

        // after creating user then update profile
        [Authorize]
        [HttpPost]
        [Route("UpdateProfileNew")]
        public Data RegisterNew()
        {
            int role_id = 0;
            Data d = new Data();
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {

                IEnumerable<Claim> claims = identity.Claims;
                string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;
                long uid = 0;
                if (user_id != null)
                {
                    uid = Convert.ToInt32(user_id);
                }
                else
                {
                    d.status = false;
                    d.message = "User is not exists!";
                    d.data = null;
                    return d;
                }
                db.Configuration.ProxyCreationEnabled = false;
                user user = db.users.Where(x => x.id == uid).FirstOrDefault();
                if (user != null)
                {
                    user.name = HttpContext.Current.Request.Form["name"];
                    user.last_name = HttpContext.Current.Request.Form["lastname"];
                    user.phone = HttpContext.Current.Request.Form["phone"];
                    role_id = Convert.ToInt16(HttpContext.Current.Request.Form["role_id"]);
                    user.role_id = role_id;
                    user.app_type = Convert.ToInt16(HttpContext.Current.Request.Form["app_type"]);
                    user.region_id = Convert.ToInt16(HttpContext.Current.Request.Form["region_id"]);
                    user.region = GetRegionName(Convert.ToInt16(HttpContext.Current.Request.Form["region_id"]));
                    user.country = HttpContext.Current.Request.Form["country"];
                    user.governorate = HttpContext.Current.Request.Form["governorate"];
                    user.municipality = HttpContext.Current.Request.Form["municipality"];
                    user.skills = HttpContext.Current.Request.Form["skills"];
                    //user.linked_in = HttpContext.Current.Request.Form["linked_in"];
                    user.prefix = HttpContext.Current.Request.Form["prefix"];
                    user.profile = HttpContext.Current.Request.Form["profile"];
                    user.is_profile_completed = true;
                    if (role_id == (int)Role.Farmers)
                    { user.is_approved = true; }
                    user.city_code = HttpContext.Current.Request.Form["city_code"];
                    if (HttpContext.Current.Request.Form["city_id"] != null && HttpContext.Current.Request.Form["city_id"] != "")
                        user.governorate = HttpContext.Current.Request.Form["city_id"];

                    var httpContext = HttpContext.Current;

                    // Check for any uploaded file  
                    if (httpContext.Request.Files.Count > 0)
                    {
                        //Loop through uploaded files  
                        for (int i = 0; i < httpContext.Request.Files.Count; i++)
                        {
                            HttpPostedFile httpPostedFile = httpContext.Request.Files[i];
                            if (httpPostedFile != null)
                            {
                                Guid guid = Guid.NewGuid();
                                var img = guid.ToString() + Path.GetExtension(httpPostedFile.FileName);
                                var path = Path.Combine(HostingEnvironment.MapPath("~/Media/Images/Avatar/"), img);
                                // Save the uploaded file  
                                httpPostedFile.SaveAs(path);
                                user.avatar = "/Media/Images/Avatar/" + img;
                            }
                        }
                    }


                    db.Entry(user).State = EntityState.Modified;
                    //db.SaveChanges();

                    if (role_id == 6)
                    {
                        if (HttpContext.Current.Request.Form["skill_ids"] != null)
                        {
                            long[] skill_id = HttpContext.Current.Request.Form["skill_ids"].Split(',').Select(p => Convert.ToInt64(p)).ToArray();
                            long[] oldskill_id = db.skill_user.Where(x => x.user_id == uid).ToList().Select(x => x.skill_id).ToArray();
                            long[] listCartisan = skill_id.Except(oldskill_id).ToArray();
                            foreach (var item in listCartisan)
                            {
                                skill_user su = new skill_user();
                                su.user_id = uid;
                                su.skill_id = item;
                                su.is_approved = false;
                                su.created_at = DateTime.Now;
                                db.skill_user.Add(su);
                            }
                        }
                    }

                    db.SaveChanges();

                    d.status = true;
                    d.message = GetToken(user.phone);
                    user.role = null;
                    d.data = user;//GetToken(user.phone);
                    return d;
                }
                else
                {
                    d.status = false;
                    d.message = "User not exists";
                    d.data = null;
                    return d;
                }
            }

            d.status = false;
            d.message = "something wrong contact to technical person";
            return d;
        }

        // after creating user then update profile
        [Authorize]
        [HttpPost]
        [Route("UpdateProfileIOS")]
        public Data RegisterNew_V2()
        {
            int role_id = 0;
            Data d = new Data();
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {

                IEnumerable<Claim> claims = identity.Claims;
                string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;
                long uid = 0;
                if (user_id != null)
                {
                    uid = Convert.ToInt32(user_id);
                }
                else
                {
                    d.status = false;
                    d.message = "User is not exists!";
                    d.data = null;
                    return d;
                }
                db.Configuration.ProxyCreationEnabled = false;
                user user = db.users.Where(x => x.id == uid).FirstOrDefault();
                if (user != null)
                {
                    user.name = HttpContext.Current.Request.Form["name"];
                    user.last_name = HttpContext.Current.Request.Form["lastname"];
                    user.phone = HttpContext.Current.Request.Form["phone"];
                    role_id = Convert.ToInt16(HttpContext.Current.Request.Form["role_id"]);
                    user.role_id = role_id;
                    user.app_type = Convert.ToInt16(HttpContext.Current.Request.Form["app_type"]);
                    user.region_id = Convert.ToInt16(HttpContext.Current.Request.Form["region_id"]);
                    user.region = GetRegionName(Convert.ToInt16(HttpContext.Current.Request.Form["region_id"]));
                    user.country = HttpContext.Current.Request.Form["country"];
                    user.governorate = HttpContext.Current.Request.Form["governorate"];
                    user.municipality = HttpContext.Current.Request.Form["municipality"];
                    user.skills = HttpContext.Current.Request.Form["skills"];
                    //user.linked_in = HttpContext.Current.Request.Form["linked_in"];
                    user.prefix = HttpContext.Current.Request.Form["prefix"];
                    user.profile = HttpContext.Current.Request.Form["profile"];
                    user.is_profile_completed = true;
                    if (role_id == 5)
                    { user.is_approved = true; }
                    user.city_code = HttpContext.Current.Request.Form["city_code"];
                    if (HttpContext.Current.Request.Form["city_id"] != null && HttpContext.Current.Request.Form["city_id"] != "")
                        user.governorate =  HttpContext.Current.Request.Form["city_id"];

                    var httpContext = HttpContext.Current;

                    // Check for any uploaded file  
                    if (httpContext.Request.Files.Count > 0)
                    {
                        //Loop through uploaded files  
                        for (int i = 0; i < httpContext.Request.Files.Count; i++)
                        {
                            HttpPostedFile httpPostedFile = httpContext.Request.Files[i];
                            if (httpPostedFile != null)
                            {
                                Guid guid = Guid.NewGuid();
                                var img = guid.ToString() + Path.GetExtension(httpPostedFile.FileName);
                                var path = Path.Combine(HostingEnvironment.MapPath("~/Media/Images/Avatar/"), img);
                                // Save the uploaded file  
                                httpPostedFile.SaveAs(path);
                                user.avatar = "/Media/Images/Avatar/" + img;
                            }
                        }
                    }


                    db.Entry(user).State = EntityState.Modified;
                    //db.SaveChanges();

                    if (role_id == 6)
                    {
                        if (HttpContext.Current.Request.Form["skill_ids"] != null)
                        {
                            long[] skill_id = HttpContext.Current.Request.Form["skill_ids"].Split(',').Select(p => Convert.ToInt64(p)).ToArray();
                            long[] oldskill_id = db.skill_user.Where(x => x.user_id == uid).ToList().Select(x => x.skill_id).ToArray();
                            long[] listCartisan = skill_id.Except(oldskill_id).ToArray();
                            foreach (var item in listCartisan)
                            {
                                skill_user su = new skill_user();
                                su.user_id = uid;
                                su.skill_id = item;
                                su.is_approved = false;
                                su.created_at = DateTime.Now;
                                db.skill_user.Add(su);
                            }
                        }
                    }

                    db.SaveChanges();

                    d.status = true;
                    d.message = GetToken(user.phone);
                    user.role = null;
                    user.skill_user = null;
                    user.region = null;
                    d.data = new { user = user, skills = getUserSkills(uid) };
                    return d;
                }
                else
                {
                    d.status = false;
                    d.message = "User not exists";
                    d.data = null;
                    return d;
                }
            }

            d.status = false;
            d.message = "something wrong contact to technical person";
            return d;
        }

        [Authorize]
        [HttpPost]
        [Route("updatecallstatus")]
        public Data updatecallstatus()
        {
            Data d = new Data();
            try
            {
                var identity = User.Identity as ClaimsIdentity;
                IEnumerable<Claim> claims = identity.Claims;
                string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;
                long uid = Convert.ToInt32(user_id);
                var claim_user_chatid = db.users.Find(uid).chatId;
                int status = Convert.ToInt16(HttpContext.Current.Request.Form["status"].ToString());
                int type = Convert.ToInt16(HttpContext.Current.Request.Form["type"].ToString());
                Int32 to_user = Convert.ToInt32(HttpContext.Current.Request.Form["to_user"].ToString());
                string call_id = HttpContext.Current.Request.Form["call_id"].ToString();
                calldetail cd = new calldetail();
                cd.status = status;
                cd.type = type;
                cd.user_to = to_user;
                cd.user_from = uid;
                cd.call_id = call_id;
                cd.created_at = DateTime.Now;
                db.calldetails.Add(cd);
                db.SaveChanges();
                d.status = true;
                d.message = "successfully";
                d.data = cd.id;
                string calltype = type == 1 ? "صوتية" : "فيديو";
                if (status == 3 || status == 4 || status == 5)
                {
                    Generic g = new Generic();
                    MurshadikCP.Models.DB.user user = db.users.Find(to_user);
                    string fromname = claims.Where(p => p.Type == "fullname").FirstOrDefault()?.Value;
                    //g.sendNotificationOnChats("سؤال", "تمت الموافقة على سؤالك!", _Questions.user.phone);
                    var result = g.sendNotificationOnChats("مكالمة فائتة", "🚫 مكالمة  " + calltype+ " فائتة من " + fromname +"!", user.phone, "question_approved", "1", claim_user_chatid.ToString());
                    g.InsertNotificationData(to_user, "🚫 مكالمة  " + calltype + " فائتة من " + fromname + "!", (Int16)AppConstants.Type.Chat, 10, claim_user_chatid.ToString());
                }
                return d;
            }
            catch
            {
                d.status = false;
                d.message = "failed";
                d.data = null;
                return d;
            }
        }

        // after creating user then update profile
        [Authorize]
        [HttpPost]
        [Route("UpdateProfile")]
        public Data Register(string name, string lastname, string phone, int role_id, int app_type, int region_id, string country = null, string governorate = null,
            string municipality = null, string skills = null, string linked_in = null, string profile = null, string prefix = null, string city_code = null, string skill_ids = null)
        {
            Data d = new Data();
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {

                IEnumerable<Claim> claims = identity.Claims;
                string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;
                long uid = 0;
                if (user_id != null)
                {
                    uid = Convert.ToInt32(user_id);
                }
                else
                {
                    d.status = false;
                    d.message = "User is not exists!";
                    d.data = null;
                    return d;
                }
                db.Configuration.ProxyCreationEnabled = false;
                user user = db.users.Where(x => x.id == uid).FirstOrDefault();
                if (user != null)
                {
                    user.name = name;
                    user.last_name = lastname;
                    user.phone = phone;
                    user.role_id = role_id;
                    user.app_type = app_type;
                    user.region_id = region_id;
                    user.region = GetRegionName(region_id);
                    user.country = country;
                    user.governorate = governorate;
                    user.municipality = municipality;
                    user.skills = skills;
                    //user.linked_in = linked_in;
                    user.prefix = prefix;
                    user.profile = profile;
                    user.is_profile_completed = true;
                    user.city_code = city_code;

                    var httpContext = HttpContext.Current;

                    // Check for any uploaded file  
                    if (httpContext.Request.Files.Count > 0)
                    {
                        //Loop through uploaded files  
                        for (int i = 0; i < httpContext.Request.Files.Count; i++)
                        {
                            HttpPostedFile httpPostedFile = httpContext.Request.Files[i];
                            if (httpPostedFile != null)
                            {
                                Guid guid = Guid.NewGuid();
                                var img = guid.ToString() + Path.GetExtension(httpPostedFile.FileName);
                                var path = Path.Combine(HostingEnvironment.MapPath("~/Media/Images/Avatar/"), img);
                                // Save the uploaded file  
                                httpPostedFile.SaveAs(path);
                                user.avatar = "/Media/Images/Avatar/" + img;
                            }
                        }
                    }


                    db.Entry(user).State = EntityState.Modified;
                    //db.SaveChanges();

                    if (role_id == 6)
                    {
                        if (HttpContext.Current.Request.Form["skill_ids"] != null)
                        {
                            long[] skill_id = HttpContext.Current.Request.Form["skill_ids"].Split(',').Select(p => Convert.ToInt64(p)).ToArray();
                            long[] oldskill_id = db.skill_user.Where(x => x.user_id == uid).ToList().Select(x => x.skill_id).ToArray();
                            long[] listCartisan = skill_id.Except(oldskill_id).ToArray();
                            foreach (var item in listCartisan)
                            {
                                skill_user su = new skill_user();
                                su.user_id = uid;
                                su.skill_id = item;
                                su.is_approved = false;
                                su.created_at = DateTime.Now;
                                db.skill_user.Add(su);
                            }
                        }
                    }

                    db.SaveChanges();

                    d.status = true;
                    d.message = GetToken(user.phone);
                    user.role = null;
                    d.data = user;//GetToken(user.phone);
                    return d;
                }
                else
                {
                    d.status = false;
                    d.message = "User not exists";
                    d.data = null;
                    return d;
                }
            }

            d.status = false;
            d.message = "something wrong contact to technical person";
            return d;
        }


        [Authorize]
        [HttpGet]
        [Route("consultants")]
        public object getAllConsultants()
        {
            var identity = User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity.Claims;
            string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;
            long uid = Convert.ToInt32(user_id);

            List<user> consultants = db.users.Where(user => user.role_id == 6 && user.is_profile_completed == true && user.id != uid && user.is_approved == true && !user.phone.StartsWith("_")).ToList();
            if (consultants.Count() > 0)
            {
                for (int i = 0; i < consultants.Count(); i++)
                {
                    consultants[i].skills = GetSkillsByConsultant(consultants[i].id);
                }
            }

            var consultatns = consultants
                                .Select(u => new
                                {
                                    id = u.id,
                                    name = u.name + " " + u.last_name,
                                    phone = u.phone,
                                    region = u.region,
                                    skills = u.skills,
                                    profile = u.profile,
                                    avatar = u.avatar,
                                    chatId = u.chatId,
                                    is_online = u.is_online,
                                    rating = u.consultant_rating.Count() > 0 ? u.consultant_rating.Select(q => q.rating).Average() : 0,
                                    role_id = u.role_id,
                                    last_login = u.updated_at
                                })
                                .ToList();

            return consultatns;
        }

        [Authorize]
        [HttpGet]
        [Route("updateChatID/{chatID}")]
        public object updateChatID(string chatID)
        {
            var identity = User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity.Claims;
            string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;
            long uid = Convert.ToInt32(user_id);
            var u = db.users.Where(user => user.id == uid).FirstOrDefault();
            u.chatId = chatID;
            db.SaveChanges();

            return Ok("successfully inserted");

        }

        [Authorize]
        [HttpGet]
        [Route("getkeywords")]
        public Data getkeywords()
        {
            Data d = new Data();
            if (User.Identity.IsAuthenticated)
            {
                var keywords = db.keywords.Select(p => new { p.id, p.keyword1 }).ToList();

                d.status = true;
                d.message = "successfully fetch the records";
                d.data = keywords;
                return d;
            }

            d.status = false;
            d.message = "user is not authenticated!";
            d.data = null;
            return d;
        }

        [HttpGet]
        [Route("getskills")]
        public object getskills()
        {
            SkillItem skills = new SkillItem()
            {
                items = db.skills.Where(x => x.parent_skill == null && x.active == true).OrderBy(x => x.menu_sort).Select(p => new Skills()
                {
                    id = p.id,
                    name_ar = p.name,
                    menu_sort = p.menu_sort,
                    description = p.description,
                    icon = p.icon,
                    banner = p.banner,
                    text_color = p.text_color,
                    bg_color = p.bg_color
                }).ToList()

            };

            for (int i = 0; i < skills.items.Count(); i++)
            {
                if (db.skills.Where(x => x.parent_skill == skills.items[i].id && x.active == true) != null)
                {
                    skills.items[i].children = new List<Skills>();
                    int p_id = Convert.ToInt16(skills.items[i].id);
                    skills.items[i].children.AddRange(db.skills.Where(x => x.parent_skill == p_id && x.active == true).Select(q => new Skills()
                    {
                        id = q.id,
                        name_ar = q.name,
                        menu_sort = 0,
                        description = q.description,
                        icon = q.icon,
                        banner = q.banner,
                        text_color = q.text_color,
                        bg_color = q.bg_color
                    }).ToList());
                }

                for (int y = 0; y < skills.items[i].children.Count(); y++)
                {
                    if (db.skills.Where(x => x.parent_skill == skills.items[i].children[y].id && x.active == true) != null)
                    {
                        skills.items[i].children[y].children = new List<Skills>();
                        int y_id = Convert.ToInt16(skills.items[i].children[y].id);
                        skills.items[i].children[y].children.AddRange(db.skills.Where(x => x.parent_skill == y_id && x.active == true).Select(q => new Skills()
                        {
                            id = q.id,
                            name_ar = q.name,
                            menu_sort = 0,
                            description = q.description,
                            icon = q.icon,
                            banner = q.banner,
                            text_color = q.text_color,
                            bg_color = q.bg_color
                        }).ToList());
                    }
                }
            }

            return skills.items;
        }

        [HttpGet]
        [Route("getroles")]
        public Data getroles()
        {
            Data d = new Data();

            var roles = db.roles.Select(p => new { p.id, p.name_ar }).ToList();
            d.status = true;
            d.message = "successfully fetch the records";
            d.data = roles;
            return d;
        }

        [HttpGet]
        [Route("getcities")]
        public object getcities()
        {
            db.Configuration.ProxyCreationEnabled = false;
            return db.cities.ToList();
        }

        [HttpGet]
        [Route("getregions")]
        public Data getregions()
        {
            Data d = new Data();

            var regions = db.regions.Where(x => x.active == true).Select(p => new { p.id, p.name_ar, p.name }).ToList();
            d.status = true;
            d.message = "successfully fetch the records";
            d.data = regions;
            return d;

        }

        [Authorize]
        [HttpGet]
        [Route("getgroups")]
        public Data getgroups()
        {
            Data d = new Data();
            if (User.Identity.IsAuthenticated)
            {
                var groups = db.groups.Select(p => new { p.id, p.region_id, p.region.name, p.name_ar }).ToList();
                d.status = true;
                d.message = "successfully fetch the records";
                d.data = groups;
                return d;
            }
            d.status = false;
            d.message = "user is not authenticated!";
            d.data = null;
            return d;
        }

        [Authorize]
        [HttpGet]
        [Route("GetAllConsultantRating")]
        public Object GetAllConsultantRating()
        {
            var identity = User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity.Claims;
            string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;
            long uid = Convert.ToInt32(user_id);
            
            var result = db.consultant_rating.Where(x => x.consultant_id == uid).Select(q => new { 
                q.comment, q.rating,q.user1.avatar, q.user1.name
            }).ToList();

            return result;
        }

        // insert consultant rating which submit by farmers
        [Authorize]
        [HttpPost]
        [Route("ConsultantRating")]
        public Data ConsultantRating()
        {
            try
            {
                Data d = new Data();
                var identity = User.Identity as ClaimsIdentity;
                IEnumerable<Claim> claims = identity.Claims;
                string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;
                long uid = Convert.ToInt32(user_id);
                int consultant_id = Convert.ToInt32(HttpContext.Current.Request.Form["consultant_id"].ToString());
                string comment = HttpContext.Current.Request.Form["comment"].ToString();
                decimal rating = Convert.ToDecimal(HttpContext.Current.Request.Form["rating"].ToString());

                // check first if userid, consultant_id is exists then update else insert
                consultant_rating cr = db.consultant_rating.Where(x => x.user_id == uid && x.consultant_id == consultant_id).FirstOrDefault();
                if (cr != null)
                {
                    cr.comment = comment;
                    cr.rating = rating;
                    cr.created_at = DateTime.Now;
                    db.Entry(cr).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    cr = new consultant_rating();
                    cr.comment = comment;
                    cr.rating = rating;
                    cr.user_id = uid;
                    cr.consultant_id = consultant_id;
                    cr.created_at = DateTime.Now;
                    db.consultant_rating.Add(cr);
                    db.SaveChanges();
                }

                


                Generic g = new Generic();
                user u = db.users.Find(uid);
                user consultant = db.users.Find(consultant_id);

                // for user rating update
                decimal? consultant_Rating = db.consultant_rating.Where(x => x.consultant_id == consultant_id).Select(x => x.rating).Average();
                consultant.rating = consultant_Rating != null ? consultant_Rating.Value : 0;
                db.Entry(consultant).State = EntityState.Modified;
                db.SaveChanges();


                // for sending notification
                g.sendNotificationOnPriceUpdate("تقييم", "تم تقييمك بواسطة  : " + u.name, consultant.phone);

                g.InsertNotificationData(consultant_id, "تم تقييمك بواسطة  : " + g.GetFullName(uid), (Int16)AppConstants.Type.Rating, cr.id, "");

                d.status = true;
                d.message = "successful";
                d.data = null;
                return d;
            }
            catch (Exception ex)
            {
                Data d = new Data();
                d.status = false;
                d.message = ex.Message;
                d.data = ex.InnerException;
                return d;
            }
        }

        [HttpGet]
        [Route("GetQuestionDetail")]
        public Data GetQuestionDetail(long id)
        {
            Data d = new Data();
            var identity = User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity.Claims;
            string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;
            long uid = 0;
            if (user_id != null && user_id != "")
            {
                uid = Convert.ToInt32(user_id);
            }

            var questions = db.qa_questions.Where(x => (x.id == id && x.is_approved == true) || (x.id == id && x.user_id == uid)).Select(p => new
            {
                p.id,
                p.title,
                p.user_id,
                p.description,
                p.qa_category_id,
                date = p.created_at,
                p.keywords,
                p.is_verified,
                p.is_approved,
                p.vote_count,
                created_by = p.user.name,
                attachments = db.qa_attachments.Where(z => z.type_id == p.id && z.type == false).Select(
                    a => new
                    {
                        id = a.id,
                        media_id = a.media_id,
                        type_id = a.type_id,
                        type = a.type,
                        url = db.media.Where(x => x.id == a.media_id).FirstOrDefault() != null ? "/Media/QA/Question/" + db.media.Where(x => x.id == a.media_id).FirstOrDefault().file_name : ""
                    }
                    ),
                answers = p.qa_answers.Where(x => x.is_approved == true || x.user_id == uid).OrderBy(x => x.id).ToList().Select(ans => new
                {
                    ans.id,
                    ans.title,
                    ans.description,
                    date = ans.created_at,
                    ans.vote_count,
                    ans.is_verified,
                    ans.is_approved,
                    created_by = ans.user.name,
                    avatar = ans.user.avatar,
                    user_id = ans.user.id,
                    approved_by = ans.user1.name,
                    verified_by = ans.user2.name,
                    attachments = db.qa_attachments.Where(z => z.type_id == ans.id && z.type == true).Select(a => new
                    {
                        id = a.id,
                        media_id = a.media_id,
                        type_id = a.type_id,
                        type = a.type,
                        url = db.media.Where(x => x.id == a.media_id).FirstOrDefault() != null ? "/Media/QA/Answer/" + db.media.Where(x => x.id == a.media_id).FirstOrDefault().file_name : ""
                    })
                })
            }).FirstOrDefault();
            d.status = true;
            d.message = "successfully fetch the records";
            //questions.date.Value.FormattedDate();
            
            d.data = questions;
            return d;
        }

        public string CategoryName(int id)
        {
            return db.categories.Where(x => x.id == id).FirstOrDefault().name;
        }
        public string CheckFileFormat(string type)
        {
            string filetype = "";
            if (type.Contains("image"))
            {
                filetype = "Image";
            }
            else if (type.Contains("video"))
            {
                filetype = "Video";
            }

            return filetype;
        }

        // anwer vote up or down from the farmers
        [Authorize]
        [HttpGet]
        [Route("AnswerVote")]
        public Data AnswerVote(long aid, int value)
        {
            Data d = new Data();
            var identity = User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity.Claims;
            string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;
            long uid = Convert.ToInt32(user_id);
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                // for updating QA_Question table
                qa_answers a = db.qa_answers.Where(x => x.id == aid).FirstOrDefault();
                if (a != null)
                {
                    if (a.user_id != uid)
                    {
                        if (value == 1)
                        {
                            qa_votes qa = db.qa_votes.Where(y => y.type == true && y.type_id == a.id && y.user_id == uid && y.is_up_vote == true).FirstOrDefault();
                            if (qa == null)
                            {
                                a.vote_count = a.vote_count + 1;

                                db.Entry(a).State = EntityState.Modified;
                                db.SaveChanges();

                                qa_votes qa1 = db.qa_votes.Where(y => y.type == true && y.type_id == a.id && y.user_id == uid && y.is_up_vote == false).FirstOrDefault();
                                if (qa1 != null)
                                {
                                    qa1.is_up_vote = true;
                                    db.Entry(qa1).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                                else
                                {
                                    //for inserting into QA_votes table
                                    qa_votes v = new qa_votes();
                                    v.created_at = DateTime.Now;
                                    v.is_up_vote = value == 1 ? true : false;
                                    v.type = true; // 0 for question and 1 for Answer
                                    v.type_id = aid;
                                    v.user_id = Convert.ToInt32(user_id);
                                    db.qa_votes.Add(v);
                                    db.SaveChanges();
                                }
                            }
                        }
                        else if (value == 0)
                        {
                            qa_votes qa = db.qa_votes.Where(y => y.type == true && y.type_id == a.id && y.user_id == uid && y.is_up_vote == false).FirstOrDefault();
                            if (qa == null)
                            {
                                a.vote_count = a.vote_count - 1;
                                if (a.vote_count < 0)
                                {
                                    a.vote_count = 0;
                                }
                                db.Entry(a).State = EntityState.Modified;
                                db.SaveChanges();

                                qa_votes qa1 = db.qa_votes.Where(y => y.type == true && y.type_id == a.id && y.user_id == uid && y.is_up_vote == true).FirstOrDefault();
                                if (qa1 != null)
                                {
                                    qa1.is_up_vote = false;
                                    db.Entry(qa1).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                                else
                                {
                                    //for inserting into QA_votes table
                                    qa_votes v = new qa_votes();
                                    v.created_at = DateTime.Now;
                                    v.is_up_vote = value == 1 ? true : false;
                                    v.type = true; // 0 for question and 1 for Answer
                                    v.type_id = aid;
                                    v.user_id = Convert.ToInt32(user_id);
                                    db.qa_votes.Add(v);
                                    db.SaveChanges();
                                }
                            }
                        }
                        dbContextTransaction.Commit();
                    }
                }

                d.status = true;
                d.message = "successfully answer voted inserted";
                d.data = a.vote_count;
                return d;
            }
        }

        // question vote up or down from the farmers
        [Authorize]
        [HttpGet]
        [Route("QuestionVote")]
        public Data QuestionVote(long qid, int value)
        {
            Data d = new Data();
            var identity = User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity.Claims;
            string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;
            long uid = Convert.ToInt32(user_id);
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                // for updating QA_Question table
                qa_questions q = db.qa_questions.Where(x => x.id == qid).FirstOrDefault();
                if (q.user_id != uid)
                {
                    if (value == 1)
                    {
                        qa_votes qa = db.qa_votes.Where(y => y.type == false && y.type_id == q.id && y.user_id == uid && y.is_up_vote == true).FirstOrDefault();
                        if (qa == null)
                        {
                            q.vote_count = q.vote_count + 1;

                            db.Entry(q).State = EntityState.Modified;
                            db.SaveChanges();

                            qa_votes qa1 = db.qa_votes.Where(y => y.type == false && y.type_id == q.id && y.user_id == uid && y.is_up_vote == false).FirstOrDefault();
                            if (qa1 != null)
                            {
                                qa1.is_up_vote = true;
                                db.Entry(qa1).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                //for inserting into QA_votes table
                                qa_votes v = new qa_votes();
                                v.created_at = DateTime.Now;
                                v.is_up_vote = value == 1 ? true : false;
                                v.type = false; // 0 for question and 1 for Answer
                                v.type_id = qid;
                                v.user_id = Convert.ToInt32(user_id);
                                db.qa_votes.Add(v);
                                db.SaveChanges();
                            }
                        }
                        
                    }
                    else if (value == 0)
                    {
                        qa_votes qa = db.qa_votes.Where(y => y.type == false && y.type_id == q.id && y.user_id == uid && y.is_up_vote == false).FirstOrDefault();
                        if (qa == null)
                        {
                            q.vote_count = q.vote_count - 1;
                            if (q.vote_count < 0)
                            {
                                q.vote_count = 0;
                            }
                            db.Entry(q).State = EntityState.Modified;
                            db.SaveChanges();

                            qa_votes qa1 = db.qa_votes.Where(y => y.type == false && y.type_id == q.id && y.user_id == uid && y.is_up_vote == true).FirstOrDefault();
                            if (qa1 != null)
                            {
                                qa1.is_up_vote = false;
                                db.Entry(qa1).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                //for inserting into QA_votes table
                                qa_votes v = new qa_votes();
                                v.created_at = DateTime.Now;
                                v.is_up_vote = value == 1 ? true : false;
                                v.type = false; // 0 for question and 1 for Answer
                                v.type_id = qid;
                                v.user_id = Convert.ToInt32(user_id);
                                db.qa_votes.Add(v);
                                db.SaveChanges();
                            }
                        }
                    }
                }
                
                dbContextTransaction.Commit();

                d.status = true;
                d.message = "successfully question voted inserted";
                d.data = q.vote_count;
                return d;

            }
        }

        // get all products which are subscribed by end user
        [Authorize]
        [HttpGet]
        [Route("getsubscribersproduct")]
        public object getsubscribersproduct()
        {
            var identity = User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity.Claims;
            string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;
            long uid = Convert.ToInt32(user_id);
            List<product_subscribers> ps = db.product_subscribers.Where(x => x.user_id == uid).ToList();
            List<fun_GetLatestProductPricesByUser_Result> final = new List<fun_GetLatestProductPricesByUser_Result>();
            foreach (var item in ps)
            {
                int mid = Convert.ToInt32(item.market_id);
                int pid = Convert.ToInt32(item.product_id);
                var result = db.fun_GetLatestProductPricesByUser(mid, pid).FirstOrDefault();
                if (result != null)
                {
                    final.Add(result);
                }
            }

            return final;
        }

        [HttpGet]
        [Route("category/{category}")]
        public object getCategoryDetails(long category)
        {

            var allNews = db.articles
                                .Where(art => art.category.id == category)
                                .Where(art => art.active == true)
                                .Select(art => new Article()
                                {
                                    id = art.id,
                                    title_ar = art.title,
                                    desc_ar = art.short_description,
                                    image = art.image,
                                    created_at = art.created_at
                                }).OrderByDescending(x => x.created_at)
                                .ToList();


            return Ok(allNews);
        }

        [HttpGet]
        [Route("subcategory/{category}")]
        public object getSubcategoryDetails(long category)
        {

            var data = db.categories
                            .Where(cat => cat.parent_category == category && cat.active == true)
                            .Select(cat => new DashboardItems()
                            {
                                id = cat.id,
                                name_ar = cat.name,
                                desc_ar = cat.description,
                                icon = cat.icon,
                                bg_color = cat.bg_color,
                                text_color = cat.text_color,
                                banner_ar = cat.banner
                            })
                            .ToList();

            var news = db.articles
                                .Where(art => art.category.id == category)
                                .Where(art => art.active == true)
                                .Select(art => new Article()
                                {
                                    id = art.id,
                                    title_ar = art.title,
                                    desc_ar = art.short_description,
                                    image = art.image,
                                    created_at = art.created_at
                                }).OrderByDescending(x => x.created_at)
                                .ToList();


            return Ok(new { data, news });
        }

        // check the user is online or offline and send the notification
        [Authorize]
        [HttpGet]
        [Route("IsOnline")]
        public async Task<Data> IsOnline(bool value)
        {
            Data d = new Data();
            try
            {
                var utoEnd = System.Configuration.ConfigurationManager.AppSettings["DefaltActivHours"];
                
                var identity = User.Identity as ClaimsIdentity;
                IEnumerable<Claim> claims = identity.Claims;
                string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;
                long uid = Convert.ToInt32(user_id);
               
                user u = db.users.Where(x => x.id == uid).FirstOrDefault();

                ConsultantsStatusLog consultantStatusLog = new ConsultantsStatusLog()
                {
                    UsrId = uid,
                    IsOnline = value,
                    EndTime = DateTime.Now.AddHours(int.Parse(utoEnd)),
                    StartTime = DateTime.Now,

                };
                var consultantlog = db.ConsultantsStatusLogs.Where(w => w.UsrId == uid).FirstOrDefault();

                if (value)
                {

                    if (consultantlog!=null)
                    {
                        consultantStatusLog.Id = consultantlog.Id;
                        db.ConsultantsStatusLogs.AddOrUpdate(consultantStatusLog);
                        db.SaveChanges();
                    }
                    else
                    {
                        db.ConsultantsStatusLogs.Add(consultantStatusLog);
                        db.SaveChanges();
                    }
                }
                else
                {
                    if (consultantlog != null)
                    {
                        consultantStatusLog.Id = consultantlog.Id;
                        db.ConsultantsStatusLogs.AddOrUpdate(consultantStatusLog);
                        var activHours = new ConsultantsActiveHoursInDay()
                        {
                            UserId = consultantlog.UsrId,
                            Hours = DateTime.Now.Hour - consultantlog.StartTime.Hour ,
                            Date = DateTime.Now 
                        };
                        db.ConsultantsActiveHoursInDays.Add(activHours);
                        db.SaveChanges();

                    }
                    else
                    {
                        
                    }

                }
                u.is_online = value;
                db.Entry(u).State = EntityState.Modified; 
                db.SaveChanges();
                Generic g = new Generic();
                
                
                //******//
                        // notification is disabled
                        // g.SendTopicNotification("status change", "", u.phone, "work_status", u.phone);
                //******//
                d.status = true;
                d.message = "Updated Successfully";
                d.data = null;
                return d;
            }
            catch (Exception ex)
            {
                d.status = false;
                d.message = ex.Message.ToString();
                d.data = null;
                return d;
            }
        }

        // it changes the status of user
        // he/she can group admin or can_post 
        [Authorize]
        [HttpGet]
        [Route("ChangeStatusOfUser")]
        public Data ChangeStatusOfUser(long user_id, bool status)
        {
            Data d = new Data();
            user u = db.users.Find(user_id);
            if (u != null)
            {
                u.is_group_admin = status == true ? false : true;
                u.can_post = status == true ? false : true;
                db.SaveChanges();
            }
            d.status = true;
            d.message = "Success";
            d.data = null;
            return d;
        }

        [Authorize]
        [HttpGet]
        [Route("DeleteProductSubscriber")]
        public Data DeleteProductSubscriber(long marketid, long productid)
        {
            Data d = new Data();
            var identity = User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity.Claims;
            string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;
            long uid = Convert.ToInt32(user_id);
            product_subscribers ps = db.product_subscribers.Where(x => x.market_id == marketid && x.product_id == productid && x.user_id == uid).FirstOrDefault();
            if (ps != null)
            {
                db.product_subscribers.Remove(ps);
                db.SaveChanges();
                d.message = "successfully deleted product subscriber";
            }
            else
            {
                d.message = "record not exists against this market and product";
            }

            d.status = true;
            d.data = null;
            return d;

        }

        // showing list of product based on marketid, productid, and increase or decrease the price
        [Authorize]
        [HttpGet]
        [Route("ProductSubscriber")]
        public Data ProductSubscriber(long marketid, long productid, int onincrease)
        {
            Data d = new Data();
            var identity = User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity.Claims;
            string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;
            long uid = Convert.ToInt32(user_id);


            product_subscribers ps = db.product_subscribers.Where(x => x.market_id == marketid && x.product_id == productid && x.user_id == uid).FirstOrDefault();
            if (ps == null)
            {
                try
                {
                    ps = new product_subscribers();
                    ps.created_at = DateTime.Now;
                    ps.is_active = true;
                    ps.market_id = marketid;
                    ps.product_id = productid;
                    ps.on_increase = onincrease;
                    ps.user_id = Convert.ToInt32(user_id);
                    db.product_subscribers.Add(ps);
                    db.SaveChanges();

                    d.message = "successfully insert product subscriber";
                }
                catch (Exception ex)
                {
                    d.message = ex.Message.ToString();
                    d.data = ex.InnerException.InnerException.Message.ToString();
                }
            }
            else
            {
                ps.updated_at = DateTime.Now;
                ps.on_increase = onincrease;
                db.Entry(ps).State = EntityState.Modified;
                db.SaveChanges();
                d.message = "successfully updated product subscriber";
            }

            d.status = true;
            //d.data = null;
            return d;
        }

        // avatar is user profile pic
        // just update from the end user
        [Authorize]
        [HttpPost]
        [Route("UploadAvatar")]
        public Data UploadAvatar()
        {
            Data d = new Data();
            var identity = User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity.Claims;
            string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;
            var httpContext = HttpContext.Current;
            HttpPostedFile httpPostedFile = httpContext.Request.Files[0];
            if (httpPostedFile != null)
            {
                Guid guid = Guid.NewGuid();
                var img = guid.ToString() + Path.GetExtension(httpPostedFile.FileName);
                var path = Path.Combine(HostingEnvironment.MapPath("~/Media/Images/Avatar/"), img);
                // Save the uploaded file  
                httpPostedFile.SaveAs(path);
                long uid = Convert.ToInt32(user_id);
                user u = db.users.Find(uid);
                u.avatar = "/Media/Images/Avatar/" + img;
                db.Entry(u).State = EntityState.Modified;
                db.SaveChanges();
                d.status = true;
                d.message = "Successfully Upload Avatar";
                d.data = u.avatar;
                return d;
            }

            d.status = false;
            d.message = "unsuccessful";
            d.data = null;
            return d;
        }

        [HttpGet]
        [Route("getCalendarArticles")]
        public object getCalendarArticles()
        {
            return db.Agriculture_Calender.Select(p => new { p.title, p.description, p.file_name }).ToList();
        }

        [Authorize]
        [HttpPost]
        [Route("CreateNewAnswer")]
        public Data CreateNewAnswer()
        {
            Data d = new Data();
            var identity = User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity.Claims;
            string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;
            string fullname = claims.Where(p => p.Type == "fullname").FirstOrDefault()?.Value;
            // for inserting into QA_Answer Table

            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                qa_answers ans = new qa_answers();
                ans.title = HttpContext.Current.Request.Form["title"];
                ans.description = HttpContext.Current.Request.Form["description"];
                ans.question_id = Convert.ToInt32(HttpContext.Current.Request.Form["qid"].ToString());
                ans.created_at = DateTime.Now;
                ans.user_id = Convert.ToInt32(user_id);
                ans.vote_count = 0;

                user _u = db.users.Find(ans.user_id.Value);
                Generic g = new Generic();
                if (_u.role_id == 6 || _u.role_id == 5)
                {
                    ans.approved_at = DateTime.Now;
                    ans.verified_at = DateTime.Now;
                    ans.approved_by = ans.user_id;
                    ans.verified_by = ans.user_id;
                    ans.is_approved = true;
                    //ans.is_verified = true;

                    qa_questions questionUser = db.qa_questions.Find(ans.question_id);
                    g.sendNotificationOnChats("إجابة", "تم نشر إجابتك!", _u.phone, "answer_approved", ans.question_id.ToString());
                    //g.sendNotificationOnChats("إجابة", "تمت الموافقة على إجابتك!", _u.phone);

                    g.InsertNotificationData(_u.id, "تم نشر إجابتك!", (Int16)AppConstants.Type.Answer, ans.id, "");

                    // for question user send notification when his or her question answered by another user.
                    qa_questions _Questions = db.qa_questions.Where(x => x.id == ans.question_id).FirstOrDefault();
                    if (_Questions != null)
                    {
                        g.sendNotificationOnChats("إجابة", "تمت الإجابة على سؤالك!", _Questions.user.phone, "answer_approved", ans.question_id.ToString());

                        g.InsertNotificationData(_Questions.user_id.Value, "تمت الإجابة على سؤالك", (Int16)AppConstants.Type.Answer, ans.question_id.Value, "");
                    }
                }
                
                db.qa_answers.Add(ans);
                db.SaveChanges();

                var httpContext = HttpContext.Current;

                // Check for any uploaded file  
                if (httpContext.Request.Files.Count > 0)
                {
                    //Loop through uploaded files  
                    for (int i = 0; i < httpContext.Request.Files.Count; i++)
                    {
                        HttpPostedFile httpPostedFile = httpContext.Request.Files[i];
                        if (httpPostedFile != null)
                        {
                            Guid guid = Guid.NewGuid();
                            var img = guid.ToString() + Path.GetExtension(httpPostedFile.FileName);
                            var path = Path.Combine(HostingEnvironment.MapPath("~/Media/QA/Answer/"), img);
                            // Save the uploaded file  
                            httpPostedFile.SaveAs(path);

                            medium m = new medium();
                            m.title = ans.title;
                            m.description = ans.description;
                            m.file_name = img;
                            m.file_type = CheckFileFormat(httpPostedFile.ContentType);
                            m.is_internal_file = true;
                            m.is_active = true;
                            m.created_at = DateTime.Now;
                            m.created_by = ans.user_id;
                            db.media.Add(m);
                            db.SaveChanges();

                            // for inserting into attachments table
                            qa_attachments attach = new qa_attachments();
                            attach.media_id = m.id;
                            attach.type = true; //0 for question and 1 for Answer
                            attach.type_id = ans.id;
                            db.qa_attachments.Add(attach);
                            db.SaveChanges();
                        }
                    }
                }

                dbContextTransaction.Commit();
                var result = Request.CreateResponse(HttpStatusCode.Created);

                d.status = true;
                d.message = "successfully answer inserted";
                d.data = null;
                return d;
            }
        }

        
        [HttpGet]
        [Route("GetCurrentWeatherNoficationByCityId")]
        public Data GetCurrentWeatherNoficationByCityId(int city_id)
        {
            Data d = new Data();
            List<currentweathernotificationforCity> wdata = db.currentweathernotificationforCities.Where(x => x.city_id == city_id).ToList();
            d.data = wdata;
            d.status = true;
            d.message = wdata.Count() > 0 ? "successfully fetch records!" : "no record found!";
            return d;
        }

        [Authorize]
        [HttpPost]
        [Route("CreateNewQuestion")]
        public async Task<Data> CreateNewQuestion()
        {
            Data d = new Data();
            var identity = User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity.Claims;
            string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;
            // for inserting into Question Table

            try
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {

                    qa_questions questions = new qa_questions();
                    questions.title = HttpContext.Current.Request.Form["title"];
                    questions.description = HttpContext.Current.Request.Form["description"];
                    questions.qa_category_id = Convert.ToInt32(HttpContext.Current.Request.Form["category_id"].ToString());
                    questions.keywords = HttpContext.Current.Request.Form["keywords"];
                    questions.user_id = Convert.ToInt32(user_id);
                    questions.created_at = DateTime.Now;
                    questions.vote_count = 0;

                    user _u = db.users.Find(questions.user_id.Value);
                    if (_u.role_id == 6)
                    {
                        questions.approved_at = DateTime.Now;
                        questions.verified_at = DateTime.Now;
                        questions.approved_by = questions.user_id;
                        questions.verified_by = questions.user_id;
                        questions.is_approved = true;
                        questions.is_verified = true;
                        db.qa_questions.Add(questions);
                        db.SaveChanges();
                        //long Question_id = questions.id;

                        

                    }
                    else
                    {

                        db.qa_questions.Add(questions);
                        db.SaveChanges();
                        long q_id = questions.id;
                    }
                    var httpContext = HttpContext.Current;

                    // Check for any uploaded file  
                    if (httpContext.Request.Files.Count > 0)
                    {
                        //Loop through uploaded files  
                        for (int i = 0; i < httpContext.Request.Files.Count; i++)
                        {
                            HttpPostedFile httpPostedFile = httpContext.Request.Files[i];
                            if (httpPostedFile != null)
                            {
                                Guid guid = Guid.NewGuid();
                                var img = guid.ToString() + Path.GetExtension(httpPostedFile.FileName);
                                var path = Path.Combine(HostingEnvironment.MapPath("~/Media/QA/Question/"), img);
                                // Save the uploaded file  
                                httpPostedFile.SaveAs(path);

                                medium m = new medium();
                                m.title = questions.title;
                                m.description = questions.description;
                                m.file_name = img;
                                m.file_type = CheckFileFormat(httpPostedFile.ContentType);
                                m.is_internal_file = true;
                                m.keywords = questions.keywords;
                                m.related_to = CategoryName(Convert.ToInt32(HttpContext.Current.Request.Form["category_id"].ToString()));
                                m.is_active = true;
                                m.created_at = DateTime.Now;
                                m.created_by = questions.user_id;
                                db.media.Add(m);
                                db.SaveChanges();

                                // for inserting into attachments table
                                qa_attachments attach = new qa_attachments();
                                attach.media_id = m.id;
                                attach.type = false; //0 for question and 1 for Answer
                                attach.type_id = questions.id;
                                db.qa_attachments.Add(attach);
                                db.SaveChanges();
                            }
                        }
                    }

                    dbContextTransaction.Commit();
                    var result = Request.CreateResponse(HttpStatusCode.Created);

                    if (_u.role_id == 6)
                    {
                        var QCB = new QAController();
                        string updateQuestion = await QCB.AddQuestionToChatBot(questions);

                        Generic g = new Generic();

                        g.sendNotificationOnChats("سؤال", "تمت الموافقة على سؤالك!", _u.phone, "question_approved", questions.id.ToString());
                        g.InsertNotificationData(_u.id, "تمت الموافقة على سؤالك", (Int16)AppConstants.Type.Question, questions.id, updateQuestion);
                    }


                    d.status = true;
                    d.message = "successfully question inserted";
                    d.data = null;//new { questions, qa_attachments = db.qa_attachments.Where(x => x.type_id == questions.id && x.type == false).ToList() };
                    return d;
                }
            }
            catch (Exception ex)
            {
                d.status = true;
                d.message = ex.Message;
                d.data = null;
                return d;
            }
        }

        [Authorize]
        [HttpPost]
        [Route("updateAppVersion")]
        public Data UpdateApp()
        {
            Data d = new Data();
            var identity = User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity.Claims;
            string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;
            string appVersion = HttpContext.Current.Request.Form["appVersion"];
            string lastLocation = HttpContext.Current.Request.Form["lastLocation"];
            int appType = Convert.ToInt16(HttpContext.Current.Request.Form["appType"].ToString());
            long uid = 0;
            if (user_id != "" && user_id != null)
            {
                uid = Convert.ToInt32(user_id);
            }
            else
            {
                d.status = false;
                d.message = "some thing went wrong!";
            }

            user u = db.users.SingleOrDefault(us => us.id == uid);
            if (u != null)
            {
                u.app_type = appType;
                u.app_ver = appVersion;
                u.firebase_token = lastLocation;
                u.updated_at = DateTime.Now;
                db.SaveChanges();

                d.status = true;
                d.message = "successfully updated!";
            }
            else
            {
                d.status = false;
                d.message = "some thing went wrong!";
            }

            return d;
        }

        [HttpGet]
        [Route("searcharticles_V2")]
        public Data searcharticles_V2(int? Page_No = 0, string Search_Data = null, int page_size = 20, int catid = 0, int rno = 1)
        {
            Data d = new Data();

            List<article> articles = db.articles.Where(x => x.active == true).ToList();

            int pagesize = page_size;

            int No_Of_Page = (Page_No ?? 1);
            if (Page_No == 0 || Page_No == null) { Page_No = 1; }

            if (catid > 0)
            {
                articles = articles.Where(x => x.category_id == catid).ToList();
            }

            if (!String.IsNullOrEmpty(Search_Data))
            {
                articles = articles.Where(x => x.title.Contains(Search_Data) || x.short_description.Contains(Search_Data)).ToList();
            }

            int TotalPages = (int)Math.Ceiling(articles.Count() / (double)pagesize);

            int articles_count = articles.Count();

            articles = articles.Skip((No_Of_Page - 1) * pagesize).Take(pagesize).ToList();

            // if CurrentPage is greater than 1 means it has previousPage  
            var previous_Page = Page_No > 1 ? true : false;
            // if TotalPages is greater than CurrentPage means it has nextPage  
            var next_Page = Page_No < TotalPages ? true : false;

            d.status = true;
            d.message = "successfully fetch the records";
            d.data = articles.Select(art => new { id = art.id, title_ar = art.title, category_name = art.category.name, description = art.description, short_description = art.short_description, type = 1, category_id = art.category_id, Image = art.image }).ToList();
            d.page_info = new { total_count = articles_count, current_page = Page_No, total_page = TotalPages, next_Page, previous_Page, catid = catid, rno = rno };
            return d;
        }

        [HttpGet]
        [Route("GetTop5OnlineConsultantForChatBot")]
        public Data GetUserDetail(long regionid = 0, int skillid = 0, long userID = 0)
        {
            Data d = new Data();
            IQueryable<user> user = db.users.Where(x => x.role_id == (int)AppConstants.Role.Consultants && x.is_profile_completed == true && x.is_online == true && x.chatId != null);
            user = user.OrderByDescending(x => x.rating);
            //List<user> user = db.users.Where(x => x.role_id == 6 && x.is_online == true && x.chatId != null).ToList();

            if (userID > 0)
            {
                user = user.Where(x => x.id != userID);
            }
            
            if (regionid > 0)
            {
                user = user.Where(x => x.region_id == regionid);
            }
            
            if (skillid > 0)
            {
                user = user.Where(x => x.skill_user.Where(y => y.is_approved == true && y.skill_id == skillid).Any());
            }

            if (user != null)
            {
                d.status = true;
                d.message = "successfully fetch the records";
                user = user.Take(5);
                //List<string> su = db.skill_user.Where(x => x.user_id == user.id && x.is_approved).Select(x => x.skill.name).ToList();
                d.data = user.Select(us => new 
                { 
                    id = us.id, 
                    name = us.name + " " + us.last_name, 
                    phone = us.phone, 
                    chatid = us.chatId, 
                    avatar = us.avatar, 
                    region = us.region, 
                    skills = db.skill_user.Where(x => x.user_id == us.id && x.is_approved).Select(x => x.skill.name).ToList(), 
                    rating = us.rating 
                }).ToList();
            }
            return d;
        }

        //[Authorize]
        [HttpGet]
        [Route("GetUserDetailForChatBot")]
        public Data GetUserDetailForChatBot(long id = 0, string phoneNo = "")
        {
            Data d = new Data();
            user user = new user();
            if (id > 0)
            {
                user = db.users.Find(id);
            }
            
            if (phoneNo != "")
            {
                user = db.users.Where(x => x.phone == phoneNo).FirstOrDefault();
            }

            if (user != null)
            {
                d.status = true;
                d.message = "successfully fetch the records";
                List<string> su = db.skill_user.Where(x => x.user_id == user.id && x.is_approved).Select(x => x.skill.name).ToList();
                d.data = new { id = user.id, name = user.name + " " + user.last_name, phone = user.phone, region = user.region, skills = su };
            }
            return d;
        }

        [HttpGet]
        [Route("searcharticles")]
        public Data searcharticles(int? Page_No = 0, string Search_Data = null, int page_size = 20, int catid = 0, int rno = 1)
        {
            Data d = new Data();
            
            List<article> articles = db.articles.Where(x => x.active == true).ToList();

            int pagesize = page_size;

            int No_Of_Page = (Page_No ?? 1);
            if (Page_No == 0 || Page_No == null) { Page_No = 1; }

            if (catid > 0)
            {
                articles = articles.Where(x => x.category_id == catid).ToList();
            }

            if (!String.IsNullOrEmpty(Search_Data))
            {
                articles = articles.Where(x => x.title.Contains(Search_Data) || x.short_description.Contains(Search_Data)).ToList();
            }

            int TotalPages = (int)Math.Ceiling(articles.Count() / (double)pagesize);

            int articles_count = articles.Count();

            articles = articles.Skip((No_Of_Page - 1) * pagesize).Take(pagesize).ToList();

            // if CurrentPage is greater than 1 means it has previousPage  
            var previous_Page = Page_No > 1 ? true : false;
            // if TotalPages is greater than CurrentPage means it has nextPage  
            var next_Page = Page_No < TotalPages ? true : false;

            d.status = true;
            d.message = "successfully fetch the records";
            d.data = articles.Select(art => new { id = art.id, title_ar = art.title, category_name = art.category.name, category_id = art.category_id, Image = art.image }).ToList();
            d.page_info = new { total_count = articles_count, current_page = Page_No, total_page = TotalPages, next_Page, previous_Page, catid = catid, rno = rno };
            return d;
        }

        [HttpGet]
        [Route("GetAllQuestionsByPage")]
        public Data GetAllQuestions(int? Page_No = 0, string Search_Data = null, string Filter_Value = null, Boolean sort_value = false, int page_size = 50)
        {
            // if sort_Value true then send the list id descending order sort by vote_count
            // if sort_value false then send the list id ascending order
            // Filter_Value = CategoryID
            Data d = new Data();
            
            long uid = 0;
            var identity = User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity.Claims;
            string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;
            uid = Convert.ToInt32(user_id);
            int pagesize = page_size;

            int No_Of_Page = (Page_No ?? 1);
            if (Page_No == 0 || Page_No == null) { Page_No = 1; }
            if (Search_Data != null)
            {
                Page_No = 1;
            }
            //else
            //{
            //    Search_Data = Filter_Value;
            //}

            //Filter_Value = Search_Data;

            var questions = db.qa_questions.Where(x => x.is_approved == true || x.user_id == uid).Select(p => new
            {
                p.id,
                p.title,
                p.qa_category_id,
                date = p.created_at,
                p.user_id,
                p.keywords,
                p.vote_count,
                p.is_verified,
                p.is_approved,
                created_by = p.user.name,
                Avatar = p.user.avatar,
                verified_by = p.user2.name,
                approved_by = p.user1.name,
                approved_at = p.approved_at,
                attachments = db.qa_attachments.Where(z => z.type_id == p.id && z.type == false).Select(a => new
                {
                    id = a.id,
                    media_id = a.media_id,
                    type_id = a.type_id,
                    type = a.type,
                    url = db.media.Where(x => x.id == a.media_id).FirstOrDefault() != null ? "/Media/QA/Question/" + db.media.Where(x => x.id == a.media_id).FirstOrDefault().file_name : ""
                })

            }).OrderByDescending(x => x.approved_at).ToList();

            if (!String.IsNullOrEmpty(Search_Data))
            {
                questions = questions.Where(x => x.title.Contains(Search_Data) || x.keywords.Contains(Search_Data)).ToList();
            }

            if (Filter_Value != null &&  Filter_Value != "" && Filter_Value != "0")
            {
                int category_id = Convert.ToInt32(Filter_Value);
                questions = questions.Where(x => x.qa_category_id == category_id).ToList();
            }

            if (sort_value)
            {
                questions = questions.OrderByDescending(x => x.vote_count).ToList();
            }

            int TotalPages = (int)Math.Ceiling(questions.Count() / (double)pagesize);

            int question_count = questions.Count();

            questions = questions.Skip((No_Of_Page - 1) * pagesize).Take(pagesize).ToList();//ToPagedList(No_Of_Page, pagesize);

            // if CurrentPage is greater than 1 means it has previousPage  
            var previous_Page = Page_No > 1 ? true : false;
            // if TotalPages is greater than CurrentPage means it has nextPage  
            var next_Page = Page_No < TotalPages ? true : false;

            d.status = true;
            d.message = "successfully fetch the records";
            d.data = questions;
            d.page_info = new { total_count = question_count, current_page = Page_No, total_page = TotalPages, next_Page, previous_Page };
            return d;

        }

        

        [HttpGet]
        [Route("GetAllQuestions")]
        public Data GetAllQuestions()
        {
            Data d = new Data();

            var questions = db.qa_questions.Select(p => new
            {
                p.id,
                p.title,
                p.qa_category_id,
                date = p.created_at,
                p.user_id,
                p.keywords,
                p.vote_count,
                p.is_verified,
                p.is_approved,
                created_by = p.user.name,
                Avatar = p.user.avatar,
                verified_by = p.user2.name,
                approved_by = p.user1.name,
                attachments = db.qa_attachments.Where(z => z.type_id == p.id && z.type == false).Select(a => new
                {
                    id = a.id,
                    media_id = a.media_id,
                    type_id = a.type_id,
                    type = a.type,
                    url = db.media.Where(x => x.id == a.media_id).FirstOrDefault() != null ? "/Media/QA/Question/" + db.media.Where(x => x.id == a.media_id).FirstOrDefault().file_name : ""
                })

            }).ToList();

            d.status = true;
            d.message = "successfully fetch the records";
            d.data = questions;
            return d;

        }

        [Authorize]
        [HttpGet]
        [Route("RandomKeyword")]
        public Object RandomKeyword()
        {

            var kws = db.keywords.OrderByDescending(r => r.keyword_count).Take(10).Select(q => new { q.id, name = q.keyword1 });

            return kws;
        }

        [Authorize]
        [HttpPost]
        [Route("AddKeyword")]
        public Data AddKeyword(string word)
        {
            Data d = new Data();
            try
            {
                
                keyword k = new keyword();
                k.created_at = DateTime.Now;
                k.keyword1 = word;
                db.keywords.Add(k);
                db.SaveChanges();
                d.status = true;
                d.message = "successfully inserted!";
                d.data = new { id = k.id, name = k.keyword1 };
                return d;
            }
            catch (Exception ex)
            {
                d.status = false;
                d.message = "already exists";
                if (ex.InnerException != null && ex.InnerException.InnerException != null)
                    d.data = ex.InnerException.InnerException.Message;
                else
                    d.data = ex.Message;
                return d;
            }
        }

        // it is for keywords when user enter 3 characters then this api check into our table and return keywords of list
        [Authorize]
        [HttpGet]
        [Route("AutoComplete")]
        public Object AutoComplete(string word)
        {
            var k = db.keywords.Where(x => x.keyword1.Contains(word)).Select(x => new { name = x.keyword1, id = x.id }).ToList();
            return k;
        }

        [Authorize]
        [HttpGet]
        [Route("GetLabReportByID")]
        public List<lab_reports> GetLabReportByID(long appointment_id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            return db.lab_reports.Where(x => x.appointment_id == appointment_id).ToList();
        }

        [Authorize]
        [HttpGet]
        [Route("CancelAppointment")]
        public Data CancelAppointment(long appointment_id)
        {
            Data d = new Data();
            var identity = User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity.Claims;
            string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;
            long userid = Convert.ToInt32(user_id);
            appointment appointment = db.appointments.Where(x => x.id == appointment_id && x.user_id == userid).FirstOrDefault();
            if (appointment != null)
            {
                if (appointment.is_completed == false)
                {
                    if (appointment.no_samples == 0)
                    {
                        appointment.user_id = null;
                        appointment.is_booked = false;
                        appointment.updated_at = DateTime.Now;
                        db.Entry(appointment).State = EntityState.Modified;
                        db.SaveChanges();

                        d.message = "تم إلغاء موعدك";
                        d.status = true;
                        d.data = null;

                        
                        user u = db.users.Where(x => x.id == userid).FirstOrDefault();

                        Generic g = new Generic();
                        g.sendNotificationOnChats(u.name + " " + u.last_name, "تم إلغاء موعدك", u.phone, "lab_appointment_cancelled", appointment.lab_id.ToString());

                        return d;
                    }
                    else
                    {
                        d.message = "لايمكن إلغاء الموعد يتم فحص العينات المستلمة";
                        d.status = true;
                        d.data = null;
                        return d;
                    }
                }
                else
                {
                    d.message = "لايمكن إلغاء الموعد تم الإنتهاء من تحليل العينة";
                    d.status = true;
                    d.data = null;
                    return d;
                }
            }
            else
            {
                d.message = "لم يتم العثور على موعد مقابل معرّف الموعد هذا!";
                d.status = true;
                d.data = null;
                return d;
            }
        }

        [Authorize]
        [HttpGet]
        [Route("GetLabReport")]
        public Data GetLabReport(long appointment_id)
        {
            Data d = new Data();

            var identity = User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity.Claims;
            string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;
            long userid = Convert.ToInt32(user_id);

            db.Configuration.ProxyCreationEnabled = false;
            var query = from lr in db.lab_reports
                        join l in db.labs on lr.lab_id equals l.id
                        select new
                        {
                            lr,
                            l
                        };

            //lab_reports lr = db.lab_reports.Where(x => x.appointment_id == appointment_id && x.user_id == userid).FirstOrDefault();
            if (query != null)
            {
                d.message = "successfully fetch the records";
            }
            else
            {
                d.message = "no data found against this appointment id";
            }
            d.data = query.FirstOrDefault();
            d.status = true;
            return d;
        }

        [Authorize]
        [HttpGet]
        [Route("GetMyLabAppointment")]
        public Object GetMyLabAppointment()
        {
            var identity = User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity.Claims;
            string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;
            long userid = Convert.ToInt32(user_id);
            return db.fun_GetAllMyLabAppointment(userid);
        }


        [Authorize]
        [HttpGet]
        [Route("GetAllAppointmentByUser")]
        public Data GetAllAppointmentByUser()
        {
            Data d = new Data();
            var identity = User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity.Claims;
            string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;
            long userid = Convert.ToInt32(user_id);
            List<appointment> app = new List<appointment>();
            db.Configuration.ProxyCreationEnabled = false;
            if (db.appointments.Where(x => x.user_id == userid).Count() > 0)
            {
                app = db.appointments.Where(x => x.user_id == userid).ToList();
                d.message = "successfully fetch the appointment data";
            }
            else
            {
                d.message = "no appointment data against this user";
            }

            d.status = true;
            d.data = app;
            return d;
        }

        [HttpGet]
        [Route("Dashboard")]
        public Object dashboard()
        {

            var dashboard = new DashboardData()
            {
                firstSection = new Section()
                {
                    title = db.categories.Where(x => x.id == 1).FirstOrDefault().description,
                    type = "category",
                    items = db.categories.Where(x => x.parent_category == 1 && x.active == true)
                            .Select(p => new DashboardItems()
                            {
                                id = p.id,
                                name_ar = p.name,
                                desc_ar = p.description,
                                icon = p.icon,
                                bg_color = p.bg_color,
                                text_color = p.text_color,
                                banner_ar = p.banner
                            }).ToList()
                },
                secondSection = new Section_Article()
                {
                    title = "الأخبار",
                    type = "article",
                    item = db.categories.Where(x => x.name == "الأخبار" && x.active == true).Select(p => new DashboardItems()
                    {
                        id = p.id,
                        name_ar = p.name,
                        desc_ar = p.description,
                        icon = p.icon,
                        bg_color = p.bg_color,
                        text_color = p.text_color,
                        banner_ar = p.banner
                    }).FirstOrDefault(),
                    items = db.articles.Where(x => x.active == true).OrderByDescending(x => x.id).Take(5).Select(p => new Article()
                    {
                        id = p.id,
                        title_ar = p.title,
                        desc_ar = p.short_description,
                        image = p.image,
                        created_at = p.created_at
                    }).ToList()
                },
                thirdSection = new Section()
                {
                    title = "طلب إستشارة",
                    type = "consultation",
                    items = db.categories.Where(x => x.name == "طلب إستشارة" && x.active == true).Select(p => new DashboardItems()
                    {
                        id = p.id,
                        name_ar = p.name,
                        desc_ar = p.description,
                        icon = p.icon,
                        bg_color = p.bg_color,
                        text_color = p.text_color,
                        banner_ar = p.banner
                    }).ToList()
                },
                fourthSection = new fourthSection()
                {
                    articles = new Section()
                    {
                        title = "الإرشاد الزراعي",
                        type = "articles",
                        items = db.categories.Where(x => x.name == "الإرشاد الزراعي" && x.active == true).Select(p => new DashboardItems()
                        {
                            id = p.id,
                            name_ar = p.name,
                            desc_ar = p.description,
                            icon = p.icon,
                            bg_color = p.bg_color,
                            text_color = p.text_color,
                            banner_ar = p.banner
                        }).ToList()
                    },
                    Calendar = new Section()
                    {
                        title = "التقويم الزراعي",
                        type = "calendar",
                        items = db.categories.Where(x => x.name == "التقويم الزراعي" && x.active == true).Select(p => new DashboardItems()
                        {
                            id = p.id,
                            name_ar = p.name,
                            desc_ar = p.description,
                            icon = p.icon,
                            bg_color = p.bg_color,
                            text_color = p.text_color,
                            banner_ar = p.banner
                        }).ToList()
                    }
                },
                fifthSection = new Section_Article()
                {
                    title = "الندوات و ورش العمل",
                    type = "article",
                    item = db.categories.Where(x => x.name == "الندوات و ورش العمل" && x.active == true).Select(p => new DashboardItems()
                    {
                        id = p.id,
                        name_ar = p.name,
                        desc_ar = p.description,
                        icon = p.icon,
                        bg_color = p.bg_color,
                        text_color = p.text_color,
                        banner_ar = p.banner
                    }).FirstOrDefault(),
                    items = db.articles.Where(x => x.category_id == 34 && x.active == true).OrderByDescending(x => x.id).Take(5).Select(p => new Article()
                    {
                        id = p.id,
                        title_ar = p.title,
                        desc_ar = p.short_description,
                        image = p.image,
                        created_at = p.created_at
                    }).ToList()
                },
                sixthSection = new Section_Article()
                {
                    title = "المنتجات الزراعية",
                    type = "article",
                    item = db.categories.Where(x => x.name == "المنتجات الزراعية" && x.active == true).Select(p => new DashboardItems()
                    {
                        id = p.id,
                        name_ar = p.name,
                        desc_ar = p.description,
                        icon = p.icon,
                        bg_color = p.bg_color,
                        text_color = p.text_color,
                        banner_ar = p.banner
                    }).FirstOrDefault(),
                    items = db.articles.Where(x => x.category_id == 33 && x.active == true).OrderByDescending(x => x.id).Take(5).Select(p => new Article()
                    {
                        id = p.id,
                        title_ar = p.title,
                        desc_ar = p.short_description,
                        image = p.image,
                        created_at = p.created_at
                    }).ToList()
                },
                seventhSection = new Section()
                {
                    title = "التجارب الناجحة",
                    type = "category",
                    items = db.categories.Where(x => x.name == "التجارب الناجحة" && x.active == true).Select(p => new DashboardItems()
                    {
                        id = p.id,
                        name_ar = p.name,
                        desc_ar = p.description,
                        icon = p.icon,
                        bg_color = p.bg_color,
                        text_color = p.text_color,
                        banner_ar = p.banner
                    }).ToList()
                },
                eightSection = new Section_Category()
                {
                    title = "أسئلة وأجوبة إرشادية",
                    type = "category",
                    item = db.categories.Where(x => x.name == "أسئلة وأجوبة إرشادية" && x.active == true).Select(p => new DashboardItems()
                    {
                        id = p.id,
                        name_ar = p.name,
                        desc_ar = p.description,
                        icon = p.icon,
                        bg_color = p.bg_color,
                        text_color = p.text_color,
                        banner_ar = p.banner
                    }).FirstOrDefault(),
                    items = db.categories.Where(x => x.parent_category == 30 && x.active == true).Select(p => new DashboardItems()
                    {
                        id = p.id,
                        name_ar = p.name,
                        desc_ar = p.description,
                        icon = p.icon,
                        bg_color = p.bg_color,
                        text_color = p.text_color,
                        banner_ar = p.banner
                    }).ToList()
                },
                ninthSection = new Section()
                {
                    title = "المكتبة الرقمية",
                    type = "category",
                    items = db.categories.Where(x => x.name == "المكتبة الرقمية" && x.active == true).Select(p => new DashboardItems()
                    {
                        id = p.id,
                        name_ar = p.name,
                        desc_ar = p.description,
                        icon = p.icon,
                        bg_color = p.bg_color,
                        text_color = p.text_color,
                        banner_ar = p.banner
                    }).ToList()
                },
                tenthSection = new LiveStream()
                {
                    type = "livestream",
                    title = "Live Stream",
                    enabled = "on",
                    banner = db.app_settings.Where(x => x.ap_key == "AP_STREAM_BANNER").FirstOrDefault().ap_value,
                    //"categories/hxOzuT04fKwv4FFJYbHEJroqTGxpXAQBaWxytRlk.gif",
                    url = db.app_settings.Where(x => x.ap_key == "AP_STREAM_URL").FirstOrDefault().ap_value
                    //"https://www.pscp.tv/w/clxGWTFkcmplTFJHbmtwS2J8MU1ZeE5WZ1ZaYnd4d-4-V1KUgo8wtnXfVi_oGX4opNR5r67BE-mUtPqeUP9h"
                },
                elevethSection = new DashboadStatistics()
                {
                    totalconsultants = "250",
                    totalfarmers = "62451",
                    totalmessages = "3254",
                    totalusers = "123652"
                },
                contactUsLinks = new ContactUs()
                {
                    AP_SYSTEM_EMAIL = "info@rose76.com",
                    AP_CUSTOMER_SUPPORT_WA_NUMBER = "+966566793327",
                    AP_CUSTOMER_SUPPORT_NUMBER = "+966596472727",
                    AP_FACEBOOK_URL = "saatco_it",
                    AP_TWITTER_URL = "agri_ext",
                    AP_YOUTUBE_URL = "saatco_it",
                    AP_INSTA_URL = "saatco_it",
                    AP_SNAP_URL = "saatco_it",
                },
                skills = new SkillItem()
                {
                    items = db.skills.Where(x => x.parent_skill == null && x.active == true).Select(p => new Skills()
                    {
                        id = p.id,
                        name_ar = p.name,
                        menu_sort = p.menu_sort,
                        description = p.description,
                        icon = p.icon,
                        banner = p.banner,
                        text_color = p.text_color,
                        bg_color = p.bg_color
                    }).ToList()

                },
                groups = new GroupItem()
                {
                    items = db.groups.Select(p => new Groups()
                    {
                        id = p.id,
                        region_id = p.region_id,
                        name_ar = p.name
                        //,
                        //region = db.regions.Where(x => x.id == p.region_id).Select(q => new region()
                        //{
                        //    id = q.id,
                        //    name_ar = q.name
                        //}).FirstOrDefault()
                    }).ToList()
                },
                regions = new RegionItem()
                {
                    items = db.regions.Where(x => x.active == true).Select(p => new Region()
                    {
                        id = p.id,
                        name_ar = p.name_ar
                    }).ToList()
                }

            };

            dashboard.skills.items[0].children = new List<Skills>();

            dashboard.skills.items[0].children.AddRange(db.skills.Where(x => x.parent_skill == 1 && x.active == true).Select(q => new Skills()
            {
                id = q.id,
                name_ar = q.name,
                menu_sort = 0,
                description = q.description,
                icon = q.icon,
                banner = q.banner,
                text_color = q.text_color,
                bg_color = q.bg_color
            }).ToList());

            for (int i = 0; i < dashboard.groups.items.Count(); i++)
            {
                long region_id = dashboard.groups.items[i].region_id;
                dashboard.groups.items[i].region = new Region();
                dashboard.groups.items[i].region.id = db.regions.Where(x => x.id == region_id && x.active == true).FirstOrDefault().id;
                dashboard.groups.items[i].region.name_ar = db.regions.Where(x => x.id == region_id && x.active == true).FirstOrDefault().name;
            }

            var arr = new ArrayList();
            arr.Add(dashboard);
            return arr.ToArray();

        }

        // shows list of QA category names
        [Authorize]
        [HttpGet]
        [Route("Get_QA_Category")]
        public Data Get_QA_Category()
        {
            Data d = new Data();
            if (User.Identity.IsAuthenticated)
            {
                db.Configuration.ProxyCreationEnabled = false;
                var category = db.qa_category.ToList();
                d.status = true;
                d.message = "successfully fetch the records";
                d.data = category;
                return d;
            }
            d.status = false;
            d.message = "user is not authenticated!";
            d.data = null;
            return d;
        }

        [Authorize]
        [HttpGet]
        [Route("InsertUserSkill")]
        public Data InsertUserSkill(long skill_id)
        {
            Data d = new Data();
            List<skill_user> list = new List<skill_user>();
            var identity = User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity.Claims;
            string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;
            long uid = Convert.ToInt32(user_id);

            skill_user su = new skill_user();
            su.user_id = uid;
            su.skill_id = skill_id;
            su.created_at = DateTime.Now;
            db.skill_user.Add(su);
            db.SaveChanges();

            d.status = true;
            d.message = "successfully inserted!";
            d.data = null;
            return d;
        }

        [Authorize]
        [HttpGet]
        [Route("GetUserSkills")]
        public List<SkillUser> GetUserSkills()
        {
            List<skill_user> list = new List<skill_user>();
            var identity = User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity.Claims;
            string user_id = claims.Where(p => p.Type == "userid").FirstOrDefault()?.Value;
            long uid = Convert.ToInt32(user_id);
            List<SkillUser> su = db.skill_user.Where(x => x.user_id == uid).Select(p => new SkillUser 
            {
                skill_id = p.skill_id, 
                name = p.skill.name, 
                is_approved = p.is_approved
            }).ToList();

            return su;
        }

        // based on productid, marketid and return market products baesd on filter
        [HttpGet]
        [Route("GetProductPriceFor")]
        public List<ProductPrice> GetMarketProduct(long product_id, long market_id, int filter = 0)
        {
            List<ProductPrice> pp = new List<ProductPrice>();
            if (filter == 360)
            {
                int mid = Convert.ToInt16(market_id);
                List<fun_GetLatestProductPricesbyYear_Result> price_product = db.fun_GetLatestProductPricesbyYear(mid, product_id).ToList();
                if (price_product.Count() > 0)
                {
                    for (int i = 0; i < price_product.Count; i++)
                    {
                        ProductPrice price = new ProductPrice();
                        price.product_id = product_id;
                        price.price = price_product[i].price;
                        price.price_date = price_product[i].price_date.Value.FormattedDate();
                        pp.Add(price);
                    }
                }
            }
            else
            {
                var price_product = db.market_products.Where(x => x.product_id == product_id && x.market_id == market_id).OrderByDescending(x => x.id).Take(filter).ToList();
                {
                    for (int i = 0; i < price_product.Count; i++)
                    {
                        ProductPrice price = new ProductPrice();
                        price.product_id = product_id;
                        price.price = price_product[i].price;
                        price.price_date = price_product[i].price_date.Value.FormattedDate();
                        pp.Add(price);
                    }
                }
            }
            
            return pp;
        }

        [HttpGet]
        [Route("GetAllMarkets")]
        public Data GetAllMarkets(int regionId = 0)
        {
            Data d = new Data();
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                if (regionId > 0)
                {
                    var markets = db.markets.Where(x => x.region_id == regionId && x.is_active == true).Include(x => x.region).ToList();

                    List<MarketData> md = new List<MarketData>();
                    if (markets != null)
                    {
                        for (int i = 0; i < markets.Count; i++)
                        {
                            MarketData m = new MarketData();
                            m.id = markets[i].id;
                            m.marketname = markets[i].marketname;
                            m.location = markets[i].location;
                            m.address = markets[i].address;
                            m.open_at = markets[i].open_at;
                            m.close_at = markets[i].close_at;
                            m.contact_person = markets[i].contact_person;
                            m.phone = markets[i].phone;
                            m.is_active = markets[i].is_active;
                            m.region_Name = markets[i].region.name;
                            m.region_id = markets[i].region_id.Value;
                            m.market_image = markets[i].market_image;
                            md.Add(m);
                        }
                    }
                    d.status = true;
                    d.message = "successfully fetch single record!";
                    d.data = md;
                    return d;
                }
                else
                {
                    var markets = db.markets.Where(x => x.is_active == true).Include(x => x.region).ToList();

                    List<MarketData> md = new List<MarketData>();
                    if (markets != null)
                    {
                        for (int i = 0; i < markets.Count; i++)
                        {
                            MarketData m = new MarketData();
                            m.id = markets[i].id;
                            m.marketname = markets[i].marketname;
                            m.location = markets[i].location;
                            m.address = markets[i].address;
                            m.open_at = markets[i].open_at;
                            m.close_at = markets[i].close_at;
                            m.contact_person = markets[i].contact_person;
                            m.phone = markets[i].phone;
                            m.is_active = markets[i].is_active;
                            m.region_Name = markets[i].region.name;
                            m.region_id = markets[i].region_id.Value;
                            m.market_image = markets[i].market_image;
                            md.Add(m);
                        }
                    }
                    d.status = true;
                    d.message = "successfully fetch single record!";
                    d.data = md;
                    return d;
                }
            }
            else
            {
                d.status = false;
                d.message = "User not authorized!";
                d.data = null;
                return d;
            }
        }

        [HttpGet]
        [Route("chatbot")]
        public List<chatbotDTO> chatbot(string val)
        {
            List<chatbotDTO> chatbot = new List<chatbotDTO>();
            List<article> art = db.articles.Where(x => x.active == true && x.description.Contains(val) || x.title.Contains(val) || x.keywords.Contains(val)).Take(5).ToList();
            if (art != null && art.Count() == 0)
            {
                List<qa_questions> que = db.qa_questions.Where(x => x.is_approved == true && x.is_verified == true && (x.keywords.Contains(val) || x.description.Contains(val) || x.title.Contains(val))).Take(5).ToList();
                if (que != null && que.Count() == 0)
                {
                    List<user> u = db.users.Where(x => x.is_approved == true && x.role_id == 6).Take(5).ToList();
                    List<long> u_id = new List<long>();
                    foreach (var item in u)
                    {
                        skill_user su = db.skill_user.Where(x => x.is_approved == true && x.user_id == item.id && x.skill.name.Contains(val)).FirstOrDefault();
                        if (su != null)
                        {
                            u_id.Add(item.id);
                        }
                    }

                    if (u_id.Count() > 0)
                    {
                        for (int i = 0; i < u_id.Count(); i++)
                        {
                            chatbotDTO cbd = new chatbotDTO();
                            cbd.id = u_id[i];
                            user consultant = db.users.Find(u_id[i]);

                            cbd.image = consultant.avatar;
                            cbd.description = consultant.name + " " + consultant.last_name;
                            cbd.type = 3;
                            chatbot.Add(cbd);
                        }
                        return chatbot;
                    }
                }
                else
                {
                    for (int i = 0; i < que.Count(); i++)
                    {
                        chatbotDTO cbd = new chatbotDTO();
                        cbd.id = que[i].id;
                        cbd.description = que[i].description;
                        long? media_id = db.qa_attachments.Where(x => x.type_id == que[i].id && x.type == false).FirstOrDefault().media_id;
                        if (media_id != null)
                        {
                            cbd.image = db.media.Find(media_id).file_name;
                        }
                        else
                        {
                            cbd.image = null;
                        }
                        cbd.type = 2;
                        chatbot.Add(cbd);
                    }
                    return chatbot;
                }
            }
            else
            {
                for (int i = 0; i < art.Count(); i++)
                {
                    chatbotDTO cbd = new chatbotDTO();
                    cbd.id = art[i].id;
                    cbd.description = art[i].title;
                    cbd.image = art[i].image;
                    cbd.type = 1;
                    chatbot.Add(cbd);
                }
                return chatbot;
            }
            return chatbot;
        }

        [HttpGet]
        [Route("GetMarketDetails")]
        public Data GetMarketDetails(int market_id)
        {
            Data d = new Data();
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var markets = db.markets.Where(x => x.id == market_id).Include(x => x.region).FirstOrDefault();
                MarketProduct _marketproduct = new MarketProduct();
                MarketData m = new MarketData();
                if (markets != null)
                {
                    m.id = markets.id;
                    m.marketname = markets.marketname;
                    m.location = markets.location;
                    m.address = markets.address;
                    m.open_at = markets.open_at;
                    m.close_at = markets.close_at;
                    m.contact_person = markets.contact_person;
                    m.phone = markets.phone;
                    m.is_active = markets.is_active;
                    m.region_Name = markets.region.name;
                    m.market_image = markets.market_image;

                    List<market_products> mp = new List<market_products>();
                    List<ProductPrice> productPrices = new List<ProductPrice>();

                    List<fun_GetLatestProductPrices_Result> productprices = db.fun_GetLatestProductPrices(market_id).ToList();
                    if (productprices.Count() <= 0)
                    {
                        _marketproduct.product_price = null;
                        _marketproduct.products = null;
                        _marketproduct.market = m;

                        d.status = true;
                        d.message = "Successfully fetch the records!";
                        d.data = _marketproduct;
                        return d;
                    }
                    long pid = productprices.First().id;
                    mp = db.market_products.Where(x => x.product_id == pid).OrderByDescending(x => x.id).Take(7).ToList();

                    foreach (var item in mp)
                    {
                        ProductPrice pp = new ProductPrice();
                        pp.product_id = pid;
                        pp.price = item.price;
                        pp.price_date = item.price_date.Value.FormattedDate();
                        productPrices.Add(pp);
                    }

                    _marketproduct.product_price = productPrices;
                    _marketproduct.products = productprices;
                    _marketproduct.market = m;
                }

                d.status = true;
                d.message = "Successfully fetch the records!";
                d.data = _marketproduct;
                return d;

            }

            d.status = false;
            d.message = "User is not authenticated!";
            d.data = null;
            return d;
        }

        [Route("getWeatherDataForCity/{cid}")]
        [HttpGet]
        public Object getWeatherDataForCity(string cid)
        {
            var wde = db.WeatherDatas.Where(wd => wd.city_identifier == cid).OrderByDescending(x => x.id).Select(w => w.weather).FirstOrDefault();
            if (wde != null)
            {
                return JsonConvert.DeserializeObject<WeatherJson>(wde);
            }

            return NotFound();
        }


        [HttpGet]
        [Route("getRegionWithCities")]
        public Object getRegionData()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var regions = db.regions.Where(x => x.active == true)
                        .Include(region => region.cities)
                        .ToList();

            SkillItem skills = new SkillItem()
            {
                items = db.skills.Where(x => x.parent_skill == null && x.active == true).Select(p => new Skills()
                {
                    id = p.id,
                    name_ar = p.name,
                    menu_sort = p.menu_sort,
                    description = p.description,
                    icon = p.icon,
                    banner = p.banner,
                    text_color = p.text_color,
                    bg_color = p.bg_color
                }).ToList()

            };

            for (int i = 0; i < skills.items.Count(); i++)
            {
                if (db.skills.Where(x => x.parent_skill == skills.items[i].id && x.active == true) != null)
                {
                    skills.items[i].children = new List<Skills>();
                    int p_id = Convert.ToInt16(skills.items[i].id);
                    skills.items[i].children.AddRange(db.skills.Where(x => x.parent_skill == p_id && x.active == true).Select(q => new Skills()
                    {
                        id = q.id,
                        name_ar = q.name,
                        menu_sort = 0,
                        description = q.description,
                        icon = q.icon,
                        banner = q.banner,
                        text_color = q.text_color,
                        bg_color = q.bg_color
                    }).ToList());
                }

                for (int y = 0; y < skills.items[i].children.Count(); y++)
                {
                    if (db.skills.Where(x => x.parent_skill == skills.items[i].children[y].id && x.active == true) != null)
                    {
                        skills.items[i].children[y].children = new List<Skills>();
                        int y_id = Convert.ToInt16(skills.items[i].children[y].id);
                        skills.items[i].children[y].children.AddRange(db.skills.Where(x => x.parent_skill == y_id && x.active == true).Select(q => new Skills()
                        {
                            id = q.id,
                            name_ar = q.name,
                            menu_sort = 0,
                            description = q.description,
                            icon = q.icon,
                            banner = q.banner,
                            text_color = q.text_color,
                            bg_color = q.bg_color
                        }).ToList());
                    }
                }
            }

            

            return new { regions, skills = skills.items };
        }
        [HttpGet]
        [Route("productPriceInMarkets")]
		public Data ProductPrice(long productId)
		{
            Data result = new Data();
            if(db.market_products.Any(a=>a.product_id == productId))
            {
            var data = db.market_products.Where(w => w.product_id == productId & w.market.is_active==true &w.price>0)
                .Select(s => new produc_Prices
                {
                    MarketName = s.market.marketname,
                    Price = s.price.ToString(),
                    Unit_Value =s.unit_value,
                    Unite = s.product.unit.name,
                    ProductName = s.product.product_name,
                    MarketId = s.market_id,
                    date = s.created_at
                }).ToList().OrderByDescending(aa => aa.date).GroupBy(g => g.MarketId).Select(sg => sg.FirstOrDefault());
                if (data.Count()==0)
                {
                    result.status = true;
					result.message = "NotFound";
					result.data = "".ToArray();
					return result;

                }
				result.status = true;
				result.message = "Success";
				result.data = data;
				return result;

			}
            result.status = false;
            result.message = "NotFound";
            result.data = "".ToArray();
			return result;
		}


	}



    #region classes

    public class produc_Prices
    {
        public string Price { get; set; }
        public string ProductName { get; set; }
        public string MarketName { get; set; }
        public string Unit_Value { get; set; }
        public string Unite { get; set; }
        public long? MarketId { get; set; }
        public DateTime? date { get; set; }

	}
    public class RegionDTO
    {

        private ICollection<city> _dbcities;
        public long id { get; set; }
        public string name { get; set; }
        public string name_ar { get; set; }
        public List<CityDTO> cities { get; set; }

        public ICollection<city> dbcities
        {
            get
            {
                return _dbcities;
            }
            set
            {

                this.cities = new List<CityDTO>();

                foreach (var city in value)
                {
                    this.cities.Add(new CityDTO()
                    {
                        id = city.id,
                        name = city.name_en,
                        name_ar = city.name_ar,
                        weather_identifier = city.weather_identifier

                    });
                };
            }
        }
    }

    public class CityDTO
    {

        public long id { get; set; }
        public long region_id { get; set; }
        public string name { get; set; }
        public string name_ar { get; set; }
        public string weather_identifier { get; set; }

    }

    public class ProductDetailForDashboard
    {
        public long id { get; set; }
        public string product_name { get; set; }
        public string product_image { get; set; }
        public string product_category { get; set; }
        public string product_type { get; set; }
        public string product_origin { get; set; }
        public string unit_name { get; set; }
        public string unit_value { get; set; }
        public bool is_active { get; set; }
        public decimal? product_price { get; set; }
    }

    public class ProductDetail
    {
        public long id { get; set; }
        public string product_name { get; set; }
        public string product_image { get; set; }
        public string product_category { get; set; }
        public string product_type { get; set; }
        public string product_origin { get; set; }
        public string unit_name { get; set; }
        public string unit_value { get; set; }
        public bool is_active { get; set; }
        public List<ProductPrice> product_price { get; set; }
    }

    public class SkillUser
    {
        public long skill_id { get; set; }
        public string name { get; set; }
        public bool is_approved { get; set; }
    }

    public class ChatDetails
    {
        public long id { get; set; }
        public long role_id { get; set; }
        public string last_message { get; set; }
        public int status { get; set; }
        public DateTime created_at { get; set; }
        public string avatar { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public bool active { get; set; }
        public string chatId { get; set; }
        public decimal rating { get; set; }
        public bool is_online { get; set; }
        public int message_type { get; set; }
    }

    public class chatbotDTO
    {
        public long id { get; set; }
        public string description { get; set; }
        public string image { get; set; }
        public int type { get; set; }
    }

    public class ProductPrice
    {
        public long product_id { get; set; }
        public decimal? price { get; set; }
        public string price_date { get; set; }
    }

    public class MarketProduct
    {
        public MarketData market { get; set; }
        public List<fun_GetLatestProductPrices_Result> products { get; set; }
        public List<ProductPrice> product_price { get; set; }
    }

    public class MarketData
    {
        public long id { get; set; }
        public string marketname { get; set; }
        public long region_id { get; set; }
        public string location { get; set; }
        public string address { get; set; }
        public TimeSpan? open_at { get; set; }
        public TimeSpan? close_at { get; set; }
        public string contact_person { get; set; }
        public string phone { get; set; }
        public bool is_active { get; set; }
        public string region_Name { get; set; }
        public string market_image { get; set; }
        //public ProductDetail product_Detail { get; set; }
    }

    public class DashboardData
    {
        public Section firstSection { get; set; }
        public Section_Article secondSection { get; set; }
        public Section thirdSection { get; set; }
        public fourthSection fourthSection { get; set; }
        public Section_Article fifthSection { get; set; }
        public Section_Article sixthSection { get; set; }
        public Section seventhSection { get; set; }
        public Section_Category eightSection { get; set; }
        public Section ninthSection { get; set; }
        public LiveStream tenthSection { get; set; }
        public DashboadStatistics elevethSection { get; set; }
        public ContactUs contactUsLinks { get; set; }
        public SkillItem skills { get; set; }
        public GroupItem groups { get; set; }
        public RegionItem regions { get; set; }
        public List<UserDetail> consultants { get; set; }
        public List<UserDetail> farmers { get; set; }
        public List<UserDetail> appUsers { get; set; }
        //public user 
    }

    public class DashboadStatistics
    {
        public string totalfarmers { get; set; }
        public string totalusers { get; set; }
        public string totalconsultants { get; set; }
        public string totalmessages { get; set; }
    }

    public class Section
    {
        public string type { get; set; }
        public string title { get; set; }
        public List<DashboardItems> items { get; set; }
    }

    public class fourthSection
    {
        public Section articles { get; set; }
        public Section Calendar { get; set; }
    }

    public class LiveStream
    {
        public string type { get; set; }
        public string title { get; set; }
        public string enabled { get; set; }
        public string banner { get; set; }
        public string url { get; set; }
    }

    public class Section_Category
    {
        public string type { get; set; }
        public string title { get; set; }
        public DashboardItems item { get; set; }
        public List<DashboardItems> items { get; set; }
    }
    public class Section_Article
    {
        public string type { get; set; }
        public string title { get; set; }
        public DashboardItems item { get; set; }
        public List<Article> items { get; set; }
    }

    public class Category
    {
        public long id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public Nullable<long> parent_category { get; set; }
        public string icon { get; set; }
        public string banner { get; set; }
        public string bg_color { get; set; }
        public string text_color { get; set; }
        public bool active { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
    }

    public class Article
    {
        private string _image;
        public long id { get; set; }
        public string title_ar { get; set; }
        public string desc_ar { get; set; }
        public string image { get; set;  }
        public DateTime? created_at { get; set; }
    }

    public class DashboardItems
    {
        private string _icon, _banner_ar;

        public long id { get; set; }
        public string name_ar { get; set; }
        public string desc_ar { get; set; }
        public string icon { get; set; }
        //public string icon { get; set; }
        public string bg_color { get; set; }
        public string text_color { get; set; }
        public string banner_ar { get; set; }
    }

    public class ContactUs
    {
        public string AP_SYSTEM_EMAIL { get; set; }
        public string AP_CUSTOMER_SUPPORT_WA_NUMBER { get; set; }
        public string AP_CUSTOMER_SUPPORT_NUMBER { get; set; }
        public string AP_FACEBOOK_URL { get; set; }
        public string AP_INSTA_URL { get; set; }
        public string AP_SNAP_URL { get; set; }
        public string AP_YOUTUBE_URL { get; set; }
        public string AP_TWITTER_URL { get; set; }
    }

    public class SkillItem
    {
        public List<Skills> items { get; set; }
    }

    public class Skills
    {
        public long id { get; set; }
        public string name_ar { get; set; }
        public long menu_sort { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
        public string banner { get; set; }
        public string bg_color { get; set; }
        public string text_color { get; set; }
        public List<Skills> children { get; set; }
    }

    public class GroupItem
    {
        public List<Groups> items { get; set; }
    }

    public class Groups
    {
        public long id { get; set; }
        public string name_ar { get; set; }
        public long region_id { get; set; }
        public Region region { get; set; }
    }

    public class OneTimePassword
    {
        public string OTP { get; set; }
    }

    public class RegionItem
    {
        public List<Region> items { get; set; }
    }

    public class Region
    {
        public long id { get; set; }
        public string name_ar { get; set; }
    }



    public class UserDetail
    {
        public int id { get; set; }
        public string phone { get; set; }
        public string skills { get; set; }
        public string prefix { get; set; }
        public string name { get; set; }
        public string last_name { get; set; }
        public string country { get; set; }
        public string region { get; set; }
        public float rating { get; set; }
        public int role_id { get; set; }
        public string linked_in { get; set; }
        public string profile { get; set; }
        public string avatar { get; set; }
        public string full_name { get; set; }
    }

    public class Data
    {
        public bool status { get; set; }
        public string message { get; set; }
        public object data { get; set; }
        public object page_info { get; set; }
    }


    public class FormData
    {
        public string title { get; set; }
        public HttpPostedFileBase upload_file { get; set; }
    }

    public class AppointmentDTO
    {
        public string appointment_time
        {
            get
            {
                TimeSpan ts = dt;
                DateTime time = DateTime.Today.Add(ts);
                string displayTime = time.ToString("hh:mm tt");
                return displayTime;
            }
        }
        public bool is_booked { get; set; }
        public TimeSpan dt { get; set; }
        public long id { get; set; }
    }

    public class StatsDTO
    {
        public string title { get; set; }
        public int total { get; set; }
        public string icon { get; set; }
        public string desc { get; set; }
        public string bgColor { get; set; }
    }
}

#endregion