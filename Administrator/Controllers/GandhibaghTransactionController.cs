using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;

namespace Administrator.Controllers
{
    public class GandhibaghTransactionController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private int fUserId = -1;
        private int fUserTypeId = -1;
        private int pageSize = 10;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            /* This Methode Initialize feild fUserId and fUserTypeId
             * to current logged user's UserID and UserTypeID  
             */
            fUserId = 1;
            fUserTypeId = 1;
            if (requestContext.HttpContext.User.Identity.IsAuthenticated)
            {
                fUserId = 1;
                fUserTypeId = 1;
            }
        }

        // GET: /GandhibaghTransaction/
        public ActionResult Index()
        {
            var gandhibaghtransactions = db.GandhibaghTransactions.Include(g => g.BusinessType).Include(g => g.BusinessType1).Include(g => g.BusinessType2).Include(g => g.Charge).Include(g => g.CustomerOrderDetail).Include(g => g.PersonalDetail).Include(g => g.PersonalDetail1).Include(g => g.PersonalDetail2).Include(g => g.PersonalDetail3);
            return View(gandhibaghtransactions.ToList());
        }

        // GET: /GandhibaghTransaction/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GandhibaghTransaction gandhibaghtransaction = db.GandhibaghTransactions.Find(id);
            if (gandhibaghtransaction == null)
            {
                return HttpNotFound();
            }
            return View(gandhibaghtransaction);
        }

        // GET: /GandhibaghTransaction/Create
        public ActionResult Create()
        {
            ViewBag.ToBusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name");
            ViewBag.FromBusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name");
            ViewBag.ToBusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name");
            ViewBag.ChargeID = new SelectList(db.Charges, "ID", "Name");
            ViewBag.CustomerOrderDetailID = new SelectList(db.CustomerOrderDetails, "ID", "ShopOrderCode");
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
            ViewBag.FromPersonalDetailId = new SelectList(db.PersonalDetails, "ID", "FirstName");
            ViewBag.ToPersonalDetailID = new SelectList(db.PersonalDetails, "ID", "FirstName");
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
            return View();
        }

        // POST: /GandhibaghTransaction/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,TransactionID,ChargeID,ChargeType,FromBusinessTypeID,FromPersonalDetailId,ToBusinessTypeID,ToPersonalDetailID,ApplicablePercent,ApplicableRupee,Particular,CustomerOrderDetailID,Amount,TransactionAmount,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] GandhibaghTransaction gandhibaghtransaction)
        {
            try
            {
                gandhibaghtransaction.CreateDate = DateTime.Now;
                gandhibaghtransaction.CreateBy = fUserId;
                if (ModelState.IsValid)
                {
                    db.GandhibaghTransactions.Add(gandhibaghtransaction);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.ToBusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", gandhibaghtransaction.ToBusinessTypeID);
                ViewBag.FromBusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", gandhibaghtransaction.FromBusinessTypeID);
                ViewBag.ToBusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", gandhibaghtransaction.ToBusinessTypeID);
                ViewBag.ChargeID = new SelectList(db.Charges, "ID", "Name", gandhibaghtransaction.ChargeID);
                ViewBag.CustomerOrderDetailID = new SelectList(db.CustomerOrderDetails, "ID", "ShopOrderCode", gandhibaghtransaction.CustomerOrderDetailID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", gandhibaghtransaction.CreateBy);
                ViewBag.FromPersonalDetailId = new SelectList(db.PersonalDetails, "ID", "FirstName", gandhibaghtransaction.FromPersonalDetailId);
                ViewBag.ToPersonalDetailID = new SelectList(db.PersonalDetails, "ID", "FirstName", gandhibaghtransaction.ToPersonalDetailID);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", gandhibaghtransaction.ModifyBy);
                return View(gandhibaghtransaction);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.ToBusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", gandhibaghtransaction.ToBusinessTypeID);
                ViewBag.FromBusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", gandhibaghtransaction.FromBusinessTypeID);
                ViewBag.ToBusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", gandhibaghtransaction.ToBusinessTypeID);
                ViewBag.ChargeID = new SelectList(db.Charges, "ID", "Name", gandhibaghtransaction.ChargeID);
                ViewBag.CustomerOrderDetailID = new SelectList(db.CustomerOrderDetails, "ID", "ShopOrderCode", gandhibaghtransaction.CustomerOrderDetailID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", gandhibaghtransaction.CreateBy);
                ViewBag.FromPersonalDetailId = new SelectList(db.PersonalDetails, "ID", "FirstName", gandhibaghtransaction.FromPersonalDetailId);
                ViewBag.ToPersonalDetailID = new SelectList(db.PersonalDetails, "ID", "FirstName", gandhibaghtransaction.ToPersonalDetailID);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", gandhibaghtransaction.ModifyBy);
                return View(gandhibaghtransaction);
            }
        }
        // GET: /GandhibaghTransaction/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GandhibaghTransaction gandhibaghtransaction = db.GandhibaghTransactions.Find(id);
            if (gandhibaghtransaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.ToBusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", gandhibaghtransaction.ToBusinessTypeID);
            ViewBag.FromBusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", gandhibaghtransaction.FromBusinessTypeID);
            ViewBag.ToBusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", gandhibaghtransaction.ToBusinessTypeID);
            ViewBag.ChargeID = new SelectList(db.Charges, "ID", "Name", gandhibaghtransaction.ChargeID);
            ViewBag.CustomerOrderDetailID = new SelectList(db.CustomerOrderDetails, "ID", "ShopOrderCode", gandhibaghtransaction.CustomerOrderDetailID);
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", gandhibaghtransaction.CreateBy);
            ViewBag.FromPersonalDetailId = new SelectList(db.PersonalDetails, "ID", "FirstName", gandhibaghtransaction.FromPersonalDetailId);
            ViewBag.ToPersonalDetailID = new SelectList(db.PersonalDetails, "ID", "FirstName", gandhibaghtransaction.ToPersonalDetailID);
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", gandhibaghtransaction.ModifyBy);
            return View(gandhibaghtransaction);
        }

        // POST: /GandhibaghTransaction/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,TransactionID,ChargeID,ChargeType,FromBusinessTypeID,FromPersonalDetailId,ToBusinessTypeID,ToPersonalDetailID,ApplicablePercent,ApplicableRupee,Particular,CustomerOrderDetailID,Amount,TransactionAmount,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] GandhibaghTransaction gandhibaghtransaction)
        {
            try
            {
                GandhibaghTransaction lGandhibaghTransaction = new GandhibaghTransaction();
                gandhibaghtransaction.CreateDate = lGandhibaghTransaction.CreateDate;
                gandhibaghtransaction.CreateBy = lGandhibaghTransaction.CreateBy;
                gandhibaghtransaction.ModifyDate = DateTime.Now;
                gandhibaghtransaction.ModifyBy = fUserId;


                if (ModelState.IsValid)
                {
                    db.Entry(gandhibaghtransaction).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.ToBusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", gandhibaghtransaction.ToBusinessTypeID);
                ViewBag.FromBusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", gandhibaghtransaction.FromBusinessTypeID);
                ViewBag.ToBusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", gandhibaghtransaction.ToBusinessTypeID);
                ViewBag.ChargeID = new SelectList(db.Charges, "ID", "Name", gandhibaghtransaction.ChargeID);
                ViewBag.CustomerOrderDetailID = new SelectList(db.CustomerOrderDetails, "ID", "ShopOrderCode", gandhibaghtransaction.CustomerOrderDetailID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", gandhibaghtransaction.CreateBy);
                ViewBag.FromPersonalDetailId = new SelectList(db.PersonalDetails, "ID", "FirstName", gandhibaghtransaction.FromPersonalDetailId);
                ViewBag.ToPersonalDetailID = new SelectList(db.PersonalDetails, "ID", "FirstName", gandhibaghtransaction.ToPersonalDetailID);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", gandhibaghtransaction.ModifyBy);
                return View(gandhibaghtransaction);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty,ex.Message);
                ViewBag.ToBusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", gandhibaghtransaction.ToBusinessTypeID);
                ViewBag.FromBusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", gandhibaghtransaction.FromBusinessTypeID);
                ViewBag.ToBusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", gandhibaghtransaction.ToBusinessTypeID);
                ViewBag.ChargeID = new SelectList(db.Charges, "ID", "Name", gandhibaghtransaction.ChargeID);
                ViewBag.CustomerOrderDetailID = new SelectList(db.CustomerOrderDetails, "ID", "ShopOrderCode", gandhibaghtransaction.CustomerOrderDetailID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", gandhibaghtransaction.CreateBy);
                ViewBag.FromPersonalDetailId = new SelectList(db.PersonalDetails, "ID", "FirstName", gandhibaghtransaction.FromPersonalDetailId);
                ViewBag.ToPersonalDetailID = new SelectList(db.PersonalDetails, "ID", "FirstName", gandhibaghtransaction.ToPersonalDetailID);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", gandhibaghtransaction.ModifyBy);
                return View(gandhibaghtransaction);

            }
            }

        // GET: /GandhibaghTransaction/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GandhibaghTransaction gandhibaghtransaction = db.GandhibaghTransactions.Find(id);
            if (gandhibaghtransaction == null)
            {
                return HttpNotFound();
            }
            return View(gandhibaghtransaction);
        }

        // POST: /GandhibaghTransaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            GandhibaghTransaction gandhibaghtransaction = db.GandhibaghTransactions.Find(id);
            db.GandhibaghTransactions.Remove(gandhibaghtransaction);
            db.SaveChanges();
            return RedirectToAction("Index");
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
