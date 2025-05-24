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
	public class FarmVisitServiceController: BaseController
	{
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public	ActionResult Index()
		{
			if (currentPageID("FarmVisitService") > 0)
			{
				if (!CurrentUser.canView(currentPageID("FarmVisitService")))
					return RedirectToAction("NotAllow", "Custom");
			}
			
			var farmService = db.FarmVisitServices.FirstOrDefault();
			if (farmService == null)
			{
				farmService = new FarmVisitService()
				{
					CreatedBy = CurrentUser.Id,
					IsActive=true,
					CreatedDate=DateTime.Now,
					UpdatedDate = DateTime.Now,
				};
				db.FarmVisitServices.Add(farmService);
				db.SaveChanges();

			}

			return View(farmService);
		}
		[HttpPost]
		public ActionResult ActiveFarmVisitService(long Id , string IsActive)
		{
			try
			{
				if (Id < 0 || string.IsNullOrEmpty(IsActive))
				{
					return Json("error");
				}
				var farmService = db.FarmVisitServices.Find(Id);
				if (IsActive.Equals("1"))
				{
					farmService.IsActive = true;
				}
				else { 
				farmService.IsActive = false;
				}
				farmService.UpdatedDate = DateTime.Now;
				farmService.CreatedBy = this.CurrentUser.Id;
				db.Entry(farmService).State = EntityState.Modified; ;
				db.SaveChanges();

				return Json("Success");
			}
			catch (Exception)
			{

				return Json("Error");
			}
			
			
		}
	}
}