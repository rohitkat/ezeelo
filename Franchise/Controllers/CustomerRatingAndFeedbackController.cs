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
using Franchise.Models.ViewModel;
using System.Collections;
using Franchise.Models;

namespace Franchise.Controllers
{
    public class CustomerRatingAndFeedbackController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private CustomerCareSessionViewModel customerCareSessionViewModel = new CustomerCareSessionViewModel();
        private int pageSize = 10;        
        public void SessionDetails()
        {
            try
            {
                customerCareSessionViewModel.UserLoginID = Convert.ToInt64(Session["ID"]);
                customerCareSessionViewModel.Username = Session["USER_NAME"].ToString();
                customerCareSessionViewModel.PersonalDetailID = Convert.ToInt64(Session["PERSONAL_ID"]);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[SessionDetails]", "Can't assign Session Details..!" + Environment.NewLine + myEx.Message);
            }
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "CustomerRatingAndFeedback/CanRead")]
        // GET: /CustomerRatingAndFeedback/
        public ActionResult Index(int? page, int ReviewID = (int)Common.Constant.REVIEW_TYPE.PENDING, string FromDate = "", string ToDate = "", string SearchString = "")
        {
            SessionDetails();
            int franchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);
            int pageNumber = (page ?? 1);
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.SearchString = SearchString;
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;

            //var customerratingandfeedbacks = db.CustomerRatingAndFeedbacks.Include(c => c.PersonalDetail).Include(c => c.PersonalDetail1).ToList();

            var customerratingandfeedbacks = (from SP in db.ShopProducts
                                              join S in db.Shops on SP.ShopID equals S.ID
                                              join CRF in db.CustomerRatingAndFeedbacks on SP.ProductID equals CRF.OwnerID
                                              join RAT in db.Ratings on CRF.RatingID equals RAT.ID
                                              where RAT.Name.ToUpper() == ((string)Common.Constant.RATING_NAME_FOR_PRODUCT).ToUpper() && S.FranchiseID == franchiseID
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
                                                  ProductName = SP.Product.Name,
                                                  ProductID = SP.ProductID
                                              }).Distinct().ToList();


            //------------------------------------Code for get count of all order status------------------------------------//

            if ((FromDate != null && FromDate != "") || (ToDate != null && ToDate != ""))
            {
                DateTime lFromDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(FromDate);
                DateTime lToDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(ToDate);

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

            return View(customerratingandfeedbacks.OrderByDescending(x=>x.CreateDate).ToPagedList(pageNumber, pageSize));

            // return View(customerratingandfeedbacks.ToList());
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "CustomerRatingAndFeedback/CanRead")]
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
        [CustomAuthorize(Roles = "CustomerRatingAndFeedback/CanRead")]
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
        [CustomAuthorize(Roles = "CustomerRatingAndFeedback/CanWrite")]
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
