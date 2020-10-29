using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Collections;
using DeliveryPartner.Models.ViewModel;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using DeliveryPartner.Models;


namespace DeliveryPartner.Common
{
    public static class Common
    {

        private static EzeeloDBContext db = new EzeeloDBContext();

        private static string fConnectionString = WebConfigurationManager.ConnectionStrings["EzeeloDBContext"].ToString();

        public static void InsertUpdateDeliveryPincodeByState(long pDeliveryPartnerID, long pCreateUserID, ArrayList pAllUncheckedStates, ArrayList pAllCheckedStates)
        {
            long lDeliveryPartnerID = pDeliveryPartnerID;
            long lCreateUserID = pCreateUserID;
            ArrayList lAllUncheckedStates = pAllUncheckedStates;
            ArrayList lAllCheckedStates = pAllCheckedStates;

            DataTable lDataTableUnchecked = new DataTable();
            lDataTableUnchecked.Columns.Add("ID");
            foreach (int i in lAllUncheckedStates)
            {
                DataRow dr = lDataTableUnchecked.NewRow();
                dr[0] = i;
                lDataTableUnchecked.Rows.Add(dr);
            }

            DataTable lDataTableChecked = new DataTable();
            lDataTableChecked.Columns.Add("ID");
            foreach (int i in lAllCheckedStates)
            {
                DataRow dr = lDataTableChecked.NewRow();
                dr[0] = i;
                lDataTableChecked.Rows.Add(dr);
            }
            int lCountChecked = lDataTableChecked.Rows.Count;
            int lCountUnChecked = lDataTableUnchecked.Rows.Count;
            try
            {
                using (SqlConnection conn = new SqlConnection(fConnectionString))
                {
                    //SqlCommand sqlComm = new SqlCommand("InsertUpdateDeliveryPincodeByState", conn);
                    SqlCommand sqlComm = new SqlCommand("InsertUpdateDeliveryPincodeByState", conn);
                    sqlComm.CommandType = CommandType.StoredProcedure;
                    sqlComm.Parameters.AddWithValue("@DeliveryPartnerID", SqlDbType.BigInt).Value = lDeliveryPartnerID;
                    sqlComm.Parameters.AddWithValue("@CreateByUserID", SqlDbType.BigInt).Value = lCreateUserID;
                    sqlComm.Parameters.AddWithValue("@CreateDate", SqlDbType.DateTime2).Value = DateTime.Now;
                    sqlComm.Parameters.AddWithValue("@GetAllNotCheckedStates", SqlDbType.Structured).Value = lDataTableUnchecked;
                    sqlComm.Parameters.AddWithValue("@GetAllCheckedState", SqlDbType.Structured).Value = lDataTableChecked;

                    //SqlParameter lSqlParameterStatus = new SqlParameter("@pStatus", SqlDbType.Bit);
                    //lSqlParameterStatus.Direction = ParameterDirection.Output;
                    //sqlComm.Parameters.Add(lSqlParameterStatus);

                    //SqlParameter lSqlParameterServerMsg = new SqlParameter("@pServerMsg", SqlDbType.VarChar, 1000);
                    //lSqlParameterServerMsg.Direction = ParameterDirection.Output;
                    //sqlComm.Parameters.Add(lSqlParameterServerMsg);

                    conn.Open();
                    sqlComm.ExecuteNonQuery();
                    //Boolean lStatus = Convert.ToBoolean(sqlComm.Parameters["@pStatus"].Value);
                    //pServerMsg = sqlComm.Parameters["@pServerMsg"].Value.ToString();
                    conn.Close();
                    //if (lStatus)
                    //{
                    //    return true;
                    //}
                }
                //return false;
            }
            catch (Exception ex)
            {
                //pServerMsg += "\nError : " + (int)IsoftConstant.IS_ERROR_TYPE.EXCEPTION + " : " + ex.Message;
                //return false;
            }
        }

