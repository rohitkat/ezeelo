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
    public class DeliveryDetailLogController : Controller
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
        public ActionResult Index(int DeliveryOrderDetailID)
        {
            SessionDetails();
            var deliverydetaillogs = db.DeliveryDetailLogs.Include(d => d.DeliveryOrderDetail).Include(d => d.PersonalDetail).Include(d => d.PersonalDetail1).Where(x => x.DeliveryOrderDetailID == DeliveryOrderDetailID).ToList().OrderByDescending(x => x.ID);
            //if (SearchString != "")
            //{
            //    return View(deliverydetaillogs.Where(x => xsdds.ToString() == SearchString).ToPagedList(pageNumber, pageSize));

            //}
            return View(deliverydetaillogs.ToList());
        }

        // GET: /DeliveryDetailLog/Details/5
        [SessionExpire]
        public ActionResult Details(long? id)
        {
            SessionDetails();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeliveryDetailLog deliverydetaillog = db.DeliveryDetailLogs.Find(id);
            if (deliverydetaillog == null)
            {
                return HttpNotFound();
            }
            return View(deliverydetaillog);
        }

        // GET: /DeliveryDetailLog/Create
        [SessionExpire]
        public ActionResult Create(int DeliveryOrderDetailID)
        {
            SessionDetails();
            ViewBag.DeliveryOrderDetailID = new SelectList(db.DeliveryOrderDetails, "ID", "ShopOrderCode");
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
            return View();
        }

        // POST: /DeliveryDetailLog/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        public ActionResult Create([Bind(Include = "ID,DeliveryOrderDetailID,Description")] DeliveryDetailLog deliverydetaillog, int DeliveryOrderDetailID)
        //public ActionResult Create(string Description, int DeliveryOrderDetailID)
        {
            SessionDetails();
            try
            {
                deliverydetaillog.CreateDate = DateTime.Now;
                deliverydetaillog.CreateBy = deliveryPartnerSessionViewModel.PersonalDetailID;
                deliverydetaillog.DeliveryOrderDetailID = DeliveryOrderDetailID;
                if (ModelState.IsValid)
                {
                    db.DeliveryDetailLogs.Add(deliverydetaillog);
                    db.SaveChanges();
                    return RedirectToAction("Index", new { DeliveryOrderDetailID = deliverydetaillog.DeliveryOrderDetailID });
                }

                ViewBag.DeliveryOrderDetailID = new SelectList(db.DeliveryOrderDetails, "ID", "ShopOrderCode", deliverydetaillog.DeliveryOrderDetailID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliverydetaillog.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliverydetaillog.ModifyBy);
                return View(deliverydetaillog);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's something wrong with the delivery detail log values!");

                //Code to write error log
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DeliveryDetailLog][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.DeliveryPartner, System.Web.HttpContext.Current.Server);

                ViewBag.DeliveryOrderDetailID = new SelectList(db.DeliveryOrderDetails, "ID", "ShopOrderCode", deliverydetaillog.DeliveryOrderDetailID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliverydetaillog.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliverydetaillog.ModifyBy);
                return View(deliverydetaillog);
            }
        }

        // GET: /DeliveryDetailLog/Edit/5
        [SessionExpire]
        public ActionResult Edit(long? id)
        {
            SessionDetails();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeliveryDetailLog deliverydetaillog = db.DeliveryDetailLogs.Find(id);
            if (deliverydetaillog == null)
            {
                return HttpNotFound();
            }
            ViewBag.DeliveryOrderDetailID = new SelectList(db.DeliveryOrderDetails, "ID", "ShopOrderCode", deliverydetaillog.DeliveryOrderDetailID);
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliverydetaillog.CreateBy);
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliverydetaillog.ModifyBy);
            return View(deliverydetaillog);
        }

        // POST: /DeliveryDetailLog/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        public ActionResult Edit([Bind(Include="ID,DeliveryOrderDetailID,Description,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] DeliveryDetailLog deliverydetaillog)
        {
            SessionDetails();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(deliverydetaillog).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.DeliveryOrderDetailID = new SelectList(db.DeliveryOrderDetails, "ID", "ShopOrderCode", deliverydetaillog.DeliveryOrderDetailID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliverydetaillog.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliverydetaillog.ModifyBy);
                return View(deliverydetaillog);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's something wrong with the delivery detail log values!");

                //Code to write error log
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DeliveryDetailLog][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.DeliveryPartner, System.Web.HttpContext.Current.Server);

                ViewBag.DeliveryOrderDetailID = new SelectList(db.DeliveryOrderDetails, "ID", "ShopOrderCode", deliverydetaillog.DeliveryOrderDetailID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliverydetaillog.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliverydetaillog.ModifyBy);
                return View(deliverydetaillog);
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
