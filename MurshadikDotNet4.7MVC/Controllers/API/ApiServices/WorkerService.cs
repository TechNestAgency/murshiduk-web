
using MurshadikCP.Controllers.API.ApiServices.Interfaces;
using MurshadikCP.Controllers.API.ApiViewModel;
using MurshadikCP.Models.DB;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;



namespace MurshadikCP.Controllers.API.ApiServices
{
	public class WorkerService: IWorkerService
	{
		mlaraEntities db = new mlaraEntities();
		public readonly IApiResultService _apiResultService = new ResultService();

		public async Task<object> AddWorker(Worker worker)
		{
			var IsExist = db.Workers.Where(w=>w.UserId == worker.UserId).FirstOrDefault();
			if (IsExist != null)
				return _apiResultService.Result(false, new {}, "Error. Worker is already exist", false);
		
				
			var result = db.Workers.Add(worker);
		    await   db.SaveChangesAsync();

			return _apiResultService.Result(true, result, "Success", false);
		}

		public async Task<object> UpdateWorker(Worker worker)
		{
			var workerResult = db.Workers.Find(worker.Id);
			if (workerResult == null)
				return _apiResultService.Result(false, "".ToList(), "Error", false);
			workerResult.Address = worker.Address;
			workerResult.Phone = worker.Phone;
			workerResult.DateOfBirth = (DateTime)worker.DateOfBirth;
			workerResult.IDNumber = worker.IDNumber;
			workerResult.Isbusy = worker.Isbusy;
			workerResult.ExpectedSalary = worker.ExpectedSalary;
			workerResult.NationaltyId = worker.NationaltyId;
			workerResult.UpdatedAt = DateTime.Now;

			 db.Entry(workerResult).State = EntityState.Modified;
		     await	db.SaveChangesAsync();

			return _apiResultService.Result(true, new { }, "Success", false);
		}

		public async Task<object> GetAll(int JobId)
		{
			var result = await (from worker in db.Workers
						 join wj in db.WorkerJobs on worker.Id equals wj.WorkerId
						 where wj.JobId == JobId && worker.IsActive==true && worker.IsDeleted==false
						 select new WorkerDto()
						 {
							 Id = worker.Id,
							 Name = worker.user.name + " " + worker.user.last_name,
							 Job = wj.Job.Name,
							 Isbusy = worker.Isbusy,
							 Nationalty= worker.Nationalty.Name,
							 NationaltyAr = worker.Nationalty.NameArabic,
							 DateOfBirth = (DateTime)worker.DateOfBirth,
							 Img = worker.user.avatar,
							 UserId= worker.user.id,
							 Address = worker.Address,
							 ExpectedSalary= worker.ExpectedSalary
						 }).ToListAsync();
			if (result.Count == 0)
			{
				return _apiResultService.Result(false, "".ToList(), "Error", false);
			}

			return _apiResultService.Result(true, result, "Success", false);

		}	
		public async Task<object> GetAllWorkerByPage(int noOfPage,int pageNumber,int pageSize)
		{
			var result = await (from worker in db.Workers
					
						 where worker.IsActive==true&& worker.IsDeleted == false
								select new
						 {
							 Id = worker.Id,
							 Name = worker.user.name + " " + worker.user.last_name,
							 Jobs = worker.WorkerJobs.Select(s => new JobDto
							 {
								 Id = s.Id,
								 Name = s.Job.Name,
								 Description = s.Job.Description

							 }).ToList(),
							 Isbusy = worker.Isbusy,
							 Nationalty= worker.Nationalty.Name,
							 NationaltyAr = worker.Nationalty.NameArabic,
							 DateOfBirth = (DateTime)worker.DateOfBirth,
							 Img = worker.user.avatar,
							 UserId= worker.user.id,
							 Address = worker.Address,
							 ExpectedSalary= worker.ExpectedSalary
						 }).OrderByDescending(x=>x.Id).Skip((noOfPage-1)*pageSize).Take(pageSize).ToListAsync();


			if (result.Count == 0)
			{
				return _apiResultService.Result(false, "".ToList(), "Error", false);
			}

			int workerCount = db.Workers.Where(w=>w.IsActive ==true).Count();
			int totalPages = (int)Math.Ceiling(workerCount / (double)pageSize);

			// if CurrentPage is greater than 1 means it has previousPage  
			var previousPage = pageNumber > 1 ? true : false;
			// if TotalPages is greater than CurrentPage means it has nextPage  
			var nextPage = pageNumber < totalPages ? true : false;

			return _apiResultService.Result(true, result, "Success", new { totalCount = workerCount, currentPage = pageNumber, totalPage = totalPages, nextPage, previousPage });

		}

