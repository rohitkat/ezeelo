using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using PagedList;
using PagedList.Mvc;
using BusinessLogicLayer;

namespace Administrator.Controllers
{
    public class SubscriptionFacilityController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private int pageSize = 10;

        // GET: /SubscriptionFacility/
        public ActionResult Index(int? page, string SearchString = "")
        {
            int pageNumber = (page ?? 1);
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.SearchString = SearchString;
            List<SubscriptionFacility> lSubscriptionFacilities = db.SubscriptionFacilities.ToList();
            if (SearchString != "")
            {
                lSubscriptionFacilities = lSubscriptionFacilities.Where(x => x.Name.Contains(SearchString)).ToList();
            }
            return View(lSubscriptionFacilities.ToPagedList(pageNumber, pageSize));
        }

        // GET: /SubscriptionFacility/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubscriptionFacility subscriptionfacility = db.SubscriptionFacilities.Find(id);
            if (subscriptionfacility == null)
            {
                return HttpNotFound();
            }
            return View(subscriptionfacility);
        }

        // GET: /SubscriptionFacility/Create
        public ActionResult Create()
        {
            var BehaviorType = from Administrator.Models.Constant.BEHAVIOR_TYPE d in Enum.GetValues(typeof(Administrator.Models.Constant.BEHAVIOR_TYPE)) select new { ID = (int)d, Name = d.ToString() };
            ViewBag.BehaviorType = new SelectList(BehaviorType, "ID", "Name");            
            return View();
        }

        // POST: /SubscriptionFacility/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,BehaviorType,IsActive")] SubscriptionFacility subscriptionfacility)
        {
            try
            {
                subscriptionfacility.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                subscriptionfacility.CreateDate = DateTime.UtcNow.AddHours(5.3);
                if (ModelState.IsValid)
                {
                    db.SubscriptionFacilities.Add(subscriptionfacility);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                //ModelState.AddModelError("Error", "There's something wrong with receive order on call values!");

                ////Code to write error log
                //BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                //    + Environment.NewLine + ex.Message + Environment.NewLine
                //    + "[ReceiveOrderOnCall][POST:Create]",
                //    BusinessLogicLayer.ErrorLog.Module.CRM, System.Web.HttpContext.Current.Server);
                return View(subscriptionfacility);
            }
            var BehaviorType = from Administrator.Models.Constant.BEHAVIOR_TYPE d in Enum.GetValues(typeof(Administrator.Models.Constant.BEHAVIOR_TYPE)) select new { ID = (int)d, Name = d.ToString() };
            ViewBag.BehaviorType = new SelectList(BehaviorType, "ID", "Name", subscriptionfacility.BehaviorType);
            return View(subscriptionfacility);
        }

        // GET: /SubscriptionFacility/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubscriptionFacility subscriptionfacility = db.SubscriptionFacilities.Find(id);
            if (subscriptionfacility == null)
            {
                return HttpNotFound();
            }
            var BehaviorType = from Administrator.Models.Constant.BEHAVIOR_TYPE d in Enum.GetValues(typeof(Administrator.Models.Constant.BEHAVIOR_TYPE)) select new { ID = (int)d, Name = d.ToString() };
            ViewBag.BehaviorType = new SelectList(BehaviorType, "ID", "Name", subscriptionfacility.BehaviorType);
            return View(subscriptionfacility);
        }

        // POST: /SubscriptionFacility/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,BehaviorType,IsActive")] SubscriptionFacility subscriptionfacility)
        {
            try
            {
                EzeeloDBContext db1 = new EzeeloDBContext();
                SubscriptionFacility lSubscriptionFacility = db1.SubscriptionFacilities.Find(subscriptionfacility.ID);
                subscriptionfacility.CreateBy = lSubscriptionFacility.CreateBy;
                subscriptionfacility.CreateDate = lSubscriptionFacility.CreateDate;
                subscriptionfacility.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                subscriptionfacility.ModifyDate = DateTime.UtcNow.AddHours(5.3);
                //var BehaviorType = from Administrator.Models.Constant.BEHAVIOR_TYPE d in Enum.GetValues(typeof(Administrator.Models.Constant.BEHAVIOR_TYPE)) select new { ID = (int)d, Name = d.ToString() };

                //if (subscriptionplan.Name.Equals(lSubscriptionPlan.Name))
                //{
                //    throw new Exception("SubscriptionPlan Name : " + lSubscriptionPlan.Name + " already exist.");
                //}
                db1.Dispose();
                if (ModelState.IsValid)
                {
                    db.Entry(subscriptionfacility).State = EntityState.Modified;
                    db.SaveChanges();
                    var BehaviorType = from Administrator.Models.Constant.BEHAVIOR_TYPE d in Enum.GetValues(typeof(Administrator.Models.Constant.BEHAVIOR_TYPE)) select new { ID = (int)d, Name = d.ToString() };
                    ViewBag.BehaviorType = new SelectList(BehaviorType, "ID", "Name", subscriptionfacility.BehaviorType);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                var BehaviorType = from Administrator.Models.Constant.BEHAVIOR_TYPE d in Enum.GetValues(typeof(Administrator.Models.Constant.BEHAVIOR_TYPE)) select new { ID = (int)d, Name = d.ToString() };
                ViewBag.BehaviorType = new SelectList(BehaviorType, "ID", "Name", subscriptionfacility.BehaviorType);
                return View(subscriptionfacility);
            }
            return View(subscriptionfacility);
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
