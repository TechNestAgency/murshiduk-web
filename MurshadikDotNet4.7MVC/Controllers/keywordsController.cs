using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using MurshadikCP.Models;
using MurshadikCP.Models.DB;

namespace MurshadikCP.Controllers
{
    [Authorize]
    public class keywordsController : BaseController
    {
        // GET: keywords
        // Index view shows list of keywords
        // pagination and search criteria also there
        public ActionResult Index(int? Page_No, string Search_Data, string Filter_Value, long keyword_id = 0)
        {
            if (currentPageID("keywords") > 0)
            {
                if (!CurrentUser.canView(currentPageID("keywords")))
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

            ViewBag.keywords_Name = new SelectList(db.keywords.ToList(), "keyword1", "keyword1");
            ParentKeyword pk = new ParentKeyword();
            if (keyword_id > 0)
            {
                if (!String.IsNullOrEmpty(Search_Data))
                {
                    pk.keywordList = db.keywords.Where(x => x.keyword1.Contains(Search_Data)).OrderByDescending(x => x.id).ToPagedList(No_Of_Page, pagesize);
                    pk.keywordGroupList = db.keyword_group.Where(x => x.keyword_group_name.Contains(Search_Data) || x.group_keywords.Contains(Search_Data)).OrderByDescending(x => x.id).ToPagedList(No_Of_Page, pagesize);
                }
                else
                {
                    pk.keywordList = db.keywords.OrderByDescending(x => x.id).ToPagedList(No_Of_Page, pagesize);
                    pk.keywordGroupList = db.keyword_group.OrderByDescending(x => x.id).ToPagedList(No_Of_Page, pagesize);
                }
                pk.Keyword = db.keywords.Where(x => x.id == keyword_id).FirstOrDefault();
            }
            else
            {
                if (!String.IsNullOrEmpty(Search_Data))
                {
                    pk.keywordList = db.keywords.Where(x => x.keyword1.Contains(Search_Data)).OrderByDescending(x => x.id).ToPagedList(No_Of_Page, pagesize);
                    pk.keywordGroupList = db.keyword_group.Where(x => x.keyword_group_name.Contains(Search_Data) || x.group_keywords.Contains(Search_Data)).OrderByDescending(x => x.id).ToPagedList(No_Of_Page, pagesize);
                }
                else
                {
                    pk.keywordList = db.keywords.OrderByDescending(x => x.id).ToPagedList(No_Of_Page, pagesize);
                    pk.keywordGroupList = db.keyword_group.OrderByDescending(x => x.id).ToPagedList(No_Of_Page, pagesize);
                }
                pk.Keyword = new keyword();
            }
            return View(pk);
        }

        // this method adding the keyword group name
        // so if user want to add some media and put the same keywords so just select the keyword group from drop down it comes all keywords
        [HttpPost]
        public ActionResult AddKeywordGroup(string keyword_group_name, string keywords, int keyword_group_id = 0)
        {
            if (currentPageID("keywords") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("keywords")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (keyword_group_name != "" && keywords != "")
            {
                
                keyword_group kg = new keyword_group();
                kg.keyword_group_name = keyword_group_name;
                kg.group_keywords = keywords;
                if (keyword_group_id > 0)
                {
                    kg.id = keyword_group_id;
                    db.Entry(kg).State = EntityState.Modified;
                }
                else
                {
                    db.keyword_group.Add(kg);
                }
                db.SaveChanges();
                return Json("Success");
            }
            return Json("error");
        }

        // GET: keywords/Create
        public ActionResult Create()
        {
            if (currentPageID("keywords") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("keywords")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            return View();
        }

        // POST: keywords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(keyword keyword)
        {
            if (currentPageID("keywords") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("keywords")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (ModelState.IsValid)
            {
                keyword.created_at = DateTime.Now;
                db.keywords.Add(keyword);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(keyword);
        }

        // GET: keywords/Edit/5
        public ActionResult Edit(long? id)
        {
            if (currentPageID("keywords") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("keywords")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            keyword keyword = db.keywords.Find(id);
            if (keyword == null)
            {
                return HttpNotFound();
            }
            return View(keyword);
        }

        // POST: keywords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,keyword1,created_at,updated_at")] keyword keyword, long keyword_id)
        {
            if (currentPageID("keywords") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("keywords")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (ModelState.IsValid)
            {
                keyword.id = keyword_id;
                keyword.created_at = DateTime.Now;
                db.Entry(keyword).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(keyword);
        }

        // if the keywords is not exists then it add first in our database
        // this method call automatically from the view based on some condition
        [HttpPost]
        public JsonResult AddKeywords(string name)
        {
            keyword keyword = db.keywords.Where(x => x.keyword1 == name).FirstOrDefault();
            if (keyword == null)
            {
                keyword = new keyword();
                keyword.keyword1 = name;
                keyword.created_at = DateTime.Now;
                db.keywords.Add(keyword);
                db.SaveChanges();
            }
            return Json("Success");
        }

        [HttpPost, ActionName("DeleteKeywordGroup")]
        public ActionResult DeleteKeywordGroup(int id)
        {
            if (currentPageID("keywords") > 0)
            {
                if (!CurrentUser.canDelete(currentPageID("keywords")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            keyword_group keyword = db.keyword_group.Find(id);
            db.keyword_group.Remove(keyword);
            db.SaveChanges();
            return Json("Success");
        }

        // POST: keywords/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            if (currentPageID("keywords") > 0)
            {
                if (!CurrentUser.canDelete(currentPageID("keywords")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            keyword keyword = db.keywords.Find(id);
            db.keywords.Remove(keyword);
            db.SaveChanges();
            return Json("Success");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
    public class ParentKeyword
    {
        public IPagedList<keyword> keywordList { get; set; }
        public IPagedList<keyword_group> keywordGroupList { get; set; }
        public keyword Keyword { get; set; }
        public keyword_group keyword_Group { get; set; }
    }
}
