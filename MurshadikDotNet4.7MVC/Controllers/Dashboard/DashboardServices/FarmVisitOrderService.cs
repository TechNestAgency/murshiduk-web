using MurshadikCP.Controllers.API.ApiViewModel;
using MurshadikCP.Controllers.Dashboard.DashboardInterface;
using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using MurshadikCP.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace MurshadikCP.Controllers.Dashboard.DashboardServices
{
	public class FarmVisitOrderService : IFarmVisitOrderService
	{
		private mlaraEntities db = new mlaraEntities();
		string hostURL = WebConfigurationManager.AppSettings["publicHostUrl"];


		public object GetFarmOrder(long OrderId)
		{

			var FarmVisit = (from visitOrder in db.FarmVisitOrders
									join region in db.regions on visitOrder.RegionId equals region.id
									join users in db.users on visitOrder.FarmerId equals users.id
									join city in db.cities on visitOrder.CityId equals city.id
									where visitOrder.Id == OrderId
									select new FarmVisitViewModel
									{
										id = visitOrder.Id,
										FarmName = visitOrder.FarmName,
										FarmerName = users.name + " " + users.last_name,
										Location = visitOrder.Location,
										City = city.name_ar,
										Region = region.name_ar,
										Medias = visitOrder.FarmVisitMedias.Select(s => new Media() { FileCategory = s.FileCategory, FileName = s.FileName, Path = s.Path + s.FileName }).ToList(),
										Report = visitOrder.FarmVisitReports.Select(sr => new Report()
										{
											Description = sr.Description,
											FileName = sr.FarmVisitMedias.Where(w => w.FarmVisitReportId != null && w.FarmVisitReportId == sr.Id).Select(ss => ss.FileName).FirstOrDefault(),
											Path = hostURL + "/" + sr.FarmVisitMedias.Where(w => w.FarmVisitReportId != null && w.FarmVisitReportId == sr.Id).Select(ss => ss.Path).FirstOrDefault() + "/" + sr.FarmVisitMedias.Where(w => w.FarmVisitReportId != null && w.FarmVisitReportId == sr.Id).Select(ss => ss.FileName).FirstOrDefault()

										}).ToList(),
										//Report = visitOrder.FarmVisitReports.Select(s => new
										//{
										//	Report = s.FarmVisitMedias.Select(ss => new
										//	{
										//		Description = s.Description,
										//		FileName = ss.FileName,
										//		Path = hostURL + "/" + ss.Path + "/" + ss.FileName
										//	})
										//}).ToList(),
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
			return FarmVisit;
		
		}

		public object GetFarmOrders(long UserRegionId,long UserRoleId)
		{
			if (UserRoleId==1)
			{
				var FarmVisits = (from visitOrder in db.FarmVisitOrders
								  join region in db.regions on visitOrder.RegionId equals region.id
								  join users in db.users on visitOrder.FarmerId equals users.id
								  join city in db.cities on visitOrder.CityId equals city.id
								  select new FarmVisitViewModel
								  {
									  id = visitOrder.Id,
									  FarmName = visitOrder.FarmName,
									  FarmerName = users.name + " " + users.last_name,
									  Location = visitOrder.Location,
									  City = city.name_ar,
									  Region = region.name_ar,
									  Medias = visitOrder.FarmVisitMedias.Select(s => new Media() { FileCategory = s.FileCategory, FileName = s.FileName, Path = s.Path + s.FileName }).ToList(),

									  Report = visitOrder.FarmVisitReports.Select(sr => new Report()
									  {
										  Description = sr.Description,
										  FileName = sr.FarmVisitMedias.Where(w => w.FarmVisitReportId != null && w.FarmVisitReportId == sr.Id).Select(ss => ss.FileName).FirstOrDefault(),
										  Path = hostURL + "/" + sr.FarmVisitMedias.Where(w => w.FarmVisitReportId != null && w.FarmVisitReportId == sr.Id).Select(ss => ss.Path).FirstOrDefault() + "/" + sr.FarmVisitMedias.Where(w => w.FarmVisitReportId != null && w.FarmVisitReportId == sr.Id).Select(ss => ss.FileName).FirstOrDefault()

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
								  }).OrderByDescending(o=>o.OrderDate).ToList();
				return FarmVisits;


			}
			else
			{

			var FarmVisits = (from visitOrder in db.FarmVisitOrders
									join region in db.regions on visitOrder.RegionId equals region.id
									join users in db.users on visitOrder.FarmerId equals users.id
									join city in db.cities on visitOrder.CityId equals city.id
									where visitOrder.RegionId == UserRegionId
							  select new FarmVisitViewModel
									{
										id=visitOrder.Id,
										FarmName = visitOrder.FarmName,
										FarmerName = users.name+" "+users.last_name,
										Location = visitOrder.Location,
										City = city.name_ar,
										Region = region.name_ar,
										Medias = visitOrder.FarmVisitMedias.Select(s => new Media() { FileCategory = s.FileCategory, FileName = s.FileName, Path = s.Path+ s.FileName }).ToList(),
								  Report = visitOrder.FarmVisitReports.Select(sr => new Report()
								  {
									  Description = sr.Description,
									  FileName = sr.FarmVisitMedias.Where(w => w.FarmVisitReportId != null && w.FarmVisitReportId == sr.Id).Select(ss => ss.FileName).FirstOrDefault(),
									  Path = hostURL + "/" + sr.FarmVisitMedias.Where(w => w.FarmVisitReportId != null && w.FarmVisitReportId == sr.Id).Select(ss => ss.Path).FirstOrDefault() + "/" + sr.FarmVisitMedias.Where(w => w.FarmVisitReportId != null && w.FarmVisitReportId == sr.Id).Select(ss => ss.FileName).FirstOrDefault()

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
									}).OrderByDescending(o => o.OrderDate).ToList();
				return FarmVisits;

			}


		}

		public object AddReport(long OrderId)
		{
			
			return "";
		}

		public object UpdateFarmOrder(FarmVisitOrder farmVisitOrder)
		{
			return "";
		}
	}
}