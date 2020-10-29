//-----------------------------------------------------------------------
// <copyright file="CustomerProfileController" company="Ezeelo Consumer Services Pvt. Ltd.">
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
using System.Text;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Mvc;
using System.Globalization;
using System.Net.Http.Formatting;
using Newtonsoft.Json.Serialization;
using System.Transactions;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using System.Web.Http.Results;
using RouteAttribute = System.Web.Http.RouteAttribute;

namespace API.Controllers
{
    public class CustomerProfileController : ApiController
    {
        MerchantNotification merchantNotification = new MerchantNotification();


        // GET api/customerprofile/5
        /// <summary>
        /// Get Customer Details
        /// </summary>
        /// <param name="lCustLoginID">CustomerLoginID</param>
        /// <returns></returns>
        [ValidateModel]
        // [TokenVerification] //---Commented by Ashwini 09-Jan-2017------------------//
        [ApiException]
        public object Get(long lCustLoginID)
        {
            object obj = new object();
            try
            {
                if (lCustLoginID <= 0)
                {
                    return obj = new { Success = 0, Message = "Invalid parameter.", data = string.Empty };
                }
                EzeeloDBContext db = new EzeeloDBContext();

                BusinessLogicLayer.CustomerDetails lcust = new BusinessLogicLayer.CustomerDetails(System.Web.HttpContext.Current.Server);
                CustomerDetailsViewModel cust = new CustomerDetailsViewModel();
                cust = lcust.GetCustomerDetails(lCustLoginID);

                if (cust != null)
                {
                    obj = new { Success = 1, Message = "Customer details found.", data = cust };
                }
                else
                {
                    obj = new { Success = 0, Message = "Customer details not found.", data = string.Empty };
                }
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }

            //var formatter = new JsonMediaTypeFormatter();
            //var json = formatter.SerializerSettings;
            //json.Converters.Add(new MyDateTimeConvertor());

            //json.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            //json.Formatting = Newtonsoft.Json.Formatting.Indented;
            //json.ContractResolver = new CamelCasePropertyNamesContractResolver();
            //json.Culture = new CultureInfo("it-IT");      

            // return Request.CreateResponse(HttpStatusCode.OK, cust, formatter);
            return obj;

        }
        /*
         * Changes By :- Pradnyakar N. Badge       
         * 
         */
        /// <summary>
        /// Customer Registration
        /// </summary>
        /// <param name="lcust">CustomerDetails</param>
        /// <returns>Registration Operation Status</returns>
        [ValidateModel]
        [LoginSuccess]
        [ApiException]
        // POST api/customerprofile
        public HttpResponseMessage Post(CustomerDetailsViewModel lcust)
        {
            EzeeloDBContext db = new EzeeloDBContext();
            object obj = new object();
            long uid = 0;

            if (lcust != null)
            {
                BusinessLogicLayer.CustomerDetails lcustDetails = new BusinessLogicLayer.CustomerDetails(System.Web.HttpContext.Current.Server);
                uid = lcustDetails.CreateCustomer(lcust);
            }
            else
            {
                obj = new { HTTPStatusCode = "400", UserMessage = "Invalid request", CustomerLoginID = 0 };
                return Request.CreateResponse(HttpStatusCode.BadRequest, obj);
            }

            if (uid == 106)
            {
                obj = new { HTTPStatusCode = "409", UserMessage = "The user with same email or mobile no is already exists. It Seems that you are already registerd.", CustomerLoginID = 0 };
                return Request.CreateResponse(HttpStatusCode.Conflict, obj);
            }
            else if (uid == 107)
            {
                obj = new { HTTPStatusCode = "400", UserMessage = "Invalid request, Email and Mobile No both can't be null", CustomerLoginID = 0 };
                return Request.CreateResponse(HttpStatusCode.BadRequest, obj);
            }
            else if (uid == 108)
            {
                obj = new { HTTPStatusCode = "400", UserMessage = "Mobile No is Already Exists", CustomerLoginID = db.UserLogins.Where(x => x.Mobile == lcust.MobileNo).FirstOrDefault().ID };
                return Request.CreateResponse(HttpStatusCode.BadRequest, obj);
            }
            else if (uid == 109)
            {
                obj = new { HTTPStatusCode = "400", UserMessage = "Email ID Already Exists", CustomerLoginID = db.UserLogins.Where(x => x.Email == lcust.EmailId).FirstOrDefault().ID };
                return Request.CreateResponse(HttpStatusCode.BadRequest, obj);
            }
            else if (uid == 500)
            {
                obj = new { HTTPStatusCode = "500", UserMessage = "Internal Server Error. There can be possibility that user with same email or mobile no is already exists. Please Verify or contact to admin.", CustomerLoginID = 0 };
                return Request.CreateResponse(HttpStatusCode.InternalServerError, obj);
            }
            else if (uid == 104)
            {
                obj = new { HTTPStatusCode = "304", UserMessage = "ConditionNotMet: Invalid Pincode.", CustomerLoginID = 0 };
                return Request.CreateResponse(HttpStatusCode.NotModified, obj);
            }
            else if (uid == 105)
            {
                obj = new { HTTPStatusCode = "304", UserMessage = "ConditionNotMet: Role 'Customer' does not Exists. Please contact to admin.", CustomerLoginID = 0 };
                return Request.CreateResponse(HttpStatusCode.NotModified, obj);
            }
            else
            {
                obj = new { HTTPStatusCode = "200", UserMessage = "Customer Registered Successfully.", CustomerLoginID = uid };
                return Request.CreateResponse(HttpStatusCode.OK, obj);
            }
            return Request.CreateResponse(HttpStatusCode.ExpectationFailed, new { HTTPStatusCode = "417", UserMessage = "Invalid request" });
        }

