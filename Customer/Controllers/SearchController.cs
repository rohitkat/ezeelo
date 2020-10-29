using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gandhibagh.Controllers
{
    public class SearchController : Controller
    {
        public ActionResult Index()
        {
            return PartialView("_SearchPartial");
        }
	}
}