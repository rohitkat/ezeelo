using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogicLayer
{
    public class SMS : GateWay
    {
        /// <summary>
        /// Initialises a SMS Gateway
        /// </summary>
        /// <param name="server">Server Utility Object</param>
        public SMS(System.Web.HttpServerUtility server) : base(server) { }

        public override bool SendSMS(SMSGateWays smsGateWay, SMSOptions smsOptions,
            SMSTypes smsType, string[] recipients, Dictionary<string, string> smsValues)
        {
            try
            {
                string smsText = this.InsertSMSValues(this.GetSMSText(smsType), smsValues);

                Dictionary<string, string> gateWayDetails = this.GetGateWayDetails(smsGateWay, smsOptions);

                string routerID = this.GetRouterID(smsType);


                return this.SendSMSText(smsOptions, smsType, smsText, recipients, gateWayDetails, routerID);
            }
            catch (MyException myEx)
            {
                throw new MyException("[<N:GateWays><C:SMS><M:SendSMS>]" + myEx.EXCEPTION_PATH, "Can't Send SMS!!" + Environment.NewLine + myEx.EXCEPTION_MSG);
            }
            catch (Exception ex)
            {
                throw new MyException("[<N:GateWays><C:SMS><M:SendSMS>]", "Can't Send SMS!!" + Environment.NewLine + ex.Message);
            }
        }

        private string GetRouterID(SMSTypes smsType)
        {
            try
            {
                ReadConfig readConfig = new ReadConfig(server);

                return MyXML.GetSingleAttribute(readConfig.SMS_XML_PATH, @"/SMS/FORMAT[@SHORT_CODE='" + smsType.ToString().ToUpper().Trim() + "']", "ROUTER_ID");
            }
            catch (MyException myEx)
            {
                throw new MyException("[<N:GateWays><C:SMS><M:GetRouterID>]" + myEx.EXCEPTION_PATH, "Can't Retrive Router ID from SMS XML!!" + Environment.NewLine + myEx.EXCEPTION_MSG);
            }
            catch (Exception ex)
            {
                throw new MyException("[<N:GateWays><C:SMS><M:GetRouterID>]", "Can't Retrive Router ID from SMS XML!!" + Environment.NewLine + ex.Message);
            }
        }

        private bool SendSMSText(SMSOptions smsOptions, SMSTypes smsType, string smsText, string[] recipients, Dictionary<string, string> gateWayDetails, string routerID)
        {
            try
            {
                SMSService smsService = new SMSService(gateWayDetails["URL"], gateWayDetails["USR"], gateWayDetails["PWD"], gateWayDetails["SENDER_ID"]);

                string[] smsMsgs = new string[recipients.Length];

                switch (smsOptions)
                {
                    case SMSOptions.SINGLE:
                    case SMSOptions.MULTIPLE:
                        smsMsgs = smsService.SendSMS(recipients, smsText, routerID.Equals("T") ? RouterId.Transactional : RouterId.Commerical);
                        break;
                    case SMSOptions.UNI_SINGLE:
                    case SMSOptions.UNI_MULTIPLE:
                        smsMsgs = smsService.USendSMS(recipients, smsText, routerID.Equals("T") ? RouterId.Transactional : RouterId.Commerical);
                        break;
                }

                if (smsMsgs != null && smsMsgs.Length > 0)
                    return false;

                return true;

            }
            catch (MyException myEx)
            {
                throw new MyException("[<N:GateWays><C:SMS><M:SendSMSText>]" + myEx.EXCEPTION_PATH, "Can't Access SMS Service!!" + Environment.NewLine + myEx.EXCEPTION_MSG);
            }
            catch (Exception ex)
            {
                throw new MyException("[<N:GateWays><C:SMS><M:SendSMSText>]", "Can't Access SMS Service!!" + Environment.NewLine + ex.Message);
            }

            return false;
        }

        private Dictionary<string, string> GetGateWayDetails(SMSGateWays smsGateWay, SMSOptions smsOptions)
        {
            Dictionary<string, string> gateWayDetails = new Dictionary<string, string>();

            try
            {

                ReadConfig readConfig = new ReadConfig(this.server);

                MyXML.GetAttributes(readConfig.GATEWAYS_XML_PATH, @"/GATEWAYS/SMS/GATEWAY[@NAME='"
                    + smsGateWay.ToString().Trim().ToUpper() + "']", ref gateWayDetails);

                gateWayDetails.Add("URL", MyXML.GetSingleAttribute(readConfig.GATEWAYS_XML_PATH, @"/GATEWAYS/SMS/GATEWAY[@NAME='"
                    + smsGateWay.ToString().Trim().ToUpper() + "']/URL[@TYPE='" + smsOptions.ToString().ToUpper().Trim() + "']", "LINK"));

            }
            catch (MyException myEx)
            {
                throw new MyException("[<N:GateWays><C:SMS><M:GetGateWayDetails>]" + myEx.EXCEPTION_PATH, "Can't Retrive GateWay from GateWays XML!!" + Environment.NewLine + myEx.EXCEPTION_MSG);
            }
            catch (Exception ex)
            {
                throw new MyException("[<N:GateWays><C:SMS><M:GetGateWayDetails>]", "Can't Retrive GateWay from GateWays XML!!" + Environment.NewLine + ex.Message);
            }

            return gateWayDetails;
        }

        private string InsertSMSValues(StringBuilder sbSMS, Dictionary<string, string> smsValues)
        {
            try
            {
                string[] keys = smsValues.Keys.ToArray();

                foreach (string key in keys)
                {
                    sbSMS.Replace(key, smsValues[key]);
                }
            }
            catch (Exception ex)
            {
                //throw new LogFile.MyException("<N:GateWays><C:Email><M:InsertEmailValues>", "Can't Get Email Text from GateWays XML!!" + Environment.NewLine + ex.Message);

            }


            return this.RemoveKeywords(sbSMS.ToString());
        }

        private string RemoveKeywords(string smsText)
        {
            try
            {

                int start = smsText.IndexOf("#--");

                int count = smsText.IndexOf("--#") - start;

                for (int i = 0; i < smsText.Length; i++)
                {
                    smsText.Remove(start, count);

                    start = smsText.IndexOf("#--");
                    count = smsText.IndexOf("--#") - start;
                }

            }
            catch { }

            return smsText;
        }

        private StringBuilder GetSMSText(SMSTypes smsType)
        {
            StringBuilder sbSMSText;
            try
            {
                ReadConfig readConfig = new ReadConfig(this.server);

                sbSMSText = new StringBuilder(MyXML.GetInnerText(readConfig.SMS_XML_PATH, @"/SMS/FORMAT[@SHORT_CODE='" + smsType.ToString().ToUpper().Trim() + "']/SMS_TEXT"));
            }
            catch (MyException myEx)
            {
                throw new MyException("[<N:GateWays><C:SMS><M:GetSMSText>]" + myEx.EXCEPTION_PATH, "Can't Retrive SMS Text from XML file!!" + Environment.NewLine + myEx.EXCEPTION_MSG);
            }
            catch (Exception ex)
            {
                throw new MyException("[<N:GateWays><C:SMS><M:GetSMSText>]", "Can't Retrive SMS Text from XML file!!" + Environment.NewLine + ex.Message);
            }

            return sbSMSText;
        }
        private StringBuilder GetSMSText1(SMSTypes smsType)
        {
            StringBuilder sbSMSText;
            try
            {
                ReadConfig readConfig = new ReadConfig(this.server);

                sbSMSText = new StringBuilder(MyXML.GetInnerText(readConfig.SMS_XML_PATH, @"/SMS/FORMAT[@SHORT_CODE='" + smsType.ToString().ToUpper().Trim() + "']/SMS_TEXT"));
            }
            catch (MyException myEx)
            {
                throw new MyException("[<N:GateWays><C:SMS><M:GetSMSText>]" + myEx.EXCEPTION_PATH, "Can't Retrive SMS Text from XML file!!" + Environment.NewLine + myEx.EXCEPTION_MSG);
            }
            catch (Exception ex)
            {
                throw new MyException("[<N:GateWays><C:SMS><M:GetSMSText>]", "Can't Retrive SMS Text from XML file!!" + Environment.NewLine + ex.Message);
            }

            return sbSMSText;
        }
    }
}
