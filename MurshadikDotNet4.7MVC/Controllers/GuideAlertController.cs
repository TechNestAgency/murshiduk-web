using MurshadikCP.Controllers.Dashboard.DashboardInterface;
using MurshadikCP.Controllers.Dashboard.DashboardServices;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MurshadikCP.Controllers
{
    public class GuideAlertController : BaseController
    {
        // GET: GuideAlertontroller
        private IGuideAlert _guideAlert = new GuideAlertService();
        public ActionResult Index(int? Page_No)
        {
            if (currentPageID("GuideAlert") > 0)
            {
                if (!CurrentUser.canView(currentPageID("GuideAlert")))
                    return RedirectToAction("NotAllow", "Custom");
            }
            int pagesize = 10;
            int No_Of_Page = (Page_No ?? 1);         
            return View(_guideAlert.GetGuideAlert(pagesize, No_Of_Page));
          
        }      
    }
}
