using MurshadikCP.Controllers.API.ApiViewModel;
using MurshadikCP.Controllers.Dashboard.DashboardViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MurshadikCP.Controllers.Dashboard.DashboardInterface
{
	public interface IWorkerBoardService
	{
		Task<IEnumerable<WorkerViewModel>> GetWorkerList();
		Task<WorkerDetail> GetWorker(long WorkerId);
	}
}