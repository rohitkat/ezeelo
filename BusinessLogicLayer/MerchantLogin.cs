//-----------------------------------------------------------------------
// <copyright file="MerchantLogin.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Mohit Sinha</author>
//-----------------------------------------------------------------------
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class MerchantLogin : MerchantManagment
    {
        public MerchantLogin(System.Web.HttpServerUtility server)
            : base(server)
        {

        }
        /// <summary>
        /// Validate user entered credentials and authenticate login request
        /// </summary>
        /// <param name="login">LoginViewModel</param>
        /// <returns>User Login ID and UserName</returns>
        public MerchantCredentials Login(LoginViewModel login)
        {
            MerchantCredentials lUserDetails = new MerchantCredentials();

            if (IsValidEmailId(login.UserName) || IsValidMobile(login.UserName))
            {
                lUserDetails = this.CheckLogin(login.UserName, login.Password);

            }
            return lUserDetails;
        }
        /// <summary>
        /// Email Address validation
        /// </summary>
        /// <param name="pInputEmail">email address</param>
        /// <returns>Validation status</returns>
        public bool IsValidEmailId(string pInputEmail)
        {
            //Regex To validate Email Address
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(pInputEmail);
            if (match.Success)
                return true;
            else
                return false;
        }
        /// <summary>
        /// Mobile No. validation
        /// </summary>
        /// <param name="pInputEmail">Mobile no</param>
        /// <returns>Validation status</returns>
        public bool IsValidMobile(string pInputMobile)
        {
            //Regex To validate Email Address
            Regex regex = new Regex(@"^[7-9]{1}[0-9]{9}$");
            Match match = regex.Match(pInputMobile);
            if (match.Success)
                return true;
            else
                return false;
        }
        /// <summary>
        /// Get Customer's Login Id and User Name Depending on his\her entered User name and Password
        /// </summary>
        /// <param name="pUserName">UserName</param>
        /// <param name="pPassword">Password</param>
        /// <returns></returns>
        private MerchantCredentials CheckLogin(string pUserName, string pPassword)
        {
            MerchantCredentials lUserDetails = new MerchantCredentials();

            var userExist = db.UserLogins.Select(x => new { x.Email, x.Mobile, x.Password, x.ID })
                                         .Where(x => (x.Email == pUserName || x.Mobile == pUserName) && x.Password == pPassword).ToList();
            
            
            if (userExist.Count() > 0)
            {
                foreach (var item in userExist)
                {
                    var i =db.BusinessDetails.Where(p => p.BusinessTypeID == 1 && p.UserLoginID == item.ID).Count();
                    if (i > 0)
                    {
                    lUserDetails.UserLoginID = item.ID;
                    lUserDetails.UserName = item.Email.ToString();
                    lUserDetails.MobileNo = item.Mobile;
                    lUserDetails.EmailID = item.Email.ToString();
                    }
                }
            }
            return lUserDetails;
        }
    }
    /// <summary>
    /// This Class is to to send the response after successfull login.
    /// </summary>
    public class MerchantCredentials
    {
        public long UserLoginID { get; set; }
        public string UserName { get; set; }
        public string EmailID { get; set; }
        public string MobileNo { get; set; }

    }
}
