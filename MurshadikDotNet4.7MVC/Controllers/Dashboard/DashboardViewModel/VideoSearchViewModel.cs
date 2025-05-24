using MurshadikCP.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Models.ViewModels
{
    public class VideoSearchViewModel
    {
        public string SearshValue { get; set; }
        public long VideoCategoryId { get; set; }
        public long DepartmentId { get; set; }
        public long UserId { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public List<Category> CategoryRole { get; set; }
        public bool TypeSort { get; set; } 
    }
}