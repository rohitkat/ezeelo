using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BusinessLogicLayer;
using API.Models;
using System.Text.RegularExpressions;
using ModelLayer.Models; 

namespace API.Controllers
{
    public class SendOTPController : ApiController
    {
        EzeeloDBContext db = new EzeeloDBContext();

        // POST api/sendotp
        /// <summary>
        /// Generate OTP 
        /// </summary>
        /// <param name="mobileNo">mobile no</param>
        /// <returns></returns>
        [LoginSuccess]
        [ApiException]
        [ValidateModel]
        public object GetOTP(string mobileNo,params string[] ShopOrderCode)//Added params string[] ShopOrderCode
        {
            BusinessLogicLayer.CustomerLogin login = new BusinessLogicLayer.CustomerLogin(System.Web.HttpContext.Current.Server);
            int OrderStatus =  (int)ModelLayer.Models.Enum.ORDER_STATUS.DISPATCHED_FROM_GODOWN;//Added
           
            if (!login.IsValidMobile(mobileNo))
                return new { HTTPStatusCode = "400", UserMessage = "Invalid Mobile no.", OTP = "", SessionCode =""};

            //Added by Tejaswee(7-5-2016)
            //Check for: user with provided mobile is exists.
            UserLogin ul = db.UserLogins.Where(x => x.Mobile == mobileNo.Trim()).FirstOrDefault();
            if (ul == null)
                return new { HTTPStatusCode = "409", UserMessage = "Customer with given Mobile No is not available." };

            Dictionary<string, string> dictOTP = BusinessLogicLayer.OTP.GenerateOTP("USC");
            BusinessLogicLayer.OTP otp = new BusinessLogicLayer.OTP();
            otp.InsertOTPDetails(dictOTP["USC"], dictOTP["OTP"], ShopOrderCode);//added ShopOrderCode
            //Session["OTPCode"] = dictOTP["USC"];
            
            // Send OTP to customer
            otp.SendOTPToCustomer(mobileNo, dictOTP["OTP"]);
            return new { HTTPStatusCode = "200", UserMessage = "OTP generated Successfully. Valid for 10 min only.", OTP = dictOTP["OTP"], SessionCode = dictOTP["USC"] };
        }

      
    }
}
