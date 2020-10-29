//Version :1.0.1
//Author :Ajit Jain
//start Date: 12 Jan 2015 3:00PM
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Mail;



namespace BusinessLogicLayer
{
    #region Email Service Code

    public class EmailService
    {
        string exceptionPath = "[SERVICES]<N:Services><C:EmailService>";

        string SSmtpServerName, SSenderMailId, SPassword, SDisplayName;
        int SPort;
        bool SSL = false;
        string[] ErrorMsg;
        MailMessage mail = new MailMessage();


        /// <summary>
        /// Check Email Id validation
        /// </summary>
        /// <param name="strEmailId">Email Id</param>
        /// <returns>True and False</returns>
        private bool CheckEmail(string strEmailId)
        {
            string MatchEmailIdPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
                + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
               + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
            //string MatchEmailIdPattern =@"/^([\w-\.]+@@([\w-]+\.)+[\w-]{2,4})?$/";
            if (strEmailId != null)
            {
                var ischeck = Regex.IsMatch(strEmailId, MatchEmailIdPattern);
                return ischeck;
            }
            //return Regex.IsMatch(strEmailId, MatchEmailIdPattern);
            else
                return false;
        }

        /// <summary>
        /// Mail Send By Smtp Server
        /// </summary>
        private void MailSend()
        {
            try
            {
                SmtpClient SmtpServer = new SmtpClient(SSmtpServerName);
                mail.From = new MailAddress(SSenderMailId, SDisplayName);
                SmtpServer.Port = SPort;
                SmtpServer.Credentials = new System.Net.NetworkCredential(SSenderMailId, SPassword);
                SmtpServer.EnableSsl = this.SSL;
                //SmtpServer.Timeout = 5;
                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                throw new MyException(this.exceptionPath + "<M:MailSend>", "Can't Send Email : " + ex.Message);
            }
        }

        /// <summary>
        /// Object For Email services
        /// </summary>
        /// <param name="SmtpServerName">Smtp Mail Server Name</param>
        /// <param name="SenderMailId">Sender Mail Id</param>
        /// <param name="DisplayName">Display Name</param>
        /// <param name="Password">Password</param>
        /// <param name="Port">Smtp Server Port</param>
        /// <param name="SSL">Specifies whether SMTP Server Uses SSL</param>
        public EmailService(string SmtpServerName, string SenderMailId, string Password, string DisplayName, int Port, bool SSL)
        {
            SSmtpServerName = SmtpServerName;
            SSenderMailId = SenderMailId;
            SDisplayName = DisplayName;
            SPassword = Password;
            SPort = Port;
            this.SSL = SSL;
        }

        /// <summary>
        /// Mail Send Function with Multiple TO.
        /// </summary>
        /// <param name="TO">Reciver mail Id's</param>
        /// <param name="Subject">Mail Subject</param>
        /// <param name="Body">Mail Body</param>
        /// <param name="IsBodyHtml">HTML Body</param>
        /// <returns>Error Msg Strings</returns>
        public string[] SendEmail(string[] TO, string Subject, string Body, bool IsBodyHtml)
        {
            try
            {
                ErrorMsg = new string[TO.Count()];
                for (int i = 0; i < TO.Count(); i++)
                {
                    if (TO[i] != null)
                    {
                        if (CheckEmail(TO[i].ToString().ToLower()))
                        {
                            mail.To.Add(TO[i].ToString()); ErrorMsg[i] = TO[i] + "Email Send";
                        }
                        else
                        {
                            ErrorMsg[i] = TO[i] + "Invalid Email ID";
                        }
                    }
                }
                mail.Subject = Subject;
                mail.Body = Body;
                mail.BodyEncoding = Encoding.UTF8;
                mail.SubjectEncoding = Encoding.Default;
                mail.IsBodyHtml = IsBodyHtml;
                MailSend();
                return ErrorMsg;
            }
            catch (MyException ObjectMyException)
            {
                throw new MyException(this.exceptionPath + "<M:SendEmail>" + ObjectMyException.EXCEPTION_PATH,
                    "There's Something Wrong with Sending Email!" + Environment.NewLine + ObjectMyException.EXCEPTION_MSG);
            }
            catch (Exception ex)
            {
                throw new MyException(this.exceptionPath + "<M:SendEmail>", "Can't Send Email : " + ex.Message);
            }
        }

