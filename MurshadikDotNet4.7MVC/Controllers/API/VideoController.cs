
using MurshadikCP.Controllers.API.ApiServices;
using MurshadikCP.Controllers.API.ApiServices.Interfaces;
using MurshadikCP.Controllers.API.ApiViewModel;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;

namespace MurshadikCP.Controllers.API
{
    [RoutePrefix("Api/Murshadik/video")]
    public class VideoController : ApiController
    {
        mlaraEntities db = new mlaraEntities();
        string hostURL = WebConfigurationManager.AppSettings["publicHostUrl"];
        public readonly IVideoService _videoService = new VideoService();
        public readonly IApiResultService _apiResultService = new ResultService();
        // GET api/<controller>
        [Authorize]
        [HttpGet]
        [Route("GetGallery")]
        public object Get()
        
        {
            var identity = User.Identity as ClaimsIdentity;
            string user_id = identity.Claims.FirstOrDefault(x => x.Type.Equals("userid")).Value;
            var userId = long.Parse(user_id);
            return _videoService.GetGallery(userId);           
        }
        // GET api/<controller>/5
        [Authorize]
        [HttpGet]
        [Route("GetVideo")]
        public object Get(string Id)
        {
            if (!string.IsNullOrEmpty(Id))
            {
                try
                {
                    Guid videoId = Guid.Parse(Id);
                    return _videoService.GetVideo(videoId);
                }
                catch (Exception ex)
                {
                    return _apiResultService.Result(false, " review data", "Error", false);
                }
            }
            return _apiResultService.Result(false, " data can't be null", "Error", false);
        }
        // POST api/<controller>
        [Authorize]
        [HttpPost]
        [Route("PostVideo")]
        public object Post()
        {
            var identity = User.Identity as ClaimsIdentity;
            string user_id = identity.Claims.FirstOrDefault(x => x.Type.Equals("userid")).Value;
            var data = HttpContext.Current.Request;
            long CategoryId,DepartmentId; 
            string Title, Description, Tag;
            HttpPostedFile httpPostedFile;
            try
            {
                int status = 2;
                // when status == 1 mean the vieo is active
                //when status == 2 mean the video is not active 
                CategoryId = long.Parse(data.Form["CategoryId"]);
                 DepartmentId = long.Parse(data.Form["DepartmentId"]);
                 Title = data.Form["Title"];
                 Description = data.Form["Description"];
                 Tag = data.Form["Tag"];
                httpPostedFile = HttpContext.Current.Request.Files["Video"];
                if (httpPostedFile.ContentType == null || CategoryId <=0 || DepartmentId <=0 || Title == null || Description == null)
                 return _apiResultService.Result(false, "your request data  not valid", "Error", false);
                 string[] mediaExtensions = {".avi", ".mp4",".wmv"};
                if (!mediaExtensions.Contains(Path.GetExtension(httpPostedFile.FileName).ToLower()))
                {
                    return _apiResultService.Result(false, "this file is not supported", "Error", false);
                }
                Guid videoId = new Guid();
                    var video = new Video()
                    {
                        Id = videoId = Guid.NewGuid(),
                        VideoCategoryId = CategoryId,
                        DepartmentId = DepartmentId,
                        Description = Description,
                        Title = Title,
                        Path = "Media/ConsultantsVideos/",
                        UserId = long.Parse(user_id),
                       
                        VideoStatusId = status,
                        VideoTag = new VideoTag()
                        {
                            TagName = Tag,
                            VideoId = videoId,
                            CreatedAt = DateTime.Now,
                        },
                        Extension = Path.GetExtension(httpPostedFile.FileName),
                        CreatedAt = DateTime.Now,
                        IsHidden = false
                    };
                    return _videoService.PostVideo(video, httpPostedFile);
            }
            catch (Exception ex)
            {
                return _apiResultService.Result(false, "review data", "Error", false);
            }

        }
        // PUT api/<controller>/5
        [Authorize]
        [HttpPut]
        [Route("PutVideo")]
        public object Put(string Id, string Title, string Description, string Tage ,long DepartmentId, long CategoryId,bool IsHidden = false)        
        {
            if (!string.IsNullOrEmpty(Id) && !string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(Description))
            {
                try
                {
                    Guid VideoId = Guid.Parse(Id);
                    return _videoService.PutVideo(VideoId, Title,Description,Tage, DepartmentId,CategoryId, IsHidden);
                }
                catch (Exception ex)
                {
                      return _apiResultService.Result(false, " review data", "Error", false);                 
                }
            }
            return _apiResultService.Result(false, " data can't be null", "Error", false);
        }
        [Authorize]
        [HttpGet]
        [Route("GetAllVideos")]      
        public object GetAll(string categoryId)
        {         
            if (string.IsNullOrEmpty(categoryId))
            {
                 return _apiResultService.Result(false, " data can't be null", "Error", false);
            }
            try
            {
                long CategoryId = long.Parse(categoryId);
                return _videoService.GetVideoByCategoryId(CategoryId);           
            }
            catch (Exception ex)
            {
                return _apiResultService.Result(false, "review data", "Error", false);
            }
        }
        [Authorize]
        [HttpGet]
        [Route("GetCategories")]
        public object GetCategories()
        {
            try
            {
                List<CategoryViewModel> categories = (from category in db.VideoCategories
                            where category.ParentId.Equals(null)
                            select new CategoryViewModel
                            {
                                Id = category.Id.ToString(),
                                Name = category.Name,
                                Image = hostURL + category.Image,
                                Sup = category.Categories.Select(sel => new SupCategoryViewModel
                                {
                                    Id = sel.Id.ToString(),
                                    Name = sel.Name
                                }).ToList()
                            }).ToList();
                return _apiResultService.Result(true, categories, "Success", false);            
            }
            catch (Exception ex)
            {
                return _apiResultService.Result(false, null, "Error", false);               
            }
        }
    }
}