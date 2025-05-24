using MurshadikCP.Controllers;
using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Hosting;

namespace MurshadikCP.Repositories
{
    public class QARepository : GenericRepository<qa_questions>, IQARepository
    {
        public qa_questions GetQuestionsByID(long id)
        {
            return table.Where(u => u.id == id).SingleOrDefault();
        }

        public string CheckFileFormat(string type)
        {
            string filetype = "";
            if (type.Contains("image"))
            {
                filetype = "Image";
            }
            else if (type.Contains("video"))
            {
                filetype = "Video";
            }

            return filetype;
        }

        public string CategoryName(int id)
        {
            return _db.categories.Where(x => x.id == id).FirstOrDefault().name;
        }

        public Data GetQuestionDetail(long id, long user_id)
        {
            Data d = new Data();

            var questions = _db.qa_questions.Where(x => (x.id == id && x.is_approved == true) || (x.id == id && x.user_id == user_id)).Select(p => new
            {
                p.id,
                p.title,
                p.user_id,
                p.description,
                p.qa_category_id,
                date = p.created_at,
                p.keywords,
                p.is_verified,
                p.is_approved,
                p.vote_count,
                created_by = p.user.name,
                attachments = _db.qa_attachments.Where(z => z.type_id == p.id && z.type == false).Select(
                    a => new
                    {
                        id = a.id,
                        media_id = a.media_id,
                        type_id = a.type_id,
                        type = a.type,
                        url = _db.media.Where(x => x.id == a.media_id).FirstOrDefault() != null ? "/Media/QA/Question/" + _db.media.Where(x => x.id == a.media_id).FirstOrDefault().file_name : ""
                    }
                    ),
                answers = p.qa_answers.Where(x => x.is_approved == true || x.user_id == user_id).ToList().Select(ans => new
                {
                    ans.id,
                    ans.title,
                    ans.description,
                    date = ans.created_at,
                    ans.vote_count,
                    ans.is_verified,
                    ans.is_approved,
                    created_by = ans.user.name,
                    avatar = ans.user.avatar,
                    user_id = ans.user.id,
                    approved_by = ans.user1.name,
                    verified_by = ans.user2.name,
                    attachments = _db.qa_attachments.Where(z => z.type_id == ans.id && z.type == true).Select(a => new
                    {
                        id = a.id,
                        media_id = a.media_id,
                        type_id = a.type_id,
                        type = a.type,
                        url = _db.media.Where(x => x.id == a.media_id).FirstOrDefault() != null ? "/Media/QA/Answer/" + _db.media.Where(x => x.id == a.media_id).FirstOrDefault().file_name : ""
                    })
                })
            }).FirstOrDefault();
            d.status = true;
            d.message = "successfully fetch the records";
            d.data = questions;
            return d;
        }

