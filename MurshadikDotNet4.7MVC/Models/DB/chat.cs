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
    
    public partial class chat
    {
        public long id { get; set; }
        public string chat_ids { get; set; }
        public string last_message { get; set; }
        public int message_type { get; set; }
        public Nullable<int> status { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
    
        public virtual messagetype messagetype { get; set; }
    }
}
