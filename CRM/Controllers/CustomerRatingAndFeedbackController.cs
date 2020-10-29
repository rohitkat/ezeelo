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
using CRM.Models.ViewModel;
using System.Collections;
using CRM.Models;

namespace CRM.Controllers
{
    public class CustomerRatingAndFeedbackController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private CustomerCareSessionViewModel customerCareSessionViewModel = new CustomerCareSessionViewModel();
        private int pageSize = 10;

        public void SessionDetails()
        {
            customerCareSessionViewModel.UserLoginID = Convert.ToInt64(Session["ID"]);
            customerCareSessionViewModel.Username = Session["UserName"].ToString();
            Common.Common.GetAllLoginDetailFromSession(ref customerCareSessionViewModel);
        }

        [SessionExpire]
        // GET: /CustomerRatingAndFeedback/
        public ActionResult Index(int? page, int ReviewID = (int)Common.Constant.REVIEW_TYPE.PENDING, string FromDate = "", string ToDate = "", string SearchString = "")
        {
            SessionDetails();
            int pageNumber = (page ?? 1);
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.SearchString = SearchString;
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;

            //var customerratingandfeedbacks = db.CustomerRatingAndFeedbacks.Include(c => c.PersonalDetail).Include(c => c.PersonalDetail1).ToList();

            var customerratingandfeedbacks = (from PRO in db.Products
                                              join CRF in db.CustomerRatingAndFeedbacks on PRO.ID equals CRF.OwnerID
                                              join RAT in db.Ratings on CRF.RatingID equals RAT.ID
                                              where RAT.Name.ToUpper() == ((string)Common.Constant.RATING_NAME_FOR_PRODUCT).ToUpper()
                                              select new ReviewIndexViewModel
                                              {
                                                  ID = CRF.ID,
                                                  PersonalDetailID = CRF.PersonalDetailID,
                                                  OwnerID = CRF.OwnerID,
                                                  RatingID = CRF.RatingID,
                                                  Point = CRF.Point,
                                                  Feedback = CRF.Feedback,
                                                  IsApproved = CRF.IsApproved,
                                                  ApprovedBy = CRF.ApprovedBy,
                                                  CreateDate = CRF.CreateDate,
                                                  CreateBy = CRF.CreateBy,
                                                  ModifyDate = CRF.ModifyDate,
                                                  ModifyBy = CRF.ModifyBy,
                                                  NetworkIP = CRF.NetworkIP,
                                                  DeviceType = CRF.DeviceType,
                                                  DeviceID = CRF.DeviceID,
                                                  ProductName = PRO.Name,
                                                  ProductID = PRO.ID
                                              }).Distinct().ToList();


            //------------------------------------Code for get count of all order status------------------------------------//

            if ((FromDate != null && FromDate != "") || (ToDate != null && ToDate != ""))
            {
                DateTime lFromDate = DateTime.Now;
                if (DateTime.TryParse(FromDate, out lFromDate)) { }

                DateTime lToDate = DateTime.Now;
                if (DateTime.TryParse(ToDate, out lToDate)) { }

                customerratingandfeedbacks = customerratingandfeedbacks.Where(x => x.CreateDate.Date >= lFromDate.Date && x.CreateDate.Date <= lToDate.Date).ToList();
            }

            int lNewReview = customerratingandfeedbacks.Count(x => x.IsApproved == false && x.ApprovedBy == null);
            int lReviweApproved = customerratingandfeedbacks.Count(x => x.IsApproved == true);
            int lReviweRejected = customerratingandfeedbacks.Count(x => x.IsApproved == false && x.ApprovedBy != null);


            Dictionary<string, int> lReviewStatus = new Dictionary<string, int>();
            lReviewStatus.Add("ReviweApproved", lReviweApproved);
            lReviewStatus.Add("ReviweRejected", lReviweRejected);
            lReviewStatus.Add("NewReview", lNewReview);
            ViewBag.ReviewStatus = lReviewStatus;

            if (ReviewID == 1)
            {
                customerratingandfeedbacks = customerratingandfeedbacks.Where(x => x.IsApproved == false && x.ApprovedBy == null).ToList();
            }
            if (ReviewID == 2)
            {
                customerratingandfeedbacks = customerratingandfeedbacks.Where(x => x.IsApproved == true).ToList();
            }
            if (ReviewID == 3)
            {
                customerratingandfeedbacks = customerratingandfeedbacks.Where(x => x.IsApproved == false && x.ApprovedBy != null).ToList();
            }

            return View(customerratingandfeedbacks.ToPagedList(pageNumber, pageSize));

            // return View(customerratingandfeedbacks.ToList());
        }

