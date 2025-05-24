using MurshadikCP.Controllers.API.ApiServices.Interfaces;
using MurshadikCP.Controllers.API.ApiViewModel;
using MurshadikCP.Models.DB;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;

namespace MurshadikCP.Controllers.API.ApiServices
{
    public class VideoService : IVideoService
    {
    public readonly IApiResultService _apiResultService = new ResultService();     
        mlaraEntities db = new mlaraEntities();
        // when status == 1 mean the vieo is active
        //when status == 2 mean the video is not active 
        public long status = 1;
        string hostURL = WebConfigurationManager.AppSettings["publicHostUrl"];
        public object GetGallery(long UserId)
        {
            try
            {
                var videos = (from video in db.Videos
                              join user in db.users on video.UserId equals user.id
                              where video.UserId.Equals(UserId) 
                              select
                              new VideoViewModel
                              {
                                  Id = video.Id.ToString(),
                                  Title = video.Title,
                                  ConsultantName = user.name + " " + user.last_name,
                                  CreatAt = video.CreatedAt.ToString(),                                 
                                  Description = video.Description,
                                  Location = hostURL + video.Path + video.Id.ToString() + video.Extension,
                                  Category = video.VideoCategory.Name,
                                  Tag = video.VideoTag.TagName,
                                  VideoStatus = video.VideoStatusId,
                                  VideoLike= video.VideoLikes.Count(),
                                  Department = video.VideoCategory.Categories.Where(a => a.Id == video.DepartmentId).Select(s => s.Name).FirstOrDefault(),
                                  UserId = video.UserId.ToString(),
                                  IsHidden = video.IsHidden
                              }).ToList();
                if (videos.Equals(null))
                {
                    return _apiResultService.Result(false, "you gallery is empty", "Error", false);

                }

                return _apiResultService.Result(true, videos, "Success", false);         
            }
            catch (Exception ex)
            {
                return _apiResultService.Result(false, null, "Error", false);
            }
        }
        public object GetVideo(Guid VideoId)
        {     
            try
            {
                var Video = (from video in db.Videos
                              join user in db.users on video.UserId equals user.id
                              where video.Id.Equals(VideoId) && video.VideoStatusId.Equals(status)
                              select
                                new VideoViewModel
                                {
                                    Id = video.Id.ToString(),
                                    Title = video.Title,
                                    ConsultantName = user.name + " " + user.last_name,
                                    CreatAt = video.CreatedAt.ToString(),
                                    Description = video.Description,
                                    VideoLike = video.VideoLikes.Count(),
                                    Location = hostURL + video.Path + video.Id.ToString() + video.Extension,
                                    Category = video.VideoCategory.Name,
                                    Tag = video.VideoTag.TagName,
                                    Department = video.VideoCategory.Categories.Where(a => a.Id == video.DepartmentId).Select(s => s.Name).FirstOrDefault(),
                                    UserId = video.UserId.ToString(),

                                    Comments = (from v in video.VideoComments
                                                where video.Id.Equals(v.VideoId) && v.ParentId.Equals(null)
                                                select new CommentViewModel
                                                {
                                                    Id = v.Id,
                                                    Comment = v.Comment,
                                                    User = db.users.Where(w=>w.id.Equals(v.CommentBy)).Select(s=>s.name + " " + s.last_name).FirstOrDefault(),
                                                    Date = v.CreatedAt,
                                                    Reply = v.CommentsReplies.Select(sel => new CommentReplyViewModel
                                                    {
                                                        Id = sel.Id,
                                                        Comment = sel.Comment,
                                                        Date = sel.CreatedAt,
                                                        User = db.users.Where(w => w.id.Equals(sel.CommentBy)).Select(s => s.name + " " + s.last_name).FirstOrDefault(),
                                                    }).ToList()
                                                }).ToList()
                                }).FirstOrDefault();
                if (Video.Equals(null))
                {
                    return _apiResultService.Result(false, "not found", "Error", false);

                }
                return _apiResultService.Result(true, Video, "Success", false);             
            }
            catch (Exception ex)
            {
                return _apiResultService.Result(false, null, "Error", false);            
            }
        }
        public object GetVideoByCategoryId(long CategoryId)
        {
            try
            {
                var videos = (from video in db.Videos
                              join user in db.users on video.UserId equals user.id
                              where video.VideoCategoryId.Equals(CategoryId) && video.VideoStatusId.Equals(status) && video.IsHidden==(false)
                              select
                              new VideoViewModel
                              {
                                  Id = video.Id.ToString(),
                                  Title = video.Title,
                                  ConsultantName = user.name + " " + user.last_name,
                                  CreatAt = video.CreatedAt.ToString(),
                                  Description = video.Description,
                                  Location = hostURL + video.Path + video.Id.ToString() + video.Extension,
                                  Category = video.VideoCategory.Name,

                                  Tag = video.VideoTag.TagName,
                                  Department = video.VideoCategory.Categories.Where(a => a.Id == video.DepartmentId).Select(s => s.Name).FirstOrDefault(),
                                  UserId = video.UserId.ToString()
                              }).ToList();

                if (videos.Count.Equals(0))
                {
                    return _apiResultService.Result(false, "no data are found", "Error", false);
                }

                return _apiResultService.Result(true, videos, "Success", false);
            }
            catch (Exception ex)
            {
                return _apiResultService.Result(false, null, "Error", false);
            }
        }
        public object PostVideo(Video video , HttpPostedFile file)
        {
            var path = Path.Combine(HostingEnvironment.MapPath("~/Media/ConsultantsVideos/"), video.Id + Path.GetExtension(file.FileName));
           
