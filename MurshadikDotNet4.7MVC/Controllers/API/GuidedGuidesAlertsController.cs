using MurshadikCP.Controllers.API.ApiServices;
using MurshadikCP.Controllers.API.ApiServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace MurshadikCP.Controllers.API
{
    [RoutePrefix("Api/Murshadik/GuidedGuidesAlerts")]
    public class GuidedGuidesAlertsController : ApiController
    {
        public readonly IGuidedGuidesAlertsService _guidedGuidesAlertsService =new GuidedGuidesAlertsService();
        public readonly IApiResultService _apiResultService = new ResultService();

        //// GET: api/GuidedGuidesAlerts

        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET: api/GuidedGuidesAlerts/5
        //public string Get(int id)
        //{
        //    return "value";
        //}


        [Authorize]
        [HttpPost]
        [Route("PostGuideAlert")]
        // POST: api/GuidedGuidesAlerts
        public async Task<object> PostAsync()
        {
            var identity = User.Identity as ClaimsIdentity;
            string User_id = identity.Claims.FirstOrDefault(x => x.Type.Equals("userid")).Value;
            var data = HttpContext.Current.Request;
            string GuideAlert = data.Form["GuideAlert"];
            string Region_id = data.Form["region_id"];
            string [] Cities = data.Form["cities"].Split(',').Select(p => p.ToString()).ToArray();
            string [] Skills = data.Form["skills"].Split(',').Select(p => p.ToString()).ToArray();
			if (string.IsNullOrEmpty(GuideAlert) )
			{
                return _apiResultService.Result(false, " GuideAlert can't be null", "Error", false);
			}
            if (string.IsNullOrEmpty(Region_id))
			{
                return _apiResultService.Result(false, " Region_id can't be null", "Error", false);
            }

            return await _guidedGuidesAlertsService.PostGuidedGuide(long.Parse(User_id),Region_id,GuideAlert,Cities,Skills);
        }
        [Authorize]
        [HttpGet]
        [Route("GetGuidAlert")]
        public async Task<object> GetAsync()
		{
            var identity = User.Identity as ClaimsIdentity;
            string User_id = identity.Claims.FirstOrDefault(x => x.Type.Equals("userid")).Value;
			try
			{
            var GuidedGuideAlerts =await _guidedGuidesAlertsService.GetGuidedGuide(long.Parse(User_id));

            return _apiResultService.Result(true, GuidedGuideAlerts, "Success", true); 
			}
			catch (Exception)
			{

                return _apiResultService.Result(true, "", "Error", false);
            }
		}
       
    }
}
