using MurshadikCP.Controllers.API.ApiViewModel;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MurshadikCP.Controllers.API.ApiServices.Interfaces
{
	public interface IWorkerService
	{
		Task<object> GetAll(int JobId);
		Task<object> GetAllWorkerByPage(int noOfPage, int pageNumber, int pageSize);
		Task<object> GetWorker(long WorkerId);
		Task<object> AddWorker(Worker worker);
		Task<object> UpdateWorker(Worker worker);
		Task<object> DeleteWorkerJob(int Id);
        Task<object> AddWorkerJob(int jobId, long workerId);
		Task<object> AddWorkerExperienas(ExperienceDto experience);
		Task<object> UpdateWorkerExperienas(ExperienceDto experience);
		Task<object> DeleteWorkerExperienas(int Id);
		Task<object> WorkerStatus(long workerId, bool Isbusy);
		Task<object> GetNationalties();
		Task<object> GetWorkerJobs();
		Task<object> GetWorkerByUserId(long userId);
	}
}
