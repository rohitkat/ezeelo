using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer;
using ModelLayer.Models;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BusinessLogicLayer
{
    public class OrderStatusSMSandEMAIL
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        public void GCMMsgTemplate(string pOrderStatus, string pOrderCode,long pCustomerOrderID,DateTime pCreateDate)
        {
            try
            {

                string lprimaryKey = "";
                string lOrderCode = "";
                DataTable ldatatable = new DataTable();
                ldatatable = GetGCMMsgTemplate();
                foreach (DataRow dr in ldatatable.Rows)
                {
                    if (dr["Name"].ToString() == pOrderStatus)
                    {
                        lprimaryKey = dr["PrimaryKey"].ToString();
                        //break;
                    }
                }
                if (lprimaryKey == Convert.ToString("OrderCode"))
                {
                    lOrderCode = pOrderCode;
                }
                //CreateDate = DateTime.Now;
                InsertGCMMsgTemplateTransaction(pOrderStatus, lOrderCode, pCustomerOrderID, pCreateDate);
            }
            catch
            {
                throw;
            }
        }
        public int InsertGCMMsgTemplateTransaction(string pOrderStatus, string pPrimaryKey,long CustomerOrderID, DateTime CreateDate)
        {
                  
            DataTable ldt = ListToDatatable(pOrderStatus, pPrimaryKey);
            try
            {
                ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                string conn = readCon.DB_CONNECTION;
                SqlConnection con = new SqlConnection(conn);
                SqlCommand sqlComm = new SqlCommand("Insert_Update_GCMMsgTemplateTransaction", con);
                sqlComm.CommandType = CommandType.StoredProcedure;
                sqlComm.Parameters.AddWithValue("@CustomerOrderID", CustomerOrderID);
                sqlComm.Parameters.AddWithValue("@CreateDate", CreateDate);
                SqlParameter lsqlparam = sqlComm.Parameters.AddWithValue("@dt", ldt);
                lsqlparam.SqlDbType = SqlDbType.Structured;
                con.Open();
                sqlComm.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                return -1;
            }
            return -1;
        }
        public DataTable GetGCMMsgTemplate()
        {
            DataTable ldt = new DataTable();
            try
            {
                ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                string conn = readCon.DB_CONNECTION;
                SqlConnection con = new SqlConnection(conn);
                SqlCommand sqlComm = new SqlCommand("Get_GCMMsgTemplate", con);
                sqlComm.CommandType = CommandType.StoredProcedure;
                con.Open();
                sqlComm.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                da.Fill(ldt);
                con.Close();
                return ldt;

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private DataTable ListToDatatable(string pOrderStatus, string pPrimaryKey)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Name");
            dt.Columns.Add("PrimaryKey");
            DataRow dr = dt.NewRow();
            dr["Name"] = pOrderStatus;
            dr["PrimaryKey"] = pPrimaryKey;
            dt.Rows.Add(dr);
            return dt;
        }

        public int SendNotification(long pUserLoginID,string pOrderStatus)
        {
            //GcmUser lGcmUser = db.GcmUsers.Find(pUserLoginID);
            List<string> lRegIds=new List<string>();
            List<GcmUser> lGcmUsers = new List<GcmUser>();
            lGcmUsers = (from c in db.GcmUsers
                         where c.UserLoginID==pUserLoginID && c.IsActive==true
                         select c).ToList();
                      
            lRegIds = lGcmUsers.Select(x => x.GcmRegID.ToString()).ToList(); 
           
            if (lRegIds == null)
            {
                return 0;
            }
            string lRegdata = "";
            lRegdata = string.Join("\",\"", lRegIds);
            string Status = "Order";
            string lOrderMsg=string.Empty;
            if(pOrderStatus=="ORD_CONF")
            {
                lOrderMsg = "Order Confirmed Successfully";
            }
            else if (pOrderStatus =="ORD_DIL")
            {
                lOrderMsg="Order Delivered Successfully";
            }
            //lRegdata = string.Join("\",\"", pAPIOrderViewModel.GcmRegID);

            // applicationID means google Api key                                                                                                     
            var applicationID =   "AIzaSyDSymqUriO1nwSuUSgawXBXGaMvgsy26zE";
            // SENDER_ID is nothing but your ProjectID (from API Console- google code)//                                          
            var SENDER_ID = "555896632846";
            WebRequest tRequest;
            tRequest = WebRequest.Create("https://android.googleapis.com/gcm/send");
            tRequest.Method = "post";
            //tRequest.ContentType = " application/x-www-form-urlencoded;charset=UTF-8";
            tRequest.ContentType = "application/json;charset=UTF-8";
            //tRequest.ContentType = "application/json";
            tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
            tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));
            string postData = "{ \"registration_ids\": [ \"" + lRegdata + "\" ], " +
                                  "\"data\": {\"message\":\"" + lOrderMsg + "\"," +
                                   "\"Status\":\"" + Status + "\"}}";
            // "\"data\": {\"message\":\"" + "hiii pihu" + "\"}}";
            //"\"title\":\"" + pGCMNotification.gCMMsgDetailViewModel.Title + "\", " +
            //"\"imageurl\":\"" + pGCMNotification.gCMMsgDetailViewModel.ImgUrl + "\", " +
            //"\"cat_id\":\"" + pGCMNotification.gCMMsgDetailViewModel.LevelToID + "\", " +
            //"\"level\":\"" + pGCMNotification.gCMMsgDetailViewModel.Level + "\", " +
            //"\"cityid\": \"" + pGCMNotification.GcmUsers.FirstOrDefault().City + "\"}}";

            Byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            tRequest.ContentLength = byteArray.Length;
            Stream dataStream = tRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            WebResponse tResponse = tRequest.GetResponse();
            dataStream = tResponse.GetResponseStream();
            StreamReader tReader = new StreamReader(dataStream);
            String sResponseFromServer = tReader.ReadToEnd();   //Get response from GCM server.
            string lServerMsg = sResponseFromServer;  //Assigning GCM response to Label text 
            Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(lServerMsg); //JObject.Parse(lServerMsg);

            tReader.Close();
            dataStream.Close();
            tResponse.Close();

            var data = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(lServerMsg);
            string status = data["success"].Value<string>();
            return Convert.ToInt32(status);

        }
    }
}
