using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using FirebaseAdmin.Messaging;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using MurshadikCP.Extensions;
using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using WebMatrix.WebData;
using System.Net;
using System.Data.Entity;

namespace MurshadikCP.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        
        UserInfo ui = new UserInfo();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private mlaraEntities db = new mlaraEntities();
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            ui = (UserInfo)Session["User"];
            if (ui != null)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // here pass model and it contains username and password
        // after successfully authenticate the user then fill the session and based of the role id then redirect to the controller page.
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            //ui = (UserInfo)Session["User"];
            //if (ui != null)
            //{
            //    return RedirectToAction("Index", "Dashboard");
            //}

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await SignInManager.PasswordSignInAsync(model.MobileNo, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    {
                        //var user = await SignInManager.UserManager.FindAsync(model.MobileNo, model.Password);
                        //string rolename = SignInManager.UserManager.GetRoles(user.Id).FirstOrDefault();
                        
                        UserInfo userInformation = new UserInfo();
                        user_access user_Access = new user_access();
                        Models.DB.UserAccessTypeIdentifier uati = new Models.DB.UserAccessTypeIdentifier();
                        user u = db.users.Where(x => x.phone == model.MobileNo).FirstOrDefault();

                        u.last_login_date = DateTime.Now;
                        db.Entry(u).State = EntityState.Modified;
                        db.SaveChanges();

                        if (u != null)
                        {
                            if (u.active == false)
                            {
                                ModelState.AddModelError("", "you are not active.");
                                return View(model);
                            }

                            uati = db.UserAccessTypeIdentifiers.Where(x => x.user_id == u.id).FirstOrDefault();
                            //user_Access = db.user_access.Where(x => x.userid == u.id).FirstOrDefault();

                            userInformation.Id = u.id;
                            userInformation.UserName = u.phone;
                            userInformation.RoleId = Convert.ToInt16(u.role_id);
                            userInformation.RoleName = u.role.name_en;
                            userInformation.region_id =u.region_id;
                            
                            userInformation.roles_Permissions = db.roles_permission.Where(x => x.role_id == u.role_id).OrderBy(x => x.Page.Sort).ToList();
                            Session["Menu"] = userInformation.roles_Permissions;
                        }
                        
                        bool a = User.Identity.IsAuthenticated;
                        if (userInformation.RoleId == (int)Role.LabManager)
                        {
                            if (uati != null)
                            {
                                userInformation.labid = uati != null ? Convert.ToInt32(uati.type_value) : 0;
                                Session["User"] = userInformation;
                                return RedirectToAction("Index", "Appointments", new { @id = userInformation.labid });
                            }
                        }
                        else if (userInformation.RoleId == (int)Role.Market)
                        {

                            if (uati != null)
                            {
                                int marketCount = uati.type_value.Split(',').Count();

                                userInformation.multiplemarketids = uati.type_value;
                                userInformation.marketid = Convert.ToInt16(uati.type_value.Split(',')[0]);

                                Session["User"] = userInformation;
                                if (marketCount > 1)
                                {
                                    return RedirectToAction("Index", "Markets");
                                }
                                else
                                {
                                    return RedirectToAction("View", "Markets", new { @id = userInformation.marketid });
                                }

                            }

                        }
                        else if (userInformation.RoleId == (int)Role.MarketAdmin)
                        {
                            Session["User"] = userInformation;
                            return RedirectToAction("Index", "Markets");
                        }
                        else if (userInformation.RoleId == (int)Role.RegionManager || userInformation.RoleId == (int)Role.AllRegionManager)
                        {
                            Session["User"] = userInformation;
                            return RedirectToAction("Index", "Workerextension");
                        }
                        else if (userInformation.RoleId == (int)Role.WeatherAdmin)
                        {
                            Session["User"] = userInformation;
                            return RedirectToAction("Index", "AlertMessages");
                        }
                        else if (userInformation.RoleId == (int)Role.ExtensionManager)
                        {
                            Session["User"] = userInformation;
                            return RedirectToAction("Index", "QA");
                        }
                        else if (userInformation.RoleId == (int)Role.VegetarianGuide || userInformation.RoleId == (int)Role.FishGuide || userInformation.RoleId == (int)Role.AnimalGuide)
                        {
                            Session["User"] = userInformation;
                            return RedirectToAction("Index", "VideoBoard");
                        }


                        Session["User"] = userInformation;

                        FormsAuthentication.SetAuthCookie(model.MobileNo, model.RememberMe);

                        return RedirectToLocal(returnUrl);
                    }
                    
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        // Get: for user edit
        public ActionResult UpdateUser(string Id)
        {
            ui = (UserInfo)Session["User"];
            if (ui != null)
            {
                List<roles_permission> roles = ui.roles_Permissions.ToList();
                if (roles != null)
                {
                    if (roles.Exists(x => x.Page.PageName == "User"))
                    {
                        if (roles.Find(x => x.Page.PageName == "User").can_update)
                        {
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
                                
                                //new SelectList(db.markets.Where(x => x.is_active == true), "marketname", )
                                //a.keywords = a.keywords != null ? a.keywords.Replace(",", "','") : "";
                                return View(user);
                            }
                            else
                            {
                                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                            }
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

        public ActionResult Profile(string id, int type = 0)
        {
            if (id == null)
            {
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
                return View(cpv);
            }
            else
            {
                return RedirectToAction("NotAllow", "Custom");
            }


        }

        // 
        // GET: /Account/Register
        //[AllowAnonymous]
        public ActionResult Register(string[] MarketID, int LabID = 0)
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
                            ViewBag.Market = new SelectList(db.markets, "id", "marketname");
                            ViewBag.Lab = new SelectList(db.labs, "id", "Name");
                            
                            if (MarketID != null)
                            {
                                ViewBag.MarketID = MarketID;
                                ViewBag.Role_id = new SelectList(db.roles.Where(x => x.id == 8), "id", "name_ar");
                            }
                            else if (LabID > 0)
                            {
                                ViewBag.LabID = LabID;
                                ViewBag.Role_id = new SelectList(db.roles.Where(x => x.id == 4), "id", "name_ar");
                            }
                            else
                            {
                                ViewBag.Role_id = new SelectList(db.roles.Where(x => x.id != 1 && x.id != 5 && x.id != 6 && x.id != 7), "id", "name_ar");
                            }

                            ViewBag.region_id = new SelectList(db.regions.Where(x => x.active == true), "id", "name_ar");
                            RegisterViewModel model = new RegisterViewModel();
                            return View(model);
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

        //[AllowAnonymous]
        public bool RegisterUser(string userName)
        {
            try
            {
                user user1 = new user();
                user1.user_id = Guid.NewGuid().ToString();
                user1.password = "123456";
                user1.phone = userName;
                user1.last_name = "";
                user1.app_type = 1;
                user1.role_id = 2;
                user1.name = userName;
                user1.active = true;
                user1.region_id = 1;
                user1.country = "1";
                user1.rating = 0;
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

        // this method is using for creating the new user
        public async Task<ActionResult> NewUserRegister(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.MobileNo, Email = model.MobileNo + "@Saatco.com" };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    MurshadikCP.Models.DB.user user1 = new Models.DB.user();
                    user1.user_id = user.Id;
                    user1.role_id = model.Role_id;
                    user1.password = user.PasswordHash;
                    user1.phone = user.UserName;
                    user1.name = model.Name;
                    user1.governorate = model.Govornorate_id.ToString();
                    user1.municipality = model.Municipality_id.ToString();
                    user1.app_type = 1;
                    user1.active = true;
                    user1.region_id = 1;
                    user1.region = "منطقة الرياض";
                    user1.country = "المملكة العربية السعودية";
                    user1.rating = 0;
                    MurshadikCP.Models.DB.mlaraEntities db = new Models.DB.mlaraEntities();
                    db.users.Add(user1);
                    db.SaveChanges();
                }
                AddErrors(result);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateUser(user u, long region_id, string[] Market, int LabID = 0, int MarketID = 0, int Lab = 0)
        {
            if (u != null)
            {
                if (u.id > 0)
                {
                    user user = db.users.Find(u.id);
                    if (user != null)
                    {
                        user user_phone = db.users.Where(x => x.id != u.id && x.phone == u.phone).FirstOrDefault();
                        if (user_phone == null)
                        {
                            user.name = u.name;
                            user.role_id = u.role_id;
                            user.region_id = u.region_id;
                            user.phone = u.phone;
                            if (u.role_id == (int)Role.LabManager)
                            {
                                Models.DB.UserAccessTypeIdentifier uati = db.UserAccessTypeIdentifiers.Where(x => x.user_id == u.id).FirstOrDefault();
                                if (uati != null)
                                {
                                    uati.type_value = Lab.ToString();
                                    uati.type = (int)UserAccessTypeIdentifier.Labs;
                                    uati.created_at = DateTime.Now;
                                    uati.created_by = u.id;
                                    db.Entry(uati).State = EntityState.Modified;
                                }//user_access ua = db.user_access.Where(x => x.userid == u.id).FirstOrDefault();
                                else 
                                {
                                    uati = new Models.DB.UserAccessTypeIdentifier();
                                    uati.type_value = Lab.ToString();
                                    uati.type = (int)UserAccessTypeIdentifier.Labs;
                                    uati.user_id = user.id;
                                    uati.created_at = DateTime.Now;
                                    uati.created_by = u.id;
                                    db.UserAccessTypeIdentifiers.Add(uati);
                                }
                            }
                            else if (u.role_id == (int)Role.Market)
                            {
                                Models.DB.UserAccessTypeIdentifier uati = db.UserAccessTypeIdentifiers.Where(x => x.user_id == u.id).FirstOrDefault();
                                if (uati != null)
                                {
                                    uati.type_value = string.Join(",", Market);
                                    uati.type = (int)UserAccessTypeIdentifier.Markets;
                                    uati.created_at = DateTime.Now;
                                    uati.created_by = u.id;
                                    db.Entry(uati).State = EntityState.Modified;
                                }
                                else
                                {
                                    uati = new Models.DB.UserAccessTypeIdentifier();
                                    uati.created_at = DateTime.Now;
                                    uati.created_by = u.id;
                                    uati.user_id = user.id;
                                    uati.type = (int)UserAccessTypeIdentifier.Markets;
                                    uati.type_value = string.Join(",", Market);
                                    db.UserAccessTypeIdentifiers.Add(uati);
                                }
                            }
                            else
                            {
                                Models.DB.UserAccessTypeIdentifier uati = db.UserAccessTypeIdentifiers.Where(x => x.user_id == u.id).FirstOrDefault();
                                if (uati != null)
                                {
                                    db.UserAccessTypeIdentifiers.Remove(uati);
                                }
                            }

                            db.Entry(user).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            ViewBag.error = "رقم الجوال موجود مسبقاً";
                            ViewBag.Market = new SelectList(db.markets, "id", "marketname");
                            ViewBag.Lab = new SelectList(db.labs, "id", "Name");
                            ViewBag.Role_id = new SelectList(db.roles.Where(x => x.id != 1), "id", "name_ar", user.role_id);
                            ViewBag.region_id = new SelectList(db.regions.Where(x => x.active == true), "id", "name_ar", user.region_id);
                            return View(u);
                        }
                    }
                }
            }
            return RedirectToAction("Index", "User");
        }
        // using this method for creating the new user
        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model, long region_id, string[] Market, int LabID = 0, int MarketID = 0, int Lab = 0)
        {
            MurshadikCP.Models.DB.mlaraEntities db = new Models.DB.mlaraEntities();
            string username = model.MobileNo.CheckUser();
            if (username != "" && username.Length == 12)
            {
                if (ModelState.IsValid)
                {
                    var user = new ApplicationUser { UserName = model.MobileNo, Email = model.MobileNo + "@Saatco.com" };
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        MurshadikCP.Models.DB.user user1 = new Models.DB.user();
                        user1.user_id = user.Id;
                        user1.role_id = model.Role_id;
                        user1.password = user.PasswordHash;
                        user1.phone = user.UserName;
                        user1.name = model.Name;
                        user1.governorate = null;
                        user1.municipality = null;
                        user1.app_type = 1;
                        user1.active = true;
                        user1.last_login_date = DateTime.Now;
                        user1.created_at = DateTime.Now;
                        user1.region_id = region_id == 0 ? 9999 : region_id;
                        user1.region = region_id != 0 ? db.regions.Where(x => x.id == region_id).FirstOrDefault().name_ar : "";
                        user1.country = "المملكة العربية السعودية";
                        user1.rating = 0;

                        db.users.Add(user1);
                        db.SaveChanges();

                        if (model.Role_id == (int)Role.LabManager) // 4 for labmanager
                        {
                            Models.DB.UserAccessTypeIdentifier ua = new Models.DB.UserAccessTypeIdentifier();
                            ua.created_at = DateTime.Now;
                            ua.created_by = ui.Id;
                            ua.type = (int)UserAccessTypeIdentifier.Labs;
                            ua.type_value = Lab.ToString();
                            ua.user_id = user1.id;
                            db.UserAccessTypeIdentifiers.Add(ua);
                            db.SaveChanges();
                        }
                        else if (model.Role_id == (int)Role.Market) // 8 for market
                        {
                            Models.DB.UserAccessTypeIdentifier ua = new Models.DB.UserAccessTypeIdentifier();
                            ua.created_at = DateTime.Now;
                            ua.created_by = ui.Id;
                            ua.type = (int)UserAccessTypeIdentifier.Markets;
                            ua.type_value = string.Join(",", Market);
                            ua.user_id = user1.id;
                            db.UserAccessTypeIdentifiers.Add(ua);
                            db.SaveChanges();
                        }

                        return RedirectToAction("Index", "User");
                    }
                    AddErrors(result);
                    ViewBag.Market = new SelectList(db.markets, "id", "marketname");
                    ViewBag.Lab = new SelectList(db.labs, "id", "Name");

                    ViewBag.region_id = new SelectList(db.regions.Where(x => x.active == true), "id", "name_ar");
                }
            }

            ViewBag.Role_id = new SelectList(db.roles, "id", "name_ar");
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.MobileNo + "@Saatco.com");
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ChangePasswordAsync(user.Id, model.CurrentPassword, model.Password);
            //var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword_V2(ChangePasswordViewModel model)
        {
            
            var user = await UserManager.FindByNameAsync(model.Mobile);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("NotAllow", "Custom");
            }

            string token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);

            var result = await UserManager.ResetPasswordAsync(user.Id, token, model.NewPassword);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "User", new { msg = "Successfully Reset Password!" });
            }

            IdentityResult ir = result;
            AddErrors(result);

            return RedirectToAction("Profile","User", new { id = user.Id, type = 1, Error = ((string[])ir.Errors).Length > 0 ? ((string[])ir.Errors)[0] : "" });
        }

        //
        // POST: /Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Mobile);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("NotAllow", "Custom");
            }

            //string token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);

            var result = await UserManager.ChangePasswordAsync(user.Id, model.OldPassword, model.NewPassword);

            //var result = await UserManager.ResetPasswordAsync(user.Id, "3687", model.NewPassword);

            //var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Session.Remove("User");
            Session.RemoveAll();
            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Dashboard");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}