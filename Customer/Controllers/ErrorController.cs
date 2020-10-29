using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gandhibagh.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/
        public ActionResult NotFound()
        {
            return View("HttpError");
        }

        public ActionResult ErrorPage()
        {
            return View("ErrorPage");
        }
	}
}