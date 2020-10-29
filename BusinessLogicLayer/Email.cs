/// --- Used to Send Emails using Email GateWay
/// Author : Prashant
/// Version : 1.0.0     Date : Feb 3, 2015 4:40pm
///
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BusinessLogicLayer
{
    /// <summary>
    /// Class for using Email Gateway
    /// </summary>
    public class Email : GateWay
    {
        /// <summary>
        /// Initialises an Email Gateway
        /// </summary>
        /// <param name="server">Server Utility Object</param>
        public Email(System.Web.HttpServerUtility server) : base(server) { }

        /// <summary>
        /// Method to send Emails to the Recipients
        /// </summary>
        /// <param name="emailGateWay">Specify which Gateway to Use</param>
        /// <param name="emailType">Specify Which Type of Email to Send</param>
        /// <param name="recipients">Email IDs of recipients </param>
        /// <param name="emailValues">Specify Values which will be Filled within blanks of Standard EMial text</param>
        /// <param name="sender">Specifies the sender mail account which is stored within settings file</param>
        /// <param name="isBodyHtml">TRUE : The Body of eMail is in HTML format <para>FALSE : The Body of eMail is a normal text</para></param>
        /// <returns>TRUE : If mail Sent Successfully; FALSE : If failed</returns>
        public override bool SendEmail(EmailGateWays emailGateWay, SenderMail sender,
            EMailTypes emailType, string[] recipients, Dictionary<string, string> emailValues, bool isBodyHtml)
        {
            try
            {
                string subject = string.Empty;

                StringBuilder sbEmailText = this.InsertEmailValues(this.GetEmailText(emailType, out subject), emailValues);
                //String sbEmailText = this.InsertEmailValues(this.GetEmailText(emailType, out subject), emailValues);

                StringBuilder subjectText = this.InsertEmailValues(new StringBuilder(subject), emailValues);
                //String subjectText = this.InsertEmailValues(subject, emailValues);


                subject = subjectText.ToString();

                Dictionary<string, string> senderDetails = this.GetSender(emailGateWay, sender);

                int port = 0;

                int.TryParse(senderDetails["PORT"], out port);

                EmailService emailService = new EmailService(senderDetails["SMTP_HOST"], senderDetails["USR"],
                    senderDetails["PWD"], senderDetails["DISPLAY"], port, senderDetails["SSL"].Equals("TRUE") ? true : false);

                emailService.SendEmail(recipients, subject, sbEmailText.ToString(), isBodyHtml);

                return true;
            }
            catch (MyException myEx)
            {
                throw new MyException("<N:GateWays><C:Email><M:SendEmail>" + myEx.EXCEPTION_PATH, "Can't Send Email!!" + Environment.NewLine + myEx.EXCEPTION_MSG);
            }
            catch (Exception ex)
            {
                throw new MyException("<N:GateWays><C:Email><M:SendEmail>", "Can't Send Email!!" + Environment.NewLine + ex.Message);
            }
        }

        //Added by mohit on 16/12/15--- For Common Email Functionality ----// 
        private StringBuilder GetEmailText(EMailTypes emailType, out string subject)
        {
            try
            {
                subject = "NO SUBJECT!";

                ReadConfig readConfig = new ReadConfig(this.server);
                //ReadConfig readConfig = new ReadConfig("http://192.168.1.102:9292/Communication/EMAIL/");

                string emailsXml = readConfig.EMAILS_XML_PATH;

                Dictionary<string, string> emailSettings = new Dictionary<string, string>();

                MyXML.GetAttributes(readConfig.EMAILS_XML_PATH, @"/EMAILS/FORMAT[@SHORT_CODE='" + emailType.ToString().ToUpper().Trim() + "']", ref emailSettings);

                StringBuilder sbEmailText = this.ReadTextFile(readConfig.EMAILS_FOLDER_PATH + @"/" + emailSettings["TEXT_FILE"]);
                //String sbEmailText = this.ReadTextFile(readConfig.EMAILS_FOLDER_PATH + @"/" + emailSettings["TEXT_FILE"]);
                subject = MyXML.GetSingleAttribute(emailsXml, @"/EMAILS/FORMAT[@SHORT_CODE='" + emailType.ToString().ToUpper().Trim() + "']", "SUBJECT");

                return sbEmailText;
            }
            catch (MyException myEx)
            {
                throw new MyException("<N:GateWays><C:Email><M:GetEmailText>" + myEx.EXCEPTION_PATH, "Can't Get Email Text from GateWays XML!!" + Environment.NewLine + myEx.EXCEPTION_MSG);
            }
            catch (Exception ex)
            {
                throw new MyException("<N:GateWays><C:Email><M:GetEmailText>", "Can't Get Email Text from GateWays XML!!" + Environment.NewLine + ex.Message);
            }
        }

        //Added by mohit on 16/12/15--- For Common Email Functionality ----// 
        private StringBuilder ReadTextFile(string txtFilePath)
        {
            try
            {
                System.Net.HttpWebRequest webReq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(new Uri(txtFilePath));
                StringBuilder Content = new StringBuilder();
                if (webReq.GetResponse().ContentLength > 0)
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(webReq.GetResponse().GetResponseStream());
                    Content.Append(sr.ReadToEnd());

                }
                return Content;
                //return new StringBuilder(System.IO.File.ReadAllText(txtFilePath));                
            }
            catch (MyException myEx)
            {
                throw new MyException("<N:GateWays><C:Email><M:ReadTextFile>" + myEx.EXCEPTION_PATH, "Can't Get Email Text from text File!!" + Environment.NewLine + myEx.EXCEPTION_MSG);
            }
            catch (Exception ex)
            {
                throw new MyException("<N:GateWays><C:Email><M:ReadTextFile>", "Can't Get Email Text from text File!!" + Environment.NewLine + ex.Message);
            }
        }

        //Added by mohit on 16/12/15--- For Common Email Functionality ----// 
        private StringBuilder InsertEmailValues(StringBuilder sbEmailText, Dictionary<string, string> mailValues)
        {
            try
            {
                string[] keys = mailValues.Keys.ToArray();

                foreach (string key in keys)
                {
                    sbEmailText.Replace(key, mailValues[key]);
                }
            }
            catch (MyException myEx)
            {
                //throw new LogFile.MyException("<N:GateWays><C:Email><M:InsertEmailValues>" + myEx.EXCEPTION_PATH, "Can't Get Email Text from GateWays XML!!" + Environment.NewLine + myEx.EXCEPTION_MSG);
            }
            catch (Exception ex)
            {
                //throw new LogFile.MyException("<N:GateWays><C:Email><M:InsertEmailValues>", "Can't Get Email Text from GateWays XML!!" + Environment.NewLine + ex.Message);
            }
            return sbEmailText;
        }


        private Dictionary<string, string> GetSender(EmailGateWays emailGateWay, SenderMail sender)
        {
            try
            {
                Dictionary<string, string> senderValues = new Dictionary<string, string>();

                ReadConfig readConfig = new ReadConfig(this.server);

                MyXML.GetAttributes(readConfig.GATEWAYS_XML_PATH, @"/GATEWAYS/EMAIL/GATEWAY[@NAME='"
                    + emailGateWay.ToString().Trim().ToUpper() + "']/SENDER[@TYPE='" + sender.ToString().Trim().ToUpper() + "']", ref senderValues);

                return senderValues;
            }
            catch (MyException myEx)
            {
                throw new MyException("<N:GateWays><C:Email><M:GetSender>" + myEx.EXCEPTION_PATH, "Can't Get Email Text from GateWays XML!!" + Environment.NewLine + myEx.EXCEPTION_MSG);
            }
            catch (Exception ex)
            {
                throw new MyException("<N:GateWays><C:Email><M:GetSender>", "Can't Get Email Text from GateWays XML!!" + Environment.NewLine + ex.Message);
            }
        }
    }
}