        /// <summary>
        /// Edit Customer Details
        /// </summary>
        /// <param name="id">Customer Login ID</param>
        /// <param name="lcust">Customer Details</param>
        /// <returns></returns>
        [ValidateModel]
        [TokenVerification]
        [ApiException]
        // PUT api/customerprofile/5
        public object Put(long id, CustomerDetailsViewModel lcust)
        {
            object obj = new object();
            EzeeloDBContext db = new EzeeloDBContext();

            try
            {
                if (!ModelState.IsValid)
                {
                    return obj = new { Success = 0, Message = "Validation faild.", data = string.Empty };
                    // obj = new { HTTPStatusCode = HttpStatusCode.BadRequest, UserMessage = "Validation faild." };
                }
                if (id != lcust.UserLoginID)
                {
                    return obj = new { Success = 0, Message = "Invalid request.", data = string.Empty };
                    // obj = new { HTTPStatusCode = HttpStatusCode.BadRequest, UserMessage = "Invalid request." };
                }
                if (lcust.AnswerList == null || lcust.AnswerList.Count <= 0)
                    return obj = new { Success = 0, Message = "Please select security question.", data = string.Empty };

                int lSecurityQuestion = 0;
                if (lcust.AnswerList != null && lcust.AnswerList.Count > 0)
                    lSecurityQuestion = lcust.AnswerList.Select(x => x.SecurityQuestionID ?? 0).FirstOrDefault();
                // Check email exist or not

                /*allow null email Address
                 * Pradnyakar Badge
                 * 05-01-2016
                 */
                if (lcust.EmailId != null)
                {
                    var isEmailExist = db.UserLogins.Where(x => x.ID != lcust.UserLoginID && x.Email == lcust.EmailId).ToList();
                    if (isEmailExist.Count() > 0)
                        return obj = new { Success = 0, Message = "Another user with same email is already exists.", data = string.Empty };
                }

                // Check mobile exist or not
                if (lcust.MobileNo != null)
                {
                    var isMobileExist = db.UserLogins.Where(x => x.ID != lcust.UserLoginID && x.Mobile == lcust.MobileNo).ToList();
                    if (isMobileExist.Count() > 0)
                        return obj = new { Success = 0, Message = "Another user with same mobile no is already exists.", data = string.Empty };
                }
                //Check validation
                if (lcust.Address == null || lcust.Address == string.Empty)
                    return obj = new { Success = 0, Message = "Please! Enter Address!!", data = string.Empty };
                //if (model.DOB == null)
                //{
                //    ViewBag.Message = "Please! Enter DOB!!"; 
                //    return View(model);
                //}
                if (lcust.Gender == null)
                    return obj = new { Success = 0, Message = "Please! Select Gender!!", data = string.Empty };
                using (TransactionScope ts = new TransactionScope())
                {
                    // Get pincode details from database
                    var lPincodeDB = from element in db.Pincodes
                                     where element.Name == lcust.Pincode
                                     select element.ID;

                    // Check user entered pincode is valid or not
                    if (lPincodeDB.Count() == 0)
                        return obj = new { Success = 0, Message = "Invalid Pincode!!", data = string.Empty };

                    // Get pincode id
                    int lPincodeID = 0;
                    foreach (var element in lPincodeDB)
                    {
                        lPincodeID = element;
                    }

                    // Changes in User login

                    UserLogin userLoginDB = db.UserLogins.Single(x => x.ID == id);

                    UserLogin userLogin = new UserLogin();

                    userLogin.ID = userLoginDB.ID;
                    userLogin.Mobile = lcust.MobileNo;
                    userLogin.Email = lcust.EmailId;
                    userLogin.Password = userLoginDB.Password;
                    userLogin.IsLocked = false;
                    userLogin.CreateBy = CommonFunctions.GetPersonalDetailsID(id);
                    userLogin.CreateDate = userLoginDB.CreateDate;
                    userLogin.ModifyBy = CommonFunctions.GetPersonalDetailsID(id);
                    userLogin.ModifyDate = DateTime.Now;

                    db.Entry(userLoginDB).CurrentValues.SetValues(userLogin);
                    db.SaveChanges();


                    // Changes in personal detail
                    PersonalDetail personalDetailDB = db.PersonalDetails.Single(x => x.UserLoginID == lcust.UserLoginID);

                    PersonalDetail personalDetail = new PersonalDetail();

                    personalDetail.ID = personalDetailDB.ID;
                    personalDetail.UserLoginID = personalDetailDB.UserLoginID;
                    personalDetail.SalutationID = lcust.SalutationID;
                    personalDetail.FirstName = lcust.FirstName;
                    personalDetail.MiddleName = lcust.MiddleName;
                    personalDetail.LastName = lcust.LastName;
                    personalDetail.DOB = lcust.DOB;
                    personalDetail.Gender = lcust.Gender;
                    personalDetail.PincodeID = lPincodeID;
                    personalDetail.Address = lcust.Address;
                    personalDetail.AlternateMobile = lcust.AlternateMobile;
                    personalDetail.AlternateEmail = null; //model.AlternateEmailID;
                    personalDetail.IsActive = personalDetailDB.IsActive;
                    personalDetail.CreateBy = personalDetailDB.CreateBy;
                    personalDetail.CreateDate = personalDetailDB.CreateDate;
                    personalDetail.ModifyBy = 1;
                    personalDetail.ModifyDate = DateTime.UtcNow.AddHours(5.5);
                    personalDetail.NetworkIP = personalDetailDB.NetworkIP;
                    personalDetail.DeviceID = personalDetailDB.DeviceID;
                    personalDetail.DeviceType = personalDetailDB.DeviceType;



                    db.Entry(personalDetailDB).CurrentValues.SetValues(personalDetail);
                    db.SaveChanges();

                    //Tejaswee (07-11-2015)
                    //Changes in customer shipping address
                    CustomerShippingAddress customershippingaddress = new CustomerShippingAddress();
                    customershippingaddress.PincodeID = lPincodeID;
                    customershippingaddress.PrimaryMobile = lcust.MobileNo;
                    customershippingaddress.SecondaryMobile = lcust.AlternateMobile;
                    customershippingaddress.ShippingAddress = lcust.Address;

                    customershippingaddress.CreateDate = DateTime.UtcNow;
                    customershippingaddress.CreateBy = 1;
                    customershippingaddress.ModifyDate = null;
                    customershippingaddress.ModifyBy = null;
                    customershippingaddress.NetworkIP = string.Empty;
                    customershippingaddress.DeviceType = string.Empty;
                    customershippingaddress.DeviceID = string.Empty;
                    customershippingaddress.UserLoginID = personalDetailDB.UserLoginID;
                    customershippingaddress.IsActive = true;

                    db.CustomerShippingAddresses.Add(customershippingaddress);
                    db.SaveChanges();

                    // Changes in login security anwser
                    LoginSecurityAnswer loginSecurityAnswer = new LoginSecurityAnswer();

                    loginSecurityAnswer.ID = 0;
                    loginSecurityAnswer.UserLoginID = lcust.UserLoginID;
                    loginSecurityAnswer.SecurityQuestionID = lSecurityQuestion;
                    if (lcust.AnswerList != null)
                        loginSecurityAnswer.Answer = lcust.AnswerList.Select(x => x.Answer).FirstOrDefault();
                    else
                        loginSecurityAnswer.Answer = string.Empty;
                    loginSecurityAnswer.CreateBy = 1;
                    loginSecurityAnswer.CreateDate = DateTime.UtcNow.AddHours(5.5);

                    var loginSecurityQuestionDB = db.LoginSecurityAnswers.Where(x => x.UserLoginID == lcust.UserLoginID);

                    // Execute if security question not exists!!
                    if (loginSecurityQuestionDB.Count() == 0)
                    {
                        db.LoginSecurityAnswers.Add(loginSecurityAnswer);
                        db.SaveChanges();
                    }
                    else
                    {
                        // Execute if security question exists!!
                        var lLoginSecurityAnswerDB = db.LoginSecurityAnswers.Single(x => x.UserLoginID == lcust.UserLoginID);

                        var lloginsecurityanswerdb = from element in db.LoginSecurityAnswers
                                                     where element.SecurityQuestionID == lSecurityQuestion
                                                     select element.ID;
                        Int64 ID = 0;
                        foreach (var element in lloginsecurityanswerdb)
                        {
                            ID = element;
                        }
                        loginSecurityAnswer.ID = lLoginSecurityAnswerDB.ID;

                        db.Entry(lLoginSecurityAnswerDB).CurrentValues.SetValues(loginSecurityAnswer);
                        db.SaveChanges();
                    }
                    obj = new { Success = 1, Message = "User Profile Updated Successfully!!", data = string.Empty };
                    // Commit transaction!!
                    ts.Complete();


                }


                //BusinessLogicLayer.CustomerDetails lcustDetails = new BusinessLogicLayer.CustomerDetails(System.Web.HttpContext.Current.Server);
                //long oprStatus = lcustDetails.EditCustomer(lcust);
                //if (oprStatus == 102)
                //    obj = new { Success = 1, Message = "Profile Updated Successfully.", data = string.Empty };
                ////obj = new { HTTPStatusCode = "200", UserMessage = "Profile Updated Successfully." };
                ////return Request.CreateResponse(HttpStatusCode.OK, obj);
                //else if (oprStatus == 108)
                //    // {
                //    obj = new { Success = 0, Message = "User with provided login ID does not exists.", data = string.Empty };
                ////obj = new { HTTPStatusCode = HttpStatusCode.BadRequest, UserMessage = "User with provided login ID does not exists." };
                ////return Request.CreateResponse(HttpStatusCode.BadRequest, obj);
                ////}
                //else if (oprStatus == 107)
                //    //{
                //    obj = new { Success = 0, Message = "ConditionNotMet: Please provide Customer login ID.", data = string.Empty };
                ////    obj = new { HTTPStatusCode = "304", UserMessage = "ConditionNotMet: Please provide Customer login ID." };
                ////    return Request.CreateResponse(HttpStatusCode.NotModified, obj);
                ////}
                //if (oprStatus == 106)
                //    obj = new { Success = 0, Message = "Another user with same email or mobile no is already exists.", data = string.Empty };
                //// obj = new { HTTPStatusCode = "409", UserMessage = "Another user with same email or mobile no is already exists." };
                //else if (oprStatus == 100)
                //    //{
                //    obj = new { Success = 0, Message = "ConditionNotMet: Invalid UserLogin ID.", data = string.Empty };
                ////obj = new { HTTPStatusCode = "304", UserMessage = "ConditionNotMet: Invalid UserLogin ID." };
                ////return Request.CreateResponse(HttpStatusCode.NotModified, obj);
                ////}
                //else if (oprStatus == 500)
                //    //{
                //    obj = new { Success = 0, Message = "Internal Server Error. There can be possibility that another user with same email or mobile no is already exists. Please Verify or contact to admin.", data = string.Empty };
                ////    obj = new { HTTPStatusCode = "500", UserMessage = "Internal Server Error. There can be possibility that another user with same email or mobile no is already exists. Please Verify or contact to admin." };
                ////    return Request.CreateResponse(HttpStatusCode.InternalServerError, obj);
                ////}
                //else if (oprStatus == 104)
                //    //{
                //    obj = new { Success = 0, Message = "ConditionNotMet: Invalid Pincode.", data = string.Empty };
                ////    obj = new { HTTPStatusCode = "304", UserMessage = "ConditionNotMet: Invalid Pincode." };
                ////    return Request.CreateResponse(HttpStatusCode.NotModified, obj);

                ////}
                //else if (oprStatus == 105)
                //    //{
                //    obj = new { Success = 0, Message = "ConditionNotMet: Role 'Customer' does not Exists. Please contact to admin.", data = string.Empty };
                ////    obj = new { HTTPStatusCode = "304", UserMessage = "ConditionNotMet: Role 'Customer' does not Exists. Please contact to admin." };
                ////    return Request.CreateResponse(HttpStatusCode.NotModified, obj);
                ////}
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }

