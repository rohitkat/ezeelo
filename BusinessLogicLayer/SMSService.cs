//Version :1.0.0
//Author :Ajit Jain
//start Date: 3 Jan 2015 4:00PM

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Mail;



namespace BusinessLogicLayer
{
    #region SMS Service Code
    /// <summary>
    /// RouterId Enum 
    /// </summary>
    public enum RouterId
    {
        Commerical = 'C',
        Transactional = 'T'
    }

    public class SMSService
    {
        /// <summary>
        /// <param name="SURL">Service URL</param>
        /// <param name="SUserName">Service User Name</param>
        /// <param name="SPassword">Service Password</param>
        /// <param name="SSenderId">Service SenderId</param>
        /// </summary>
        string SURL, SUserName, SPassword, SSenderId;

        /// <summary>
        /// <param name="ErrorMsg">Multiple Error Return </param>
        /// </summary>
        string[] ErrorMsg;

        /// <summary>
        /// Object For SMS Services 
        /// </summary>
        /// <param name="URL">Pass SMS URL</param>
        /// <param name="UserName">SMS URL UserName</param>
        /// <param name="Password">SMS URL Password</param>
        /// <param name="SenderId">SMS URL SenderId</param>
        public SMSService(string URL, string UserName, string Password, string SenderId)
        {
            SURL = URL;
            SUserName = UserName;
            SPassword = Password;
            SSenderId = SenderId;

        }

        /// <summary>
        /// Text File Write Message
        /// </summary>
        /// <param name="msg">Message</param>
        private void WriteTextFile(string msg)
        {
            System.IO.File.WriteAllText(@"D:\MSG.txt", msg);
        }

        /// <summary>
        /// Send SMS using URL
        /// </summary>
        /// <param name="msg">Message</param>
        private void MsgSendToUrl(string url)
        {
            HttpWebRequest httpreq = (HttpWebRequest)WebRequest.Create(new Uri(url, false));
            HttpWebResponse httpresponce = (HttpWebResponse)(httpreq.GetResponse());
        }

        /// <summary>
        /// Check Mobile Number validation
        /// </summary>
        /// <param name="strPhoneNumber">Mobile Number</param>
        /// <returns>True and False</returns>
        private bool CheckNumber(string strPhoneNumber)
        {
            try
            {
                string MatchPhoneNumberPattern = @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$";
                if (strPhoneNumber != null)
                    return Regex.IsMatch(strPhoneNumber, MatchPhoneNumberPattern);
                else
                    return false;
            }
            catch (MyException ObjectMyException)
            {
                throw new MyException("Send SMS Function" + ObjectMyException.EXCEPTION_PATH, "Send SMS Function" + ObjectMyException.EXCEPTION_MSG);
            }
        }

        /// <summary>
        /// This Function Sending Sms for Single Number
        /// </summary>
        /// <param name="MobileNumber">Valid Mobile Number</param>
        /// <param name="Message">Text Message for 160 char.</param>
        /// <param name="objectRouterId">RouterId.Commerical,RouterId.Transitional</param>
        /// <returns>Error Msg String</returns>
        public string SendSMS(string MobileNumber, string Message, RouterId objectRouterId)
        {
            try
            {
                if (CheckNumber(MobileNumber))
                {
                    string HttpURL = "";// SURL + "?uname=" + SUserName + "&pwd=" + SPassword + "&senderid=" + SSenderId + "&to=" + MobileNumber + "&msg=" + Message + "&route=" + (char)objectRouterId;
                    //HttpURL = SURL + "?uname=" + SUserName + "&pwd=" + SPassword + "&senderid=" + SSenderId + "&to=" + MobileNumber;
                    //HttpURL += "msg=" + Message + "&route=";
                    //;
                    HttpURL += SURL + "?uname=" + SUserName + "&pass=" + SPassword + "&send=" + SSenderId + "&dest=" + MobileNumber + "&msg=" + Message + "&priority=1";
                    this.MsgSendToUrl(HttpURL);
                    return "SMS Sent Successfully";
                }
                else
                    return MobileNumber + " Mobile Number Is Invalid";
            }
            catch (MyException ObjectMyException)
            {
                throw new MyException("Send SMS Function" + ObjectMyException.EXCEPTION_PATH, "Send SMS Function" + ObjectMyException.EXCEPTION_MSG);
            }
        }

