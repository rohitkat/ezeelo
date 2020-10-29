using API.Models;
using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace API.Controllers
{
    public class LoginWithFacebookController : ApiController
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        [LoginSuccess]
        [ApiException]
        //[ValidateModel]
        // POST api/loginwithfacebook
        [HttpPost]
        public object Post(SocialMediaParameters lSocialMediaParameters)
        {
            var externalLogin = db.UserLogins.Where(x => x.Email == lSocialMediaParameters.email && x.IsLocked == false).FirstOrDefault();
            object obj = new object();
            if (externalLogin != null)
            {
                obj = new { HTTPStatusCode = "200 ", UserMessage = "Login Successfull.", UserLoginID = externalLogin.ID, UserName = externalLogin.Email, EmailID = externalLogin.Email, MobileNo = externalLogin.Mobile };
            }
            else
            {
                //New customer
                RegisterNewUser regUser = new RegisterNewUser();
                CustomerRegistrationViewModel model = new CustomerRegistrationViewModel();
                Random r = new Random();
                model.EmailId = lSocialMediaParameters.email;
                model.Password = regUser.CreatePassword(10);
                model.FirstName = (lSocialMediaParameters.name == string.Empty ? lSocialMediaParameters.email : lSocialMediaParameters.name);
                regUser.CreateNew_Account(model);
                Int64 UID = db.UserLogins.Where(x => x.Email == lSocialMediaParameters.email).FirstOrDefault().ID;
                obj = new { HTTPStatusCode = "200 ", UserMessage = "Login Successfull.", UserLoginID = UID, UserName = lSocialMediaParameters.email, EmailID = lSocialMediaParameters.email, MobileNo = "" };
            }

            return obj;
        }

        
    }

    public class SocialMediaParameters
    {
        public string email { get; set; }
        public string name { get; set; }
    }
}
