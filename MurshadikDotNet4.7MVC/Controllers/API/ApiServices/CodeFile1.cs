using MurshadikCP.Controllers.API.ApiServices.Interfaces;
using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System.Linq;


namespace MurshadikCP.Controllers.API.ApiServices
{
    public class ReefComponentService : IReefComponentService
    {
        mlaraEntities db = new mlaraEntities();
        public readonly IApiResultService _apiResultService = new ResultService();
        public object GetReefComponentList()
        {
            var ReefComponentList = db.ReefComponents.Select(s => new
            {
                id = s.Id,
                s.name,
                s.icon,
            }).ToList();
            return _apiResultService.Result(true, ReefComponentList, "Success", null);
        }

        public object GetReefComponentSuggestionsById(int id)
        {
            var ReefComponentSuggestions = db.ReefComponentSuggestions.Where(x => x.ReefComponentId == id).Select(s => new
            {
                id = s.Id,
                s.name,
            }).ToList();
            return _apiResultService.Result(true, ReefComponentSuggestions, "Success", null);
        }
    }
}