        /// <summary>
        /// This Function Sending Sms for Single Number with Unicode 
        /// </summary>
        /// <param name="MobileNumber">Valid Mobile Number</param>
        /// <param name="Message">Text Message for 160 char.</param>
        /// <param name="objectRouterId">RouterId.Commerical,RouterId.Transitional</param>
        /// <returns>Error Msg String</returns>
        public string USendSMS(string MobileNumber, string Message, RouterId objectRouterId)
        {
            try
            {
                if (CheckNumber(MobileNumber))
                {
                    string HttpURL = "";// SURL + "?uname=" + SUserName + "&pwd=" + SPassword + "&senderid=" + SSenderId + "&to=" + MobileNumber + "&msg=" + Message + "&route=" + (char)objectRouterId;
                    
                    //HttpURL = SURL + "?uname=" + SUserName + "&pwd=" + SPassword + "&senderid=" + SSenderId + "&to=" + MobileNumber;
                    //HttpURL += "msg=" + Message + "&route="; 

                    HttpURL += SURL + "?uname=" + SUserName + "&pass=" + SPassword + "&send=" + SSenderId + "&dest=" + MobileNumber + "&msg=" + Message + "&priority=1";
                    this.MsgSendToUrl(HttpURL);
                    return "SMS Send Successfully";
                }
                else
                    return MobileNumber + " Mobile Number Is Invalid";
            }
            catch (MyException ObjectMyException)
            {
                throw new MyException("Send SMS Function" + ObjectMyException.EXCEPTION_PATH, "Send SMS Function" + ObjectMyException.EXCEPTION_MSG);
            }
        }

        /// <summary>
        /// This Function Sending Sms for Multiple Number 
        /// </summary>
        /// <param name="MMobileNumber">Valid Mobile Number Array</param>
        /// <param name="Message">Text Message for 160 char.</param>
        /// <param name="objectRouterId">RouterId.Commerical,RouterId.Transitional</param>
        /// <returns>Error Msg Strings</returns>
        public string[] SendSMS(string[] MMobileNumber, string Message, RouterId objectRouterId)
        {
            try
            {
                string HttpURL = string.Empty;

                ErrorMsg = new string[MMobileNumber.Length];
                int j = 0;
                for (int i = 0; i < MMobileNumber.Length; i++)
                {
                    if (CheckNumber(MMobileNumber[i]))
                    {
                        j++;
                        if (j == 1)
                        {
                            //Added by sonali for set route value on 15_01_2019
                            //string route = string.Empty;
                            //if (objectRouterId == RouterId.Transactional)
                            //{
                            //    route = "TA";
                            //}
                            //else
                            //{
                            //    route = "C";
                            //}

                            //Added by sonali for set route value on 15_01_2019
                            //HttpURL += SURL + "?uname=" + SUserName + "&pwd=" + SPassword + "&senderid=" + SSenderId + "&to=" + MMobileNumber[i];
                            //Yashaswi SMS gatway change 10-7-2018
                            //HttpURL += SURL + "?user=" + SUserName + "&pass=" + SPassword + "&sender=" + SSenderId + "&phone=" + MMobileNumber[i];
                            // Sms gateway api change for Vodaphone url on 15_01_2019
                            //HttpURL += SURL + "?uname=" + SUserName + "&password=" + SPassword + "&sender=" + SSenderId + "&receiver=" + MMobileNumber[i] + "&route=" + route;
                            //Yashaswi 08-08-2019
                            //HttpURL += SURL + "?uname=" + SUserName + "&pwd=" + SPassword + "&senderid=" + SSenderId + "&to=" + MMobileNumber[i];
                            //<!--Prashanth - Changes in SMS Gateway 22-08-2020--> 
                            HttpURL += SURL + "?uname=" + SUserName + "&pass=" + SPassword + "&send=" + SSenderId + "&dest=" + MMobileNumber[i] + "&msg=" + Message + "&priority=1";
                            // = 20200923 &  = india123 & send = SMSINF & dest = MobileNo & msg = TextSMS & priority
                        }
                        else
                        {
                            HttpURL += "," + MMobileNumber[i];
                        }
                        ErrorMsg[i] = MMobileNumber[i] + " SMS Send Successfully";
                    }
                    else
                    {
                        ErrorMsg[i] = MMobileNumber[i] + " Mobile Number Is Invalid";
                    }
                }
                // HttpURL += "&msg=" + Message + "&route=" + (char)objectRouterId;
                //Yashaswi SMS gatway change 10-7-2018
                // HttpURL += "&text=" + Message + "&priority=ndnd&stype=normal";
                //changes by sonali for sms gateway api path change on 15_01_2019
                //HttpURL += "&msg=" + Message + "&route=T";
                //<!--Prashanth - Changes in SMS Gateway 22-08-2020--> 
                //HttpURL += "&msg=" + Message;
                this.MsgSendToUrl(HttpURL);
                return ErrorMsg;
            }
            catch (MyException ObjectMyException)
            {
                throw new MyException("Send SMS Function" + ObjectMyException.EXCEPTION_PATH, "Send SMS Function" + ObjectMyException.EXCEPTION_MSG);
            }
        }

