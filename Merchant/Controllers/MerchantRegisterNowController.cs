using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models.ViewModel;
using ModelLayer.Models;
using System.Transactions;
using System.Text.RegularExpressions;
using System.Web.Security;

namespace Merchant.Controllers
{
    public class MerchantRegisterNowController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        
        public ActionResult Create()
        {
            Session["USER_LOGIN_ID"] = null;
            ViewBag.Salutation = new SelectList(db.Salutations.ToList(), "ID", "Name", 1);
            ViewBag.SecurityQuestion = new SelectList(db.SecurityQuestions, "ID", "Question");
            //MerchantRegisterNowViewModel merchantRegisterNow = new MerchantRegisterNowViewModel();
            return View();
        }

        [HttpPost]
        public ActionResult Create(MerchantRegisterNowViewModel merchantRegisterNow, int Salutation, int SecurityQuestion)
        {
            ViewBag.Salutation = new SelectList(db.Salutations.ToList(), "ID", "Name", 1);
            ViewBag.SecurityQuestion = new SelectList(db.SecurityQuestions, "ID", "Question");

            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    merchantRegisterNow.SecurityQuestionID = SecurityQuestion;

                    long UserID = this.InsertLoginDetails(merchantRegisterNow);

                   
                    long personalDetailID = this.InsertPersonalDetails(merchantRegisterNow, Salutation, UserID);

                    ModelLayer.Models.BusinessType busines = new BusinessType();

                    busines = db.BusinessTypes.Where(x => x.Prefix == "GBMR").FirstOrDefault();

                    if (busines == null)
                    {
                        throw new Exception("Business Type is not valid or does not exist");
                    }

                    ModelLayer.Models.Pincode pin = new ModelLayer.Models.Pincode();
                    pin = db.Pincodes.Where(x => x.Name == merchantRegisterNow.PinCode).FirstOrDefault();

                    if (pin == null)
                    {
                        throw new Exception("Pincod is not valid or does not exist");
                    }

                    long businessID = this.InsertBussinessDetails(merchantRegisterNow, UserID, busines.ID, pin.ID);

                    long shopID = this.InsertShopDetails(merchantRegisterNow, businessID, pin.ID);

                    this.InsertSecurityQuestion(merchantRegisterNow, UserID);

                    ts.Complete();

                    //this.SendEmail();
                    //this.SendSMS();

                    //this.SendLoginDetailsEMail(UserID, merchantRegisterNow.FirstName, merchantRegisterNow.Mobile, merchantRegisterNow.Email, merchantRegisterNow.Password);

                   // this.SendWelcomeEMail(UserID, merchantRegisterNow.FirstName, merchantRegisterNow.Mobile, merchantRegisterNow.Email, merchantRegisterNow.Password);

                    merchantRegisterNow.LoginID = UserID;
                    merchantRegisterNow.SalutationID = Salutation;
                    merchantRegisterNow.ID = personalDetailID;
                    merchantRegisterNow.BusinessID = businessID;
                    merchantRegisterNow.ShopID = shopID;
                    
                    TempData["merchantRegisterNow"] = merchantRegisterNow;

                    
                    merchantRegisterNow = null;
                    ModelState.AddModelError("Message", "Merchant Registered Succesfully, please check your email and mobile for further verification!");

                    return RedirectToAction("Create", "MerchantRegistrationPost");
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    ModelState.AddModelError("Message", ex.Message);
                }

                
                
                try
                {
                    // Sending email to the user
                    BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                    
                    Dictionary<string, string> emailParaMetres = new Dictionary<string, string>();
                    emailParaMetres.Add("<!--ACCOUNT_URL-->", "" + (new URLsFromConfig()).GetURL("MERCHANT") + "");
                    emailParaMetres.Add("<!--ORDERS_URL-->", "" + (new URLsFromConfig()).GetURL("MERCHANT") + "");
                    emailParaMetres.Add("<!--NAME-->", merchantRegisterNow.FirstName);
                    emailParaMetres.Add("<!--URL-->", "" + (new URLsFromConfig()).GetURL("MERCHANT") + "");
                    
                   // gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.ACT_LINK, new string[] { merchantRegisterNow.Email }, emailParaMetres, true);

                }
                catch (BusinessLogicLayer.MyException myEx)
                {
                    ModelState.AddModelError("Message", "Merchant Registered Succesfully, there might be problem sending email, please check your email or contact administrator!");

                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()+Environment.NewLine+ "Can't send Email..! " + myEx.EXCEPTION_MSG + Environment.NewLine + myEx.EXCEPTION_PATH, BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                }

                try
                {
                    // Sending SMS to the user
                    BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);

                    Dictionary<string,string> otp = BusinessLogicLayer.OTP.GenerateOTP("MRG");

