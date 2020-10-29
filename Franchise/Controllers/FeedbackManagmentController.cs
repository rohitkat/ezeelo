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
using Franchise.Models;
using Franchise.Models.ViewModel;

namespace Franchise.Controllers
{
    public class FeedbackManagmentController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private CustomerCareSessionViewModel customerCareSessionViewModel = new CustomerCareSessionViewModel();
        private int pageSize = 10;

        public void SessionDetails()
        {
            try
            {
                customerCareSessionViewModel.UserLoginID = Convert.ToInt64(Session["ID"]);
                customerCareSessionViewModel = (from fr in db.Franchises
                                                join bd in db.BusinessDetails on fr.BusinessDetailID equals bd.ID
                                                join bt in db.BusinessTypes on bd.BusinessTypeID equals bt.ID
                                                join ul in db.UserLogins on bd.UserLoginID equals ul.ID
                                                join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                                                where ul.ID == customerCareSessionViewModel.UserLoginID
                                                //&& bt.Name.Contains("GBFR")
                                                select new CustomerCareSessionViewModel
                                                {
                                                    UserLoginID = ul.ID,
                                                    BusinessTypeId = bt.ID,
                                                    Username = "",
                                                    PersonalDetailID = pd.ID,
                                                    BusinessDetailID = bd.ID,
                                                    DeliveryPartnerID = 0,
                                                    EmployeeCode = ""
                                                }).ToList().FirstOrDefault();
                customerCareSessionViewModel.Username = Session["USER_NAME"].ToString();
                customerCareSessionViewModel.PersonalDetailID = Convert.ToInt64(Session["PERSONAL_ID"]);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[SessionDetails]", "Can't assign Session Details..!" + Environment.NewLine + myEx.Message);
            }
        }

        /*Code Which return all feedback records*/
        //[SessionExpire]
        //[CustomAuthorize(Roles = "FeedbackManagment/CanRead")]
        //public ActionResult Index(string FromDate, string ToDate, int? page, int? FeedBackTypeID, string SearchString = "", string FeedbackCategaryID = "")
        //{
        //    SessionDetails();
        //    int pageNumber = (page ?? 1);
        //    ViewBag.PageNumber = pageNumber;
        //    ViewBag.PageSize = pageSize;
        //    ViewBag.FromDate = FromDate;
        //    ViewBag.ToDate = ToDate;

        //    ViewBag.FeedbackCategaryID = new SelectList(db.FeedbackCategaries, "ID", "Name");
        //    ViewBag.FeedbackCategaryID1 = FeedbackCategaryID;

        //    ViewBag.FeedBackTypeID = new SelectList(db.FeedBackTypes, "ID", "Name");
        //    ViewBag.FeedBackTypeID1 = FeedBackTypeID;

        //    List<FeedbackManagment> feedbackmanagments = new List<FeedbackManagment>();
        //    BusinessDetail lBusinessDetail = db.BusinessDetails.Find(customerCareSessionViewModel.BusinessDetailID);
        //    if (lBusinessDetail != null)
        //    {
        //        long lCityID = lBusinessDetail.Pincode.CityID;
        //        if (lCityID != null)
        //        {
        //            feedbackmanagments = db.FeedbackManagments.Include(f => f.FeedbackCategary).Include(f => f.PersonalDetail).Include(f => f.PersonalDetail1).OrderByDescending(x => x.ID).Where(x => x.CityID == lCityID).ToList();
        //        }
        //    }


        //    if ((FromDate != null && FromDate != "") || (ToDate != null && ToDate != ""))
        //    {
        //        DateTime lFromDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(FromDate);
        //        DateTime lToDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(ToDate);

        //        feedbackmanagments = feedbackmanagments.Where(x => x.CreateDate.Date >= lFromDate.Date && x.CreateDate.Date <= lToDate.Date).ToList();
        //    }
        //    if (FeedbackCategaryID != "")
        //    {
        //        feedbackmanagments = feedbackmanagments.Where(x => x.FeedbackCategaryID.ToString() == FeedbackCategaryID).ToList();
        //    }

        //    if (FeedBackTypeID != null)
        //    {
        //        feedbackmanagments = feedbackmanagments.Where(x => x.FeedBackTypeID == FeedBackTypeID).ToList();
        //    }