        /// <summary>
        /// This Function Sending Sms for Multiple Number with Unicode
        /// </summary>
        /// <param name="MMobileNumber">Valid Mobile Number Array</param>
        /// <param name="Message">Text Message for 160 char.</param>
        /// <param name="objectRouterId">RouterId.Commerical,RouterId.Transitional</param>
        /// <returns>Error Msg Strings</returns>
        public string[] USendSMS(string[] MMobileNumber, string Message, RouterId objectRouterId)
        {
            string HttpURL = string.Empty;

            try
            {
                ErrorMsg = new string[MMobileNumber.Length];
                int j = 0;
                for (int i = 0; i < MMobileNumber.Length; i++)
                {
                    if (CheckNumber(MMobileNumber[i].ToString()))
                    {
                        j++;
                        if (j == 1)
                        {
                            //HttpURL += SURL + "?uname=" + SUserName + "&pwd=" + SPassword + "&senderid=" + SSenderId + "&to=" + MMobileNumber[i];
                            //Yashaswi SMS gatway change 10-7-2018
                            //HttpURL += SURL + "?uname=" + SUserName + "&pwd=" + SPassword + "&senderid=" + SSenderId + "&to=" + MMobileNumber[i];
                            HttpURL += SURL + "?uname=" + SUserName + "&pass=" + SPassword + "&send=" + SSenderId + "&dest=" + MMobileNumber[i] + "&msg=" + Message + "&priority=1";
                        }
                        else
                        {
                            HttpURL += "," + MMobileNumber[i];
                        }
                        ErrorMsg[i] = MMobileNumber[i] + "SMS Send Successfully";
                    }
                    else
                    {
                        ErrorMsg[i] = MMobileNumber[i] + "Mobile Number Is Invalid";
                    }
                }
                //HttpURL += "&msg=" + Message + "&route=" + (char)objectRouterId;
                //Yashaswi SMS gatway change 10-7-2018
                //HttpURL += "&text=" + Message + "&priority=ndnd&stype=normal";
               // HttpURL += "&apid=60059&" + "text=" + Message + "&dcs=0";
                this.MsgSendToUrl(HttpURL);

                return ErrorMsg;
            }
            catch (MyException ObjectMyException)
            {
                throw new MyException("Send SMS Function" + ObjectMyException.EXCEPTION_PATH, "Send SMS Function" + ObjectMyException.EXCEPTION_MSG);
            }
        }

    }
    #endregion

}
