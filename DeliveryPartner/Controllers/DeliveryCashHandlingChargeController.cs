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
using DeliveryPartner.Models.ViewModel;
using System.Collections;
using DeliveryPartner.Models;


namespace DeliveryPartner.Controllers
{
    public class DeliveryCashHandlingChargeController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private DeliveryPartnerSessionViewModel deliveryPartnerSessionViewModel = new DeliveryPartnerSessionViewModel();
        private int pageSize = 10;

        public void SessionDetails()
        {
            deliveryPartnerSessionViewModel.UserLoginID = Convert.ToInt64(Session["ID"]);
            deliveryPartnerSessionViewModel.Username = Session["UserName"].ToString();
            Common.Common.GetAllLoginDetailFromSession(ref deliveryPartnerSessionViewModel);
        }
        [SessionExpire]
        public ActionResult Index(int? page, string SearchString = "")
        {
            SessionDetails();
            int pageNumber = (page ?? 1);
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.SearchString = SearchString;
            var deliverycashhandlingcharges = db.DeliveryCashHandlingCharges.Include(d => d.DeliveryPartner).Include(d => d.PersonalDetail).Include(d => d.PersonalDetail1).Include(d => d.PersonalDetail2).Where(x => x.DeliveryPartnerID == deliveryPartnerSessionViewModel.DeliveryPartnerID).ToList().OrderBy(x => x.MaxAmount);
            
            if (SearchString != "")
            {
                return View(deliverycashhandlingcharges.Where(x => x.MaxAmount.ToString() == SearchString).ToPagedList(pageNumber, pageSize));

            }
            return View(deliverycashhandlingcharges.ToPagedList(pageNumber, pageSize));
        }

        // GET: /DeliveryCashHandlingCharge/Details/5
        [SessionExpire]
        public ActionResult Details(long? id)
        {
            SessionDetails();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DeliveryCashHandlingCharge deliverycashhandlingcharge = db.DeliveryCashHandlingCharges.Find(id);
            if (deliverycashhandlingcharge == null)
            {
                return HttpNotFound();
            }
            if (deliverycashhandlingcharge.DeliveryPartnerID != deliveryPartnerSessionViewModel.DeliveryPartnerID)
            {
                return View("AccessDenied");
            }
            return PartialView("_Details", deliverycashhandlingcharge);
        }

        // GET: /DeliveryCashHandlingCharge/Create
        [SessionExpire]
        public ActionResult Create()
        {
            SessionDetails();
            ViewBag.DeliveryPartnerID = new SelectList(db.DeliveryPartners, "ID", "GodownAddress");
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
            ViewBag.ApprovedBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
            return View();
        }

