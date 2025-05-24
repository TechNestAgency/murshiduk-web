using MurshadikCP.Controllers;
using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public class ArticleRepository : GenericRepository<article>, IArticleRepository
    {
        public article GetArticleByID(long id)
        {
            return table.Where(u => u.id == id).SingleOrDefault();
        }

        public Data GetAllAgricultureCalender()
        {
            Data d = new Data();
            d.data = _db.Agriculture_Calender.Where(x => x.active == true).ToList();
            d.message = "successfully fetch records";
            d.status = true;
            return d;
        }

        public object GetCategoryDetails(long category)
        {
            var allNews = _db.articles.Include("category")
                                .Where(art => art.category.id == category)
                                .Where(art => art.active == true)
                                .Select(art => new Article()
                                {
                                    id = art.id,
                                    title_ar = art.title,
                                    desc_ar = art.short_description,
                                    image = art.image,
                                    created_at = art.created_at
                                })
                                .ToList();


            return allNews;
        }

        public Data SearchArticles(int? Page_No = 0, string Search_Data = null, int page_size = 20, int catid = 0, int rno = 1)
        {
            Data d = new Data();

            List<article> articles = _db.articles.ToList();

            int pagesize = page_size;

            int No_Of_Page = (Page_No ?? 1);
            if (Page_No == 0 || Page_No == null) { Page_No = 1; }

            if (catid > 0)
            {
                articles = articles.Where(x => x.category_id == catid).ToList();
            }

            if (!String.IsNullOrEmpty(Search_Data))
            {
                articles = articles.Where(x => x.title.Contains(Search_Data) || x.short_description.Contains(Search_Data)).ToList();
            }

            int TotalPages = (int)Math.Ceiling(articles.Count() / (double)pagesize);

            int articles_count = articles.Count();

            articles = articles.Skip((No_Of_Page - 1) * pagesize).Take(pagesize).ToList();

            // if CurrentPage is greater than 1 means it has previousPage  
            var previous_Page = Page_No > 1 ? true : false;
            // if TotalPages is greater than CurrentPage means it has nextPage  
            var next_Page = Page_No < TotalPages ? true : false;

            d.status = true;
            d.message = "successfully fetch the records";
            d.data = articles.Select(art => new { id = art.id, title_ar = art.title, category_name = art.category.name, category_id = art.category_id, Image = art.image }).ToList();
            d.page_info = new { total_count = articles_count, current_page = Page_No, total_page = TotalPages, next_Page, previous_Page, catid = catid, rno = rno };
            return d;
        }

        public List<chatbotDTO> ChatBot(string val)
        {
            List<chatbotDTO> chatbot = new List<chatbotDTO>();
            List<article> art = _db.articles.Where(x => x.description.Contains(val) || x.title.Contains(val) || x.keywords.Contains(val)).Take(5).ToList();
            if (art != null && art.Count() == 0)
            {
                List<qa_questions> que = _db.qa_questions.Where(x => x.is_approved == true && x.is_verified == true && (x.keywords.Contains(val) || x.description.Contains(val) || x.title.Contains(val))).Take(5).ToList();
                if (que != null && que.Count() == 0)
                {
                    List<user> u = _db.users.Where(x => x.is_approved == true && x.role_id == 6).Take(5).ToList();
                    List<long> u_id = new List<long>();
                    foreach (var item in u)
                    {
                        skill_user su = _db.skill_user.Where(x => x.is_approved == true && x.user_id == item.id && x.skill.name.Contains(val)).FirstOrDefault();
                        if (su != null)
                        {
                            u_id.Add(item.id);
                        }
                    }

                    if (u_id.Count() > 0)
                    {
                        for (int i = 0; i < u_id.Count(); i++)
                        {
                            chatbotDTO cbd = new chatbotDTO();
                            cbd.id = u_id[i];
                            user consultant = _db.users.Find(u_id[i]);

                            cbd.image = consultant.avatar;
                            cbd.description = consultant.name + " " + consultant.last_name;
                            cbd.type = 3;
                            chatbot.Add(cbd);
                        }
                        return chatbot;
                    }
                }
                else
                {
                    for (int i = 0; i < que.Count(); i++)
                    {
                        chatbotDTO cbd = new chatbotDTO();
                        cbd.id = que[i].id;
                        cbd.description = que[i].description;
                        long? media_id = _db.qa_attachments.Where(x => x.type_id == que[i].id && x.type == false).FirstOrDefault().media_id;
                        if (media_id != null)
                        {
                            cbd.image = _db.media.Find(media_id).file_name;
                        }
                        else
                        {
                            cbd.image = null;
                        }
                        cbd.type = 2;
                        chatbot.Add(cbd);
                    }
                    return chatbot;
                }
            }
            else
            {
                for (int i = 0; i < art.Count(); i++)
                {
                    chatbotDTO cbd = new chatbotDTO();
                    cbd.id = art[i].id;
                    cbd.description = art[i].title;
                    cbd.image = art[i].image;
                    cbd.type = 1;
                    chatbot.Add(cbd);
                }
                return chatbot;
            }
            return chatbot;
        }
    }
}