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
    
    public partial class FarmVisitMessage
    {
        public long Id { get; set; }
        public long FarmOrderId { get; set; }
        public string Message { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public long CreatedBy { get; set; }
    }
}