        // POST: /DeliveryCashHandlingCharge/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        public ActionResult Create([Bind(Include="ID,MaxAmount,PerHourCharge,IsActive")] DeliveryCashHandlingCharge deliverycashhandlingcharge)
        {
            SessionDetails();
            try
            {
                deliverycashhandlingcharge.CreateDate = DateTime.Now;
                deliverycashhandlingcharge.CreateBy = deliveryPartnerSessionViewModel.PersonalDetailID;
                deliverycashhandlingcharge.DeliveryPartnerID = deliveryPartnerSessionViewModel.DeliveryPartnerID;
                deliverycashhandlingcharge.IsApproved = false;
                deliverycashhandlingcharge.ApprovedBy = null;
                if (ModelState.IsValid)
                {
                    db.DeliveryCashHandlingCharges.Add(deliverycashhandlingcharge);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.DeliveryPartnerID = new SelectList(db.DeliveryPartners, "ID", "GodownAddress", deliverycashhandlingcharge.DeliveryPartnerID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliverycashhandlingcharge.CreateBy);
                ViewBag.ApprovedBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliverycashhandlingcharge.ApprovedBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliverycashhandlingcharge.ModifyBy);
                return View(deliverycashhandlingcharge);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the delivery cash handling charge values!");

                //Code to write error log
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DeliveryCashHandlingCharge][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.DeliveryPartner, System.Web.HttpContext.Current.Server);


                ViewBag.DeliveryPartnerID = new SelectList(db.DeliveryPartners, "ID", "GodownAddress", deliverycashhandlingcharge.DeliveryPartnerID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliverycashhandlingcharge.CreateBy);
                ViewBag.ApprovedBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliverycashhandlingcharge.ApprovedBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliverycashhandlingcharge.ModifyBy);
                return View(deliverycashhandlingcharge);
            }
        }

        // GET: /DeliveryCashHandlingCharge/Edit/5
        [SessionExpire]
        public ActionResult Edit(long? id)
        {
            SessionDetails();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeliveryCashHandlingCharge deliverycashhandlingcharge = db.DeliveryCashHandlingCharges.Find(id);
            if (deliverycashhandlingcharge == null)
            {
                return HttpNotFound();
            }
            if (deliverycashhandlingcharge.DeliveryPartnerID != deliveryPartnerSessionViewModel.DeliveryPartnerID)
            {
                return View("AccessDenied");
            }

            ViewBag.DeliveryPartnerID = new SelectList(db.DeliveryPartners, "ID", "GodownAddress", deliverycashhandlingcharge.DeliveryPartnerID);
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliverycashhandlingcharge.CreateBy);
            ViewBag.ApprovedBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliverycashhandlingcharge.ApprovedBy);
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliverycashhandlingcharge.ModifyBy);
            return View(deliverycashhandlingcharge);
        }

        // POST: /DeliveryCashHandlingCharge/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        public ActionResult Edit([Bind(Include="ID,DeliveryPartnerID,MaxAmount,PerHourCharge,IsActive")] DeliveryCashHandlingCharge deliverycashhandlingcharge)
        {
            SessionDetails();
            try
            {
                EzeeloDBContext db1 = new EzeeloDBContext();
                DeliveryCashHandlingCharge lDeliveryCashHandlingCharge = db1.DeliveryCashHandlingCharges.Find(deliverycashhandlingcharge.ID);
                deliverycashhandlingcharge.CreateBy = lDeliveryCashHandlingCharge.CreateBy;
                deliverycashhandlingcharge.CreateDate = lDeliveryCashHandlingCharge.CreateDate;
                deliverycashhandlingcharge.ModifyBy = deliveryPartnerSessionViewModel.PersonalDetailID;
                deliverycashhandlingcharge.ModifyDate = DateTime.Now;
                deliverycashhandlingcharge.IsApproved = false;
                deliverycashhandlingcharge.ApprovedBy = null;
                deliverycashhandlingcharge.DeliveryPartnerID = lDeliveryCashHandlingCharge.DeliveryPartnerID;
                db1.Dispose();
                if (ModelState.IsValid)
                {
                    db.Entry(deliverycashhandlingcharge).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.DeliveryPartnerID = new SelectList(db.DeliveryPartners, "ID", "GodownAddress", deliverycashhandlingcharge.DeliveryPartnerID);
                ViewBag.ApprovedBy = new SelectList(db.UserLogins, "ID", "Mobile", deliverycashhandlingcharge.ApprovedBy);
                return View(deliverycashhandlingcharge);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the delivery cash handling charge values!");

                //Code to write error log
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DeliveryCashHandlingCharge][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.DeliveryPartner, System.Web.HttpContext.Current.Server);

                ViewBag.DeliveryPartnerID = new SelectList(db.DeliveryPartners, "ID", "GodownAddress", deliverycashhandlingcharge.DeliveryPartnerID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliverycashhandlingcharge.CreateBy);
                ViewBag.ApprovedBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliverycashhandlingcharge.ApprovedBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliverycashhandlingcharge.ModifyBy);
                return View(deliverycashhandlingcharge);
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
