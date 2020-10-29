using BusinessLogicLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Franchise
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
           // GlobalConfiguration.Configure(WebApiConfig.Register);
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
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                string[] roles = null;
                BusinessLogicLayer.Authorise obj = new Authorise();
                if (HttpContext.Current.Session["ID"] != null)
                {
                    roles = obj.AuthorizedUserRight(System.Web.HttpContext.Current.Server, "Franchise", Convert.ToInt64(Session["ID"].ToString()));
                    Context.User = new System.Security.Principal.GenericPrincipal(User.Identity, roles);


                }
            }
        }


        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            //if (Context.Request.IsAuthenticated)
            //{
            //    string[] roles = null;
            //    BusinessLogicLayer.AuthorizedUser obj = new AuthorizedUser();

            //    //var underconstruction = HttpContext.Current.Session["underconstruction"];

            //    if (User.Identity.Name != null)
            //    {
            //        roles = obj.AuthorizedUserRight(System.Web.HttpContext.Current.Server, "Administrator", User.Identity.Name);
            //    }
            //    Context.User = new System.Security.Principal.GenericPrincipal(User.Identity, roles);
            //}
        }

        protected void Application_BeginRequest()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
        }

    }
}
