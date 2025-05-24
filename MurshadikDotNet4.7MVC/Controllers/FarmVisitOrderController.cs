using MurshadikCP.Controllers.API.ApiViewModel;
using MurshadikCP.Controllers.Dashboard.DashboardInterface;
using MurshadikCP.Controllers.Dashboard.DashboardServices;
using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Mvc;

namespace MurshadikCP.Controllers
{
    [Authorize]
    public class FarmVisitOrderController : BaseController
    {
        public IFarmVisitOrderService _farmVisit = new FarmVisitOrderService();
        string hostURL = WebConfigurationManager.AppSettings["publicHostUrl"];

        // GET: FarmVisitOrder
        [Authorize]
        public ActionResult Index()
        
        {
            if (currentPageID("FarmVisitOrder") > 0)
            {
                if (!CurrentUser.canView(currentPageID("FarmVisitOrder")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            return View(_farmVisit.GetFarmOrders(this.CurrentUser.region_id,this.CurrentUser.RoleId));
        }

        // GET: FarmVisitOrder/Details/5
        [Authorize]
        public ActionResult Details(long id)
        {
            return View(_farmVisit.GetFarmOrder(id));
        }
        [Authorize]
        public ActionResult AddReport(long id)
        {
            FarmVisitReport FarmVisitReport = new FarmVisitReport();
            FarmVisitReport.FarmVisitOrderId = id;
            ViewBag.validate ="";
            return View(FarmVisitReport);
        }
        [HttpPost]
        public ActionResult AddReport(HttpPostedFileBase file ,string description ,long Id)
         {
            FarmVisitReport FarmVisitReport = new FarmVisitReport();
            FarmVisitMedia FarmVisitMedia = new FarmVisitMedia();
            if (file !=null && !string.IsNullOrEmpty(description) && Id> 0)
            {
                FarmVisitMedia.FileCategory = "Report";
                FarmVisitMedia.FileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                FarmVisitMedia.Path = hostURL + "Media/FarmVisitOrderMedia/";
                var path = Path.Combine(HostingEnvironment.MapPath("~/Media/FarmVisitOrderMedia/"), FarmVisitMedia.FileName);
                file.SaveAs(path);

                FarmVisitReport.Description = description;
                FarmVisitReport.FarmVisitOrderId = Id;
                FarmVisitReport.CreatedAt = DateTime.Now;
                FarmVisitReport.CreatedBy = this.CurrentUser.Id;
                FarmVisitReport.FarmVisitMedias.Add(FarmVisitMedia);
                this.db.FarmVisitReports.Add(FarmVisitReport);
                this.db.SaveChanges();
                return View(FarmVisitReport);

            }
            else
			{
             ViewBag.validate = "الرجاء ا لتاكد من كل الحقول";
             return View(FarmVisitReport);
			}
        }


        public ActionResult chageStatus(long id) 
        {
         
            FarmVisitStatus farmVisitStatus = new FarmVisitStatus();
            farmVisitStatus.FarmVisitOrderId = id;
                var VisitStatus= db.FarmVisitStatuses.OrderByDescending(o=>o.CreatedAt).Select(s=>new OrderStatus()
                {
                    stat = s.Status,
                    description = s.Description

                }).FirstOrDefault();
            ViewBag.status = VisitStatus;


            return View(farmVisitStatus);
        }
        [HttpPost]
        public ActionResult chageStatus(FarmVisitStatus farmVisitStatus, long Id, int Status,string Description)
        {
           
            
            farmVisitStatus.CreatedAt = DateTime.Now;
            farmVisitStatus.CreatedBy = CurrentUser.Id;
            farmVisitStatus.FarmVisitOrderId = Id;
         
                var FarmFarmer = db.FarmVisitOrders.Find(Id);
                var user = db.users.Find(FarmFarmer.FarmerId);


            string msg = "";

            SendSMS sms = new SendSMS();
           
            switch (Status)
			{                
                case 1:
                    farmVisitStatus.Status = 1;
                    farmVisitStatus.Description = "مقبول";
                    msg = "تم قبول طلبك لزيارة المزرعة اضغط على الرابط التالي\n" + "https://mewa-ershad.net/FarmVisitOrder/OrderDetail/"+Id.ToString();
                    sms.SMSSend(user.phone, msg);
                    break;                  
                case 2:
                    farmVisitStatus.Status = 2;
                    farmVisitStatus.Description = Description;
                    msg = "تم رفض الطلب للاسباب الاتية " + Description;
                    sms.SMSSend(user.phone, msg);
                    break;
                case 3:
                    farmVisitStatus.Status = 3;
                    farmVisitStatus.Description = "تمت الزيارة";
                    break;
                case 4:
                    farmVisitStatus.Status = 4;
                    farmVisitStatus.Description = "انتظار";
                    break;
                default:
					break;
			}

            db.FarmVisitStatuses.Add(farmVisitStatus);
            db.SaveChanges();
            return Json("Success"); 
            
        }

        [AllowAnonymous]
        public ActionResult OrderDetail(long? id)
		{
            if (id > 0)
			{
                long Id = (long)id;

            return View(_farmVisit.GetFarmOrder(Id));
			}
                return View();
		}

    

        // POST: FarmVisitOrder/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

    }
}
