
using PagedList;
using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MurshadikCP.Controllers
{
    [Authorize]
    public class MediasController : BaseController
    {
        // GET: Media
        // Index view shows all media list
        // pagination and searching criteria also there
        public ActionResult Index(int? Page_No, string Search_Data, string Filter_Value, string start_date = null, string end_date = null)
        {
            if (currentPageID("Media") > 0)
            {
                if (!CurrentUser.canView(currentPageID("Media")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            int pagesize = 10;
            int No_Of_Page = (Page_No ?? 1);

            if (Search_Data != null)
            {
                Page_No = 1;
            }
            else
            {
                Search_Data = Filter_Value;
            }

            ViewBag.Filter_Value = Search_Data;

            IQueryable<medium> media = from m in db.media select m;

            if (!string.IsNullOrEmpty(start_date))
            {
                ViewBag.Start_Date = start_date;
                var sdt = DateTime.Parse(start_date, null, System.Globalization.DateTimeStyles.RoundtripKind);
                media = media.Where(art => art.created_at >= sdt);
            }
            if (!string.IsNullOrEmpty(end_date))
            {
                ViewBag.End_Date = end_date;
                var edt = DateTime.Parse(end_date, null, System.Globalization.DateTimeStyles.RoundtripKind);
                media = media.Where(art => art.created_at <= edt);
            }

            if (!String.IsNullOrEmpty(Search_Data))
            {
                media = media.Where(x => x.title.Contains(Search_Data) || x.keywords.Contains(Search_Data));
            }

            return View(media.OrderByDescending(x => x.id).ToPagedList(No_Of_Page, pagesize));
        }

        // create form for media
        public ActionResult Create()
        {
            if (currentPageID("Media") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("Media")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            medium m = new medium();
            // these are drop down list
            ViewBag.regions = new SelectList(db.regions.ToList(), "name_ar", "name_ar");
            ViewBag.skills = new SelectList(db.skills.ToList(), "name", "name");
            ViewBag.notification_class_id = new SelectList(db.notification_class.ToList(), "id", "notification_class1");
            ViewBag.keywords = new SelectList(db.keywords.ToList(), "keyword1", "keyword1");
            ViewBag.keyword_template = new SelectList(db.keyword_group.ToList(), "group_keywords", "keyword_group_name");
            return View(m);
        }

        // adding the data into the table
        // data comes from the Form
        // more than 1 images are allowed, keywords, regions, skills, multiseason
        // if link there then put link also
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(medium media, HttpPostedFileBase[] image, string[] keywords, string[] regions, string[] skills,string[] multiseason, string link)
        {
            if (currentPageID("Media") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("Media")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (ModelState.IsValid)
            {
                if (media.file_type == "link")
                {
                    media.file_name = link;
                    media.is_internal_file = false;
                }
                else
                {
                    Guid imgGuid = Guid.NewGuid();
                    foreach (HttpPostedFileBase file in image)
                    {
                        if (file != null && file.ContentLength > 0)
                        {
                            var extention = Path.GetExtension(file.FileName);
                            var img = imgGuid.ToString() + Path.GetExtension(file.FileName);
                            if (extention == ".xlxs" || extention == ".xls")
                            {
                                var path = Path.Combine(Server.MapPath("~/Media/Excel/"), img);
                                file.SaveAs(path);
                            }
                            else if (extention == ".pdf")
                            {
                                var path = Path.Combine(Server.MapPath("~/Media/PDF/"), img);
                                file.SaveAs(path);
                            }
                            else if (extention == ".jpg" || extention == ".png" || extention == ".jpeg" || extention == ".gif")
                            {
                                var path = Path.Combine(Server.MapPath("~/Media/Images/"), img);
                                file.SaveAs(path);
                            }
                            media.is_internal_file = true;
                            media.file_name = image != null ? imgGuid.ToString() + Path.GetExtension(file.FileName) : "";
                        }
                        media.regions = string.Join(",", regions);
                        media.skills = string.Join(",", skills);
                        media.is_active = true;
                        media.keywords = string.Join(",", keywords);
                        media.seasons = multiseason != null && multiseason[0].ToString() != "كل" ? string.Join(",", multiseason) : "فصل الخريف,فصل الشتاء,فصل الربيع,فصل الصيف";
                        media.created_at = DateTime.Now;
                        media.created_by = CurrentUser.Id;
                        db.media.Add(media);
                        db.SaveChanges();
                    }
                }

                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        // POST: Medias/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            if (currentPageID("Media") > 0)
            {
                if (!CurrentUser.canDelete(currentPageID("Media")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            medium m = db.media.Find(id);
            db.media.Remove(m);
            db.SaveChanges();
            return Json("Success");
        }
    }
}