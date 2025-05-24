using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Controllers.API.ApiViewModel
{
    public class Result
    {
        public bool status { get; set; }
        public string message { get; set; }
        public object data { get; set; }
        public object info { get; set; }

    }
}