using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Models
{
    public class StatsCardModel
    {

        //public StatsCardModel(string title, string description)
        //{
        //    this.title = title;
        //    this.description = description;
        //    this.icon = "bx bxs-square-rounded";
        //    this.bgColor = "light";
        //    this.textColor = "black";
        //    this.link = "";
        //}

        //public StatsCardModel(string title, string description, string icon)
        //{
        //    this.title = title;
        //    this.description = description;
        //    this.icon = icon;
        //    this.bgColor = "light";
        //    this.textColor = "black";
        //    this.link = "";
        //}

        //public StatsCardModel(string title, string description, string icon, string bgColor)
        //{
        //    this.title = title;
        //    this.description = description;
        //    this.icon = icon;
        //    this.bgColor = bgColor;
        //    this.textColor = "black";
        //    this.link = "";
        //}

        //public StatsCardModel(string title, string description, string icon , string bgColor, string textColor)
        //{
        //    this.title = title;
        //    this.description = description;
        //    this.icon = icon;
        //    this.bgColor = bgColor;
        //    this.textColor = textColor;
        //    this.link = "";
        //}

        public StatsCardModel(string title, string description, string icon = "bx bxs-square-rounded", string bgColor = "light", string textColor = "black", string link = "")
        {
            this.title = title;
            this.description = description;
            this.icon = icon;
            this.bgColor = bgColor;
            this.textColor = textColor;
            this.link = link;
        }
        public string title { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
        public string bgColor { get; set; }
        public string textColor { get; set; }
        public string link { get; set; }
        
    }
}