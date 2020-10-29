using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using API.Models;
using System.Data.Entity.Validation;
using ModelLayer.Models;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using System.Data.Entity;

namespace API.Controllers
{
    public class CustomerRegistrationController : ApiController
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        [ApiException]
        //[ValidateModel]
        [LoginSuccess]
        public object post(CustomerRegistrationViewModel model)
        {
            object obj = new object();
            if (string.IsNullOrEmpty(model.FirstName) || string.IsNullOrEmpty(model.MobileNo) || string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.ConfirmPassword))
            {
                Request.Properties["UserLoginId"] = null;//Added by Sonali for authorization filter on 16-04-2019
                return obj = new { Success = 0, Message = "Sorry! Please enter valid details.", data = string.Empty };
            }
            if (!Regex.IsMatch(model.MobileNo, @"^([5-9]{1}[0-9]{9})$"))//Sonali_24/10/2018
            {
                Request.Properties["UserLoginId"] = null;//Added by Sonali for authorization filter on 16-04-2019
                return obj = new { Success = 0, Message = "Enter valid MobileNo.", data = string.Empty };
            }
            if (string.IsNullOrEmpty(model.ReferralId))
            {
                Request.Properties["UserLoginId"] = null;
                return obj = new { Success = 0, Message = "Please enter ReferralId.", data = string.Empty };
            }
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                if (model.EmailId != null)
                {
                    if (model.EmailId.Trim().ToString().Equals(""))
                    {
                        model.EmailId = null;
                    }
                }

