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
    
    public partial class ConsultationAvailablity
    {
        public int Id { get; set; }
        public System.TimeSpan startTime { get; set; }
        public int workingHours { get; set; }
        public int duration { get; set; }
        public string workingDays { get; set; }
    }
}