        public Data AnswerVote(long aid, int value, user user)
        {
            Data d = new Data();
            using (var dbContextTransaction = _db.Database.BeginTransaction())
            {
                // for updating QA_Question table
                qa_answers a = _db.qa_answers.Where(x => x.id == aid).FirstOrDefault();
                if (a != null)
                {
                    if (a.user_id != user.id)
                    {
                        if (value == 1)
                        {
                            qa_votes qa = _db.qa_votes.Where(y => y.type == true && y.type_id == a.id && y.user_id == user.id && y.is_up_vote == true).FirstOrDefault();
                            if (qa == null)
                            {
                                a.vote_count = a.vote_count + 1;
                                _db.Entry(a).State = EntityState.Modified;
                                _db.SaveChanges();

                                qa_votes qa1 = _db.qa_votes.Where(y => y.type == true && y.type_id == a.id && y.user_id == user.id && y.is_up_vote == false).FirstOrDefault();
                                if (qa1 != null)
                                {
                                    qa1.is_up_vote = true;
                                    _db.Entry(qa1).State = EntityState.Modified;
                                    _db.SaveChanges();
                                }
                                else
                                {
                                    //for inserting into QA_votes table
                                    qa_votes v = new qa_votes();
                                    v.created_at = DateTime.Now;
                                    v.is_up_vote = value == 1 ? true : false;
                                    v.type = true; // 0 for question and 1 for Answer
                                    v.type_id = aid;
                                    v.user_id = user.id;
                                    _db.qa_votes.Add(v);
                                    _db.SaveChanges();
                                }
                            }
                        }
                        else if (value == 0)
                        {
                            qa_votes qa = _db.qa_votes.Where(y => y.type == true && y.type_id == a.id && y.user_id == user.id && y.is_up_vote == false).FirstOrDefault();
                            if (qa == null)
                            {
                                a.vote_count = a.vote_count - 1;
                                if (a.vote_count < 0)
                                {
                                    a.vote_count = 0;
                                }
                                _db.Entry(a).State = EntityState.Modified;
                                _db.SaveChanges();

                                qa_votes qa1 = _db.qa_votes.Where(y => y.type == true && y.type_id == a.id && y.user_id == user.id && y.is_up_vote == true).FirstOrDefault();
                                if (qa1 != null)
                                {
                                    qa1.is_up_vote = false;
                                    _db.Entry(qa1).State = EntityState.Modified;
                                    _db.SaveChanges();
                                }
                                else
                                {
                                    //for inserting into QA_votes table
                                    qa_votes v = new qa_votes();
                                    v.created_at = DateTime.Now;
                                    v.is_up_vote = value == 1 ? true : false;
                                    v.type = true; // 0 for question and 1 for Answer
                                    v.type_id = aid;
                                    v.user_id = user.id;
                                    _db.qa_votes.Add(v);
                                    _db.SaveChanges();
                                }
                            }
                        }
                        dbContextTransaction.Commit();
                    }
                }

                d.status = true;
                d.message = "successfully answer voted inserted";
                d.data = a.vote_count;
                return d;
            }
        }

        public Data QuestionVote(long qid, int value, long user_id)
        {
            Data d = new Data();
            using (var dbContextTransaction = _db.Database.BeginTransaction())
            {
                // for updating QA_Question table
                qa_questions q = _db.qa_questions.Where(x => x.id == qid).FirstOrDefault();
                if (q.user_id != user_id)
                {
                    if (value == 1)
                    {
                        qa_votes qa = _db.qa_votes.Where(y => y.type == false && y.type_id == q.id && y.user_id == user_id && y.is_up_vote == true).FirstOrDefault();
                        if (qa == null)
                        {
                            q.vote_count = q.vote_count + 1;

                            _db.Entry(q).State = EntityState.Modified;
                            _db.SaveChanges();

                            qa_votes qa1 = _db.qa_votes.Where(y => y.type == false && y.type_id == q.id && y.user_id == user_id && y.is_up_vote == false).FirstOrDefault();
                            if (qa1 != null)
                            {
                                qa1.is_up_vote = true;
                                _db.Entry(qa1).State = EntityState.Modified;
                                _db.SaveChanges();
                            }
                            else
                            {
                                //for inserting into QA_votes table
                                qa_votes v = new qa_votes();
                                v.created_at = DateTime.Now;
                                v.is_up_vote = value == 1 ? true : false;
                                v.type = false; // 0 for question and 1 for Answer
                                v.type_id = qid;
                                v.user_id = Convert.ToInt32(user_id);
                                _db.qa_votes.Add(v);
                                _db.SaveChanges();
                            }
                        }

                    }
                    else if (value == 0)
                    {
                        qa_votes qa = _db.qa_votes.Where(y => y.type == false && y.type_id == q.id && y.user_id == user_id && y.is_up_vote == false).FirstOrDefault();
                        if (qa == null)
                        {
                            q.vote_count = q.vote_count - 1;
                            if (q.vote_count < 0)
                            {
                                q.vote_count = 0;
                            }
                            _db.Entry(q).State = EntityState.Modified;
                            _db.SaveChanges();

                            qa_votes qa1 = _db.qa_votes.Where(y => y.type == false && y.type_id == q.id && y.user_id == user_id && y.is_up_vote == true).FirstOrDefault();
                            if (qa1 != null)
                            {
                                qa1.is_up_vote = false;
                                _db.Entry(qa1).State = EntityState.Modified;
                                _db.SaveChanges();
                            }
                            else
                            {
                                //for inserting into QA_votes table
                                qa_votes v = new qa_votes();
                                v.created_at = DateTime.Now;
                                v.is_up_vote = value == 1 ? true : false;
                                v.type = false; // 0 for question and 1 for Answer
                                v.type_id = qid;
                                v.user_id = Convert.ToInt32(user_id);
                                _db.qa_votes.Add(v);
                                _db.SaveChanges();
                            }
                        }
                    }
                }

                dbContextTransaction.Commit();

                d.status = true;
                d.message = "successfully question voted inserted";
                d.data = q.vote_count;
                return d;

            }
        }