            //   return Request.CreateResponse(HttpStatusCode.ExpectationFailed, new { HTTPStatusCode = "417", UserMessage = "Invalid request" });
            return obj;

        }

        //[HttpGet]
        //[System.Web.Http.Route("getuserupto16level")]
        [HttpGet]
        [Route("api/customerprofile/getuserupto16level")]
        public object GetUsersUpto16Level(int userId)
        {
            EzeeloDBContext db = new EzeeloDBContext();
            List<MLMUser> mLMUsers = new List<MLMUser>();
            try
            {
                MLMUser mlmuser = db.MLMUsers.FirstOrDefault(q => q.Ref_Id == db.MLMUsers.FirstOrDefault(p => p.UserID == userId).Refered_Id_ref);

                if (mlmuser != null)
                    mLMUsers.Add(mlmuser);
                else
                    return new { data = 0 };

                if (mLMUsers != null && mLMUsers.Count > 0)
                {
                    for (int i = 0; i < 15; i++)
                    {
                        if (mLMUsers[i] != null)
                        {
                            long tempUserId = mLMUsers[i].UserID;
                            MLMUser tempMlmUser = db.MLMUsers.FirstOrDefault(q => q.Ref_Id == db.MLMUsers.FirstOrDefault(p => p.UserID == tempUserId).Refered_Id_ref);
                            if (tempMlmUser == null)
                                break;
                            else
                                mLMUsers.Add(tempMlmUser);
                        }
                        else
                            break;
                    }
                }

                return new { data = mLMUsers };
            }
            catch (Exception ex)
            {
                throw new Exception("MLMWalletPoints : GetUpLine() " + ex.Message);
            }
        }

