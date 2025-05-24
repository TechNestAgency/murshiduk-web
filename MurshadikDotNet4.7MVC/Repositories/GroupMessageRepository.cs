using MurshadikCP.Controllers;
using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace MurshadikCP.Repositories
{
    public class GroupMessageRepository : GenericRepository<group_messages>, IGroupMessageRepository
    {
        public List<group_messages> GetGroupMessages(int id = 0, string dt = null)
        {
            List<group_messages> gm = new List<group_messages>();
            if (id > 0)
            {
                gm = _db.group_messages.Where(x => x.id > id).ToList();
            }
            if (dt != null && dt != "")
            {
                DateTime date = Convert.ToDateTime(dt);
                gm = _db.group_messages.Where(x => x.created_at > date).OrderByDescending(x => x.id).Take(20).ToList();
            }
            return gm;
        }

        public Data GetGroupMessagesByPagination(int group_id, int? Page_No = 0, int id = 0, int page_size = 20)
        {
            Data d = new Data();

            var skip = Page_No == 1 ? 0 : page_size * (Page_No - 1);
            var query = _db.group_messages.Where(group => group.group_id == group_id);

            var results = query.OrderByDescending(message => message.id)
                   .Select(m => new
                   {
                       Messages = m,
                       TotalCount = query.Count()
                   })
                   .Skip(skip.Value).Take(page_size)
                   .ToList();

            var totalCount = results.First().TotalCount;
            var pages = Math.Ceiling(totalCount / (double)page_size);
            var messages = results.Select(r => r.Messages).ToList();

            // if CurrentPage is greater than 1 means it has previousPage  
            var previous_Page = Page_No > 1 ? true : false;
            // if TotalPages is greater than CurrentPage means it has nextPage  
            var next_Page = Page_No < pages ? true : false;

            d.status = true;
            d.message = "successfully fetch the records";
            d.data = results;
            d.page_info = new { total_count = totalCount, current_page = Page_No, total_page = pages, next_Page, previous_Page };
            return d;
        }

        public Data GetGroupMessageBy(int group_id, int? Page_No = 0, int page_size = 20, bool withMembers = false)
        {
            Data d = new Data();
            List<group_messages> gm = new List<group_messages>();

            int pagesize = page_size;

            int No_Of_Page = (Page_No ?? 1);
            if (Page_No == 0 || Page_No == null) { Page_No = 1; }

            if (group_id > 0)
            {
                gm = _db.group_messages.Where(x => x.group_id == group_id).OrderByDescending(x => x.id).Skip((No_Of_Page - 1) * pagesize).Take(pagesize).ToList();
            }

            int gm_count = _db.group_messages.Where(x => x.group_id == group_id).Count();
            int TotalPages = (int)Math.Ceiling(gm_count / (double)pagesize);

            // if CurrentPage is greater than 1 means it has previousPage  
            var previous_Page = Page_No > 1 ? true : false;
            // if TotalPages is greater than CurrentPage means it has nextPage  
            var next_Page = Page_No < TotalPages ? true : false;

            if (withMembers)
            {
                List<user> users = _db.users.Where(x => x.role_id == (int)Role.Consultants && x.is_approved == true && x.region_id == group_id).ToList();
                d.status = true;
                d.message = "successfully fetch the records";
                d.data = new { group_message = gm, consultant = users.Select(p => new { p.id, p.name, p.phone, p.avatar, p.chatId, p.is_group_admin, p.can_post, p.city_code }) };
                d.page_info = new { total_count = gm_count, current_page = Page_No, total_page = TotalPages, next_Page, previous_Page };
                return d;
            }

            d.status = true;
            d.message = "successfully fetch the records";
            d.data = new { group_message = gm, consultant = new string[0] };
            d.page_info = new { total_count = gm_count, current_page = Page_No, total_page = TotalPages, next_Page, previous_Page };
            return d;
        }

        public Data GetGroupMessageBy(int group_id, int? Page_No = 0, int page_size = 20)
        {
            GroupMessageRepository gmr = new GroupMessageRepository();
            Data d = new Data();
            List<group_messages> gm = new List<group_messages>();

            int pagesize = page_size;

            int No_Of_Page = (Page_No ?? 1);
            if (Page_No == 0 || Page_No == null) { Page_No = 1; }

            if (group_id > 0)
            {
                gm = _db.group_messages.Where(x => x.group_id == group_id).OrderByDescending(x => x.id).Skip((No_Of_Page - 1) * pagesize).Take(pagesize).ToList();
            }

            int gm_count = _db.group_messages.Where(x => x.group_id == group_id).Count();
            int TotalPages = (int)Math.Ceiling(gm_count / (double)pagesize);

            // if CurrentPage is greater than 1 means it has previousPage  
            var previous_Page = Page_No > 1 ? true : false;
            // if TotalPages is greater than CurrentPage means it has nextPage  
            var next_Page = Page_No < TotalPages ? true : false;

            d.status = true;
            d.message = "successfully fetch the records";
            d.data = gm;
            d.page_info = new { total_count = gm_count, current_page = Page_No, total_page = TotalPages, next_Page, previous_Page };
            return d;
        }

        public group_messages InsertGroupMessage(int type_id, string message, long user_id, int group_id, string fullname)
        {
            group_messages gm = new group_messages();
            gm.message_type = type_id;
            if (type_id != 1)
            {
                var httpContext = HttpContext.Current;
                HttpPostedFile httpPostedFile = httpContext.Request.Files[0];
                if (httpPostedFile != null)
                {
                    Guid guid = Guid.NewGuid();
                    var file = guid.ToString() + Path.GetExtension(httpPostedFile.FileName);
                    var path = Path.Combine(HostingEnvironment.MapPath("~/Media/Chat/"), file);
                    // Save the uploaded file  
                    httpPostedFile.SaveAs(path);
                    gm.message = "/Media/Chat/" + file;
                }
            }
            else
            {
                gm.message = message;
            }

            gm.message_from = Convert.ToInt16(user_id);
            gm.group_id = group_id;
            gm.created_at = DateTime.Now;
            Insert(gm);
            Save();

            string group_name = _db.regions.Where(x => x.id == gm.group_id && x.active == true).FirstOrDefault().name_ar;

            Generic g = new Generic();
            g.sendNotificationOnChats(fullname, gm.message.Length > 20 ? gm.message.Substring(0, 20) : gm.message, group_name, "group_message", gm.group_id.ToString());

            return gm;
        }
    }
}