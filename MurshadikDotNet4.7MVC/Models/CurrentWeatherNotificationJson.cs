using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Models
{
    public class CurrentWeatherNotificationJson
    {
        public string id { get; set; }
        public string link { get; set; }
        public string title { get; set; }
        public string regiondID { get; set; }
        public List<governorates> governorates { get; set; }
        public string message { get; set; }
        public string alertType { get; set; }
        public string alertStatusCategory { get; set; }
        public string alertStatusAr { get; set; }
        public List<alertHazards> alertHazards { get; set; }
        public string lastModified { get; set; }
        public string fromdate { get; set; }
        public string todate { get; set; }
    }

    public class governorates
    {
        public string id { get; set; }
    }

    public class alertHazards
    {
        public string id { get; set; }
    }
}