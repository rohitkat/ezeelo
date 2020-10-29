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
    public class OwnerBankController : Controller
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

        // GET: /OwnerBank/
        public ActionResult Index()
        {
            var ownerbanks = db.OwnerBanks.Include(o => o.Bank).Include(o => o.BankAccountType).Include(o => o.BusinessType).Include(o => o.PersonalDetail).Include(o => o.PersonalDetail1);
            return View(ownerbanks.ToList());
        }

        // GET: /OwnerBank/Details/5
        public ActionResult Details(long? id)
        {
            SessionDetails();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OwnerBank ownerbank = db.OwnerBanks.Find(id);
            if (ownerbank == null)
            {
                return HttpNotFound();
            }
            if (ownerbank.OwnerID  != deliveryPartnerSessionViewModel.DeliveryPartnerID)
            {
                return View("AccessDenied");
            }
            return View(ownerbank);
        }

        // GET: /OwnerBank/Create
        public ActionResult Create()
        {
            ViewBag.BankID = new SelectList(db.Banks, "ID", "Name");
            ViewBag.BankAccountTypeID = new SelectList(db.BankAccountTypes, "ID", "Name");
            ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name");
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
            return View();
        }

        // POST: /OwnerBank/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,BusinessTypeID,OwnerID,BankID,BranchName,IFSCCode,MICRCode,AccountNumber,BankAccountTypeID,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] OwnerBank ownerbank)
        { 
            SessionDetails();
            try
            {
                ownerbank.CreateDate = DateTime.Now;
                ownerbank.CreateBy = deliveryPartnerSessionViewModel.PersonalDetailID;
                ownerbank.OwnerID = deliveryPartnerSessionViewModel.DeliveryPartnerID;
                ownerbank.BusinessTypeID = (int)Common.Constant.BUSINESS_TYPE.DELIVERY_PARTNER;
                if (ModelState.IsValid)
                {
                    db.OwnerBanks.Add(ownerbank);
                    db.SaveChanges();
                    return RedirectToAction("ownerbank");
                }

                ViewBag.BankID = new SelectList(db.Banks, "ID", "Name", ownerbank.BankID);
                ViewBag.BankAccountTypeID = new SelectList(db.BankAccountTypes, "ID", "Name", ownerbank.BankAccountTypeID);
                ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", ownerbank.BusinessTypeID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerbank.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerbank.ModifyBy);
                return View(ownerbank);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's something wrong with the Bank detail values!");
                ViewBag.BankID = new SelectList(db.Banks, "ID", "Name", ownerbank.BankID);
                ViewBag.BankAccountTypeID = new SelectList(db.BankAccountTypes, "ID", "Name", ownerbank.BankAccountTypeID);
                ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", ownerbank.BusinessTypeID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerbank.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerbank.ModifyBy);
                return View(ownerbank);
            }
        }

        // GET: /OwnerBank/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OwnerBank ownerbank = db.OwnerBanks.Find(id);
            if (ownerbank == null)
            {
                return HttpNotFound();
            }
            ViewBag.BankID = new SelectList(db.Banks, "ID", "Name", ownerbank.BankID);
            ViewBag.BankAccountTypeID = new SelectList(db.BankAccountTypes, "ID", "Name", ownerbank.BankAccountTypeID);
            ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", ownerbank.BusinessTypeID);
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerbank.CreateBy);
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerbank.ModifyBy);
            return View(ownerbank);
        }

        // POST: /OwnerBank/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,BusinessTypeID,OwnerID,BankID,BranchName,IFSCCode,MICRCode,AccountNumber,BankAccountTypeID,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] OwnerBank ownerbank)
        {
            SessionDetails();
            try
            {
                EzeeloDBContext db1 = new EzeeloDBContext();
                OwnerBank lOwnerBank = db1.OwnerBanks.Find(ownerbank.ID);
                ownerbank.BusinessTypeID = lOwnerBank.BusinessTypeID;
                ownerbank.OwnerID = lOwnerBank.OwnerID;
                ownerbank.CreateBy = lOwnerBank.CreateBy;
                ownerbank.CreateDate = lOwnerBank.CreateDate;
                ownerbank.ModifyBy = deliveryPartnerSessionViewModel.PersonalDetailID;
                ownerbank.ModifyDate = DateTime.Now;
                db1.Dispose();
            if (ModelState.IsValid)
            {
                db.Entry(ownerbank).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Msg"] = "Data Saved Successfully";
                return View("Details", ownerbank);
                //return RedirectToAction("ownerbank");
            }
            ViewBag.BankID = new SelectList(db.Banks, "ID", "Name", ownerbank.BankID);
            ViewBag.BankAccountTypeID = new SelectList(db.BankAccountTypes, "ID", "Name", ownerbank.BankAccountTypeID);
            ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", ownerbank.BusinessTypeID);
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerbank.CreateBy);
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerbank.ModifyBy);
            return View(ownerbank);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's something wrong with the Bank detail values!");

                ViewBag.BankID = new SelectList(db.Banks, "ID", "Name", ownerbank.BankID);
                ViewBag.BankAccountTypeID = new SelectList(db.BankAccountTypes, "ID", "Name", ownerbank.BankAccountTypeID);
                ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", ownerbank.BusinessTypeID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerbank.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerbank.ModifyBy);
                return View(ownerbank);
            }
        }

        // GET: /OwnerBank/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OwnerBank ownerbank = db.OwnerBanks.Find(id);
            if (ownerbank == null)
            {
                return HttpNotFound();
            }
            return View(ownerbank);
        }

        // POST: /OwnerBank/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            OwnerBank ownerbank = db.OwnerBanks.Find(id);
            db.OwnerBanks.Remove(ownerbank);
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
