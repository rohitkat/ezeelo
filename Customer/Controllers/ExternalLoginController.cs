using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Gandhibagh.Controllers
{
    public class ExternalLoginController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        //
        // GET: /ExternalLogin/
        public ActionResult Index(string email, string callFrom, string externalCallBackURL, string userName,string serviceType)
        {
            ViewBag.email = email;
            ViewBag.callFrom = callFrom;
            ViewBag.externalCallBackURL = externalCallBackURL;
            ViewBag.userName = userName;

            //ViewBag.returnurl = Request.UrlReferrer.ToString();
            return View();
        }
        public ActionResult Index(string email)
        {
            ViewBag.email = email;
           

            //ViewBag.returnurl = Request.UrlReferrer.ToString();
            return View();
        }
        [HttpPost]
        public ActionResult Index(string email, string callFrom, string externalCallBackURL, string userName)
        {
            ExternalLogin1 externalLogin_detail = new ExternalLogin1();
            if (Session["UID"] == null)
            {

                var externalLogin = db.UserLogins.Where(x => x.Email == email).FirstOrDefault();

                if (externalLogin != null)
                {
                    LoginViewModel model = new LoginViewModel();
                    model.UserName = email;
                    model.Password = externalLogin.Password;
                    return RedirectToAction("Login", "Login", new { model, callFrom, isExpressBuy = false, externalCallBackURL });
                    //call.Login(model, callFrom, false, externalCallBackURL);
                    //externalLogin_detail.email = externalLogin.Email;
                    //externalLogin_detail.password = externalLogin_detail.password;
                }
                else
                {
                    RegisterNewUser regUser = new RegisterNewUser();
                    CustomerRegistrationViewModel model = new CustomerRegistrationViewModel();
                    model.EmailId = email;
                    model.Password = CreatePassword(10);
                    model.FirstName = userName;
                    regUser.CreateNew_Account(model);
                    //externalLogin_detail.email = email;
                    //externalLogin_detail.password = model.Password;
                    return RedirectToAction("Login", "Login", new { model, callFrom, isExpressBuy = false, externalCallBackURL });
                }
            }

            return View();
        }


       

        public string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
    }

    public class ExternalLogin1
    {
        public string email { get; set; }
        public string password { get; set; }
    }


}