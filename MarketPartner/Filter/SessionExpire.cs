using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace MarketPartner.Filter
{
    public class SessionExpire : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (HttpContext.Current.Session["MerchantID"] == null)
            {
                FormsAuthentication.SignOut();
                filterContext.Result =
               new RedirectToRouteResult(new RouteValueDictionary
               {
                { "action", "login" },
                { "controller", "Home" },
                { "returnUrl", filterContext.HttpContext.Request.RawUrl}
             });

                return;

            }
            else
            {
                long MerchantID = Convert.ToInt64(HttpContext.Current.Session["MerchantID"]);
                EzeeloDBContext db = new EzeeloDBContext();
                MerchantTopupRecharge recharge = db.merchantTopupRecharges.FirstOrDefault(p => p.MerchantID == MerchantID);
                if(recharge == null)
                {
                    HttpContext.Current.Session["ShopNAme"] = null;
                    HttpContext.Current.Session["ContactPerson"] = null;
                    HttpContext.Current.Session["ShopImgPath"] = null;
                    filterContext.Result =
                   new RedirectToRouteResult(new RouteValueDictionary
                   {
                { "action", "Index" },
                { "controller", "RechargeAccount" },
                 { "flag",1}
                 });

                    return;
                }
            }
        }
    }
}