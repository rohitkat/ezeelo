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
    public class SubscriptionPlanFacilityController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private int pageSize = 10;

        // GET: /SubscriptionPlanFacility/
        public ActionResult Index(int? page, string SearchString = "")
        {
            int pageNumber = (page ?? 1);
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.SearchString = SearchString;
            //List<SubscriptionPlanFacility> lSubscriptionPlanFacilities = db.SubscriptionPlanFacilities.ToList();
            var subscriptionplanfacilities = db.SubscriptionPlanFacilities.Include(s => s.SubscriptionFacility).Include(s => s.SubscriptionPlan).ToList();
            if (SearchString != "")
            {
                subscriptionplanfacilities = subscriptionplanfacilities.Where(x => x.SubscriptionFacility.Name.Contains(SearchString)).ToList();
            }
            return View(subscriptionplanfacilities.ToPagedList(pageNumber, pageSize));
        }

        // GET: /SubscriptionPlanFacility/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubscriptionPlanFacility subscriptionplanfacility = db.SubscriptionPlanFacilities.Find(id);
            if (subscriptionplanfacility == null)
            {
                return HttpNotFound();
            }
            return View(subscriptionplanfacility);
        }

        // GET: /SubscriptionPlanFacility/Create
        public ActionResult Create()
        {
            ViewBag.SubscriptionFacilityID = new SelectList(db.SubscriptionFacilities, "ID", "Name");
            ViewBag.SubscriptionPlanID = new SelectList(db.SubscriptionPlans, "ID", "Name");
            return View();
        }

        // POST: /SubscriptionPlanFacility/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="SubscriptionPlanID,SubscriptionFacilityID,FacilityValue,IsActive")] SubscriptionPlanFacility subscriptionplanfacility)
        {
            try
            {
                subscriptionplanfacility.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                subscriptionplanfacility.CreateDate = DateTime.UtcNow.AddHours(5.3);
                if (ModelState.IsValid)
                {
                    db.SubscriptionPlanFacilities.Add(subscriptionplanfacility);
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
                return View(subscriptionplanfacility);
            }
            ViewBag.SubscriptionFacilityID = new SelectList(db.SubscriptionFacilities, "ID", "Name", subscriptionplanfacility.SubscriptionFacilityID);
            ViewBag.SubscriptionPlanID = new SelectList(db.SubscriptionPlans, "ID", "Name", subscriptionplanfacility.SubscriptionPlanID);
            return View(subscriptionplanfacility);
        }

        // GET: /SubscriptionPlanFacility/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubscriptionPlanFacility subscriptionplanfacility = db.SubscriptionPlanFacilities.Find(id);
            if (subscriptionplanfacility == null)
            {
                return HttpNotFound();
            }
            ViewBag.SubscriptionFacilityID = new SelectList(db.SubscriptionFacilities, "ID", "Name", subscriptionplanfacility.SubscriptionFacilityID);
            ViewBag.SubscriptionPlanID = new SelectList(db.SubscriptionPlans, "ID", "Name", subscriptionplanfacility.SubscriptionPlanID);
            return View(subscriptionplanfacility);
        }

        // POST: /SubscriptionPlanFacility/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,SubscriptionPlanID,SubscriptionFacilityID,FacilityValue,IsActive")] SubscriptionPlanFacility subscriptionplanfacility)
        {
            try
            {
                EzeeloDBContext db1 = new EzeeloDBContext();
                SubscriptionPlanFacility lsubscriptionplanfacility = db1.SubscriptionPlanFacilities.Find(subscriptionplanfacility.ID);
                subscriptionplanfacility.CreateBy = lsubscriptionplanfacility.CreateBy;
                subscriptionplanfacility.CreateDate = lsubscriptionplanfacility.CreateDate;
                subscriptionplanfacility.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                subscriptionplanfacility.ModifyDate = DateTime.UtcNow.AddHours(5.3);
                db1.Dispose();
                if (ModelState.IsValid)
                {
                    db.Entry(subscriptionplanfacility).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.SubscriptionFacilityID = new SelectList(db.SubscriptionFacilities, "ID", "Name", subscriptionplanfacility.SubscriptionFacilityID);
                ViewBag.SubscriptionPlanID = new SelectList(db.SubscriptionPlans, "ID", "Name", subscriptionplanfacility.SubscriptionPlanID);
                return View(subscriptionplanfacility);
            }
            catch (Exception ex)
            {
                return View(subscriptionplanfacility);
            }
            return View(subscriptionplanfacility);
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
