using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MurshadikCP.Controllers
{
    public class OTPController : BaseController
    {
        // GET: OTP

        public ActionResult Index(int? Page_No)
        {
            int pagesize = 10;
            int No_Of_Page = (Page_No ?? 1);
            IPagedList<OTPEntity> result = (from o in db.otps
                     join u in db.users on o.user_id equals u.id
                     orderby o.id descending
                     select new OTPEntity
                     {
                         phone = u.phone,
                         otp = o.otp1,
                         created_at = o.created_at,
                         attempts = o.attempts
                     }).ToPagedList(No_Of_Page, pagesize);
            return View(result);
        }

        
    }

    public class OTPEntity
    {
        public string phone { get; set; }
        public string otp { get; set; }
        public DateTime created_at { get; set; }
        public int attempts { get; set; }
    }
}