using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Models.ViewModels
{
    public class VideoViewModelForDashboard
    {
    
       
            public string Id { get; set; }
            public string Title { get; set; }
            public string Location { get; set; }
            public string ConsultantName { get; set; }
            public string ConsultantPhone { get; set; }
            public string Category { get; set; }
            public string Department { get; set; }
            public string UserId { get; set; }
            public DateTime CreatAt { get; set; } 
            public int CommentCount { get; set; }
            public int LikeCount { get; set; }

            public string Tag { get; set; }

            public long IsActive { get; set; }

        public string Description { get; set; }


        /// <summary>
        /// For Filter
        /// </summary>
        public long CategoryId { get; set; }
        public long DepartmentId { get; set; }
        public DateTime Date { get; set; }


    }
}