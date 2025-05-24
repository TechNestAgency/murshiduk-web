using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Controllers.API.ApiViewModel
{
    public class CategoryViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }

        public List<SupCategoryViewModel> Sup { get; set; }
    }
    public class SupCategoryViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}