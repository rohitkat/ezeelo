using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Administrator.Models
{
    public class CustomAuthorize : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                //if not logged, it will work as normal Authorize and redirect to the Login
                base.HandleUnauthorizedRequest(filterContext);

            }
            else
            {
                //logged and wihout the role to access it - redirect to the custom controller action
                //string returnUrl = HttpContext.Current.Request.Url.ToString().ToLower();
                //Tempdata["returnUrl"] = HttpContext.Current.Request.Url.ToString().ToLower();
                //filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "AccessDenied", returnUrl }));

                filterContext.Result =
              new RedirectToRouteResult(new RouteValueDictionary   
               {  
                { "action", "AccessDenied" },  
                { "controller", "Home" },  
                { "returnUrl", filterContext.HttpContext.Request.RawUrl}  
             });

            }
        }
    }
}