using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace MurshadikCP.Repositories
{
    public class ChatMessageRepository : GenericRepository<chatmessage>, IChatMessageRepository
    {
        public List<chatmessage> GetMessageByReceiverId(string receiver_id)
        {
            return _db.chatmessages.Where(x => x.chat_ids.Contains(receiver_id)).OrderByDescending(x => x.id).ToList();
        }

        public chatmessage GetLastChatMessageByUserId(string receiver_id)
        {
            return _db.chatmessages.Where(x => x.chat_ids.Contains(receiver_id)).LastOrDefault();
        }

        public object InsertChatMessage(long user_id, string receiver_id, int type_id, string message, int status)
        {
            int[] ids = new int[2];
            ids[0] = Convert.ToInt16(user_id);
            ids[1] = Convert.ToInt16(receiver_id);
            Array.Sort(ids);
            string chatIDs = ids[0] + "," + ids[1];

            string phoneno = "";
            long receiver_ID = Convert.ToInt32(receiver_id);
            phoneno = _db.users.Where(x => x.id == receiver_ID).FirstOrDefault().phone;
            _db.Configuration.ProxyCreationEnabled = false;
            chatmessage cm = new chatmessage();
            cm.message_type = type_id;
            if (type_id != 1 && type_id != 6)
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
                    cm.message = "/Media/Chat/" + file;
                }
            }
            else
            {
                cm.message = message;
            }

            cm.chat_ids = chatIDs;
            cm.status = status;
            cm.message_from = Convert.ToInt16(user_id);
            cm.created_at = DateTime.Now;
            Insert(cm);
            Save();

            // for check chat table
            chat chat = new chat();
            chat = _db.chats.Where(x => x.chat_ids == cm.chat_ids).FirstOrDefault();
            if (chat == null)
            {
                chat = new chat();
                chat.chat_ids = cm.chat_ids;
                chat.created_at = DateTime.Now;
                if (cm.message_type != 1)
                {
                    chat.last_message = null;
                }
                else
                {
                    chat.last_message = cm.message;
                }
                chat.status = cm.status;
                chat.message_type = cm.message_type;
                _db.chats.Add(chat);
                _db.SaveChanges();
            }
            else
            {
                chat.last_message = cm.message;
                chat.created_at = DateTime.Now;
                chat.message_type = cm.message_type;
                _db.Entry(chat).State = EntityState.Modified;
                _db.SaveChanges();
            }

            Generic g = new Generic();

            object mes = new { cm.id, cm.message, cm.message_from, cm.message_type, cm.status, cm.chat_ids, cm.created_at };

            return mes;
        }
    }
}