        [SessionExpire]
        // GET: /CustomerRatingAndFeedback/Details/5
        public ActionResult Details(int? id, string Product)
        {
            SessionDetails();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerRatingAndFeedback customerratingandfeedback = db.CustomerRatingAndFeedbacks.Find(id);
            if (customerratingandfeedback == null)
            {
                return HttpNotFound();
            }
            return View(customerratingandfeedback);
        }

        [SessionExpire]
        // GET: /CustomerRatingAndFeedback/Edit/5
        public ActionResult Edit(int? id, string Product)
        {
            SessionDetails();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerRatingAndFeedback customerratingandfeedback = db.CustomerRatingAndFeedbacks.Find(id);
            if (customerratingandfeedback == null)
            {
                return HttpNotFound();
            }
            ViewBag.Product = Product;
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", customerratingandfeedback.CreateBy);
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", customerratingandfeedback.ModifyBy);
            return View(customerratingandfeedback);
        }

        // POST: /CustomerRatingAndFeedback/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [SessionExpire]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string Product, [Bind(Include = "ID,IsApproved,ApprovedBy")] CustomerRatingAndFeedback customerratingandfeedback)
        {
            SessionDetails();
            try
            {
                EzeeloDBContext db1 = new EzeeloDBContext();
                CustomerRatingAndFeedback lCustomerRatingAndFeedback = db1.CustomerRatingAndFeedbacks.Find(customerratingandfeedback.ID);
                customerratingandfeedback.PersonalDetailID = lCustomerRatingAndFeedback.PersonalDetailID;
                customerratingandfeedback.OwnerID = lCustomerRatingAndFeedback.OwnerID;
                customerratingandfeedback.RatingID = lCustomerRatingAndFeedback.RatingID;
                customerratingandfeedback.Feedback = lCustomerRatingAndFeedback.Feedback;
                customerratingandfeedback.Feedback = lCustomerRatingAndFeedback.Feedback;
                customerratingandfeedback.CreateDate = lCustomerRatingAndFeedback.CreateDate;
                customerratingandfeedback.CreateBy = lCustomerRatingAndFeedback.CreateBy;
                customerratingandfeedback.Point = lCustomerRatingAndFeedback.Point;
                customerratingandfeedback.ModifyDate = DateTime.Now;
                customerratingandfeedback.ModifyBy = customerCareSessionViewModel.PersonalDetailID;
                customerratingandfeedback.ApprovedBy = customerCareSessionViewModel.PersonalDetailID;
                db1.Dispose();
                if (ModelState.IsValid)
                {
                    db.Entry(customerratingandfeedback).State = EntityState.Modified;
                    db.SaveChanges();
                    //return View("Details", customerratingandfeedback);
                    return RedirectToAction("Details", new { id = customerratingandfeedback.ID, Product = Product });
                }
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", customerratingandfeedback.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", customerratingandfeedback.ModifyBy);
                return View(customerratingandfeedback);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's something wrong with the customer rating and feedback values!");

                //Code to write error log
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CustomerRatingAndFeedback][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.CRM, System.Web.HttpContext.Current.Server);

                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", customerratingandfeedback.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", customerratingandfeedback.ModifyBy);
                return View(customerratingandfeedback);
            }

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