        //[HttpGet]
        //[System.Web.Http.Route("getuserupto16level")]
        [System.Web.Mvc.HttpPost]
        [Route("api/customerprofile/transactiondistributionlevel16")]
        public object AddMerchantTransactionDistributionUptoLevel16(MerchantTransactionDistributionLevel16 distributionLevel16ListReq)
        {
            EzeeloDBContext db = new EzeeloDBContext();

            if (distributionLevel16ListReq == null)
                return new { data = "Your list was empty or parameters didn't match. Try again" }; ;

            MerchantTransactionDistributionLevel16 distributionLevel16s = new MerchantTransactionDistributionLevel16();
            distributionLevel16s = distributionLevel16ListReq;

            try
            {
                db.MerchantTransactionDistributionLevel16.Add(distributionLevel16s);
                db.SaveChanges();
                return new { data =  "Success!" };
            }
            catch (Exception ex)
            {
                throw new Exception("Server error :  " + ex.Message);
            }
        }
        [HttpGet]
        [Route("api/customerprofile/updatetransactiondistributionlevel16")]
        public object UpdateMerchantTransactionDistributionUptoLevel16(int merchantTransactionId)
        {
            EzeeloDBContext db = new EzeeloDBContext();

            if (merchantTransactionId == 0)
                return new { data = "Your list was empty or parameters didn't match. Try again" };

