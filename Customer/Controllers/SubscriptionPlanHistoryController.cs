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
using Gandhibagh.Models;

namespace Gandhibagh.Controllers
{
    public class SubscriptionPlanHistoryController : Controller
    {

        private EzeeloDBContext db = new EzeeloDBContext();
        public static long UserID = 0;
        //
        // GET: /SubscriptionPlanHistory/
        [SessionExpire]
        public ActionResult Index()
        {

            if (Session["UID"] != null)
            {
                UserID = Convert.ToInt64(Session["UID"]);
            }
            var FacilityDetail = (from sp in db.SubscriptionPlans
                                  join spf in db.SubscriptionPlanFacilities on sp.ID equals spf.SubscriptionPlanID
                                  join sf in db.SubscriptionFacilities on spf.SubscriptionFacilityID equals sf.ID
                                  join sppb in db.SubscriptionPlanPurchasedBies on sp.ID equals sppb.SubscriptionPlanID
                                  where sppb.UserLoginID == UserID
                                  select new SubscriptionFacilityDetailViewModel
                                  {
                                      ID = sp.ID,
                                      StartDate = sppb.StartDate,
                                      EndDate = sppb.EndDate,
                                      SubscriptionPlan = sp.Name,
                                      // Facility = sf.Name,
                                      Fees = sp.Fees,
                                      NoOfDays = sp.NoOfDays,
                                      IsActive = sppb.IsActive
                                      //NoOfCoupens = sp.NoOfCoupens
                                  }).Distinct().ToList();
            //var subscriptionplanpurchasedbies = db.SubscriptionPlanPurchasedBies.Include(s => s.SubscriptionPlan).Where(x => x.UserLoginID == UserID).OrderByDescending(x => x.StartDate).Distinct().ToList();
            return View(FacilityDetail.ToList());
        }

        //
        // GET: /SubscriptionPlanHistory/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /SubscriptionPlanHistory/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /SubscriptionPlanHistory/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /SubscriptionPlanHistory/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /SubscriptionPlanHistory/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /SubscriptionPlanHistory/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /SubscriptionPlanHistory/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
