using MurshadikCP.Controllers.API.ApiServices;
using MurshadikCP.Controllers.API.ApiServices.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;

namespace MurshadikCP.Controllers.API
{

    [RoutePrefix("Api/Murshadik/FarmVisitOrder")]
    public class FarmVisitOrderApiController : ApiController
    {
        public readonly IApiResultService _apiResultService = new ResultService();
        public readonly IFarmVisitApiService _farmVisitService = new FarmVisitApiService();

        // GET: api/FarmVisitOrder
        [Authorize]
        [HttpGet]
        [Route("Get")]
        public object Get()
        {
            var identity = User.Identity as ClaimsIdentity;
            string UserId = identity.Claims.FirstOrDefault(x => x.Type.Equals("userid")).Value;
                 _farmVisitService.GetFarmVisit(long.Parse(UserId));
            return _apiResultService.Result(true, _farmVisitService.GetFarmVisit(long.Parse(UserId)), "Success", false);

        }

        // GET: api/FarmVisitOrder/5
        [Authorize]
        [HttpGet]
        [Route("GetByOrderId")]
        public object Get(int farmOrderId)
        {          
            return _farmVisitService.GetFarmVisitById(farmOrderId);
        }
		// GET: api/FarmVisitOrder/5
		[Authorize]
		[HttpGet]
		[Route("CheckFarmVisitService")]
		public bool CheckFarmVisitService()
		{
			return _farmVisitService.CheckingFarmVisitService();
		}
		// POST: api/FarmVisitOrder
		[Authorize]
        [HttpPost]
        [Route("PostFarmVisitOrder")]
        public object Post()
            {
            var identity = User.Identity as ClaimsIdentity;
            string UserId = identity.Claims.FirstOrDefault(x => x.Type.Equals("userid")).Value;
            var data = HttpContext.Current.Request;
            long CityId, RegionId;
            DateTime VisitDate;
            string FarmName, PurposeOfVisit,Location;
            List<HttpPostedFile> httpPostedFile = new List<HttpPostedFile>() ;
			var farmService = _farmVisitService.CheckingFarmVisitService();
            if (!farmService)
            {
				return _apiResultService.Result(false, "".ToArray(), "الخدمة غير متاحة ", false);
			}
			try
			{
                FarmName = data.Form["FarmName"];
                PurposeOfVisit = data.Form["PurposeOfVisit"];
                Location = data.Form["Location"];
                RegionId = long.Parse(data.Form["RegionId"]);
                CityId = long.Parse(data.Form["CityId"]);
                var date = data.Form["VisitDate"].ToString();
                VisitDate= Convert.ToDateTime(date);
                
                if (RegionId <= 0 || CityId <= 0 || FarmName == null || PurposeOfVisit == null || Location == null)
                    return _apiResultService.Result(false, "your request data  not valid", "Error", false);
                for (int i = 0; i < data.Files.Count; i++)
                {                
                     httpPostedFile.Add(data.Files[i]);                 
                }
                _farmVisitService.AddFarmVisit(long.Parse(UserId), CityId, RegionId, VisitDate, FarmName, PurposeOfVisit, Location, httpPostedFile);
                return _apiResultService.Result(true, true, "Success", false);
                    
            }
            catch (Exception ex)
			{
                return _apiResultService.Result(false, "review data", "Error", false);
            }
        }
        // DELETE: api/FarmVisitOrder/5
        public void Delete(int id)
        {
        }
    }
}
