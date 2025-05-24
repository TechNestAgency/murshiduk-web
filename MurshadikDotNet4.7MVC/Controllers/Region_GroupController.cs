using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MurshadikCP.Controllers
{
    public class Region_GroupController : BaseController
    {
        // GET: Region_Group
        // Index view shows region group of list
        public ActionResult Index()
        {
            if (currentPageID("Region_Group") > 0)
            {
                if (!CurrentUser.canView(currentPageID("Region_Group")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            RegionGroupPageModel rg;
            db.Configuration.ProxyCreationEnabled = false;
            // only Super Admin can view this page and performs action
            if (CurrentUser.RoleId == (int)Role.AllRegionManager || CurrentUser.HasAdminAccess)
            {

                var reg = db.regions.Include("users").Where(x => x.active == true).Select(r => new { users = r.users.Where(y => y.role_id == 6), region = r }
                    ).ToList();

                List<region> regionsList = new List<region>();

                foreach (var r in reg)
                {
                    regionsList.Add(new region
                    {
                        id = r.region.id,
                        name = r.region.name,
                        name_ar = r.region.name_ar,
                        active = r.region.active,
                        users = r.users.ToList()
                    });
                }

                rg = new RegionGroupPageModel(regionsList, db);
            }
            else if (CurrentUser.RoleId == (int)Role.RegionManager)
            {
                long region_id = db.users.Where(x => x.id == CurrentUser.Id).FirstOrDefault().region_id;
                rg = new RegionGroupPageModel(db.regions.Where(x => x.active == true && x.id == region_id).ToList(), db);
            }
            else
            {
                return RedirectToAction("NotAllow", "Custom");
            }

            foreach (var region in rg.regions)
            {
                db.GetTop5ConsultantCalling(Convert.ToInt16(region.id)).ToList().ForEach(delegate (GetTop5ConsultantCalling_Result tc)
                {
                    rg.tcc.Add(tc);
                });

                db.GetTop5ConsultantRating(Convert.ToInt16(region.id)).ToList().ForEach(delegate (GetTop5ConsultantRating_Result tr)
                {
                    rg.tcr.Add(tr);
                });
            }

            return View(rg);
        }



        // change stataus of user
        [HttpPost]
        public ActionResult StatusChangeOfUser(long user_id, bool status)
        {
            if (currentPageID("Region_Group") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("Region_Group")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            user u = db.users.Find(user_id);
            if (u != null)
            {
                u.is_group_admin = status == true ? false : true;
                u.can_post = status == true ? false : true;
                db.SaveChanges();
            }
            return Json("Success");
        }
    }

    public class RegionGroupPageModel
    {
        public mlaraEntities db;
        public IEnumerable<region> regions;
        public List<GetTop5ConsultantCalling_Result> tcc;
        public List<GetTop5ConsultantRating_Result> tcr;

        public RegionGroupPageModel(IEnumerable<region> _regions, mlaraEntities _db)
        {
            db = _db;
            regions = _regions;
            tcc = new List<GetTop5ConsultantCalling_Result>();
            tcr = new List<GetTop5ConsultantRating_Result>();
        }
        
    }
}