        public static void InsertUpdateDeliveryPincodeByDistrict(long pDeliveryPartnerID, long pCreateUserID, ArrayList pAllUncheckedDistrict, ArrayList pAllCheckedDistrict)
        {
            long lDeliveryPartnerID = pDeliveryPartnerID;
            long lCreateUserID = pCreateUserID;
            ArrayList lAllUncheckedDistrict = pAllUncheckedDistrict;
            ArrayList lAllCheckedDistrict = pAllCheckedDistrict;

            DataTable lDataTableUnchecked = new DataTable();
            lDataTableUnchecked.Columns.Add("ID");
            foreach (int i in lAllUncheckedDistrict)
            {
                DataRow dr = lDataTableUnchecked.NewRow();
                dr[0] = i;
                lDataTableUnchecked.Rows.Add(dr);
            }

            DataTable lDataTableChecked = new DataTable();
            lDataTableChecked.Columns.Add("ID");
            foreach (int i in lAllCheckedDistrict)
            {
                DataRow dr = lDataTableChecked.NewRow();
                dr[0] = i;
                lDataTableChecked.Rows.Add(dr);
            }
            int lCountChecked = lDataTableChecked.Rows.Count;
            int lCountUnChecked = lDataTableUnchecked.Rows.Count;
            try
            {
                using (SqlConnection conn = new SqlConnection(fConnectionString))
                {
                    SqlCommand sqlComm = new SqlCommand("InsertUpdateDeliveryPincodeByDistrict", conn);
                    sqlComm.CommandType = CommandType.StoredProcedure;
                    sqlComm.Parameters.AddWithValue("@DeliveryPartnerID", SqlDbType.BigInt).Value = lDeliveryPartnerID;
                    sqlComm.Parameters.AddWithValue("@CreateByUserID", SqlDbType.BigInt).Value = lCreateUserID;
                    sqlComm.Parameters.AddWithValue("@CreateDate", SqlDbType.DateTime2).Value = DateTime.Now;
                    sqlComm.Parameters.AddWithValue("@GetAllNotCheckedDistrict", SqlDbType.Structured).Value = lDataTableUnchecked;
                    sqlComm.Parameters.AddWithValue("@GetAllCheckedDistrict", SqlDbType.Structured).Value = lDataTableChecked;
                    conn.Open();
                    sqlComm.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static void InsertUpdateDeliveryPincodeByCity(long pDeliveryPartnerID, long pCreateUserID, ArrayList pAllUncheckedCity, ArrayList pAllCheckedCity)
        {
            long lDeliveryPartnerID = pDeliveryPartnerID;
            long lCreateUserID = pCreateUserID;
            ArrayList lAllUncheckedCity = pAllUncheckedCity;
            ArrayList lAllCheckedCity = pAllCheckedCity;

            DataTable lDataTableUnchecked = new DataTable();
            lDataTableUnchecked.Columns.Add("ID");
            foreach (int i in lAllUncheckedCity)
            {
                DataRow dr = lDataTableUnchecked.NewRow();
                dr[0] = i;
                lDataTableUnchecked.Rows.Add(dr);
            }

            DataTable lDataTableChecked = new DataTable();
            lDataTableChecked.Columns.Add("ID");
            foreach (int i in lAllCheckedCity)
            {
                DataRow dr = lDataTableChecked.NewRow();
                dr[0] = i;
                lDataTableChecked.Rows.Add(dr);
            }
            int lCountChecked = lDataTableChecked.Rows.Count;
            int lCountUnChecked = lDataTableUnchecked.Rows.Count;
            try
            {
                using (SqlConnection conn = new SqlConnection(fConnectionString))
                {
                    SqlCommand sqlComm = new SqlCommand("InsertUpdateDeliveryPincodeByCity", conn);
                    sqlComm.CommandType = CommandType.StoredProcedure;
                    sqlComm.Parameters.AddWithValue("@DeliveryPartnerID", SqlDbType.BigInt).Value = lDeliveryPartnerID;
                    sqlComm.Parameters.AddWithValue("@CreateByUserID", SqlDbType.BigInt).Value = lCreateUserID;
                    sqlComm.Parameters.AddWithValue("@CreateDate", SqlDbType.DateTime2).Value = DateTime.Now;
                    sqlComm.Parameters.AddWithValue("@GetAllNotCheckedCity", SqlDbType.Structured).Value = lDataTableUnchecked;
                    sqlComm.Parameters.AddWithValue("@GetAllCheckedCity", SqlDbType.Structured).Value = lDataTableChecked;
                    conn.Open();
                    sqlComm.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static void InsertUpdateDeliveryPincodeByPincode(long pDeliveryPartnerID, long pCreateUserID, ArrayList pAllUncheckedPincode, ArrayList pAllCheckedPincode)
        {
            long lDeliveryPartnerID = pDeliveryPartnerID;
            long lCreateUserID = pCreateUserID;
            ArrayList lAllUncheckedPincode = pAllUncheckedPincode;
            ArrayList lAllCheckedPincode = pAllCheckedPincode;

            DataTable lDataTableUnchecked = new DataTable();
            lDataTableUnchecked.Columns.Add("ID");
            foreach (int i in lAllUncheckedPincode)
            {
                DataRow dr = lDataTableUnchecked.NewRow();
                dr[0] = i;
                lDataTableUnchecked.Rows.Add(dr);
            }

            DataTable lDataTableChecked = new DataTable();
            lDataTableChecked.Columns.Add("ID");
            foreach (int i in lAllCheckedPincode)
            {
                DataRow dr = lDataTableChecked.NewRow();
                dr[0] = i;
                lDataTableChecked.Rows.Add(dr);
            }
            int lCountChecked = lDataTableChecked.Rows.Count;
            int lCountUnChecked = lDataTableUnchecked.Rows.Count;
            try
            {
                using (SqlConnection conn = new SqlConnection(fConnectionString))
                {
                    SqlCommand sqlComm = new SqlCommand("InsertUpdateDeliveryPincodeByPincode", conn);
                    sqlComm.CommandType = CommandType.StoredProcedure;
                    sqlComm.Parameters.AddWithValue("@DeliveryPartnerID", SqlDbType.BigInt).Value = lDeliveryPartnerID;
                    sqlComm.Parameters.AddWithValue("@CreateByUserID", SqlDbType.BigInt).Value = lCreateUserID;
                    sqlComm.Parameters.AddWithValue("@CreateDate", SqlDbType.DateTime2).Value = DateTime.Now;
                    sqlComm.Parameters.AddWithValue("@GetAllNotCheckedPincode", SqlDbType.Structured).Value = lDataTableUnchecked;
                    sqlComm.Parameters.AddWithValue("@GetAllCheckedPincode", SqlDbType.Structured).Value = lDataTableChecked;
                    conn.Open();
                    sqlComm.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static Boolean GetAllLoginDetailFromSession(ref DeliveryPartnerSessionViewModel deliveryPartnerSessionViewModel)
        {
            long lUserLoginId = deliveryPartnerSessionViewModel.UserLoginID;
            ModelLayer.Models.PersonalDetail lPersonalDetail = db.PersonalDetails.SingleOrDefault(x => x.UserLoginID == lUserLoginId);
            if (lPersonalDetail == null)
            {
                return false;
            }
            deliveryPartnerSessionViewModel.PersonalDetailID = lPersonalDetail.ID;

            ModelLayer.Models.BusinessDetail lBusinessDetail = db.BusinessDetails.SingleOrDefault(x => x.UserLoginID == lUserLoginId && x.BusinessTypeID == (int)Constant.BUSINESS_TYPE.DELIVERY_PARTNER);
            if (lBusinessDetail == null)
            {
                return false;
            }
            deliveryPartnerSessionViewModel.BusinessTypeId = lBusinessDetail.BusinessTypeID;
            deliveryPartnerSessionViewModel.BusinessDetailID = lBusinessDetail.ID;

            ModelLayer.Models.DeliveryPartner lDeliveryPartner = db.DeliveryPartners.SingleOrDefault(x => x.BusinessDetailID == lBusinessDetail.ID && x.IsActive == true);
            if (lDeliveryPartner == null)
            {
                return false;
            }
            deliveryPartnerSessionViewModel.DeliveryPartnerID = lDeliveryPartner.ID;

            ModelLayer.Models.OwnerBank lOwnerBank = db.OwnerBanks.SingleOrDefault(x => x.OwnerID == lDeliveryPartner.ID && x.IsActive == true && x.BusinessTypeID == (int)Constant.BUSINESS_TYPE.DELIVERY_PARTNER);
            if (lOwnerBank != null)
            {
                // return false;
                deliveryPartnerSessionViewModel.OwnerBankID = lOwnerBank.ID;
            }


            return true;
        }

        public static string GetVarients(long pShopStockID)
        {
            StringBuilder Varients = new StringBuilder();
            try
            {
                ProductVarient PV = db.ShopStocks.Where(x => x.ID == pShopStockID).Select(x => x.ProductVarient).FirstOrDefault();
                if (!PV.Color.Name.ToUpper().Contains("N/A"))
                {
                    Varients.Append("Color : " + PV.Color.Name + ", ");
                }
                if (!PV.Size.Name.ToUpper().Contains("N/A"))
                {
                    Varients.Append("Size : " + PV.Size.Name + ", ");
                }
                if (!PV.Dimension.Name.ToUpper().Contains("N/A"))
                {
                    Varients.Append("Dimension : " + PV.Dimension.Name + ", ");
                }
                if (!PV.Material.Name.ToUpper().Contains("N/A"))
                {
                    Varients.Append("Material : " + PV.Material.Name + ", ");
                }
                if (Varients.Length > 0)
                {
                    Varients.Remove(Varients.Length - 2, 2);
                }
                return Varients.ToString().Trim();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[Common][GetVarients]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.DeliveryPartner, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[Common][GetVarients]",
                    BusinessLogicLayer.ErrorLog.Module.DeliveryPartner, System.Web.HttpContext.Current.Server);
            }
            return string.Empty;
        }
    }
}