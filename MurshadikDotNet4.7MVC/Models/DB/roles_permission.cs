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
    
    public partial class roles_permission
    {
        public long id { get; set; }
        public int role_id { get; set; }
        public int page_id { get; set; }
        public bool can_view { get; set; }
        public bool can_insert { get; set; }
        public bool can_update { get; set; }
        public bool can_delete { get; set; }
    
        public virtual Page Page { get; set; }
    }
}
