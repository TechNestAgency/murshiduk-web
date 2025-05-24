using Microsoft.AspNet.Identity;
using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using MurshadikCP.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MurshadikCP.Controllers
{
    public abstract class IBaseController : Controller
    {

        private mlaraEntities _db = new mlaraEntities();

        public mlaraEntities db
        {
            get { return _db; }
        }

        public List<MenuModel> menu {get; set;}
        public List<roles_permission> userMenuPermission { get; set; }
        public IBaseController()
        {
            menu = new List<MenuModel>();
            List<UserAccessMenuModel> menuAccess = new List<UserAccessMenuModel>();

            var allPages = db.Pages.OrderBy(p => p.Sort).ToList();

            allPages.ForEach(page =>
                {

                    if (page.Parent_page is null)
                    {
                        MenuModel menuItem = new MenuModel();

                        menuItem.id = page.id;
                        menuItem.route = page.PageName;
                        menuItem.display_name = page.Display_Name;
                        menuItem.icon = page.Icon;
                        menuItem.should_display = page.should_display;
                        menuItem.is_active = page.is_active;

                        var children = allPages.Where(p => p.Parent_page == page.id).OrderBy(p => p.Sort).ToList();
                        if (children.Any())
                        {
                            List<MenuModel> childMenuItems = new List<MenuModel>();
                            menuItem.has_childern = true;
                            children.ForEach(cp =>
                                {
                                    MenuModel childMenuItem = new MenuModel();
                                    childMenuItem.id = cp.id;
                                    childMenuItem.route = cp.PageName;
                                    childMenuItem.display_name = cp.Display_Name;
                                    childMenuItem.has_childern = false;
                                    childMenuItem.should_display = cp.should_display;
                                    childMenuItem.is_active = cp.is_active;
                                    childMenuItem.icon = cp.Icon;
                                    childMenuItems.Add(childMenuItem);
                                }
                            );

                            menuItem.children = childMenuItems;
                        }
                        else
                        {
                            menuItem.has_childern = false;
                        }

                        menu.Add(menuItem);
                    }

                }
            );

            ViewData["menu"] = menu;
        }

        public void setRolePermissions(long roleId) {

            var mps = db.roles_permission.Where(rp => rp.role_id == roleId).ToList();
            this.userMenuPermission = mps;
            ViewData["menuPermissions"] = mps;

        }

    }

    public class BaseController : IBaseController
    {
        private UserRepository userRepository = null;
        public UserInfo CurrentUser { get; set; }

        public BaseController() {
            this.userRepository = new UserRepository();
        }

        public long currentPageID(string route) {
            var page = menu.Where(m => m.route == route).FirstOrDefault();
            return (page != null) ? page.id : 0;
        }
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            if (requestContext.HttpContext.User.Identity.IsAuthenticated) {
                string uID = requestContext.HttpContext.User.Identity.Name;
                CurrentUser = userRepository.GetUserInfoByID(uID);
                this.userMenuPermission = CurrentUser.roles_Permissions;

                //this.setRolePermissions(CurrentUser.RoleId);
            }

            ViewData["currentUser"] = CurrentUser;

        }
    }
}