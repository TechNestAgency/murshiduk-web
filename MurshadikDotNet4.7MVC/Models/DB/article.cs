//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MurshadikCP.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public partial class article
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public article()
        {
            this.articles_galleries = new HashSet<articles_galleries>();
            this.category_articles = new HashSet<category_articles>();
        }
    
        public long id { get; set; }
        public long category_id { get; set; }
        public string title { get; set; }
        [AllowHtml]
        public string description { get; set; }
        public string short_description { get; set; }
        public string keywords { get; set; }
        public bool show_bg { get; set; }
        public bool show_cats { get; set; }
        public string image { get; set; }
        public bool active { get; set; }
        public Nullable<long> user_id { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
        public Nullable<System.DateTime> deleted_at { get; set; }
    
        public virtual category category { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<articles_galleries> articles_galleries { get; set; }
        public virtual user user { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<category_articles> category_articles { get; set; }
    }
}
