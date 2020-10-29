using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using System.Text.RegularExpressions;
using System.Web.Security;

namespace DeliveryPartner.Controllers
{
    public class LoginController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        [HttpGet]
        public ActionResult Login()
        {
            return PartialView("_Login");
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model, string callFrom)
        {

            if (model.UserName == null)
            {
                TempData["Message"] = "Login ID is Required";
                return View("_Login", model);
            }
            if (model.UserName == null)
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
                    Session["USER_LOGIN_ID"] = lDictLoginDetails["ID"];

                    FormsAuthentication.SetAuthCookie(model.UserName, true);
                    string Rurl = Request.QueryString["ReturnURL"];
                    return RedirectToAction("Index", "Dashboard");
                }
            }
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

        public ActionResult Logout()
        {
            if (Session["ID"] != null)
            {
                Session["ID"] = null;
            }
            if (Session["Username"] != null)
            {
                Session["Username"] = null;
            }
            return RedirectToAction("Login", "Login");
        }
	}
}