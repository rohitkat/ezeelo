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
    public class DeliveryWeightSlabController : Controller
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

            var deliveryweightslabs = db.DeliveryWeightSlabs.Include(d => d.DeliveryPartner).Include(d => d.PersonalDetail).Include(d => d.PersonalDetail1).Include(d => d.PersonalDetail2).Where(x => x.DeliveryPartnerID == deliveryPartnerSessionViewModel.DeliveryPartnerID).ToList().OrderBy(x => x.MaxWeight);
            if (SearchString != "")
            {
                return View(deliveryweightslabs.Where(x => x.MaxWeight.ToString() == SearchString).ToPagedList(pageNumber, pageSize));

            }
            return View(deliveryweightslabs.ToPagedList(pageNumber, pageSize));
        }

        // GET: /DeliveryWeightSlab/Details/5
        [SessionExpire]
        public ActionResult Details(int? id)
        {
            SessionDetails();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeliveryWeightSlab deliveryweightslab = db.DeliveryWeightSlabs.Find(id);
            if (deliveryweightslab == null)
            {
                return HttpNotFound();
            }
            if (deliveryweightslab.DeliveryPartnerID != deliveryPartnerSessionViewModel.DeliveryPartnerID)
            {
                return View("AccessDenied");
            }
            return PartialView("_Details", deliveryweightslab);
            //return View(deliveryweightslab);
        }

        // GET: /DeliveryWeightSlab/Create
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

        // POST: /DeliveryWeightSlab/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        public ActionResult Create([Bind(Include="ID,MaxWeight,NormalRateWithinPincodeList,ExpressRateWithinPincodeList,ExpressRateOutOfPincodeList,NormalRateOutOfPincodeList,IsActive")] DeliveryWeightSlab deliveryweightslab)
        {
            SessionDetails();
            try 
            {
                deliveryweightslab.CreateDate = DateTime.Now;
                deliveryweightslab.CreateBy = deliveryPartnerSessionViewModel.PersonalDetailID;
                deliveryweightslab.DeliveryPartnerID = deliveryPartnerSessionViewModel.DeliveryPartnerID;
                deliveryweightslab.IsApproved = false;
                deliveryweightslab.ApprovedBy = null;
                if (ModelState.IsValid)
                {
                    db.DeliveryWeightSlabs.Add(deliveryweightslab);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.DeliveryPartnerID = new SelectList(db.DeliveryPartners, "ID", "GodownAddress", deliveryweightslab.DeliveryPartnerID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliveryweightslab.CreateBy);
                ViewBag.ApprovedBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliveryweightslab.ApprovedBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliveryweightslab.ModifyBy);
                return View(deliveryweightslab);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's something wrong with the delivery weight slab values!");

                //Code to write error log
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DeliveryWeightSlab][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.DeliveryPartner, System.Web.HttpContext.Current.Server);

                ViewBag.DeliveryPartnerID = new SelectList(db.DeliveryPartners, "ID", "GodownAddress", deliveryweightslab.DeliveryPartnerID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliveryweightslab.CreateBy);
                ViewBag.ApprovedBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliveryweightslab.ApprovedBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliveryweightslab.ModifyBy);
                return View(deliveryweightslab);
            }
        }

        // GET: /DeliveryWeightSlab/Edit/5
        [SessionExpire]
        public ActionResult Edit(int? id)
        {
            SessionDetails();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeliveryWeightSlab deliveryweightslab = db.DeliveryWeightSlabs.Find(id);
            if (deliveryweightslab == null)
            {
                return HttpNotFound();
            }
            if (deliveryweightslab.DeliveryPartnerID != deliveryPartnerSessionViewModel.DeliveryPartnerID)
            {
                return View("AccessDenied");
            }
            ViewBag.DeliveryPartnerID = new SelectList(db.DeliveryPartners, "ID", "GodownAddress", deliveryweightslab.DeliveryPartnerID);
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliveryweightslab.CreateBy);
            ViewBag.ApprovedBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliveryweightslab.ApprovedBy);
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliveryweightslab.ModifyBy);
            return View(deliveryweightslab);
        }

        // POST: /DeliveryWeightSlab/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        public ActionResult Edit([Bind(Include = "ID,MaxWeight,NormalRateWithinPincodeList,ExpressRateWithinPincodeList,ExpressRateOutOfPincodeList,NormalRateOutOfPincodeList,IsActive")] DeliveryWeightSlab deliveryweightslab)
        {
            SessionDetails();
            try
            {
                EzeeloDBContext db1 = new EzeeloDBContext();
                DeliveryWeightSlab lDeliveryWeightSlab = db1.DeliveryWeightSlabs.Find(deliveryweightslab.ID);
                deliveryweightslab.CreateBy = lDeliveryWeightSlab.CreateBy;
                deliveryweightslab.CreateDate = lDeliveryWeightSlab.CreateDate;
                deliveryweightslab.ModifyBy = deliveryPartnerSessionViewModel.PersonalDetailID;
                deliveryweightslab.ModifyDate = DateTime.Now;
                deliveryweightslab.ApprovedBy = null;
                deliveryweightslab.IsApproved = false;
                deliveryweightslab.DeliveryPartnerID = lDeliveryWeightSlab.DeliveryPartnerID;
                db1.Dispose();
                if (ModelState.IsValid)
                {
                    db.Entry(deliveryweightslab).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.DeliveryPartnerID = new SelectList(db.DeliveryPartners, "ID", "GodownAddress", deliveryweightslab.DeliveryPartnerID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliveryweightslab.CreateBy);
                ViewBag.ApprovedBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliveryweightslab.ApprovedBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliveryweightslab.ModifyBy);
                return View(deliveryweightslab);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's something wrong with the delivery weight slab values!");

                //Code to write error log
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DeliveryWeightSlab][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.DeliveryPartner, System.Web.HttpContext.Current.Server);

                ViewBag.DeliveryPartnerID = new SelectList(db.DeliveryPartners, "ID", "GodownAddress", deliveryweightslab.DeliveryPartnerID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliveryweightslab.CreateBy);
                ViewBag.ApprovedBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliveryweightslab.ApprovedBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliveryweightslab.ModifyBy);
                return View(deliveryweightslab);
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
