using Administrator.Models;
using BusinessLogicLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Administrator.Controllers
{
    public class MenuListingController : Controller
    {
        [SessionExpire]
        //
        // GET: /MenuListing/
        public ActionResult Index()
        {
            string[] roles = null;
            BusinessLogicLayer.Authorise obj = new Authorise();
            
            roles = obj.AuthorizedUserRight(System.Web.HttpContext.Current.Server, "Administrator", Convert.ToInt64(Session["ID"].ToString()));
            
            return View();
        }



	}
}