using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;

namespace Leaders.Areas.Admin.Controllers
{
    public class SubstractDaysForPayoutMonthController : Controller
    {
        GANDHIBAGHV22Context db = new GANDHIBAGHV22Context();

        public ActionResult Index()
        {
            LeadersSubstractDaysForPayoutMonth substractMonths = db.LeadersSubstractDaysForPayoutMonths.FirstOrDefault();

            return View(substractMonths);
        }

        [HttpPost]
        public ActionResult Update(int ID)
        {
            LeadersSubstractDaysForPayoutMonth modelSubstractMonths = db.LeadersSubstractDaysForPayoutMonths.FirstOrDefault();

            // db.SaveChanges();

            return View(modelSubstractMonths);
        }

        [HttpPost]
        public ActionResult UpdateEmail(bool checkedStatus)
        {
            LeadersMobileDisplay_Downline objMobileDisplay = db.LeadersMobileDisplay_Downlines.FirstOrDefault();
            objMobileDisplay.IsEmailDisplay = checkedStatus;
            objMobileDisplay.ModifiedDate = System.DateTime.Now;
            db.SaveChanges();

            return Json(JsonRequestBehavior.AllowGet);
        }
    }
}