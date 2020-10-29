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
using Franchise.Models;

namespace Franchise.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public class MOwnerBankController : Controller
    {

        #region Genral Code
        private EzeeloDBContext db = new EzeeloDBContext();
        long? UserLogin;
        private long GetPersonalDetailID()
        {
            //Session["USER_LOGIN_ID"] = 1;
            //long UserLoginID = Convert.ToInt32(Session["USER_LOGIN_ID"]);
            long UserLoginID = Convert.ToInt32(Session["ID"]);
            long PersonalDetailID = 0;
            try
            {
                if (UserLoginID > 0)
                {
                    PersonalDetailID = Convert.ToInt32(db.PersonalDetails.Where(x => x.UserLoginID == UserLoginID).Select(x => x.ID).First());
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[PlacedController][GetShopID]", "Can't find PersonalDetailID !" + Environment.NewLine + ex.Message);
            }
            return PersonalDetailID;
        }

        private long GetShopID(long? UserLoginID)
        {
            //Session["USER_LOGIN_ID"] = 1;
            //long UserLoginID = Convert.ToInt32(Session["ID"]);
            long BusinessDetailID = 0;
            long ShopID = 0;
            try
            {
                if (UserLoginID > 0)
                {
                    BusinessDetailID = Convert.ToInt32(db.BusinessDetails.Where(x => x.UserLoginID == UserLoginID).Select(x => x.ID).First());
                    ShopID = Convert.ToInt32(db.Shops.Where(x => x.BusinessDetailID == BusinessDetailID).Select(x => x.ID).First());
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[PlacedController][GetShopID]", "Can't find ShopID !" + Environment.NewLine + ex.Message);
            }
            return ShopID;
        }
        private long GetOwnerBankID(long? UserLoginID)
        {
            long ownerBankID = 0;
            try
            {
                //long UserLoginID = Convert.ToInt32(Session["USER_LOGIN_ID"]);
                long shopID = 0, businesstypeID = 0;
                long businessDetailID = 0;
                businessDetailID = db.BusinessDetails.Where(x => x.UserLoginID == UserLoginID).Select(x => x.ID).FirstOrDefault();
                businesstypeID = db.BusinessDetails.Where(x => x.UserLoginID == UserLoginID).Select(x => x.BusinessTypeID).FirstOrDefault();
                shopID = db.Shops.Where(x => x.BusinessDetailID == businessDetailID).Select(x => x.ID).FirstOrDefault();
                ownerBankID = db.OwnerBanks.Where(x => x.BusinessTypeID == businesstypeID && x.OwnerID == shopID).Select(x => x.ID).FirstOrDefault();


            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[GetOwnerBankID]", "Can't Get Owner Bank ID! in Method !" + Environment.NewLine + ex.Message);
            }
            return ownerBankID;
        }
        #endregion
        // GET: /OwnerBank/
        [SessionExpire]
        [CustomAuthorize(Roles = "MOwnerBank/CanRead")]
        public ActionResult Index()
        {
            var ownerbanks = db.OwnerBanks.Include(o => o.Bank).Include(o => o.BankAccountType).Include(o => o.BusinessType).Include(o => o.PersonalDetail).Include(o => o.PersonalDetail1);
            return View(ownerbanks.ToList());
        }

        // GET: /OwnerBank/Details/5
        public ActionResult Details(long? id)
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

        // GET: /OwnerBank/Create
        [SessionExpire]
        [CustomAuthorize(Roles = "MOwnerBank/CanRead")]
        public ActionResult Create()
        {
            ViewBag.BankID = new SelectList(db.Banks.Where(x => x.IsActive == true).OrderBy(x => x.Name).ToList(), "ID", "Name");
            ViewBag.BankAccountTypeID = new SelectList(db.BankAccountTypes, "ID", "Name");
            ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name");
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
            return View();
        }

        // POST: /OwnerBank/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [SessionExpire]
        [CustomAuthorize(Roles = "MOwnerBank/CanWrite")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,BankID,AccountName,BranchName,IFSCCode,MICRCode,AccountNumber,BankAccountTypeID")] OwnerBank ownerbank)
        {
            try
            {
                long ID = Convert.ToInt64(TempData["UserLogin1"].ToString());
                long ShopId = GetShopID(ID);
                //long ShopId = GetShopID();
                long PersonalDetailID = GetPersonalDetailID();

                OwnerBank ob = new OwnerBank();

                ob.BusinessTypeID = db.BusinessTypes.Where(x => x.Prefix == "GBMR").Select(x => x.ID).FirstOrDefault(); // ownerbank.BusinessTypeID;
                ob.OwnerID = ShopId; //ownerbank.OwnerID;
                ob.AccountName = ownerbank.AccountName;
                ob.BankID = ownerbank.BankID;
                ob.BranchName = ownerbank.BranchName;
                ob.IFSCCode = ownerbank.IFSCCode;
                ob.MICRCode = ownerbank.MICRCode;
                ob.AccountNumber = ownerbank.AccountNumber;
                ob.BankAccountTypeID = ownerbank.BankAccountTypeID;
                ob.IsActive = true;
                ob.CreateBy = PersonalDetailID;
                ob.CreateDate = DateTime.UtcNow;
                ob.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                ob.DeviceID = "x";
                ob.DeviceType = "x";
                if (ModelState.IsValid)
                {
                    db.OwnerBanks.Add(ob);
                    db.SaveChanges();
                    ViewBag.Message = "Record Saved Successfully.";
                }
                ViewBag.BankID = new SelectList(db.Banks.Where(x => x.IsActive == true).OrderBy(x => x.Name).ToList(), "ID", "Name", ownerbank.BankID);
                ViewBag.BankAccountTypeID = new SelectList(db.BankAccountTypes, "ID", "Name", ownerbank.BankAccountTypeID);
                ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", ownerbank.BusinessTypeID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerbank.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerbank.ModifyBy);
                //return View(ownerbank);
                return View();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the Product upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductUploadTemp][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                ViewBag.BankID = new SelectList(db.Banks.Where(x => x.IsActive == true).OrderBy(x => x.Name).ToList(), "ID", "Name", ownerbank.BankID);
                ViewBag.BankAccountTypeID = new SelectList(db.BankAccountTypes, "ID", "Name", ownerbank.BankAccountTypeID);
                ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", ownerbank.BusinessTypeID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerbank.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerbank.ModifyBy);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong with the Product upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductUploadTemp][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                ViewBag.BankID = new SelectList(db.Banks.Where(x => x.IsActive == true).OrderBy(x => x.Name).ToList(), "ID", "Name", ownerbank.BankID);
                ViewBag.BankAccountTypeID = new SelectList(db.BankAccountTypes, "ID", "Name", ownerbank.BankAccountTypeID);
                ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", ownerbank.BusinessTypeID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerbank.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerbank.ModifyBy);
            }
            return View();
        }

        // GET: /OwnerBank/Edit/5
        [SessionExpire]
        [CustomAuthorize(Roles = "MOwnerBank/CanRead")]
        public ActionResult Edit(long? UserLoginID)
        {
            //long id = GetPersonalDetailID();
            long id = GetOwnerBankID(UserLoginID);


            TempData["UserLogin1"] = UserLoginID;
            TempData.Keep();
            //UserLogin = UserLoginID;
            if (id == 0)
            {
                return RedirectToAction("Create", "MOwnerBank");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OwnerBank ownerbank = db.OwnerBanks.Find(id);
            if (ownerbank == null)
            {
                return HttpNotFound();
            }
            ViewBag.BankID = new SelectList(db.Banks.Where(x => x.IsActive == true).OrderBy(x => x.Name).ToList(), "ID", "Name", ownerbank.BankID);
            ViewBag.BankAccountTypeID = new SelectList(db.BankAccountTypes, "ID", "Name", ownerbank.BankAccountTypeID);
            ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", ownerbank.BusinessTypeID);
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerbank.CreateBy);
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerbank.ModifyBy);
            return View(ownerbank);
        }

        // POST: /OwnerBank/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [SessionExpire]
        [CustomAuthorize(Roles = "MOwnerBank/CanWrite")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,BusinessTypeID,OwnerID,AccountName,BankID,BranchName,IFSCCode,MICRCode,AccountNumber,BankAccountTypeID,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] OwnerBank ownerbank)
        {

            try
            {
                long PersonalDetailID = GetPersonalDetailID();

                ViewBag.BankID = new SelectList(db.Banks.Where(x => x.IsActive == true).OrderBy(x => x.Name).ToList(), "ID", "Name", ownerbank.BankID);
                ViewBag.BankAccountTypeID = new SelectList(db.BankAccountTypes, "ID", "Name", ownerbank.BankAccountTypeID);
                ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", ownerbank.BusinessTypeID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerbank.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerbank.ModifyBy);

                OwnerBank ob = db.OwnerBanks.Find(ownerbank.ID);

                ob.BusinessTypeID = ownerbank.BusinessTypeID;
                ob.OwnerID = ownerbank.OwnerID;
                ob.AccountName = ownerbank.AccountName;
                ob.BankID = ownerbank.BankID;
                ob.BranchName = ownerbank.BranchName;
                ob.IFSCCode = ownerbank.IFSCCode;
                ob.MICRCode = ownerbank.MICRCode;
                ob.AccountNumber = ownerbank.AccountNumber;
                ob.BankAccountTypeID = ownerbank.BankAccountTypeID;
                ob.IsActive = true;
                ob.CreateBy = ownerbank.CreateBy;
                ob.CreateDate = ownerbank.CreateDate;
                ob.ModifyDate = DateTime.UtcNow;
                ob.ModifyBy = PersonalDetailID;
                ob.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                ob.DeviceID = "x";
                ob.DeviceType = "x";
                db.SaveChanges();
                ViewBag.Message = "Record Saved Successfully.";
                return View();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the Product upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductUploadTemp][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                ViewBag.BankID = new SelectList(db.Banks.Where(x => x.IsActive == true).OrderBy(x => x.Name).ToList(), "ID", "Name", ownerbank.BankID);
                ViewBag.BankAccountTypeID = new SelectList(db.BankAccountTypes, "ID", "Name", ownerbank.BankAccountTypeID);
                ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", ownerbank.BusinessTypeID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerbank.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerbank.ModifyBy);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong with the Product upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductUploadTemp][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                ViewBag.BankID = new SelectList(db.Banks.Where(x => x.IsActive == true).OrderBy(x => x.Name).ToList(), "ID", "Name", ownerbank.BankID);
                ViewBag.BankAccountTypeID = new SelectList(db.BankAccountTypes, "ID", "Name", ownerbank.BankAccountTypeID);
                ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", ownerbank.BusinessTypeID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerbank.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerbank.ModifyBy);
            }
            return View(ownerbank);
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

        public object UserLogin1 { get; set; }
    }
}
