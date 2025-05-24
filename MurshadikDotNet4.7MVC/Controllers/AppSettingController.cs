using MurshadikCP.Models;
using MurshadikCP.Models.BLL;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace MurshadikCP.Controllers
{
    [Authorize]
    public class AppSettingController : BaseController
    {
        // GET: AppSetting
        // view of the list of application setting
        public ActionResult Index()
        {
            if (currentPageID("AppSetting") > 0)
            {
                if (!CurrentUser.canView(currentPageID("AppSetting")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            ViewBag.canInsert = CurrentUser.canInsert(currentPageID("AppSetting"));
            List<app_settings> result = db.app_settings.ToList();
            AppSettingRepository repo = new AppSettingRepository();
            repo.AP_STREAM_URL = result.Where(r => r.ap_key == AppSettings.AP_STREAM_URL).FirstOrDefault().ap_value;
            repo.AP_STREAM_TITLE = result.Where(r => r.ap_key == AppSettings.AP_STREAM_TITLE).FirstOrDefault().ap_value;
            repo.AP_STREAM_BANNER = result.Where(r => r.ap_key == AppSettings.AP_STREAM_BANNER).FirstOrDefault().ap_value;
            repo.TERMS_EN = result.Where(r => r.ap_key == AppSettings.TERMS_EN).FirstOrDefault().ap_value;
            repo.TERMS_AR = result.Where(r => r.ap_key == AppSettings.TERMS_AR).FirstOrDefault().ap_value;
            repo.ABOUT_US_EN = result.Where(r => r.ap_key == AppSettings.ABOUT_US_EN).FirstOrDefault().ap_value;
            repo.ABOUT_US_AR = result.Where(r => r.ap_key == AppSettings.ABOUT_US_AR).FirstOrDefault().ap_value;
            repo.AP_SYSTEM_CC_EMAILS = result.Where(r => r.ap_key == AppSettings.AP_SYSTEM_CC_EMAILS).FirstOrDefault().ap_value;
            repo.AP_CUSTOMER_SUPPORT_WA_NUMBER = result.Where(r => r.ap_key == AppSettings.AP_CUSTOMER_SUPPORT_WA_NUMBER).FirstOrDefault().ap_value;
            repo.AP_CUSTOMER_SUPPORT_NUMBER = result.Where(r => r.ap_key == AppSettings.AP_CUSTOMER_SUPPORT_NUMBER).FirstOrDefault().ap_value;
            repo.AP_FACEBOOK_URL = result.Where(r => r.ap_key == AppSettings.AP_FACEBOOK_URL).FirstOrDefault().ap_value;
            repo.AP_TWITTER_URL = result.Where(r => r.ap_key == AppSettings.AP_TWITTER_URL).FirstOrDefault().ap_value;
            repo.AP_YOUTUBE_URL = result.Where(r => r.ap_key == AppSettings.AP_YOUTUBE_URL).FirstOrDefault().ap_value;
            repo.AP_INSTA_URL = result.Where(r => r.ap_key == AppSettings.AP_INSTA_URL).FirstOrDefault().ap_value;
            repo.AP_SNAP_URL = result.Where(r => r.ap_key == AppSettings.AP_SNAP_URL).FirstOrDefault().ap_value;
            repo.AP_PRODUCT_TEMPLATE = result.Where(r => r.ap_key == AppSettings.AP_PRODUCT_TEMPLATE).FirstOrDefault().ap_value;
            repo.AP_AD_ICON = result.Where(r => r.ap_key == AppSettings.AP_AD_ICON).FirstOrDefault().ap_value;
            repo.AP_AD_TITLE = result.Where(r => r.ap_key == AppSettings.AP_AD_TITLE).FirstOrDefault().ap_value;
            repo.AP_AD_CONTENT = result.Where(r => r.ap_key == AppSettings.AP_AD_CONTENT).FirstOrDefault().ap_value;
            repo.AP_AD_LINK = result.Where(r => r.ap_key == AppSettings.AP_AD_LINK).FirstOrDefault().ap_value;
            repo.AP_SMSGatewayURL = result.Where(r => r.ap_key == AppSettings.AP_SMSGatewayURL).FirstOrDefault().ap_value;
            repo.AP_SMSSaudiID = result.Where(r => r.ap_key == AppSettings.AP_SMSSaudiID).FirstOrDefault().ap_value;
            repo.AP_SMSSaudiPwd = result.Where(r => r.ap_key == AppSettings.AP_SMSSaudiPwd).FirstOrDefault().ap_value;
            repo.AP_SMSWorldID = result.Where(r => r.ap_key == AppSettings.AP_SMSWorldID).FirstOrDefault().ap_value;
            repo.AP_SMSWorldPwd = result.Where(r => r.ap_key == AppSettings.AP_SMSWorldPwd).FirstOrDefault().ap_value;
            repo.AP_SMSGatewaySenderID = result.Where(r => r.ap_key == AppSettings.AP_SMSGatewaySenderID).FirstOrDefault().ap_value;

            return View(repo);
        }

        // Modify the appsettings
        public ActionResult Add(AppSettingRepository repo, HttpPostedFileBase AP_STREAM_BANNER, HttpPostedFileBase AP_AD_ICON)
        {
            if (currentPageID("AppSetting") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("AppSetting")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            app_settings apps = db.app_settings.Where(x => x.ap_key == nameof(repo.ABOUT_US_AR)).FirstOrDefault();
            apps.ap_value = repo.ABOUT_US_AR;
            db.Entry(apps).State = EntityState.Modified;
            db.SaveChanges();

            apps = db.app_settings.Where(x => x.ap_key == nameof(repo.ABOUT_US_EN)).FirstOrDefault();
            apps.ap_value = repo.ABOUT_US_EN;
            db.Entry(apps).State = EntityState.Modified;
            db.SaveChanges();

            apps = db.app_settings.Where(x => x.ap_key == nameof(repo.TERMS_EN)).FirstOrDefault();
            apps.ap_value = repo.TERMS_EN;
            db.Entry(apps).State = EntityState.Modified;
            db.SaveChanges();

            apps = db.app_settings.Where(x => x.ap_key == nameof(repo.TERMS_AR)).FirstOrDefault();
            apps.ap_value = repo.TERMS_AR;
            db.Entry(apps).State = EntityState.Modified;
            db.SaveChanges();

            apps = db.app_settings.Where(x => x.ap_key == nameof(repo.AP_SYSTEM_CC_EMAILS)).FirstOrDefault();
            apps.ap_value = repo.AP_SYSTEM_CC_EMAILS;
            db.Entry(apps).State = EntityState.Modified;
            db.SaveChanges();

            apps = db.app_settings.Where(x => x.ap_key == nameof(repo.AP_CUSTOMER_SUPPORT_WA_NUMBER)).FirstOrDefault();
            if (repo.AP_CUSTOMER_SUPPORT_WA_NUMBER != null)
                apps.ap_value = repo.AP_CUSTOMER_SUPPORT_WA_NUMBER;
            else
                apps.ap_value = "";
            db.Entry(apps).State = EntityState.Modified;
            db.SaveChanges();

            apps = db.app_settings.Where(x => x.ap_key == nameof(repo.AP_CUSTOMER_SUPPORT_NUMBER)).FirstOrDefault();
            apps.ap_value = repo.AP_CUSTOMER_SUPPORT_NUMBER;
            db.Entry(apps).State = EntityState.Modified;
            db.SaveChanges();

            apps = db.app_settings.Where(x => x.ap_key == nameof(repo.AP_FACEBOOK_URL)).FirstOrDefault();
            if (repo.AP_FACEBOOK_URL != null)
                apps.ap_value = repo.AP_FACEBOOK_URL;
            else
                apps.ap_value = "";
            db.Entry(apps).State = EntityState.Modified;
            db.SaveChanges();

            apps = db.app_settings.Where(x => x.ap_key == nameof(repo.AP_TWITTER_URL)).FirstOrDefault();
            if (repo.AP_TWITTER_URL != null)
                apps.ap_value = repo.AP_TWITTER_URL;
            else
                apps.ap_value = "";
            db.Entry(apps).State = EntityState.Modified;
            db.SaveChanges();

            apps = db.app_settings.Where(x => x.ap_key == nameof(repo.AP_YOUTUBE_URL)).FirstOrDefault();
            if (repo.AP_YOUTUBE_URL != null)
                apps.ap_value = repo.AP_YOUTUBE_URL;
            else
                apps.ap_value = "";
            db.Entry(apps).State = EntityState.Modified;
            db.SaveChanges();

            apps = db.app_settings.Where(x => x.ap_key == nameof(repo.AP_INSTA_URL)).FirstOrDefault();
            if (repo.AP_INSTA_URL != null)
                apps.ap_value = repo.AP_INSTA_URL;
            else
                apps.ap_value = "";
            db.Entry(apps).State = EntityState.Modified;
            db.SaveChanges();

            apps = db.app_settings.Where(x => x.ap_key == nameof(repo.AP_SNAP_URL)).FirstOrDefault();
            if (repo.AP_SNAP_URL != null)
                apps.ap_value = repo.AP_SNAP_URL;
            else
                apps.ap_value = "";
            db.Entry(apps).State = EntityState.Modified;
            db.SaveChanges();

            apps = db.app_settings.Where(x => x.ap_key == nameof(repo.AP_PRODUCT_TEMPLATE)).FirstOrDefault();
            apps.ap_value = repo.AP_PRODUCT_TEMPLATE;
            db.Entry(apps).State = EntityState.Modified;
            db.SaveChanges();

            apps = db.app_settings.Where(x => x.ap_key == nameof(repo.AP_STREAM_URL)).FirstOrDefault();
            apps.ap_value = repo.AP_STREAM_URL;
            db.Entry(apps).State = EntityState.Modified;
            db.SaveChanges();

            
            if (AP_STREAM_BANNER != null && AP_STREAM_BANNER.ContentLength > 0)
            {
                Guid bannerGuid = Guid.NewGuid();
                var img = bannerGuid.ToString() + Path.GetExtension(AP_STREAM_BANNER.FileName);
                var path = Path.Combine(Server.MapPath("~/Media/Images/"), img);
                AP_STREAM_BANNER.SaveAs(path);
                apps = db.app_settings.Where(x => x.ap_key == nameof(repo.AP_STREAM_BANNER)).FirstOrDefault();
                apps.ap_value = "/Media/Images/" + bannerGuid.ToString() + Path.GetExtension(AP_STREAM_BANNER.FileName);
                db.Entry(apps).State = EntityState.Modified;
                db.SaveChanges();
            }

            apps = db.app_settings.Where(x => x.ap_key == nameof(repo.AP_STREAM_TITLE)).FirstOrDefault();
            apps.ap_value = repo.AP_STREAM_TITLE;
            db.Entry(apps).State = EntityState.Modified;
            db.SaveChanges();

            if (AP_AD_ICON != null && AP_AD_ICON.ContentLength > 0)
            {
                Guid ADGuid = Guid.NewGuid();
                var img = ADGuid.ToString() + Path.GetExtension(AP_AD_ICON.FileName);
                var path = Path.Combine(Server.MapPath("~/Media/Images/"), img);
                AP_AD_ICON.SaveAs(path);
                apps = db.app_settings.Where(x => x.ap_key == nameof(repo.AP_AD_ICON)).FirstOrDefault();
                apps.ap_value = "/Media/Images/" + ADGuid.ToString() + Path.GetExtension(AP_AD_ICON.FileName);
                db.Entry(apps).State = EntityState.Modified;
                db.SaveChanges();
            }

            apps = db.app_settings.Where(x => x.ap_key == nameof(repo.AP_AD_TITLE)).FirstOrDefault();
            apps.ap_value = repo.AP_AD_TITLE;
            db.Entry(apps).State = EntityState.Modified;
            db.SaveChanges();

            apps = db.app_settings.Where(x => x.ap_key == nameof(repo.AP_AD_CONTENT)).FirstOrDefault();
            apps.ap_value = repo.AP_AD_CONTENT;
            db.Entry(apps).State = EntityState.Modified;
            db.SaveChanges();

            apps = db.app_settings.Where(x => x.ap_key == nameof(repo.AP_AD_LINK)).FirstOrDefault();
            apps.ap_value = repo.AP_AD_LINK ?? "";
            db.Entry(apps).State = EntityState.Modified;
            db.SaveChanges();

            apps = db.app_settings.Where(x => x.ap_key == nameof(repo.AP_SMSGatewayURL)).FirstOrDefault();
            apps.ap_value = repo.AP_SMSGatewayURL ?? "";
            db.Entry(apps).State = EntityState.Modified;
            db.SaveChanges();

            apps = db.app_settings.Where(x => x.ap_key == nameof(repo.AP_SMSSaudiID)).FirstOrDefault();
            apps.ap_value = repo.AP_SMSSaudiID ?? "";
            db.Entry(apps).State = EntityState.Modified;
            db.SaveChanges();

            apps = db.app_settings.Where(x => x.ap_key == nameof(repo.AP_SMSSaudiPwd)).FirstOrDefault();
            apps.ap_value = repo.AP_SMSSaudiPwd ?? "";
            db.Entry(apps).State = EntityState.Modified;
            db.SaveChanges();

            apps = db.app_settings.Where(x => x.ap_key == nameof(repo.AP_SMSWorldID)).FirstOrDefault();
            apps.ap_value = repo.AP_SMSWorldID ?? "";
            db.Entry(apps).State = EntityState.Modified;
            db.SaveChanges();

            apps = db.app_settings.Where(x => x.ap_key == nameof(repo.AP_SMSWorldPwd)).FirstOrDefault();
            apps.ap_value = repo.AP_SMSWorldPwd ?? "";
            db.Entry(apps).State = EntityState.Modified;
            db.SaveChanges();

            apps = db.app_settings.Where(x => x.ap_key == nameof(repo.AP_SMSGatewaySenderID)).FirstOrDefault();
            apps.ap_value = repo.AP_SMSGatewaySenderID ?? "";
            db.Entry(apps).State = EntityState.Modified;
            db.SaveChanges();


            return RedirectToAction("Index");
        }
    }
}