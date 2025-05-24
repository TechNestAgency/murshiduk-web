using MurshadikCP.Controllers.API.ApiServices.Interfaces;
using MurshadikCP.Controllers.API.ApiViewModel;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Controllers.API.ApiServices
{
    public class VideoCommentService : IVideoCommentService
    {
        public readonly IApiResultService _apiResultService = new ResultService();
        public Data data = new Data();
        mlaraEntities db = new mlaraEntities();
        VideoComment _Comment;
        public object AddComment(Guid VideoId, long UserId, string Comment)
        {
            _Comment = new VideoComment
                {
                    VideoId = VideoId,
                    Comment = Comment,
                    CommentBy = UserId,
                    CreatedAt = DateTime.Now,
                };
            return SaveComment(_Comment);
        }

        public object AddCommentReply(Guid VideoId, long UserId, string Comment, long CommentId)
        {
            try
            {
                if (!db.VideoComments.Any(any => any.Id.Equals(CommentId)&& any.VideoId==VideoId&& any.ParentId==null))
                {
                    return _apiResultService.Result(false, "Comment  not found ", "Error", false);
                }
                _Comment = new VideoComment
                {
                    VideoId = VideoId,
                    Comment = Comment,
                    CommentBy = UserId,
                    ParentId = CommentId,
                    CreatedAt = DateTime.Now,
                };
                return SaveComment(_Comment);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public object GetVideoComment(Guid VideoId)
        {
            try
            {
                var Comments = (from comment in db.VideoComments

                                where comment.VideoId.Equals(VideoId) && comment.ParentId.Equals(null)
                                select new CommentViewModel
                                {
                                    Id = comment.Id,
                                    Comment = comment.Comment,
                                    User = db.users.Where(w => w.id.Equals(comment.CommentBy)).Select(s => s.name + " " + s.last_name).FirstOrDefault(),
                                    Date = comment.CreatedAt,
                                    Reply = comment.CommentsReplies.Select(sel => new CommentReplyViewModel
                                    {
                                        Id = sel.Id,
                                        User = db.users.Where(w => w.id.Equals(sel.CommentBy)).Select(s => s.name + " " + s.last_name).FirstOrDefault(),
                                        Comment = sel.Comment,
                                        Date = sel.CreatedAt,
                                    }).ToList()
                                }).ToList();
                if (Comments.Count.Equals(0))
                {
                    return _apiResultService.Result(false, "not found", "Error", false);
                }
                return _apiResultService.Result(true, Comments, "Success", false);

            }
            catch (Exception ex)
            {

                return _apiResultService.Result(false, null, "Error", false);
            }
          
        }
        private object SaveComment(VideoComment videoComment)
        {
            try
            {
                db.VideoComments.Add(videoComment);
                db.SaveChanges();
                return _apiResultService.Result(true, videoComment.Comment, "Success", false);             
            }
            catch (Exception)
            {
                return _apiResultService.Result(false, null, "Error", false);           
            }
        }
    }
}