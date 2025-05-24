using MurshadikCP.Controllers;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public class ChatRepository : GenericRepository<chat>, IChatRepository
    {
        public object GetAllChatsByUserID(string user_id)
        {
            List<chat> chats = _db.chats.Where(x => x.chat_ids.Contains(user_id)).ToList();
            List<ChatDetails> cd_list = new List<ChatDetails>();
            if (chats.Count() > 0)
            {
                for (int i = 0; i < chats.Count(); i++)
                {
                    ChatDetails cd = new ChatDetails();
                    cd.last_message = chats[i].last_message;
                    cd.status = chats[i].status.Value;
                    cd.created_at = chats[i].created_at.Value;
                    cd.message_type = chats[i].message_type;
                    string[] ids = chats[i].chat_ids.Split(',');
                    long claim_id = Convert.ToInt16(user_id);
                    if (user_id != ids[0])
                    {
                        Int32 u_id = Convert.ToInt32(ids[0]);
                        user u = _db.users.Where(x => x.id == u_id).FirstOrDefault();
                        if (u != null)
                        {
                            cd.avatar = u.avatar;
                            cd.is_online = u.is_online;
                            cd.name = u.name + " " + u.last_name;
                            cd.phone = u.phone;
                            cd.rating = u.rating;
                            cd.active = u.active;
                            cd.chatId = u.chatId;
                            cd.id = u.id;
                            cd.role_id = u.role_id;
                            cd_list.Add(cd);
                        }
                    }
                    else
                    {
                        Int32 u_id = Convert.ToInt32(ids[1]);
                        user u = _db.users.Where(x => x.id == u_id).FirstOrDefault();
                        if (u != null)
                        {
                            cd.avatar = u.avatar;
                            cd.is_online = u.is_online;
                            cd.name = u.name + " " + u.last_name;
                            cd.phone = u.phone;
                            cd.rating = u.rating;
                            cd.active = u.active;
                            cd.chatId = u.chatId;
                            cd.id = u.id;
                            cd.role_id = u.role_id;
                            cd_list.Add(cd);
                        }
                    }

                }
            }
            return cd_list;
        }
    }
}