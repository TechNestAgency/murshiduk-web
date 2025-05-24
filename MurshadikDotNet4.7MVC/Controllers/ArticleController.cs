using PagedList;
using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Newtonsoft.Json;
using MurshadikCP.Repositories;

namespace MurshadikCP.Controllers
{
    [Authorize]
    public class ArticleController : BaseController
    {
        // GET: Article
        // list of the articles show in Index View
        // also putting pagination, search criteria, and category id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchNews(string q)
        {
            if (q.Length <= 3)
            {
                return Json(null);
            }

            var articles = db.articles
                            .Where(
                                art => art.title.Contains(q) || art.short_description.Contains(q)
                                )
                            .Select(art => new { id = art.id, title_ar = art.title + " - " + art.category.name, category_id = art.category_id })
                            .ToList();
            return Json(articles);
        }
        public ActionResult Index(int? page, string Search_Data, int Filter_Value = 0, string start_date = null, string end_date = null)
        {
            if (currentPageID("Article") > 0)
            {
                if (!CurrentUser.canView(currentPageID("Article")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            start_date = start_date == string.Empty ? null : start_date;
            end_date = end_date == string.Empty ? null : end_date;

            DateTime sdt = DateTime.Now.AddYears(-2); DateTime edt = DateTime.Now;
            TimeSpan daysToSub = new System.TimeSpan(6, 0, 0, 0);
            var startDate = DateTime.Today.Subtract(daysToSub).StartOfDay();
            var endDate = DateTime.Today.EndOfDay();

            if (!string.IsNullOrEmpty(start_date))
            {
                ViewBag.Start_Date = start_date;
                sdt = DateTime.Parse(start_date, null, System.Globalization.DateTimeStyles.RoundtripKind);
                startDate = sdt;
            }
            if (!string.IsNullOrEmpty(end_date))
            {
                ViewBag.End_Date = end_date;
                edt = DateTime.Parse(end_date, null, System.Globalization.DateTimeStyles.RoundtripKind);
                endDate = edt;
            }

            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;

            int pageNumber = (page ?? 1);

            var ids = db.fun_GetCategoryTreeIDs(0).ToList();
            
            var articles = getArticlesPagedForCategory(null, pageNumber, Filter_Value, start_date, end_date);

            ViewBag.ptitle = "جميع الأخبار";
            ViewData["returnAction"] = "";

            return View("~/Views/Article/Index.cshtml", articles);
        }

        public ActionResult AgricultureNotebook(int? page, string searchString, int Filter_Value = 0, string start_date = null, string end_date = null)
        {
            if (currentPageID("AgricultureNotebook") > 0)
            {
                if (!CurrentUser.canView(currentPageID("AgricultureNotebook")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            start_date = start_date == string.Empty ? null : start_date;
            end_date = end_date == string.Empty ? null : end_date;

            DateTime sdt = DateTime.Now.AddYears(-2); DateTime edt = DateTime.Now;
            TimeSpan daysToSub = new System.TimeSpan(6, 0, 0, 0);
            var startDate = DateTime.Today.Subtract(daysToSub).StartOfDay();
            var endDate = DateTime.Today.EndOfDay();

            if (!string.IsNullOrEmpty(start_date))
            {
                ViewBag.Start_Date = start_date;
                sdt = DateTime.Parse(start_date, null, System.Globalization.DateTimeStyles.RoundtripKind);
                startDate = sdt;
            }
            if (!string.IsNullOrEmpty(end_date))
            {
                ViewBag.End_Date = end_date;
                edt = DateTime.Parse(end_date, null, System.Globalization.DateTimeStyles.RoundtripKind);
                endDate = edt;
            }

            int pageNumber = (page ?? 1);

            var ids = db.fun_GetCategoryTreeIDs(1).ToList();
            var articles = getArticlesPagedForCategory(ids.ToArray(), pageNumber, Filter_Value, start_date, end_date);

            ViewBag.ptitle = "المفكرة الزراعية";
            ViewData["returnAction"] = "AgricultureNotebook";

            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;

            return View("~/Views/Article/Index.cshtml", articles);
        }

        public ActionResult AgricultureCalender(int? page, string searchString, int Filter_Value = 0, string start_date = null, string end_date = null)
        {
            if (currentPageID("AgricultureCalender") > 0)
            {
                if (!CurrentUser.canView(currentPageID("AgricultureCalender")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            start_date = start_date == string.Empty ? null : start_date;
            end_date = end_date == string.Empty ? null : end_date;

            DateTime sdt = DateTime.Now.AddYears(-2); DateTime edt = DateTime.Now;
            TimeSpan daysToSub = new System.TimeSpan(6, 0, 0, 0);
            var startDate = DateTime.Today.Subtract(daysToSub).StartOfDay();
            var endDate = DateTime.Today.EndOfDay();

            if (!string.IsNullOrEmpty(start_date))
            {
                ViewBag.Start_Date = start_date;
                sdt = DateTime.Parse(start_date, null, System.Globalization.DateTimeStyles.RoundtripKind);
                startDate = sdt;
            }
            if (!string.IsNullOrEmpty(end_date))
            {
                ViewBag.End_Date = end_date;
                edt = DateTime.Parse(end_date, null, System.Globalization.DateTimeStyles.RoundtripKind);
                endDate = edt;
            }

            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;

            int pageNumber = (page ?? 1);

            var ids = db.fun_GetCategoryTreeIDs(13).ToList();
            var articles = getArticlesPagedForCategory(ids.ToArray(), pageNumber, Filter_Value, start_date, end_date);

            ViewBag.ptitle = "تقويم الزراعي";
            ViewData["returnAction"] = "AgricultureCalender";

            return View("~/Views/Article/Index.cshtml", articles);
        }

        public ActionResult AgriculturalGuidance(int? page, string searchString, int Filter_Value = 0, string start_date = null, string end_date = null)
        {
            if (currentPageID("AgriculturalGuidance") > 0)
            {
                if (!CurrentUser.canView(currentPageID("AgriculturalGuidance")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            start_date = start_date == string.Empty ? null : start_date;
            end_date = end_date == string.Empty ? null : end_date;

            DateTime sdt = DateTime.Now.AddYears(-2); DateTime edt = DateTime.Now;
            TimeSpan daysToSub = new System.TimeSpan(6, 0, 0, 0);
            var startDate = DateTime.Today.Subtract(daysToSub).StartOfDay();
            var endDate = DateTime.Today.EndOfDay();

            if (!string.IsNullOrEmpty(start_date))
            {
                ViewBag.Start_Date = start_date;
                sdt = DateTime.Parse(start_date, null, System.Globalization.DateTimeStyles.RoundtripKind);
                startDate = sdt;
            }
            if (!string.IsNullOrEmpty(end_date))
            {
                ViewBag.End_Date = end_date;
                edt = DateTime.Parse(end_date, null, System.Globalization.DateTimeStyles.RoundtripKind);
                endDate = edt;
            }

            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;

            int pageNumber = (page ?? 1);

            var ids = db.fun_GetCategoryTreeIDs(12).ToList();
            var articles = getArticlesPagedForCategory(ids.ToArray(), pageNumber, Filter_Value, start_date, end_date);

            ViewBag.ptitle = "الإرشاد الزراعي";
            ViewData["returnAction"] = "AgriculturalGuidance";

            return View("~/Views/Article/Index.cshtml", articles);
        }

        public ActionResult AgriculturalProducts(int? page, string searchString, int Filter_Value = 0, string start_date = null, string end_date = null)
        {
            if (currentPageID("AgriculturalProducts") > 0)
            {
                if (!CurrentUser.canView(currentPageID("AgriculturalProducts")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            start_date = start_date == string.Empty ? null : start_date;
            end_date = end_date == string.Empty ? null : end_date;

            DateTime sdt = DateTime.Now.AddYears(-2); DateTime edt = DateTime.Now;
            TimeSpan daysToSub = new System.TimeSpan(6, 0, 0, 0);
            var startDate = DateTime.Today.Subtract(daysToSub).StartOfDay();
            var endDate = DateTime.Today.EndOfDay();

            if (!string.IsNullOrEmpty(start_date))
            {
                ViewBag.Start_Date = start_date;
                sdt = DateTime.Parse(start_date, null, System.Globalization.DateTimeStyles.RoundtripKind);
                startDate = sdt;
            }
            if (!string.IsNullOrEmpty(end_date))
            {
                ViewBag.End_Date = end_date;
                edt = DateTime.Parse(end_date, null, System.Globalization.DateTimeStyles.RoundtripKind);
                endDate = edt;
            }

            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;

            int pageNumber = (page ?? 1);

            var ids = db.fun_GetCategoryTreeIDs(33).ToList();
            var articles = getArticlesPagedForCategory(ids.ToArray(), pageNumber, Filter_Value, start_date, end_date);

            ViewBag.ptitle = "المنتجات الزراعية";
            ViewData["returnAction"] = "AgriculturalProducts";

            return View("~/Views/Article/Index.cshtml", articles);
        }

        public ActionResult DigitalLibrary(int? page, string searchString, int Filter_Value = 0, string start_date = null, string end_date = null)
        {
            if (currentPageID("DigitalLibrary") > 0)
            {
                if (!CurrentUser.canView(currentPageID("DigitalLibrary")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            start_date = start_date == string.Empty ? null : start_date;
            end_date = end_date == string.Empty ? null : end_date;

            DateTime sdt = DateTime.Now.AddYears(-2); DateTime edt = DateTime.Now;
            TimeSpan daysToSub = new System.TimeSpan(6, 0, 0, 0);
            var startDate = DateTime.Today.Subtract(daysToSub).StartOfDay();
            var endDate = DateTime.Today.EndOfDay();

            if (!string.IsNullOrEmpty(start_date))
            {
                ViewBag.Start_Date = start_date;
                sdt = DateTime.Parse(start_date, null, System.Globalization.DateTimeStyles.RoundtripKind);
                startDate = sdt;
            }
            if (!string.IsNullOrEmpty(end_date))
            {
                ViewBag.End_Date = end_date;
                edt = DateTime.Parse(end_date, null, System.Globalization.DateTimeStyles.RoundtripKind);
                endDate = edt;
            }

            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;

            int pageNumber = (page ?? 1);

            var ids = db.fun_GetCategoryTreeIDs(15).ToList();
            var articles = getArticlesPagedForCategory(ids.ToArray(), pageNumber, Filter_Value, start_date, end_date);

            ViewBag.ptitle = "المكتبة الرقمية";
            ViewData["returnAction"] = "DigitalLibrary";

            return View("~/Views/Article/Index.cshtml", articles);
        }

        public ActionResult InformativeQA(int? page, string searchString, int Filter_Value = 0, string start_date = null, string end_date = null)
        {
            if (currentPageID("InformativeQA") > 0)
            {
                if (!CurrentUser.canView(currentPageID("InformativeQA")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            start_date = start_date == string.Empty ? null : start_date;
            end_date = end_date == string.Empty ? null : end_date;

            DateTime sdt = DateTime.Now.AddYears(-2); DateTime edt = DateTime.Now;
            TimeSpan daysToSub = new System.TimeSpan(6, 0, 0, 0);
            var startDate = DateTime.Today.Subtract(daysToSub).StartOfDay();
            var endDate = DateTime.Today.EndOfDay();

            if (!string.IsNullOrEmpty(start_date))
            {
                ViewBag.Start_Date = start_date;
                sdt = DateTime.Parse(start_date, null, System.Globalization.DateTimeStyles.RoundtripKind);
                startDate = sdt;
            }
            if (!string.IsNullOrEmpty(end_date))
            {
                ViewBag.End_Date = end_date;
                edt = DateTime.Parse(end_date, null, System.Globalization.DateTimeStyles.RoundtripKind);
                endDate = edt;
            }

            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;

            int pageNumber = (page ?? 1);

            var ids = db.fun_GetCategoryTreeIDs(30).ToList();
            var articles = getArticlesPagedForCategory(ids.ToArray(), pageNumber, Filter_Value, start_date, end_date);

            ViewBag.ptitle = "أسئلة وأجوبة إرشادية";
            ViewData["returnAction"] = "InformativeQA";

            return View("~/Views/Article/Index.cshtml", articles);
        }

        public ActionResult SuccessfulExperiences(int? page, string searchString, int Filter_Value = 0, string start_date = null, string end_date = null)
        {
            if (currentPageID("SuccessfulExperiences") > 0)
            {
                if (!CurrentUser.canView(currentPageID("SuccessfulExperiences")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            start_date = start_date == string.Empty ? null : start_date;
            end_date = end_date == string.Empty ? null : end_date;

            DateTime sdt = DateTime.Now.AddYears(-2); DateTime edt = DateTime.Now;
            TimeSpan daysToSub = new System.TimeSpan(6, 0, 0, 0);
            var startDate = DateTime.Today.Subtract(daysToSub).StartOfDay();
            var endDate = DateTime.Today.EndOfDay();

            if (!string.IsNullOrEmpty(start_date))
            {
                ViewBag.Start_Date = start_date;
                sdt = DateTime.Parse(start_date, null, System.Globalization.DateTimeStyles.RoundtripKind);
                startDate = sdt;
            }
            if (!string.IsNullOrEmpty(end_date))
            {
                ViewBag.End_Date = end_date;
                edt = DateTime.Parse(end_date, null, System.Globalization.DateTimeStyles.RoundtripKind);
                endDate = edt;
            }

            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;

            int pageNumber = (page ?? 1);

            var ids = db.fun_GetCategoryTreeIDs(14).ToList();
            var articles = getArticlesPagedForCategory(ids.ToArray(), pageNumber, Filter_Value, start_date, end_date);

            ViewBag.ptitle = Resources.Resources.News;//"التجارب الناجحة";
            ViewData["returnAction"] = "SuccessfulExperiences";

            return View("~/Views/Article/Index.cshtml", articles);
        }

        [HttpPost]
        public ActionResult ActiveInactive(long id, bool active)
        {
            article a = db.articles.Find(id);
            a.active = !active ? true : false;
            a.updated_at = DateTime.Now;
            db.Entry(a).State = EntityState.Modified;
            db.SaveChanges();
            return Json("Success");
        }

        // Add view for articles
        public ActionResult Add(int catid = 0)
        {
            if (currentPageID("Article") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("Article")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            article a = new article();
            //CategoryController ca = new CategoryController();
            CategoryRepository cr = new CategoryRepository();
            List<category> categories = cr.GetCategories();
            if (catid > 0)
            {
                List<category> categories1 = cr.GetCategoryByCatID(catid);
                ViewBag.category_id = new SelectList(categories1, "id", "name", catid);
            }
            else
            {
                ViewBag.category_id = new SelectList(categories, "id", "name");
            }
            ViewBag.keywords = new SelectList(db.keywords.ToList(), "keyword1", "keyword1");
            return View(a);
        }

        public ActionResult View(int id)
        {
            if (currentPageID("Article") > 0)
            {
                if (!CurrentUser.canView(currentPageID("Article")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            article a = db.articles.Where(x => x.id == id).FirstOrDefault();
            ViewBag.category_id = new SelectList(db.categories.ToList(), "id", "name", a.category_id);
            ViewBag.articlecategoryimg = db.articles_galleries.Where(x => x.article_id == a.id).FirstOrDefault().image;
            ViewBag.keywords = new SelectList(db.keywords.ToList(), "keyword1", "keyword1");
            a.keywords = a.keywords.Replace(",", "','");
            return View(a);
        }

        // edit view for articles
        public ActionResult Edit(int id)
        {
            if (currentPageID("Article") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("Article")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            //CategoryController ca = new CategoryController();
            CategoryRepository cr = new CategoryRepository();
            List<category> categories = cr.GetCategories();
            article a = db.articles.Where(x => x.id == id).FirstOrDefault();
            if (Request.UrlReferrer == null ? false : Request.UrlReferrer.Query.Contains("catid="))
            {
                try
                {
                    int catid = Convert.ToInt32(Request.UrlReferrer.Query.Split('=').Last());
                    List<category> categories1 = cr.GetCategoryByCatID(catid);
                    ViewBag.category_id = new SelectList(categories1, "id", "name", a.category_id);
                }
                catch
                {
                    ViewBag.category_id = new SelectList(categories, "id", "name", a.category_id);
                }
            }
            else
            {
                ViewBag.category_id = new SelectList(categories, "id", "name", a.category_id);
            }

            articles_galleries ag = db.articles_galleries.Where(x => x.article_id == a.id).FirstOrDefault();
            if (ag != null)
            {
                ViewBag.articlecategoryimg = ag.image;
            }
            else
            {
                ViewBag.articlecategoryimg = "";
            }

            ViewBag.keywords = new SelectList(db.keywords.ToList(), "keyword1", "keyword1");
            a.keywords = a.keywords != null ? a.keywords.Replace(",", "','") : "";
            return View(a);
        }

        [HttpPost]
        public JsonResult DeleteChildImage(long id, string image)
        {
            try
            {
                articles_galleries a = db.articles_galleries.Find(id);
                if (a != null)
                {
                    db.articles_galleries.Remove(a);
                }
                db.SaveChanges();

                string fullpath = Request.MapPath(image);
                if (System.IO.File.Exists(fullpath))
                {
                    System.IO.File.Delete(fullpath);
                }
                return Json("successful");
            }
            catch (Exception ex)
            {
                return Json("error");
            }
        }

        [HttpPost]
        public JsonResult DeleteImage(long id, string image)
        {
            try
            {
                article a = db.articles.Find(id);
                a.image = null;
                a.updated_at = DateTime.Now;
                db.Entry(a).State = EntityState.Modified;
                db.SaveChanges();

                string fullpath = Request.MapPath(image);
                if (System.IO.File.Exists(fullpath))
                {
                    System.IO.File.Delete(fullpath);
                }
                return Json("successful");
            }
            catch (Exception ex)
            {
                return Json("error");
            }
        }

        [HttpPost]
        public JsonResult upload(HttpPostedFileBase file)
        {
            Guid imgGuid = Guid.NewGuid();
            if (file != null && file.ContentLength > 0)
            {
                var img = imgGuid.ToString() + Path.GetExtension(file.FileName);
                var path = Path.Combine(Server.MapPath(MediaPath.Articles), img);

                string hostURL = WebConfigurationManager.AppSettings["HostUrl"];

                file.SaveAs(path);

                return Json(new { location = hostURL + "Media/Images/Articles/" + img });
            }

            return Json("error");
        }


        // modidy the articles
        // keywords also more than 1 can be added
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(article article, HttpPostedFileBase image, HttpPostedFileBase[] childimage, string[] keywords, string articleImage)
        {
            if (currentPageID("Article") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("Article")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (ModelState.IsValid)
            {
                Guid imgGuid = Guid.NewGuid();

                article.image = articleImage;

                if (image != null && image.ContentLength > 0)
                {
                    var img = imgGuid.ToString() + Path.GetExtension(image.FileName);
                    var path = Path.Combine(Server.MapPath(MediaPath.Articles), img);
                    image.SaveAs(path);
                    article.image = MediaPath.Articles + img;
                }


                foreach (HttpPostedFileBase file in childimage)
                {
                    if (file == null) continue;

                    var img = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var path = Path.Combine(Server.MapPath(MediaPath.Articles), img);
                    file.SaveAs(path);

                    articles_galleries ag = new articles_galleries { article_id = article.id, image = MediaPath.Articles + img };
                    db.Entry(ag).State = EntityState.Added;
                    article.articles_galleries.Add(ag);
                }

                article.updated_at = DateTime.Now;
                article.keywords = string.Join(",", keywords);
                article.user_id = CurrentUser.Id;
                db.Entry(article).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Redirect(Request.UrlReferrer.ToString());
            //return RedirectToAction("Index");
        }
        // this method we creating for importing the old data from old application ( for news, articles)
        public void getArticleCompleteData()
        {
            try
            {
                HttpClient client = new HttpClient();
                var result = client.GetAsync("https://murshadik.saatco.net/api/getAllArticles");
                result.Wait();

                var result1 = result.Result;
                var test = result1.Content.ReadAsStringAsync();
                test.Wait();
                List<articleAPIdata> articles = Newtonsoft.Json.JsonConvert.DeserializeObject<List<articleAPIdata>>(test.Result);

                foreach (var item in articles)
                {
                    article article = db.articles.Find(item.id);
                    if (article != null)
                    {
                        article.category_id = item.categories[0].id;
                        article.title = item.title_ar;
                        article.description = item.desc_ar;
                        article.short_description = item.desc;
                        article.updated_at = DateTime.Now;
                        db.Entry(article).State = EntityState.Modified;
                        db.SaveChanges();

                        if (item.gallery_images.Count() > 0)
                        {
                            for (int i = 0; i < item.gallery_images.Count(); i++)
                            {
                                long article_id = Convert.ToInt32(item.gallery_images[i].news_id);
                                articles_galleries gallery = new articles_galleries();

                                gallery.article_id = item.gallery_images[i].news_id;
                                gallery.image = item.gallery_images[i].image;
                                gallery.created_at = DateTime.Now;
                                db.articles_galleries.Add(gallery);
                                db.SaveChanges();

                            }

                        }
                    }
                    else
                    {
                        article = new article();
                        article.category_id = item.categories[0].id;
                        article.title = item.title_ar;
                        article.description = item.desc_ar;
                        article.short_description = item.desc;
                        article.updated_at = DateTime.Now;
                        db.articles.Add(article);
                        db.SaveChanges();

                        if (item.gallery_images.Count() > 0)
                        {
                            for (int i = 0; i < item.gallery_images.Count(); i++)
                            {
                                articles_galleries gallery = new articles_galleries();

                                gallery.article_id = item.gallery_images[i].news_id;
                                gallery.image = item.gallery_images[i].image;
                                gallery.created_at = DateTime.Now;
                                db.articles_galleries.Add(gallery);
                                db.SaveChanges();

                            }

                        }
                    }

                }
            }
            catch (Exception ex)
            {

            }
        }

        // this method we creating for importing the old data from old application ( for categories)
        public void getCategories()
        {
            try
            {
                HttpClient client = new HttpClient();
                var result = client.GetAsync("https://murshadik.saatco.net/api/getAllCats");
                result.Wait();

                var result1 = result.Result;
                var test = result1.Content.ReadAsStringAsync();
                test.Wait();
                List<categories> categories = Newtonsoft.Json.JsonConvert.DeserializeObject<List<categories>>(test.Result);

                foreach (var item in categories)
                {
                    category c = new category();
                    c.id = item.id;
                    c.icon = item.icon;
                    c.banner = item.banner_ar;
                    c.created_at = DateTime.Now;
                    c.parent_category = item.parent_category;
                    c.active = true;
                    c.description = item.desc_ar;
                    c.bg_color = item.bg_color;
                    c.text_color = item.text_color;
                    c.name = item.name_ar;
                    db.categories.Add(c);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }
        }

        // this method we creating for importing the old data from old application ( for categories images)
        public void getCategoryImage()
        {
            try
            {
                List<category> categories = db.categories.ToList();

                foreach (var item in categories)
                {
                    if (item.banner != null)
                    {
                        string image = "https://murshadik.saatco.net/storage/" + item.banner;
                        Guid imgGuid = Guid.NewGuid();

                        using (WebClient client1 = new WebClient())
                        {
                            var img = imgGuid.ToString() + Path.GetExtension(item.banner);
                            var path = Path.Combine(Server.MapPath("~/Media/Images/Categories/"), img);
                            client1.DownloadFile(new Uri(image), path);

                            category cart = db.categories.Find(item.id);
                            if (cart != null)
                            {
                                cart.banner = "/Media/Images/Categories/" + img;
                                db.Entry(cart).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }

                    if (item.icon != null)
                    {
                        string image = "https://murshadik.saatco.net/storage/" + item.icon;
                        Guid imgGuid = Guid.NewGuid();

                        using (WebClient client1 = new WebClient())
                        {
                            var img = imgGuid.ToString() + Path.GetExtension(item.icon);
                            var path = Path.Combine(Server.MapPath("~/Media/Images/Categories/"), img);
                            client1.DownloadFile(new Uri(image), path);

                            category cart = db.categories.Find(item.id);
                            if (cart != null)
                            {
                                cart.icon = "/Media/Images/Categories/" + img;
                                db.Entry(cart).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {

            }
        }

        // this method we creating for importing the old data from old application ( for news, article images)
        public void getArticleImage()
        {
            try
            {
                List<article> articles = db.articles.ToList();

                foreach (var item in articles)
                {
                    if (item.image != null)
                    {
                        string image = "https://murshadik.saatco.net/storage/" + item.image;
                        Guid imgGuid = Guid.NewGuid();

                        using (WebClient client1 = new WebClient())
                        {
                            var img = imgGuid.ToString() + Path.GetExtension(item.image);
                            var path = Path.Combine(Server.MapPath(MediaPath.Articles), img);
                            client1.DownloadFile(new Uri(image), path);

                            article art = db.articles.Find(item.id);
                            if (art != null)
                            {
                                art.image = MediaPath.Articles + img;
                                db.Entry(art).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }

                    if (item.articles_galleries.Count() > 0)
                    {
                        foreach (var item1 in item.articles_galleries)
                        {
                            string image1 = "https://murshadik.saatco.net/storage/" + item1.image;
                            Guid imgGuid = Guid.NewGuid();
                            using (WebClient client1 = new WebClient())
                            {
                                var img1 = imgGuid.ToString() + Path.GetExtension(item1.image);
                                var path1 = Path.Combine(Server.MapPath(MediaPath.Articles), img1);
                                client1.DownloadFile(new Uri(image1), path1);

                                articles_galleries art = db.articles_galleries.Find(item1.id);
                                if (art != null)
                                {
                                    art.image = MediaPath.Articles + img1;
                                    db.Entry(art).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        // add article from here and put images, keywords etc.
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public ActionResult Add(article article, HttpPostedFileBase image, HttpPostedFileBase childimage, string[] keywords)
        {
            if (currentPageID("Article") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("Article")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (ModelState.IsValid)
            {
                Guid imgGuid = Guid.NewGuid();
                Guid childGuid = Guid.NewGuid();
                if (image != null && image.ContentLength > 0)
                {
                    var img = imgGuid.ToString() + Path.GetExtension(image.FileName);

                    var path = Path.Combine(Server.MapPath(MediaPath.Articles), img);
                    image.SaveAs(path);
                }
                if (childimage != null && childimage.ContentLength > 0)
                {
                    var img = childGuid.ToString() + Path.GetExtension(childimage.FileName);
                    var path = Path.Combine(Server.MapPath(MediaPath.Articles), img);
                    childimage.SaveAs(path);
                }

                article.image = image != null ? "/Media/Images/Articles/" + imgGuid.ToString() + Path.GetExtension(image.FileName) : "";
                article.keywords = string.Join(", ", keywords);
                article.user_id = db.users.Where(x => x.phone == User.Identity.Name).FirstOrDefault().id;
                article.created_at = DateTime.Now;
                db.articles.Add(article); 
                db.SaveChanges();

                if (childimage != null && childimage.ContentLength > 0)
                { 
                    articles_galleries _Galleries = new articles_galleries();
                    _Galleries.article_id = article.id;
                    _Galleries.image = childimage != null ? "/Media/Images/Articles/" + childGuid.ToString() + Path.GetExtension(childimage.FileName) : "";
                    _Galleries.created_at = DateTime.Now;
                    db.articles_galleries.Add(_Galleries);
                    db.SaveChanges();
                }
                medium medium = new medium();
                medium.title = article.title;
                medium.keywords = string.Join(",", keywords);
                medium.is_active = true;
                medium.is_internal_file = false;
                medium.file_name = "/Article/View?id=" + article.id;
                medium.file_type = null;
                medium.created_at = DateTime.Now;
                medium.created_by = CurrentUser.Id;
                db.media.Add(medium);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }
        private IPagedList<article> getArticlesPagedForCategory(long?[] catIDs, int pageNumber, int filter = 0, string start_date = null, string end_date = null)
        {

            IQueryable<article> articles = from art in db.articles select art;
            if (catIDs != null && catIDs.Count() > 0)
            {
                articles = articles.Where(art => catIDs.Contains(art.category_id));
            }
            else
            {
                //articles = articles.Where(art => catIDs.Contains(art.category_id));
            }
            //articles = articles.Where(art => catIDs.Contains(art.category_id));
            if (filter > 0)
            {
                var filterValue = filter == 1 ? true : false;
                articles = articles.Where(art => art.active == filterValue);
            }

            ViewBag.Filter_Value = filter;
            if (!string.IsNullOrEmpty(start_date))
            {
                ViewBag.Start_Date = start_date;
                var sdt = DateTime.Parse(start_date, null, System.Globalization.DateTimeStyles.RoundtripKind);
                articles = articles.Where(art => art.created_at >= sdt);
            }
            if (!string.IsNullOrEmpty(end_date))
            {
                ViewBag.End_Date = end_date;
                var edt = DateTime.Parse(end_date, null, System.Globalization.DateTimeStyles.RoundtripKind);
                articles = articles.Where(art => art.created_at <= edt);
            }


            articles = articles.OrderByDescending(art => art.created_at);


            int pageSize = MurshadikCP.Constants.defaultPageCount;
            return articles.ToPagedList(pageNumber, pageSize);

        }

    }
    public class MasterData
    {
        public List<articleAPIdata> articleAPIdata { get; set; }
        public List<categories> categories { get; set; }
    }

    public class articleAPIdata
    {
        public int id { get; set; }
        //public int category_id { get; set; }
        public string title_ar { get; set; }
        public string desc { get; set; }
        public string desc_ar { get; set; }
        public string image { get; set; }
        public DateTime created_at { get; set; }
        public List<categories> categories { get; set; }
        public List<gallery_images> gallery_images { get; set; }
        //public categories categories1 { get; set; }
    }

    public class categories
    {
        public int id { get; set; }
        public string name_ar { get; set; }
        public string desc_ar { get; set; }
        public int? parent_category { get; set; }
        public string icon { get; set; }
        public string bg_color { get; set; }
        public string text_color { get; set; }
        public string banner_ar { get; set; }
    }

    public class gallery_images
    {
        public int id { get; set; }
        public int news_id { get; set; }
        public string image { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }

}