using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BusinessLogicLayer
{
    public class TrackSearchBusiness
    {
        private static EzeeloDBContext db = new EzeeloDBContext();
        public static void InsertSearchDetails(long? UserLoginID, long? CategoryID, long? ShopID, string ProductName, string Lattitude, string Longitude, string DeviceType, string DeviceID, string City, string IMEI_NO,int? FranchiseID=null)////Added int? FranchiseID for Multiple MCO/Old App
        {
            // Result = 0;
            try
            {
                DateTime CreateDate = DateTime.UtcNow;
                string NetworkIP = CommonFunctions.GetClientIP();
                long CreateBy = 1;
                if(ProductName==null)
                {
                    ProductName = "";
                }
                ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                SqlConnection con = new SqlConnection(readCon.DB_CONNECTION);
                con.Open();
                SqlParameter parm = new SqlParameter("@return", SqlDbType.Int);
                SqlCommand sqlComm = new SqlCommand("CP_Insert_TrackSearch", con);
                sqlComm.CommandType = CommandType.StoredProcedure;
                parm.Direction = ParameterDirection.ReturnValue;
                sqlComm.Parameters.AddWithValue("@UserLoginID", UserLoginID);
                sqlComm.Parameters.AddWithValue("@CategoryID", CategoryID);
                sqlComm.Parameters.AddWithValue("@ShopID", ShopID);
                sqlComm.Parameters.AddWithValue("@ProductName", ProductName);
                sqlComm.Parameters.AddWithValue("@Lattitude", Lattitude);
                sqlComm.Parameters.AddWithValue("@Longitude", Longitude);
                sqlComm.Parameters.AddWithValue("@NetworkIP", NetworkIP);
                sqlComm.Parameters.AddWithValue("@DeviceType", DeviceType);
                sqlComm.Parameters.AddWithValue("@DeviceID", DeviceID);
                sqlComm.Parameters.AddWithValue("@CreateDate", CreateDate);
                sqlComm.Parameters.AddWithValue("@CreateBy", CreateBy);
                sqlComm.Parameters.AddWithValue("@City", City);

                sqlComm.Parameters.AddWithValue("@FranchiseID", FranchiseID);//--added by Ashish for multiple franchise in same city--//

                sqlComm.Parameters.AddWithValue("@IMEI_NO", IMEI_NO);
                sqlComm.Parameters.Add(parm);
                sqlComm.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception)
            {
                //  Result = 103; //Exception Found
                throw;
            }
            //return Result;
        }

       
    }
}
