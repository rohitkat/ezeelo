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
    public class SubscriptionPlanController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private int pageSize = 10;
        // GET: /SubscriptionPlan/
        public ActionResult Index(int? page, string SearchString = "")
        {
            int pageNumber = (page ?? 1);
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.SearchString = SearchString;
            List<SubscriptionPlan> lSubscriptionPlans = db.SubscriptionPlans.ToList();
            if (SearchString != "")
            {
                lSubscriptionPlans = lSubscriptionPlans.Where(x => x.Name.Contains(SearchString)).ToList();
            }
            return View(lSubscriptionPlans.ToPagedList(pageNumber, pageSize));
        }

        // GET: /SubscriptionPlan/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubscriptionPlan subscriptionplan = db.SubscriptionPlans.Find(id);
            if (subscriptionplan == null)
            {
                return HttpNotFound();
            }
            return View(subscriptionplan);
        }

        // GET: /SubscriptionPlan/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /SubscriptionPlan/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Fees,NoOfDays,NoOfCoupens,ServiceTax,ExtraTax,StartDate,IsActive")] SubscriptionPlan subscriptionplan)
        {
            try
            {
                subscriptionplan.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                subscriptionplan.CreateDate = DateTime.UtcNow.AddHours(5.3);
                if (ModelState.IsValid)
                {
                    db.SubscriptionPlans.Add(subscriptionplan);
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
                return View(subscriptionplan);
            }
            return View(subscriptionplan);
        }

        // GET: /SubscriptionPlan/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubscriptionPlan subscriptionplan = db.SubscriptionPlans.Find(id);
            if (subscriptionplan == null)
            {
                return HttpNotFound();
            }
            return View(subscriptionplan);
        }

        // POST: /SubscriptionPlan/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Fees,NoOfDays,NoOfCoupens,ServiceTax,ExtraTax,StartDate,IsActive")] SubscriptionPlan subscriptionplan)
        {
            try
            {
                EzeeloDBContext db1 = new EzeeloDBContext();
                SubscriptionPlan lSubscriptionPlan = db1.SubscriptionPlans.Find(subscriptionplan.ID);
                subscriptionplan.CreateBy = lSubscriptionPlan.CreateBy;
                subscriptionplan.CreateDate = lSubscriptionPlan.CreateDate;
                subscriptionplan.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                subscriptionplan.ModifyDate = DateTime.UtcNow.AddHours(5.3);


                //if (subscriptionplan.Name.Equals(lSubscriptionPlan.Name))
                //{
                //    throw new Exception("SubscriptionPlan Name : " + lSubscriptionPlan.Name + " already exist.");
                //}
                db1.Dispose();
                if (ModelState.IsValid)
                {
                    db.Entry(subscriptionplan).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                return View(subscriptionplan);
            }
            return View(subscriptionplan);
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
