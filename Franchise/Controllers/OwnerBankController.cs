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
using Franchise.Models;

namespace Franchise.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public class OwnerBankController : Controller
    {

        #region Genral Code
        private EzeeloDBContext db = new EzeeloDBContext();

        private long GetOwnerBankID()
        {
            long ownerBankID = 0;
            try
            {
                int franchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);
                int businesstypeID = 0;
                long businessDetailID = 0;
                businessDetailID = db.Franchises.Where(x => x.ID == franchiseID).Select(x => x.BusinessDetailID).FirstOrDefault();
                businesstypeID = db.BusinessDetails.Where(x => x.ID == businessDetailID).Select(x => x.BusinessTypeID).FirstOrDefault();
                ownerBankID = db.OwnerBanks.Where(x => x.BusinessTypeID == businesstypeID && x.OwnerID == franchiseID).Select(x => x.ID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[GetOwnerBankID]", "Can't Get Owner Bank ID! in Method !" + Environment.NewLine + ex.Message);
            }
            return ownerBankID;
        }
        //private int GetFranchiseID()
        //{
        //    int franchiseID = 0;
        //    try
        //    {
        //        long UID = Convert.ToInt64(Session["ID"]);

        //        long businessDetailID = 0;
        //        businessDetailID = db.BusinessDetails.Where(x => x.UserLoginID == UID).Select(x => x.ID).FirstOrDefault();
        //        franchiseID = db.Franchises.Where(x => x.BusinessDetailID == businessDetailID).Select(x => x.ID).FirstOrDefault();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new BusinessLogicLayer.MyException("[GetOwnerBankID]", "Can't Get Owner Bank ID! in Method !" + Environment.NewLine + ex.Message);
        //    }
        //    return franchiseID;
        //}
        #endregion
        // GET: /OwnerBank/
        //public ActionResult Index()
        //{
        //    var ownerbanks = db.OwnerBanks.Include(o => o.Bank).Include(o => o.BankAccountType).Include(o => o.BusinessType).Include(o => o.PersonalDetail).Include(o => o.PersonalDetail1);
        //    return View(ownerbanks.ToList());
        //}

        // GET: /OwnerBank/Details/5
        [SessionExpire]
        [CustomAuthorize(Roles = "OwnerBank/CanRead")]
        public ActionResult Details()
        {
            try
            {
                long id = GetOwnerBankID();
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
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OwnerBankController][GET:Details]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OwnerBankController][GET:Details]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // GET: /OwnerBank/Create
        [SessionExpire]
        [CustomAuthorize(Roles = "OwnerBank/CanRead")]
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "OwnerBank/CanWrite")]
        public ActionResult Create([Bind(Include = "ID,BusinessTypeID,BankID,AccountName,BranchName,IFSCCode,MICRCode,AccountNumber,BankAccountTypeID,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] OwnerBank ownerbank)
        {
            try
            {
                long PersonalDetailID = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                long FranchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]); //GetFranchiseID();
                OwnerBank ob = new OwnerBank();

                ob.BusinessTypeID = db.BusinessTypes.Where(x => x.Prefix == "GBFR").Select(x => x.ID).FirstOrDefault();  // ownerbank.BusinessTypeID;
                ob.OwnerID = FranchiseID; //ownerbank.OwnerID;
                ob.AccountName = ownerbank.AccountName;
                ob.BankID = ownerbank.BankID;
                ob.BranchName = ownerbank.BranchName;
                ob.IFSCCode = ownerbank.IFSCCode;
                ob.MICRCode = ownerbank.MICRCode;
                ob.AccountNumber = ownerbank.AccountNumber;
                ob.BankAccountTypeID = ownerbank.BankAccountTypeID;
                ob.IsActive = true;
                ob.CreateBy = PersonalDetailID;
                ob.CreateDate = DateTime.Now;
                ob.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                ob.DeviceID = "x";
                ob.DeviceType = "x";
                if (ModelState.IsValid)
                {
                    db.OwnerBanks.Add(ob);
                    db.SaveChanges();
                }
                ViewBag.Message = "Bank Detail Save Successfully.";
                ViewBag.BankID = new SelectList(db.Banks.Where(x=>x.IsActive==true).OrderBy(x=>x.Name).ToList(), "ID", "Name", ownerbank.BankID);
                ViewBag.BankAccountTypeID = new SelectList(db.BankAccountTypes, "ID", "Name", ownerbank.BankAccountTypeID);
                ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", ownerbank.BusinessTypeID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerbank.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerbank.ModifyBy);
                //return View(ownerbank);

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OwnerBankController][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OwnerBankController][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
        }


        // GET: /OwnerBank/Edit/5
        [SessionExpire]
        [CustomAuthorize(Roles = "OwnerBank/CanRead")]
        public ActionResult Edit()
        {
            try
            {
                long id = GetOwnerBankID();

                if (id == 0)
                {
                    return RedirectToAction("Create", "OwnerBank");
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
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OwnerBankController][GET:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OwnerBankController][GET:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // POST: /OwnerBank/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "OwnerBank/CanWrite")]
        public ActionResult Edit([Bind(Include = "ID,BusinessTypeID,OwnerID,AccountName,BankID,BranchName,IFSCCode,MICRCode,AccountNumber,BankAccountTypeID,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] OwnerBank ownerbank)
        {
            try
            {
                long PersonalDetailID = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));

                ViewBag.BankID = new SelectList(db.Banks.Where(x => x.IsActive == true).OrderBy(x => x.Name).ToList(), "ID", "Name", ownerbank.BankID);
                ViewBag.BankAccountTypeID = new SelectList(db.BankAccountTypes, "ID", "Name", ownerbank.BankAccountTypeID);
                ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", ownerbank.BusinessTypeID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerbank.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerbank.ModifyBy);

                OwnerBank ob = db.OwnerBanks.Find(ownerbank.ID);

                ob.OwnerID = Convert.ToInt32(Session["FRANCHISE_ID"]); //GetFranchiseID();
                ob.AccountName = ownerbank.AccountName;
                ob.BankID = ownerbank.BankID;
                ob.BranchName = ownerbank.BranchName;
                ob.IFSCCode = ownerbank.IFSCCode;
                ob.MICRCode = ownerbank.MICRCode;
                ob.AccountNumber = ownerbank.AccountNumber;
                ob.BusinessTypeID = db.BusinessTypes.Where(x => x.Prefix == "GBFR").Select(x => x.ID).FirstOrDefault();
                ob.IsActive = true;
                ob.CreateBy = PersonalDetailID;
                ob.CreateDate = DateTime.UtcNow;
                ob.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                ob.DeviceID = "x";
                ob.DeviceType = "x";
                db.SaveChanges();
                ViewBag.Message = "Bank Detail Save Successfully.";
                return View(ownerbank);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OwnerBankController][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OwnerBankController][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View(ownerbank);
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