		public async Task<object> GetWorker(long WorkerId)
		{
			var result = await (from worker in db.Workers
								where worker.Id == WorkerId && worker.IsActive == true&& worker.IsDeleted == false
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
										Id=s.Id,
										WorkerId=s.WorkerId,
										Description=s.Description,
										FromDate= s.FromDate,
										ToDate =s.ToDate
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
			if (result ==null)
			{
				return _apiResultService.Result(false, new {}, "Error", false);

			}
			return _apiResultService.Result(true, result, "Success", false);

		}

		public async Task<object> DeleteWorkerJob(int Id)
		{
			var result =await  db.WorkerJobs.FindAsync(Id);
			if (result == null)
				return _apiResultService.Result(false, "".ToList(), "Error", false);

			db.WorkerJobs.Remove(result);
			await db.SaveChangesAsync();
			return _apiResultService.Result(true, "".ToList(), "Success", false);

		}

		public async Task<object> AddWorkerJob(int jobId ,long workerId)
		{
			var IsExist = db.WorkerJobs.Where(w => w.JobId == jobId & w.WorkerId == workerId).FirstOrDefault();
			if (IsExist != null)
				return _apiResultService.Result(false, "Worker have this job", "Error", false);
			var result = db.WorkerJobs.Add(new WorkerJob()
			{
				JobId=jobId,
				WorkerId=workerId,
				CreatedAt=DateTime.Now,

			});
			await db.SaveChangesAsync();
			return _apiResultService.Result(true, result, "Success", false);

		}

		public async Task<object> AddWorkerExperienas(ExperienceDto experience)
		{
			var result =  db.WorkerExperiences.Add(new WorkerExperience
			{
				Description =experience.Description,
				FromDate = experience.FromDate,
				ToDate=experience.ToDate,
				CreatedAt = DateTime.Now,
				WorkerId = experience.WorkerId

			});
			await db.SaveChangesAsync();

			return _apiResultService.Result(true, result, "Success", false);

		}

		public async Task<object> UpdateWorkerExperienas(ExperienceDto experience)
		{
			var experienceResult = db.WorkerExperiences.Find(experience.Id);
			if (experienceResult == null)
				return _apiResultService.Result(false, new { }, "Error", false);
			experienceResult.Description = experience.Description;
			experienceResult.FromDate = experienceResult.FromDate;
			experienceResult.ToDate = experienceResult.ToDate;
			experienceResult.UpdatedBy = experienceResult.UpdatedBy;
			
			db.Entry(experienceResult).State = EntityState.Modified;
			await db.SaveChangesAsync();
			return _apiResultService.Result(true, experience, "Success", false);



		}

		public async Task<object> DeleteWorkerExperienas(int Id)
		{
			var result = await db.WorkerExperiences.FindAsync(Id);
			if (result == null) 
				return _apiResultService.Result(false, "".ToList(), "Error", false);

			db.WorkerExperiences.Remove(result);
			await db.SaveChangesAsync();
			return _apiResultService.Result(true, "".ToArray(), "Success", false);

		}
		public async Task<object> WorkerStatus(long workerId,bool Isbusy)
		{
			var worker = db.Workers.Find(workerId);
			if (worker == null)
				return _apiResultService.Result(false, "".ToList(), "Error", false);
			worker.Isbusy = Isbusy;
			db.Entry(worker).State = EntityState.Modified;
			await db.SaveChangesAsync();
			 return _apiResultService.Result(true, "".ToList(), "Success", false);

		}

		public async Task<object> GetNationalties()
		{
			var result = await (from nationalty in db.Nationalties
								
								select new
								{
									Id = nationalty.Id,
									Name = nationalty.Name,
									NameArabic =nationalty.NameArabic

								}).ToListAsync();
			if (result.Count == 0)
			{
				return _apiResultService.Result(false, "".ToList(), "Error", false);
			}

			return _apiResultService.Result(true, result, "Success", false);

		}
		public async Task<object> GetWorkerJobs()
		{
			var result = await (from job in db.Jobs

								select new
								{
									Id = job.Id,
									Name = job.Name,
									Description = job.Description

								}).ToListAsync();
			if (result.Count == 0)
			{
				return _apiResultService.Result(false, "".ToList(), "Error", false);
			}

			return _apiResultService.Result(true, result, "Success", false);

		}

		public async Task<object> GetWorkerByUserId(long userId)
		{
			var result = await (from worker in db.Workers
								where worker.UserId == userId && worker.IsActive == true
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
			if (result == null)
			{
				return _apiResultService.Result(false, new { }, "Error", false);

			}
			return _apiResultService.Result(true, result, "Success", false);

		}
	}
}