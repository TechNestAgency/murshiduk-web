using MurshadikCP.Controllers.API.ApiServices.Interfaces;
using MurshadikCP.Controllers.API.ApiViewModel;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MurshadikCP.Controllers.API.ApiServices
{
    public class VideoLikeService : IVideoLikeService
    {
        public readonly IApiResultService _apiResultService = new ResultService();
        mlaraEntities db = new mlaraEntities();
        public object AddLike(Guid videoId, long userId, int LikeStatus)
        {
            if (!db.Videos.Any(any => any.Id.Equals(videoId)))
            {
                return _apiResultService.Result(false, "not found", "Error", false);
            }
            var VideoLike = db.VideoLikes.Where(w => w.VideoId.Equals(videoId) && w.UserId.Equals(userId)).FirstOrDefault();
            if (VideoLike == null)
            {
                return CreateVideoLike(videoId, userId, LikeStatus);
            }
            else
            {
                switch (LikeStatus)
                {
                    case 1:
                        return UpdateVideoLike(VideoLike, LikeStatus);
                    case 2:
                        return UpdateVideoLike(VideoLike, LikeStatus);
                    case 0:
                        return UpdateVideoLike(VideoLike, LikeStatus);
                    default:
                        return _apiResultService.Result(false, "un expected case", "Error", false);
                }
            }
        }
        public object GetVideoLike(Guid videoId, long userId)
        {

            try
            {
                var data = db.VideoLikes.Where(w => w.VideoId.Equals(videoId))
                    .Select(s => new likeVewModel
                    {
                        Dislikecount = db.VideoLikes.Where(w => w.LikeStatus.Equals(2) && w.VideoId.Equals(videoId)).Count(),
                        likecount = db.VideoLikes.Where(w => w.LikeStatus.Equals(1) && w.VideoId.Equals(videoId)).Count(),
                        LikeStatus = db.VideoLikes.Any(any => any.UserId.Equals(userId) && any.VideoId.Equals(videoId)) ? db.VideoLikes.FirstOrDefault(f => f.UserId.Equals(userId) && f.VideoId.Equals(videoId)).LikeStatus : 0,
                    }).FirstOrDefault();

                if (data==null)
                {
                    return _apiResultService.Result(false, "not found", "Error", false);
                }
                return _apiResultService.Result(true, data, "Success", false);
            }
            catch (Exception ex)
            {
                return _apiResultService.Result(false, null, "Error", false);
            }

        }

        public object CreateVideoLike(Guid videoId, long userId, int Status)
        {
            try
            {
                var videoLike = new VideoLike
                {
                    LikeStatus = Status,
                    UserId = userId,
                    VideoId = videoId,
                    CreatedAt = DateTime.Now,
                };
                db.VideoLikes.Add(videoLike);
                db.SaveChanges();
                return _apiResultService.Result(true, null, "Success", false);
            }
            catch (Exception)
            {
                return _apiResultService.Result(false, null, "Error", false);

            }
        }
        public object UpdateVideoLike(VideoLike videoLike, int status)
        {
            try
            {
                videoLike.LikeStatus = status;
                videoLike.UpdatedAt = DateTime.Now;
                db.Entry(videoLike).State = EntityState.Modified;
                db.SaveChanges();
                return _apiResultService.Result(true, null, "Success", false);
            }
            catch (Exception)
            {
                return _apiResultService.Result(false, null, "Error", false);
            }

        }
    }
    public class likeVewModel
    {
        public int likecount { get; set; }
        public int Dislikecount { get; set; }
        public int? LikeStatus { get; set; }
    }
}