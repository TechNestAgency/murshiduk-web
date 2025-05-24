using MurshadikCP.Controllers;
using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public class UserRepository : GenericRepository<user>, IUserRepository
    {
        public user GetUserByID(string uID)
        {
            return table.Where(u => u.phone == uID).SingleOrDefault();
        }

        public user GetUserIncludeRoleByID(string phoneNo, string include)
        {
            return table.Include(include).Where(u => u.phone == phoneNo).SingleOrDefault();
        }

        public user GetUserByID(long Id)
        {
            return table.Find(Id);
        }

        public otp GetOtpFor(long userId)
        {
           return _db.otps.Where(x => x.user_id == userId).FirstOrDefault();
        }

        public UserInfo GetUserInfoByID(string uID)
        {
            long user_id = _db.users.Where(x => x.phone == uID).FirstOrDefault().id;
            Models.DB.UserAccessTypeIdentifier uati = _db.UserAccessTypeIdentifiers.Where(ua => ua.user_id == user_id).FirstOrDefault();
            long lab_id = 0; long market_id = 0; string multiplemarketids = null;
            if (uati != null)
            {
                if (uati.type == 1) { lab_id = Convert.ToInt32(uati.type_value); }
                else
                { 
                    market_id = Convert.ToInt32(uati.type_value.Split(',')[0]);
                    multiplemarketids = uati.type_value;
                }
            }

            var result = table.Include("Role").Where(u => u.phone == uID).FirstOrDefault();
            UserInfo userInfo = new UserInfo();
            userInfo.Id = result.id;
            userInfo.UserName = result.phone;
            userInfo.RoleId = result.role_id;
            userInfo.RoleName = result.role.name_ar;
            userInfo.labid = lab_id;
            userInfo.marketid = market_id;
            userInfo.multiplemarketids = multiplemarketids;
            userInfo.region_id = result.region_id;
            userInfo.roles_Permissions = _db.roles_permission.Include("Page").Where(r => r.role_id == result.role_id).ToList();
            return userInfo;
        }

        public user GetUserByChatId(string chatId)
        {
             return _db.users.Where(x => x.chatId == chatId).FirstOrDefault();
        }

        public string GetRegionNameByRegionId(long regionId)
        {
            return _db.regions.Where(x => x.id == regionId && x.active == true).FirstOrDefault().name_ar;
        }

        public object GetDashboardData()
        {
            long[] sectionCategoriesArticles = { 14, 33 };
            long[] sectionCategories = { 1, 30 };

            var articles = _db.categories.Include("Article")
                                        .Where(cat => sectionCategoriesArticles.Contains(cat.id) && cat.active == true)
                                        .Select(cat => new
                                        {
                                            id = cat.id,
                                            title = cat.name,
                                            banner = cat.banner,
                                            icon = cat.icon,
                                            articles = cat.articles.Where(x => x.active == true)
                                                .Select(art => new Article()
                                                {
                                                    id = art.id,
                                                    title_ar = art.title,
                                                    desc_ar = art.short_description,
                                                    image = art.image,
                                                    created_at = art.created_at
                                                })
                                                .Take(10)
                                                .ToList()
                                        })
                                        .ToList();

            var categories = _db.categories.Where(cat => cat.active == true && (sectionCategories
                                                        .Contains(cat.parent_category.Value) || sectionCategories.Contains(cat.id))
                                                )
                                                .Select(cat => new Category()
                                                {
                                                    id = cat.id,
                                                    name = cat.name,
                                                    description = cat.description,
                                                    parent_category = cat.parent_category,
                                                    icon = cat.icon,
                                                    bg_color = cat.bg_color,
                                                    text_color = cat.text_color,
                                                    banner = cat.banner,
                                                    active = cat.active,
                                                    created_at = cat.created_at,
                                                    updated_at = cat.updated_at,
                                                }
                                                )
                                                .ToList();

            List<StatsDTO> stats = _db.app_stats.Where(x => x.enabled).Take(3).Select(p => new StatsDTO()
            {
                title = p.stats,//p.stats.Length > 3 ? p.stats.Substring(0,2)+"K" : p.stats,
                total = p.id == 1 ? _db.msgs.Count() :
                p.id == 2 ? _db.users.Where(x => x.role_id == 5).Count() :
                p.id == 3 ? _db.users.Where(x => x.role_id == 7).Count() :
                p.id == 4 ? 10 :
                p.id == 5 ? _db.msgs.Count() :
                p.id == 6 ? _db.qa_questions.Count() :
                p.id == 7 ? _db.qa_answers.Count() :
                p.id == 8 ? _db.lab_reports.Count() :
                p.id == 9 ? _db.labs.Count() :
                p.id == 10 ? _db.articles.Count() :
                p.id == 11 ? _db.markets.Count() :
                p.id == 12 ? _db.products.Count() :
                597,
                icon = p.icon,
                desc = p.title,
                bgColor = p.bg_color
            }).ToList();

            var firstSection = new
            {
                ParentCat = categories.Where(cat => cat.id == sectionCategories.First()).Select(c => new
                {
                    id = c.id,
                    title = c.name,
                    banner = c.banner,
                    icon = c.icon
                }).First(),
                Categories = categories.Where(cat => cat.parent_category == sectionCategories.First()),
            };

            var secondSection = new
            {
                ParentCat = categories.Where(cat => cat.id == sectionCategories.Last()).Select(c => new
                {
                    id = c.id,
                    title = c.name,
                    banner = c.banner,
                    icon = c.icon
                }
                                            ).First(),
                Categories = categories.Where(cat => cat.parent_category == sectionCategories.Last()),
            };

            var dashboardData = new
            {
                liveTransmission = new LiveStream()
                {
                    type = "livestream",
                    title = "Live Stream",
                    enabled = "on",
                    banner = _db.app_settings.Where(x => x.ap_key == "AP_STREAM_BANNER").FirstOrDefault().ap_value,
                    url = _db.app_settings.Where(x => x.ap_key == "AP_STREAM_URL").FirstOrDefault().ap_value
                },
                sectionOne = new
                {
                    id = firstSection.ParentCat.id,
                    title = firstSection.ParentCat.title,
                    banner = firstSection.ParentCat.banner,
                    icon = firstSection.ParentCat.icon,
                    categories = firstSection.Categories
                },
                agCalendarCatID = 13,
                agGuideCatID = 12,
                sectionTwo = articles[0],
                sectionThree = articles[1],
                mediaLibraryCatID = 15,
                sectionFour = new
                {
                    id = secondSection.ParentCat.id,
                    title = secondSection.ParentCat.title,
                    banner = secondSection.ParentCat.banner,
                    icon = secondSection.ParentCat.icon,
                    categories = secondSection.Categories
                },
                contactUsSection = new
                {
                    AP_SYSTEM_EMAIL = _db.app_settings.Where(x => x.ap_key == "AP_SYSTEM_EMAIL").FirstOrDefault().ap_value,
                    AP_CUSTOMER_SUPPORT_WA_NUMBER = _db.app_settings.Where(x => x.ap_key == "AP_CUSTOMER_SUPPORT_WA_NUMBER").FirstOrDefault().ap_value,
                    AP_CUSTOMER_SUPPORT_NUMBER = _db.app_settings.Where(x => x.ap_key == "AP_CUSTOMER_SUPPORT_NUMBER").FirstOrDefault().ap_value,
                    AP_FACEBOOK_URL = _db.app_settings.Where(x => x.ap_key == "AP_FACEBOOK_URL").FirstOrDefault().ap_value,
                    AP_TWITTER_URL = _db.app_settings.Where(x => x.ap_key == "AP_TWITTER_URL").FirstOrDefault().ap_value,
                    AP_YOUTUBE_URL = _db.app_settings.Where(x => x.ap_key == "AP_YOUTUBE_URL").FirstOrDefault().ap_value,
                    AP_INSTA_URL = _db.app_settings.Where(x => x.ap_key == "AP_INSTA_URL").FirstOrDefault().ap_value,
                    AP_SNAP_URL = _db.app_settings.Where(x => x.ap_key == "AP_SNAP_URL").FirstOrDefault().ap_value
                },
                stats = stats
            };

            return dashboardData;
        }

        public void UpdateProfileNew()
        {
            
        }

        public object DashboardData()
        {
            var dashboard = new DashboardData()
            {
                firstSection = new Section()
                {
                    title = _db.categories.Where(x => x.id == 1).FirstOrDefault().description,
                    type = "category",
                    items = _db.categories.Where(x => x.parent_category == 1 && x.active == true)
                            .Select(p => new DashboardItems()
                            {
                                id = p.id,
                                name_ar = p.name,
                                desc_ar = p.description,
                                icon = p.icon,
                                bg_color = p.bg_color,
                                text_color = p.text_color,
                                banner_ar = p.banner
                            }).ToList()
                },
                secondSection = new Section_Article()
                {
                    title = "الأخبار",
                    type = "article",
                    item = _db.categories.Where(x => x.name == "الأخبار" && x.active == true).Select(p => new DashboardItems()
                    {
                        id = p.id,
                        name_ar = p.name,
                        desc_ar = p.description,
                        icon = p.icon,
                        bg_color = p.bg_color,
                        text_color = p.text_color,
                        banner_ar = p.banner
                    }).FirstOrDefault(),
                    items = _db.articles.Where(x => x.active == true).OrderByDescending(x => x.id).Take(5).Select(p => new Article()
                    {
                        id = p.id,
                        title_ar = p.title,
                        desc_ar = p.short_description,
                        image = p.image,
                        created_at = p.created_at
                    }).ToList()
                },
                thirdSection = new Section()
                {
                    title = "طلب إستشارة",
                    type = "consultation",
                    items = _db.categories.Where(x => x.name == "طلب إستشارة" && x.active == true).Select(p => new DashboardItems()
                    {
                        id = p.id,
                        name_ar = p.name,
                        desc_ar = p.description,
                        icon = p.icon,
                        bg_color = p.bg_color,
                        text_color = p.text_color,
                        banner_ar = p.banner
                    }).ToList()
                },
                fourthSection = new fourthSection()
                {
                    articles = new Section()
                    {
                        title = "الإرشاد الزراعي",
                        type = "articles",
                        items = _db.categories.Where(x => x.name == "الإرشاد الزراعي" && x.active == true).Select(p => new DashboardItems()
                        {
                            id = p.id,
                            name_ar = p.name,
                            desc_ar = p.description,
                            icon = p.icon,
                            bg_color = p.bg_color,
                            text_color = p.text_color,
                            banner_ar = p.banner
                        }).ToList()
                    },
                    Calendar = new Section()
                    {
                        title = "التقويم الزراعي",
                        type = "calendar",
                        items = _db.categories.Where(x => x.name == "التقويم الزراعي" && x.active == true).Select(p => new DashboardItems()
                        {
                            id = p.id,
                            name_ar = p.name,
                            desc_ar = p.description,
                            icon = p.icon,
                            bg_color = p.bg_color,
                            text_color = p.text_color,
                            banner_ar = p.banner
                        }).ToList()
                    }
                },
                fifthSection = new Section_Article()
                {
                    title = "الندوات و ورش العمل",
                    type = "article",
                    item = _db.categories.Where(x => x.name == "الندوات و ورش العمل" && x.active == true).Select(p => new DashboardItems()
                    {
                        id = p.id,
                        name_ar = p.name,
                        desc_ar = p.description,
                        icon = p.icon,
                        bg_color = p.bg_color,
                        text_color = p.text_color,
                        banner_ar = p.banner
                    }).FirstOrDefault(),
                    items = _db.articles.Where(x => x.category_id == 34 && x.active == true).OrderByDescending(x => x.id).Take(5).Select(p => new Article()
                    {
                        id = p.id,
                        title_ar = p.title,
                        desc_ar = p.short_description,
                        image = p.image,
                        created_at = p.created_at
                    }).ToList()
                },
                sixthSection = new Section_Article()
                {
                    title = "المنتجات الزراعية",
                    type = "article",
                    item = _db.categories.Where(x => x.name == "المنتجات الزراعية" && x.active == true).Select(p => new DashboardItems()
                    {
                        id = p.id,
                        name_ar = p.name,
                        desc_ar = p.description,
                        icon = p.icon,
                        bg_color = p.bg_color,
                        text_color = p.text_color,
                        banner_ar = p.banner
                    }).FirstOrDefault(),
                    items = _db.articles.Where(x => x.category_id == 33 && x.active == true).OrderByDescending(x => x.id).Take(5).Select(p => new Article()
                    {
                        id = p.id,
                        title_ar = p.title,
                        desc_ar = p.short_description,
                        image = p.image,
                        created_at = p.created_at
                    }).ToList()
                },
                seventhSection = new Section()
                {
                    title = "التجارب الناجحة",
                    type = "category",
                    items = _db.categories.Where(x => x.name == "التجارب الناجحة" && x.active == true).Select(p => new DashboardItems()
                    {
                        id = p.id,
                        name_ar = p.name,
                        desc_ar = p.description,
                        icon = p.icon,
                        bg_color = p.bg_color,
                        text_color = p.text_color,
                        banner_ar = p.banner
                    }).ToList()
                },
                eightSection = new Section_Category()
                {
                    title = "أسئلة وأجوبة إرشادية",
                    type = "category",
                    item = _db.categories.Where(x => x.name == "أسئلة وأجوبة إرشادية" && x.active == true).Select(p => new DashboardItems()
                    {
                        id = p.id,
                        name_ar = p.name,
                        desc_ar = p.description,
                        icon = p.icon,
                        bg_color = p.bg_color,
                        text_color = p.text_color,
                        banner_ar = p.banner
                    }).FirstOrDefault(),
                    items = _db.categories.Where(x => x.parent_category == 30 && x.active == true).Select(p => new DashboardItems()
                    {
                        id = p.id,
                        name_ar = p.name,
                        desc_ar = p.description,
                        icon = p.icon,
                        bg_color = p.bg_color,
                        text_color = p.text_color,
                        banner_ar = p.banner
                    }).ToList()
                },
                ninthSection = new Section()
                {
                    title = "المكتبة الرقمية",
                    type = "category",
                    items = _db.categories.Where(x => x.name == "المكتبة الرقمية" && x.active == true).Select(p => new DashboardItems()
                    {
                        id = p.id,
                        name_ar = p.name,
                        desc_ar = p.description,
                        icon = p.icon,
                        bg_color = p.bg_color,
                        text_color = p.text_color,
                        banner_ar = p.banner
                    }).ToList()
                },
                tenthSection = new LiveStream()
                {
                    type = "livestream",
                    title = "Live Stream",
                    enabled = "on",
                    banner = _db.app_settings.Where(x => x.ap_key == "AP_STREAM_BANNER").FirstOrDefault().ap_value,
                    //"categories/hxOzuT04fKwv4FFJYbHEJroqTGxpXAQBaWxytRlk.gif",
                    url = _db.app_settings.Where(x => x.ap_key == "AP_STREAM_URL").FirstOrDefault().ap_value
                    //"https://www.pscp.tv/w/clxGWTFkcmplTFJHbmtwS2J8MU1ZeE5WZ1ZaYnd4d-4-V1KUgo8wtnXfVi_oGX4opNR5r67BE-mUtPqeUP9h"
                },
                elevethSection = new DashboadStatistics()
                {
                    totalconsultants = "250",
                    totalfarmers = "62451",
                    totalmessages = "3254",
                    totalusers = "123652"
                },
                contactUsLinks = new ContactUs()
                {
                    AP_SYSTEM_EMAIL = "info@rose76.com",
                    AP_CUSTOMER_SUPPORT_WA_NUMBER = "+966566793327",
                    AP_CUSTOMER_SUPPORT_NUMBER = "+966596472727",
                    AP_FACEBOOK_URL = "saatco_it",
                    AP_TWITTER_URL = "agri_ext",
                    AP_YOUTUBE_URL = "saatco_it",
                    AP_INSTA_URL = "saatco_it",
                    AP_SNAP_URL = "saatco_it",
                },
                skills = new SkillItem()
                {
                    items = _db.skills.Where(x => x.parent_skill == null && x.active == true).Select(p => new Skills()
                    {
                        id = p.id,
                        name_ar = p.name,
                        menu_sort = p.menu_sort,
                        description = p.description,
                        icon = p.icon,
                        banner = p.banner,
                        text_color = p.text_color,
                        bg_color = p.bg_color
                    }).ToList()

                },
                groups = new GroupItem()
                {
                    items = _db.groups.Select(p => new Groups()
                    {
                        id = p.id,
                        region_id = p.region_id,
                        name_ar = p.name
                        
                    }).ToList()
                },
                regions = new RegionItem()
                {
                    items = _db.regions.Where(x => x.active == true).Select(p => new Region()
                    {
                        id = p.id,
                        name_ar = p.name_ar
                    }).ToList()
                }

            };

            dashboard.skills.items[0].children = new List<Skills>();

            dashboard.skills.items[0].children.AddRange(_db.skills.Where(x => x.parent_skill == 1 && x.active == true).Select(q => new Skills()
            {
                id = q.id,
                name_ar = q.name,
                menu_sort = 0,
                description = q.description,
                icon = q.icon,
                banner = q.banner,
                text_color = q.text_color,
                bg_color = q.bg_color
            }).ToList());

            for (int i = 0; i < dashboard.groups.items.Count(); i++)
            {
                long region_id = dashboard.groups.items[i].region_id;
                dashboard.groups.items[i].region = new Region();
                dashboard.groups.items[i].region.id = _db.regions.Where(x => x.id == region_id && x.active == true).FirstOrDefault().id;
                dashboard.groups.items[i].region.name_ar = _db.regions.Where(x => x.id == region_id && x.active == true).FirstOrDefault().name;
            }

            var arr = new ArrayList();
            arr.Add(dashboard);
            return arr.ToArray();
        }

        public object GetAllConsultantRating(long user_id)
        {
            var result = _db.consultant_rating.Where(x => x.consultant_id == user_id).Select(q => new {
                q.comment,
                q.rating,
                q.user1.avatar,
                q.user1.name
            }).ToList();

            return result;
        }

        public Data GetRoles()
        {
            Data d = new Data();

            var roles = _db.roles.Select(p => new { p.id, p.name_ar }).ToList();
            d.status = true;
            d.message = "successfully fetch the records";
            d.data = roles;
            return d;
        }

        public void UpdateChatID(string chatID, long uid)
        {
            var u = _db.users.Where(user => user.id == uid).FirstOrDefault();
            u.chatId = chatID;
            Save();
        }

        public object GetAllConsultant(long user_id)
        {
            SkillRepository sr = new SkillRepository();
            List<user> consultants = _db.users.Include("consultant_rating").Where(user => user.role_id == 6 && user.is_profile_completed == true && user.id != user_id && user.is_approved == true).ToList();
            if (consultants.Count() > 0)
            {
                for (int i = 0; i < consultants.Count(); i++)
                {
                    consultants[i].skills = sr.GetSkillsByConsultant(consultants[i].id);
                }
            }

            Console.WriteLine(consultants.Count());

            var consultatns = consultants
                                .Select(u => new
                                {
                                    id = u.id,
                                    name = u.name + " " + u.last_name,
                                    phone = u.phone,
                                    region = u.region,
                                    skills = u.skills,
                                    profile = u.profile,
                                    avatar = u.avatar,
                                    chatId = u.chatId,
                                    is_online = u.is_online,
                                    rating = u.consultant_rating.Count() > 0 ? u.consultant_rating.Select(q => q.rating).Average() : 0,
                                    role_id = u.role_id,
                                    last_login = u.last_login_date
                                })
                                .ToList();

            return consultatns;
        }

        public void AddSkillUser(long[] skill_id, long user_id)
        {
            long[] oldskill_id = _db.skill_user.Where(x => x.user_id == user_id).ToList().Select(x => x.skill_id).ToArray();
            long[] listCartisan = skill_id.Except(oldskill_id).ToArray();
            foreach (var item in listCartisan)
            {
                skill_user su = new skill_user();
                su.user_id = user_id;
                su.skill_id = item;
                su.is_approved = false;
                su.created_at = DateTime.Now;
                _db.skill_user.Add(su);
            }
            Save();
        }

        public string GetRegionName(int region_id)
        {
            return _db.regions.Where(x => x.id == region_id && x.active == true).FirstOrDefault().name_ar;
        }

        public Data IsOnline(bool value, user u)
        {
            Data d = new Data();
            u.is_online = value;
            Update(u);
            Save();

            Generic g = new Generic();
            g.SendGeneralNotification("status change", "", "work_status", u.phone, value == true ? "on" : "off");

            d.status = true;
            d.message = "Updated Successfully";
            d.data = null;
            return d;
        }

        public Data ChangeStatusOfUser(long user_id, bool status)
        {
            Data d = new Data();
            user u = GetUserByID(user_id);
            if (u != null)
            {
                u.is_group_admin = status == true ? false : true;
                u.can_post = status == true ? false : true;
                Save();
            }
            d.status = true;
            d.message = "Success";
            d.data = null;
            return d;
        }

        public Data UploadAvatar(user u, string img)
        {
            Data d = new Data();
            u.avatar = "/Media/Images/Avatar/" + img;
            Update(u);
            Save();
            d.status = true;
            d.message = "Successfully Upload Avatar";
            d.data = u.avatar;
            return d;
        }

        public Data UpdateApp(string appVersion, string lastLocation, int appType, user u)
        {
            Data d = new Data();
            u.app_type = appType;
            u.app_ver = appVersion;
            u.firebase_token = lastLocation;
            u.updated_at = DateTime.Now;
            Save();
            d.status = true;
            d.message = "successfully updated!";
            return d;
        }
    }
}