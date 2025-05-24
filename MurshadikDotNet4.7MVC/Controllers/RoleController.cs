using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MurshadikCP.Controllers
{
    [Authorize]
    public class RoleController : BaseController
    {
        //private mlaraEntities db = new mlaraEntities();
        // GET: Role
        public ActionResult Index(int role_id = 0)
        {
            if (currentPageID("Role") > 0)
            {
                if (!CurrentUser.canView(currentPageID("Role")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            ViewBag.role_id = new SelectList(db.roles.Where(x => x.id != 1), "id", "name_ar");
            List<roles_permission> list = new List<roles_permission>();
            if (role_id == 0)
            {
                list = db.roles_permission.Where(x => x.role_id != 1).ToList();
            }
            else
            {
                list = db.roles_permission.Where(x => x.role_id == role_id).ToList();
            }
            
            return View(list);
        }

        [HttpPost]
        public ActionResult test(int role_id = 0)
        {
            List<roles_permission> list = new List<roles_permission>();
            if (role_id == 0)
            {
                list = db.roles_permission.Where(x => x.role_id != 1).ToList();
            }
            else
            {
                list = db.roles_permission.Where(x => x.role_id == role_id).ToList();
            }
            return View(list);
        }

        public ActionResult BindGrid(int role_id)
        {
            ViewBag.role_id = new SelectList(db.roles.Where(x => x.id != 1), "id", "name_ar");
            var list = db.roles_permission.Where(x => x.role_id == role_id).ToList();
            return View(list);
        }
    }
}