        /// <summary>
        /// Mail Send Function with multiple TO,CC.
        /// </summary>
        /// <param name="TO">Reciver mail Id's</param>
        /// <param name="CC">CC Reciver mail Id's</param>
        /// <param name="Subject">Mail Subject</param>
        /// <param name="Body">Mail Body</param>
        /// <param name="IsBodyHtml">HTML Body</param>
        /// <returns>Error Msg Strings</returns>
        public string[] SendEmail(string[] TO, string[] CC, string Subject, string Body, bool IsBodyHtml)
        {
            try
            {
                ErrorMsg = new string[TO.Count() + CC.Count()];
                for (int i = 0; i < TO.Count(); i++) { if (CheckEmail(TO[i].ToString())) { mail.To.Add(TO[i].ToString()); ErrorMsg[i] = TO[i] + "Email Send"; } else ErrorMsg[i] = TO[i] + "Invalid Email ID"; }
                for (int i = 0; i < CC.Count(); i++) { if (CheckEmail(CC[i].ToString())) { mail.CC.Add(CC[i].ToString()); ErrorMsg[i] = CC[i] + "Email Send"; } else ErrorMsg[i] = CC[i] + "Invalid Email ID"; }
                mail.Subject = Subject;
                mail.Body = Body;
                mail.BodyEncoding = Encoding.UTF8;
                mail.SubjectEncoding = Encoding.Default;
                mail.IsBodyHtml = IsBodyHtml;
                MailSend();
                return ErrorMsg;
            }
            catch (MyException ObjectMyException)
            {
                throw new MyException(this.exceptionPath + "<M:SendEmail>" + ObjectMyException.EXCEPTION_PATH,
                    "There's Something Wrong with Sending Email!" + Environment.NewLine + ObjectMyException.EXCEPTION_MSG);
            }
            catch (Exception ex)
            {
                throw new MyException(this.exceptionPath + "<M:SendEmail>", "Can't Send Email : " + ex.Message);
            }
        }

        /// <summary>
        /// Mail Send Function with Multiple TO,CC,BCC
        /// </summary>
        /// <param name="TO">Reciver mail Id's</param>
        /// <param name="CC">CC Reciver mail Id's</param>
        /// <param name="BCC">BCC Reciver mail Id's</param>
        /// <param name="Subject">Mail Subject</param>
        /// <param name="Body">Mail Body</param>
        /// <param name="IsBodyHtml">HTML Body</param>
        /// <returns>Error Msg Strings</returns>
        public string[] SendEmail(string[] TO, string[] CC, string[] BCC, string Subject, string Body, bool IsBodyHtml)
        {
            try
            {
                ErrorMsg = new string[TO.Count() + CC.Count() + BCC.Count()];
                for (int i = 0; i < TO.Count(); i++) { if (CheckEmail(TO[i].ToString())) { mail.To.Add(TO[i].ToString()); ErrorMsg[i] = TO[i] + "Email Send"; } else ErrorMsg[i] = TO[i] + "Invalid Email ID"; }
                for (int i = 0; i < CC.Count(); i++) { if (CheckEmail(CC[i].ToString())) { mail.CC.Add(CC[i].ToString()); ErrorMsg[i] = CC[i] + "Email Send"; } else ErrorMsg[i] = CC[i] + "Invalid Email ID"; }
                for (int i = 0; i < BCC.Count(); i++) { if (CheckEmail(BCC[i].ToString())) { mail.Bcc.Add(BCC[i].ToString()); ErrorMsg[i] = BCC[i] + "Email Send"; } else ErrorMsg[i] = BCC[i] + "Invalid Email ID"; }
                mail.Subject = Subject;
                mail.Body = Body;
                mail.BodyEncoding = Encoding.UTF8;
                mail.SubjectEncoding = Encoding.Default;
                mail.IsBodyHtml = IsBodyHtml;
                MailSend();
                return ErrorMsg;
            }
            catch (MyException ObjectMyException)
            {
                throw new MyException(this.exceptionPath + "<M:SendEmail>" + ObjectMyException.EXCEPTION_PATH,
                    "There's Something Wrong with Sending Email!" + Environment.NewLine + ObjectMyException.EXCEPTION_MSG);
            }
            catch (Exception ex)
            {
                throw new MyException(this.exceptionPath + "<M:SendEmail>", "Can't Send Email : " + ex.Message);
            }
        }

    }
    #endregion
}
