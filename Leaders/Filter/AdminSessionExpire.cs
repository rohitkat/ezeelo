using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace Leaders.Filter
{
    public class AdminSessionExpire : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (HttpContext.Current.Session["ID"] == null || HttpContext.Current.Session["RoleName"] == null)
            {
                FormsAuthentication.SignOut();
                filterContext.Result =
               new RedirectToRouteResult(new RouteValueDictionary
               {
                { "action", "Index" },
                { "controller", "Login" },
                   { "area", "Admin" },
                { "returnUrl", filterContext.HttpContext.Request.RawUrl}
             });

                return;

            }
        }
    }
}