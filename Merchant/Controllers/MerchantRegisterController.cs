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
using BusinessLogicLayer;

namespace Merchant.Controllers
{
    public class MerchantRegisterController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        // GET: /MerchantRegister/
        public ActionResult Index()
        {
            ViewBag.Salutation = new SelectList(db.Salutations, "ID", "Name", 1);
            ViewBag.SecurityQuestion = new SelectList(db.SecurityQuestions.Where(c => c.IsActive == true), "ID", "Question");
            return View();
        }
        [HttpPost]
        public ActionResult Index(MerchantRegisterViewModel merchantRegisterViewModel, int salutation, string Pincode, string Mobile)
        {
            //switch (submit)
            //{
            //    case "Save":

            using (var dbContextTransaction = db.Database.BeginTransaction())
            {

                try
                {
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
                    shop.FranchiseID = 1;//-----------------------------------------------
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

                    dbContextTransaction.Commit();

                    //this.SendWelcomeSMS(bussinesDetails.ID, merchantRegisterViewModel.personalDetail.FirstName, Mobile);

                    this.SendSignUpSMS(bussinesDetails.ID, merchantRegisterViewModel.personalDetail.FirstName, Mobile, merchantRegisterViewModel.userLogin.Email);

                    this.SendLoginDetailsEMail(bussinesDetails.ID, merchantRegisterViewModel.personalDetail.FirstName, Mobile, merchantRegisterViewModel.userLogin.Email, userLogin.Password);

                    this.SendWelcomeEMail(bussinesDetails.ID, merchantRegisterViewModel.personalDetail.FirstName, Mobile, merchantRegisterViewModel.userLogin.Email, userLogin.Password);

                    //return RedirectToAction("View1", "MerchantRegister");
                    ViewBag.Message1 = "Register Successfully! Our Executive will contact you within 24 hour !!";
                    TempData["Message1"] = "Register Successfully! Our Executive will contact you within 24 hour !!";
                }
                catch
                {
                    ViewBag.Message1 = "Theres somthiong wrong with databse, chnages not saved !!";
                    TempData["Message1"] = "Theres somthiong wrong with databse, chnages not saved !!";
                }
            }
            return RedirectToAction("Index", "MerchantRegister");

        }

