using MurshadikCP.Models.DB;
using MurshadikCP.Repositories;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MurshadikCP.Controllers
{
    public class faqsController : BaseController
    {
        // GET: faqs

        faqsRepository fr = new faqsRepository();
        public ActionResult Index(int? Page_No, string Search_Data, string Filter_Value, long faqId = 0)
        {
            if (currentPageID("faqs") > 0)
            {
                if (!CurrentUser.canView(currentPageID("faqs")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            List<faq> faqs = db.faqs.OrderBy(x => x.sort).ToList();
            int pagesize = 60;
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

            ParentFaqs pf = new ParentFaqs();
            
            pf.faqList = faqs.ToPagedList(No_Of_Page, pagesize);
            //ViewBag.parent_skill = new SelectList(skills, "id", "name");

            if (faqId > 0)
            {
                pf.faq = db.faqs.Where(x => x.id == faqId).FirstOrDefault();
            }
            else
            {
                pf.faq = new faq();
            }

            if (!String.IsNullOrEmpty(Search_Data))
            {
                pf.faqList = db.faqs.Where(x => x.question.Contains(Search_Data) || x.answer.Contains(Search_Data)).ToPagedList(No_Of_Page, pagesize);
            }

            return View(pf);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ParentFaqs f)
        {
            if (currentPageID("faqs") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("faqs")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (f.faq.id == 0)
            {
                f.faq.sort = fr.FaqsSort() + 1;
                f.faq.created_at = DateTime.Now;
                f.faq.created_by = CurrentUser.Id;
                f.faq.updated_at = DateTime.Now;
                fr.Addfaqs(f.faq);
            }
            else
            {
                f.faq.updated_by = CurrentUser.Id;
                f.faq.updated_at = DateTime.Now;
                fr.Updatefaqs(f.faq);
            }
            fr.Save();
            int page = 0;
            if (Request.UrlReferrer.Query.Contains("Page_No"))
            {
                page = Convert.ToInt16(HttpUtility.ParseQueryString(Request.UrlReferrer.Query)["Page_No"]);
            }
            else
            {
                page = 1;
            }

            return RedirectToAction("Index", new { Page_No = page });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ParentFaqs f, long fid)
        {
            if (currentPageID("faqs") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("faqs")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (ModelState.IsValid)
            {
                faq mainfaq = db.faqs.Find(fid);
                mainfaq.question = f.faq.question;
                mainfaq.answer = f.faq.answer;
                mainfaq.active = f.faq.active;
                //mainfaq.sort = f.faq.sort;
                mainfaq.updated_at = DateTime.Now;
                mainfaq.updated_by = CurrentUser.Id;
                db.Entry(mainfaq).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Successfully Updated!";
                //return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        // sorting by jquery
        [HttpPost]
        public ActionResult Sorting(int[] sortId)
        {

            //HolidayLocationsEntities entities = new HolidayLocationsEntities();
            int preference = 1;
            foreach (int id in sortId)
            {
                var faqs = db.faqs.Find(id);
                faqs.sort = preference;
                //db.SaveChanges();
                preference += 1;
            }
            db.SaveChanges();
            //return View(entities.HolidayLocations.OrderBy(p => p.Preference).ToList());
            return RedirectToAction("Index");
        }

        // POST: Skill/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            if (currentPageID("Skill") > 0)
            {
                if (!CurrentUser.canDelete(currentPageID("Skill")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            faq faq = db.faqs.Find(id);
            db.faqs.Remove(faq);
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

    public class ParentFaqs
    {
        public IPagedList<faq> faqList { get; set; }
        public faq faq { get; set; }
    }
}