using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;

namespace Gandhibagh.Controllers
{
    public class SubscriptionPlanPurchasedByController : Controller
    {
        //
        // GET: /SubscriptionPlanPurchasedBy/
        private EzeeloDBContext db = new EzeeloDBContext();
        public static long UserID = 0;

        // GET: /SubscriptionPlanPurchasedBy/
        public ActionResult Index()
        {

            if (Session["UID"] != null)
            {
                UserID = Convert.ToInt64(Session["UID"]);
            }

            BusinessLogicLayer.SubscriptionCalculator subscriptioncalculator = new SubscriptionCalculator(System.Web.HttpContext.Current.Server);
            var FacilityDetail = SubscriptionCalculator.SubscribedDetails(UserID);
            return View(FacilityDetail.ToList());
           
        }
        [HttpPost]
        public ActionResult FacilityDetail(long SubPlanID)
        {
            try
            {

                BusinessLogicLayer.SubscriptionCalculator subscriptioncalculator = new SubscriptionCalculator(System.Web.HttpContext.Current.Server);
                var FacilityDetail= SubscriptionCalculator.SubscribedDetails(UserID);
                return View("_FacilityDetail", FacilityDetail);
                //long SubPlanPurchasedbyId=db.SubscriptionPlanPurchasedBies.Where(x=>x.UserLoginID==UserID && x.SubscriptionPlanID==SubPlanID && x.IsActive==true).Select(x=>x.ID).FirstOrDefault();
                //decimal RemainingSubscriptionValue = db.SubscriptionPlanUsedBies.OrderByDescending(x => x.ID).Where(x => x.SubscriptionPlanPurchasedByID == SubPlanPurchasedbyId).Select(x => x.SubsriptionValue).FirstOrDefault();
                //var FacilityDetail = (from sp in db.SubscriptionPlans
                //                      join spf in db.SubscriptionPlanFacilities on sp.ID equals spf.SubscriptionPlanID
                //                      join sf in db.SubscriptionFacilities on spf.SubscriptionFacilityID equals sf.ID
                //                      join sppb in db.SubscriptionPlanPurchasedBies on sp.ID equals sppb.SubscriptionPlanID
                //                      where sppb.UserLoginID == UserID 
                //                      && sp.ID == SubPlanID
                //                      select new SubscriptionFacilityDetailViewModel
                //                      {
                //                          Facility = sf.Name,
                //                          Fees = sp.Fees,
                //                          NoOfDays = sp.NoOfDays,
                //                          NoOfCoupens = sp.NoOfCoupens
                //                      }).Distinct().ToList();
               
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in displaying facility details!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SubscriptionPlanPurchasedBy][POST:FacilityDetail]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong in displaying facility details!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SubscriptionPlanPurchasedBy][POST:FacilityDetail]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

      
        // GET: /SubscriptionPlanPurchasedBy/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubscriptionPlanPurchasedBy subscriptionplanpurchasedby = db.SubscriptionPlanPurchasedBies.Find(id);
            if (subscriptionplanpurchasedby == null)
            {
                return HttpNotFound();
            }
            return View(subscriptionplanpurchasedby);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}