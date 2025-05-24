using MurshadikCP.Controllers.API.ApiServices;
using MurshadikCP.DataModel;
using MurshadikCP.Interface;
using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using MurshadikCP.Models.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MurshadikCP.Controllers
{
    [Authorize]
    public class VideoBoardController : BaseController
    {

        public readonly VideoBoardServices _videoBoardServices = new VideoBoardServices();
        public readonly IVideoCommentService _videoCommentServices = new VideoCommentService();
        UserInfo userInformation = new UserInfo();
        public ActionResult Index(VideoSearchViewModel videoSearch)
        {
            string[] CategoryId = System.Configuration.ConfigurationManager.AppSettings["VideoCategoryId"].Split(',');
            if (currentPageID("VideoBoard") > 0)
            {
                if (!CurrentUser.canView(currentPageID("VideoBoard")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            userInformation = (UserInfo)Session["User"];
            if (userInformation.RoleId == (int)Role.VegetarianGuide)
            {
                //إرشاد نباتي
                videoSearch.VideoCategoryId = int.Parse(CategoryId[0]);
            }
            else if (userInformation.RoleId == (int)Role.AnimalGuide)
            {
                //إرشاد سمكي
                videoSearch.VideoCategoryId = int.Parse(CategoryId[1]);
            }
            else if(userInformation.RoleId == (int)Role.FishGuide)
            {
                //إرشاد حيواني
                videoSearch.VideoCategoryId = int.Parse(CategoryId[2]);

            }
            else if (userInformation.RoleId == (int)Role.Manager || userInformation.RoleId == (int)Role.SuperAdmin)
			{
                videoSearch.VideoCategoryId = 0;

            }
            else
            {
                return RedirectToAction("NotAllow", "Custom");
            }

			// ViewBag.CategoryId = new SelectList(db.VideoCategories, "Id", "Name");
			if (videoSearch.VideoCategoryId==0)
			{
                ViewBag.DepartmentId = new SelectList(db.VideoCategories.Where(x => x.ParentId !=null ), "id", "name");
            }
			else
			{
                ViewBag.DepartmentId = new SelectList(db.VideoCategories.Where(x => x.ParentId == videoSearch.VideoCategoryId), "id", "name");

            }
            
            ViewBag.UserId = new SelectList(db.users.Where(x => x.role_id == 6), "id", "name");

            if (videoSearch != null && videoSearch.start_date.Date.Year > 1 && videoSearch.end_date.Date.Year > 1) 
            {
                ViewBag.start_date = videoSearch.start_date;
                ViewBag.end_date = videoSearch.end_date; 
            }

            return View(_videoBoardServices.GetGallery(videoSearch));
        }

        public String ajaxSortVideo(VideoSearchViewModel videoSearch) {
            string[] CategoryId = System.Configuration.ConfigurationManager.AppSettings["VideoCategoryId"].Split(',');
            //if (currentPageID("VideoBoard") > 0)
            //{
            //    if (!CurrentUser.canView(currentPageID("VideoBoard")))
            //        return RedirectToAction("NotAllow", "Custom");
            //}

            userInformation = (UserInfo)Session["User"];
            if (userInformation.RoleId == (int)Role.VegetarianGuide)
            {
                //إرشاد نباتي
                videoSearch.VideoCategoryId = int.Parse(CategoryId[0]);
            }
            else if (userInformation.RoleId == (int)Role.AnimalGuide)
            {
                //إرشاد سمكي
                videoSearch.VideoCategoryId = int.Parse(CategoryId[1]);
            }
            else if (userInformation.RoleId == (int)Role.FishGuide)
            {
                //إرشاد حيواني
                videoSearch.VideoCategoryId = int.Parse(CategoryId[2]);

            }
            else if (userInformation.RoleId == (int)Role.Manager || userInformation.RoleId == (int)Role.Employee)
            {
                videoSearch.VideoCategoryId = 0;

            }
            //else
            //{
            //    return RedirectToAction("NotAllow", "Custom");
            //}

            // ViewBag.CategoryId = new SelectList(db.VideoCategories, "Id", "Name");
            if (videoSearch.VideoCategoryId == 0)
            {
                ViewBag.DepartmentId = new SelectList(db.VideoCategories.Where(x => x.ParentId != null), "id", "name");
            }
            else
            {
                ViewBag.DepartmentId = new SelectList(db.VideoCategories.Where(x => x.ParentId == videoSearch.VideoCategoryId), "id", "name");

            }
            ViewBag.UserId = new SelectList(db.users.Where(x => x.role_id == 6), "id", "name");

            return JsonConvert.SerializeObject(_videoBoardServices.GetGallery(videoSearch));
        }
        [HttpPost]
        public ActionResult ActiveVideo(string videoId, int videoStatusId)
        {
            var video = db.Videos.Find(Guid.Parse(videoId));
            video.VideoStatusId = videoStatusId;
            video.ApprovedAt = DateTime.Now;
            video.ApprovedBy = this.CurrentUser.Id;
            db.Entry(video).State = EntityState.Modified; ;
            db.SaveChanges();
            if (videoStatusId == 1)
            {
                Generic generic = new Generic();
                var user = db.users.Find(video.UserId);
                generic.SendTopicNotification("موافقه", "تمت الموافقة على المقطع الخاص بك بعنوان " + " " + video.Title, user.phone, "video approved", user.phone);
                return Json("Success");
            }
            return Json("Success");
        }
        public ActionResult GetVideoComment(string videoId)
        {
            var Conmments = _videoCommentServices.GetVideoComment(Guid.Parse(videoId));
            return Json(Conmments);
        }

    }
}
