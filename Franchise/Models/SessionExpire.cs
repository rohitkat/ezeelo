using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace Franchise.Models
{
    public class SessionExpire : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (HttpContext.Current.Session["USER_LOGIN_ID"] == null)
            {
                FormsAuthentication.SignOut();
                filterContext.Result =
               new RedirectToRouteResult(new RouteValueDictionary   
               {  
                { "action", "Index" },  
                { "controller", "FranchiseRegister" },  
                { "returnUrl", filterContext.HttpContext.Request.RawUrl}  
             });

                return;

            }
        }
    }
}