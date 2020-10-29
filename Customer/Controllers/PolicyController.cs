using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gandhibagh.Controllers
{
    public class PolicyController : Controller
    {
        //
        // GET: /Policy/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Policies()
        {
            return View();
        }
        //public ActionResult Return_Policy()
        //{
        //    return View();
        //}
        //public ActionResult Cancellation_Policy()
        //{
        //    return View();
        //}
        //public ActionResult Advertise_With_Us()
        //{
        //    return View();
        //}
	}
}