using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Leaders.Filter;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;


namespace Leaders.Areas.Admin.Controllers
{
    [AdminSessionExpire]
    public class DisplayMobileInDownlineController : Controller
    {
        EzeeloDBContext db = new EzeeloDBContext();

        public ActionResult Index()
        {
           LeadersMobileDisplay_Downline displayMobile = db.LeadersMobileDisplay_Downlines.FirstOrDefault();

            return View(displayMobile);
        }

        [HttpPost]
        public ActionResult Update(bool checkedStatus)
        {
            LeadersMobileDisplay_Downline objDisplayMobile = db.LeadersMobileDisplay_Downlines.FirstOrDefault();
            objDisplayMobile.IsMobileDisplay = checkedStatus;
            db.SaveChanges();

            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateEmail(bool emailStatus)
        {
            LeadersMobileDisplay_Downline objDisplayMobile = db.LeadersMobileDisplay_Downlines.FirstOrDefault();
            objDisplayMobile.IsEmailDisplay = emailStatus;
            db.SaveChanges();

            return Json(JsonRequestBehavior.AllowGet);
        }
    }
}