        public Data CreateNewAnswer(long user_id, string title, string description, long question_id, HttpContext httpContext)
        {
            Data d = new Data();
            using (var dbContextTransaction = _db.Database.BeginTransaction())
            {
                qa_answers ans = new qa_answers();
                ans.title = title;
                ans.description = description;
                ans.question_id = question_id;
                ans.created_at = DateTime.Now;
                ans.user_id = user_id;
                ans.vote_count = 0;

                user _u = _db.users.Find(ans.user_id.Value);
                Generic g = new Generic();
                if (_u.role_id == (int)Role.Consultants)
                {
                    ans.approved_at = DateTime.Now;
                    ans.verified_at = DateTime.Now;
                    ans.approved_by = ans.user_id;
                    ans.verified_by = ans.user_id;
                    ans.is_approved = true;

                    qa_questions questionUser = _db.qa_questions.Find(ans.question_id);
                    g.sendNotificationOnChats("إجابة", "تم نشر إجابتك!", questionUser.user.phone, "answer_approved", ans.question_id.ToString());

                    g.InsertNotificationData(_u.id, "تم نشر إجابتك!", (Int16)AppConstants.Type.Answer, ans.id, "");

                    // for question user send notification when his or her question answered by another user.
                    qa_questions _Questions = _db.qa_questions.Where(x => x.id == ans.question_id).FirstOrDefault();
                    if (_Questions != null)
                    {
                        g.sendNotificationOnChats("إجابة", "تمت الإجابة على سؤالك!", _Questions.user.phone, "answer_approved", ans.question_id.ToString());

                        g.InsertNotificationData(_Questions.user_id.Value, "تمت الإجابة على سؤالك", (Int16)AppConstants.Type.Answer, ans.question_id.Value, "");
                    }
                }

                _db.qa_answers.Add(ans);
                _db.SaveChanges();

                //var httpContext = HttpContext.Current;

                // Check for any uploaded file  
                if (httpContext.Request.Files.Count > 0)
                {
                    //Loop through uploaded files  
                    for (int i = 0; i < httpContext.Request.Files.Count; i++)
                    {
                        HttpPostedFile httpPostedFile = httpContext.Request.Files[i];
                        if (httpPostedFile != null)
                        {
                            Guid guid = Guid.NewGuid();
                            var img = guid.ToString() + Path.GetExtension(httpPostedFile.FileName);
                            var path = Path.Combine(HostingEnvironment.MapPath("~/Media/QA/Answer/"), img);
                            // Save the uploaded file  
                            httpPostedFile.SaveAs(path);

                            medium m = new medium();
                            m.title = ans.title;
                            m.description = ans.description;
                            m.file_name = img;
                            m.file_type = CheckFileFormat(httpPostedFile.ContentType);
                            m.is_internal_file = true;
                            m.is_active = true;
                            m.created_at = DateTime.Now;
                            m.created_by = ans.user_id;
                            _db.media.Add(m);
                            _db.SaveChanges();

                            // for inserting into attachments table
                            qa_attachments attach = new qa_attachments();
                            attach.media_id = m.id;
                            attach.type = true; //0 for question and 1 for Answer
                            attach.type_id = ans.id;
                            _db.qa_attachments.Add(attach);
                            _db.SaveChanges();
                        }
                    }
                }

                dbContextTransaction.Commit();
                
                d.status = true;
                d.message = "successfully answer inserted";
                d.data = null;
                return d;
            }
        }