        //    return View(feedbackmanagments.ToPagedList(pageNumber, pageSize));
        //}


        [SessionExpire]
        [CustomAuthorize(Roles = "FeedbackManagment/CanRead")]
        public ActionResult Index(string FromDate, string ToDate, int? AllFeedbackpage, int? OrderWiseFeedbackpage, int? LookingForFeedbackpage, int? FeedBackTypeID, string SearchString = "", string FeedbackCategaryID = "")
        {
            SessionDetails();
            int AllFeedbackpageNumber = (AllFeedbackpage ?? 1);
            ViewBag.AllFeedbackPageNumber = AllFeedbackpageNumber;

            int OrderWiseFeedbackpageNumber = (OrderWiseFeedbackpage ?? 1);
            ViewBag.OrderWiseFeedbackPageNumber = OrderWiseFeedbackpageNumber;

            int LookingForFeedbackpageNumber = (LookingForFeedbackpage ?? 1);
            ViewBag.LookingForFeedbackPageNumber = LookingForFeedbackpageNumber;

            ViewBag.PageSize = pageSize;
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;

            ViewBag.FeedbackCategaryID = new SelectList(db.FeedbackCategaries, "ID", "Name");
            ViewBag.FeedbackCategaryID1 = FeedbackCategaryID;

            ViewBag.FeedBackTypeID = new SelectList(db.FeedBackTypes, "ID", "Name");
            ViewBag.FeedBackTypeID1 = FeedBackTypeID;


            FeedbackGroupWise lFeedbackGroupWise = new FeedbackGroupWise(); 



            List<FeedbackManagment> feedbackmanagments = new List<FeedbackManagment>();
            BusinessDetail lBusinessDetail = db.BusinessDetails.Find(customerCareSessionViewModel.BusinessDetailID);
            if (lBusinessDetail != null)
            {
                long lCityID = lBusinessDetail.Pincode.CityID;
                if (lCityID != null)
                {
                    feedbackmanagments = db.FeedbackManagments.Include(f => f.FeedbackCategary).Include(f => f.PersonalDetail).Include(f => f.PersonalDetail1).OrderByDescending(x => x.ID).Where(x => x.CityID == lCityID).ToList();
                }
            }


            if ((FromDate != null && FromDate != "") || (ToDate != null && ToDate != ""))
            {
                DateTime lFromDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(FromDate);
                DateTime lToDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(ToDate);

                feedbackmanagments = feedbackmanagments.Where(x => x.CreateDate.Date >= lFromDate.Date && x.CreateDate.Date <= lToDate.Date).ToList();
            }
            if (FeedbackCategaryID != "")
            {
                feedbackmanagments = feedbackmanagments.Where(x => x.FeedbackCategaryID.ToString() == FeedbackCategaryID).ToList();
            }

            if (FeedBackTypeID != null)
            {
                feedbackmanagments = feedbackmanagments.Where(x => x.FeedBackTypeID == FeedBackTypeID).ToList();
            }

            lFeedbackGroupWise.AllFeedback = feedbackmanagments.ToPagedList(AllFeedbackpageNumber, pageSize);
            lFeedbackGroupWise.OrderWiseFeedback = feedbackmanagments.Where(x => x.CustOrderCode != null).ToPagedList(OrderWiseFeedbackpageNumber, pageSize);
            lFeedbackGroupWise.ProductRequirementFeedback = feedbackmanagments.Where(x => x.FeedbackCategaryID == 6).ToPagedList(LookingForFeedbackpageNumber, pageSize);

           return View(lFeedbackGroupWise);
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "FeedbackManagment/CanRead")]
        // GET: /FeedbackManagment/Details/5
        public ActionResult Details(int? id)
        {
            SessionDetails();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeedbackManagment feedbackmanagment = db.FeedbackManagments.Find(id);
            if (feedbackmanagment == null)
            {
                return HttpNotFound();
            }
            return View(feedbackmanagment);
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "FeedbackManagment/CanRead")]
        // GET: /FeedbackManagment/Edit/5
        public ActionResult Edit(int? id)
        {
            SessionDetails();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeedbackManagment feedbackmanagment = db.FeedbackManagments.Find(id);
            if (feedbackmanagment == null)
            {
                return HttpNotFound();
            }
            ViewBag.FeedbackCategaryID = new SelectList(db.FeedbackCategaries, "ID", "Name", feedbackmanagment.FeedbackCategaryID);
            ViewBag.FeedBackTypeID = new SelectList(db.FeedBackTypes, "ID", "Name", feedbackmanagment.FeedBackTypeID);
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", feedbackmanagment.CreateBy);
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", feedbackmanagment.ModifyBy);
            return View(feedbackmanagment);
        }

        // POST: /FeedbackManagment/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [SessionExpire]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "FeedbackManagment/CanWrite")]
        public ActionResult Edit([Bind(Include = "ID,FeedBackTypeID,EmailSMSText")] FeedbackManagment feedbackmanagment)
        {
            SessionDetails();
            try
            {
                EzeeloDBContext db1 = new EzeeloDBContext();
                FeedbackManagment lFeedbackManagment = db1.FeedbackManagments.Find(feedbackmanagment.ID);
                feedbackmanagment.CreateDate = lFeedbackManagment.CreateDate;
                feedbackmanagment.CreateBy = lFeedbackManagment.CreateBy;
                feedbackmanagment.Email = lFeedbackManagment.Email;
                feedbackmanagment.Message = lFeedbackManagment.Message;
                feedbackmanagment.FeedbackCategaryID = lFeedbackManagment.FeedbackCategaryID;
                feedbackmanagment.ModifyDate = DateTime.Now;
                feedbackmanagment.ModifyBy = customerCareSessionViewModel.PersonalDetailID;
                feedbackmanagment.Mobile = lFeedbackManagment.Mobile;
                feedbackmanagment.CityID = lFeedbackManagment.CityID;
                feedbackmanagment.IsActive = true;
                //feedbackmanagment.EmailSMSText = lFeedbackManagment.EmailSMSText;
                db1.Dispose();

                if (feedbackmanagment.EmailSMSText!=null && feedbackmanagment.EmailSMSText != string.Empty)
                {
                    try
                    {
                        if (lFeedbackManagment.Email != null && lFeedbackManagment.Email != string.Empty)
                        {
                            Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();
                            dictEmailValues.Add("<!--TEXT-->", feedbackmanagment.EmailSMSText);
                            BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                            gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.FEED_CUST_FROM_ADMIN, new string[] { lFeedbackManagment.Email }, dictEmailValues, true);
                            ViewBag.Msg = "Your email has been sent";
                        }
                        else if (lFeedbackManagment.Mobile != null && lFeedbackManagment.Mobile != string.Empty)
                        {
                            Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();
                            dictSMSValues.Add("#--TEXT--#", feedbackmanagment.EmailSMSText);
                            BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                            gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.FEED_CUST_FROM_ADMIN, new string[] { lFeedbackManagment.Mobile }, dictSMSValues);
                            ViewBag.Msg = "Your message has been sent";
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("Error", "There's something wrong with semding email or sms!");

                        //Code to write error log
                        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                            + Environment.NewLine + ex.Message + Environment.NewLine
                            + "[FeedbackManagment][POST:Edit]",
                            BusinessLogicLayer.ErrorLog.Module.CRM, System.Web.HttpContext.Current.Server);
                    }
                }

                //if (ModelState.IsValid)+61
                {
                    db.Entry(feedbackmanagment).State = EntityState.Modified;
                    db.SaveChanges();
                    //return View("Details", feedbackmanagment);
                }
                ViewBag.FeedbackCategaryID = new SelectList(db.FeedbackCategaries, "ID", "Name", feedbackmanagment.FeedbackCategaryID);
                ViewBag.FeedBackTypeID = new SelectList(db.FeedBackTypes, "ID", "Name", feedbackmanagment.FeedBackTypeID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", feedbackmanagment.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", feedbackmanagment.ModifyBy);
                return View(feedbackmanagment);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's something wrong with the feedback values!");

                //Code to write error log
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FeedbackManagment][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.CRM, System.Web.HttpContext.Current.Server);

                ViewBag.FeedbackCategaryID = new SelectList(db.FeedbackCategaries, "ID", "Name", feedbackmanagment.FeedbackCategaryID);
                ViewBag.FeedBackTypeID = new SelectList(db.FeedBackTypes, "ID", "Name", feedbackmanagment.FeedBackTypeID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", feedbackmanagment.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", feedbackmanagment.ModifyBy);
                return View(feedbackmanagment);
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
