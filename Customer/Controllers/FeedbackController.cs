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
    public class FeedbackController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        #region Submit Rating

        // GET: /Feedback/
        public ActionResult Index()
        {
            var feedbackmanagments = db.FeedbackManagments.Include(f => f.FeedbackCategary).Include(f => f.FeedBackType).Include(f => f.PersonalDetail).Include(f => f.PersonalDetail1);
            return View(feedbackmanagments.ToList());
        }
       
        public ActionResult Create()
        {
            try
            {
                ViewBag.FeedbackCategaryID = new SelectList(db.FeedbackCategaries, "ID", "Name");
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's something wrong with loading value in feedback dropdown list!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[FeedbackController][GET:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's something wrong with loading value in feedback dropdown list!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FeedbackController][GET:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include="ID,Email,Mobile,FeedbackCategaryID,Message,FeedBackTypeID,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] FeedbackManagment feedbackmanagment)
        public ActionResult Create([Bind(Include="ID,Email,Mobile,FeedbackCategaryID,Message")] ModelLayer.Models.FeedbackManagment feedbackmanagment)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if(feedbackmanagment.Email==null && feedbackmanagment.Mobile==null)
                    {
                        ViewBag.FeedbackCategaryID = new SelectList(db.FeedbackCategaries, "ID", "Name");
                        TempData["message"] = "Please Provide Mobile No or Email ID";
                        return RedirectToRoute("FEEDBACK");
                    }
                    long cityId = 0;
                    int franchiseId = 0;////added
                    feedbackmanagment.IsActive = true;
                    feedbackmanagment.CreateDate = BusinessLogicLayer.CommonFunctions.GetLocalTime();
                    
                    if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null && ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value != string.Empty)
                    {
                        cityId = Convert.ToInt32(ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[0].Trim());
                        franchiseId = Convert.ToInt32(ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[2].Trim());////added 
                    }
                    feedbackmanagment.CityID = cityId;
                    feedbackmanagment.FranchiseID = franchiseId;////added
                    db.FeedbackManagments.Add(feedbackmanagment);
                    db.SaveChanges();
                    ViewBag.FeedbackCategaryID = new SelectList(db.FeedbackCategaries, "ID", "Name", feedbackmanagment.FeedbackCategaryID);
                   // return RedirectToAction("Index", "Home");
                    TempData["message"]= "Thank you for your valuable feedback!";
                    SendEmailAlertOnFeedback(feedbackmanagment);
                    this.SendEmailToCustomer(feedbackmanagment.Email);
                    //return RedirectToAction("Create");
                    return RedirectToRoute("FEEDBACK");
                }
                else
                {
                    if (feedbackmanagment.Message == null)
                    {
                        ViewBag.FeedbackCategaryID = new SelectList(db.FeedbackCategaries, "ID", "Name");
                        TempData["message"] = "Please fill the message";
                    }
                    else
                    {
                        ViewBag.FeedbackCategaryID = new SelectList(db.FeedbackCategaries, "ID", "Name");
                        TempData["message"] = "Please select feedback category";
                    }
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's something wrong with saving customer's feedback detail!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[FeedbackController][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's something wrong with saving customer's feedback detail!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FeedbackController][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            //return View(feedbackmanagment);
            return RedirectToRoute("FEEDBACK");
        }

        // GET: /Feedback/        
        public ActionResult Rating()
        {
            if (Session["UID"] == null)
            {
                return View("AccessDenied");
            }
            RatingViewModel rView = new RatingViewModel();
            try
            {
                List<CustomerRatingViewModel> lCustRatingList = new List<CustomerRatingViewModel>();           
                lCustRatingList = (from r in db.Ratings
                                   join bt in db.BusinessTypes on r.BusinessTypeID equals bt.ID
                                   where r.IsActive == true && bt.Prefix  == "GBOD"
                                   select new CustomerRatingViewModel
                                   {
                                       RatingID = r.ID,
                                       RatingName = r.Name

                                   }).ToList();
                if (lCustRatingList == null)
                {
                    return View("HttpError");
                }
                rView.MasterRating = lCustRatingList;
                rView.CustomerOrderID = GetLastDeliveredOrderID();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's something wrong with loading rating page!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[FeedbackController][GET:Rating]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's something wrong with loading rating page!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FeedbackController][GET:Rating]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View("Feedback", rView);
        }
        public ActionResult ProductRating(long ID)
        {
            //ID => ProductID
            ProductRatingViewModel productRating = new ProductRatingViewModel();
            if (Session["UID"] == null)
            {//Session Expired
                return RedirectToAction("Login", "Login", new { returnUrl = Request.Url.ToString() });
            }
            if(db.Products.Where(x=>x.ID == ID).FirstOrDefault() == null)
            {// Product Not Exists
                return View("AccessDenied");
            }
            else
            {
                BusinessLogicLayer.ProductDetails pD = new BusinessLogicLayer.ProductDetails(System.Web.HttpContext.Current.Server);
                productRating.ProductDetails = pD.GetBasicDetails(ID); 

            }
            
            try
            {
                List<CustomerRatingViewModel> lCustRatingList = new List<CustomerRatingViewModel>();
                lCustRatingList = (from r in db.Ratings
                                   join bt in db.BusinessTypes on r.BusinessTypeID equals bt.ID
                                   where r.IsActive == true && bt.Prefix == "GBPR"
                                   select new CustomerRatingViewModel
                                   {
                                       RatingID = r.ID,
                                       RatingName = r.Name
                                   }).ToList();
                if (lCustRatingList == null)
                {
                    return View("HttpError");
                }

                productRating.RatingList = lCustRatingList; 
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's something wrong with loading product rating page!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[FeedbackController][GET:ProductRating]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's something wrong with loading product rating page!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FeedbackController][GET:ProductRating]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View("ProductRating", productRating);
        }
        public ActionResult ShopRating(long ID,long prodId)
        {
            //ID => ShopID
            ShopRatingViewModel shopRating = new ShopRatingViewModel();

            if (Session["UID"] == null)
            {//Session Expired
                return RedirectToAction("Login", "Login", new { returnUrl = Request.Url.ToString() });
            }
            if (db.Shops.Where(x => x.ID == ID).FirstOrDefault() == null)
            {// Product Not Exists
                return View("AccessDenied");
            }
            else
            {
                BusinessLogicLayer.ShopDetails sD = new BusinessLogicLayer.ShopDetails(System.Web.HttpContext.Current.Server);
                shopRating.ShopDetails = sD.GetShopBasicDetails(ID);

            }            
            try
            {
                List<CustomerRatingViewModel> lCustRatingList = new List<CustomerRatingViewModel>();
                lCustRatingList = (from r in db.Ratings
                                   join bt in db.BusinessTypes on r.BusinessTypeID equals bt.ID
                                   where r.IsActive == true && bt.Prefix == "GBMR"
                                   select new CustomerRatingViewModel
                                   {
                                       RatingID = r.ID,
                                       RatingName = r.Name
                                   }).ToList();
                if (lCustRatingList == null)
                {
                    return View("HttpError");
                }
                shopRating.RatingList = lCustRatingList;
                shopRating.productId = prodId;
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's something wrong with loading product rating page!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[FeedbackController][GET:ShopRating]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's something wrong with loading product rating page!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FeedbackController][GET:ShopRating]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View("ShopRating", shopRating);
        }
        private long GetLastDeliveredOrderID()
        {
            return 30011;

        }
        public JsonResult SubmitRating(string ratings, string comment, long OwnerID)
        {
            int oprStatus = 0;
            System.Threading.Thread.Sleep(1000);
            try
            {

                //rating Id specifies the type of rating Shop, product, order etc...
                if (ratings.ToString().Length >= 3)
                {
                    string[] rateList = ratings.Split('$');
                    foreach (var item in rateList)
                    {
                        string[] idAndPoints = item.Split(':');
                        if (idAndPoints.Length != 2)
                            continue;
                        CustomerRatingAndFeedback custRate = new CustomerRatingAndFeedback();
                        custRate.PersonalDetailID = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["UID"]));
                        custRate.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["UID"]));
                        custRate.CreateDate = CommonFunctions.GetLocalTime();
                        custRate.IsApproved = false;
                        custRate.OwnerID = OwnerID;
                        custRate.Point = Convert.ToInt32(idAndPoints[1]);
                        custRate.RatingID = Convert.ToInt32(idAndPoints[0]);
                        custRate.Feedback = comment;
                        db.CustomerRatingAndFeedbacks.Add(custRate);
                        db.SaveChanges();
                        oprStatus = 101;
                    }
                }
            }
            catch (Exception ex)
            {
                oprStatus = 0;
               // throw;
            }           
            // = cw.RemoveFromWishlist(lCustID, lShopStockID);
            return Json(oprStatus.ToString(), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Display Rating

        public ActionResult Reviews(long ownerID, BusinessLogicLayer.Review.REVIEWS reviewfor)
        {
            BusinessLogicLayer.Review review = new Review();
            DisplayReviewsViewModel displayReviews = new DisplayReviewsViewModel();
            displayReviews = review.GetReviews(ownerID, reviewfor);
            if (reviewfor ==  Review.REVIEWS.PRODUCT) 
                return View("_ProductReviews",displayReviews);
            else
                return View("_ShopReviews", displayReviews);
        }


        #endregion


        public JsonResult LookingFor(string lookingFor, string lookingDetails, string nameOfPerson, string emailAddress, string mobile)
        {

            int oprStatus = 0;
            long cityID = 0;
            ModelLayer.Models.FeedbackManagment feedbackManagment = new ModelLayer.Models.FeedbackManagment();
            feedbackManagment.Email = emailAddress;
            feedbackManagment.Mobile = mobile;
            //FeedbackCategaryID = 6 For Product request
            feedbackManagment.FeedbackCategaryID = 6;
            feedbackManagment.Message = lookingFor + ", " + lookingDetails;
            feedbackManagment.IsActive = true;
            feedbackManagment.CreateDate = DateTime.UtcNow.AddHours(5.5);
            feedbackManagment.CreateBy = 1;
            feedbackManagment.FeedBackTypeID = 1;
            if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null)
            {
                string cookieValue = ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value;
                string[] arr = cookieValue.Split('$');
                cityID = Convert.ToInt64(arr[0]);
            }
            feedbackManagment.CityID = cityID;
            try
            {
                if (ModelState.IsValid)
                {
                    EzeeloDBContext db1 = new EzeeloDBContext();
                    db1.FeedbackManagments.Add(feedbackManagment);
                    db1.SaveChanges();

                    oprStatus = 1;
                    try
                    {
                        SendLookingForSMS(mobile);
                    }
                    catch (Exception ex)
                    {

                        BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in saving details of customer looking for :" + ex.Message + ex.InnerException, ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
                    }
                    try
                    {
                        SendLookingForMail(emailAddress, lookingFor);
                    }
                    catch (Exception ex)
                    {

                        BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in saving details of customer looking for :" + ex.Message + ex.InnerException, ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
                    }


                }

            }
            catch (Exception ex)
            {
                string msg = "";
                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        msg += error + "-";
                    }
                }
                oprStatus = 0;
                BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in saving details of customer looking for :" + ex.Message + ex.InnerException, ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

            }

            return Json(oprStatus.ToString(), JsonRequestBehavior.AllowGet);
        }

        //public JsonResult LookingFor(myParam myParam)//string lookingFor, string lookingDetails, string nameOfPerson, string emailAddress, string mobile)
        //{

        //    int oprStatus = 0;
        //    long cityID = 0;
        //    ModelLayer.Models.FeedbackManagment feedbackManagment = new ModelLayer.Models.FeedbackManagment();
        //    feedbackManagment.Email = myParam.emailAddress; //emailAddress;
        //    feedbackManagment.Mobile = myParam.mobile;
        //    //FeedbackCategaryID = 6 For Product request
        //    feedbackManagment.FeedbackCategaryID = 6;
        //    feedbackManagment.Message = myParam.lookingFor + ", " + myParam.lookingDetails;
        //    feedbackManagment.IsActive = true;
        //    feedbackManagment.CreateDate = DateTime.UtcNow.AddHours(5.5);
        //    feedbackManagment.CreateBy = 1;
        //    feedbackManagment.FeedBackTypeID = 1;
        //    if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null)
        //    {
        //        string cookieValue = ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value;
        //        string[] arr = cookieValue.Split('$');
        //        cityID = Convert.ToInt64(arr[0]);
        //    }
        //    feedbackManagment.CityID = cityID;
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            EzeeloDBContext db1 = new EzeeloDBContext();
        //            db1.FeedbackManagments.Add(feedbackManagment);
        //            db1.SaveChanges();

        //            oprStatus = 1;
        //            try
        //            {
        //                SendLookingForSMS(myParam.mobile);
        //            }
        //            catch (Exception ex)
        //            {

        //                BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in saving details of customer looking for :" + ex.Message + ex.InnerException, ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
        //            }
        //            try
        //            {
        //                SendLookingForMail(myParam.emailAddress, myParam.lookingFor);
        //            }
        //            catch (Exception ex)
        //            {

        //                BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in saving details of customer looking for :" + ex.Message + ex.InnerException, ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
        //            }


        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        string msg = "";
        //        foreach (ModelState modelState in ViewData.ModelState.Values)
        //        {
        //            foreach (ModelError error in modelState.Errors)
        //            {
        //                msg += error + "-";
        //            }
        //        }
        //        oprStatus = 0;
        //        BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in saving details of customer looking for :" + ex.Message + ex.InnerException, ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

        //    }

        //    return Json(oprStatus.ToString(), JsonRequestBehavior.AllowGet);
        //}
        private void SendLookingForMail(string emailId,string lookingFor)
        {
            try
            {
                Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();

                //dictEmailValues.Add("<!--ORDERS_URL-->", "http://www.ezeelo.com/CustomerOrder/MyOrders");
                dictEmailValues.Add("<!--NAME-->", lookingFor);
               
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);

                gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.CUST_LOOKING_FOR, new string[] { emailId, "crm@ezeelo.com", "sales@ezeelo.com" }, dictEmailValues, true);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void SendLookingForSMS(string mobileNo)
        {
            try
            {
                CommonFunctions cf = new CommonFunctions();

                Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();
                
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);

                gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.CUST_LOOKING_FOR, new string[] { mobileNo }, dictSMSValues);

            }
            catch (Exception ex)
            {
                throw;
            }
        }


        private void SendEmailAlertOnFeedback(ModelLayer.Models.FeedbackManagment feedbackManagment)
        {
            /* This method is added by Avi Verma.
             * Date : 26-Oct-2015.
             * As discussed with Mahesh Sir.. on date : 23-Oct-2015. 
             * When a customer gives feedback, it is reflected in CRM Module. 
             * But, the same feedback should also be send by mail to CRM person and respective employee, manager etc.
             */
            try
            {
                FeedbackCategary lFeedbackCategary = db.FeedbackCategaries.Find(feedbackManagment.FeedbackCategaryID);
                string lFeedbackCategoryName = "";
                if(lFeedbackCategary != null)
                {
                    lFeedbackCategoryName = lFeedbackCategary.Name;
                }

                string lPersonalDetailName = "";
                PersonalDetail lPersonalDetail = db.PersonalDetails.Find(feedbackManagment.CreateBy);
                if(lPersonalDetail != null)
                {
                    lPersonalDetailName = lPersonalDetail.FirstName;
                }

                Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();

                //dictEmailValues.Add("<!--ORDERS_URL-->", "http://www.ezeelo.com/CustomerOrder/MyOrders");
                dictEmailValues.Add("<!--Email-->", feedbackManagment.Email);
                dictEmailValues.Add("<!--Mobile-->", feedbackManagment.Mobile);
                dictEmailValues.Add("<!--Category-->", feedbackManagment.FeedbackCategary.Name);
                dictEmailValues.Add("<!--Message-->", feedbackManagment.Message);
                dictEmailValues.Add("<!--Type-->", lFeedbackCategoryName);
                dictEmailValues.Add("<!--CreatedDate-->", feedbackManagment.CreateDate.ToString());

                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);

                gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.FEEDBACK_MANAGEMENT, new string[] { "feedback@ezeelo.com", "crm@ezeelo.com" }, dictEmailValues, true);
            }
            catch (Exception)
            {

                throw;
            }

        }

        private void SendEmailToCustomer(string email)
        {
            try
            {

                Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.FEEDBACK_CUSTOMER, new string[] { email }, dictEmailValues, true);
            }
            catch (Exception)
            {

                throw;
            }

        }

    }
}

public class myParam
{
  public string  lookingFor;
  public string lookingDetails;
  public string nameOfPerson;
  public string emailAddress;
  public string mobile;
}