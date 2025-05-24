using PagedList;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MurshadikCP.Models;
using Microsoft.Ajax.Utilities;
using System.Data.Entity;
using MurshadikCP.Repositories;

namespace MurshadikCP.Controllers
{
    [Authorize]
    public class CategoryController : BaseController
    {
        // GET: Category
        // Index view for showing the categories
        // search criteria and pagination there
        // catid also if user pass values then the list shows according that parameters
        CategoryRepository cr = new CategoryRepository();
        public ActionResult Index(int? Page_No, string Search_Data, string Filter_Value, long catId = 0)
        {
            if (currentPageID("Category") > 0)
            {
                if (!CurrentUser.canView(currentPageID("Category")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            int pagesize = 60;

            List<category> categories = cr.GetCategories();

            ViewBag.Filter_Value = Search_Data;

            ViewBag.parent_category = new SelectList(categories, "id", "name");

            ParentCategory pc = new ParentCategory();
            pc.categoryList = cr.GetCategoryList(Page_No, Search_Data, Filter_Value, catId).ToPagedList(Page_No ?? 1, pagesize);
            if (catId > 0)
            {
                pc.Category = cr.GetCategoryByID(catId); //db.categories.Where(x => x.id == catId).FirstOrDefault();
                ViewBag.parent_category = new SelectList(categories, "id", "name", pc.Category.parent_category);
            }
            else
            {
                pc.Category = new category();
            }

            return View(pc);
        }

        //// show all categories for drop down list
        //// shows also first parent then child
        //public List<category> GetCategoryByCatID(long catid)
        //{
        //    List<category> finalcat = new List<category>();
        //    category _cat = db.categories.Where(x => x.id == catid).FirstOrDefault();
        //    category cat_1 = new category();
        //    cat_1.id = _cat.id;
        //    cat_1.name = _cat.name;
        //    cat_1.active = _cat.active;
        //    finalcat.Add(cat_1);
                
        //    List<category> categories = db.categories.Where(x => x.parent_category == catid).ToList();
        //    foreach (var item in categories)
        //    {
        //        category cat = new category();
        //        cat.id = item.id;
        //        cat.name = " - " + item.name;
        //        cat.active = item.active;
        //        finalcat.Add(cat);
        //        List<category> categories1 = db.categories.Where(x => x.parent_category == item.id).ToList();
        //        foreach (var item1 in categories1)
        //        {
        //            category cat1 = new category();
        //            cat1.id = item1.id;
        //            cat1.name = " - - " + item1.name;
        //            cat1.active = item1.active;
        //            finalcat.Add(cat1);
        //            List<category> categories2 = db.categories.Where(x => x.parent_category == item1.id).ToList();
        //            foreach (var item2 in categories2)
        //            {
        //                category cat2 = new category();
        //                cat2.id = item2.id;
        //                cat2.name = " - - - " + item2.name;
        //                cat2.active = item2.active;
        //                finalcat.Add(cat2);
        //                List<category> categories3 = db.categories.Where(x => x.parent_category == item2.id).ToList();
        //                foreach (var item3 in categories3)
        //                {
        //                    category cat3 = new category();
        //                    cat3.id = item3.id;
        //                    cat3.name = " - - - - " + item3.name;
        //                    cat3.active = item3.active;
        //                    finalcat.Add(cat3);
        //                    List<category> categories4 = db.categories.Where(x => x.parent_category == item3.id).ToList();
        //                    foreach (var item4 in categories4)
        //                    {
        //                        category cat4 = new category();
        //                        cat4.id = item4.id;
        //                        cat4.name = " - - - - - " + item4.name;
        //                        cat4.active = item4.active;
        //                        finalcat.Add(cat4);
        //                        List<category> categories5 = db.categories.Where(x => x.parent_category == item4.id).ToList();
        //                        foreach (var item5 in categories5)
        //                        {
        //                            category cat5 = new category();
        //                            cat5.id = item5.id;
        //                            cat5.name = " - - - - - - " + item5.name;
        //                            cat5.active = item5.active;
        //                            finalcat.Add(cat5);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return finalcat;
        //}

        //// show all categories for drop down list
        //// shows also first parent then child
        //public List<category> GetCategories()
        //{
        //    List<category> finalcat = new List<category>();
        //    List<category> categories = db.categories.Where(x => x.parent_category == null).ToList();
        //    foreach (var item in categories)
        //    {
        //        category cat = new category();
        //        cat.id = item.id;
        //        cat.name = item.name;
        //        cat.active = item.active;
        //        finalcat.Add(cat);
        //        List<category> categories1 = db.categories.Where(x => x.parent_category == item.id).ToList();
        //        foreach (var item1 in categories1)
        //        {
        //            category cat1 = new category();
        //            cat1.id = item1.id;
        //            cat1.name = " - " + item1.name;
        //            cat1.active = item1.active;
        //            finalcat.Add(cat1);
        //            List<category> categories2 = db.categories.Where(x => x.parent_category == item1.id).ToList();
        //            foreach (var item2 in categories2)
        //            {
        //                category cat2 = new category();
        //                cat2.id = item2.id;
        //                cat2.name = " - - " +item2.name;
        //                cat2.active = item2.active;
        //                finalcat.Add(cat2);
        //                List<category> categories3 = db.categories.Where(x => x.parent_category == item2.id).ToList();
        //                foreach (var item3 in categories3)
        //                {
        //                    category cat3 = new category();
        //                    cat3.id = item3.id;
        //                    cat3.name = " - - - " + item3.name;
        //                    cat3.active = item3.active;
        //                    finalcat.Add(cat3);
        //                    List<category> categories4 = db.categories.Where(x => x.parent_category == item3.id).ToList();
        //                    foreach (var item4 in categories4)
        //                    {
        //                        category cat4 = new category();
        //                        cat4.id = item4.id;
        //                        cat4.name = " - - - - " + item4.name;
        //                        cat4.active = item4.active;
        //                        finalcat.Add(cat4);
        //                        List<category> categories5 = db.categories.Where(x => x.parent_category == item4.id).ToList();
        //                        foreach (var item5 in categories5)
        //                        {
        //                            category cat5 = new category();
        //                            cat5.id = item5.id;
        //                            cat5.name = " - - - - - " + item5.name;
        //                            cat5.active = item5.active;
        //                            finalcat.Add(cat5);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return finalcat;
        //}

        // modify the category from here
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(category c, HttpPostedFileBase imgIcon, HttpPostedFileBase bannerImg, string active)
        {
            if (currentPageID("Category") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("Category")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            category category = db.categories.Where(x => x.id == c.id).FirstOrDefault();
            if (category != null)
            {
                Guid iconGuid = Guid.NewGuid();
                Guid bannerGuid = Guid.NewGuid();
                if (imgIcon != null && imgIcon.ContentLength > 0)
                {
                    var img = iconGuid.ToString() + Path.GetExtension(imgIcon.FileName);

                    var path = Path.Combine(Server.MapPath("~/Media/Images/Categories/"), img);
                    imgIcon.SaveAs(path);
                    category.icon = "/Media/Images/Categories/" + iconGuid.ToString() + Path.GetExtension(imgIcon.FileName);
                }
                if (bannerImg != null && bannerImg.ContentLength > 0)
                {
                    var img = bannerGuid.ToString() + Path.GetExtension(bannerImg.FileName);
                    var path = Path.Combine(Server.MapPath("~/Media/Images/Categories/"), img);
                    bannerImg.SaveAs(path);
                    category.banner = "/Media/Images/Categories/" + bannerGuid.ToString() + Path.GetExtension(bannerImg.FileName);
                }
                category.updated_at = DateTime.Now;
                category.name = c.name;
                category.parent_category = c.parent_category;
                category.text_color = c.text_color;
                category.description = c.description;
                category.bg_color = c.bg_color;
                category.active = c.active;
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // add category from here
        // image icon, banner image save into the directory and name save into the category table.
        // guid name replace the original file. so avoiding the duplication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(category c, HttpPostedFileBase imgIcon, HttpPostedFileBase bannerImg)
        {
            if (currentPageID("Category") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("Category")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            Guid iconGuid = Guid.NewGuid();
            Guid bannerGuid = Guid.NewGuid();
            if (imgIcon.ContentLength > 0)
            {
                var img = iconGuid.ToString() + Path.GetExtension(imgIcon.FileName);

                var path = Path.Combine(Server.MapPath("~/Media/Images/Categories/"), img);
                imgIcon.SaveAs(path);
                c.icon = "/Media/Images/Categories/" + iconGuid.ToString() + Path.GetExtension(imgIcon.FileName);
            }
            if (bannerImg != null && bannerImg.ContentLength > 0)
            {
                var img = bannerGuid.ToString() + Path.GetExtension(bannerImg.FileName);
                var path = Path.Combine(Server.MapPath("~/Media/Images/Categories/"), img);
                bannerImg.SaveAs(path);
                c.banner = "/Media/Images/Categories/" + bannerGuid.ToString() + Path.GetExtension(bannerImg.FileName);
            }
            
            c.created_at = DateTime.Now;
            db.categories.Add(c);
            db.SaveChanges();


            return RedirectToAction("Index");
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            if (currentPageID("Category") > 0)
            {
                if (!CurrentUser.canDelete(currentPageID("Category")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            category category = db.categories.Find(id);
            db.categories.Remove(category);
            db.SaveChanges();
            return Json("Success");
        }
    }

    public class ParentCategory
    {
        public IPagedList<category> categoryList { get; set; }
        public category Category { get; set; }
    }
}