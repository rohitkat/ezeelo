using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Transactions;
using System.Text.RegularExpressions;
using System.Web.Security;
using Merchant;

namespace Franchise.Controllers
{
    public class FranchiseRegisterNowController : Controller
    {
        private ModelLayer.Models.EzeeloDBContext db = new ModelLayer.Models.EzeeloDBContext();
        //
        // GET: /MerchantRegisterNow/
        
        public ActionResult Register()
        {
            ViewBag.Salutation = new SelectList(db.Salutations.ToList(), "ID", "Name", 1);
            ViewBag.SecurityQuestion = new SelectList(db.SecurityQuestions, "ID", "Question");
            
            return View();
        }

        [HttpPost]
        public ActionResult Register(ModelLayer.Models.ViewModel.FranchiseRegisterNowViewModel frnvm, int Salutation, int SecurityQuestion)
        {
            ViewBag.Salutation = new SelectList(db.Salutations.ToList(), "ID", "Name", 1);
            ViewBag.SecurityQuestion = new SelectList(db.SecurityQuestions, "ID", "Question");

            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    frnvm.SecurityQuestionID = SecurityQuestion;

                    long UserID = this.InsertLoginDetails(frnvm);

                    long personalDetailID = this.InsertPersonalDetails(frnvm, Salutation, UserID);

                    ModelLayer.Models.BusinessType busines = new ModelLayer.Models.BusinessType();

                    busines = db.BusinessTypes.Where(x => x.Prefix == "GBFR").FirstOrDefault();

                    if (busines == null)
                    {
                        throw new Exception("Business Type is not valid or does not exist");
                    }

                    ModelLayer.Models.Pincode pin = new ModelLayer.Models.Pincode();
                    pin = db.Pincodes.Where(x => x.Name == frnvm.PinCode).FirstOrDefault();

                    if (pin == null)
                    {
                        throw new Exception("Pincod is not valid or does not exist");
                    }

                    long businessID = this.InsertBussinessDetails(frnvm, UserID, busines.ID, pin.ID);


                    this.InsertSecurityQuestion(frnvm, UserID);

                    ts.Complete();

                    //this.SendEmail();
                    //this.SendSMS();

                    frnvm.LoginID = UserID;
                    frnvm.SalutationID = Salutation;
                    frnvm.ID = personalDetailID;
                    frnvm.BusinessID = businessID;
                    


                    TempData["FRNVM"] = frnvm;


                    frnvm = null;
                    ModelState.AddModelError("Message", "Franchise Registered Succesfully, please check your email and mobile for further verification!");

                    return RedirectToAction("Register", "FranchiseRegisterPost");
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
                    emailParaMetres.Add("<!--NAME-->", "Prashant");//merchantRegisterNow.FirstName);
                    emailParaMetres.Add("<!--URL-->", "" + (new ModelLayer.Models.ViewModel.URLsFromConfig()).GetURL("CUSTOMER") + "");
                    
                    gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.ACT_LINK, new string[] { "pnbhoyar@gmail.com" }, emailParaMetres, true);

                }
                catch (BusinessLogicLayer.MyException myEx)
                {
                    ModelState.AddModelError("Message", "Merchant Registered Succesfully, there might be problem sending email, please check your email or contact administrator!");

                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()+Environment.NewLine+ "Can't send Email..! " + myEx.EXCEPTION_MSG + Environment.NewLine + myEx.EXCEPTION_PATH, BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                }

                try
                {
                    // Sending email to the user
                    BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);

                    Dictionary<string,string> otp = BusinessLogicLayer.OTP.GenerateOTP("MRG");

                    Dictionary<string, string> smsValues = new Dictionary<string, string>();
                    smsValues.Add("#--NAME--#","Prashant");
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

                return View("Register");
            }
            else
            {
                Dictionary<string, string> lDictLoginDetails = this.CheckLogin(model.UserName, model.Password);

                if (lDictLoginDetails.Count() > 0)
                {
                    Session["ID"] = lDictLoginDetails["ID"];
                    Session["USER_LOGIN_ID"] = lDictLoginDetails["ID"];

                    ModelLayer.Models.PersonalDetail pd = new ModelLayer.Models.PersonalDetail();
                    long UserLoginID=Convert.ToInt64(lDictLoginDetails["ID"].ToString());
                    pd = db.PersonalDetails.Where(x => x.UserLoginID ==UserLoginID).FirstOrDefault();

                    Session["PERSONAL_ID"] = pd == null ? 0 : pd.ID;
                    
                    Session["USER_NAME"] = lDictLoginDetails["UserName"];
                    FormsAuthentication.SetAuthCookie(model.UserName, true);
                    return RedirectToAction("Index", "Home");

                }
            }
            return View("Register");
        }

        public ActionResult ForgotPassword(ModelLayer.Models.ViewModel.LoginViewModel loginView)
        {
            
            return View("Register");
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

        private void InsertSecurityQuestion(ModelLayer.Models.ViewModel.FranchiseRegisterNowViewModel frnvm, long UserID)
        {
            ModelLayer.Models.LoginSecurityAnswer security = new ModelLayer.Models.LoginSecurityAnswer();

            security.UserLoginID = UserID;
            security.SecurityQuestionID = frnvm.SecurityQuestionID;
            security.Answer = frnvm.SecurityAnswer;

            security.CreateBy = 1;
            security.CreateDate = DateTime.UtcNow.AddHours(5.30);
            security.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
            security.DeviceID = string.Empty;
            security.DeviceType = string.Empty;

            db.LoginSecurityAnswers.Add(security);
            db.SaveChanges();
        }

        private long InsertLoginDetails(ModelLayer.Models.ViewModel.FranchiseRegisterNowViewModel frnvm)
        {
            ModelLayer.Models.UserLogin userLogin = new ModelLayer.Models.UserLogin();

            userLogin.Mobile = frnvm.Mobile;
            userLogin.Email = frnvm.Email;
            userLogin.Password = frnvm.Password;
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

        private long InsertBussinessDetails(ModelLayer.Models.ViewModel.FranchiseRegisterNowViewModel frnvm, long userID, int bussinessID, int pincodeID)
        {
            ModelLayer.Models.BusinessDetail bussinesDetails = new ModelLayer.Models.BusinessDetail();
            bussinesDetails.UserLoginID = userID;
            bussinesDetails.Name = frnvm.FirstName;           
            bussinesDetails.ContactPerson = frnvm.FirstName + " " + frnvm.MiddleName + " " + frnvm.LastName;
            bussinesDetails.Mobile = frnvm.Mobile;
            bussinesDetails.Email = frnvm.Email;
            bussinesDetails.BusinessTypeID = bussinessID;
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

        private long InsertPersonalDetails(ModelLayer.Models.ViewModel.FranchiseRegisterNowViewModel frnvm, int salutationID, long userID)
        {
            ModelLayer.Models.PersonalDetail personalDetail = new ModelLayer.Models.PersonalDetail();

            personalDetail.UserLoginID = userID;
            personalDetail.SalutationID = salutationID;
            personalDetail.FirstName = frnvm.FirstName;
            personalDetail.MiddleName = frnvm.MiddleName;
            personalDetail.LastName = frnvm.LastName;
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