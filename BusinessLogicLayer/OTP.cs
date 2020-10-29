using ModelLayer.Models;
/*=============================================================================================================
 * <Organisation> Ezeelo Consumer Services Pvt. Ltd. </Organisation>
 * 
 * <Copyrights> 
 *  Copyrights to NSP Futuretech. Pvt. Ltd. 
 *  All contents are not subject to change before prior permission of the author or copyright owner</Copyrights>
 *  
 * <Author> Gaurav Dixit </Author>
 * 
 * <CreationDate> MAY 21, 2015 5.30pm </CreationDate>
 * 
 * <Version>1.0.0</Version>
 * ============================================================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/*
 Handed over to Pradnyakar Sir
 */
namespace BusinessLogicLayer
{
    public class OTP : ISecurities
    {
        EzeeloDBContext db = new EzeeloDBContext();
        /*   
            Developed By: Gaurav Dixit
            Created Date: May 18, 2015
        */

        /// <summary>
        /// This method is used to generate OTP and SessionCode
        /// </summary>
        /// <param name="pSessionPrefix">Session Prefix which will be formed a session code with OTP Generated</param>
        /// <returns>Dictionary Object
        /// KEY:1. OTP 
        ///     2. USC
        /// 
        /// </returns>
        public static Dictionary<string, string> GenerateOTP(string pSessionPrefix)
        {
            // Declare string variable for storing One Time Password
            string lOneTimePassword = string.Empty, lSessionCode = string.Empty;

            // Declare dictionary for storing OTP and Session Code
            Dictionary<string, string> dictionaryOTP = new Dictionary<string, string>();
            
            try
            {
                //Create object of random class
                Random random = new Random();
                // Set One Time Password by using next function of random class
            
                lOneTimePassword = random.Next(100000, 999999).ToString();
                //Generate Session Code
                lSessionCode = GenerateSessionCode(lOneTimePassword, pSessionPrefix).Substring(0,8).ToString();

                // Add values to dictionary object
                dictionaryOTP.Add("OTP", lOneTimePassword.Substring(0, 4).ToString());
                dictionaryOTP.Add("USC", lSessionCode);

            }
            catch (Exception ex)
            {
                throw new Exception("Can't generate One Time Password" + Environment.NewLine + ex.InnerException);
            }
            // Return dictionary
            return dictionaryOTP;
        }

        /// <summary>
        /// This method is used to generate session code
        /// </summary>
        /// <param name="pOneTimePassword">OTP which will be suffixed with SessionPrefix provided</param>
        /// <param name="pSessionPrefix">Session Prefix which will be formed a session code with OTP provided</param>
        /// <returns>Session Code String [i.e. Prefix + OTP]</returns>
        private static string GenerateSessionCode(string pOneTimePassword, string pSessionPrefix)
        {
            // Create object of StringBuilder Class
            StringBuilder sb = new StringBuilder();
            try
            {
                // Append prefix and OTP to string builder object
                sb.Append(pSessionPrefix + pOneTimePassword);
            }
            catch (Exception ex)
            {
                throw new Exception("Can't generate session code" + Environment.NewLine + ex.InnerException);
            }
            // Return Session Code
            return sb.ToString();
        }

        public string[] AuthorizedUserRight(System.Web.HttpServerUtility server, string ApplicationName, long LoginID)
        {
            throw new NotImplementedException();
        }

        public void InsertOTPDetails(string sessionCode, string oneTimePassword, params string[] ShopOrderCode) //Added ShopOrderCode
        {
            /*
              Indents:
            * Description: This method is used to insert OTP in OTP table
             
            * Parameters: 
             
            * Precondition: 
            * Postcondition:
            * Logic: 
            */

            try
            {
                if (ShopOrderCode != null)
                {
                    // Add new By Ashish Nagrale //
                    // Hide from Ashish for Live
                   /* int index = ShopOrderCode.Length - 1;
                    string PayableAmount = ShopOrderCode[index];
                    Array.Resize(ref ShopOrderCode, ShopOrderCode.Length - 1);
                    EzeeloDBContext dbOTP = new EzeeloDBContext();
                    foreach (string list in ShopOrderCode)//Added loop
                    {
                        // string shopOrderCode = ShopOrderCode[0].ToString();//hide
                        long CustID = db.CustomerOrderDetails.Where(x => x.ShopOrderCode == list).Select(x => x.CustomerOrderID).FirstOrDefault();//shopOrderCode with list
                        string CustOrderCode = db.CustomerOrders.Where(x => x.ID == CustID).Select(x => x.OrderCode).FirstOrDefault();

                        ModelLayer.Models.OTP otp = new ModelLayer.Models.OTP();

                        otp.ID = 0;
                        otp.SessionCode = sessionCode;
                        otp.OTP1 = oneTimePassword;
                        otp.IsActive = true;
                        otp.CreateDate = CommonFunctions.GetLocalTime();
                        otp.ExpirationTime = CommonFunctions.GetLocalTime().AddMinutes(10);
                        otp.CreateBy = 1;
                        // otp.OrderCode = OrderCode[0];//old hide
                        otp.OrderCode = CustOrderCode;//added
                        otp.ShopOrderCode = list.ToString();// ShopOrderCode[0];//added
                        otp.PayableAmount = decimal.Parse(PayableAmount);
                        dbOTP.OTPs.Add(otp);
                    }
                    dbOTP.SaveChanges();
                    */
                }
                else
                {//Old code
                    ModelLayer.Models.OTP otp = new ModelLayer.Models.OTP();

                    otp.ID = 0;
                    otp.SessionCode = sessionCode;
                    otp.OTP1 = oneTimePassword;
                    otp.IsActive = true;
                    otp.CreateDate = CommonFunctions.GetLocalTime();
                    otp.ExpirationTime = CommonFunctions.GetLocalTime().AddMinutes(10);
                    otp.CreateBy = 1;

                    db.OTPs.Add(otp);
                    db.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SendOTPToCustomer(string mobileNumber, string OTP)
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

                dictSMSValues.Add("#--PASSWORD--#", OTP);

                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);

                gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.CUST_SEND_PWD, new string[] { mobileNumber }, dictSMSValues);
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
            //try
            //{
            //    Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();

            //    dictSMSValues.Add("#--NAME--#", "Customer");
            //    dictSMSValues.Add("#--OTP--#", OTP);

            //    BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);

            //    gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.OTP_FORGET_PASS, new string[] { mobileNumber }, dictSMSValues);
            //}
            //catch (BusinessLogicLayer.MyException myEx)
            //{
            //    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
            //        + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
            //        + "[PaymentProcessController][M:SendOTPToCustomer]" + myEx.EXCEPTION_PATH,
            //        BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            //}
            //catch (Exception ex)
            //{
            //    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
            //        + Environment.NewLine + ex.Message + Environment.NewLine
            //        + "[PaymentProcessController][M:SendOTPToCustomer]",
            //        BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            //}
        }



    }
}
