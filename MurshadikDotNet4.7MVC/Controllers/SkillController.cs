using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MurshadikCP.Models.DB;
using PagedList;
using MurshadikCP.Models;
using System.IO;

namespace MurshadikCP.Controllers
{
    [Authorize]
    public class SkillController : BaseController
    {
        //private mlaraEntities db = new mlaraEntities();
        //UserInfo ui = new UserInfo();
        // GET: Skill
        // Index view shows skills list
        public ActionResult Index(int? Page_No, string Search_Data, string Filter_Value, long skillId = 0)
        {
            if (currentPageID("Skill") > 0)
            {
                if (!CurrentUser.canView(currentPageID("Skill")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            List<skill> skills = GetSkill();
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

            ParentSkill ps = new ParentSkill();
            ps.skillList = skills.ToPagedList(No_Of_Page, pagesize);
            ViewBag.parent_skill = new SelectList(skills, "id", "name");

            if (skillId > 0)
            {
                ps.skill = db.skills.Where(x => x.id == skillId).FirstOrDefault();
                ViewBag.parent_skill = new SelectList(skills, "id", "name", ps.skill.parent_skill);
            }
            else
            {
                ps.skill = new skill();
            }

            if (!String.IsNullOrEmpty(Search_Data))
            {
                ps.skillList = ps.skillList.Where(x => x.name.Contains(Search_Data)).ToPagedList(No_Of_Page, pagesize);
            }

            return View(ps);
        }
        // this method for bind the drop down list according to the parent id
        public List<skill> GetSkill()
        {
            List<skill> finalskill = new List<skill>();
            List<skill> skills = db.skills.Where(x => x.parent_skill == null).OrderBy(x => x.menu_sort).ToList();
            foreach (var item in skills)
            {
                skill cat = new skill();
                cat.id = item.id;
                cat.name = item.name;
                cat.description = item.description;
                cat.parent_skill = item.parent_skill;
                cat.active = item.active;
                cat.icon = item.icon;
                cat.banner = item.banner;
                cat.bg_color = item.bg_color;
                cat.text_color = item.text_color;
                finalskill.Add(cat);
                List<skill> skills1 = db.skills.Where(x => x.parent_skill == item.id).ToList();
                foreach (var item1 in skills1)
                {
                    skill cat1 = new skill();
                    cat1.id = item1.id;
                    cat1.name = " - " + item1.name;
                    cat1.description = item1.description;
                    cat1.parent_skill = item1.parent_skill;
                    cat1.active = item1.active;
                    cat1.icon = item1.icon;
                    cat1.banner = item1.banner;
                    cat1.bg_color = item1.bg_color;
                    cat1.text_color = item1.text_color;
                    finalskill.Add(cat1);
                    List<skill> skills2 = db.skills.Where(x => x.parent_skill == item1.id).ToList();
                    foreach (var item2 in skills2)
                    {
                        skill cat2 = new skill();
                        cat2.id = item2.id;
                        cat2.name = " - - " + item2.name;
                        cat2.description = item2.description;
                        cat2.parent_skill = item2.parent_skill;
                        cat2.active = item2.active;
                        cat2.icon = item2.icon;
                        cat2.banner = item2.banner;
                        cat2.bg_color = item2.bg_color;
                        cat2.text_color = item2.text_color;
                        finalskill.Add(cat2);
                        List<skill> skills3 = db.skills.Where(x => x.parent_skill == item2.id).ToList();
                        foreach (var item3 in skills3)
                        {
                            skill cat3 = new skill();
                            cat3.id = item3.id;
                            cat3.name = " - - - " + item3.name;
                            cat3.description = item3.description;
                            cat3.parent_skill = item3.parent_skill;
                            cat3.active = item3.active;
                            cat3.icon = item3.icon;
                            cat3.banner = item3.banner;
                            cat3.bg_color = item3.bg_color;
                            cat3.text_color = item3.text_color;
                            finalskill.Add(cat3);
                            List<skill> skills4 = db.skills.Where(x => x.parent_skill == item3.id).ToList();
                            foreach (var item4 in skills4)
                            {
                                skill cat4 = new skill();
                                cat4.id = item4.id;
                                cat4.name = " - - - - " + item4.name;
                                cat4.description = item4.description;
                                cat4.parent_skill = item4.parent_skill;
                                cat4.active = item4.active;
                                cat4.icon = item4.icon;
                                cat4.banner = item4.banner;
                                cat4.bg_color = item4.bg_color;
                                cat4.text_color = item4.text_color;
                                finalskill.Add(cat4);
                                List<skill> skills5 = db.skills.Where(x => x.parent_skill == item4.id).ToList();
                                foreach (var item5 in skills5)
                                {
                                    skill cat5 = new skill();
                                    cat5.id = item5.id;
                                    cat5.name = " - - - - - " + item5.name;
                                    cat5.description = item5.description;
                                    cat5.parent_skill = item5.parent_skill;
                                    cat5.active = item5.active;
                                    cat5.icon = item5.icon;
                                    cat5.banner = item5.banner;
                                    cat5.bg_color = item5.bg_color;
                                    cat5.text_color = item5.text_color;
                                    finalskill.Add(cat5);
                                }
                            }
                        }
                    }
                }
            }
            return finalskill;
        }

        // sorting by jquery
        [HttpPost]
        public ActionResult Sorting(int[] sortId)
        {

            //HolidayLocationsEntities entities = new HolidayLocationsEntities();
            int preference = 1;
            foreach (int id in sortId)
            {
                var skill = db.skills.Find(id);
                skill.menu_sort = preference;
                //db.SaveChanges();
                preference += 1;
            }
            db.SaveChanges();
            //return View(entities.HolidayLocations.OrderBy(p => p.Preference).ToList());
            return RedirectToAction("Index");
        }


        // GET: Skill/Create
        //public ActionResult Create()
        //{
        //    ui = (UserInfo)Session["User"];
        //    if (ui != null)
        //    {
        //        List<roles_permission> roles = ui.roles_Permissions.ToList();
        //        if (roles != null)
        //        {
        //            if (roles.Exists(x => x.Page.PageName == "Skill"))
        //            {
        //                if (roles.Find(x => x.Page.PageName == "Skill").can_insert)
        //                {
        //                    ViewBag.parent_category = new SelectList(db.categories.ToList(), "id", "name");
        //                    return View();
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("Login", "Account");
        //    }

        //    return RedirectToAction("NotAllow", "Custom");
        //}

        // POST: Skill/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(skill skill, string parent_skill, HttpPostedFileBase imgIcon, HttpPostedFileBase bannerImg, string bg_color, string text_color)
        {
            if (currentPageID("Skill") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("Skill")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (ModelState.IsValid)
            {
                Guid iconGuid = Guid.NewGuid();
                Guid bannerGuid = Guid.NewGuid();
                if (imgIcon.ContentLength > 0)
                {
                    var img = iconGuid.ToString() + Path.GetExtension(imgIcon.FileName);

                    var path = Path.Combine(Server.MapPath("~/Media/Images/Skills/"), img);
                    imgIcon.SaveAs(path);
                    skill.icon = "/Media/Images/Skills/" + iconGuid.ToString() + Path.GetExtension(imgIcon.FileName);
                }
                if (bannerImg.ContentLength > 0)
                {
                    var img = bannerGuid.ToString() + Path.GetExtension(bannerImg.FileName);
                    var path = Path.Combine(Server.MapPath("~/Media/Images/Skills/"), img);
                    bannerImg.SaveAs(path);
                    skill.banner = "/Media/Images/Skills/" + bannerGuid.ToString() + Path.GetExtension(bannerImg.FileName);
                }

                if (parent_skill != null && parent_skill != "")
                {
                    skill.parent_skill = Convert.ToInt32(parent_skill);
                }
                skill.bg_color = bg_color;
                skill.text_color = text_color;
                skill.created_at = DateTime.Now;
                db.skills.Add(skill);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(skill);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(skill skill, string parent_skill, long sid, HttpPostedFileBase imgIcon, HttpPostedFileBase bannerImg, string bg_color, string text_color)
        {
            if (currentPageID("Skill") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("Skill")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (ModelState.IsValid)
            {
                skill mainSkill = db.skills.Find(sid);
                
                Guid iconGuid = Guid.NewGuid();
                Guid bannerGuid = Guid.NewGuid();
                if (imgIcon != null && imgIcon.ContentLength > 0)
                {
                    var img = iconGuid.ToString() + Path.GetExtension(imgIcon.FileName);

                    var path = Path.Combine(Server.MapPath("~/Media/Images/Skills/"), img);
                    imgIcon.SaveAs(path);
                    mainSkill.icon = "/Media/Images/Skills/" + iconGuid.ToString() + Path.GetExtension(imgIcon.FileName);
                }
                if (bannerImg != null && bannerImg.ContentLength > 0)
                {
                    var img = bannerGuid.ToString() + Path.GetExtension(bannerImg.FileName);
                    var path = Path.Combine(Server.MapPath("~/Media/Images/Skills/"), img);
                    bannerImg.SaveAs(path);
                    mainSkill.banner = "/Media/Images/Skills/" + bannerGuid.ToString() + Path.GetExtension(bannerImg.FileName);
                }

                if (parent_skill != null && parent_skill != "")
                {
                    mainSkill.parent_skill = Convert.ToInt32(parent_skill);
                }
                mainSkill.name = skill.name;
                mainSkill.description = skill.description;
                mainSkill.text_color = text_color;
                mainSkill.bg_color = bg_color;
                mainSkill.id = sid;
                mainSkill.active = skill.active;
                mainSkill.updated_at = DateTime.Now;
                db.Entry(mainSkill).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Successfully Updated!";
                //return RedirectToAction("Index");
            }
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
            skill skill = db.skills.Find(id);
            db.skills.Remove(skill);
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

    public class ParentSkill
    {
        public IPagedList<skill> skillList { get; set; }
        public skill skill { get; set; }
    }
}
