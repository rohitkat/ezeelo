using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Optimization;

namespace Leaders.Areas.Admin.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class AccountController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        [HttpGet]
        public ActionResult Login()
        {
            Session.Abandon();
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            //Response.Cache.SetNoStore(); 
            Session["ID"] = null;
           // return View("Login");
           return PartialView("_LeadersLogin");
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model, string callFrom)
        {
            Session["ID"] = null;
            if (model.UserName == null)
            {
                TempData["Message"] = "Login ID is Required";
                return View("_Login", model);
            }
            if (model.Password == null)
            {
                TempData["Message"] = "Password is Required";
                return View("_Login", model);
            }
            bool IsEmailValid = false, IsMobileValid = false;

            IsEmailValid = this.IsValidEmailId(model.UserName);

            if (IsEmailValid == false)
                IsMobileValid = this.IsValidMobile(model.UserName);

            if (IsEmailValid == false && IsMobileValid == false)
            {
                //ViewBag.Message = "Invalid UserName/Password!!";
                TempData["Message"] = "Invalid UserName/Password!!";

                return View("_Login", model);
            }
            else
            {
                Dictionary<string, string> lDictLoginDetails = this.CheckLogin(model.UserName, model.Password);

                if (lDictLoginDetails.Count() <= 0)
                {
                    //ViewBag.Message = "Invalid UserName/Password!!";
                    TempData["Message"] = "Invalid UserName/Password!!";
                    return View("_Login", model);
                }
                else
                {
                    var str = HttpContext.Request.RawUrl;
                    var controller = RouteData.Values["controller"].ToString();
                    var action = RouteData.Values["action"].ToString();

                    Session["ID"] = lDictLoginDetails["ID"];
                    Session["UserName"] = lDictLoginDetails["UserName"];
                    Session["RoleName"] = lDictLoginDetails["RoleName"];
                    FormsAuthentication.SetAuthCookie(model.UserName, true);
                    string Rurl = Request.QueryString["ReturnURL"];
                    return RedirectToAction("Index", "Home");
                }
            }
        }
        private bool IsValidEmailId(string pInputEmail)
        {
            //Regex To validate Email Address
            Regex regex = new Regex(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");
            //Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
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
                strMessage = "Your Both Password and Email are  invalid";
            }
            else
            {

                UserLogin lUserlogin = new UserLogin();
                lUserlogin = VerifyForgetPasswordRequest(EmailId, MobileNo);

                if (lUserlogin != null)
                {
                    SendPassword(lUserlogin.Email, lUserlogin.ID);
                    strMessage = "Please check you email";
                }
            }

            return Json(strMessage, JsonRequestBehavior.AllowGet);
        }

        private UserLogin VerifyForgetPasswordRequest(string EmailId, string MobileNo)
        {
            UserLogin lUserlogin = new UserLogin();
            lUserlogin = db.UserLogins.Where(x => (x.Email == EmailId || x.Mobile == MobileNo) && x.IsLocked == false).FirstOrDefault();
            return lUserlogin;
        }

        private void SendPassword(string pEmailId, long userLoginID)
        {
            try
            {
                EzeeloDBContext db = new EzeeloDBContext();

                var personalDetail = db.PersonalDetails.Where(x => x.UserLoginID == userLoginID);
                var loginDetail = db.UserLogins.Where(x => x.ID == userLoginID).FirstOrDefault();
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary.Add("<!--PERSONAL_NAME-->", personalDetail.FirstOrDefault().FirstName);
                dictionary.Add("<!--USER_NAME-->", loginDetail.Mobile);
                dictionary.Add("<!--USER_PASSWORD-->", loginDetail.Password);

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
    }
}
