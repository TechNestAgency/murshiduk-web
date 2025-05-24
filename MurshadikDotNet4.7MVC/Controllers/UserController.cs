using PagedList;
using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Net;

namespace MurshadikCP.Controllers
{
    [Authorize]
    public class UserController : BaseController
    {
        //private mlaraEntities db = new mlaraEntities();
        UserInfo ui = new UserInfo();
        // GET: User
        public ActionResult Index(int? Page_No, string msg = null)
        {
            if (currentPageID("User") > 0)
            {
                if (!CurrentUser.canView(currentPageID("User")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            int pagesize = 10;
            int No_Of_Page = (Page_No ?? 1);
            var users = db.GetAllUsers().Where(x => x.RoleName != "المدير العام");
            ViewBag.msg = "";
            if (msg != null && msg != "")
            {
                ViewBag.msg = msg;
            }
            return View(users.ToList().ToPagedList(No_Of_Page, pagesize));
        }
        public ActionResult Add()
        {
            if (currentPageID("User") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("User")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            ViewBag.role_id = new SelectList(db.roles, "id", "name_ar");
            return View();
        }

        public ActionResult UpdateUser(string Id)
        {
            if (currentPageID("User") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("User")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            ViewBag.Market = new SelectList(db.markets, "id", "marketname");
            ViewBag.Lab = new SelectList(db.labs, "id", "Name");

            user user = db.users.Where(x => x.user_id == Id).FirstOrDefault();

            if (user != null)
            {
                ViewBag.Role_id = new SelectList(db.roles.Where(x => x.id != 1 && x.id != 5 && x.id != 6 && x.id != 7), "id", "name_ar", user.role_id);
                ViewBag.region_id = new SelectList(db.regions.Where(x => x.active == true), "id", "name_ar", user.region_id);
                string multipleMarketValue = db.UserAccessTypeIdentifiers.Where(x => x.user_id == user.id).FirstOrDefault() != null ?
                    db.UserAccessTypeIdentifiers.Where(x => x.user_id == user.id).FirstOrDefault().type_value : null;
                if (multipleMarketValue != null && user.role_id == 8)
                {
                    ViewBag.Market_id = multipleMarketValue.Replace(",", "','");
                }

                return View(user);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        public ActionResult Register(string[] MarketID, int LabID = 0)
        {
            if (currentPageID("User") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("User")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            ViewBag.Market = new SelectList(db.markets, "id", "marketname");
            ViewBag.Lab = new SelectList(db.labs, "id", "Name");

            if (MarketID != null)
            {
                ViewBag.MarketID = MarketID;
                ViewBag.Role_id = new SelectList(db.roles.Where(x => x.id == (int)Role.Market), "id", "name_ar");
            }
            else if (LabID > 0)
            {
                ViewBag.LabID = LabID;
                ViewBag.Role_id = new SelectList(db.roles.Where(x => x.id == (int)Role.LabManager), "id", "name_ar");
            }
            else
            {
                ViewBag.Role_id = new SelectList(db.roles.Where(x => x.id != (int)Role.SuperAdmin && x.id != (int)Role.Farmers && x.id != (int)Role.Consultants && x.id != (int)Role.Technician), "id", "name_ar");
            }

            ViewBag.region_id = new SelectList(db.regions.Where(x => x.active == true), "id", "name_ar");
            RegisterViewModel model = new RegisterViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult RolePermission(int roleid, string[] permissionlist)
        {

            List<roles_permission> rp = new List<roles_permission>();

            foreach (var item in permissionlist)
            {
                roles_permission roles_Permission = new roles_permission();
                roles_Permission.role_id = roleid;
                string[] value = item.Split(',');
                string[] page = value[1].Split('-');
                int page_id = Convert.ToInt16(page[0].Trim().ToString());
                roles_Permission.page_id = page_id;

                roles_Permission.page_id = page_id;
                if (value[0] == "View")
                {
                    roles_Permission.can_view = true;
                }
                if (value[0] == "Insert")
                {
                    roles_Permission.can_insert = true;
                }
                if (value[0] == "Edit")
                {
                    roles_Permission.can_update = true;
                }
                if (value[0] == "Delete")
                {
                    roles_Permission.can_delete = true;
                }

                rp.Add(roles_Permission);

            }

            
            return Json("Success");
        }

        public ActionResult PermissionByRole()
        {
            ui = (UserInfo)Session["User"];
            if (ui != null)
            {
                List<roles_permission> roles = ui.roles_Permissions.ToList();
                if (roles != null)
                {
                    if (roles.Exists(x => x.Page.PageName == "User"))
                    {
                        if (roles.Find(x => x.Page.PageName == "User").can_insert)
                        {
                            ViewBag.roleid = 2;
                            var pages = db.Pages.ToList();
                            //var roles_permission = db.roles_permission.Where(x => x.role_id == 2).ToList();
                            //if (roles_permission.Count() == 35)
                            //{
                                
                            //}
                            
                            return View(pages);
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

        public ActionResult Profile(string id, int type = 0, string Error = null)
        {
            if (currentPageID("User") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("User")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            user u = db.users.Where(x => x.user_id == id).FirstOrDefault();
            if (u != null)
            {
                ChangePasswordViewModel cpv = new ChangePasswordViewModel();
                cpv.Mobile = u.phone;
                cpv.Name = u.name;
                if (type == 1)
                {
                    ViewBag.ResetPassword = "1";
                }
                else
                {
                    ViewBag.ResetPassword = "0";
                }

                if (Error != null)
                {
                    ViewBag.Error = Error;
                }

                return View(cpv);
            }
            else
            {
                return RedirectToAction("NotAllow", "Custom");
            }
        }

        public ActionResult UserEdit()
        {
            if (currentPageID("User") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("User")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            var users = db.GetAllUsers().FirstOrDefault();
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserEdit([Bind(Include = "id,name,phone,rolename,active")] GetAllUsers_Result user)
        {
            if (ModelState.IsValid)
            {
                user user1 = new user();
                user1.name = user.name;
                user1.phone = user.phone;
                //user1.role_id = user.RoleName;
                user1.active = user.active;
                db.users.Add(user1);
                //db.regions.Add(region);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Block(string id, string type)
        {
            if (currentPageID("User") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("User")))
                    return Json("Error");
            }
            
            user u = db.users.Where(x => x.user_id == id).FirstOrDefault();
            if (u != null)
            {
                if (type == "1")
                    u.active = false;
                else
                    u.active = true;
                db.Entry(u).State = EntityState.Modified;
                db.SaveChanges();

                return Json("Success");
            }
            else
            {
                return Json("Error");
            }
        }
    }
}