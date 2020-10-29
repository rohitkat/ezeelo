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
    public class SubscriptionPlanDealWithCategoryController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private int pageSize = 10;

        // GET: /SubscriptionPlanDealWithCategory/
        public ActionResult Index(int? page, string SearchString = "")
        {
            int pageNumber = (page ?? 1);
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.SearchString = SearchString;

            var subscriptionplandealwithcategories = db.SubscriptionPlanDealWithCategories.Include(s => s.SubscriptionPlan).ToList();
            if (SearchString != "")
            {
                subscriptionplandealwithcategories = subscriptionplandealwithcategories.Where(x => x.SubscriptionPlan.Name.Contains(SearchString)).ToList();
            }
            return View(subscriptionplandealwithcategories.ToPagedList(pageNumber, pageSize));
        }

        // GET: /SubscriptionPlanDealWithCategory/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubscriptionPlanDealWithCategory subscriptionplandealwithcategory = db.SubscriptionPlanDealWithCategories.Find(id);
            if (subscriptionplandealwithcategory == null)
            {
                return HttpNotFound();
            }
            return View(subscriptionplandealwithcategory);
        }

        // GET: /SubscriptionPlanDealWithCategory/Create
        public ActionResult Create()
        {
            ViewBag.SubscriptionPlanID = new SelectList(db.SubscriptionPlans, "ID", "Name");
            ViewBag.CategoryID = new SelectList(db.Categories.Where(x => x.Level == 1 && x.IsActive == true).ToList(), "ID", "Name");
            return View();
        }

        // POST: /SubscriptionPlanDealWithCategory/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="SubscriptionPlanID,CategoryID,MinimumAmount,DiscountInRs,DiscountInPer,IsActive")] SubscriptionPlanDealWithCategory subscriptionplandealwithcategory)
        {
            ViewBag.CategoryID = new SelectList(db.Categories.Where(x => x.Level == 1 && x.IsActive == true).ToList(), "ID", "Name");
            try
            {
                subscriptionplandealwithcategory.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                subscriptionplandealwithcategory.CreateDate = DateTime.UtcNow.AddHours(5.3);
                if (ModelState.IsValid)
                {
                    db.SubscriptionPlanDealWithCategories.Add(subscriptionplandealwithcategory);
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
                return View(subscriptionplandealwithcategory);
            }
            ViewBag.SubscriptionPlanID = new SelectList(db.SubscriptionPlans, "ID", "Name", subscriptionplandealwithcategory.SubscriptionPlanID);
            return View(subscriptionplandealwithcategory);
        }

        // GET: /SubscriptionPlanDealWithCategory/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubscriptionPlanDealWithCategory subscriptionplandealwithcategory = db.SubscriptionPlanDealWithCategories.Find(id);
            if (subscriptionplandealwithcategory == null)
            {
                return HttpNotFound();
            }
            ViewBag.SubscriptionPlanID = new SelectList(db.SubscriptionPlans, "ID", "Name", subscriptionplandealwithcategory.SubscriptionPlanID);
            ViewBag.CategoryID = new SelectList(db.Categories.Where(x => x.Level == 1 && x.IsActive == true).ToList(), "ID", "Name", subscriptionplandealwithcategory.CategoryID);
            return View(subscriptionplandealwithcategory);
        }

        // POST: /SubscriptionPlanDealWithCategory/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,SubscriptionPlanID,CategoryID,MinimumAmount,DiscountInRs,DiscountInPer,IsActive")] SubscriptionPlanDealWithCategory subscriptionplandealwithcategory)
        {
            try
            {
                EzeeloDBContext db1 = new EzeeloDBContext();
                SubscriptionPlanDealWithCategory lSubscriptionPlanDealWithCategory = db1.SubscriptionPlanDealWithCategories.Find(subscriptionplandealwithcategory.ID);
                subscriptionplandealwithcategory.CreateBy = lSubscriptionPlanDealWithCategory.CreateBy;
                subscriptionplandealwithcategory.CreateDate = lSubscriptionPlanDealWithCategory.CreateDate;
                subscriptionplandealwithcategory.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                subscriptionplandealwithcategory.ModifyDate = DateTime.UtcNow.AddHours(5.3);
                db1.Dispose();
                if (ModelState.IsValid)
                {
                    db.Entry(subscriptionplandealwithcategory).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ViewBag.SubscriptionPlanID = new SelectList(db.SubscriptionPlans, "ID", "Name", subscriptionplandealwithcategory.SubscriptionPlanID);
                ViewBag.CategoryID = new SelectList(db.Categories.Where(x => x.Level == 1 && x.IsActive == true).ToList(), "ID", "Name", subscriptionplandealwithcategory.CategoryID);
                return View(subscriptionplandealwithcategory);
            }
            ViewBag.SubscriptionPlanID = new SelectList(db.SubscriptionPlans, "ID", "Name", subscriptionplandealwithcategory.SubscriptionPlanID);
            ViewBag.CategoryID = new SelectList(db.Categories.Where(x => x.Level == 1 && x.IsActive == true).ToList(), "ID", "Name", subscriptionplandealwithcategory.CategoryID);
            return View(subscriptionplandealwithcategory);
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
