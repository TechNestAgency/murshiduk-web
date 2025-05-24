using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MurshadikCP.Controllers
{
    public class CustomController : BaseController
    {
        // GET: Custom
        // this view just shows the end user if he or she don't have access the page or view then this page appear.
        public ActionResult NotAllow()
        {
            return View();
        }
    }
}