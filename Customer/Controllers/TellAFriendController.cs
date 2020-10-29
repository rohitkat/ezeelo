using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gandhibagh.Controllers
{
    public class TellAFriendController : Controller
    {
        //
        // GET: /TellAFriend/
        public ActionResult Index()
        {
            if (Session["UID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("login", "Login", new { callfrom = "normal" });
            }
        }
	}
}