        public Data CreateNewQuestion(long user_id, string title, string description, long category_id, string keywords, HttpContext httpContext)
        {
            Data d = new Data();
            using (var dbContextTransaction = _db.Database.BeginTransaction())
            {

                qa_questions questions = new qa_questions();
                questions.title = title;
                questions.description = description;
                questions.qa_category_id = category_id;
                questions.keywords = keywords;
                questions.user_id = user_id;
                questions.created_at = DateTime.Now;
                questions.vote_count = 0;

                user _u = _db.users.Find(questions.user_id.Value);
                if (_u.role_id == (int)Role.Consultants)
                {
                    questions.approved_at = DateTime.Now;
                    questions.verified_at = DateTime.Now;
                    questions.approved_by = questions.user_id;
                    questions.verified_by = questions.user_id;
                    questions.is_approved = true;
                    questions.is_verified = true;
                    _db.qa_questions.Add(questions);
                    _db.SaveChanges();
                    
                    Generic g = new Generic();
                    //g.sendNotificationOnChats("إجابة", "تمت الموافقة على سؤالك!", _u.phone, "question_approved", questions.id.ToString());

                    //g.InsertNotificationData(_u.id, "تمت الموافقة على سؤالك", (Int16)AppConstants.Type.Question, questions.id, "");
                    g.sendNotificationOnChats("سؤال", "تمت الموافقة على سؤالك!", _u.phone, "question_approved", questions.id.ToString());
                    g.InsertNotificationData(_u.id, "تمت الموافقة على سؤالك", (Int16)AppConstants.Type.Question, questions.id, "");

                }
                else
                {
                    _db.qa_questions.Add(questions);
                    _db.SaveChanges();
                }

                // Check for any uploaded file  
                if (httpContext.Request.Files.Count > 0)
                {
                    //Loop through uploaded files  
                    for (int i = 0; i < httpContext.Request.Files.Count; i++)
                    {
                        HttpPostedFile httpPostedFile = httpContext.Request.Files[i];
                        if (httpPostedFile != null)
                        {
                            Guid guid = Guid.NewGuid();
                            var img = guid.ToString() + Path.GetExtension(httpPostedFile.FileName);
                            var path = Path.Combine(HostingEnvironment.MapPath("~/Media/QA/Question/"), img);
                            // Save the uploaded file  
                            httpPostedFile.SaveAs(path);

                            medium m = new medium();
                            m.title = questions.title;
                            m.description = questions.description;
                            m.file_name = img;
                            m.file_type = CheckFileFormat(httpPostedFile.ContentType);
                            m.is_internal_file = true;
                            m.keywords = questions.keywords;
                            m.related_to = CategoryName(Convert.ToInt32(HttpContext.Current.Request.Form["category_id"].ToString()));
                            m.is_active = true;
                            m.created_at = DateTime.Now;
                            m.created_by = questions.user_id;
                            _db.media.Add(m);
                            _db.SaveChanges();

                            // for inserting into attachments table
                            qa_attachments attach = new qa_attachments();
                            attach.media_id = m.id;
                            attach.type = false; //0 for question and 1 for Answer
                            attach.type_id = questions.id;
                            _db.qa_attachments.Add(attach);
                            _db.SaveChanges();
                        }
                    }
                }

                dbContextTransaction.Commit();
                
                d.status = true;
                d.message = "successfully question inserted";
                d.data = null;
                return d;
            }
        }

