using API.Models;
using BusinessLogicLayer;
using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;

namespace API.Controllers
{
    public class ForgetPasswordController : ApiController
    {
        EzeeloDBContext db = new EzeeloDBContext();
        //[LoginSuccess]
        [ApiException]
        public object Get(string EmailId, string MobileNo)
        {
            object obj = new object();
            try
            {
                if (string.IsNullOrEmpty(EmailId) && string.IsNullOrEmpty(MobileNo))
                {
                    return obj = new { Success = 0, Message = "Enter valid EmailId or MobileNo.", data = string.Empty };
                }
                if (!string.IsNullOrEmpty(MobileNo))
                {
                    if (!Regex.IsMatch(MobileNo, @"^([5-9]{1}[0-9]{9})$"))//Sonali_24/10/2018
                        return obj = new { Success = 0, Message = "Enter valid MobileNo.", data = string.Empty };
                    UserLogin ul = db.UserLogins.Where(x => x.Mobile == MobileNo).FirstOrDefault();
                    if (ul != null)
                    {
                        string FirstName = db.PersonalDetails.Where(x => x.UserLoginID == ul.ID).Select(x => x.FirstName).FirstOrDefault();
                        this.SendPasswordToCustomer(MobileNo, ul.Password, FirstName);
                        obj = new { Success = 1, Message = "Password is sent to your mobile through SMS.", data = new { Password = ul.Password, CustomerLoginID = ul.ID, UserName = ul.Email, EmailID = ul.Email, MobileNo = ul.Mobile } };
                    }
                    else
                    {
                        obj = new { Success = 1, Message = "Customer with given Mobile No is not available.", data = string.Empty };
                    }
                }
                else
                {
                    if (!Regex.IsMatch(EmailId, @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                        @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                        @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"))//Sonali_24/10/2018
                        return obj = new { Success = 0, Message = "Enter valid EmailId.", data = string.Empty };

                    UserLogin ul = db.UserLogins.Where(x => x.Email == EmailId).FirstOrDefault();
                    if (ul != null)
                    {
                        this.SendPasswordToCustomerOnEmail(EmailId, ul.Password);
                        obj = new { Success = 1, Message = "Password is sent to your mail.", data = new { Password = ul.Password, CustomerLoginID = ul.ID, UserName = ul.Email, EmailID = ul.Email, MobileNo = ul.Mobile } };
                    }
                    else
                    {
                        obj = new { Success = 0, Message = "Customer with given Email Id is not available.", data = string.Empty };
                    }
                }
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }


        // POST api/changepassword
        /// <summary>
        /// Receive Password.
        /// </summary>
        /// <param name="changePassword">Credentials required to get forgot password.</param>
        /// <returns>Password for authenticated user</returns>
        //[LoginSuccess]
        [ValidateModel]
        [ApiException]
        [ResponseType(typeof(object))]
        // POST api/forgetpassword
        public object Post(ForgetPassword fPassword)
        {
            if (fPassword == null)
                return new { HTTPStatusCode = "400", UserMessage = "The value provided for parameter is not in the correct format." };


            //Check for: otp is valid.
            //OTP otp = db.OTPs.Where(x => x.SessionCode == fPassword.SessionCode && x.OTP1 == fPassword.OTP && x.IsActive == true).FirstOrDefault();

            ModelLayer.Models.OTP otp = db.OTPs.Where(x => x.SessionCode == fPassword.SessionCode && x.OTP1 == fPassword.OTP && x.IsActive == true).OrderByDescending(y => y.ID).FirstOrDefault();
            if (otp == null)
                return new { HTTPStatusCode = "400", UserMessage = "Invalid OTP/SessionCode." };

            //Check for: user with provided mobile is exists.
            UserLogin ul = db.UserLogins.Where(x => x.Mobile == fPassword.Mobile).FirstOrDefault();
            if (ul == null)
                return new { HTTPStatusCode = "409", UserMessage = "Customer with given Mobile No is not available." };

            return new { HTTPStatusCode = "200", Password = ul.Password, CustomerLoginID = ul.ID, UserName = ul.Email, EmailID = ul.Email, MobileNo = ul.Mobile };

        }
        private void SendPasswordToCustomerOnEmail(string email, string PWD)
        {
            /*
              Indents:
            * Description: This method is used to send Password to customer on mail
             
            * Parameters: 
             
            * Precondition: 
            * Postcondition:
            * Logic: 
            */

            try
            {
                EzeeloDBContext db = new EzeeloDBContext();

                Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();

                dictEmailValues.Add("<!--PWD-->", PWD);

                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);

                gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.FORGET_PASS, new string[] { email }, dictEmailValues, true);

                // throw new System.ArgumentException("BusinessLogicLayer.MyException", "original");
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PaymentProcessController][M:SendPasswordToCustomerOnEmail]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PaymentProcessController][M:SendPasswordToCustomerOnEmail]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
        }

        private void SendPasswordToCustomer(string guestMobileNumber, string password,string Name)
        {
            /*
              Indents:
            * Description: This method is used to send OTP to customer

            * Parameters: 

            * Precondition: 
            * Postcondition:
            * Logic: 
            */

            try
            {
                Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();
                dictSMSValues.Add("#--NAME--#", Name);
                dictSMSValues.Add("#--PASSWORD--#", password);

                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);

                gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.CUST_SEND_PWD, new string[] { guestMobileNumber }, dictSMSValues);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PaymentProcessController][M:SendOTPToCustomer]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PaymentProcessController][M:SendOTPToCustomer]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
        }

    }
}
