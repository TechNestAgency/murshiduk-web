using MurshadikCP.Controllers.API.ApiServices.Interfaces;
using MurshadikCP.Controllers.API.ApiViewModel;
using MurshadikCP.Interface;
using MurshadikCP.Models.DB;
using MurshadikCP.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;

namespace MurshadikCP.DataModel
{
    public class VideoBoardServices : IVideoBoard
    {

        mlaraEntities db = new mlaraEntities();

        public List<VideoViewModelForDashboard> GetGallery(VideoSearchViewModel videoSearch)
        {
            var videos = new List<VideoViewModelForDashboard>();
			if (videoSearch.VideoCategoryId==0)

			{

                videos =(from video in db.Videos
                 join user in db.users on video.UserId equals user.id
                 join category in db.VideoCategories on video.DepartmentId equals category.Id
                 select new VideoViewModelForDashboard
                 {
                     Id = video.Id.ToString(),
                     Title = video.Title,
                     ConsultantName = user.name + " " + user.last_name,
                     ConsultantPhone = user.phone,
                     Location = video.Path + video.Id.ToString() + video.Extension,
                     Category = video.VideoCategory.Name,
                     Tag = video.VideoTag.TagName,
                     Department = video.VideoCategory.Categories.Where(a => a.Id == video.DepartmentId).Select(s => s.Name).FirstOrDefault(),
                     UserId = video.UserId.ToString(),
                     CreatAt = video.CreatedAt,
                     IsActive = video.VideoStatusId,
                     CategoryId = video.VideoCategoryId,
                     DepartmentId = (int)video.DepartmentId,
                     Date = video.CreatedAt,
                     Description = video.Description,
                 }).OrderByDescending(o => o.CreatAt).ToList();
            }
			else
			{
                videos = (from video in db.Videos
                          join user in db.users on video.UserId equals user.id
                          join category in db.VideoCategories on video.DepartmentId equals category.Id
                          where (video.VideoCategoryId == videoSearch.VideoCategoryId)
                          select new VideoViewModelForDashboard
                          {
                              Id = video.Id.ToString(),
                              Title = video.Title,
                              ConsultantName = user.name + " " + user.last_name,
                              ConsultantPhone = user.phone,
                              Location = video.Path + video.Id.ToString() + video.Extension,
                              Category = video.VideoCategory.Name,
                              Tag = video.VideoTag.TagName,
                              Department = video.VideoCategory.Categories.Where(a => a.Id == video.DepartmentId).Select(s => s.Name).FirstOrDefault(),
                              UserId = video.UserId.ToString(),
                              CreatAt = video.CreatedAt,
                              IsActive = video.VideoStatusId,
                              CategoryId = video.VideoCategoryId,
                              DepartmentId = (int)video.DepartmentId,
                              Date = video.CreatedAt,
                              Description = video.Description,
                          }).OrderByDescending(o => o.CreatAt).ToList();


            }
            //videos = (from video in db.Videos
            //              join user in db.users on video.UserId equals user.id
            //              join category in db.VideoCategories on video.DepartmentId equals category.Id
            //              where(video.VideoCategoryId == videoSearch.VideoCategoryId)
            //              select new VideoViewModelForDashboard
            //              {
            //                  Id = video.Id.ToString(),
            //                  Title = video.Title,
            //                  ConsultantName = user.name + " " + user.last_name,
            //                  ConsultantPhone = user.phone,
            //                  Location = video.Path + video.Id.ToString() + video.Extension,
            //                  Category = video.VideoCategory.Name,
            //                  Tag = video.VideoTag.TagName,
            //                  Department = video.VideoCategory.Categories.Where(a => a.Id == video.DepartmentId).Select(s => s.Name).FirstOrDefault(),
            //                  UserId = video.UserId.ToString(),
            //                  CreatAt = video.CreatedAt,
            //                  IsActive = video.VideoStatusId,
            //                  CategoryId= video.VideoCategoryId,
            //                  DepartmentId=(int)video.DepartmentId,
            //                  Date = video.CreatedAt,
            //                  Description = video.Description,
            //              }).OrderByDescending(o => o.CreatAt).ToList();

            //if (videoSearch != null && videoSearch.VideoCategoryId > 0)
            //    videos = videos.Where(x => x.CategoryId == videoSearch.VideoCategoryId).ToList();

            if (videoSearch != null && videoSearch.DepartmentId > 0)
                videos = videos.Where(x => x.DepartmentId == videoSearch.DepartmentId).ToList();

            if (videoSearch != null && videoSearch.UserId > 0)
                videos = videos.Where(x => x.UserId == videoSearch.UserId.ToString()).ToList();

            if (videoSearch != null && videoSearch.start_date.Date.Year>1)
                videos = videos.Where(x => x.CreatAt.Date >= videoSearch.start_date.Date).ToList();

            if (videoSearch.end_date.Date.Year > 1)
                videos = videos.Where(x=> x.CreatAt.Date <= videoSearch.end_date.Date).ToList();

            if(videoSearch.TypeSort)
                videos = videos.OrderBy(o => o.CreatAt).ToList();

            if (videoSearch != null && videoSearch.SearshValue != null)
            {
                videos = videos.Where(x => x.Title.Trim().Contains(videoSearch.SearshValue) ||
                         x.Tag.Contains(videoSearch.SearshValue) ||
                         x.Category.Trim().Contains(videoSearch.SearshValue) ||
                         x.Tag.Contains(videoSearch.SearshValue) ||
                         x.Description.Contains(videoSearch.SearshValue) ||
                         x.Category.Trim().Contains(videoSearch.SearshValue) ||
                         x.ConsultantName.Trim().Contains(videoSearch.SearshValue)).ToList();
            }

            return videos.ToList();
        }
        
    }

}