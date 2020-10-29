using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace API
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            // Sonali starts 02/02/2019 for the change App bug ID 47 - Get profile for ignore json serializer
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            GlobalConfiguration.Configuration.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);
            // Sonali ends 02/02/2019 for the change App bug ID 47 - Get profile for ignore json serializer
            //Yashaswi 07-02-2019 For Log4net
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Debug("Application Start"); //This will create log file in \logs\Debug\yyyyMMdd.log
            log.Info("Application Start"); //This will create log file in \logs\Info\yyyyMMdd.log
            log.Error("Application Start"); //This will create log file in \logs\Error\yyyyMMdd.log
            //log.Fatal("Fatal logging");
            //log.Warn("Warn logging");
        }
    }
}
