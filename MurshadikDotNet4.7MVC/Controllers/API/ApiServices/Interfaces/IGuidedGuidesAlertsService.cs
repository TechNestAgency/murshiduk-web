using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MurshadikCP.Controllers.API.ApiServices.Interfaces
{
	public interface IGuidedGuidesAlertsService
	{
		 Task<object> PostGuidedGuide(long UserId,string Region_id, string GuideAlert, string[] Cities,string[] Skills);
		Task<object>  GetGuidedGuide(long UserId);
	}
}
