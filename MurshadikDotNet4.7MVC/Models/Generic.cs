using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MurshadikCP.Models
{
    public class Generic
    {
        private mlaraEntities db = new mlaraEntities();

        public async Task<Object> SendGeneralNotification(string Title, string Body)
        {

            var messaging = FirebaseMessaging.DefaultInstance;

            var alert = new Message()
            {

                Notification = new Notification
                {
                    Title = Title,
                    Body = Body

                },
                Android = new AndroidConfig
                {
                    Notification = new AndroidNotification
                    {
                        Sound = "default"
                    }
                },
                Apns = new ApnsConfig
                {
                    Aps = new Aps
                    {
                        Sound = "default"
                    }
                }
            };

            var response = await messaging.SendAsync(alert); //await messaging.SendAllAsync(m);
            return response;

        }

        public async Task<Object> SendGeneralNotification(string Title, string Body, string message_type, string message_user, string value = "off")
        {

            var messaging = FirebaseMessaging.DefaultInstance;

            //List<Message> m = new List<Message>();

            var alert = new Message()
            {
                
                Notification = new Notification
                {
                    Title = Title,// "My Message",
                    Body = Body//"Your account has been approved"
                    
                },
                Android = new AndroidConfig 
                {
                    Notification = new AndroidNotification 
                    {
                        Sound = "default"
                    }
                },
                Apns = new ApnsConfig 
                {
                    Aps = new Aps 
                    {
                        Sound = "default"
                    }
                },

                Data = new Dictionary<string, string>()
                {
                    ["murshadik.message_type"] = message_type,//"account_approved",
                    ["murshadik.message_user"] = message_user,//"user phone number i.e. 966591794406"
                    ["murshadik.message_value"] = value
                }
            };

            //m.Add(alert);

            var response = await messaging.SendAsync(alert); //await messaging.SendAllAsync(m);
            return response;

        }
        public async Task<Object> SendTopicNotification(string Title, string Body, string Topic, string message_type, string message_user)
        {
            var messaging = FirebaseMessaging.DefaultInstance;

            //List<Message> m = new List<Message>();

            var alert = new Message()
            {
                Notification = new Notification
                {
                    Title = Title,// "My Message",
                    Body = Body//"Your account has been approved"
                },
                Topic = Topic,//"User Phone Number i.e. 966591794406",
                
                Android = new AndroidConfig
                {
                    Notification = new AndroidNotification
                    {
                        Sound = "default"
                    }
                },
                Apns = new ApnsConfig
                {
                    Aps = new Aps
                    {
                        Sound = "default"
                    }
                },
                Data = new Dictionary<string, string>()
                {
                    ["murshadik.message_type"] = message_type,//"account_approved",
                    ["murshadik.message_user"] = message_user//"user phone number i.e. 966591794406"
                }
            };

            //m.Add(alert);

            var response = await messaging.SendAsync(alert); //await messaging.SendAllAsync(m);
            return response;

        }

        public async Task<Object> sendGeneralNotification(string sender_name, string message, string[] topics)
        {
            var messaging = FirebaseMessaging.DefaultInstance;

            List<Message> m = new List<Message>();


            foreach (string t in topics) {
                Message mes = new Message();
                mes.Notification = new Notification { Title = sender_name, Body = message };
                mes.Topic = t;                

                mes.Android = new AndroidConfig
                {
                    Notification = new AndroidNotification
                    {
                        Sound = "default"
                    }
                };
                mes.Apns = new ApnsConfig
                {
                    Aps = new Aps
                    {
                        Sound = "default"
                    }
                };

                m.Add(mes);
            }

            var response = await messaging.SendEachAsync(m);
            
            return response;
        }

        public async Task<Object> sendNotificationOnChats(string sender_name, string message, string[] receiver_id, string message_type, string type_id)
        {
            var messaging = FirebaseMessaging.DefaultInstance;

            List<Message> m = new List<Message>();

            for (int i = 0; i < receiver_id.Count(); i++)
            {
                Message mes = new Message();
                mes.Notification = new Notification { Title = sender_name, Body = message };
                mes.Topic = receiver_id[i].ToString();

                mes.Android = new AndroidConfig
                {
                    Notification = new AndroidNotification
                    {
                        Sound = "default"
                    }
                };
                mes.Apns = new ApnsConfig
                {
                    Aps = new Aps
                    {
                        Sound = "default"
                    }
                };
                mes.Data = new Dictionary<string, string>()
                {
                    ["murshadik.message_type"] = message_type,//lab_report_available, //market_price_change,
                    ["murshadik.message_type_id"] = type_id // market_id , lab_id, Qna_id 
                };
                
                m.Add(mes);
            }

            var response = await messaging.SendEachAsync(m);
            return response;
        }

        public async Task<Object> sendMultipleNotification(List<NotificationDTO> message)
        {
            try
            {
                var messaging = FirebaseMessaging.DefaultInstance;
                if (messaging == null)
                {
                    
                    var pathToKey = Path.Combine(HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["firebaseJson"]));
                    FirebaseApp.Create(new AppOptions
                    {
                        Credential = GoogleCredential.FromFile(pathToKey)
                    });
                    messaging = FirebaseMessaging.DefaultInstance;
                }

                List<Message> m = new List<Message>();

                foreach (var item in message)
                {
                    var alert = new Message()
                    {
                        Notification = new Notification
                        {
                            Title = item.title,
                            Body = item.body
                        },
                        
                        Android = new AndroidConfig
                        {
                            Notification = new AndroidNotification
                            {
                                Sound = "default"
                            }
                        },
                        Apns = new ApnsConfig
                        {
                            Aps = new Aps
                            {
                                Sound = "default"
                            }
                        },
                        Topic = item.topic
                    };

                    m.Add(alert);
                }

                var response = await messaging.SendEachAsync(m);
                WriteToFile("Response from firebase : " + response.ToString());
                return response;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }

        public async Task<Object> sendNotificationOnChats(string sender_name, string message, string receiver_id, string message_type, string type_id)
        {
            var messaging = FirebaseMessaging.DefaultInstance;

            List<Message> m = new List<Message>();

            var alert = new Message()
            {
                Notification = new Notification
                {
                    Title = sender_name,
                    Body = message
                },
                Topic = receiver_id
                ,
                Android = new AndroidConfig
                {
                    Notification = new AndroidNotification
                    {
                        Sound = "default"
                    }
                },
                Apns = new ApnsConfig
                {
                    Aps = new Aps
                    {
                        Sound = "default"
                    }
                },
                Data = new Dictionary<string, string>()
                {
                    ["murshadik.message_type"] = message_type,//lab_report_available, //market_price_change,
                    ["murshadik.message_type_id"] = type_id // market_id , lab_id, Qna_id 
                }

        };

            m.Add(alert);

            var response = await messaging.SendEachAsync(m);
            return response;
        }

        public async Task<Object> sendNotificationOnChats(string sender_name, string message, string receiver_id, string message_type, string type_id, string link)
        {
            var messaging = FirebaseMessaging.DefaultInstance;

            List<Message> m = new List<Message>();

            var alert = new Message()
            {
                Notification = new Notification
                {
                    Title = sender_name,
                    Body = message
                },
                Topic = receiver_id
                ,
                Android = new AndroidConfig
                {
                    Notification = new AndroidNotification
                    {
                        Sound = "default"
                    }
                },
                Apns = new ApnsConfig
                {
                    Aps = new Aps
                    {
                        Sound = "default"
                    }
                },
                Data = new Dictionary<string, string>()
                {
                    ["murshadik.message_type"] = message_type,//lab_report_available, //market_price_change,
                    ["murshadik.message_type_id"] = type_id, // market_id , lab_id, Qna_id 
                    ["murshadik.message_link"] = link
                }

            };

            m.Add(alert);

            var response = await messaging.SendEachAsync(m);
            return response;
        }



        public async Task<Object> sendNotificationOnPriceUpdate(string title, string body, string topic)
        {
            var messaging = FirebaseMessaging.DefaultInstance;

            List<Message> m = new List<Message>();

            var alert = new Message()
            {
                Notification = new Notification
                {
                    Title = title,//"Product Price Updated",
                    Body = body//"Testing"
                },
                Topic = topic//"Developers"
                ,
                Android = new AndroidConfig
                {
                    Notification = new AndroidNotification
                    {
                        Sound = "default"
                    }
                },
                Apns = new ApnsConfig
                {
                    Aps = new Aps
                    {
                        Sound = "default"
                    }
                }
            };

            m.Add(alert);

            var response = await messaging.SendEachAsync(m);
            return response;
        }




        public void InsertNotificationData(long user_id, string text, int type, long type_id, string link)
        {
            notification n = new notification();
            n.user_id = user_id;
            n.text = text;
            n.type = type;
            n.type_id = type_id;
            n.link = link;
            n.created_at = DateTime.Now;
            db.notifications.Add(n);
            db.SaveChanges();
        }

        public string GetFullName(long id)
        {
            return db.users.Find(id).name;
        }

        
}
}