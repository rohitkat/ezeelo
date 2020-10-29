using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using System.Web.Security;
using System.Text.RegularExpressions;
using System.Data;

namespace Inventory.Controllers
{
    public class WarehouseRegisterController : Controller
    {
        //
        // GET: /WarehouseRegister/
        private EzeeloDBContext db = new EzeeloDBContext();

        // GET: /MerchantRegister/
               
        //public ActionResult Index()
        //{
        //    ViewBag.Salutation = new SelectList(db.Salutations, "ID", "Name", 1);
        //    ViewBag.SecurityQuestion = new SelectList(db.SecurityQuestions.Where(c => c.IsActive == true), "ID", "Question");
        //    return View();
        //}


        public ActionResult Index()
        {
            WarehouseRegisterViewModelList TOVML = new WarehouseRegisterViewModelList();
            WarehouseRegisterViewModel tovm = new WarehouseRegisterViewModel();
            //long ShopID = GetShopID();
            try
            {
                //var IsDeliveryOutSource = (from S in db.Shops.Where(x => x.ID.Equals(ShopID)) select new { S.IsDeliveryOutSource }).First();
                              
                    var queryResult = (
                from CO in db.Warehouses
                //join COD in db.BusinessDetails on CO.BusinessDetailID equals COD.ID                
                //join UL in db.UserLogins on COD.UserLoginID equals UL.ID     
                //join C in db.Cities on  CO.CityID equals C.ID
                //join S in db.States on CO.StateID equals S.ID
                //where COD.ShopID.Equals(ShopID) && COD.OrderStatus.Equals((int)ORDER_STATUS.DELIVERED)               
                select new
                {
                    CO.ID,                 
                    CO.Name,
                    CO.WarehouseCode,
                    CO.GSTNumber,
                    //C.Name,
                    //S.Name,        
                    //COD.Mobile,
                    //COD.Address,
                    //COD.ContactPerson,
                    CO.IsActive,
                    CO.CreateDate
                }).OrderByDescending(x => x.CreateDate).ToList();

                    List<WarehouseRegisterViewModel> listWarehouseList = new List<WarehouseRegisterViewModel>();

                //    //List<TrackOrderViewModel> listTrackOrder = new List<TrackOrderViewModel>();

                    foreach (var ReadRecord in queryResult)
                    {
                        //WarehouseRegisterViewModel tovm = new WarehouseRegisterViewModel();
                        tovm.ID = ReadRecord.ID;
                        tovm.warehouse.Name = ReadRecord.Name;
                        tovm.warehouse.WarehouseCode = ReadRecord.WarehouseCode;
                        tovm.warehouse.CreateDate = ReadRecord.CreateDate;
                        tovm.warehouse.IsActive = ReadRecord.IsActive;
                        //tovm.businessDetail.Mobile = ReadRecord.Mobile;
                        //tovm.businessDetail.Address = ReadRecord.Address;
                        //tovm.businessDetail.ContactPerson = ReadRecord.ContactPerson;
                        listWarehouseList.Add(tovm);
                    }

                //    TOVML.LWarehouseRegisterViewModelList = listWarehouseList;

                    //tovm = queryResult.ToList();
                
               
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[DeliveredController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DeliveredController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View(TOVML);
        }

        public ActionResult Create()
        {
            //ViewBag.Salutation = new SelectList(db.Salutations, "ID", "Name", 1);
            //ViewBag.SecurityQuestion = new SelectList(db.SecurityQuestions.Where(c => c.IsActive == true), "ID", "Question");
            return View();
        }
        [HttpPost]
        public ActionResult Create(WarehouseRegisterViewModel warehouseRegisterViewModel, int salutation, string Pincode, string Mobile)
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
                    userLogin.Mobile = Mobile;// warehouseRegisterViewModel.userLogin.Mobile;
                    userLogin.Email = warehouseRegisterViewModel.userLogin.Email;
                    userLogin.Password = warehouseRegisterViewModel.userLogin.Password;
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
                    personalDetail.SalutationID = salutation;// warehouseRegisterViewModel.salutation.ID;
                    personalDetail.FirstName = warehouseRegisterViewModel.personalDetail.FirstName;
                    personalDetail.MiddleName = warehouseRegisterViewModel.personalDetail.MiddleName;
                    personalDetail.LastName = warehouseRegisterViewModel.personalDetail.LastName;
                    personalDetail.DOB = warehouseRegisterViewModel.personalDetail.DOB;
                    personalDetail.Gender = warehouseRegisterViewModel.personalDetail.Gender;
                    personalDetail.PincodeID = pin.ID; // warehouseRegisterViewModel.personalDetail.PincodeID;
                    personalDetail.Address = warehouseRegisterViewModel.personalDetail.Address;
                    personalDetail.AlternateMobile = warehouseRegisterViewModel.personalDetail.AlternateMobile;
                    personalDetail.AlternateEmail = warehouseRegisterViewModel.personalDetail.AlternateEmail;
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
                    bussinesDetails.Name = string.Empty;// warehouseRegisterViewModel.businessDetail.Name;
                    bussinesDetails.BusinessTypeID = 1; //warehouseRegisterViewModel.businessDetail.BusinessTypeID;
                    bussinesDetails.ContactPerson = warehouseRegisterViewModel.personalDetail.FirstName + " " + warehouseRegisterViewModel.personalDetail.LastName; //warehouseRegisterViewModel.businessDetail.ContactPerson;
                    bussinesDetails.Mobile = Mobile;// warehouseRegisterViewModel.businessDetail.Mobile;
                    bussinesDetails.Email = warehouseRegisterViewModel.userLogin.Email; //warehouseRegisterViewModel.businessDetail.Email;
                    bussinesDetails.PincodeID = pin.ID; //warehouseRegisterViewModel.businessDetail.PincodeID;
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
                    shop.Website = warehouseRegisterViewModel.shop.Website;
                    shop.PincodeID = pin.ID;
                    shop.AreaID = null;
                    shop.ContactPerson = warehouseRegisterViewModel.personalDetail.FirstName + " " + warehouseRegisterViewModel.personalDetail.LastName;
                    shop.Email = warehouseRegisterViewModel.userLogin.Email;
                    shop.Mobile = Mobile;
                    // shop.VAT = warehouseRegisterViewModel.shop.WAT;
                    shop.TIN = warehouseRegisterViewModel.shop.TIN;
                    shop.PAN = warehouseRegisterViewModel.shop.PAN;
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

                    //this.SendWelcomeSMS(bussinesDetails.ID, warehouseRegisterViewModel.personalDetail.FirstName, Mobile);

                    this.SendSignUpSMS(bussinesDetails.ID, warehouseRegisterViewModel.personalDetail.FirstName, Mobile, warehouseRegisterViewModel.userLogin.Email);

                    this.SendLoginDetailsEMail(bussinesDetails.ID, warehouseRegisterViewModel.personalDetail.FirstName, Mobile, warehouseRegisterViewModel.userLogin.Email, userLogin.Password);

                    this.SendWelcomeEMail(bussinesDetails.ID, warehouseRegisterViewModel.personalDetail.FirstName, Mobile, warehouseRegisterViewModel.userLogin.Email, userLogin.Password);

                    //return RedirectToAction("View1", "WarehouseRegister");
                    ViewBag.Message1 = "Register Successfully! Our Executive will contact you within 24 hour !!";
                    TempData["Message1"] = "Register Successfully! Our Executive will contact you within 24 hour !!";
                }
                catch
                {
                    ViewBag.Message1 = "Theres somthiong wrong with databse, chnages not saved !!";
                    TempData["Message1"] = "Theres somthiong wrong with databse, chnages not saved !!";
                }
            }
            return RedirectToAction("Index", "WarehouseRegister");

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
                emailParaMetres.Add("<!--ORDERS_URL-->", "" + (new URLsFromConfig()).GetURL("MERCHANT") + "placed");
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
                emailParaMetres.Add("<!--ORDERS_URL-->", "" + (new URLsFromConfig()).GetURL("MERCHANT") + "/placed");
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
            return RedirectToAction("Index", "WarehouseRegister");
        }

