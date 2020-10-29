using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    /// <summary>
    /// /Bussiness Logic for Market Partner Module
    /// </summary>
    public class MarketPartner
    {
        EzeeloDBContext db = new EzeeloDBContext();
        /// <summary>
        /// SEnd SMS when Transaction request send through QR code scanning
        /// </summary>
        /// <param name="UserLoginId"></param>
        /// <param name="TransID"></param>
        /// <param name="BillAmt"></param>
        /// <param name="MerchantId"></param>
        public void SendSMS_TransRequest(long UserLoginId,string TransID,string BillAmt,long MerchantId)
        {
            Merchant merchant = db.Merchants.FirstOrDefault(p => p.Id == MerchantId);
            UserLogin userLogin = db.UserLogins.FirstOrDefault(p => p.ID == UserLoginId);
            PersonalDetail personalDetail = db.PersonalDetails.FirstOrDefault(p => p.UserLoginID == UserLoginId);
            Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();
            dictSMSValues.Add("#--NAME--#", personalDetail.FirstName);
            dictSMSValues.Add("#--CODE--#", TransID);
            dictSMSValues.Add("#--RS--#", BillAmt);
            dictSMSValues.Add("#--SHOP--#", merchant.FranchiseName);
            BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
            //SMS to User
            gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.MERCHANT_TRANSACTION_REQUEST_USER, new string[]{ userLogin.Mobile }, dictSMSValues);
            //SMS to Merchant
            gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.MERCHANT_TRANSACTION_REQUEST_MERCHANT, new string[] { merchant.ContactNumber }, dictSMSValues);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetNextTransactionCode()
        {
            string newOrderCode = string.Empty;
            int lYear = 0;
            int lMonth = 0;
            int lDay = 0;
            int.TryParse(DateTime.UtcNow.AddHours(5.5).Year.ToString(), out lYear);
            int.TryParse(DateTime.UtcNow.AddHours(5.5).Month.ToString(), out lMonth);
            int.TryParse(DateTime.UtcNow.AddHours(5.5).Day.ToString(), out lDay);
            string lOrderPrefix = "EZMR" + lYear.ToString().Substring(2, 2) + lMonth.ToString("00") + lDay.ToString("00");

            try
            {
                int code = GetNextCODE();
                if (code > 0)
                {
                    newOrderCode = lOrderPrefix + code.ToString("00000");
                    return newOrderCode;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return null;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetNextTransactionCodeThroughQRCode()
        {
            string newOrderCode = string.Empty;
            int lYear = 0;
            int lMonth = 0;
            int lDay = 0;
            int.TryParse(DateTime.UtcNow.AddHours(5.5).Year.ToString(), out lYear);
            int.TryParse(DateTime.UtcNow.AddHours(5.5).Month.ToString(), out lMonth);
            int.TryParse(DateTime.UtcNow.AddHours(5.5).Day.ToString(), out lDay);
            string lOrderPrefix = "EZMQ" + lYear.ToString().Substring(2, 2) + lMonth.ToString("00") + lDay.ToString("00");

            try
            {
                int code = GetNextCODE();
                if (code > 0)
                {
                    newOrderCode = lOrderPrefix + code.ToString("00000");
                    return newOrderCode;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return null;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetNextCODE()
        {
            int code = -1;

            try
            {
                DataTable lDataTableCustomerOrder = new DataTable();
                SqlConnection con = new SqlConnection(db.Database.Connection.ConnectionString);
                SqlCommand sqlComm = new SqlCommand("SelectNextEzMerchantTransCode", con);
                sqlComm.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    code = Convert.ToInt32(dt.Rows[0][0]);
                }
                con.Close();
                return code;
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[MarketPlace- ->CommomnController-> GetNextCODE]", "Problem in getting CODE" + Environment.NewLine + ex.Message);
            }
        }
    }
}
