using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Controllers.API.ApiViewModel
{
    public class VideoViewModel
    {

        public string Id { get; set; }
        public string Title { get; set; }
        public string CreatAt { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string ConsultantName { get; set; }
        public string Category { get; set; }
        public string Department { get; set; }
        public string UserId { get; set; }
        public string Tag { get; set; }
        public long VideoStatus { get; set; }
        public int  VideoLike { get; set; }
        public bool? IsHidden { get; set; }
        public List<CommentViewModel> Comments { get; set; }
    }
}