        private void SendSignUpSMS(long UserID, string firstName, string Mobile, string email)
        {
            try
            {
                // Sending email to the user
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);

                Dictionary<string, string> smsParaMetres = new Dictionary<string, string>();
                smsParaMetres.Add("#--NAME--#", firstName);
                smsParaMetres.Add("#--LOGIN_ID--#", Mobile + " or " + email);

                gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.MER_SIGN_UP, new string[] { Mobile }, smsParaMetres);

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString() + Environment.NewLine + "Can't send Merchant Welcome - SMS..! " + myEx.EXCEPTION_MSG + Environment.NewLine + myEx.EXCEPTION_PATH + "[C:MerchantRegisterController][M:SendSignUpSMS]", BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString() + Environment.NewLine + "Can't send Merchant Welcome - SMS..! " + ex.Message + Environment.NewLine + "[C:MerchantRegisterController][M:SendSignUpSMS]", BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
        }

        private void SendWelcomeSMS(long UserID, string firstName, string Mobile)
        {
            try
            {
                // Sending email to the user
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);

                Dictionary<string, string> smsParaMetres = new Dictionary<string, string>();
                smsParaMetres.Add("#--NAME--#", firstName);
                smsParaMetres.Add("#--REG_NUM--#", UserID.ToString());

                gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.MER_WELCOME, new string[] { Mobile }, smsParaMetres);

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString() + Environment.NewLine + "Can't send Merchant Welcome - SMS..! " + myEx.EXCEPTION_MSG + Environment.NewLine + myEx.EXCEPTION_PATH + "[C:MerchantRegisterController][M:SendWelcomeSMS]", BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString() + Environment.NewLine + "Can't send Merchant Welcome - SMS..! " + ex.Message + Environment.NewLine + "[C:MerchantRegisterController][M:SendWelcomeSMS]", BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
        }

        private void SendWelcomeEMail(long UserID, string firstName, string mobile, string email, string pwd)
        {
            try
            {
                // Sending email to the user
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);

                Dictionary<string, string> emailParaMetres = new Dictionary<string, string>();
                emailParaMetres.Add("<!--ACCOUNT_URL-->", "" + (new URLsFromConfig()).GetURL("MERCHANT") + "");
                emailParaMetres.Add("<!--ORDERS_URL-->", "" + (new URLsFromConfig()).GetURL("MERCHANT") + "");
                emailParaMetres.Add("<!--NAME-->", firstName);
                emailParaMetres.Add("<!--REG_NO-->", UserID.ToString());

                gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.MER_WELCOME, new string[] { email }, emailParaMetres, true);

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString() + Environment.NewLine + "Can't send Merchant Welcome - EMail..! " + myEx.EXCEPTION_MSG + Environment.NewLine + myEx.EXCEPTION_PATH + "[C:MerchantRegisterController][M:SendWelcomeEMail]", BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString() + Environment.NewLine + "Can't send Merchant Welcome - EMail..! " + ex.Message + Environment.NewLine + "[C:MerchantRegisterController][M:SendWelcomeEMail]", BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }

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
                emailParaMetres.Add("<!--URL-->", "" + (new URLsFromConfig()).GetURL("MERCHANT") + "");

                //gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.ACT_LINK, new string[] { email }, emailParaMetres, true);

                gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.MERCHANT_ID, new string[] { email }, emailParaMetres, true);

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

        public ActionResult LogOff()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "MerchantRegister");
        }

        public ActionResult Login(MerchantRegisterViewModel merchantRegisterViewModel)
        {
            ViewBag.Salutation = new SelectList(db.Salutations.ToList(), "ID", "Name", 1);
            ViewBag.SecurityQuestion = new SelectList(db.SecurityQuestions, "ID", "Question");

            if (merchantRegisterViewModel.loginViewModel.UserName == null)
            {
                ViewBag.Message = "Plz fill username!";
                TempData["Message"] = "Plz fill username!";
                return RedirectToAction("Index", "MerchantRegister");
            }

            if (merchantRegisterViewModel.loginViewModel.Password == null)
            {
                ViewBag.Message = "Plz fill Password!";
                TempData["Message"] = "Plz fill Password!";
                return RedirectToAction("Index", "MerchantRegister");
            }


            bool IsEmailValid = false, IsMobileValid = false;

            IsEmailValid = this.IsValidEmailId(merchantRegisterViewModel.loginViewModel.UserName);

            if (IsEmailValid == false)
                IsMobileValid = this.IsValidMobile(merchantRegisterViewModel.loginViewModel.UserName);

            if (IsEmailValid == false && IsMobileValid == false)
            {
                ViewBag.Message = "Invalid UserName/Password!!";
                TempData["Message"] = "Invalid UserName/Password!!";

                //return View("Create");
                return RedirectToAction("Index", "MerchantRegister");
            }
            else
            {
                Dictionary<string, string> lDictLoginDetails = this.CheckLogin(merchantRegisterViewModel.loginViewModel.UserName, merchantRegisterViewModel.loginViewModel.Password);

                if (lDictLoginDetails.Count() <= 0)
                {
                    ViewBag.Message = "Invalid UserName/Password!!";
                    TempData["Message"] = "Invalid UserName/Password!!";

                    //return View("Create");
                    return RedirectToAction("Index", "MerchantRegister");
                }
                else
                {
                    long UserLoginID = Convert.ToInt64(lDictLoginDetails["ID"].ToString());

                    long businessDetailID = db.BusinessDetails.Where(x => x.UserLoginID == UserLoginID && x.BusinessType.Prefix == "GBMR").Select(x => x.ID).FirstOrDefault();
                    var IsExist = db.Shops.Where(x => x.BusinessDetailID == businessDetailID && x.IsActive == true).ToList();
                    //-----------------for checking plan expiration date----------------
                    DateTime ExpirationDate = (from op in db.OwnerPlans
                                               join s in db.Shops on op.OwnerID equals s.ID
                                               join bd in db.BusinessDetails on s.BusinessDetailID equals bd.ID
                                               join p in db.Plans on op.PlanID equals p.ID
                                               where p.PlanCode.Contains("GBMR") && bd.UserLoginID == UserLoginID 
                                               && p.IsActive==true && op.IsActive==true 
                                               select op.EndDate).FirstOrDefault();

                    //-------------------------------------------------------------------
                    if (IsExist.Count > 0)
                    {
                        if (ExpirationDate >= System.DateTime.UtcNow)
                        {



                            Session["ID"] = lDictLoginDetails["ID"];
                            Session["USER_LOGIN_ID"] = lDictLoginDetails["ID"];

                            //=============== call method to get no. of days remaining for plan expiration =================
                            this.GetRemainingDaysByPlan();


                            ModelLayer.Models.PersonalDetail pd = new ModelLayer.Models.PersonalDetail();
                            long LoginID = Convert.ToInt64(lDictLoginDetails["ID"].ToString());
                            pd = db.PersonalDetails.Where(x => x.UserLoginID == LoginID).FirstOrDefault();

                            Session["PERSONAL_ID"] = pd == null ? 0 : pd.ID;

                            Session["USER_NAME"] = lDictLoginDetails["UserName"];
                            FormsAuthentication.SetAuthCookie(merchantRegisterViewModel.loginViewModel.UserName, true);
                            //==============================
                            string ShopName = db.Shops.Where(x => x.BusinessDetailID == businessDetailID && x.IsActive == true).Select(x => x.Name).FirstOrDefault();
                            Session["ShopName"] = ShopName;
                            if (ShopName == string.Empty)
                            {
                                TempData["Title"] = "Congratulation!";
                                TempData["Description"] = "“You are successfully Register… Our executive Manager contacts you as soon as possible”";
                                //return RedirectToAction("Create", "MerchantRegistrationPost");
                                return RedirectToAction("Edit1", "PersonalDetail");
                            }
                            //===============================
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ViewBag.Message = "Your plan is Expired...Please contact to Admin";
                            TempData["Message"] = "Your plan is Expired...Please contact to Admin";
                            return RedirectToAction("Index", "MerchantRegister");
                        }
                    }
                    else
                    {
                        ViewBag.Message = "You are not a Merchant";
                        TempData["Message"] = "You are not a Merchant";

                        //return View("Create");
                        return RedirectToAction("Index", "MerchantRegister");
                    }


                }
            }
            //return View("Create");
            //return RedirectToAction("Index", "MerchantRegister");

        }

        public ActionResult View1()
        {
            return View();
        }

        public ActionResult GetAddress(string Pincode)
        {
            /*This Action Responces to AJAX Call
             * After entering Pincode returens City, District and State Information
             * */
            if (!(db.Pincodes.Any(p => p.Name == Pincode)))
            {
                //var errorMsg = "Pincode Dose Not Exist, Please Contact Admin!";
                //return View(new { success = false, Error = errorMsg });
                return Json("1", JsonRequestBehavior.AllowGet);
            }

            //long CityId = db.Pincodes.Single(p => p.Name == Pincode).CityID;
            //ViewBag.City = db.Cities.Single(c => c.ID == CityId).Name.ToString();

            //long DistrictId = db.Cities.Single(c => c.ID == CityId).DistrictID;
            //ViewBag.District = db.Districts.Single(d => d.ID == DistrictId).Name.ToString();

            //long StateId = db.Districts.Single(d => d.ID == DistrictId).StateID;
            //ViewBag.State = db.States.Single(d => d.ID == StateId).Name.ToString();

            //return View(new { success = true});
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
        public void GetRemainingDaysByPlan()
        {
            DataTable dt = new DataTable();
            if (Session["USER_LOGIN_ID"] != null)
            {
                ReadConfig config = new ReadConfig(System.Web.HttpContext.Current.Server);
                DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
                List<object> paramValues = new List<object>();
                paramValues.Add(Convert.ToInt64(Session["USER_LOGIN_ID"]));
                paramValues.Add("GBMR");
                dt = dbOpr.GetRecords("SelectRemainingPlanDays", paramValues);
                if (dt != null && dt.Rows.Count > 0 && Convert.ToInt32(dt.Rows[0]["DiffDate"].ToString()) <= 30)
                {
                    Session["PlanExpDaysRem"] = dt.Rows[0]["DiffDate"].ToString();
                }
            }
            //return dt;
        }
    }
}
