using API.Models;
using BusinessLogicLayer;
using ModelLayer.Models.ViewModel;
using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace API.Controllers
{
    public class UserLoginController : ApiController
    {
        // GET api/userlogin
       /* public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/userlogin/5
        public string Get(int id)
        {
            return "value";
        }*/

        //----------------------------------  Hide EPOD from Ashish for Live -------------------------------------------------
        /*
        /// <summary>
        /// User Login 
        /// </summary>
        /// <returns></returns>
        [LoginSuccess]
        [ApiException]
        [ValidateModel]
        // POST api/ProductList
        // POST api/userlogin
        public object Post(LoginViewModel login)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var re = Request;
            var Headers = re.Headers;
            string ReqBy = "";
            string IMEI = "";
            if (Headers.Contains("ReqBy") && Headers.Contains("IMEI"))
            {
                ReqBy = Headers.GetValues("ReqBy").First();
                IMEI = Headers.GetValues("IMEI").First();
            }

            CustomerCredentials lCustCredentials = new CustomerCredentials();
            CustomerLogin lCustLogin = new CustomerLogin(System.Web.HttpContext.Current.Server);
            lCustCredentials = lCustLogin.UserLogin(login, ReqBy, IMEI);
            object obj = new object();
            if (lCustCredentials.LoginStatus != null)
                obj = new { HTTPStatusCode = "208", UserMessage = "Already Loigged In.", UserLoginID = lCustCredentials.UserLoginID, UserName = lCustCredentials.UserName, EmailID = lCustCredentials.EmailID, MobileNo = lCustCredentials.MobileNo, UserType = lCustCredentials.LoginType };
            else if (lCustCredentials == null || lCustCredentials.UserLoginID == 0 || lCustCredentials.UserName == null)
                obj = new { HTTPStatusCode = "400", UserMessage = "Invalid Username/Password.", UserLoginID = 0, UserName = string.Empty };
            else
                obj = new { HTTPStatusCode = "200", UserMessage = "Login Successfull.", UserLoginID = lCustCredentials.UserLoginID, UserName = lCustCredentials.UserName, EmailID = lCustCredentials.EmailID, MobileNo = lCustCredentials.MobileNo, UserType = lCustCredentials.LoginType };
                   
            return obj;
        }
        /// <summary>
        /// User Logout 
        /// </summary>
        /// <returns></returns>
        [LoginSuccess]
        [ApiException]
        [ValidateModel]
        // PUT api/userlogin/5
        //public object Put(string userlogout)
        public object Put(Logout userlogout)
        {
            var re = Request;
            var Headers = re.Headers;
            string ReqBy = "";
            string IMEI = "";
            if (Headers.Contains("ReqBy") && Headers.Contains("IMEI"))
            {
               ReqBy = Headers.GetValues("ReqBy").First();
              IMEI = Headers.GetValues("IMEI").First();
            }
            CustomerCredentials lCustCredentials = new CustomerCredentials();
            CustomerLogin lCustLogin = new CustomerLogin(System.Web.HttpContext.Current.Server);
            //bool Islogout = lCustLogin.UserLogout(userlogout,ReqBy,IMEI);
            lCustCredentials = lCustLogin.UserLogout(userlogout, ReqBy, IMEI);
            object obj = new object();
            if (lCustCredentials.LoginStatus == "Logout Successfull.")
                obj = new { HTTPStatusCode = "200", UserMessage = "Logout Successfull." };
            else
                if (lCustCredentials.LoginStatus == "Already Loigged Out.")
                    obj = new { HTTPStatusCode = "208", UserMessage = "Already Loigged Out." };
                else
                    obj = new { HTTPStatusCode = "400", UserMessage = "Logout Failed." }; 
            return obj;
        }*/       

        //-----------------------------------------------------------------------------

        // DELETE api/userlogin/5
        /* public void Delete(int id)
        {
        }*/
    }
}
