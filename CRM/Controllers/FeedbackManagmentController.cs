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
using CRM.Models;
using CRM.Models.ViewModel;

namespace CRM.Controllers
{
    public class FeedbackManagmentController : Controller
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
        public ActionResult Index(string FromDate, string ToDate, int? page, int?FeedBackTypeID,  string SearchString = "", string FeedbackCategaryID = "")
        {
            SessionDetails();
            int pageNumber = (page ?? 1);
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;

            ViewBag.FeedbackCategaryID = new SelectList(db.FeedbackCategaries, "ID", "Name");
            ViewBag.FeedbackCategaryID1 = FeedbackCategaryID;

            ViewBag.FeedBackTypeID = new SelectList(db.FeedBackTypes, "ID", "Name");
            ViewBag.FeedBackTypeID1 = FeedBackTypeID;


            var feedbackmanagments = db.FeedbackManagments.Include(f => f.FeedbackCategary).Include(f => f.PersonalDetail).Include(f => f.PersonalDetail1).OrderByDescending(x => x.ID).ToList();

            if ((FromDate != null && FromDate != "") || (ToDate != null && ToDate != ""))
            {
                DateTime lFromDate = DateTime.Now;
                if (DateTime.TryParse(FromDate, out lFromDate)) { }

                DateTime lToDate = DateTime.Now;
                if (DateTime.TryParse(ToDate, out lToDate)) { }

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

            return View(feedbackmanagments.ToPagedList(pageNumber, pageSize));
        }

        [SessionExpire]
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
        public ActionResult Edit([Bind(Include="ID,FeedBackTypeID")] FeedbackManagment feedbackmanagment)
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
                feedbackmanagment.IsActive = true;
                db1.Dispose();
                //if (ModelState.IsValid)
                {
                    db.Entry(feedbackmanagment).State = EntityState.Modified;
                    db.SaveChanges();
                    return View("Details", feedbackmanagment);
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
