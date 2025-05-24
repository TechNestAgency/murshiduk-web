using MurshadikCP.Controllers.API.ApiServices;
using MurshadikCP.Controllers.API.ApiServices.Interfaces;
using System;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace MurshadikCP.Controllers.API
{
    [RoutePrefix("Api/Murshadik/videoComment")]
    public class VideoCommentController : ApiController
    {      
        public readonly IVideoCommentService _videoCommentServices =new VideoCommentService();
        public readonly IApiResultService _apiResultService = new ResultService();
        [Authorize]
        [HttpPost]
        [Route("AddComment")]
        public object AddComment()
        {          
            var data = HttpContext.Current.Request;
            var identity = User.Identity as ClaimsIdentity;
            string user_id = identity.Claims.FirstOrDefault(x => x.Type.Equals("userid")).Value;
            string VideoId = data.Form["VideoId"];
            string Comment = data.Form["Comment"];
            string CommentId = data.Form["CommentId"];
            try
            {
                if (!(string.IsNullOrEmpty(VideoId) || string.IsNullOrEmpty(Comment)))
                {
                    if (string.IsNullOrEmpty(CommentId))
                    {
                        return _videoCommentServices.AddComment(Guid.Parse(VideoId), long.Parse(user_id), Comment);
                    }
                    else
                    {
                        return _videoCommentServices.AddCommentReply(Guid.Parse(VideoId), long.Parse(user_id), Comment, long.Parse(CommentId));
                    }
                }
                return _apiResultService.Result(false, " data can't be null", "Error", false);               
            }
            catch (Exception ex)
            {
                return _apiResultService.Result(false, " review data", "Error", false);
            }
           
        }
         [Authorize]
         [HttpGet]
        [Route("GetComment")]
        public object GetComment(string videoId)
        {          
    
            try
            {
                if (!(string.IsNullOrEmpty(videoId)))
                {
                    
                        return _videoCommentServices.GetVideoComment(Guid.Parse(videoId));
                }
                else
                {
                    return _apiResultService.Result(false, " data can't be null", "Error", false);
                }
                }
               
            catch (Exception ex)
            {
                return _apiResultService.Result(false, " data can't be null", "Error", false);
            }
           
        }

    }
}
