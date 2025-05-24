using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MurshadikCP.Models.DB;

namespace MurshadikCP.Models
{
    public class AppointmentLabViewModel
    {
        public IEnumerable<appointment> appointments { get; set; }
        public IEnumerable<lab_reports> lab_Reports_pending { get; set; }
        public IEnumerable<lab_reports> lab_Reports_completed { get; set; }
        public lab lab { get; set; }
    }
}