using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class Feedback_From_Mail
    {

        /// <summary>
        /// Select All the Reccord From Data to Send Maiul
        /// 
        /// Not In Use
        /// </summary>
        /// <param name="Connection">Datbase Connection</param>
        /// <returns></returns>
        public DataTable Select_For_Feed_BackMail(string Connection)
        {
            DataTable dt = new DataTable();
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(Connection);
            dt = dbOpr.GetRecords("Select_For_Feed_BackMail");
            return dt;
        }
        /// <summary>
        /// To Send Mail 
        /// 
        /// Not In Use
        /// </summary>

        public void SendMailForFeedBack()
        {
            try
            {
                // Sending email to the customer
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary.Add("<!--ACCOUNT_URL-->", "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "login/login");
                dictionary.Add("<!--ORDERS_URL-->", "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "CustomerOrder/MyOrders");
                dictionary.Add("<!--NAME-->", "Pradnyakar");
                dictionary.Add("<!--PWD_URL-->", "eZeelo/UserLogin/Edit?pUserLoginID=1");

                gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.PWD_RCVRY, new string[] { "pradnyakar786@gmail.com" }, dictionary, true);
      
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString() + Environment.NewLine + "Can't send SMS..! " + myEx.EXCEPTION_MSG + Environment.NewLine + myEx.EXCEPTION_PATH, BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                // throw new Exception("Unable to Send Email");
            }
        }

        /// <summary>
        /// To Update Record From Feedback From User
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="paramValues"></param>
        public static void Insert_Update__FeedBack_From_Mail(string Connection, List<object> paramValues)
        {
            
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.SetData(Connection);
            int i = dbOpr.SetRecords("Insert_Update__FeedBack_From_Mail", paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS.UPDATE);
        }


        public static DataTable AlreadyGivenFeedBack(long OrderID, string Connection)
        {
            DataTable dt = new DataTable();
            List<object> paramValues = new List<object>();
            paramValues.Add(OrderID);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(Connection);
            dt = dbOpr.GetRecords("AlreadyFeedBackGiven_For_Feed_BackMail", paramValues);
            return dt;
        }

        public static DataTable OrderDetail(long OrderID, string Connection)
        {
            DataTable dt = new DataTable();
            List<object> paramValues = new List<object>();
            paramValues.Add(OrderID);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(Connection);
            dt = dbOpr.GetRecords("Select_OrderDetail", paramValues);
            return dt;
        }
    }
}
