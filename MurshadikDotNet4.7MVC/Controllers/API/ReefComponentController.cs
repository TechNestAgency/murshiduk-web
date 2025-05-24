

using MurshadikCP.Controllers.API.ApiServices;
using MurshadikCP.Controllers.API.ApiServices.Interfaces;
using System.Web.Http;

namespace   MurshadikCP.Controllers.API
{
    [RoutePrefix("Api/Murshadik/Reef")]
    public class ReefComponentsController : ApiController
    {
        public readonly IApiResultService _apiResultService = new ResultService();
        public readonly IReefComponentService _reefComponentService = new ReefComponentService();

        [Authorize]
        [HttpGet]
        [Route("getReefComponentList")]
        public object GetReefComponentList()
        {
            return _reefComponentService.GetReefComponentList();
        }
        [Authorize]
        [HttpGet]
        [Route("getReefComponentSuggestionsById")]
        public object GetReefComponentSuggestionsById(int id)
        {
            return _reefComponentService.GetReefComponentSuggestionsById(id);
        }
    }
}