using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using ModelLayer.Models;
namespace Gandhibagh.Models
{
    public class SessionExpire : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            // To Check Session Value 
            //If Session is empty then logout from formAuthentication and redirect to login page
            if (HttpContext.Current.Session["UID"] == null)
            {
                FormsAuthentication.SignOut();

                if (HttpContext.Current.Request.QueryString["trackOrder"] != null)
                {
                    //filterContext.Result = new RedirectResult("/CustomerOrder/OrderStatus");
                    string cityName = "";
                    int franchiseID = 0;////added 
                    if (HttpContext.Current.Request.Cookies["CityCookie"] != null && !string.IsNullOrEmpty(HttpContext.Current.Request.Cookies["CityCookie"].Value))
                    {
                        cityName = HttpContext.Current.Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower();
                        franchiseID = Convert.ToInt32(HttpContext.Current.Request.Cookies["CityCookie"].Value.Split('$')[2]);////added 
                    }
                    filterContext.Result = new RedirectResult("/" + cityName + "/" + franchiseID + "/cust-o/order-s");////added   "/" + franchiseID +
                }
                else
                {
                    filterContext.Result =
                   new RedirectToRouteResult(new RouteValueDictionary   
                                               {  
                                                { "action", "Login" },  
                                                { "controller", "Login" },  
                                                { "returnUrl", filterContext.HttpContext.Request.RawUrl}  
                                             });
                }

                return;

            }
            else
            {
                string cityName = "";
                int franchiseID = 0;////added 
                if (HttpContext.Current.Request.Cookies["CityCookie"] != null && !string.IsNullOrEmpty(HttpContext.Current.Request.Cookies["CityCookie"].Value))
                {
                    cityName = HttpContext.Current.Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower();
                    franchiseID = Convert.ToInt32(HttpContext.Current.Request.Cookies["CityCookie"].Value.Split('$')[2]);////added 
                }
                string str = HttpContext.Current.Request.Url.ToString();
                if (franchiseID == 1 && !str.Contains("Merchant/List"))
                {
                    filterContext.Result = new RedirectResult("/" + cityName + "/" + franchiseID + "/Merchant/List");
                }
            }
        }
    }
}
