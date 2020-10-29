using Gandhibagh.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gandhibagh.Controllers
{
    public class ManagedController : Controller
    {
        //
        // GET: /Managed/
        [DynamicMetaTag]
        public ActionResult Index(long ParentCatID, int Level, long cityId, int franchiseId)//added franchiseId
        {
            URLCookie.SetCookies();
            return View();
        }
	}
}