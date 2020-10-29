using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Leaders.Controllers
{
    public class LeadersLoginController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

       
        [HttpGet]
        public ActionResult Index()
        {
            Session.Abandon();
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            //Response.Cache.SetNoStore(); 
            Session["ID"] = null;
            // return View("Login");
            return PartialView("_LeadersUserLogin");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string UserName, string Password)
        {
            LoginViewModel model = new LoginViewModel();
            model.Password = Password;
            model.UserName = UserName;
            MLMAdminLogin oLogin = new MLMAdminLogin();
            // MLMUser oUser = new MLMUser();
            Session["ID"] = null;

            bool IsEmailValid = false;

            IsEmailValid = this.IsValidEmailId(model.UserName.ToLower());

            if (IsEmailValid == false)
            {
                ModelState.AddModelError("ReferralId", "Wrong User Id!");
                ModelState.AddModelError("UserName", "Sorry You're not a registered Ezeelo Customer, First register Yourself at Ezeelo.com");
            }
            else
            {
                var objUser = db.UserLogins.Where(h => h.Email.Equals(model.UserName) && h.Password.Equals(model.Password)).FirstOrDefault();
                if (objUser != null)
                {
                    Session["ID"] = objUser.ID.ToString();
                    Session["UserEmail"] = objUser.Email.ToString();

                    string UserName_ = "";
                    string RefferalCode = "";
                    decimal EzzMoney = 0;
                    GetUserDetail(out  UserName_, out  RefferalCode, out  EzzMoney);
                    Session["UserName"] = UserName_;
                    Session["RefferalCode"] = RefferalCode;
                    Session["EzzMoney"] = EzzMoney;


                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    ModelState.AddModelError("Password", "Wrong Password!");
                }
            }
            return PartialView("_LeadersUserLogin");

        }
        public void GetUserDetail(out string UserName, out string RefferalCode, out decimal EzzMoney)
        {
            UserName = "";
            RefferalCode = "";
            long LoginUserId = 0;
            if (Session["ID"] != null)
            {
                LoginUserId = Convert.ToInt64(Session["ID"]);
            }
            UserLogin objU = db.UserLogins.FirstOrDefault(p => p.ID == LoginUserId);
            if (objU != null)
            {
                MLMUser objM = db.MLMUsers.FirstOrDefault(p => p.UserID == objU.ID);
                if (objM != null)
                {
                    RefferalCode = objM.Ref_Id;
                    PersonalDetail objP = db.PersonalDetails.FirstOrDefault(p => p.UserLoginID == objU.ID);
                    if (objP != null)
                    {
                        UserName = objP.FirstName + " " + objP.LastName;
                    }
                }
            }
            EzzMoney = GetEzzeMoney();
        }

        public decimal GetEzzeMoney()
        {
            long LoginUserId = 0;
            if (Session["ID"] != null)
            {
                LoginUserId = Convert.ToInt64(Session["ID"]);
            }
            MLMWallet obj = db.MLMWallets.FirstOrDefault(p => p.UserLoginID == LoginUserId);
            if (obj != null)
            {
                return obj.Amount;
            }
            else
            {
                return 0;
            }
        }
        public ActionResult Logout()
        {
            Session["ID"] = null;
            Session["UserEmail"] = null;
            return RedirectToAction("Index");
        }

        public ActionResult Login(string UserName, string Password)
        {
            Encryption _Decryption = new Encryption();//Added by Rumana on 13/04/2019
            var DecryptedEmail = _Decryption.DecodeFrom64(UserName);//Added by Rumana on 13/04/2019
            var DecryptedPassword = _Decryption.DecodeFrom64(Password);//Added by Rumana on 13/04/2019
            var objUser = db.UserLogins.Where(h => h.Email.Equals(DecryptedEmail) && h.Password.Equals(DecryptedPassword)).FirstOrDefault();//Added by Rumana on 13/04/2019
            //var objUser = db.UserLogins.Where(h => h.Email.Equals(UserName) && h.Password.Equals(Password)).FirstOrDefault();
            if (objUser != null)
            {
                Session["ID"] = objUser.ID.ToString();
                Session["UserEmail"] = objUser.Email.ToString();
                string UserName_ = "";
                string RefferalCode = "";
                decimal EzzMoney = 0;
                GetUserDetail(out  UserName_, out  RefferalCode, out  EzzMoney);
                Session["UserName"] = UserName_;
                Session["RefferalCode"] = RefferalCode;
                Session["EzzMoney"] = EzzMoney;

                return RedirectToAction("Index", "Dashboard");
            }
            return PartialView("_LeadersUserLogin");
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


        public ActionResult Hello()
        {
            return View();
        }

        [HttpPost]
        public JsonResult CheckUserLoginId(string Email)
        {
            try
            {
                string Name = "";
                UserLogin objU = db.UserLogins.FirstOrDefault(p => p.Email == Email);
                if (objU != null)
                {
                    MLMUser objM = db.MLMUsers.FirstOrDefault(p => p.UserID == objU.ID);
                    if (objM != null)
                    {
                        PersonalDetail objP = db.PersonalDetails.FirstOrDefault(p => p.UserLoginID == objU.ID);
                        if (objP != null)
                        {
                            Name = objP.FirstName + " " + objP.LastName;
                        }
                    }
                }
                return Json(Name, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

    }
}