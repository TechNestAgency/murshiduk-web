using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MurshadikCP
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // Add this to Application_Start

            //GlobalConfiguration.Configuration.EnsureInitialized();

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //WebApiConfig.Register(System.Web.Http.GlobalConfiguration.Configuration);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            GlobalConfiguration.Configuration.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);
            AntiForgeryConfig.SuppressIdentityHeuristicChecks = true;
            //UnityConfig.RegisterComponents();
        }

        protected void Application_Error()
        {
            var exception = Server.GetLastError();
        }

        protected void Application_End()
        {
           
        }
    }
}
