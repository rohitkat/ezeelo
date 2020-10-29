using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using System.Transactions;
using System.Text.RegularExpressions;
using DeliveryPartner.Models.ViewModel;
using BusinessLogicLayer;
using ModelLayer.Models.ViewModel;


namespace DeliveryPartner.Controllers
{
    public class DeliveryPartnerRegisterController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        //
        // GET: /MerchantRegisterNow/
        
        public ActionResult Create()
        {
            ViewBag.Salutation = new SelectList(db.Salutations.ToList(), "ID", "Name", 1);
            ViewBag.SecurityQuestion = new SelectList(db.SecurityQuestions.Where(x => x.IsActive == true), "ID", "Question");
            var ServiceLevelList = from DeliveryPartner.Common.Constant.DELIVERY_PARTNER_SERVICE_LEVEL d in Enum.GetValues(typeof(DeliveryPartner.Common.Constant.DELIVERY_PARTNER_SERVICE_LEVEL))
                         select new { ID = (int)d, Name = d.ToString() };
            ViewBag.ServiceLevel = new SelectList(ServiceLevelList, "ID", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult Create(DeliveryPartnerRegisterViewModel deliveryPartnerRegister, int Salutation, int SecurityQuestion, int ServiceLevel)
        {
            ViewBag.Salutation = new SelectList(db.Salutations.ToList(), "ID", "Name", 1);
            ViewBag.SecurityQuestion = new SelectList(db.SecurityQuestions, "ID", "Question");
            var ServiceLevelList = from DeliveryPartner.Common.Constant.DELIVERY_PARTNER_SERVICE_LEVEL d in Enum.GetValues(typeof(DeliveryPartner.Common.Constant.DELIVERY_PARTNER_SERVICE_LEVEL))
                                   select new { ID = (int)d, Name = d.ToString() };
            ViewBag.ServiceLevel = new SelectList(ServiceLevelList, "ID", "Name");

            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    deliveryPartnerRegister.SecurityQuestionID = SecurityQuestion;

                    long UserID = this.InsertLoginDetails(deliveryPartnerRegister);

                    long personalDetailID = this.InsertPersonalDetails(deliveryPartnerRegister, Salutation, UserID);

                    ModelLayer.Models.BusinessType busines = new BusinessType();

                    busines = db.BusinessTypes.Where(x => x.Prefix == "GBDP").FirstOrDefault();

                    if (busines == null)
                    {
                        throw new Exception("Business Type is not valid or does not exist");
                    }

                    ModelLayer.Models.Pincode pin = new ModelLayer.Models.Pincode();
                    pin = db.Pincodes.Where(x => x.Name == deliveryPartnerRegister.PinCode).FirstOrDefault();

                    if (pin == null)
                    {
                        throw new Exception("Pincod is not valid or does not exist");
                    }

                    long BussinessDetailsID = this.InsertBussinessDetails(deliveryPartnerRegister, UserID, busines.ID, pin.ID, personalDetailID);

                    long DeliveryPartnerID = this.InsertDeliveryPartnerDetails(deliveryPartnerRegister, UserID, BussinessDetailsID, pin.ID, personalDetailID, ServiceLevel);

                    //long shopID = this.InsertShopDetails(deliveryPartnerRegister, businessID, pin.ID);

                    this.InsertSecurityQuestion(deliveryPartnerRegister, UserID);

                    ts.Complete();

                    //this.SendEmail();
                    //this.SendSMS();

                    //deliveryPartnerRegister.LoginID = UserID;
                    //deliveryPartnerRegister.SalutationID = Salutation;
                    //deliveryPartnerRegister.ID = personalDetailID;
                    //deliveryPartnerRegister.BusinessID = BussinessDetailsID;

                   // TempData["merchantRegisterNow"] = deliveryPartnerRegister;

                    
                    deliveryPartnerRegister = null;
                    //ModelState.AddModelError("Message", " “Congratulation!  You are successfully Register… Our executive Manager contacts you as soon as possible” ");
                    TempData["Title"] = "Congratulation!";
                    TempData["Description"] = "“You are successfully Register… Our executive Manager contacts you as soon as possible”";
                    //return RedirectToAction("Create", "MerchantRegistrationPost");
                    return View("RegistrationSuccess");
                }
                catch (Exception ex)
                {
                    ts.Dispose();

                    ModelState.AddModelError("Error", "There's something wrong with the delivery partner registration!");

                    //Code to write error log
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + ex.Message + Environment.NewLine
                        + "[DeliveryPartnerRegister][POST:Create]",
                        BusinessLogicLayer.ErrorLog.Module.DeliveryPartner, System.Web.HttpContext.Current.Server);
                }

                
               /* 
                try
                {
                    // Sending email to the user
                    BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                    Dictionary<string, string> emailParaMetres = new Dictionary<string, string>();
                    emailParaMetres.Add("<!--NAME-->", deliveryPartnerRegister.FirstName);
                    emailParaMetres.Add("<!--URL-->", "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "");
                    
                    //gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.ACT_LINK, new string[] { merchantRegisterNow.Email }, emailParaMetres, true);

                }
                catch (BusinessLogicLayer.MyException myEx)
                {
                    ModelState.AddModelError("Message", "You Registered Succesfully, there might be problem sending email, please check your email or contact administrator!");

                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()+Environment.NewLine+ "Can't send Email..! " + myEx.EXCEPTION_MSG + Environment.NewLine + myEx.EXCEPTION_PATH, BusinessLogicLayer.ErrorLog.Module.DeliveryPartner, System.Web.HttpContext.Current.Server);
                }

                try
                {
                    // Sending email to the user
                    BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);

                    Dictionary<string,string> otp = BusinessLogicLayer.OTP.GenerateOTP("MRG");

                    Dictionary<string, string> smsValues = new Dictionary<string, string>();
                    smsValues.Add("#--NAME--#", deliveryPartnerRegister.FirstName);
                    smsValues.Add("#--OTP--#", otp["OTP"]);

                   // gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.OTP_MER_REG, new string[] { merchantRegisterNow.Mobile }, smsValues);

                }
                catch (BusinessLogicLayer.MyException myEx)
                {
                    ModelState.AddModelError("Message", "You Registered Succesfully, there might be problem sending SMS, please check your mobile or contact administrator!");

                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString() + Environment.NewLine + "Can't send SMS..! " + myEx.EXCEPTION_MSG + Environment.NewLine + myEx.EXCEPTION_PATH, BusinessLogicLayer.ErrorLog.Module.DeliveryPartner, System.Web.HttpContext.Current.Server);
                }
                */
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

                return View("Create");
            }
            else
            {
                Dictionary<string, string> lDictLoginDetails = this.CheckLogin(model.UserName, model.Password);

                if (lDictLoginDetails.Count() > 0)
                {

                    Session["USER_LOGIN_ID"] = lDictLoginDetails["ID"];
                    Session["USER_NAME"] = lDictLoginDetails["UserName"];


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

        private long InsertLoginDetails(DeliveryPartnerRegisterViewModel deliveryPartnerRegister)
        {
            ModelLayer.Models.UserLogin userLogin = new ModelLayer.Models.UserLogin();

            userLogin.Mobile = deliveryPartnerRegister.Mobile;
            userLogin.Email = deliveryPartnerRegister.Email;
            userLogin.Password = deliveryPartnerRegister.Password;
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

        private long InsertPersonalDetails(DeliveryPartnerRegisterViewModel deliveryPartnerRegister, int salutationID, long userID)
        {
            ModelLayer.Models.PersonalDetail personalDetail = new ModelLayer.Models.PersonalDetail();

            personalDetail.UserLoginID = userID;
            personalDetail.SalutationID = salutationID;
            personalDetail.FirstName = deliveryPartnerRegister.FirstName;
            personalDetail.MiddleName = deliveryPartnerRegister.MiddleName;
            personalDetail.LastName = deliveryPartnerRegister.LastName;
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

        private long InsertBussinessDetails(DeliveryPartnerRegisterViewModel deliveryPartnerRegister, long userID, int bussinessID, int pincodeID, long personalDetailID)
        {
            ModelLayer.Models.BusinessDetail bussinesDetails = new ModelLayer.Models.BusinessDetail();
            bussinesDetails.UserLoginID = userID;
            bussinesDetails.Name = deliveryPartnerRegister.BussinessName;
            bussinesDetails.BusinessTypeID = bussinessID;
            bussinesDetails.ContactPerson = deliveryPartnerRegister.FirstName + " " + deliveryPartnerRegister.MiddleName + " " + deliveryPartnerRegister.LastName;
            bussinesDetails.Mobile = deliveryPartnerRegister.Mobile;
            bussinesDetails.Email = deliveryPartnerRegister.Email;

            bussinesDetails.PincodeID = pincodeID;
            bussinesDetails.IsActive = true;
            bussinesDetails.CreateBy = personalDetailID;
            bussinesDetails.CreateDate = DateTime.UtcNow.AddHours(5.30);
            bussinesDetails.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
            bussinesDetails.DeviceID = string.Empty;
            bussinesDetails.DeviceType = string.Empty;
            bussinesDetails.Address = string.Empty;
            db.BusinessDetails.Add(bussinesDetails);
            db.SaveChanges();
            return bussinesDetails.ID;
        }

        private long InsertDeliveryPartnerDetails(DeliveryPartnerRegisterViewModel deliveryPartnerRegister, long userID, long BussinessDetailsID, int pincodeID, long personalDetailID, int ServiceLevel)
        {
            ModelLayer.Models.DeliveryPartner deliveryPartner = new ModelLayer.Models.DeliveryPartner();
            deliveryPartner.BusinessDetailID = BussinessDetailsID;
            deliveryPartner.GodownAddress = string.Empty;
            deliveryPartner.PincodeID = pincodeID;
            deliveryPartner.ContactPerson = deliveryPartnerRegister.FirstName + " " + deliveryPartnerRegister.MiddleName + " " + deliveryPartnerRegister.LastName;
            deliveryPartner.Mobile = deliveryPartnerRegister.Mobile;
            deliveryPartner.Email = deliveryPartnerRegister.Email;

            deliveryPartner.PincodeID = pincodeID;
            deliveryPartner.ServiceNumber = string.Empty;
            deliveryPartner.ServiceLevel = ServiceLevel;
            deliveryPartner.Mobile = deliveryPartnerRegister.Mobile;
            deliveryPartner.Email = deliveryPartnerRegister.Email;
            deliveryPartner.VehicleTypeID = 1;
            deliveryPartner.OpeningTime = DateTime.Now.TimeOfDay;
            deliveryPartner.ClosingTime = DateTime.Now.TimeOfDay;
            deliveryPartner.IsLive = false;
            deliveryPartner.IsActive = false;

            deliveryPartner.CreateBy = personalDetailID;
            deliveryPartner.CreateDate = DateTime.UtcNow.AddHours(5.30);
            deliveryPartner.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
            deliveryPartner.DeviceID = string.Empty;
            deliveryPartner.DeviceType = string.Empty;
            db.DeliveryPartners.Add(deliveryPartner);
            db.SaveChanges();
            return deliveryPartner.ID;
        }

        private void InsertSecurityQuestion(DeliveryPartnerRegisterViewModel deliveryPartnerRegister, long UserID)
        {
            ModelLayer.Models.LoginSecurityAnswer security = new ModelLayer.Models.LoginSecurityAnswer();

            security.UserLoginID = UserID;
            security.SecurityQuestionID = deliveryPartnerRegister.SecurityQuestionID;
            security.Answer = deliveryPartnerRegister.SecurityAnswer;

            security.CreateBy = 1;
            security.CreateDate = DateTime.UtcNow.AddHours(5.30);
            security.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
            security.DeviceID = string.Empty;
            security.DeviceType = string.Empty;

            db.LoginSecurityAnswers.Add(security);
            db.SaveChanges();
        }


        public ActionResult GetAddress(string Pincode)
        {
            /*This Action Responces to AJAX Call
             * After entering Pincode returens City, District and State Information
             * */
            if (!(db.Pincodes.Any(p => p.Name == Pincode)))
            {
                var errorMsg = "Pincode Dose Not Exist, Please Contact Admin!";
                return View(new { success = false, Error = errorMsg });
            }

            long CityId = db.Pincodes.Single(p => p.Name == Pincode).CityID;
            ViewBag.City = db.Cities.Single(c => c.ID == CityId).Name.ToString();

            long DistrictId = db.Cities.Single(c => c.ID == CityId).DistrictID;
            ViewBag.District = db.Districts.Single(d => d.ID == DistrictId).Name.ToString();

            long StateId = db.Districts.Single(d => d.ID == DistrictId).StateID;
            ViewBag.State = db.States.Single(d => d.ID == StateId).Name.ToString();

            return PartialView("_Address");
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

        public JsonResult IsEmailAvailable(string Email)
        {
            if (db.UserLogins.Any(x => x.Email == Email))
            {
                return Json("1", JsonRequestBehavior.AllowGet);
            }
            return Json("0", JsonRequestBehavior.AllowGet);
        }
    }
}