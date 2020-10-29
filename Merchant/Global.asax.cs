using BusinessLogicLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Merchant
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //Yashaswi 07-02-2019 For Log4net
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Debug("Application Start"); //This will create log file in \logs\Debug\yyyyMMdd.log
            log.Info("Application Start"); //This will create log file in \logs\Info\yyyyMMdd.log
            log.Error("Application Start"); //This will create log file in \logs\Error\yyyyMMdd.log
            //log.Fatal("Fatal logging");
            //log.Warn("Warn logging");
        }
        protected void Session_Start(Object sender, EventArgs e)
        {
            HttpContext.Current.Session["underconstruction"] = "true";
            HttpContext.Current.Session["beingredirected"] = "false";
        }


        protected void Application_AcquireRequestState(Object sender, EventArgs e)
        {
            //Response.Headers.Add("Access-Control-Allow-Origin", "http://ezeelo.in/");
            //Response.Headers.Add("Access-Control-Allow-Headers",
            //  "Content-Type, Access-Control-Allow-Headers, Authorization, X-Requested-With");
            //Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
            //Response.Headers.Add("Access-Control-Allow-Credentials", "true");
            //HttpContext.Current.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                string[] roles = null;
                BusinessLogicLayer.Authorise obj = new Authorise();
                if (HttpContext.Current.Session["ID"] != null)
                {
                    roles = obj.AuthorizedUserRight(System.Web.HttpContext.Current.Server, "Merchant", Convert.ToInt64(Session["ID"].ToString()));
                    Context.User = new System.Security.Principal.GenericPrincipal(User.Identity, roles);
                }
            }
        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            // Preflight request comes with HttpMethod OPTIONS
            //if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
            //{
                HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache");
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST");
                // The following line solves the error message
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
                // If any http headers are shown in preflight error in browser console add them below
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept, Pragma, Cache-Control, Authorization ");
                HttpContext.Current.Response.AddHeader("Access-Control-Max-Age", "1728000");
                HttpContext.Current.Response.End();
            //}
        }
    }
}