            file.SaveAs(path);          
            try
            {
                if (!db.VideoCategories.Any(any => any.Id.Equals(video.VideoCategoryId) && any.ParentId == null))
                {
                    return _apiResultService.Result(false, "review category  data", "Error", false);
                }
                if (!db.VideoCategories.Any(a => a.Id == (video.DepartmentId) && a.ParentId == (video.VideoCategoryId)))
                {
                    return _apiResultService.Result(false, "review Department  data", "Error", false);
                }
                db.Videos.Add(video);
                db.SaveChanges();
                var data = hostURL + "Media/ConsultantsVideos/" + video.Id + Path.GetExtension(file.FileName);
                return _apiResultService.Result(true, data, "Success", false);        
            }
            catch (Exception ex)
            {
                File.Delete(path);
                return _apiResultService.Result(false, null, "Error", false);               
            }
        }
        public object PutVideo(Guid VideoId, string Title, string Description, string Tage,long DepartmentId,long CategoryId, bool IsHidden)
        {
            try
            {
                var video = db.Videos.Find(VideoId);
                //update dos not include Category and Department
                if (CategoryId==0&& DepartmentId==0)
                {
                    video.Title = Title;
                    video.Description = Description;
                    video.UpdatedAt = DateTime.Now;                  
                    video.VideoTag.TagName = Tage;
                    video.IsHidden = IsHidden;
                }
                //update  include Category and Department
                else
                {
                    if (!db.VideoCategories.Any(any => any.Id.Equals(CategoryId) && any.ParentId == null))
                    {
                        return _apiResultService.Result(false, "review category  data", "Error", false);
                    }
                    if (!db.VideoCategories.Any(a => a.Id == (DepartmentId) && a.ParentId == (CategoryId)))
                    {
                        return _apiResultService.Result(false, "review Department  data", "Error", false);
                    }
                    if (!db.Videos.Any(any => any.Id.Equals(VideoId)))
                    {
                        return _apiResultService.Result(false, "not found", "Error", false);
                    }
                    video.Title = Title;
                    video.Description = Description;
                    video.UpdatedAt = DateTime.Now;
                    video.VideoCategoryId = CategoryId;
                    video.DepartmentId = DepartmentId;
                    video.VideoTag.TagName = Tage;
                    video.IsHidden = IsHidden;

                }
                db.Entry(video).State = EntityState.Modified;
                db.SaveChanges();
                return _apiResultService.Result(true, null, "Success", false);              
                         
            }
            catch (Exception ex)
            {
                return _apiResultService.Result(false, null, "Error", false);              
            }
        }
      
    }
}