                    Dictionary<string, string> smsValues = new Dictionary<string, string>();
                    smsValues.Add("#--NAME--#", merchantRegisterNow.FirstName);
                    smsValues.Add("#--OTP--#", otp["OTP"]);

                   // gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.OTP_MER_REG, new string[] { merchantRegisterNow.Mobile }, smsValues);

                }
                catch (BusinessLogicLayer.MyException myEx)
                {
                    ModelState.AddModelError("Message", "Merchant Registered Succesfully, there might be problem sending SMS, please check your mobile or contact administrator!");

                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString() + Environment.NewLine + "Can't send SMS..! " + myEx.EXCEPTION_MSG + Environment.NewLine + myEx.EXCEPTION_PATH, BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                }
                
            }
            return View();
        }

        private void SendLoginDetailsEMail(long UserID, string firstName, string mobile, string email, string pwd)
        {
            try
            {
                // Sending email to the user
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);

                Dictionary<string, string> emailParaMetres = new Dictionary<string, string>();
                emailParaMetres.Add("<!--ACCOUNT_URL-->", "" + (new URLsFromConfig()).GetURL("MERCHANT") + "");
                emailParaMetres.Add("<!--ORDERS_URL-->", "" + (new URLsFromConfig()).GetURL("MERCHANT") + "placed");
                emailParaMetres.Add("<!--NAME-->", firstName);
                emailParaMetres.Add("<!--USER_ID-->", mobile + " or " + email);
                emailParaMetres.Add("<!--PWD-->", pwd.Trim());

                 gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.ACT_LINK, new string[] { email }, emailParaMetres, true);

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString() + Environment.NewLine + "Can't send Merchant Login Deatils - EMail..! " + myEx.EXCEPTION_MSG + Environment.NewLine + myEx.EXCEPTION_PATH + "[C:MerchantRegisterNowController][M:SendLoginDetailsEMail]", BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString() + Environment.NewLine + "Can't send Merchant Login Deatils - EMail..! " + ex.Message + Environment.NewLine + "[C:MerchantRegisterNowController][M:SendLoginDetailsEMail]", BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }

        }

        [HttpPost]
        public ActionResult Login(ModelLayer.Models.ViewModel.LoginViewModel model)
        {
            ViewBag.Salutation = new SelectList(db.Salutations.ToList(), "ID", "Name", 1);
            ViewBag.SecurityQuestion = new SelectList(db.SecurityQuestions, "ID", "Question");


            bool IsEmailValid = false, IsMobileValid = false;

            IsEmailValid = this.IsValidEmailId(model.UserName);

            if (IsEmailValid == false)
                IsMobileValid = this.IsValidMobile(model.UserName);

            if (IsEmailValid == false && IsMobileValid == false)
            {
                ViewBag.Message = "Invalid UserName/Password!!";
                TempData["Message"] = "Invalid UserName/Password!!";

                return View("Create");
            }
            else
            {
                Dictionary<string, string> lDictLoginDetails = this.CheckLogin(model.UserName, model.Password);

                if (lDictLoginDetails.Count() > 0)
                {
                    Session["ID"] = lDictLoginDetails["ID"];
                    Session["USER_LOGIN_ID"] = lDictLoginDetails["ID"];

                    ModelLayer.Models.PersonalDetail pd = new ModelLayer.Models.PersonalDetail();
                    long LoginID=Convert.ToInt64(lDictLoginDetails["ID"].ToString());
                    pd = db.PersonalDetails.Where(x => x.UserLoginID == LoginID).FirstOrDefault();

                    Session["PERSONAL_ID"] = pd == null ? 0 : pd.ID;

                    Session["USER_NAME"] = lDictLoginDetails["UserName"];
                    FormsAuthentication.SetAuthCookie(model.UserName, true);

                    return RedirectToAction("Index", "Home");

                }
            }
            return View("Create");
        }

        public ActionResult ForgotPassword(ModelLayer.Models.ViewModel.LoginViewModel loginView)
        {
            
            return View("Create");
        }

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

            var userExist = db.UserLogins.Select(x => new { x.Email, x.Mobile, x.Password, x.ID })
                                         .Where(x => (x.Email == pUserName || x.Mobile == pUserName) && x.Password == pPassword).ToList();
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
	
        private void InsertSecurityQuestion(MerchantRegisterNowViewModel merchantRegisterNow, long UserID)
        {
            ModelLayer.Models.LoginSecurityAnswer security = new ModelLayer.Models.LoginSecurityAnswer();

            security.UserLoginID = UserID;
            security.SecurityQuestionID = merchantRegisterNow.SecurityQuestionID;
            security.Answer = merchantRegisterNow.SecurityAnswer;

            security.CreateBy = 1;
            security.CreateDate = DateTime.UtcNow.AddHours(5.30);
            security.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
            security.DeviceID = string.Empty;
            security.DeviceType = string.Empty;

            db.LoginSecurityAnswers.Add(security);
            db.SaveChanges();
        }

        private long InsertShopDetails(MerchantRegisterNowViewModel merchantRegisterNow, long bussinesID, int pincodeID)
        {
            ModelLayer.Models.Shop shop = new ModelLayer.Models.Shop();

            shop.BusinessDetailID = bussinesID;
            shop.Name = merchantRegisterNow.BussinessName;
            shop.Website = merchantRegisterNow.Website;
            shop.PincodeID = pincodeID;
            shop.AreaID = null;
            shop.ContactPerson = merchantRegisterNow.FirstName + " " + merchantRegisterNow.MiddleName + " " + merchantRegisterNow.LastName;
            shop.Email = merchantRegisterNow.Email;
            shop.Mobile = merchantRegisterNow.Mobile;
            shop.VAT = merchantRegisterNow.WAT;
            shop.TIN = merchantRegisterNow.TIN;
            shop.PAN = merchantRegisterNow.PAN;
            shop.CurrentItSetup = false;
            shop.InstitutionalMerchantPurchase = false;
            shop.InstitutionalMerchantSale = false;
            shop.NormalSale = true;
            shop.IsDeliveryOutSource = false;
            shop.MinimumAmountForFreeDelivery = 0;
            shop.IsFreeHomeDelivery = false;
            shop.IsLive = false;
            shop.IsManageInventory = false;
            shop.SearchKeywords = merchantRegisterNow.BussinessName;
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

            return shop.ID;
        }

        private long InsertLoginDetails(MerchantRegisterNowViewModel merchantRegisterNow)
        {
            ModelLayer.Models.UserLogin userLogin = new ModelLayer.Models.UserLogin();

            userLogin.Mobile = merchantRegisterNow.Mobile;
            userLogin.Email = merchantRegisterNow.Email;
            userLogin.Password = merchantRegisterNow.Password;
            userLogin.IsLocked = true;
            userLogin.CreateBy = 1;
            userLogin.CreateDate = DateTime.UtcNow.AddHours(5.30);
            userLogin.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
            userLogin.DeviceID = string.Empty;
            userLogin.DeviceType = string.Empty;

            db.UserLogins.Add(userLogin);
            db.SaveChanges();

            return userLogin.ID;
        }

        private long InsertBussinessDetails(MerchantRegisterNowViewModel merchantRegisterNow, long userID, int bussinessID, int pincodeID)
        {
            ModelLayer.Models.BusinessDetail bussinesDetails = new ModelLayer.Models.BusinessDetail();
            bussinesDetails.UserLoginID = userID;
            bussinesDetails.Name = merchantRegisterNow.BussinessName;
            bussinesDetails.BusinessTypeID = bussinessID;
            bussinesDetails.ContactPerson = merchantRegisterNow.FirstName + " " + merchantRegisterNow.MiddleName + " " + merchantRegisterNow.LastName;
            bussinesDetails.Mobile = merchantRegisterNow.Mobile;
            bussinesDetails.Email = merchantRegisterNow.Email;
            bussinesDetails.Address = string.Empty;
            bussinesDetails.PincodeID = pincodeID;
            bussinesDetails.IsActive = true;
            bussinesDetails.CreateBy = 1;
            bussinesDetails.CreateDate = DateTime.UtcNow.AddHours(5.30);
            bussinesDetails.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
            bussinesDetails.DeviceID = string.Empty;
            bussinesDetails.DeviceType = string.Empty;

            db.BusinessDetails.Add(bussinesDetails);
            db.SaveChanges();
            return bussinesDetails.ID;
        }

        private long InsertPersonalDetails(MerchantRegisterNowViewModel merchantRegisterNow, int salutationID, long userID)
        {
            ModelLayer.Models.PersonalDetail personalDetail = new ModelLayer.Models.PersonalDetail();

            personalDetail.UserLoginID = userID;
            personalDetail.SalutationID = salutationID;
            personalDetail.FirstName = merchantRegisterNow.FirstName;
            personalDetail.MiddleName = merchantRegisterNow.MiddleName;
            personalDetail.LastName = merchantRegisterNow.LastName;
            personalDetail.IsActive = true;
            personalDetail.CreateBy = 1;
            personalDetail.CreateDate = DateTime.UtcNow.AddHours(5.30);
            personalDetail.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
            personalDetail.DeviceID = string.Empty;
            personalDetail.DeviceType = string.Empty;

            db.PersonalDetails.Add(personalDetail);
            db.SaveChanges();

            return personalDetail.ID;
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