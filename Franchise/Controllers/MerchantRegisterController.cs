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
using System.Text.RegularExpressions;
using System.Web.Security;
using Franchise.Models;
using BusinessLogicLayer;

namespace Franchise.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    [SessionExpire]
    public class MerchantRegisterController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
       
        //private int GetFranchiseID()
        //{
        //    long BusinessDetailID = 0;
        //    long UID = Convert.ToInt64(Session["ID"]);
        //    int FranchiseID = 0;
        //    try
        //    {
        //        if (UID > 0)
        //        {
        //            BusinessDetailID = Convert.ToInt32(db.BusinessDetails.Where(x => x.UserLoginID == UID).Select(x => x.ID).First());
        //            FranchiseID = Convert.ToInt32(db.Franchises.Where(x => x.BusinessDetailID == BusinessDetailID).Select(x => x.ID).First());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new BusinessLogicLayer.MyException("[GetFranchiseID]", "Can't Get Franchise Details! in Method !" + Environment.NewLine + ex.Message);
        //    }
        //    return FranchiseID;
        //}
        // GET: /MerchantRegister/
         //[SessionExpire]
         [CustomAuthorize(Roles = "MerchantRegister/CanRead")]
        public ActionResult Index()
        {
            ViewBag.Salutation = new SelectList(db.Salutations, "ID", "Name", 1);
            ViewBag.SecurityQuestion = new SelectList(db.SecurityQuestions, "ID", "Question");
            return View();
        }
        [HttpPost]
        //[SessionExpire]
        [CustomAuthorize(Roles = "MerchantRegister/CanWrite")]
        public ActionResult Index(MerchantRegisterViewModel merchantRegisterViewModel, int salutation, string Pincode, string Mobile)
        {
            //switch (submit)
            //{
            //    case "Save":
            ViewBag.Salutation = new SelectList(db.Salutations, "ID", "Name");
            ViewBag.SecurityQuestion = new SelectList(db.SecurityQuestions, "ID", "Question");

            UserLogin userLogin = new UserLogin();
            userLogin.Mobile = Mobile;// merchantRegisterViewModel.userLogin.Mobile;
            userLogin.Email = merchantRegisterViewModel.userLogin.Email;
            userLogin.Password = merchantRegisterViewModel.userLogin.Password;
            userLogin.IsLocked = true;
            userLogin.CreateDate = DateTime.UtcNow;
            userLogin.CreateBy = 1;
            userLogin.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
            userLogin.DeviceType = "x";
            userLogin.DeviceID = "x";
            db.UserLogins.Add(userLogin);
            db.SaveChanges();

            //ModelLayer.Models.Pincode pin = new ModelLayer.Models.Pincode();

            var pin = db.Pincodes.Where(x => x.Name == Pincode).FirstOrDefault();

            if (pin == null)
            {
                throw new Exception("Pincod is not valid or does not exist");
            }

            PersonalDetail personalDetail = new PersonalDetail();
            personalDetail.UserLoginID = userLogin.ID;
            personalDetail.SalutationID = salutation;// merchantRegisterViewModel.salutation.ID;
            personalDetail.FirstName = merchantRegisterViewModel.personalDetail.FirstName;
            personalDetail.MiddleName = merchantRegisterViewModel.personalDetail.MiddleName;
            personalDetail.LastName = merchantRegisterViewModel.personalDetail.LastName;
            personalDetail.DOB = merchantRegisterViewModel.personalDetail.DOB;
            personalDetail.Gender = merchantRegisterViewModel.personalDetail.Gender;
            personalDetail.PincodeID = pin.ID; // merchantRegisterViewModel.personalDetail.PincodeID;
            personalDetail.Address = merchantRegisterViewModel.personalDetail.Address;
            personalDetail.AlternateMobile = merchantRegisterViewModel.personalDetail.AlternateMobile;
            personalDetail.AlternateEmail = merchantRegisterViewModel.personalDetail.AlternateEmail;
            personalDetail.IsActive = true;
            personalDetail.CreateDate = DateTime.UtcNow;
            personalDetail.CreateBy = 1;
            personalDetail.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
            personalDetail.DeviceType = "x";
            personalDetail.DeviceID = "x";
            db.PersonalDetails.Add(personalDetail);
            db.SaveChanges();

            BusinessDetail bussinesDetails = new BusinessDetail();
            bussinesDetails.UserLoginID = userLogin.ID;
            bussinesDetails.Name = string.Empty;// merchantRegisterViewModel.businessDetail.Name;
            bussinesDetails.BusinessTypeID = 1; //merchantRegisterViewModel.businessDetail.BusinessTypeID;
            bussinesDetails.ContactPerson = merchantRegisterViewModel.personalDetail.FirstName + " " + merchantRegisterViewModel.personalDetail.LastName; //merchantRegisterViewModel.businessDetail.ContactPerson;
            bussinesDetails.Mobile = Mobile;// merchantRegisterViewModel.businessDetail.Mobile;
            bussinesDetails.Email = merchantRegisterViewModel.userLogin.Email; //merchantRegisterViewModel.businessDetail.Email;
            bussinesDetails.PincodeID = pin.ID; //merchantRegisterViewModel.businessDetail.PincodeID;
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
            shop.Website = merchantRegisterViewModel.shop.Website;
            shop.PincodeID = pin.ID;
            shop.AreaID = null;
            shop.ContactPerson = merchantRegisterViewModel.personalDetail.FirstName + " " + merchantRegisterViewModel.personalDetail.LastName;
            shop.Email = merchantRegisterViewModel.userLogin.Email;
            shop.Mobile = Mobile;
            // shop.VAT = merchantRegisterViewModel.shop.WAT;
            shop.TIN = merchantRegisterViewModel.shop.TIN;
            shop.PAN = merchantRegisterViewModel.shop.PAN;
            shop.CurrentItSetup = false;
            shop.InstitutionalMerchantPurchase = false;
            shop.InstitutionalMerchantSale = false;
            shop.NormalSale = true;
            shop.IsDeliveryOutSource = false;
            shop.MinimumAmountForFreeDelivery = 0;
            shop.IsFreeHomeDelivery = false;
            shop.DeliveryPartnerId = 1;//-----------------------------------------

            //long LoginID = Convert.ToInt64(Session["USER_LOGIN_ID"].ToString());
            //long businessID = db.BusinessDetails.Where(x => x.UserLoginID == LoginID).FirstOrDefault().ID;
            //long FranchiseID = db.Franchises.Where(x => x.BusinessDetailID == businessID).FirstOrDefault().ID;

            shop.FranchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]); //db.Franchises.Where(x => x.BusinessDetailID == businessID).FirstOrDefault().ID; //-----------------------------------------------
            shop.IsLive = false;
            shop.IsManageInventory = false;
            shop.SearchKeywords = string.Empty; //merchantRegisterNow.BussinessName;
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

            //sneding mail to franchise after registration
            sendEmail(userLogin.ID);
            //sneding sms to franchise after registration
            sendSMS(userLogin.ID);
            string merchantName = db.Salutations.Where(x => x.ID == salutation).Select(x => x.Name).FirstOrDefault() + " " + merchantRegisterViewModel.personalDetail.FirstName + " " + merchantRegisterViewModel.personalDetail.LastName;
            TempData["Message1"] = merchantName + " Register Successfully! We will contact you soon !!";
            //TempData["personalDeailID"] = personalDetail.ID;
            //ModelState.AddModelError("CustomError","Merchant Register Successfully! Our Executive will contact you within 24 hour !!");
            return RedirectToAction("MerchantList");
            //return RedirectToAction("Edit", "MPersonalDetail");

        }

        //[SessionExpire]
        [CustomAuthorize(Roles = "MerchantRegister/CanRead")]
        public ActionResult MerchantList()  
        
        {
            try
            {
                long franchiseID =Convert.ToInt64( Session["FRANCHISE_ID"]);//GetFranchiseID();

                var lfav = (from ul in db.UserLogins
                            join bd in db.BusinessDetails on ul.ID equals bd.UserLoginID
                            join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                            join s in db.Shops on bd.ID equals s.BusinessDetailID
                            //where bd.BusinessType.Prefix == "GBMR" && ul.IsLocked == true && s.FranchiseID == franchiseID
                            where bd.BusinessType.Prefix == "GBMR" && s.FranchiseID == franchiseID
                            select new MerchantPendingApprovalViewModel
                            {
                                ID = pd.ID,
                                UserLoginID = bd.UserLoginID,
                                BusinessTypePrefix = bd.BusinessType.Prefix,
                                Name = pd.Salutation.Name + " " + pd.FirstName + " " + pd.LastName,
                                OwnerId = s.ID,
                                ShopName=s.Name,
                                mobile=ul.Mobile,
                                Email=ul.Email,
                                IsLock=ul.IsLocked,
                            }).Distinct().OrderBy(x => x.Name);

                return View(lfav);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantApprovalController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantApprovalController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();

        }

        public ActionResult GetAddress(string Pincode)
        {

            if (!(db.Pincodes.Any(p => p.Name == Pincode)))
            {

                return Json("1", JsonRequestBehavior.AllowGet);
            }

            return Json("0", JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsMobileAvailable(string Mobile)
        {
            if (db.UserLogins.Any(x => x.Mobile == Mobile))
            {
                return Json("1", JsonRequestBehavior.AllowGet);
            }
            return Json("0", JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        //funcn for login
        private bool IsValidEmailId(string pInputEmail)
        {
            //Regex To validate Email Address
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(pInputEmail);
            if (match.Success)
                return true;
            else
                return false;
        }
        private bool IsValidMobile(string pInputMobile)
        {
            //Regex To validate Email Address
            Regex regex = new Regex(@"^[7-9]{1}[0-9]{9}$");
            Match match = regex.Match(pInputMobile);
            if (match.Success)
                return true;
            else
                return false;
        }
        private Dictionary<string, string> CheckLogin(string pUserName, string pPassword)
        {
            Dictionary<string, string> lDictUserDetails = new Dictionary<string, string>();

            var userExist = db.UserLogins.Select(x => new { x.Email, x.Mobile, x.Password, x.ID, x.IsLocked })
                                         .Where(x => (x.Email == pUserName || x.Mobile == pUserName) && x.Password == pPassword && x.IsLocked == false).ToList();
            if (userExist.Count() > 0)
            {
                foreach (var item in userExist)
                {
                    lDictUserDetails.Add("ID", item.ID.ToString());
                    lDictUserDetails.Add("UserName", item.Email.ToString());
                }
            }

            return lDictUserDetails;
        }

        public JsonResult IsEmailAvailable(string Email)
        {
            if (db.UserLogins.Any(x => x.Email == Email))
            {
                return Json("1", JsonRequestBehavior.AllowGet);
            }
            return Json("0", JsonRequestBehavior.AllowGet);
        }

        public void sendEmail(long uid)
        {
            try
            {
                PersonalDetail lPD = db.PersonalDetails.Find(CommonFunctions.GetPersonalDetailsID(uid));
                string email = db.UserLogins.Find(uid).Email;
                // var merchantDetail= db.UserLogins.Find(uid);

                //long merchantId = db.BusinessDetails.Where(x => x.UserLoginID == uid).FirstOrDefault().ID;

                //string shopName = db.Shops.Where(x => x.BusinessDetailID == merchantId).FirstOrDefault().Name;

                // Sending email to the user
                BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                Dictionary<string, string> emailParaMetres = new Dictionary<string, string>();
                emailParaMetres.Add("<!--ACCOUNT_URL-->", "" + (new URLsFromConfig()).GetURL("MERCHANT") + "");
                emailParaMetres.Add("<!--NAME-->", lPD.FirstName);
                //emailParaMetres.Add("<!--SHOP_NAME-->", shopName);
                //emailParaMetres.Add("<!--MERCHANT_ID-->", merchantId.ToString());
                


                gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.MER_REGISTRATION, new string[] { email, rcKey.DEFAULT_ALL_EMAIL }, emailParaMetres, true);

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Merchant Approved Succesfully, there might be problem sending email, please check your email or contact administrator!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantApprovalController][sendEmail]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantApprovalController][sendEmail]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
        }

        public void sendSMS(long uid)
        {
            try
            {
                PersonalDetail lPD = db.PersonalDetails.Find(CommonFunctions.GetPersonalDetailsID(uid));
                string mbno = db.UserLogins.Find(uid).Mobile;

                // Sending sms to the user
                BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);

                Dictionary<string, string> smsValues = new Dictionary<string, string>();
                smsValues.Add("#--NAME--#", lPD.FirstName);

                gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.MER_REG, new string[] { mbno, rcKey.DEFAULT_ALL_SMS }, smsValues);

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Merchant Approved Succesfully, there might be problem sending sms, please check your email or contact administrator!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantApprovalController][sendSMS]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantApprovalController][sendSMS]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
        }
    }
}