        public Data GetAllQuestion(long uid, int? Page_No = 0, string Search_Data = null, string Filter_Value = null, Boolean sort_value = false, int page_size = 50)
        {
            // if sort_Value true then send the list id descending order sort by vote_count
            // if sort_value false then send the list id ascending order
            // Filter_Value = CategoryID
            Data d = new Data();

            int pagesize = page_size;

            int No_Of_Page = (Page_No ?? 1);
            if (Page_No == 0 || Page_No == null) { Page_No = 1; }
            if (Search_Data != null)
            {
                Page_No = 1;
            }
          
            var questions = _db.qa_questions.Where(x => x.is_approved == true || x.user_id == uid).Select(p => new
            {
                p.id,
                p.title,
                p.qa_category_id,
                date = p.created_at,
                p.user_id,
                p.keywords,
                p.vote_count,
                p.is_verified,
                p.is_approved,
                created_by = p.user.name,
                Avatar = p.user.avatar,
                verified_by = p.user2.name,
                approved_by = p.user1.name,
                attachments = _db.qa_attachments.Where(z => z.type_id == p.id && z.type == false).Select(a => new
                {
                    id = a.id,
                    media_id = a.media_id,
                    type_id = a.type_id,
                    type = a.type,
                    url = _db.media.Where(x => x.id == a.media_id).FirstOrDefault() != null ? "/Media/QA/Question/" + _db.media.Where(x => x.id == a.media_id).FirstOrDefault().file_name : ""
                })

            }).OrderByDescending(x => x.id).ToList();

            if (!String.IsNullOrEmpty(Search_Data))
            {
                questions = questions.Where(x => x.title.Contains(Search_Data) || x.keywords.Contains(Search_Data)).ToList();
            }

            if (Filter_Value != null && Filter_Value != "" && Filter_Value != "0")
            {
                int category_id = Convert.ToInt32(Filter_Value);
                questions = questions.Where(x => x.qa_category_id == category_id).ToList();
            }

            if (sort_value)
            {
                questions = questions.OrderByDescending(x => x.vote_count).ToList();
            }

            int TotalPages = (int)Math.Ceiling(questions.Count() / (double)pagesize);

            int question_count = questions.Count();

            questions = questions.Skip((No_Of_Page - 1) * pagesize).Take(pagesize).ToList();//ToPagedList(No_Of_Page, pagesize);

            // if CurrentPage is greater than 1 means it has previousPage  
            var previous_Page = Page_No > 1 ? true : false;
            // if TotalPages is greater than CurrentPage means it has nextPage  
            var next_Page = Page_No < TotalPages ? true : false;

            d.status = true;
            d.message = "successfully fetch the records";
            d.data = questions;
            d.page_info = new { total_count = question_count, current_page = Page_No, total_page = TotalPages, next_Page, previous_Page };
            return d;
        }

        public Data GetAllQuestion()
        {
            Data d = new Data();

            var questions = _db.qa_questions.Select(p => new
            {
                p.id,
                p.title,
                p.qa_category_id,
                date = p.created_at,
                p.user_id,
                p.keywords,
                p.vote_count,
                p.is_verified,
                p.is_approved,
                created_by = p.user.name,
                Avatar = p.user.avatar,
                verified_by = p.user2.name,
                approved_by = p.user1.name,
                attachments = _db.qa_attachments.Where(z => z.type_id == p.id && z.type == false).Select(a => new
                {
                    id = a.id,
                    media_id = a.media_id,
                    type_id = a.type_id,
                    type = a.type,
                    url = _db.media.Where(x => x.id == a.media_id).FirstOrDefault() != null ? "/Media/QA/Question/" + _db.media.Where(x => x.id == a.media_id).FirstOrDefault().file_name : ""
                })

            }).ToList();

            d.status = true;
            d.message = "successfully fetch the records";
            d.data = questions;
            return d;
        }

        public Data Get_QA_Category()
        {
            Data d = new Data();
            var category = _db.qa_category.ToList();
            d.status = true;
            d.message = "successfully fetch the records";
            d.data = category;
            return d;
        }

    }
}