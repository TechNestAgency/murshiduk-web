using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Models
{
    public class ErrorModel
    {


        public ErrorModel(string message, bool status)
        {
            this.status = status;
            this.message = message;
        }
        
        public string message { get; set; }
        public bool status { get; set; }
    }
}