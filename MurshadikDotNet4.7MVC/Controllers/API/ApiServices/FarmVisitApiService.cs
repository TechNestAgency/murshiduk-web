using MurshadikCP.Controllers.API.ApiServices.Interfaces;
using MurshadikCP.Controllers.API.ApiViewModel;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;

namespace MurshadikCP.Controllers.API.ApiServices
{
	public class FarmVisitApiService : IFarmVisitApiService
	{
		mlaraEntities db = new mlaraEntities();
		public readonly IApiResultService _apiResultService = new ResultService();
		string hostURL = WebConfigurationManager.AppSettings["publicHostUrl"];
		public object AddFarmVisit(long UserId, long CityId, long RegionId, DateTime VisitDate, string FarmName, string PurposeOfVisit, string Location, List<HttpPostedFile> httpPostedFile)
		{

			FarmVisitOrder farmVisitOrder = new FarmVisitOrder();
			farmVisitOrder.FarmName = FarmName;
			farmVisitOrder.FarmVisitMedias = setFarmVisitMeadia(httpPostedFile);
			farmVisitOrder.FarmVisitStatuses = setFarmvisitStatus();
			farmVisitOrder.CityId = CityId;
			farmVisitOrder.RegionId = RegionId;
			farmVisitOrder.VisitDate = VisitDate;
			farmVisitOrder.OrderDate = DateTime.Now;
			farmVisitOrder.PurposeOfVisit = PurposeOfVisit;
			farmVisitOrder.Location = Location;
			farmVisitOrder.FarmerId = UserId;
			db.FarmVisitOrders.Add(farmVisitOrder);
			db.SaveChanges();
			return "";
		}

		private ICollection<FarmVisitStatus> setFarmvisitStatus()
		{
			List<FarmVisitStatus> farmstatus = new List<FarmVisitStatus>();
			FarmVisitStatus farmVisitStatu = new FarmVisitStatus()
			{
				CreatedAt = DateTime.Now,
				Status = 4,
				Description ="انتظار",
				
			};
			farmstatus.Add(farmVisitStatu);

			return farmstatus;
		}

		private ICollection<FarmVisitMedia> setFarmVisitMeadia(List<HttpPostedFile> httpPostedFile)
		{
			List<FarmVisitMedia> farmVisitMedia = new List<FarmVisitMedia>();
			foreach (var item in httpPostedFile)
			{
				FarmVisitMedia farmVisit = new FarmVisitMedia()
				{
					FileName = Guid.NewGuid().ToString() + Path.GetExtension(item.FileName),
					Path = hostURL+"Media/FarmVisitOrderMedia/",
					Extention = Path.GetExtension(item.FileName),
					CreatedAt = DateTime.Now,
					FileCategory = "Order",
				};
				var path = Path.Combine(HostingEnvironment.MapPath("~/Media/FarmVisitOrderMedia/"), farmVisit.FileName);
				item.SaveAs(path);
				farmVisitMedia.Add(farmVisit);
			}
			return farmVisitMedia;
		}

		public object GetFarmVisit(long UserId)
		{
			try
			{

			var FarmerFarmVisits = (from visitOrder in db.FarmVisitOrders
									join region in db.regions on visitOrder.RegionId equals region.id
									join city in db.cities on visitOrder.CityId equals city.id
									where visitOrder.FarmerId == UserId
									select new FarmVisitViewModel
									{
										id = visitOrder.Id,
										FarmName = visitOrder.FarmName,
										Location = visitOrder.Location,
										City = city.name_ar,
										Region = region.name_ar,
										Medias = visitOrder.FarmVisitMedias.Select(s => new Media() {
											FileCategory = s.FileCategory,
											FileName = s.FileName,
											Path = s.Path + s.FileName
										}).ToList(),
										Report = visitOrder.FarmVisitReports.Select(sr =>new Report()
											{
												Description = sr.Description ,
												FileName = sr.FarmVisitMedias.Where(w=>w.FarmVisitReportId!=null && w.FarmVisitReportId == sr.Id).Select(ss=>ss.FileName).FirstOrDefault(),
												Path = sr.FarmVisitMedias.Where(w => w.FarmVisitReportId != null && w.FarmVisitReportId == sr.Id).Select(ss => ss.Path).FirstOrDefault()+ sr.FarmVisitMedias.Where(w => w.FarmVisitReportId != null && w.FarmVisitReportId == sr.Id).Select(ss => ss.FileName).FirstOrDefault()

										}).ToList(),
										OrderStatus = visitOrder.FarmVisitStatuses.OrderByDescending(o => o.CreatedAt).Select(s => new OrderStatus() { 
											stat = s.Status,
											description = s.Description
										}).FirstOrDefault(),
										PurposeOfVisit = visitOrder.PurposeOfVisit,
										Description = visitOrder.Description,
										OrderDate = visitOrder.OrderDate,
										VisitDate = visitOrder.VisitDate,
									}).ToList().OrderByDescending(o=>o.OrderDate);
			return FarmerFarmVisits;
			}
			catch (Exception ex)
			{

				throw;
			}
		}

		public object GetFarmVisitById(long FarmVisitId)
		{
			var FarmerFarmVisit = (from visitOrder in db.FarmVisitOrders
								   join region in db.regions on visitOrder.RegionId equals region.id
								   join city in db.cities on visitOrder.CityId equals city.id
								   where visitOrder.Id == FarmVisitId
								   select new FarmVisitViewModel
								   {
									   id = visitOrder.Id,
									   FarmName = visitOrder.FarmName,
									   Location = visitOrder.Location,
									   City = city.name_ar,
									   Region = region.name_ar,
									   Medias = visitOrder.FarmVisitMedias.Select(s => new Media() { FileCategory = s.FileCategory, FileName = s.FileName, Path = s.Path + s.FileName }).ToList(),
									   Report = visitOrder.FarmVisitReports.Select(sr => new Report()
									   {
										   Description = sr.Description,
										   FileName = sr.FarmVisitMedias.Where(w => w.FarmVisitReportId != null && w.FarmVisitReportId == sr.Id).Select(ss => ss.FileName).FirstOrDefault(),
										   Path = sr.FarmVisitMedias.Where(w => w.FarmVisitReportId != null && w.FarmVisitReportId == sr.Id).Select(ss => ss.Path).FirstOrDefault() + sr.FarmVisitMedias.Where(w => w.FarmVisitReportId != null && w.FarmVisitReportId == sr.Id).Select(ss => ss.FileName).FirstOrDefault()

									   }).ToList(),
									   OrderStatus = visitOrder.FarmVisitStatuses.OrderByDescending(o => o.CreatedAt).Select(s => new OrderStatus()
									   {
										   stat = s.Status,
										   description = s.Description
									   }).FirstOrDefault(),
									   PurposeOfVisit = visitOrder.PurposeOfVisit,
									   Description = visitOrder.Description,
									   OrderDate = visitOrder.OrderDate,
									   VisitDate = visitOrder.VisitDate,
								   }).FirstOrDefault();
			return FarmerFarmVisit;
		}
		public bool CheckingFarmVisitService()
		{
			var IsActive = db.FarmVisitServices.Select(s => s.IsActive).FirstOrDefault();
			return IsActive;
		}
	}
}