                //ModelState.Clear();
                if (string.IsNullOrEmpty(model.LastName))
                {
                    model.LastName = "  ";
                }
                if (ModelState.IsValid)
                {
                    var lRole = db.Roles.Where(x => x.Name == "Customer".ToUpper()).ToList();

                    if (lRole.Count() <= 0)
                    {
                        Request.Properties["UserLoginId"] = null;//Added by Sonali for authorization filter on 16-04-2019
                        return obj = new { Success = 0, Message = "Role not exist!!", data = string.Empty };
                    }
                    //using (TransactionScope ts = new TransactionScope())
                    //{
                    try
                    {
                        string lMessage = string.Empty;
                        string mobEmail = string.Empty;
                        string ReferralId = string.Empty;
                        // Check user details exists or not
                        lMessage = CommonFunctions.CheckUserDetails(model.EmailId, model.MobileNo, out mobEmail);

                        if (lMessage != string.Empty)
                        {
                            Request.Properties["UserLoginId"] = null;//Added by Sonali for authorization filter on 16-04-2019
                            return obj = new { Success = 0, Message = lMessage, data = string.Empty };
                        }
                        /* Started ReferralId validation on 28-11-2018*/
                        if (!string.IsNullOrEmpty(model.ReferralId))
                        {
                            bool chkReferralId = db.MLMUsers.Any(p => p.Ref_Id == model.ReferralId && db.UserLogins.Where(u => u.IsLocked == false).Select(u => u.ID).ToList().Contains(p.UserID));
                            if (chkReferralId != true)
                            {
                                Request.Properties["UserLoginId"] = null;//Added by Sonali for authorization filter on 16-04-2019
                                return obj = new { Success = 0, Message = "Invalid Referral ID | Referral ID “ " + model.ReferralId + " ” entered by you does not exist. | Please add a valid Referral ID.", data = string.Empty };
                            }
                           
                        }
                        else
                        {
                            Request.Properties["UserLoginId"] = null;
                            return obj = new { Success = 0, Message = "Please enter ReferralId.", data = string.Empty };
                        }
                        int SendOTPValue =CommonFunctions.SendOTP(model.EmailId, model.MobileNo, model.FirstName,true,true);
                        if (SendOTPValue == 1)
                        {
                            Request.Properties["UserLoginId"] = null;
                            return obj = new { Success = 1, Message = "OTP has been sent to your registered mobile number and email. This OTP is valid for 15 minutes (till SHOW_TIME)", data = new { StatusCode = 1 } };

                        }
                        else if (SendOTPValue == -3)
                        {
                            Request.Properties["UserLoginId"] = null;
                            return obj = new { Success = 1, Message = "Please contact customer care for OTP on +9172221910", data = new { StatusCode = 1 } };

                        }
                        else if (SendOTPValue == 2 || SendOTPValue ==-1)
                        {
                            Request.Properties["UserLoginId"] = null;
                            return obj = new { Success = 0, Message = "OTP regenerate limit exceeds.", data = new { StatusCode = -1 } };

                        }
                       
                        /* Ended ReferralId validation on 28-11-2018*/
                        // Creating user login
                        UserLogin userLogin = new UserLogin();
                        userLogin.ID = 0;
                        userLogin.Mobile = model.MobileNo;
                        userLogin.Email = model.EmailId;
                        userLogin.Password = model.Password;
                        userLogin.IsLocked = false;
                        userLogin.CreateBy = 1;
                        userLogin.CreateDate = DateTime.UtcNow.AddHours(5.5);

                        db.UserLogins.Add(userLogin);
                        db.SaveChanges();

                        // Getting current user login id
                        Int64 id = userLogin.ID;

                        // storing personal details
                        PersonalDetail personalDetail = new PersonalDetail();
                        personalDetail.ID = 0;
                        personalDetail.UserLoginID = id;
                        //personalDetail.SalutationID = model.ID;
                        personalDetail.SalutationID = 1;
                        personalDetail.FirstName = model.FirstName;
                        personalDetail.MiddleName = model.MiddleName;
                        personalDetail.LastName = model.LastName;
                        personalDetail.IsActive = true;
                        personalDetail.CreateBy = 1;
                        personalDetail.CreateDate = DateTime.UtcNow.AddHours(5.5);

                        db.PersonalDetails.Add(personalDetail);
                        db.SaveChanges();

                        UserRole userRole = new UserRole();

                        userRole.ID = 0;
                        userRole.RoleID = lRole.FirstOrDefault().ID;
                        userRole.UserLoginID = id;
                        userRole.IsActive = true;
                        userRole.CreateDate = DateTime.Now;
                        userRole.CreateBy = CommonFunctions.GetPersonalDetailsID(id);

                        db.UserRoles.Add(userRole);
                        db.SaveChanges();
                        //====================================Update referenceID from referedetail table=============Manoj==//
                        if (db.ReferDetails.Any(m => (m.Email == model.EmailId || m.Mobile == model.MobileNo) && m.ReferenceID == null))
                        {
                            ReferDetail lReferDetail = db.ReferDetails.Where(m => m.Email == model.EmailId || m.Mobile == model.MobileNo).FirstOrDefault();
                            if (lReferDetail != null)
                            {
                                //Update referance id in referDetail table
                                //i.e. Refer user now registered with eZeelo
                                lReferDetail.ReferenceID = id;
                                lReferDetail.ModifyDate = DateTime.Now;
                                lReferDetail.ModifyBy = CommonFunctions.GetPersonalDetailsID(id);
                                lReferDetail.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                db.SaveChanges();
                            }
                            //Check if their is user wise earn 
                            var earn = (from RES in db.ReferAndEarnSchemas
                                        join RD in db.ReferDetails on RES.ID equals RD.ReferAndEarnSchemaID
                                        where RES.UserwiseEarn == true && RD.ID == lReferDetail.ID
                                        select new { EarnMoney = RES.EarnInRS }).FirstOrDefault();
                            //If user wise earn then add earn money to refer by user account
                            if (earn != null)
                            {
                                decimal EarnRs = Convert.ToDecimal(earn.EarnMoney);

                                var PrevRemainingAmt = db.EarnDetails.OrderByDescending(u => u.ID).Where(x => x.EarnUID == lReferDetail.UserID).Select(x => x.RemainingAmount).FirstOrDefault();

                                EarnDetail lEarnDetail = new EarnDetail();
                                lEarnDetail.EarnUID = lReferDetail.UserID;
                                lEarnDetail.ReferUID = lReferDetail.ReferenceID;
                                //lEarnDetail.EarnAmount = earn.FirstOrDefault().EarnMoney;
                                lEarnDetail.EarnAmount = EarnRs;
                                lEarnDetail.UsedAmount = 0;
                                if (PrevRemainingAmt != null)
                                {
                                    lEarnDetail.RemainingAmount = PrevRemainingAmt + EarnRs;
                                }
                                else
                                {
                                    lEarnDetail.RemainingAmount = EarnRs;
                                }
                                lEarnDetail.CustomerOrderID = null;
                                lEarnDetail.CreateDate = DateTime.Now;
                                lEarnDetail.CreateBy = CommonFunctions.GetPersonalDetailsID(id);
                                lEarnDetail.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                db.EarnDetails.Add(lEarnDetail);
                                db.SaveChanges();
                            }

                        }

                        //==================================================================================================//
                        // Transaction complete
                        //ts.Complete();
                        //Start Yashaswi Leader Signup
                        try
                        {
                            if (!String.IsNullOrEmpty(model.ReferralId))
                            {
                                //using (TransactionScope ts1 = new TransactionScope())
                                //{
                                MLMWalletPoints MLMWallet = new MLMWalletPoints();
                                //long LoginUserId = id;
                                //UserLogin userLog = db.UserLogins.FirstOrDefault(p => p.ID == LoginUserId);
                                string result = MLMWallet.LeadersSingUp(id, model.Password, model.EmailId, model.ReferralId);
                                string UserName = "";

                                if (result.Contains("R_DONE"))
                                {
                                    try
                                    {
                                        string RefId = model.ReferralId;
                                        UserName = " under " + db.PersonalDetails.FirstOrDefault(p => p.UserLoginID == (db.MLMUsers.FirstOrDefault(m => m.Ref_Id == RefId).UserID)).FirstName;
                                        ReferralId = db.MLMUsers.Where(x => x.UserID == userLogin.ID).Select(x => x.Ref_Id).FirstOrDefault();
                                    }
                                    catch
                                    {

                                    }

                                    //SendEmailToCustomer(model.EmailId, model.FirstName, model.City, model.FranchiseID);
                                    SendMessageToCustomer(model.MobileNo, model.FirstName);
                                    Request.Properties["UserLoginId"] = userLogin.ID;//Added by Sonali for authorization filter on 16-04-2019
                                    return obj = new { Success = 1, Message = "Congratulations | You have successfully joined as Ezeelo Leader" + UserName + "| Welcome to Ezeelo Family.", data = new { UserLoginID = userLogin.ID, UserName = UserName, EmailID = userLogin.Email, MobileNo = userLogin.Mobile, FirstName = personalDetail.FirstName, LastName = personalDetail.LastName, IsMLMUser = true, ReferralId = ReferralId } };
                                }
                                else if (result.Contains("ALREADY_R"))
                                {
                                    try
                                    {
                                        string RefId = db.MLMUsers.FirstOrDefault(p => p.UserID == id).Refered_Id_ref;
                                        UserName = " under " + db.PersonalDetails.FirstOrDefault(p => p.UserLoginID == (db.MLMUsers.FirstOrDefault(m => m.Ref_Id == RefId).UserID)).FirstName;
                                        ReferralId = db.MLMUsers.Where(x => x.UserID == userLogin.ID).Select(x => x.Ref_Id).FirstOrDefault();
                                        // ReferralId = RefId;
                                    }
                                    catch
                                    {

                                    }
                                    Request.Properties["UserLoginId"] = userLogin.ID;//Added by Sonali for authorization filter on 16-04-2019
                                    return obj = new { Success = 0, Message = "You Are Already Registered As Ezeelo Member" + UserName, data = new { UserLoginID = userLogin.ID, UserName = UserName, EmailID = userLogin.Email, MobileNo = userLogin.Mobile, FirstName = personalDetail.FirstName, LastName = personalDetail.LastName, IsMLMUser = true, ReferralId = ReferralId } };
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
                        }
                        catch
                        {
                            Request.Properties["UserLoginId"] = null;//Added by Sonali for authorization filter on 16-04-2019
                            return obj = new { Success = 0, Message = "Facing Any Trouble , Happy To Help You, Simply Call 9172221910", data = string.Empty };
                        }
                        //End

                        var IsMLMUser = db.MLMUsers.Where(x => x.UserID == userLogin.ID).Any();
                        if (IsMLMUser)
                        {
                            ReferralId = db.MLMUsers.Where(x => x.UserID == userLogin.ID).Select(x => x.Ref_Id).FirstOrDefault();
                        }
                        //Tejaswee 28/7/2015
                        //3) Send email, message to customer
                        //SendEmailToCustomer(model.EmailId, model.FirstName, model.City, model.FranchiseID);
                        //SendMessageToCustomer(model.MobileNo, model.FirstName);
                        SendMessageToCustomer(model.MobileNo, model.FirstName);
                        // Clear model state
                        ModelState.Clear();
                        Request.Properties["UserLoginId"] = userLogin.ID;//Added by Sonali for authorization filter on 16-04-2019
                        //ViewBag.Message = "Done! Registration Successfully Done!!";
                        obj = new { Success = 1, Message = "Done! Registration Successfully Done!!", data = new { UserLoginID = userLogin.ID, UserName = personalDetail.FirstName, EmailID = userLogin.Email, MobileNo = userLogin.Mobile, FirstName = personalDetail.FirstName, LastName = personalDetail.LastName, IsMLMUser = IsMLMUser, ReferralId = ReferralId } };
                        // new { HTTPStatusCode = "200 ", UserMessage = "Done! Registration Successfully Done!!", UserLoginID = id, UserName = personalDetail.FirstName, EmailID = userLogin.Email, MobileNo = userLogin.Mobile };
                        //Session["UID"] = id;
                        model = null;

                        //4) Return to user profile page

                    }
                    catch (Exception exception)
                    {
                        Request.Properties["UserLoginId"] = null;//Added by Sonali for authorization filter on 16-04-2019
                        //ViewBag.Message = "Sorry! Problem in customer registration!!";
                        return obj = new { Success = 0, Message = "Sorry! Problem in customer registration!!", data = string.Empty };
                        //return obj = new { HTTPStatusCode = "400", UserMessage = "", UserLoginID = 0, UserName = string.Empty };
                        // Rollback transaction
                        //ts.Dispose();

                        //return View("_Login", "_Layout_Internal");
                    }
                    //}
                }
                else
                {
                    var errors = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
                    string Message = string.Empty;
                    foreach (var item in errors)
                    {
                        Message = item.FirstOrDefault().ErrorMessage;
                        break;
                    }
                    Request.Properties["UserLoginId"] = null;//Added by Sonali for authorization filter on 16-04-2019
                    return obj = new { Success = 0, Message = Message, data = string.Empty };
                }
                return obj;
            }
            catch (DbEntityValidationException ex)
            {
                Request.Properties["UserLoginId"] = null;//Added by Sonali for authorization filter on 16-04-2019
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => new { x.ErrorMessage });

                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                return obj = new { Success = 0, Message = exceptionMessage, data = string.Empty };
                //return obj = new { HTTPStatusCode = "400", UserMessage = exceptionMessage, UserLoginID = 0, UserName = string.Empty };
                //ViewBag.Message = fullErrorMessage.Replace("ErrorMessage = ", " ");
                //return View(model);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                Request.Properties["UserLoginId"] = null;//Added by Sonali for authorization filter on 16-04-2019
                //ViewBag.Message = "Sorry! Problem in customer registration!!";

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CustomerController][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
                return obj = new { Success = 0, Message = "Sorry!Problem in customer registration!!", data = string.Empty };
                // return obj = new { HTTPStatusCode = "400", UserMessage = "Sorry! Problem in customer registration!!", UserLoginID = 0, UserName = string.Empty };
                //return View();
            }
            catch (Exception ex)
            {
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                Request.Properties["UserLoginId"] = null;//Added by Sonali for authorization filter on 16-04-2019
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CustomerController][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
                return obj = new { Success = 0, Message = "Sorry!Problem in customer registration!!", data = string.Empty };
                // return obj = new { HTTPStatusCode = "400", UserMessage = "Sorry! Problem in customer registration!!", UserLoginID = 0, UserName = string.Empty };
                //return View();
            }
        }
      [HttpPost]
        [Route("api/CustomerRegistration/ValidateOTP")]
        public object IsOtpValid(RegisterOTPViewModel Rovm)
        {
            object obj = new object();
            int EmailOTPValid = 0;
            int MobileOTPValid = 0;
            OTPLog RegistrationOTPLog = db.OTPLogs.Where(x => x.Email == Rovm.EmailId && x.Mobile == Rovm.MobileNo && x.IsActive == true && DbFunctions.TruncateTime(x.CreateDate) == DbFunctions.TruncateTime(DateTime.Now)).FirstOrDefault();
            if (RegistrationOTPLog != null)
            {
                if (DateTime.Now < Convert.ToDateTime(RegistrationOTPLog.OTPExpire))
                {

                    if (RegistrationOTPLog.EmailOTP == Rovm.EmailOTP)
                    {
                        EmailOTPValid = 1;
                    }
                    if (RegistrationOTPLog.MobileOTP == Rovm.MobileOTP)
                    {
                        MobileOTPValid = 1;
                    }
                    if (EmailOTPValid == 1 && MobileOTPValid == 1)
                    {
                        RegistrationOTPLog.IsActive = false;
                        RegistrationOTPLog.IsValidated = true;
                        db.SaveChanges();
                        return obj = new { Success = 1, Message = "OTP validated.", data = new { StatusCode = 1 } };
                    }
                    else
                    {
                        return obj = new { Success = 0, Message = "Invalid OTP.", data = new { StatusCode=0 } };
                    }
                    //RegistrationOTPLog.IsActive = false;


                }
                else
                {
                    EmailOTPValid = -1;
                    MobileOTPValid = -1;
                    return obj = new { Success = 0, Message = "OTP expired.", data = new { StatusCode = -1 } };
                }
            }
            else
            {
                return obj = new { Success = 0, Message = "No record for OTP found.", data = new { StatusCode = 0 } };
            }



        }
        private void SendEmailToCustomer(string emailID, string name, string city, int? FranchiseID)
        {
            /*
               Indents:
             * Description: This method is used to send email to customer
             
             * Parameters: 
             
             * Precondition: 
             * Postcondition:
             * Logic: 
             */
            try
            {
                if (string.IsNullOrEmpty(city) && (FranchiseID == null || FranchiseID == 0))
                {
                    city = "nagpur";
                    FranchiseID = 2;
                }

                //string city = "nagpur";
                //int FranchiseID = 2;////added
                //if (System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value != null)
                //{
                //    city = System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower();
                //    FranchiseID = Convert.ToInt32(System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value.Split('$')[2]);////added
                //}
                Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();
                //dictEmailValues.Add("<!--ACCOUNT_URL-->", "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "" + city + "/" + FranchiseID + "/login");////added "/" + FranchiseID +
                //dictEmailValues.Add("<!--ORDERS_URL-->", "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "" + city + "/" + FranchiseID + "/cust-o/my-order");////added "/" + FranchiseID +
                dictEmailValues.Add("<!--NAME-->", name);
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH,
                    BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.CUST_WELCOME,
                    new string[] { emailID, "sales@ezeelo.com" }, dictEmailValues, true);
            }
            catch (Exception)
            {

                //Mantain Error Log
            }
        }

        private void SendMessageToCustomer(string mobileNo, string name)
        {
            /*
               Indents:
             * Description: This method is used to send message to customer
             
             * Parameters: 
             
             * Precondition: 
             * Postcondition:
             * Logic: 
             */

            try
            {
                string RefId = "";
                UserLogin user = db.UserLogins.FirstOrDefault(p => p.Mobile == mobileNo);
                if (user != null)
                {
                    MLMUser mLMUser = db.MLMUsers.FirstOrDefault(p => p.UserID == user.ID);
                    if(mLMUser != null)
                    {
                        RefId = mLMUser.Ref_Id;
                    }
                }
                Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                dictSMSValues.Add("#--NAME--#", name);
                dictSMSValues.Add("#--REFID--#", RefId);
                gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT,
                    BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.CUST_WELCOME,
                    new string[] { mobileNo }, dictSMSValues);
            }
            catch (Exception)
            {

                //Mantain Error Log
            }
        }

    }
}
