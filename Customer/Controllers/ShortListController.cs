using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gandhibagh.Controllers
{
    public class ShortListController : Controller
    {
        //
        // GET: /ShortList/
        public ActionResult Shortlist()
        {
            return PartialView("_ShortListItemPartial");
            //return View();
        }
	}
}