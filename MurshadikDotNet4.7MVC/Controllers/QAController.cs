using PagedList;
using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Meilisearch;
using System.Threading.Tasks;

namespace MurshadikCP.Controllers
{
    [Authorize]
    public class QAController : BaseController
    {
        // GET: QA
        // Index view shows all Questions and Answers
        // pagination and search criteria also there
        // based on user_id also
        public async Task<ActionResult> Index(int? Page_No, string Search_Data, string Filter_Value, long user_id = 0, int criteria = 0, int category = 0, int sorting = 0, string start_date = null, string end_date = null)
        {
            if (currentPageID("QA") > 0)
            {
                if (!CurrentUser.canView(currentPageID("QA")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            int pagesize = Constants.defaultPageCount;
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
            ViewBag.Filter_criteria = criteria;
            ViewBag.Filter_category = category;
            ViewBag.Filter_sorting = sorting;
            //ViewBag.Filter_approvalWaiting = approvalWaiting;

            db.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);

            IQueryable<qa_questions> qa_Questions = from que in db.qa_questions select que;
            qa_Questions = qa_Questions.Include("qa_answers");
            qa_Questions = qa_Questions.Include("qa_category");
            qa_Questions = qa_Questions.Include("user");

            StatsCardModel totalQuestion = new StatsCardModel(Resources.Resources.TotalQuestions, qa_Questions.Count().ToString(), "bx bx-question-mark", bgColor: BgColors.Danger);
            StatsCardModel totalanswer = new StatsCardModel(Resources.Resources.TotalAnswers, db.qa_answers.Count().ToString(), "bx bx-comment-dots", bgColor: BgColors.Success);
            StatsCardModel approvedquestion = new StatsCardModel(Resources.Resources.ApprovedQuestions, db.qa_questions.Where(x => x.is_approved == true).Count().ToString(), "bx bx-check-shield", bgColor: BgColors.Info);
            StatsCardModel approvedanswer = new StatsCardModel(Resources.Resources.ApprovedAnswers, db.qa_answers.Where(x => x.is_approved == true).Count().ToString(), "bx bx-check-shield", bgColor: BgColors.Warning);

            List<StatsCardModel> statsCards = new List<StatsCardModel>();
            statsCards.Add(totalQuestion);
            statsCards.Add(totalanswer);
            statsCards.Add(approvedquestion);
            statsCards.Add(approvedanswer);

            ViewBag.StatsCards = statsCards;

            if (!string.IsNullOrEmpty(start_date))
            {
                ViewBag.Start_Date = start_date;
                var sdt = DateTime.Parse(start_date, null, System.Globalization.DateTimeStyles.RoundtripKind);
                qa_Questions = qa_Questions.Where(art => art.created_at >= sdt);
            }
            if (!string.IsNullOrEmpty(end_date))
            {
                ViewBag.End_Date = end_date;
                var edt = DateTime.Parse(end_date, null, System.Globalization.DateTimeStyles.RoundtripKind);
                qa_Questions = qa_Questions.Where(art => art.created_at <= edt);
            }

            if (user_id > 0)
            {
                if (!String.IsNullOrEmpty(Search_Data))
                {
                    qa_Questions = qa_Questions.Where(x => x.user_id == user_id && (x.title.Contains(Search_Data) || x.description.Contains(Search_Data)));
                }
                else
                {
                    qa_Questions = qa_Questions.Where(x => x.user_id == user_id);
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(Search_Data))
                {
                    qa_Questions = qa_Questions.Where(x => x.title.Contains(Search_Data) || x.description.Contains(Search_Data));
                }
            }

            if (criteria == 1)
            {
                qa_Questions = qa_Questions.Where(x => x.is_approved == true);
            }
            else if (criteria == 2)
            {
                var answer = db.qa_answers.Where(x => x.is_approved == true).Select(x => x.question_id).ToList();
                qa_Questions = qa_Questions.Where(x => x.is_approved == true && answer.Contains(x.id));
            }
            else if (criteria == 3)
            {
                var answer = db.qa_answers.Where(x => x.is_approved == false).Select(x => x.question_id).ToList();
                qa_Questions = qa_Questions.Where(x => x.is_approved == true && answer.Contains(x.id));
            }
            else if (criteria == 4)
            {
                qa_Questions = qa_Questions.Where(x => x.is_approved == false);
            }

            if (category == 1)
            {
                qa_Questions = qa_Questions.Where(x => x.qa_category_id == 1);
            }
            else if (category == 2)
            {
                qa_Questions = qa_Questions.Where(x => x.qa_category_id == 2);
            }

            if (sorting == 1)
            {
                qa_Questions = qa_Questions.OrderByDescending(x => x.vote_count);
            }
            else if (sorting == 2)
            {
                qa_Questions = qa_Questions.OrderByDescending(x => x.qa_answers.Count());
            }
            else if (sorting == 3)
            {
                qa_Questions = qa_Questions.OrderByDescending(x => x.qa_answers.OrderByDescending(c => c.vote_count).Select(y => y.vote_count).Max());
            }
            else
            {
                qa_Questions = qa_Questions.OrderByDescending(x => x.created_at);
            }

            //if (approvalWaiting == 1)
            //{
            //    qa_Questions = qa_Questions.Where(x => x.qa_answers.Where(y => y.is_approved == false).Any());
            //}

            return View(qa_Questions.ToPagedList(No_Of_Page, pagesize));

        }
        // view for Answers related to the question
        public ActionResult Answers(int id)
        {

            if (currentPageID("Answer") > 0)
            {
                if (!CurrentUser.canView(currentPageID("Answer")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            qa_questions _Questions = db.qa_questions.Where(x => x.id == id).FirstOrDefault();

            List<qa_attachments> q_attachments = db.qa_attachments.Where(x => x.type_id == _Questions.id && x.type == false).ToList();
            if (q_attachments != null && q_attachments.Count() > 0)
            {
                string[] attach = new string[q_attachments.Count()];
                for (int i = 0; i < q_attachments.Count(); i++)
                {
                    attach[i] = db.media.Find(q_attachments[i].media_id).file_name;
                }

                ViewBag.questionAttachments = attach;
            }

            List<ans_attachment> a_attachments = new List<ans_attachment>();
            foreach (var item in _Questions.qa_answers)
            {
                List<qa_attachments> ans_attach = db.qa_attachments.Where(x => x.type == true && x.type_id == item.id).ToList();
                if (ans_attach != null && ans_attach.Count() > 0)
                {
                    for (int i = 0; i < ans_attach.Count(); i++)
                    {
                        ans_attachment ans = new ans_attachment();
                        ans.id = item.id;
                        ans.file_name = db.media.Find(ans_attach[i].media_id).file_name;
                        a_attachments.Add(ans);
                    }
                }
            }
            ViewBag.answerAttachments = a_attachments;

            return View(_Questions);
        }
        // edit or modify the question
        public ActionResult Edit(int id)
        {
            if (currentPageID("QA") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("QA")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            qa_questions questions = db.qa_questions.Where(x => x.id == id).FirstOrDefault();
            return View(questions);
        }

        //[HttpPost]
        //public ActionResult Answer_delete(int id, int question_id)
        //{
        //    ui = (UserInfo)Session["User"];
        //    qa_questions _Questions = db.qa_questions.Where(x => x.id == id).FirstOrDefault();
        //    if (_Questions != null)
        //    {
        //        _Questions.is_verified = true;
        //        _Questions.verified_at = DateTime.Now;
        //        _Questions.verified_by = ui.Id;
        //        db.Entry(_Questions).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return Json("Success");
        //    }
        //    return Json("Success");
        //}

        // question verify by consultant or admin
        [HttpPost]
        public ActionResult is_verified(int id)
        {
            if (currentPageID("QA") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("QA")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            qa_questions _Questions = db.qa_questions.Where(x => x.id == id).FirstOrDefault();
            if (_Questions != null)
            {
                _Questions.is_verified = true;
                _Questions.verified_at = DateTime.Now;
                _Questions.verified_by = CurrentUser.Id;
                db.Entry(_Questions).State = EntityState.Modified;
                db.SaveChanges();
                return Json("Success");
            }
            return Json("Question Not Found");

        }

        private async Task<bool> DeleteQuestionFromChatBot(long qID)
        {
            try
            {
                var ChatBotURL = WebConfigurationManager.AppSettings["ChatBotURL"].ToString();
                var ChatBotSecret = WebConfigurationManager.AppSettings["ChatBotSecret"].ToString();
                MeilisearchClient client = new MeilisearchClient(ChatBotURL, ChatBotSecret);

                var ChatBotDocument = WebConfigurationManager.AppSettings["ChatBotDocument"].ToString();

                var searchIndex = client.Index(ChatBotDocument);
                var dde = await searchIndex.DeleteOneDocument(qID.ToString());
                //var update = await searchIndex.AddDocuments<CB_Model>(new[] { qaQCBModel });
                return true;
            }
            catch (Exception ex)
            {
                return false;   
            }
        }

        public async Task<string> AddQuestionToChatBot(qa_questions question)
        {
            var ChatBotURL = WebConfigurationManager.AppSettings["ChatBotURL"].ToString();
            var ChatBotSecret = WebConfigurationManager.AppSettings["ChatBotSecret"].ToString();
            var ChatBotDocument = WebConfigurationManager.AppSettings["ChatBotDocument"].ToString();
            try
            {
                
                MeilisearchClient client = new MeilisearchClient(ChatBotURL, ChatBotSecret);
                var qaa = db.qa_attachments.Where(x => x.type_id == question.id).FirstOrDefault();
                string image_filename = null;
                if (qaa != null){
                    image_filename = db.media.Where(m => m.id == qaa.media_id).FirstOrDefault().file_name;
                }

                CB_Model qaQCBModel = new CB_Model() {
                    title = question.title,
                    created_at = question.created_at.ToString(),
                    id = question.id.ToString(),
                    image = "https://mewa-ershad.net/Media/QA/Question/" + image_filename,
                    kw = question.keywords,
                    link = "",
                    timestamp = (Int32)(question.created_at.Value.Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                    type = "qna"
                };
                
                var searchIndex = client.Index(ChatBotDocument);
                //var update = await index.AddDocuments<Book>(documents); # => { "updateId": 0 }
                var update = await searchIndex.AddDocuments<CB_Model>(new[] { qaQCBModel});
                //SearchResult<CB_Model> results = await searchIndex.Search<CB_Model>("محاصيل حقلية", new SearchQuery { Limit = 5, Matches = false });
                //foreach (var result in results.Hits)
                //{
                //    Console.WriteLine(result);
                //    Console.ReadLine();
                //}
                return update.UpdateId.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return ex.Message + " - " + ex.InnerException.InnerException.Message;
            }

        }

        // question approved by consultant or admin
        [HttpPost]
        public async Task<ActionResult> is_approved(int id)
        {
            if (currentPageID("QA") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("QA")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            qa_questions _Questions = db.qa_questions.Where(x => x.id == id).FirstOrDefault();
            if (_Questions != null)
            {
                bool ConsultantCanApproved = Convert.ToBoolean(WebConfigurationManager.AppSettings["IsAnswerForConsultantDirectlyApproved"].ToString());
                if (_Questions.user.role_id == 6 && ConsultantCanApproved)
                {
                    _Questions.is_verified = true;
                    _Questions.verified_at = DateTime.Now;
                }
                _Questions.is_approved = true;
                _Questions.approved_at = DateTime.Now;
                _Questions.approved_by = CurrentUser.Id;
                db.Entry(_Questions).State = EntityState.Modified;
                db.SaveChanges();

                // Add document to ChatBot
                var addQuestion = await AddQuestionToChatBot(_Questions);

                Generic g = new Generic();
                
                g.sendNotificationOnChats("سؤال", "تمت الموافقة على سؤالك!", _Questions.user.phone, "question_approved", id.ToString());
                g.InsertNotificationData(_Questions.user_id.Value, "تمت الموافقة على سؤالك", (Int16)AppConstants.Type.Question, id, "");

                string message = "تمت الموافقة على السؤال";

                return Json(new ErrorModel(message, true));
            }
            return Json(new ErrorModel("", false));
        }

        

        // answer verified by admin
        [HttpPost]
        public ActionResult Answer_is_verified(int id)
        {
            if (currentPageID("QA") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("QA")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            qa_answers _Answers = db.qa_answers.Where(x => x.id == id).FirstOrDefault();
            if (_Answers != null)
            {
                _Answers.is_verified = true;
                _Answers.verified_at = DateTime.Now;
                _Answers.verified_by = CurrentUser.Id;
                db.Entry(_Answers).State = EntityState.Modified;
                db.SaveChanges();
                return Json("Success");
            }
            return Json("Success");
        }

        // answer approved by admin
        [HttpPost]
        public ActionResult Answer_is_approved(int id)
        {
            if (currentPageID("QA") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("QA")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            qa_answers _Answers = db.qa_answers.Where(x => x.id == id).FirstOrDefault();
            if (_Answers != null)
            {
                bool ConsultantCanApproved = Convert.ToBoolean(WebConfigurationManager.AppSettings["IsAnswerForConsultantDirectlyApproved"].ToString());
                if (_Answers.user.role_id == (int)Role.Consultants && ConsultantCanApproved)
                {
                    _Answers.is_verified = true;
                    _Answers.verified_at = DateTime.Now;
                }
                _Answers.is_approved = true;
                _Answers.approved_at = DateTime.Now;
                _Answers.approved_by = CurrentUser.Id;
                db.Entry(_Answers).State = EntityState.Modified;
                db.SaveChanges();

                Generic g = new Generic();
                //g.sendNotificationOnChats("إجابة", "تمت الموافقة على إجابتك!", _Answers.user.phone);
                g.sendNotificationOnChats("إجابة", "تم نشر إجابتك!", _Answers.user.phone, "answer_approved", _Answers.question_id.ToString());

                g.InsertNotificationData(_Answers.user_id.Value, "تم نشر إجابتك", (Int16)AppConstants.Type.Answer, _Answers.question_id.Value, "");


                // for question user send notification when his or her question answered by another user.
                qa_questions _Questions = db.qa_questions.Where(x => x.id == _Answers.question_id).FirstOrDefault();
                if (_Questions != null)
                {
                    g.sendNotificationOnChats("إجابة", "تمت الإجابة على سؤالك!", _Questions.user.phone, "answer_approved", _Answers.question_id.ToString());

                    g.InsertNotificationData(_Questions.user_id.Value, "تمت الإجابة على سؤالك", (Int16)AppConstants.Type.Answer, _Answers.question_id.Value, "");
                }
                return Json("Success");
            }
            return Json("Success");
        }

        // POST: QA/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(long id)
        {
            if (currentPageID("QA") > 0)
            {
                if (!CurrentUser.canDelete(currentPageID("QA")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            qa_questions _q = db.qa_questions.Find(id);

            db.qa_questions.Remove(_q);
            db.SaveChanges();

            bool questionDeleted = await DeleteQuestionFromChatBot(id);

            return Json("Success");
        }

        // POST: QA/Delete/5
        [HttpPost]
        public ActionResult Answer_Delete(long id, long question_id)
        {
            if (currentPageID("QA") > 0)
            {
                if (!CurrentUser.canDelete(currentPageID("QA")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            qa_answers _a = db.qa_answers.Find(id);

            db.qa_answers.Remove(_a);
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

    public class ans_attachment
    {
        public long id { get; set; }
        public string file_name { get; set; }
    }

    public class CB_Model
    {
        public string id { get; set; }
        public string title { get; set; }
        public string created_at { get; set; }
        public long timestamp { get; set; }
        public string kw { get; set; }
        public string image { get; set; }
        public string type { get; set; }
        public string link { get; set; }
    }
}