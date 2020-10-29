using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace Leaders.Filter
{
    public class TimeOutAdmin : ActionFilterAttribute
    {
      
       // public class SessionTimeoutAttribute : ActionFilterAttribute
        
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                HttpContext ctx = HttpContext.Current;
                if (HttpContext.Current.Session["ID"] == null)
                {
                //filterContext.Result = new RedirectResult("~/Admin/Login/Index");
                //return;
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
                base.OnActionExecuting(filterContext);
            }
        }
    }

