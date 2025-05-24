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
    public class workerextensionController : BaseController
    {
        // GET: workerextension
        public ActionResult Index(int? Page_No, string Search_Data, string Filter_Value, string skill, int Region = 0, int active = 0, int approved = 0, string start_date = null, string end_date = null, int skillApproval = 0,bool? isOnline=null)
        {
            if (currentPageID("workerextension") > 0)
            {
                if (!CurrentUser.canView(currentPageID("workerextension")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            int pagesize = 20;
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
            ViewBag.skill = new SelectList(db.skills, "name", "name");
            ViewBag.Filter_Value = Search_Data;
            ViewBag.Filter_region = Region;
            ViewBag.Filter_skill = skill;
            ViewBag.Filter_active = active;
            ViewBag.Filter_approved = approved;
            ViewBag.Filter_skillApproval = skillApproval;
            ViewBag.Filter_isOnline = isOnline;


            Search_Data = Search_Data != null ? Search_Data.Trim() : null;

            IQueryable<user> users = from u in db.users where u.role_id == (int)Role.Consultants orderby u.id descending select u;
            users = users.Include("skill_user");

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
            int totalconsultant = 0; int totalonline = 0; int totalPendingApproval = 0;
            if (CurrentUser.RoleId == (int)Role.RegionManager)
            {
                long region_id = db.users.Where(x => x.id == CurrentUser.Id).FirstOrDefault().region_id;
                users = users.Where(x => x.region_id == region_id);

                totalconsultant = db.users.Where(x => x.role_id == (int)Role.Consultants && x.region_id == region_id).Count();
                totalonline = db.users.Where(x => x.role_id == (int)Role.Consultants && x.region_id == region_id && x.is_approved == true && x.is_online == true).Count();
                totalPendingApproval = db.users.Where(x => x.role_id == (int)Role.Consultants && x.region_id == region_id && x.is_approved == false).Count();
                ViewBag.RegionManager = "Yes";
            }
            else
            {
                totalconsultant = db.users.Where(x => x.role_id == (int)Role.Consultants).Count();
                totalonline = db.users.Where(x => x.role_id == (int)Role.Consultants && x.is_approved == true && x.is_online == true).Count();
                totalPendingApproval = db.users.Where(x => x.role_id == (int)Role.Consultants && x.is_approved == false).Count();
                ViewBag.RegionManager = "No";
            }
            
            ViewBag.totalconsultant = totalconsultant;
            ViewBag.totalonline = totalonline;
            ViewBag.totalPendingApproval = totalPendingApproval;

           if (!String.IsNullOrEmpty(Search_Data))
            {

                users = users.Where(x => x.name.Contains(Search_Data) || x.last_name.Contains(Search_Data) || x.name + " " + x.last_name == Search_Data.Trim()
                || x.phone.Contains(Search_Data));
            }

            if (Region > 0)
            {
                users = users.Where(x => x.region_id == Region);
            }

            if (skill != null)

            {

				users = users.Where(x => x.skills != null && x.skills.Contains(skill));

				//var data = users.Where(x => x.skills.ToLower().Contains(filter));
				//users = users.Where(x => skill.Any(y=>y == x.skills));

			}

            if (active == 1)
            {
                users = users.Where(x => x.active == true);
            }
            else if (active == 2)
            {
                users = users.Where(x => x.active == false);
            }

            if (approved == 1)
            {
                users = users.Where(x => x.is_approved == true);
            }
            else if (approved == 2)
            {
                users = users.Where(x => x.is_approved == false);
            } 
            
            
            if (isOnline == true)
            {
                users = users.Where(x => x.is_online == true);
            }
            else if (isOnline == false)
            {
                users = users.Where(x => x.is_online == false);
            }

            if (skillApproval == 1)
            {
                users = users.Where(x => x.is_approved == true && x.skill_user.Where(y => y.is_approved == false).Any());
            }

            return View(users.OrderByDescending(x => x.created_at).ToPagedList(No_Of_Page, pagesize));
        }

        public ActionResult Chat(long id)
        {
            ParentChat pc = new ParentChat();
            pc.User = db.users.Find(id);
            pc.To_Chats = db.GetChatListByID(id).ToList();
            return View(pc);
        }

        public PartialViewResult RefillChatDetails(long from_id, long to_id)
        {
            var result = db.GetChatDetailsByUserID(from_id, to_id).ToList();
            return PartialView("_ChatDetails", result);
        }
        public ActionResult Profile(long id, int? Page_No, string start_date = null, string end_date = null)
        {
            if (currentPageID("workerextension") > 0)
            {
                if (!CurrentUser.canView(currentPageID("workerextension")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            int pagesize = 5;
            int No_Of_Page = (Page_No ?? 1);
            ParentUser pu = new ParentUser();
            ViewBag.id = id;
            
            
            
            
            
            
            DateTime sdt = DateTime.Now; DateTime edt = DateTime.Now;
            if (!string.IsNullOrEmpty(start_date))
            {
                ViewBag.Start_Date = start_date;
                sdt = DateTime.Parse(start_date, null, System.Globalization.DateTimeStyles.RoundtripKind);
                //users = users.Where(art => art.created_at >= sdt);
            }
            if (!string.IsNullOrEmpty(end_date))
            {
                ViewBag.End_Date = end_date;
                edt = DateTime.Parse(end_date, null, System.Globalization.DateTimeStyles.RoundtripKind);
                //users = users.Where(art => art.created_at <= edt);
            }



            //var userOnlineHours = db.get_totalOnlineHours(DateTime.Now.AddMonths(-5), edt, int.Parse(id.ToString()));
            var ConsultantsStatus = db.ConsultantsStatusLogs.Where(w => w.UsrId.Equals(id)).FirstOrDefault();

            if (ConsultantsStatus!=(null))
            {
				if (ConsultantsStatus.IsOnline)
				{
					ViewBag.LastSeen = "متصل";
				}
				else
				{
					ViewBag.LastSeen = ConsultantsStatus.StartTime.ToString();

				}


            }
            else
            {
				ViewBag.LastSeen = "__";
			}

            
           
            
            var onlineHours = db.ConsultantsDailyActiveHours.Where(w => w.UserId.Equals(id) && w.Date <= edt & w.Date >= sdt).Select(s => (long?)s.Hours).Sum()??0;
            
            if (onlineHours < 0)
			{
                ViewBag.onlinHours = 0;
			}
			else
			{
                 ViewBag.onlinHours = onlineHours;
			}
            if (!string.IsNullOrEmpty(start_date) && !string.IsNullOrEmpty(end_date))
            {
                pu.User = db.users.Find(id);
                ViewBag.Answers = pu.User.qa_answers.Where(x => x.created_at >= sdt && x.created_at <= edt).Count();
                ViewBag.Chatmessages = db.chatmessages.Where(x => x.message_from == id && x.created_at >= sdt && x.created_at <= edt).Count();
                ViewBag.callrejected = db.calldetails.Where(x => x.status == 3 && x.user_to == id && x.created_at >= sdt && x.created_at <= edt).Count();
                ViewBag.callnoanswer = db.calldetails.Where(x => x.status == 5 && x.user_to == id && x.created_at >= sdt && x.created_at <= edt).Count();
                ViewBag.callfinished = db.calldetails.Where(x => x.status == 6 && x.user_to == id && x.created_at >= sdt && x.created_at <= edt).Count();
                pu.consultant_Ratings = pu.User.consultant_rating.Where(x => x.created_at >= sdt && x.created_at <= edt).ToPagedList(No_Of_Page, pagesize);
            }
            else
            {
                pu.User = db.users.Find(id);
                ViewBag.Answers = pu.User.qa_answers.Count();
                ViewBag.Chatmessages = db.chatmessages.Where(x => x.message_from == id).Count();
                ViewBag.callrejected = db.calldetails.Where(x => x.status == 3 && x.user_to == id).Count();
                ViewBag.callnoanswer = db.calldetails.Where(x => x.status == 5 && x.user_to == id).Count();
                ViewBag.callfinished = db.calldetails.Where(x => x.status == 6 && x.user_to == id).Count();
                pu.consultant_Ratings = pu.User.consultant_rating.ToPagedList(No_Of_Page, pagesize);
            }

            
            return View(pu);
        }

        [HttpPost]
        public ActionResult changeProfession(long id, int role)
        {
            if (currentPageID("workerextension") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("workerextension")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            user u = db.users.Find(id);
            u.role_id = role;
            u.is_profile_completed = false;
            //u.is_approved = false;
            u.updated_at = DateTime.Now;
            u.skills = null;
            db.Entry(u).State = EntityState.Modified;
            db.SaveChanges();

            List<skill_user> su = db.skill_user.Where(x => x.user_id == id).ToList();
            if (su.Count() > 0)
            {
                db.skill_user.RemoveRange(su);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }


        public ActionResult unblock(long id)
        {
            if (currentPageID("workerextension") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("workerextension")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            user u = db.users.Find(id);
            u.is_approved = true;
            u.active = true;
            u.updated_at = DateTime.Now;
            db.Entry(u).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Edit", new { id = id });
        }
        public ActionResult Approved(long id)
        {
            if (currentPageID("workerextension") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("workerextension")))
                    return RedirectToAction("NotAllow", "Custom");
            }
            user u = db.users.Find(id);
            u.is_approved = true;
            u.active = true;
            u.updated_at = DateTime.Now;
            db.Entry(u).State = EntityState.Modified;
            db.SaveChanges();

            List<skill_user> su = db.skill_user.Where(x => x.user_id == id).ToList();
            if (su != null)
            {
                for (int i = 0; i < su.Count(); i++)
                {
                    su[i].is_approved = true;
                    su[i].updated_at = DateTime.Now;
                    db.Entry(su[i]).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            Generic generic = new Generic();
            generic.SendTopicNotification("وافق المستخدم", "تمت الموافقة عليك من قبل المسؤول", u.phone, "account_approved", u.phone);
            //generic.sendNotificationOnPriceUpdate("تمت الموافقة عليك من قبل المسؤول", u.name, u.phone);

            generic.InsertNotificationData(id, "تمت الموافقة عليك من قبل المسؤول", (Int16)AppConstants.Type.Account, id, "");
            //sendNotificationOnPriceUpdate()
            return RedirectToAction("Edit", new { id = id });

        }

        public ActionResult block(long id)
        {
            if (currentPageID("workerextension") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("workerextension")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            user u = db.users.Find(id);
            u.active = false;
            u.is_approved = false;
            u.is_online = false;
            u.updated_at = DateTime.Now;
            db.Entry(u).State = EntityState.Modified;
            db.SaveChanges();

            Generic generic = new Generic();
            generic.SendTopicNotification("المستخدم المحظور", "تم حظر حسابك بسبب نشاط مشبوه", u.phone, "account_blocked", u.phone);

            return RedirectToAction("Edit", new { id = id });

        }
        public ActionResult delete(long id)
        {
            if (currentPageID("workerextension") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("workerextension")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            user u = db.users.Find(id);
            u.active = false;
            u.updated_at = DateTime.Now;
            db.Entry(u).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Edit", new { id = id });
        }

        [HttpPost]
        public ActionResult UserSkill_Delete(long uid, long skill_id)
        {
            if (uid > 0 && skill_id > 0)
            {
                skill_user su = db.skill_user.Where(x => x.user_id == uid && x.skill_id == skill_id).FirstOrDefault();
                db.skill_user.Remove(su);
                db.SaveChanges();

                user u = db.users.Find(uid);
                u.skills = string.Join(",", db.skill_user.Where(y => y.user_id == uid).ToList().Select(y => y.skill.name));
                db.Entry(u).State = EntityState.Modified;
                db.SaveChanges();

                return Json("Success");
                
            }
            return Json("Error");
        }

        [HttpPost]
        public ActionResult UserApprovedSkill(long uid, long skill_id)
        {
            if (uid > 0 && skill_id > 0)
            {
                skill_user su = db.skill_user.Where(x => x.user_id == uid && x.skill_id == skill_id).FirstOrDefault();
                su.updated_at = DateTime.Now;
                su.is_approved = true;
                db.Entry(su).State = EntityState.Modified;
                db.SaveChanges();

                user u = db.users.Find(uid);
                u.skills = string.Join(",", db.skill_user.Where(x => x.user_id == u.id && x.is_approved == true).ToList().Select(q => q.skill.name));
                db.Entry(u).State = EntityState.Modified;
                db.SaveChanges();

                Generic generic = new Generic();
                generic.SendTopicNotification("تمت الموافقة على التخصص", "تمت الموافقة على " + su.skill.name + " الخاص بك!", u.phone, "skill_approved", u.phone);
                //generic.sendNotificationOnPriceUpdate("تمت الموافقة على [ " + su.skill.name +" ] الخاص بك!", u.name, u.phone);

                generic.InsertNotificationData(uid, "تمت الموافقة على التخصص [ " + su.skill.name + " ] ", (Int16)AppConstants.Type.Account, skill_id, "");

                return Json("Success");
            }
            return Json("Error");
        }

        [HttpPost]
        public ActionResult Edit(user u, string[] skills, int region)
        {
            if (currentPageID("workerextension") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("workerextension")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            user user = db.users.Find(u.id);
            user.profile = u.profile;
            user.skills = string.Join(",", db.skill_user.Where(x => x.user_id == u.id && x.is_approved == true).ToList().Select(q => q.skill.name));
            user.region = db.regions.Find(region).name_ar;
            user.region_id = region;
            user.updated_at = DateTime.Now;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(long id)
        {
            if (currentPageID("workerextension") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("workerextension")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            long[] checkRolesIDs = new long[] { 5, 6 };
            user u = db.users.Find(id);
            ViewBag.region = new SelectList(db.regions.ToList(), "id", "name_ar", u.region_id);
            ViewBag.role = new SelectList(db.roles.Where(x => checkRolesIDs.Contains(x.id)).ToList(), "id", "name_ar", u.role_id);
            ViewBag.skills = db.skill_user.Where(x => x.user_id == id).ToList();
            return View(u);

            

            //ui = (UserInfo)Session["User"];
            //if (ui != null)
            //{
            //    List<roles_permission> roles = ui.roles_Permissions.ToList();
            //    if (roles != null)
            //    {
            //        if (roles.Exists(x => x.Page.PageName == "workerextension"))
            //        {
            //            if (roles.Find(x => x.Page.PageName == "workerextension").can_update)
            //            {

            //                long[] checkRolesIDs = new long[] { 5, 6 };
            //                user u = db.users.Find(id);
            //                ViewBag.region = new SelectList(db.regions.ToList(), "id", "name_ar", u.region_id);
            //                ViewBag.role = new SelectList(db.roles.Where(x =>  checkRolesIDs.Contains(x.id)).ToList(), "id", "name_ar", u.role_id);
            //                ViewBag.skills = db.skill_user.Where(x => x.user_id == id).ToList();
            //                return View(u);
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    return RedirectToAction("Login", "Account");
            //}

            //return RedirectToAction("NotAllow", "Custom");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchConsultant(string q)
        {
            if (q.Length <= 3)
            {
                return Json(null);
            }

            var u = db.users
                            .Where(
                                user => user.role_id == 6 && (user.name.Contains(q) || user.last_name.Contains(q) || user.phone.Contains(q))
                                )
                            .Select(user => new { id = user.id, name = user.name + " - " + user.last_name, region_id = user.region_id })
                            .ToList();
            return Json(u);
        }
    }

    public class ParentUser
    {
        public IPagedList<consultant_rating> consultant_Ratings { get; set; }
        //public IPagedList<commentsByUser> commentsByUsers { get; set; }
        public user User { get; set; }
    }

    public class ParentChat
    {
        public user User { get; set; }
        public List<GetChatListByID_Result> To_Chats { get; set; }
    }

    public class ToUser
    {
        public long id { get; set; }
        public string fullname { get; set; }
        public string avatar { get; set; }
    }
}