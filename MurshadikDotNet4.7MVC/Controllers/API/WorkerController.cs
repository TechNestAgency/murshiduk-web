using MurshadikCP.Models.DB;
using System.Web.Http;
using System.Linq;
using System.Security.Claims;
using MurshadikCP.Controllers.API.ApiServices.Interfaces;
using MurshadikCP.Controllers.API.ApiServices;
using System;
using MurshadikCP.Controllers.API.ApiViewModel;
using System.Threading.Tasks;
namespace MurshadikCP.Controllers.API
{
	[RoutePrefix("Api/Murshadik/worker")]
	public class WorkerController : ApiController
	{
		mlaraEntities db = new mlaraEntities();
		public readonly IApiResultService _apiResultService = new ResultService();
		public readonly IWorkerService _workerService = new WorkerService();
		[Authorize]
		[HttpGet]
		[Route("GetAllWorker")]
		public async Task<object> GetAll(string JobId)
		{
			if (string.IsNullOrEmpty(JobId))
			{
				return _apiResultService.Result(false, " data can't be null", "Error", false);
			}
			try
			{
				return await _workerService.GetAll(int.Parse(JobId));
			}
			catch (Exception ex)
			{
				return _apiResultService.Result(false, "review data", "Error", false);
			}
		}
		[Authorize]
		[HttpGet]
		[Route("GetAllWorkerByPage")]
		public async Task<object> GetAll(int pageSize =20, int? pageNumber = 0)
		{

			int noOfPage =pageNumber??1;
			if (pageNumber == 0||pageNumber ==null) { pageNumber = 1;noOfPage = 1; }
			try
			{
				return await _workerService.GetAllWorkerByPage(noOfPage,(int)pageNumber,pageSize);
			}
			catch (Exception ex)
			{
				return _apiResultService.Result(false, "review data", "Error", false);
			}
		}
		[Authorize]
		[HttpGet]
		[Route("GetWorker")]
		public async Task<object> GetWorker(string WorkerId)
		{
			if (string.IsNullOrEmpty(WorkerId))
			{
				return _apiResultService.Result(false, " data can't be null", "Error", false);
			}
			try
			{
				return await _workerService.GetWorker(long.Parse(WorkerId));
			} 
			catch (Exception ex)
			{
				return _apiResultService.Result(false, "review data", "Error", false);
			}
		}
		[Authorize]
		[HttpPost]
		[Route("AddWorker")]
		public async Task<object> AddWorker ([FromBody] WorkerData worker)
		{
			if (!ModelState.IsValid)
			{
				return _apiResultService.Result(false, " data can't be null", "Error", false);
			}
			try
			{
				var identity = User.Identity as ClaimsIdentity;
				string UserId = identity.Claims.FirstOrDefault(x => x.Type.Equals("userid")).Value;
				worker.UserId = long.Parse(UserId);
				return await _workerService.AddWorker(new Worker()
				{
					Phone = worker.Phone,
					IDNumber = worker.IDNumber,
					Address = worker.Address,
					DateOfBirth = (DateTime)worker.DateOfBirth,
					NationaltyId= worker.NationaltyId,
					ExpectedSalary=	worker.ExpectedSalary,
					UserId =	worker.UserId,
					Isbusy =false,
					IsDeleted=false,
					IsActive = true,
					CreatedAt = DateTime.UtcNow,
					UpdatedAt = DateTime.UtcNow,
					CteatedBy = worker.UserId

				});
			}
			catch (Exception ex)
			{
				return _apiResultService.Result(false, "review data", "Error", false);
			}
		}
		[Authorize]
		[HttpPut]
		[Route("UpdateWorker")]
		public async Task<object> UpdateWorker([FromBody]WorkerData worker)
		{
			if (!ModelState.IsValid)
			{
				return _apiResultService.Result(false, " data can't be null", "Error", false);
			}
			try
			{
				var identity = User.Identity as ClaimsIdentity;
				string UserId = identity.Claims.FirstOrDefault(x => x.Type.Equals("userid")).Value;
				worker.UserId = long.Parse(UserId);
				return await _workerService.UpdateWorker(new Worker()
				{
					Id=worker.Id,
					Phone = worker.Phone,
					IDNumber = worker.IDNumber,
					Address = worker.Address,
					DateOfBirth = (DateTime)worker.DateOfBirth,
					NationaltyId = worker.NationaltyId,
					ExpectedSalary = worker.ExpectedSalary,
					UserId = worker.UserId,
					Isbusy = worker.Isbusy,
					IsActive = false,
					IsDeleted = false,
					CreatedAt = DateTime.UtcNow,
					UpdatedAt = DateTime.UtcNow,
					CteatedBy = worker.UserId

				});
			}
			catch (Exception ex)
			{
				return _apiResultService.Result(false, "review data", "Error", false);
			}
		}
		[Authorize]
		[HttpDelete]
		[Route("DeleteWorkerJob")]
		public async Task<object> DeleteWorkerJob(string WorkerjobId)
		{
			if (string.IsNullOrEmpty(WorkerjobId))
			{
				return _apiResultService.Result(false, " data can't be null", "Error", false);
			}
			try
			{
				return await _workerService.DeleteWorkerJob(int.Parse(WorkerjobId));
			}
			catch (Exception ex)
			{
				return _apiResultService.Result(false, "review data", "Error", false);
			}
		}
		[Authorize]
		[HttpPost]
		[Route("AddWorkerJob")]
		public async Task<object> AddWorkerJob(string jobId, string workerId)
		{
			if (string.IsNullOrEmpty(jobId) || string.IsNullOrEmpty(workerId))
			{
				return _apiResultService.Result(false, " data can't be null", "Error", false);
			}
			try
			{
				return await _workerService.AddWorkerJob(int.Parse(jobId),long.Parse(workerId));
			}
			catch (Exception ex)
			{
				return _apiResultService.Result(false, "review data", "Error", false);
			}
		}
		[Authorize]
		[HttpPost]
		[Route("AddWorkerExperienas")]
		public async Task<object> AddWorkerExperienas([FromBody]ExperienceDto experience)
		{
			if (!ModelState.IsValid)
			{
				return _apiResultService.Result(false, " data can't be null", "Error", false);
			}
			try
			{
				return await _workerService.AddWorkerExperienas(experience);
			}
			catch (Exception ex)
			{
				return _apiResultService.Result(false, "review data", "Error", false);
			}
		}
		[Authorize]
		[HttpPut]
		[Route("UpdateWorkerExperienas")]
		public async Task<object> UpdateWorkerExperienas([FromBody]ExperienceDto experience)
		{
			if (!ModelState.IsValid)
			{
				return _apiResultService.Result(false, " data can't be null", "Error", false);
			}
			try
			{
				return await _workerService.UpdateWorkerExperienas(experience);
			}
			catch (Exception ex)
			{
				return _apiResultService.Result(false, "review data", "Error", false);
			}
		}
		[Authorize]
		[HttpDelete]
		[Route("DeleteWorkerExperienas")]
		public async Task<object> DeleteWorkerExperienas(string experienceId)
		{
			if (string.IsNullOrEmpty(experienceId))
			{
				return _apiResultService.Result(false, " data can't be null", "Error", false);
			}
			try
			{
				return await _workerService.DeleteWorkerExperienas(int.Parse(experienceId));
			}
			catch (Exception ex)
			{
				return _apiResultService.Result(false, "review data", "Error", false);
			}
		}
		[Authorize]
		[HttpPut]
		[Route("ChangeWorkerStatus")]
		public async Task<object> ChangeWorkerStatus(string workerId, bool Isbusy)
		{
			if (string.IsNullOrEmpty(workerId))
			{
				return _apiResultService.Result(false, new { }, "Error.  data can't be null", false);
			}
			try
			{
				return await _workerService.WorkerStatus(long.Parse(workerId),Isbusy);
			}
			catch (Exception ex)
			{
				return _apiResultService.Result(false, new { }, "Error. review data", false);
			}
		}
		[Authorize]
		[HttpGet]
		[Route("GetNationalties")]
		public async Task<object> GetNationalties()
		{	
			try
			{
				return await _workerService.GetNationalties();
			}
			catch (Exception ex)
			{
				return _apiResultService.Result(false, "review data", "Error", false);
			}
		}

		[Authorize]
		[HttpGet]
		[Route("GetWorkerJobs")]
		public async Task<object> GetWorkerJobs()
		{
			try
			{
				return await _workerService.GetWorkerJobs();
			}
			catch (Exception ex)
			{
				return _apiResultService.Result(false, "review data", "Error", false);
			}
		}
		[Authorize]
		[HttpGet]
		[Route("GetWorkerByUserId")]
		public async Task<object> GetWorkerByUserId()
		{
			var identity = User.Identity as ClaimsIdentity;
			string UserId = identity.Claims.FirstOrDefault(x => x.Type.Equals("userid")).Value;
			try
			{
				return await _workerService.GetWorkerByUserId(long.Parse(UserId));
			}
			catch (Exception ex)
			{
				return _apiResultService.Result(false, "review data", "Error", false);
			}
		}
	}
}