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
    
    public partial class lab
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public lab()
        {
            this.appointments = new HashSet<appointment>();
            this.LabCollecctionPoints = new HashSet<LabCollecctionPoint>();
            this.user_access = new HashSet<user_access>();
        }
    
        public long id { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string latlang { get; set; }
        public string Address { get; set; }
        public int city_id { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string contactperson { get; set; }
        public Nullable<System.TimeSpan> open_at { get; set; }
        public Nullable<System.TimeSpan> close_at { get; set; }
        public string working_days { get; set; }
        public Nullable<int> max_appointment_per_day { get; set; }
        public Nullable<int> appointment_interval { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
        public bool active { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<appointment> appointments { get; set; }
        public virtual city city { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LabCollecctionPoint> LabCollecctionPoints { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<user_access> user_access { get; set; }
    }
}
