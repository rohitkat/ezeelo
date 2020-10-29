using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using BusinessLogicLayer;

namespace Gandhibagh
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
          //  GlobalConfiguration.Configuration(WebApiConfig.Register);
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


        protected void Application_Error(object sender, EventArgs e)
        {

            // Preflight request comes with HttpMethod OPTIONS
            //if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
            //{
            //    HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache");
            //    HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST");
            //    The following line solves the error message
            //    HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
            //    If any http headers are shown in preflight error in browser console add them below
            //    HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept, Pragma, Cache-Control, Authorization ");
            //    HttpContext.Current.Response.AddHeader("Access-Control-Max-Age", "1728000");
            //    HttpContext.Current.Response.End();
            //}

            // Code that runs when an unhandled error occurs
            string strError;
            strError = Server.GetLastError().ToString();
            if (Context != null)
            {
                Context.ClearError();
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                   + Environment.NewLine + strError + Environment.NewLine
                  // + "[AllShopsController][GET:Index]" + myEx.EXCEPTION_PATH
                  ,BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
                Response.RedirectToRoute("notexists");
            }
        }

        //protected void Application_Error()
        //{
            
        //}
        protected void Application_BeginRequest()
        {
            //Get the current http context
            HttpContext InRequest = HttpContext.Current;

            //Get the current path
            string OldPath = InRequest.Request.Path.ToLower();

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
        }
        //protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        //{
        //    System.Diagnostics.Debugger.Break();
        //    if (Request.IsAuthenticated)
        //    {
        //        if (HttpContext.Current.User.Identity.AuthenticationType != "Forms")
        //            throw new Exception("Only forms authentication is supported, not " +
        //                HttpContext.Current.User.Identity.AuthenticationType);

        //        System.Security.Principal.IIdentity userId = HttpContext.Current.User.Identity;

        //        if (userId.Name != "")
        //        {
        //            if (HttpContext.Current.Cache[userId.Name + "_role"] != null)
        //            {
        //                string[] roles = (string[])HttpContext.Current.Cache[userId.Name + "_role"];
        //                HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(userId, roles);
        //            }
        //        }
        //    }//user != null
        //}

        //void MvcApplication_PostAuthenticateRequest(object sender, EventArgs e)
        //{
        //    var authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
        //    if (authCookie != null)
        //    {
        //        string encTicket = authCookie.Value;
        //        if (!String.IsNullOrEmpty(encTicket))
        //        {
        //            var ticket = FormsAuthentication.Decrypt(encTicket);
        //            var id = new UserIdentity(ticket);
        //            var userRoles = Roles.GetRolesForUser(id.Name);
        //            var prin = new GenericPrincipal(id, userRoles);
        //            HttpContext.Current.User = prin;
        //        }
        //    }
        //}
    }
    //public class SessionAuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    //{
    //    protected override bool AuthorizeCore(HttpContextBase httpContext)
    //    {
    //        return httpContext.Session["UID"] != null;
    //    }

    //    protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
    //    {
    //        if (HttpContext.Current.Request.QueryString["trackOrder"] != null)
    //        {
    //            filterContext.Result = new RedirectResult("/Login/Login?callFrom=normal&trackOrder=true");
    //        }
    //        else
    //        {
    //            filterContext.Result = new RedirectResult("/Home/Index");
    //        }
    //    }
    //}
}
