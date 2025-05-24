using MurshadikCP.Controllers.API.ApiServices;
using MurshadikCP.Controllers.API.ApiServices.Interfaces;
using MurshadikCP.Controllers.API.ApiViewModel;
using MurshadikCP.Controllers.Dashboard.DashboardInterface;
using MurshadikCP.Controllers.Dashboard.DashboardServices;
using MurshadikCP.Controllers.Dashboard.DashboardViewModel;
using MurshadikCP.DataModel;
using MurshadikCP.Interface;
using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using MurshadikCP.Models.ViewModels;
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;



namespace MurshadikCP.Controllers
{
	[Authorize]
	public class WorkersController: BaseController
	{
		public readonly IWorkerBoardService  _workerService = new WorkerBoardService();

		public async Task<ActionResult> Index(int? pageNo, string searchData, string filterValue, string jobs, string region, int isBusy = 0, string startDate = null, string endDate = null)
		{
			if (currentPageID("Workers") > 0)
			{
				if (!CurrentUser.canView(currentPageID("Workers")))
					return RedirectToAction("NotAllow", "Custom");
			}
			int pagesize = 10;
			int NoOfPage = (pageNo ?? 1);

			if (searchData != null)
			{
				pageNo = 1;
			}else
			{
				searchData = filterValue;
			}
			
			ViewBag.FilterValue = searchData;
			ViewBag.FilterRegion = region;
			ViewBag.IsBusy = isBusy;
			ViewBag.FilterJobs = jobs;
			ViewBag.Jobs = new SelectList(db.Jobs, "Name", "Name");
			ViewBag.Region = new SelectList(db.regions, "name_ar", "name_ar");


			var result = await _workerService.GetWorkerList();


			if (!string.IsNullOrEmpty(startDate))
			{
				ViewBag.startDate = startDate;
				var sdt = DateTime.Parse(startDate, null, System.Globalization.DateTimeStyles.RoundtripKind);
				result = result.Where(w => w.CreatedAt >= sdt);
			}
			if (!string.IsNullOrEmpty(endDate))
			{
				ViewBag.endDate = endDate;
				var edt = DateTime.Parse(endDate, null, System.Globalization.DateTimeStyles.RoundtripKind);
				result = result.Where(w => w.CreatedAt <= edt);
			}

			if (!String.IsNullOrEmpty(searchData))
			{
				result = result.Where(x => x.Name.Contains(searchData) || x.PhoneNumber.Contains(searchData));

			}

			if (region != "" && region != null)
			{
				result = result.Where(x => x.Rejon == region);
			}
			if (isBusy > 0)
				result = result.Where(x => x.IsBusy  == false ? true : false);

			if (jobs != "" && jobs != null)
			{
				result = result.Where(x => x.Job != null && x.Job.Contains(jobs));
			}
			return View(result.ToPagedList(NoOfPage, pagesize));
		}

		public async Task<ActionResult> Details(long WorkerId)
		{
			var result = await _workerService.GetWorker(WorkerId);


			return View(result);
		}


	}
}