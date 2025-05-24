using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Controllers.API.ApiViewModel
{
    public class CommentViewModel
    {
        public long Id { get; set; }
        public string Comment { get; set; }
        public string User { get; set; }
        public DateTime Date { get; set; }
        public List<CommentReplyViewModel> Reply { get; set; }
    }
    public class CommentReplyViewModel
    {
        public long Id { get; set; }
        public string Comment { get; set; }
        public object User { get; set; }
        public DateTime Date { get; set; }
    }
}