        public ActionResult Login(WarehouseRegisterViewModel warehouseRegisterViewModel)
        {
            ViewBag.Salutation = new SelectList(db.Salutations.ToList(), "ID", "Name", 1);
            ViewBag.SecurityQuestion = new SelectList(db.SecurityQuestions, "ID", "Question");

            if (warehouseRegisterViewModel.loginViewModel.UserName == null)
            {
                ViewBag.Message = "Plz fill username!";
                TempData["Message"] = "Plz fill username!";
                return RedirectToAction("Index", "WarehouseRegister");
            }

            if (warehouseRegisterViewModel.loginViewModel.Password == null)
            {
                ViewBag.Message = "Plz fill Password!";
                TempData["Message"] = "Plz fill Password!";
                return RedirectToAction("Index", "WarehouseRegister");
            }


            bool IsEmailValid = false, IsMobileValid = false;

            IsEmailValid = this.IsValidEmailId(warehouseRegisterViewModel.loginViewModel.UserName);

            if (IsEmailValid == false)
                IsMobileValid = this.IsValidMobile(warehouseRegisterViewModel.loginViewModel.UserName);

            if (IsEmailValid == false && IsMobileValid == false)
            {
                ViewBag.Message = "Invalid UserName/Password!!";
                TempData["Message"] = "Invalid UserName/Password!!";

                //return View("Create");
                return RedirectToAction("Index", "WarehouseRegister");
            }
            else
            {
                Dictionary<string, string> lDictAdminLoginDetails = this.CheckAdminLogin(warehouseRegisterViewModel.loginViewModel.UserName, warehouseRegisterViewModel.loginViewModel.Password);

                if (lDictAdminLoginDetails.Count() <= 0)
                {
                    Dictionary<string, string> lDictLoginDetails = this.CheckLogin(warehouseRegisterViewModel.loginViewModel.UserName, warehouseRegisterViewModel.loginViewModel.Password);

                    if (lDictLoginDetails.Count() <= 0)
                    {
                        ViewBag.Message = "Invalid UserName/Password!!";
                        TempData["Message"] = "Invalid UserName/Password!!";

                        //return View("Create");
                        return RedirectToAction("Index", "WarehouseRegister");
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
                                                   && p.IsActive == true && op.IsActive == true
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
                                FormsAuthentication.SetAuthCookie(warehouseRegisterViewModel.loginViewModel.UserName, true);
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
                                return RedirectToAction("Index", "WarehouseRegister");
                            }
                        }
                        else
                        {
                            ViewBag.Message = "You are not a Warehouse Keeper";
                            TempData["Message"] = "You are not a Merchant";

                            //return View("Create");
                            return RedirectToAction("Index", "WarehouseRegister");
                        }
                    }
                }
                else
                {
                    var str = HttpContext.Request.RawUrl;
                    var controller = RouteData.Values["controller"].ToString();
                    var action = RouteData.Values["action"].ToString();

                    Session["ID"] = lDictAdminLoginDetails["ID"];
                    @Session["USER_NAME"] = lDictAdminLoginDetails["UserName"];
                    Session["RoleName"] = lDictAdminLoginDetails["RoleName"];
                    FormsAuthentication.SetAuthCookie(warehouseRegisterViewModel.loginViewModel.UserName, true);
                    string Rurl = Request.QueryString["ReturnURL"];
                    return RedirectToAction("Index", "Home");
                }              
            }
            //return View("Create");
            //return RedirectToAction("Index", "WarehouseRegister");

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


        //Check Admin login
        private Dictionary<string, string> CheckAdminLogin(string pUserName, string pPassword)
        {
            Dictionary<string, string> lDictUserDetails = new Dictionary<string, string>();

            //var userExist = db.UserLogins.Select(x => new { x.Email, x.Mobile, x.Password, x.ID })
            //                             .Where(x => (x.Email == pUserName || x.Mobile == pUserName) && x.Password == pPassword).ToList();

            //List<UserLoginDetail> userExist = new List<UserLoginDetail>();
            Authenticate authenticate = new BusinessLogicLayer.Authenticate();
            DataTable userExist = new DataTable();

            userExist = authenticate.AuthenticateAdmin(System.Web.HttpContext.Current.Server, pUserName, pPassword);

            if (userExist.Rows.Count > 0)
            {
                for (int i = 0; i < userExist.Rows.Count; i++)
                {
                    try
                    {
                        //var RoleID = db.UserRoles.Where(x => x.UserLoginID == item.ID).FirstOrDefault().RoleID;
                        //var RoleName = db.Roles.Where(x => x.ID == RoleID).FirstOrDefault().Name;
                        //if (RoleName.ToString().ToUpper().Equals("ADMIN") || RoleName.ToString().ToUpper().Equals("SUPER_ADMIN"))
                        //{
                        //lDictUserDetails.Add("ID", item.ID.ToString());
                        //lDictUserDetails.Add("UserName", item.Email.ToString());
                        //lDictUserDetails.Add("RoleName", RoleName.ToString());
                        //}

                        lDictUserDetails.Add("ID", userExist.Rows[i]["ID"].ToString());
                        lDictUserDetails.Add("UserName", userExist.Rows[i]["UserName"].ToString());
                        lDictUserDetails.Add("RoleName", userExist.Rows[i]["RoleName"].ToString());
                    }
                    catch (Exception ex)
                    {
                        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                            + Environment.NewLine + ex.Message + Environment.NewLine
                            + "[LoginController][CheckLogin]",
                            BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                    }

                }
            }

            return lDictUserDetails;
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