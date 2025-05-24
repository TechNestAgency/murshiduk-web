using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Models.BLL
{
    public class ArticleRepository
    {
        public long id { get; set; }
        public Nullable<long> category_id { get; set; }
        public string title { get; set; }
        [System.Web.Mvc.AllowHtml]
        public string description { get; set; }
        public string short_description { get; set; }
        public bool show_bg { get; set; }
        public bool show_cats { get; set; }
        public string image { get; set; }
        public bool active { get; set; }
        public long user_id { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
        public Nullable<System.DateTime> deleted_at { get; set; }
        public string keywords { get; set; }
        public HttpPostedFileBase parentimage { get; set; }
        public HttpPostedFileBase childimage { get; set; }
    }
}