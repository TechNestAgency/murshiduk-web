using MurshadikCP.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace MurshadikCP
{
    enum LogType : int 
    {
        Weather = 1,
        Market,
        Lab,
        General
    }
    enum Role : long
    {
        SuperAdmin = 1,
        Manager,
        Employee,
        LabManager,
        Farmers,
        Consultants,
        Technician,
        Market,
        WeatherAdmin,
        MarketAdmin,
        RegionManager,
        AllRegionManager,
        ExtensionManager,
        VegetarianGuide,
        FishGuide,
        AnimalGuide

    }

    enum UserNotificationType : int
    {
        Market = 1,
        Weather
    }

    enum UserType: long
    {
        Farmer = 5,
        Consultant = 6
    }

    enum UserAccessTypeIdentifier : int
    {
        Labs = 1,
        Markets,
        Regions
    }

    public static class SettingKeys
    {

        public const string AboutUs = "ABOUT_US_AR";
        public const string TermAndCondition = "TERMS_AR";
    }


    public static class Constants
    {
        public const int defaultPageCount = 10;
        public const string SymetricKey = "my_secret_key_1231321";
        public const string ErrorMessageKey = "ErrorMessage";
        public const string SuccessMessageKey = "SuccessMessage";
    }

    public static class AppSettings
    {
        public const string AP_STREAM_ENABLED = "AP_STREAM_ENABLED";
        public const string AP_STREAM_URL = "AP_STREAM_URL";
        public const string AP_STREAM_BANNER = "AP_STREAM_BANNER";
        public const string AP_STREAM_TYPE = "AP_STREAM_TYPE";
        public const string TERMS_EN = "TERMS_EN";
        public const string TERMS_AR = "TERMS_AR";
        public const string AP_SYSTEM_EMAIL = "AP_SYSTEM_EMAIL";
        public const string ABOUT_US_EN = "ABOUT_US_EN";
        public const string ABOUT_US_AR = "ABOUT_US_AR";
        public const string AP_SYSTEM_CC_EMAILS = "AP_SYSTEM_CC_EMAILS";
        public const string AP_CUSTOMER_SUPPORT_WA_NUMBER =  "AP_CUSTOMER_SUPPORT_WA_NUMBER";
        public const string AP_CUSTOMER_SUPPORT_NUMBER = "AP_CUSTOMER_SUPPORT_NUMBER";
        public const string AP_FACEBOOK_URL = "AP_FACEBOOK_URL";
        public const string AP_TWITTER_URL = "AP_TWITTER_URL";
        public const string AP_YOUTUBE_URL = "AP_YOUTUBE_URL";
        public const string AP_INSTA_URL = "AP_INSTA_URL";
        public const string AP_SNAP_URL = "AP_SNAP_URL";
        public const string AP_TOTAL_CONSULTATIONS = "AP_TOTAL_CONSULTATIONS";
        public const string AP_TOTAL_MESSAGES = "AP_TOTAL_MESSAGES";
        public const string AP_PRODUCT_TEMPLATE = "AP_PRODUCT_TEMPLATE";
        public const string AP_STREAM_TITLE = "AP_STREAM_TITLE";
        public const string AP_AD_ICON = "AP_AD_ICON";
        public const string AP_AD_TITLE = "AP_AD_TITLE";
        public const string AP_AD_CONTENT = "AP_AD_CONTENT";
        public const string AP_AD_LINK = "AP_AD_LINK";
        public const string AP_SMSGatewayURL = "AP_SMSGatewayURL";
        public const string AP_SMSSaudiID = "AP_SMSSaudiID";
        public const string AP_SMSSaudiPwd = "AP_SMSSaudiPwd";
        public const string AP_SMSWorldID = "AP_SMSWorldID";
        public const string AP_SMSWorldPwd = "AP_SMSWorldPwd";
        public const string AP_SMSGatewaySenderID = "AP_SMSGatewaySenderID";
    }

    

    public static class MediaPath
    {
        public const string Articles = "/Media/Images/Articles/";
        public const string Categories = "/Media/Images/Categories/";
        public const string Skills = "/Media/Images/Skills/";
        public const string Avatar = "/Media/Images/Avatar/";
    }

    public static class BgColors {
        public const string Dark = "dark";
        public const string Pink = "pink";
        public const string Success = "success";
        public const string Warning = "warning";
        public const string Info = "info";
        public const string Danger = "danger";
        public const string Primary = "primary";
        public const string Secondary = "secondary";
        public const string Light = "light";
    }
    public static class DateTimeExtension {

        public static string UIFormatedDateTime(this DateTime date)
        {
            return date.ToString("dd/MMM/yy h:mm tt");
        }

        public static DateTime StartOfDay(this DateTime dateTime)
        {
            return new DateTime(
             dateTime.Year,
             dateTime.Month,
             dateTime.Day,
             0, 0, 0, 0);
        }
        public static DateTime EndOfDay(this DateTime dateTime)
        {
            return new DateTime(
             dateTime.Year,
             dateTime.Month,
             dateTime.Day,
             23, 59, 59, 999);
        }

        

        public static string ChartTimeFormat(this DateTime date)
        {
            return date.ToString("hh tt");
        }
    }

    public static class GeneralExtensions {

        public static string Id(this HtmlHelper htmlHelper)
        {
            var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;

            if (routeValues.ContainsKey("id"))
                return (string)routeValues["id"];
            else if (HttpContext.Current.Request.QueryString.AllKeys.Contains("id"))
                return HttpContext.Current.Request.QueryString["id"];

            return string.Empty;
        }

        public static string Controller(this HtmlHelper htmlHelper)
        {
            var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;

            if (routeValues.ContainsKey("controller"))
                return (string)routeValues["controller"];

            return string.Empty;
        }

        public static string Action(this HtmlHelper htmlHelper)
        {
            var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;

            if (routeValues.ContainsKey("action"))
                return (string)routeValues["action"];

            return string.Empty;
        }
        public static bool isKSANumber(this string phoneNo)
        {
            return phoneNo.StartsWith("966");
            
        }

        public static int isPicture(this string extension)
        {
            if (extension == "jpg" || extension == "jpeg" || extension == "gif" || extension == "bmp" || extension == "png" || extension == "tiff")
                return 1;
            else if (extension == "avi" || extension == "mp4" || extension == "wmv" || extension == "divx")
                return 2;
            else
                return 0;
        }
        public static AppUser appUser(this IIdentity identity)
        {
            if (identity == null) return null;
            
            var appUser = new AppUser(long.Parse((identity as ClaimsIdentity).FirstOrNull("userid")));
            //appUser.UserId = long.Parse((identity as ClaimsIdentity).FirstOrNull("userid"));
            
            //appUser.Name = (identity as ClaimsIdentity).FirstOrNull("name");
            //appUser.LastName = (identity as ClaimsIdentity).FirstOrNull("last_name");
            //appUser.RoleId = long.Parse(((identity as ClaimsIdentity).FirstOrNull("roleid") ?? "0"));

            //appUser.Valid = (identity as ClaimsIdentity).FirstOrNull("valid");

            return appUser;
        }

        internal static string FirstOrNull(this ClaimsIdentity identity, string claimType)
        {
            var val = identity.FindFirst(claimType);

            return val == null ? null : val.Value;
        }
    }

    public static class CustomFuntions 
    {
        public static string FormatNumber(int num)
        {
            if (num >= 100000)
                return FormatNumber(num / 1000) + "K";
            if (num >= 10000)
            {
                return (num / 1000D).ToString("0.#") + "K";
            }
            return num.ToString("#,0");
        }
    }

}