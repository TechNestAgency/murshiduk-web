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
    
    public partial class AlertMessage
    {
        public int Id { get; set; }
        public string message { get; set; }
        public string link { get; set; }
        public Nullable<int> alertActionId { get; set; }
        public Nullable<int> alertHazardId { get; set; }
        public Nullable<long> region_id { get; set; }
        public string city { get; set; }
        public Nullable<int> city_id { get; set; }
        public string season { get; set; }
        public System.DateTime created_at { get; set; }
        public long created_by { get; set; }
    
        public virtual AlertHazard AlertHazard { get; set; }
        public virtual AlertType AlertType { get; set; }
        public virtual region region { get; set; }
    }
}
