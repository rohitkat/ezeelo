using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models.ViewModel;
using ModelLayer.Models;
using PagedList;
using PagedList.Mvc;

namespace Administrator.Controllers
{
    public class MerchantRegisterController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private int pageSize = 50;
        // GET: /MerchantRegister/
        [Authorize(Roles = "MerchantRegister/CanRead")]
        public ActionResult Index(int? page, string searchString)
        {
            int TotalCount = 0;
            int pageNumber = (page ?? 1);
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            List<MerchantRegisterViewModel> lMerchantRegisterViewModel = new List<MerchantRegisterViewModel>();
            try
            {
                if (!String.IsNullOrEmpty(searchString))
                {

                    var queryResult = (
                    from UL in db.UserLogins
                    join PD in db.PersonalDetails on UL.ID equals PD.UserLoginID
                    join UR in db.UserRoles on UL.ID equals UR.UserLoginID
                    join BD in db.BusinessDetails on UL.ID equals BD.UserLoginID into t
                    from BD in t.DefaultIfEmpty()
                    where PD.IsActive == true && UR.IsActive == true && UR.RoleID == 2 && BD.UserLoginID == null
                    && (PD.FirstName.Contains(searchString) || UL.Mobile.Contains(searchString) || UL.Email.Contains(searchString))
                    select new
                    {
                        ID = UL.ID,
                        Name = PD.FirstName + "" + PD.MiddleName + "" + PD.LastName,
                        PincodeID = PD.PincodeID,
                        mobile = UL.Mobile,
                        email = UL.Email,
                        Islock = UL.IsLocked
                    }).ToList();

                    foreach (var ReadRecord in queryResult)
                    {
                        PersonalDetail lPersonalDetail = new PersonalDetail();
                        UserLogin lUserLogin = new UserLogin();
                        MerchantRegisterViewModel obj = new MerchantRegisterViewModel();

                        lPersonalDetail.FirstName = ReadRecord.Name;
                        lPersonalDetail.PincodeID = ReadRecord.PincodeID;
                        lUserLogin.Mobile = ReadRecord.mobile;
                        lUserLogin.Email = ReadRecord.email;
                        lUserLogin.ID = ReadRecord.ID;
                        lUserLogin.IsLocked = ReadRecord.Islock;
                        obj.personalDetail = lPersonalDetail;
                        obj.userLogin = lUserLogin;
                        lMerchantRegisterViewModel.Add(obj);

                    }
                    TotalCount = queryResult.Count();
                    ViewBag.TotalCount = TotalCount;
                }
                else
                {

                    var queryResult = (
                  from UL in db.UserLogins
                  join PD in db.PersonalDetails on UL.ID equals PD.UserLoginID
                  join UR in db.UserRoles on UL.ID equals UR.UserLoginID
                  join BD in db.BusinessDetails on UL.ID equals BD.UserLoginID into t
                  from BD in t.DefaultIfEmpty()
                  where PD.IsActive == true && UR.IsActive == true && UR.RoleID == 2 && BD.UserLoginID == null
                  select new
                  {
                      ID = UL.ID,
                      Name = PD.FirstName + "" + PD.MiddleName + "" + PD.LastName,
                      PincodeID = PD.PincodeID,
                      mobile = UL.Mobile,
                      email = UL.Email,
                      Islock = UL.IsLocked
                  }).ToList();

                    foreach (var ReadRecord in queryResult)
                    {
                        PersonalDetail lPersonalDetail = new PersonalDetail();
                        UserLogin lUserLogin = new UserLogin();
                        MerchantRegisterViewModel obj = new MerchantRegisterViewModel();

                        lPersonalDetail.FirstName = ReadRecord.Name;
                        lPersonalDetail.PincodeID = ReadRecord.PincodeID;
                        lUserLogin.Mobile = ReadRecord.mobile;
                        lUserLogin.Email = ReadRecord.email;
                        lUserLogin.ID = ReadRecord.ID;
                        lUserLogin.IsLocked = ReadRecord.Islock;
                        obj.personalDetail = lPersonalDetail;
                        obj.userLogin = lUserLogin;
                        lMerchantRegisterViewModel.Add(obj);

                    }
                    TotalCount = queryResult.Count();
                    ViewBag.TotalCount = TotalCount;
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantRegisterController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantRegisterController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            // return View(lMerchantRegisterViewModel);
            return View(lMerchantRegisterViewModel.ToList().OrderBy(x => x.personalDetail.FirstName).ToPagedList(pageNumber, pageSize));
        }

        [Authorize(Roles = "MerchantRegister/CanWrite")]
        public ActionResult Approval(long UserLoginID, string CutomerName, string Mobile, string Email, int? PincodeID)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {

                try
                {
                    BusinessDetail bussinesDetails = new BusinessDetail();
                    bussinesDetails.UserLoginID = UserLoginID;
                    bussinesDetails.Name = string.Empty;
                    bussinesDetails.BusinessTypeID = 1;
                    bussinesDetails.ContactPerson = CutomerName;
                    bussinesDetails.Mobile = Mobile;
                    bussinesDetails.Email = Email;
                    //if (PincodeID != null)
                    //{
                    //    bussinesDetails.PincodeID =(int) PincodeID;
                    //}
                    bussinesDetails.PincodeID = (PincodeID == null ? 34435 : (int)PincodeID);
                    bussinesDetails.IsActive = true;
                    bussinesDetails.CreateBy = 1;
                    bussinesDetails.CreateDate = DateTime.UtcNow;
                    //bussinesDetails.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                    //bussinesDetails.DeviceID = "x";
                    //bussinesDetails.DeviceType = "x";
                    db.BusinessDetails.Add(bussinesDetails);
                    db.SaveChanges();

                    Shop shop = new Shop();
                    shop.BusinessDetailID = bussinesDetails.ID; ;
                    shop.Name = string.Empty;
                    shop.Website = string.Empty;
                    shop.PincodeID = (PincodeID == null ? 34435 : (int)PincodeID);
                    shop.AreaID = null;
                    shop.ContactPerson = CutomerName;
                    shop.Email = Email;
                    shop.Mobile = Mobile;
                    // shop.VAT = merchantRegisterViewModel.shop.WAT;
                    shop.TIN = string.Empty;
                    shop.PAN = string.Empty;
                    shop.CurrentItSetup = false;
                    shop.InstitutionalMerchantPurchase = false;
                    shop.InstitutionalMerchantSale = false;
                    shop.NormalSale = true;
                    shop.IsDeliveryOutSource = false;
                    shop.MinimumAmountForFreeDelivery = 0;
                    shop.IsFreeHomeDelivery = false;
                    shop.DeliveryPartnerId = 1;//-----------------------------------------
                    shop.FranchiseID = 1;//-----------------------------------------------
                    shop.IsLive = false;
                    shop.IsManageInventory = false;
                    shop.SearchKeywords = string.Empty;
                    shop.IsAgreedOnReturnProduct = false;
                    shop.ReturnDurationInDays = 0;

                    shop.IsActive = true;
                    shop.CreateBy = 1;
                    shop.CreateDate = DateTime.UtcNow.AddHours(5.30);
                    shop.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                    shop.DeviceID = string.Empty;
                    shop.DeviceType = string.Empty;

                    db.Shops.Add(shop);
                    db.SaveChanges();

                    dbContextTransaction.Commit();

                    //this.SendWelcomeSMS(bussinesDetails.ID, merchantRegisterViewModel.personalDetail.FirstName, Mobile);

                    //this.SendSignUpSMS(bussinesDetails.ID, merchantRegisterViewModel.personalDetail.FirstName, Mobile, merchantRegisterViewModel.userLogin.Email);

                    //this.SendLoginDetailsEMail(bussinesDetails.ID, merchantRegisterViewModel.personalDetail.FirstName, Mobile, merchantRegisterViewModel.userLogin.Email, userLogin.Password);

                    //this.SendWelcomeEMail(bussinesDetails.ID, merchantRegisterViewModel.personalDetail.FirstName, Mobile, merchantRegisterViewModel.userLogin.Email, userLogin.Password);

                    //return RedirectToAction("View1", "MerchantRegister");
                    //ViewBag.Message1 = "Register Successfully! Our Executive will contact you within 24 hour !!";
                    //TempData["Message1"] = "Register Successfully! Our Executive will contact you within 24 hour !!";
                }
                catch
                {
                    ViewBag.Message1 = "Theres somthiong wrong with databse, chnages not saved !!";
                    TempData["Message1"] = "Theres somthiong wrong with databse, chnages not saved !!";
                }
            }
            return RedirectToAction("Index", "MerchantRegister");

        }

        [Authorize(Roles = "MerchantRegister/CanRead")]
        public ActionResult Edit(long UserLoginID)
        {
            MerchantRegisterViewModel lMerchantRegisterViewModel = new MerchantRegisterViewModel();
            UserLogin lUserLogin = db.UserLogins.Find(UserLoginID);
            lMerchantRegisterViewModel.userLogin = lUserLogin;
            return View(lMerchantRegisterViewModel);
        }
        [HttpPost]
        [Authorize(Roles = "MerchantRegister/CanWrite")]
        public ActionResult Edit([Bind(Include = "userLogin")] MerchantRegisterViewModel lMerchantRegisterViewModel)
        {
            try
            {
                UserLogin lUserLogin = db.UserLogins.Find(lMerchantRegisterViewModel.userLogin.ID);
                lUserLogin.Email = lMerchantRegisterViewModel.userLogin.Email;
                lUserLogin.Mobile = lMerchantRegisterViewModel.userLogin.Mobile;
                lUserLogin.IsLocked = lMerchantRegisterViewModel.userLogin.IsLocked;
                db.SaveChanges();
                ViewBag.message = "Save Successfully";

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantRegisterController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantRegisterController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View(lMerchantRegisterViewModel);
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