            var distribution = db.MerchantTransactionDistributionLevel16.Where(x => x.MerchantTransactionId == merchantTransactionId).FirstOrDefault();

            distribution.Status = true;

            try
            {
                db.SaveChanges();
                return new { data = "Success!" };
            }
            catch (Exception ex)
            {
                throw new Exception("Server error :  " + ex.Message);
            }
        }
        [System.Web.Mvc.HttpGet]
        [Route("api/customerprofile/deductmerchantcomission")]
        public object DeductCommissionFromRecharge(long MerchantID, decimal Commission)
        {
            EzeeloDBContext db = new EzeeloDBContext();

            MerchantTopupRecharge merchantTopupRecharges = db.merchantTopupRecharges.FirstOrDefault(p => p.MerchantID == MerchantID);
            Merchant obj = db.Merchants.FirstOrDefault(p => p.Id == MerchantID);
            if (merchantTopupRecharges != null)
            {
                if (merchantTopupRecharges.Amount >= Commission)
                {
                    merchantTopupRecharges.Amount = merchantTopupRecharges.Amount - Commission;
                    db.SaveChanges();
                }
                //check for condition
                MerchantCommonValues values = db.MerchantCommonValues.FirstOrDefault();
                decimal RemainingAMount = merchantTopupRecharges.Amount;
                decimal WarningPer = values.TopupMax;
                decimal BlockedPer = values.TopupMin;
                decimal BlockedValue = merchantTopupRecharges.TopupAmount * (BlockedPer / 100);
                decimal WarningValue = merchantTopupRecharges.TopupAmount * (WarningPer / 100);
                if (BlockedValue >= RemainingAMount)
                {
                    //Send blocked meassage
                    SendSMS_RechargeBlocked(MerchantID);
                    merchantNotification.SaveNotification(6, obj.FranchiseName);
                    return new { message = "Recharge blocked, SMS sent", code = 0 };
                }
                else if (WarningValue >= RemainingAMount)
                {
                    //Send warning message
                    SendSMS_RechargeWarning(MerchantID);
                    merchantNotification.SaveNotification(7, obj.FranchiseName);
                    return new { message = "Topup Balance too low, SMS sent", code = -1 };
                }
                return new { message = "Balance deducted, success!", code = 1 };
            }

