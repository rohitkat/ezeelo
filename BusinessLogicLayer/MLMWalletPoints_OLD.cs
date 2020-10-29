using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using IronPython.Hosting;
using System.Net;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;


namespace BusinessLogicLayer
{

    //public class MLMRequest
    //{
    //    public string userid {get;set;}
    //    public string orderid{get;set;}
    //    public string points_tr{get;set;}
    //    public string order_amt {get;set;}
    //    public string date {get;set;}
    //    public string wallet_pt {get;set;}
    //    public string status {get;set;}
    //}
    public class MLMWalletPoints
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        [HttpPost]
        public string MLMWalletPostRequest(int Status, long userLoginId, long customerOrderID, decimal businessPointsTotal, decimal payableAmount, DateTime Date, decimal mLMAmountUsed)
        {
            int status = Status;

            if (Status == 7 || Status == 9)
            {
                mLMAmountUsed = 0;
            }

            string responseString = "1";
            string data = "";
            try
            {
                WebRequest tRequest = WebRequest.Create("http://leaders.ezeelo.com/ewallet/");

                data = "userid=" + userLoginId + "&orderid=" + customerOrderID + "&points_tr=" + businessPointsTotal + "&order_amt=" + payableAmount + "&date=" + Convert.ToString(Date.ToString("dd/MM/yyyy")) + "&wallet_amt=" + mLMAmountUsed + "&status=" + status;

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                   + Environment.NewLine + "Parameter : " + data + Environment.NewLine
                   + "[MLMWalletPoints][M:MLMWalletPostRequest]",
                   BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);


                tRequest.Method = "POST";
                tRequest.ContentType = "application/x-www-form-urlencoded";

                var postData = Encoding.ASCII.GetBytes(data);
                tRequest.ContentLength = data.Length;

                using (var stream = tRequest.GetRequestStream())
                {
                    stream.Write(postData, 0, postData.Length);
                }


                var response = (HttpWebResponse)tRequest.GetResponse();

                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                return responseString;
            }
            catch (WebException ex)
            {
                string message = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                //HttpStatusCode statusCode = ((HttpWebResponse)ex.Response).StatusCode;
                var StatusNumber = (int)((HttpWebResponse)ex.Response).StatusCode; //(int)ex.Response.StatusCode;
                if (StatusNumber == 500)
                    throw new Exception(ex.InnerException + " AND " + ex.Message  +" And Parameter " + data);

                //BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                //    + Environment.NewLine + ex.Message + Environment.NewLine
                //    + "[MLMWalletPoints][M:MLMWalletPostRequest]",
                //    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
                return responseString;
            }
        }

        public string LeadersSingUp(long UserLoginID, string Password, string Email, string RefCode)
        {
            string responseString = "0";
            try
            {
                string data = "UserLoginID=" + UserLoginID + "&Password=" + Password + "&Email=" + Email + "&RefCode=" + RefCode;

                WebRequest tRequest = WebRequest.Create("http://leaders.ezeelo.com/signup/");
                //WebRequest tRequest = WebRequest.Create("http://202.189.234.156:8080/signup/");
                tRequest.Method = "POST";
                tRequest.ContentType = "application/x-www-form-urlencoded";

                var postData = Encoding.ASCII.GetBytes(data);
                tRequest.ContentLength = data.Length;

                using (var stream = tRequest.GetRequestStream())
                {
                    stream.Write(postData, 0, postData.Length);
                }
                var response = (HttpWebResponse)tRequest.GetResponse();

                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                return responseString;
            }
            catch (WebException ex)
            {
                string message = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                HttpStatusCode statusCode = ((HttpWebResponse)ex.Response).StatusCode;
                var StatusNumber = (int)((HttpWebResponse)ex.Response).StatusCode;

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + message + Environment.NewLine
                    + StatusNumber + Environment.NewLine
                    + "[MLMWalletPoints][M:LeadersSingUp]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
                return responseString;
            }
        }
    }
}