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
using System.Web.Configuration;
using System.Web.Services.Description;

namespace Inventory.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/
        private EzeeloDBContext db = new EzeeloDBContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LogOff()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Login");
        }

        public ActionResult Login(WarehouseRegisterViewModel warehouseRegisterViewModel, LoginViewModel model)
        {
            WarehouseRegisterViewModel OBJ = new WarehouseRegisterViewModel();
            string value = System.Configuration.ConfigurationManager.AppSettings["EZEELO_Category_ID"];
            Response.Write(value);
            string value1 = System.Configuration.ConfigurationManager.AppSettings["EZEELO_Category_pwd"];
            Response.Write(value);
            string UserName = warehouseRegisterViewModel.loginViewModel.UserName.ToString();
            string Password = warehouseRegisterViewModel.loginViewModel.Password.ToString();

            if (UserName == value && Password == value1)
            {
                long result = 0;

                //Session["ID"] = UserName.ToString();
                Session["pwd"] = Password.ToString();
                Session["USER_NAME"] = UserName.ToString();
                Session["BusinessTypeID"] = 1;
                Session["WarehouseID"] = result;
                Session["FRANCHISE_ID"] = result;
                Session["ShopID"] = result;
                string WarehouseName = db.Warehouses.Where(x => x.ID == result && x.IsActive == true).Select(x => x.Name).FirstOrDefault();
                Session["WarehouseName"] = WarehouseName;
                //return RedirectToAction("Home", "CategoryProductListReport");

                return RedirectToAction("Index", "CategoryHome");
            }
            else
            {

                try
                {
                    bool IsSupplier = false; //yashaswi 21/3/2018
                    ViewBag.Salutation = new SelectList(db.Salutations.ToList(), "ID", "Name", 1);
                    ViewBag.SecurityQuestion = new SelectList(db.SecurityQuestions, "ID", "Question");

                    if (warehouseRegisterViewModel.loginViewModel.UserName == null)
                    {
                        ViewBag.Message = "Plz fill username!";
                        TempData["Message"] = "Plz fill username!";
                        return RedirectToAction("Index", "Login");
                    }

                    if (warehouseRegisterViewModel.loginViewModel.Password == null)
                    {
                        ViewBag.Message = "Plz fill Password!";
                        TempData["Message"] = "Plz fill Password!";
                        return RedirectToAction("Index", "Login");
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
                        return RedirectToAction("Index", "Login");
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
                                return RedirectToAction("Index", "Login");
                            }
                            else
                            {

                                //Added by Priti on Partner direct login  from Inventory on button click on 5-12-2018
                                try
                                {


                                    long LoginUserId = Convert.ToInt64(lDictLoginDetails["ID"]);
                                    UserLogin userLog = db.UserLogins.FirstOrDefault(p => p.ID == LoginUserId);

                                    long businessDetailIDPartner = db.BusinessDetails.Where(x => x.UserLoginID == LoginUserId).Select(x => x.ID).FirstOrDefault();
                                    long WarehouseID = db.Warehouses.Where(x => x.BusinessDetailID == businessDetailIDPartner && x.IsActive == true).Select(x => x.ID).FirstOrDefault();
                                    long franchiseId = db.WarehouseFranchises.Where(x => x.WarehouseID == WarehouseID).Select(y => y.FranchiseID).FirstOrDefault();
                                    long businessDetailID1 = db.Franchises.Where(x => x.ID == franchiseId).Select(z => z.BusinessDetailID).FirstOrDefault();

                                    long ShopID = db.Shops.Where(x => x.FranchiseID == franchiseId).Select(s => s.ID).FirstOrDefault();
                                    long UserloginIDPartner = db.BusinessDetails.Where(x => x.ID == businessDetailID1).Select(l => l.UserLoginID).FirstOrDefault();
                                    string PasswordPartner = db.UserLogins.Where(m => m.ID == UserloginIDPartner).Select(m => m.Password).FirstOrDefault();

                                    string EmailPartner = db.UserLogins.Where(r => r.ID == UserloginIDPartner).Select(r => r.Email).FirstOrDefault();
                                    var IsExistPartner = db.Franchises.Where(x => x.BusinessDetailID == businessDetailIDPartner && x.IsActive == true).FirstOrDefault();

                                    if (UserloginIDPartner != null)
                                    {

                                        //Session["Partnerlink"] = "" + (new URLsFromConfig()).GetURL("PARTNER") + "PartnerLogin/Login/?UserName=" + userLog.Email + "&Password=" + userLog.Password;

                                        Session["FRANCHISE_ID"] = franchiseId;

                                        Session["ShopID"] = ShopID;





                                        Session["EmailPartner"] = EmailPartner;
                                        Session["PasswordPartner"] = PasswordPartner;
                                        Session["Partnerlink"] = "" + (new URLsFromConfig()).GetURL("PARTNER") + "FranchiseRegister/LoginURL?pUserName=" + EmailPartner + "&pPassword=" + PasswordPartner;
                                        TempData["PartnerSingUp"] = "You Are Already Registered As Ezeelo Member" + UserName;
                                    }
                                    else
                                    {
                                        TempData["PartnerSingUp"] = "Facing Any Trouble , Happy To Help You, Simply Call 9172221910";
                                    }
                                }
                                catch
                                {

                                }

                                //end by priti on 5-12-2018

                                long UserLoginID = Convert.ToInt64(lDictLoginDetails["ID"].ToString());
                                long businessDetailID = db.BusinessDetails.Where(x => x.UserLoginID == UserLoginID && x.BusinessType.Prefix == "EZWH").Select(x => x.ID).FirstOrDefault();
                                long businessTypeID = db.BusinessDetails.Where(x => x.ID == businessDetailID).Select(x => x.BusinessTypeID).FirstOrDefault();
                                Session["BusinessTypeID"] = businessTypeID;
                                var IsExist = db.Warehouses.Where(x => x.BusinessDetailID == businessDetailID && x.IsActive == true).ToList();

                                if (IsExist.Count > 0)
                                {
                                    Session["ID"] = lDictLoginDetails["ID"];
                                    Session["USER_LOGIN_ID"] = lDictLoginDetails["ID"];

                                    //=============== call method to get no. of days remaining for plan expiration =================

                                    ModelLayer.Models.PersonalDetail pd = new ModelLayer.Models.PersonalDetail();
                                    long LoginID = Convert.ToInt64(lDictLoginDetails["ID"].ToString());
                                    pd = db.PersonalDetails.Where(x => x.UserLoginID == LoginID).FirstOrDefault();
                                    Session["PERSONAL_ID"] = pd == null ? 0 : pd.ID;
                                    Session["USER_NAME"] = lDictLoginDetails["UserName"];
                                    FormsAuthentication.SetAuthCookie(warehouseRegisterViewModel.loginViewModel.UserName, true);
                                    //==============================
                                    Warehouse ob = db.Warehouses.FirstOrDefault(x => x.BusinessDetailID == businessDetailID && x.IsActive == true);

                                    string WarehouseName = ob.Name;
                                    Session["WarehouseName"] = WarehouseName;

                                    long WarehouseID = ob.ID;
                                    Session["WarehouseID"] = WarehouseID;



                                    ///Added by Priti on 16-11-2018

                                    string GSTNo = ob.GSTNumber;
                                    //Session["GSTNO"] = ((GSTNo == null )? "": GSTNo);
                                    Session["GSTNO"] = GSTNo;


                                    string WarehouseCode = ob.WarehouseCode;

                                    //Session["WarehouseCode"] =(( WarehouseCode==null)?"": WarehouseCode);
                                    Session["WarehouseCode"] = WarehouseCode;
                                    string WarehouseAddress = db.BusinessDetails.Where(x => x.ID == businessDetailID && x.IsActive == true).Select(z => z.Address).FirstOrDefault();

                                    //Session["WarehouseAddress"] = ((WarehouseAddress == null) ? "" : WarehouseAddress);
                                    Session["WarehouseAddress"] = WarehouseAddress;


                                    //End by Priti on 16-11-2018
                                    //===============================                              
                                    //start yashaswi 21/3/2018
                                    if (db.Suppliers.SingleOrDefault(p => p.WarehouseID == WarehouseID) != null)
                                    {
                                        IsSupplier = true;
                                        Session["IsSupplier"] = IsSupplier;
                                    }
                                    //end
                                    //start yashaswi 26/4/2018
                                    Session["Entity"] = ob.Entity;
                                    if (WarehouseID == Convert.ToInt64(WebConfigurationManager.AppSettings["EZEELO_WAREHOUSE_ID"]))
                                    {
                                        Session["WarehouseLevel"] = 1; //Yashaswi 21/5/2018
                                        Session["IsEzeeloLogin"] = "1";
                                    }
                                    else
                                    {//Yashaswi 21/5/2018

                                        Session["IsEzeeloLogin"] = "0";
                                        if (ob.IsFulfillmentCenter == true)
                                        {
                                            Session["WarehouseLevel"] = 3;
                                        }
                                        else
                                        {
                                            Session["WarehouseLevel"] = 2;
                                        }
                                    }

                                    return RedirectToAction("Index", "Home");
                                }
                                else
                                {
                                    ViewBag.Message = "You are not a Warehouse Keeper";
                                    TempData["Message"] = "You are not a Warehouse Keeper";

                                    //return View("Create");
                                    return RedirectToAction("Index", "Login");
                                }
                            }

                        }
                        else
                        {
                            var str = HttpContext.Request.RawUrl;
                            var controller = RouteData.Values["controller"].ToString();
                            var action = RouteData.Values["action"].ToString();
                            //Yashaswi 13/3/2018
                            try
                            {
                                long UserLoginID = Convert.ToInt64(lDictAdminLoginDetails["ID"].ToString());
                                long businessDetailID = db.BusinessDetails.Where(x => x.UserLoginID == UserLoginID && x.BusinessType.Prefix == "GBSA").Select(x => x.ID).FirstOrDefault();
                                long businessTypeID = db.BusinessDetails.Where(x => x.ID == businessDetailID).Select(x => x.BusinessTypeID).FirstOrDefault();
                                Session["BusinessTypeID"] = businessTypeID;
                            }
                            catch
                            {

                            }
                            Session["WarehouseLevel"] = 1; //Yashaswi 21/5/2018
                            Session["IsSupplier"] = IsSupplier; //yashaswi 21/3/2018
                            Session["USER_LOGIN_ID"] = lDictAdminLoginDetails["ID"];
                            @Session["USER_NAME"] = lDictAdminLoginDetails["UserName"];
                            Session["RoleName"] = lDictAdminLoginDetails["RoleName"];
                            FormsAuthentication.SetAuthCookie(warehouseRegisterViewModel.loginViewModel.UserName, true);
                            string Rurl = Request.QueryString["ReturnURL"];
                            Session["IsEzeeloLogin"] = "0"; //yashaswi 6/4/2018
                            Session["Entity"] = "";
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    //return View("Create");
                    //return RedirectToAction("Index", "WarehouseRegister");
                }
                catch (Exception ex)
                {

                }
                return RedirectToAction("Index", "Login");
            }
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

        public PartialViewResult ForgetPassword()
        {
            return PartialView("ForgetPassword");
        }

        [HttpPost]
        public PartialViewResult ForgetPassword(string EmailId, string MobileNo)
        {
            ViewBag.Message = "Please Check you EmailAddress For Password";
            return PartialView("ForgetPassword");
        }

        [HttpPost]
        public JsonResult ForgetPasswordGetRequest(string EmailId, string MobileNo)
        {
            string strMessage = string.Empty;

            bool IsEmailValid = false, IsMobileValid = false;
            if (EmailId.ToString().Length > 0)
            {
                IsEmailValid = this.IsValidEmailId(EmailId);
            }
            if (MobileNo.ToString().Length > 0)
            {
                IsMobileValid = this.IsValidMobile(MobileNo);
            }

            if (IsEmailValid == false && IsMobileValid == false)
            {
                strMessage = "Your Both Mobile No. and Email are  invalid";
            }
            else
            {
                UserLogin lUserlogin = new UserLogin();
                lUserlogin = VerifyForgetPasswordRequest(EmailId, MobileNo);

                if (lUserlogin != null)
                {
                    if (!string.IsNullOrEmpty(EmailId))
                    {
                        this.SendPassword(lUserlogin.Email, lUserlogin.ID);
                        strMessage = "Password is sent to your mail.";
                    }
                    else if (!string.IsNullOrEmpty(MobileNo))
                    {
                        string FirstName = db.PersonalDetails.Where(x => x.UserLoginID == lUserlogin.ID).Select(x => x.FirstName).FirstOrDefault();
                        this.SendPasswordToCustomer(MobileNo, lUserlogin.Password, FirstName);
                        strMessage = "Password is sent to your mobile through SMS.";
                    }
                }
                else
                {
                    strMessage = "User with given Email Id or Mobile No is not available.";
                }
            }

            return Json(strMessage, JsonRequestBehavior.AllowGet);
        }

        private UserLogin VerifyForgetPasswordRequest(string EmailId, string MobileNo)
        {
            UserLogin lUserlogin = new UserLogin();
            if (!string.IsNullOrEmpty(EmailId))
            {
                lUserlogin = db.UserLogins.Where(x => x.Email == EmailId && x.IsLocked == false).FirstOrDefault();
            }
            else if (!string.IsNullOrEmpty(MobileNo))
            {
                lUserlogin = db.UserLogins.Where(x => x.Mobile == MobileNo && x.IsLocked == false).FirstOrDefault();
            }
            return lUserlogin;
        }

        private void SendPassword(string pEmailId, long userLoginID)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var personalDetail = db.PersonalDetails.Where(x => x.UserLoginID == userLoginID).FirstOrDefault();
                var loginDetail = db.UserLogins.Where(x => x.ID == userLoginID).FirstOrDefault();
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary.Add("<!--PERSONAL_NAME-->", (personalDetail != null ? personalDetail.FirstName : string.Empty));
                dictionary.Add("<!--USER_NAME-->", loginDetail.Mobile);
                dictionary.Add("<!--PWD-->", loginDetail.Password);
                gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.FORGET_PASS, new string[] { pEmailId }, dictionary, true);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ForgotPasswordController][M:SendPassword]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ForgotPasswordController][M:SendPassword]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
        }
        private void SendPasswordToCustomer(string guestMobileNumber, string password, string Name)
        {
            /*
              Indents:
            * Description: This method is used to send OTP to customer

            * Parameters: 

            * Precondition: 
            * Postcondition:
            * Logic: 
            */

            try
            {
                Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();
                dictSMSValues.Add("#--NAME--#", Name);
                dictSMSValues.Add("#--PASSWORD--#", password);

                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);

                gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.CUST_SEND_PWD, new string[] { guestMobileNumber }, dictSMSValues);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PaymentProcessController][M:SendOTPToCustomer]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PaymentProcessController][M:SendOTPToCustomer]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
        }


    }
}