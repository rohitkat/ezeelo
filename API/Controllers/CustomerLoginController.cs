//-----------------------------------------------------------------------
// <copyright file="CustomerLoginController" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using API.Models;
using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace API.Controllers
{
    public class CustomerLoginController : ApiController
    {
        private EzeeloDBContext db = new EzeeloDBContext();//Sonali for MLM user _24-10-2018
        //public long UserLoginId = 0;//Added by Sonali for Authorization filter on 16-04-2019
        /*
         <summary>
         Check user Authentication
         </summary>
         <param name="uname">User Name</param>
         <param name="pass">Password</param>
         <returns></returns>
        [LoginSuccess]
        [ApiException] 
         GET api/ProductList
        [ResponseType(typeof(CustomerCredentials))]
        public IHttpActionResult GET(string uname,string pass )
        {
            LoginViewModel login = new LoginViewModel();
            login.UserName = uname;
            login.Password = pass;
            BusinessLogicLayer.ErrorLog.ErrorLogFile("enter " + login.UserName, ErrorLog.Module.API, System.Web.HttpContext.Current.Server);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            CustomerCredentials lCustCredentials = new CustomerCredentials();
            CustomerLogin lCustLogin = new CustomerLogin(System.Web.HttpContext.Current.Server);
            lCustCredentials = lCustLogin.Login(login);
            BusinessLogicLayer.ErrorLog.ErrorLogFile("login " + lCustCredentials.UserLoginID, ErrorLog.Module.API, System.Web.HttpContext.Current.Server);
            return CreatedAtRoute("DefaultApi", new { id = lCustCredentials.UserLoginID }, lCustCredentials);
        }
         */

        [LoginSuccess]
        [ApiException]
        [ValidateModel]
        public object Get(string UserName, string Password, string ReferralId = null)
        {
            LoginViewModel login = new LoginViewModel();
            object obj = new object();
            login.UserName = UserName;
            login.Password = Password;
            login.ReferralId = ReferralId;
            if (!ModelState.IsValid)
            {
                Request.Properties["UserLoginId"] = null;//Added by Sonali for authorization filter on 16-04-2019
                return obj = new { Success = 0, Message = "Not a valid Referral ID!", data = string.Empty };
            }
            CustomerCredentials lCustCredentials = new CustomerCredentials();
            CustomerLogin lCustLogin = new CustomerLogin(System.Web.HttpContext.Current.Server);
            lCustCredentials = lCustLogin.Login(login);

            if (lCustCredentials == null || lCustCredentials.UserLoginID == 0)// ||  lCustCredentials.UserName == null)
            {
                Request.Properties["UserLoginId"] = null;//Added by Sonali for authorization filter on 16-04-2019
                obj = new { Success = 0, Message = "Invalid Username/Password.", data = string.Empty };
            }
            // obj = new { HTTPStatusCode = "400", UserMessage = "Invalid Username/Password/ReferralId.", UserLoginID = 0, UserName = string.Empty };
            else
            {
                if (lCustCredentials.IsLocked)
                {
                    Request.Properties["UserLoginId"] = null;
                    return obj = new { Success = 0, Message = "It seems, you are new to eZeelo! please register.", data = string.Empty };
                }
                //Sonali for MLM user _24-10-2018
                try
                {
                    lCustCredentials.IsMLMUser = db.MLMUsers.Where(x => x.UserID == lCustCredentials.UserLoginID).Any();
                    if (!String.IsNullOrEmpty(ReferralId))
                    {
                        bool chkReferralId = db.MLMUsers.Any(p => p.Ref_Id == ReferralId);
                        if (chkReferralId != true)
                        {
                            Request.Properties["UserLoginId"] = null;//Added by Sonali for authorization filter on 16-04-2019
                            return obj = new { Success = 0, Message = "Invalid ReferralId.", data = string.Empty };
                        }
                        //using (TransactionScope ts1 = new TransactionScope())
                        //{
                        MLMWalletPoints MLMWallet = new MLMWalletPoints();
                        //long LoginUserId = id;
                        //UserLogin userLog = db.UserLogins.FirstOrDefault(p => p.ID == LoginUserId);
                        string result = MLMWallet.LeadersSingUp(lCustCredentials.UserLoginID, Password, lCustCredentials.EmailID, ReferralId);
                        if (result.Contains("R_DONE"))
                        {
                            try
                            {
                                string RefId = ReferralId;
                                UserName = " under " + db.PersonalDetails.FirstOrDefault(p => p.UserLoginID == (db.MLMUsers.FirstOrDefault(m => m.Ref_Id == RefId).UserID)).FirstName;
                                ReferralId = db.MLMUsers.Where(x => x.UserID == lCustCredentials.UserLoginID).Select(x => x.Ref_Id).FirstOrDefault();
                            }
                            catch
                            {

                            }
                            Request.Properties["UserLoginId"] = lCustCredentials.UserLoginID;//Added by Sonali for authorization filter on 16-04-2019
                            //Session["LeaderSignUpLink"] = "http://beta.mlm.ezeelo.in/LeadersLogin/Login/?UserName=" + model.EmailId + "&Password=" + model.Password;
                            //TempData["IsLeadersSignUp"] = "You Have Successfully Become Ezeelo Member" + UserName;
                            return obj = new { Success = 1, Message = "You Have Successfully Become Ezeelo Member" + UserName, data = new { UserLoginID = lCustCredentials.UserLoginID, UserName = UserName, EmailID = lCustCredentials.EmailID, MobileNo = lCustCredentials.MobileNo, ReferralId = ReferralId, FirstName = lCustCredentials.FirstName, LastName = lCustCredentials.LastName, IsMLMUser = lCustCredentials.IsMLMUser } };
                        }
                        else if (result.Contains("ALREADY_R"))
                        {
                            try
                            {
                                string RefId = db.MLMUsers.FirstOrDefault(p => p.UserID == lCustCredentials.UserLoginID).Refered_Id_ref;
                                UserName = " under " + db.PersonalDetails.FirstOrDefault(p => p.UserLoginID == (db.MLMUsers.FirstOrDefault(m => m.Ref_Id == RefId).UserID)).FirstName;
                                ReferralId = db.MLMUsers.Where(x => x.UserID == lCustCredentials.UserLoginID).Select(x => x.Ref_Id).FirstOrDefault();
                            }
                            catch
                            {

                            }
                            Request.Properties["UserLoginId"] = lCustCredentials.UserLoginID;//Added by Sonali for authorization filter on 16-04-2019
                            return obj = new { Success = 0, Message = "You Are Already Registered As Ezeelo Member" + UserName, data = new { UserLoginID = lCustCredentials.UserLoginID, UserName = UserName, EmailID = lCustCredentials.EmailID, MobileNo = lCustCredentials.MobileNo, ReferralId = ReferralId, FirstName = lCustCredentials.FirstName, LastName = lCustCredentials.LastName, IsMLMUser = lCustCredentials.IsMLMUser } };
                            //  new { HTTPStatusCode = "400", UserMessage = "You Are Already Registered As Ezeelo Member" + UserName, UserLoginID = userLogin.ID, UserName = UserName };
                        }
                        else
                        {
                            Request.Properties["UserLoginId"] = null;//Added by Sonali for authorization filter on 16-04-2019
                            // TempData["IsLeadersSignUp"] = "Facing Any Trouble , Happy To Help You, Simply Call 9172221910";
                            return obj = new { Success = 0, Message = "Facing Any Trouble , Happy To Help You, Simply Call 9172221910", data = string.Empty };
                            // new { HTTPStatusCode = "400", UserMessage = "Facing Any Trouble , Happy To Help You, Simply Call 9172221910", UserLoginID = 0, UserName = string.Empty };
                        }
                        //ts1.Complete();
                        //}

                    }
                    else
                        ReferralId = db.MLMUsers.Where(x => x.UserID == lCustCredentials.UserLoginID).Select(x => x.Ref_Id).FirstOrDefault();
                    Request.Properties["UserLoginId"] = lCustCredentials.UserLoginID;//Added by Sonali for authorization filter on 16-04-2019
                    obj = new { Success = 1, Message = "Login Successfull.", data = new { UserLoginID = lCustCredentials.UserLoginID, UserName = lCustCredentials.UserName, EmailID = lCustCredentials.EmailID, MobileNo = lCustCredentials.MobileNo, ReferralId = ReferralId, FirstName = lCustCredentials.FirstName, LastName = lCustCredentials.LastName, IsMLMUser = lCustCredentials.IsMLMUser } };
                }
                catch (Exception ex)
                {
                    Request.Properties["UserLoginId"] = null;//Added by Sonali for authorization filter on 16-04-2019
                    obj = new { Success = 0, Message = ex.Message, data = string.Empty };
                }
                //Sonali for MLM user _24-10-2018

            }

            //  obj = new { HTTPStatusCode = "200 ", UserMessage = "Login Successfull.",  };
            return obj;

        }


        /// <summary>
        /// Customer Login 
        /// </summary>
        /// <param name="login">Customer Login credentials</param>
        /// <returns></returns>
        [LoginSuccess]
        [ApiException]
        [ValidateModel]
        // POST api/ProductList
        public object post(LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            CustomerCredentials lCustCredentials = new CustomerCredentials();
            CustomerLogin lCustLogin = new CustomerLogin(System.Web.HttpContext.Current.Server);
            lCustCredentials = lCustLogin.Login(login);
            object obj = new object();
            if (lCustCredentials == null || lCustCredentials.UserLoginID == 0)// ||  lCustCredentials.UserName == null)
                obj = new { HTTPStatusCode = "400", UserMessage = "Invalid Username/Password.", UserLoginID = 0, UserName = string.Empty };
            else
                obj = new { HTTPStatusCode = "200 ", UserMessage = "Login Successfull.", UserLoginID = lCustCredentials.UserLoginID, UserName = lCustCredentials.UserName, EmailID = lCustCredentials.EmailID, MobileNo = lCustCredentials.MobileNo };
            return obj;
        }

    }
}
