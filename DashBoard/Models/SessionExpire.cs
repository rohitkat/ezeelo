using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace DashBoard.Models
{
    public class SessionExpire : ActionFilterAttribute
    {
        /// <summary>
        /// for checkin Session is availabe or not
        /// if session is over redirect to login page
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (HttpContext.Current.Session["ID"] == null)
            {
                //Signe out from from Authentication
                FormsAuthentication.SignOut();

                //Action Filter to redirect to Page
                filterContext.Result =
               new RedirectToRouteResult(new RouteValueDictionary   
               {  
                { "action", "Login" },  
                { "controller", "Login" },  
                { "returnUrl", filterContext.HttpContext.Request.RawUrl}  
             });

                return;
            }
        }


    }

    public class LogoutNocach : ActionFilterAttribute
    {
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
            filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            filterContext.HttpContext.Response.Cache.SetNoStore();



            base.OnResultExecuted(filterContext);

        }

    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class CacheControlAttribute : ActionFilterAttribute
    {
        private readonly HttpCacheability cacheability;
        public HttpCacheability Cacheability { get { return this.cacheability; } }
        public CacheControlAttribute(HttpCacheability cacheability)
        {
            this.cacheability = cacheability;
        }
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            HttpCachePolicyBase cache = filterContext.HttpContext.Response.Cache;
            cache.SetCacheability(this.cacheability);
        }
    }
}