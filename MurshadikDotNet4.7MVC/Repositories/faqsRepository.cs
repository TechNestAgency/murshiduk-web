using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public class faqsRepository : GenericRepository<faq>, IfaqsRepository
    {
        public faq GetFaqByID(long id)
        {
            return table.Where(u => u.id == id).SingleOrDefault();
        }

        public IQueryable<faq> GetFaqsBySearch(int? Page_No, string Search_Data, string Filter_Value)
        {
            int No_Of_Page = (Page_No ?? 1);

            if (Search_Data != null && Search_Data != "")
            {
                Page_No = 1;
            }
            else
            {
                Search_Data = Filter_Value;
            }

            var faqs = from f in _db.faqs
                         select f;
            if (!String.IsNullOrEmpty(Search_Data))
            {
                faqs = faqs.Where(s => s.question.Contains(Search_Data)
                                       || s.answer.Contains(Search_Data));
            }
            
            return faqs.OrderByDescending(x => x.id);
        }

        public void Addfaqs(faq f)
        {
            Insert(f);
        }

        public int FaqsSort()
        {
            return _db.faqs.Count();
        }

        public void Updatefaqs(faq f)
        {
            Update(f);
        }

        public object GetAllFaqs()
        {
            return _db.faqs.ToList();
        }

    }
}