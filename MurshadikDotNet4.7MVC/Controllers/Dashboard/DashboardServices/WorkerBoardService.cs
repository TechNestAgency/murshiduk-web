using MurshadikCP.Controllers.API.ApiServices.Interfaces;
using MurshadikCP.Controllers.API.ApiViewModel;
using MurshadikCP.Controllers.Dashboard.DashboardInterface;
using MurshadikCP.Controllers.Dashboard.DashboardViewModel;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MurshadikCP.Controllers.Dashboard.DashboardServices
{
	public class WorkerBoardService: IWorkerBoardService
	{
		mlaraEntities db = new mlaraEntities();

		public async Task<IEnumerable<WorkerViewModel>> GetWorkerList()
		{
			var result = await db.Workers.Select(s => new 
			{
				Id = s.Id,
				Name= s.user.name+" "+s.user.last_name,
				PhoneNumber = s.user.phone,
				Rejon = s.user.region1.name_ar,
				Jobs = s.WorkerJobs.Select(sj=>sj.Job.Name).ToList(),
				IsBusy = s.Isbusy,
				CreatedAt = s.CreatedAt,	
			}).ToListAsync();

			var workerList = result.Select(s => new WorkerViewModel()
			{
				Id = s.Id,
				Name = s.Name,
				PhoneNumber = s.PhoneNumber,
				Rejon = s.Rejon,
				Job = string.Join(",", s.Jobs),
				IsBusy = s.IsBusy,
				CreatedAt= s.CreatedAt,
			});
			return workerList;
		}


		public async Task<WorkerDetail> GetWorker(long WorkerId)
		{
			var result = await (from worker in db.Workers
								where worker.Id == WorkerId && worker.IsActive == true
								select new WorkerDetail()
								{
									Id = worker.Id,
									UserId = worker.user.id,
									Name = worker.user.name + " " + worker.user.last_name,
									Jobs = worker.WorkerJobs.Select(s => new JobDto
									{
										Id = s.Id,
										Name = s.Job.Name,
										Description = s.Job.Description

									}).ToList(),
									Experiences = worker.WorkerExperiences.Select(s => new ExperienceDto()
									{
										Id = s.Id,
										WorkerId = s.WorkerId,
										Description = s.Description,
										FromDate = s.FromDate,
										ToDate = s.ToDate
									}).ToList(),
									Isbusy = worker.Isbusy,
									Img = worker.user.avatar,
									DateOfBirth = (DateTime)worker.DateOfBirth,
									Nationalty = worker.Nationalty.Name,
									NationaltyAr = worker.Nationalty.NameArabic,
									Phone = worker.Phone,
									ExpectedSalary = worker.ExpectedSalary,
									Address = worker.Address
								}).FirstOrDefaultAsync();

			return result;

		}
	}

}