using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MurshadikCP.Models.BLL
{
    public class AppSettingRepository
    {
        [Required]
        [AllowHtml]
        public string TERMS_EN { get; set; }
        [Required]
        [AllowHtml]
        public string TERMS_AR { get; set; }
        [Required]
        [AllowHtml]
        public string ABOUT_US_EN { get; set; }
        [Required]
        [AllowHtml]
        public string ABOUT_US_AR { get; set; }
        [Required]
        [EmailAddress]
        public string AP_SYSTEM_CC_EMAILS { get; set; }
        [Required]
        public string AP_CUSTOMER_SUPPORT_WA_NUMBER { get; set; }
        [Required]
        public string AP_CUSTOMER_SUPPORT_NUMBER { get; set; }
        [Required]
        public string AP_FACEBOOK_URL { get; set; }
        [Required]
        public string AP_TWITTER_URL { get; set; }
        [Required]
        public string AP_YOUTUBE_URL { get; set; }
        [Required]
        public string AP_INSTA_URL { get; set; }
        [Required]
        public string AP_SNAP_URL { get; set; }
        [Required]
        public string AP_PRODUCT_TEMPLATE { get; set; }
        [Required]
        public string AP_STREAM_URL { get; set; }
        [Required]
        public string AP_STREAM_TITLE { get; set; }
        [Required]
        public string AP_STREAM_BANNER { get; set; }
        [Required]
        public string AP_AD_ICON { get; set; }
        [Required]
        public string AP_AD_TITLE { get; set; }
        [Required]
        public string AP_AD_CONTENT { get; set; }
        [Required]
        public string AP_AD_LINK { get; set; }
        [Required]
        public string AP_SMSGatewayURL { get; set; }
        [Required]
        public string AP_SMSSaudiID { get; set; }
        [Required]
        public string AP_SMSSaudiPwd { get; set; }
        [Required]
        public string AP_SMSWorldID { get; set; }
        [Required]
        public string AP_SMSWorldPwd { get; set; }
        [Required]
        public string AP_SMSGatewaySenderID { get; set; }
    }
}