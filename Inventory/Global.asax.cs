using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
//using System.Web.Optimization;
using System.Web.Routing;

namespace Inventory
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //BundleConfig.RegisterBundles(BundleTable.Bundles);

            //Yashaswi 07-02-2019 For Log4net
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Debug("Application Start"); //This will create log file in \logs\Debug\yyyyMMdd.log
            log.Info("Application Start"); //This will create log file in \logs\Info\yyyyMMdd.log
            log.Error("Application Start"); //This will create log file in \logs\Error\yyyyMMdd.log
            //log.Fatal("Fatal logging");
            //log.Warn("Warn logging");
        }
        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
        }
    }
}