            return new { message = "Merchant doesn't have any money in top up!", code = -2 };
        }

        public void SendSMS_RechargeBlocked(long MerchantId)
        {
            EzeeloDBContext db = new EzeeloDBContext();

            try
            {
                Merchant mer = db.Merchants.FirstOrDefault(p => p.Id == MerchantId);
                Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();
                dictSMSValues.Add("#--NAME--#", mer.ContactPerson);

                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.MERCHANT_TRANSACTION_2WARNING, new string[] { mer.ContactNumber }, dictSMSValues);
                gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.LEADER_TRANSACTION_2WARNING, new string[] { mer.LeaderContactNo }, dictSMSValues);
            }
            catch
            {

            }
        }

        public void SendSMS_RechargeWarning(long MerchantId)
        {
            EzeeloDBContext db = new EzeeloDBContext();

            try
            {
                Merchant mer = db.Merchants.FirstOrDefault(p => p.Id == MerchantId);
                Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();
                dictSMSValues.Add("#--NAME--#", mer.ContactPerson);

                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.MERCHANT_RECHARGE_WARNING, new string[] { mer.ContactNumber }, dictSMSValues);
                gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.LEADER_RECHARGE_WARNING, new string[] { mer.LeaderContactNo }, dictSMSValues);
            }
            catch
            {

            }
        }

        [HttpGet]
        [Route("api/customerprofile/merchantonline")]
        public object MarkMerchantAsOnline(int merchantId)
        {
            EzeeloDBContext db = new EzeeloDBContext();

            if (merchantId == 0)
                return new { data = "Your list was empty or parameters didn't match. Try again", responseCode = 0 };

            var merchant = db.Merchants.Where(x => x.Id == merchantId).FirstOrDefault();

            try
            {
                merchant.IsOnline = true;
                db.SaveChanges();
                return new { data = "Success!", responseCode = 1 };
            }
            catch (Exception ex)
            {
                throw new Exception("Server error :  " + ex.Message);
            }
        }
    }
}
