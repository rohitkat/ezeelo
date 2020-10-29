//-----------------------------------------------------------------------
// <copyright file="CustomerLoginController" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using API.Models;
using BusinessLogicLayer;
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
    public class MerchantLoginController : ApiController
    {
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
            MerchantCredentials lMerchantCredentials = new MerchantCredentials();
            MerchantLogin lMerchantLogin = new MerchantLogin(System.Web.HttpContext.Current.Server);
            lMerchantCredentials = lMerchantLogin.Login(login);
            object obj = new object();
            if (lMerchantCredentials == null || lMerchantCredentials.UserLoginID == 0 || lMerchantCredentials.UserName == null)
                obj = new { HTTPStatusCode = "400", UserMessage = "Invalid Username/Password.", UserLoginID = 0, UserName= string.Empty };
            else
                obj = new { HTTPStatusCode = "200 ", UserMessage = "Login Successfull.", UserLoginID = lMerchantCredentials.UserLoginID, UserName = lMerchantCredentials.UserName, EmailID = lMerchantCredentials.EmailID, MobileNo = lMerchantCredentials.MobileNo };
            return obj;
        }
       
    }
}
