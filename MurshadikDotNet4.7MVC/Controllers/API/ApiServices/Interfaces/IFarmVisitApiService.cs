using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MurshadikCP.Controllers.API.ApiServices.Interfaces
{
	public interface IFarmVisitApiService
	{

		object AddFarmVisit(long UserId, long CityId, long RegionId, DateTime VisitDate, string FarmName, string PurposeOfVisit, string Location, List<HttpPostedFile> httpPostedFile);
		object GetFarmVisit(long UserId);
		object GetFarmVisitById(long FarmVisitId);
		 bool CheckingFarmVisitService();

	}
}
