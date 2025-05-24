using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Models
{
    public class MenuModel
    {
        public int id { get; set; }
        public bool has_childern { get; set; }
        public string route { get; set; }
        public string display_name { get; set; }
        public string icon { get; set; }
        public List<MenuModel> children { get; set; }
        public bool should_display { get; set; }
        public bool is_active { get; set; }

        public bool ShouldDisplay()
        {
            return this.is_active && this.should_display;
        }
    }

    public class UserAccessMenuModel
    {
        public int id { get; set; }
        public int pageId { get; set; }
        public bool canView { get; set; }
        public bool canUpdate { get; set; }
        public bool canInsert { get; set; }
        public bool canDelete { get; set; }
    }
}