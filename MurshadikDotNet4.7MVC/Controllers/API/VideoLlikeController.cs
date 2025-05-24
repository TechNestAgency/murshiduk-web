using MurshadikCP.Controllers.API.ApiServices;
using MurshadikCP.Controllers.API.ApiServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace MurshadikCP.Controllers.API
{
    [RoutePrefix("Api/Murshadik/videoLike")]
    public class VideoLlikeController : ApiController
    {
        public readonly IVideoLikeService _videoLikeService = new VideoLikeService();
        public readonly IApiResultService _apiResultService = new ResultService();
        [Authorize]
        [HttpPost]
        [Route("AddLike")]
        public object AddLike()
        {
            try
            {
                var data = HttpContext.Current.Request;
                string VideoId = data.Form["VideoId"];
                var identity = User.Identity as ClaimsIdentity;
                string user_id = identity.Claims.FirstOrDefault(x => x.Type.Equals("userid")).Value;
                string LikeStatus = data.Form["LikeStatus"];
                int[] stats = { 0, 1, 2 };

                if (!string.IsNullOrEmpty(VideoId) && !string.IsNullOrEmpty(LikeStatus) && stats.Any(s => s == int.Parse(LikeStatus)))
                {
                    return _videoLikeService.AddLike(Guid.Parse(VideoId), long.Parse(user_id), int.Parse(LikeStatus));
                }
                return _apiResultService.Result(false, " data can't be null Or Empty Or out of Rang", "Error", false);

            }
            catch (Exception)
            {

                return _apiResultService.Result(false, "review data", "Error", false);
            }
           
        }
        [Authorize]
        [HttpGet]
        [Route("GetLike")]
        public object GetLike(string VideoId)
        {
            if (!string.IsNullOrEmpty(VideoId))
            {
                try
                {
                    var identity = User.Identity as ClaimsIdentity;
                    string user_id = identity.Claims.FirstOrDefault(x => x.Type.Equals("userid")).Value;
                    return _videoLikeService.GetVideoLike(Guid.Parse(VideoId), long.Parse(user_id));
                }
                catch (Exception ex)
                {    
                     return _apiResultService.Result(false, "review data", "Error", false);
                }
            }
            return _apiResultService.Result(false, "reviw VideoId", "Error", false);
        }
    }
}
