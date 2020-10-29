//-----------------------------------------------------------------------
// <copyright file="CustomerOrder.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Mohit Sinha</author>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models.ViewModel;
using System.Transactions;
using ModelLayer.Models;
using System.Web.ModelBinding;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data;

using System.Web;
using System.Web.Configuration;
using System.Collections;


namespace BusinessLogicLayer
{
    public class SubscriptionCalculator : CustomerManagement
    {
        public SubscriptionCalculator(System.Web.HttpServerUtility server)
            : base(server)
        {
        }
        /// <summary>
        /// Declare the object of DbContextClass to interact with the database 
        /// </summary>
        private static EzeeloDBContext db = new EzeeloDBContext();
        ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
        // private static string fConnectionString = WebConfigurationManager.ConnectionStrings["EzeeloDBContext"].ToString();
        public static int IsUserSubscribed(long UserLoginId, ref int Result)
        {

            Result = 0;
            if (UserLoginId == null)
            {
                return 0;
            }
            else
            {
                ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                SqlConnection con = new SqlConnection(readCon.DB_CONNECTION);
                con.Open();
                SqlParameter parm = new SqlParameter("@return", SqlDbType.Int);
                SqlCommand cmd = new SqlCommand("SubscriptionApplicable", con);
                cmd.CommandType = CommandType.StoredProcedure;
                parm.Direction = ParameterDirection.ReturnValue;
                cmd.Parameters.AddWithValue("@userLoginId", UserLoginId);
                //cmd.Parameters.AddWithValue("@ReturnNumber", parm);
                cmd.Parameters.Add(parm);
                cmd.ExecuteNonQuery();
                Result = Convert.ToInt32(parm.Value);
                con.Close();

            }
            return Result;
        }

        public static List<SubscribedDiscountOnCategoryViewModel> SubscriberDiscountOnCategory(long UserLoginId, List<OrderDetailsCartShopStock> CartShopStockID)
        {
            List<SubscribedDiscountOnCategoryViewModel> lSubscribedDiscountOnCategoryViewModel = new List<SubscribedDiscountOnCategoryViewModel>();
            try
            {
                //ArrayList lCartShopStockID = new ArrayList(CartShopStockID);

                //ArrayList lCartShopStockID = CartShopStockID;

                DataTable lDataTablelCartShopStockID = new DataTable();
                lDataTablelCartShopStockID.Columns.Add("ID");
                lDataTablelCartShopStockID.Columns.Add("PurchaseQty");
                foreach (var i in CartShopStockID)
                {
                    DataRow dr = lDataTablelCartShopStockID.NewRow();
                    dr[0] = i.shopStockId;
                    dr[1] = i.Quantity;
                    lDataTablelCartShopStockID.Rows.Add(dr);
                }

                ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                SqlConnection con = new SqlConnection(readCon.DB_CONNECTION);

                SqlCommand sqlComm = new SqlCommand("SubscriptionCheckPlan", con);
                sqlComm.CommandType = CommandType.StoredProcedure;
                sqlComm.Parameters.AddWithValue("@userLoginId", UserLoginId);
                sqlComm.Parameters.AddWithValue("@CartList", SqlDbType.Structured).Value = lDataTablelCartShopStockID;
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                DataTable dt = new DataTable();
                da.Fill(dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    SubscribedDiscountOnCategoryViewModel lSubscribedDiscountOnCategory = new SubscribedDiscountOnCategoryViewModel();

                    lSubscribedDiscountOnCategory.RootLevelCategoryId = Convert.ToInt16(dt.Rows[i]["RootLevelCategoryId"].ToString());
                    lSubscribedDiscountOnCategory.Amount = Convert.ToDecimal(dt.Rows[i]["Amount"].ToString());
                    lSubscribedDiscountOnCategory.Percent = Convert.ToDecimal(dt.Rows[i]["Percentz"].ToString());

                    lSubscribedDiscountOnCategoryViewModel.Add(lSubscribedDiscountOnCategory);
                }


            }
            catch (Exception)
            {

                throw;
            }
            return lSubscribedDiscountOnCategoryViewModel;

        }

        public static List<SubscribedFacilityViewModel> SubscribedFacility(long UserLoginId)
        {
            List<SubscribedFacilityViewModel> lSubscribedFacilityViewModel = new List<SubscribedFacilityViewModel>();
            try
            {

                ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                SqlConnection con = new SqlConnection(readCon.DB_CONNECTION);

                SqlCommand sqlComm = new SqlCommand("SubscriptionCheckFacility", con);
                sqlComm.CommandType = CommandType.StoredProcedure;
                sqlComm.Parameters.AddWithValue("@userLoginId", UserLoginId);
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                DataTable dt = new DataTable();
                da.Fill(dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    SubscribedFacilityViewModel lSubscribedFacility = new SubscribedFacilityViewModel();

                    lSubscribedFacility.ID = Convert.ToInt16(dt.Rows[i]["ID"].ToString());
                    lSubscribedFacility.Name = dt.Rows[i]["Name"].ToString();
                    lSubscribedFacility.BehaviorType = Convert.ToInt16(dt.Rows[i]["BehaviorType"].ToString());
                    lSubscribedFacility.FacilityValue = Convert.ToDecimal(dt.Rows[i]["FacilityValue"].ToString());

                    lSubscribedFacilityViewModel.Add(lSubscribedFacility);
                }

            }
            catch (Exception)
            {

                throw;
            }

            return lSubscribedFacilityViewModel;

        }



        public void InsertSubscriptionPlanAmountUsedBy(long UserLoginId, List<OrderDetailsCartShopStock> CartShopStockID, long CustOrderId, List<SubscribedFacilityViewModel> facilityList)
        {
            List<SubscribedFacilityViewModel> lSubscribedFacilityViewModel = new List<SubscribedFacilityViewModel>();
            try
            {

                DataTable lDataTablelCartShopStockID = new DataTable();
                lDataTablelCartShopStockID.Columns.Add("ID");
                lDataTablelCartShopStockID.Columns.Add("PurchaseQty");
                foreach (var i in CartShopStockID)
                {
                    DataRow dr = lDataTablelCartShopStockID.NewRow();
                    dr[0] = i.shopStockId;
                    dr[1] = i.Quantity;
                    lDataTablelCartShopStockID.Rows.Add(dr);
                }


                DataTable lDataTablelfacility = new DataTable();
                lDataTablelfacility.Columns.Add("ID");
                lDataTablelfacility.Columns.Add("Name");
                lDataTablelfacility.Columns.Add("BehaviorType");
                lDataTablelfacility.Columns.Add("FacilityValue");

                if (facilityList != null)
                {
                    foreach (var i in facilityList)
                    {
                        DataRow dr = lDataTablelfacility.NewRow();
                        dr[0] = i.ID;
                        dr[1] = i.Name;
                        dr[2] = i.BehaviorType;
                        dr[3] = i.FacilityValue;
                        lDataTablelfacility.Rows.Add(dr);
                    }
                }


                ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                SqlConnection con = new SqlConnection(readCon.DB_CONNECTION);

                SqlCommand sqlComm = new SqlCommand("SubscriptionInsert", con);
                sqlComm.CommandType = CommandType.StoredProcedure;
                sqlComm.Parameters.AddWithValue("@userLoginId", UserLoginId);
                sqlComm.Parameters.AddWithValue("@CartList", SqlDbType.Structured).Value = lDataTablelCartShopStockID;
                sqlComm.Parameters.AddWithValue("@FacilityList", SqlDbType.Structured).Value = lDataTablelfacility;
                sqlComm.Parameters.AddWithValue("@CustomerOrderID", CustOrderId);
                con.Open();
                sqlComm.ExecuteNonQuery();
                con.Close();

            }
            catch (Exception)
            {
                throw;
            }

        }

        public static Boolean InsertSubscriptionPlanAmountUsedByAPI(long UserLoginId, List<OrderDetailsCartShopStock> CartShopStockID, long CustOrderId, List<SubscribedFacilityViewModel> facilityList)
        {
            List<SubscribedFacilityViewModel> lSubscribedFacilityViewModel = new List<SubscribedFacilityViewModel>();
            try
            {

                DataTable lDataTablelCartShopStockID = new DataTable();
                lDataTablelCartShopStockID.Columns.Add("ID");
                lDataTablelCartShopStockID.Columns.Add("PurchaseQty");
                foreach (var i in CartShopStockID)
                {
                    DataRow dr = lDataTablelCartShopStockID.NewRow();
                    dr[0] = i.shopStockId;
                    dr[1] = i.Quantity;
                    lDataTablelCartShopStockID.Rows.Add(dr);
                }


                DataTable lDataTablelfacility = new DataTable();
                lDataTablelfacility.Columns.Add("ID");
                lDataTablelfacility.Columns.Add("Name");
                lDataTablelfacility.Columns.Add("BehaviorType");
                lDataTablelfacility.Columns.Add("FacilityValue");

                if (facilityList != null)
                {
                    foreach (var i in facilityList)
                    {
                        DataRow dr = lDataTablelfacility.NewRow();
                        dr[0] = i.ID;
                        dr[1] = i.Name;
                        dr[2] = i.BehaviorType;
                        dr[3] = Convert.ToInt32(i.FacilityValue);
                        lDataTablelfacility.Rows.Add(dr);
                    }
                }


                ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                SqlConnection con = new SqlConnection(readCon.DB_CONNECTION);

                SqlCommand sqlComm = new SqlCommand("SubscriptionInsert", con);
                sqlComm.CommandType = CommandType.StoredProcedure;
                sqlComm.Parameters.AddWithValue("@userLoginId", UserLoginId);
                sqlComm.Parameters.AddWithValue("@CartList", SqlDbType.Structured).Value = lDataTablelCartShopStockID;
                sqlComm.Parameters.AddWithValue("@FacilityList", SqlDbType.Structured).Value = lDataTablelfacility;
                sqlComm.Parameters.AddWithValue("@CustomerOrderID", CustOrderId);
                con.Open();
                sqlComm.ExecuteNonQuery();
                con.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
                
            }
        }

        public static List<SubscriptionFacilityDetailViewModel> SubscribedDetails(long UserLoginId)
        {
            List<SubscriptionFacilityDetailViewModel> lSubscriptionFacilityDetailViewModel = new List<SubscriptionFacilityDetailViewModel>();
            try
            {
                ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                SqlConnection con = new SqlConnection(readCon.DB_CONNECTION);

                SqlCommand sqlComm = new SqlCommand("SubscribedDetails", con);
                sqlComm.CommandType = CommandType.StoredProcedure;
                sqlComm.Parameters.AddWithValue("@userLoginId", UserLoginId);
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        SubscriptionFacilityDetailViewModel lSubscriptionFacilityDetail = new SubscriptionFacilityDetailViewModel();
                        lSubscriptionFacilityDetail.SubscriptionPlan = dt.Rows[i]["PlanName"].ToString();
                        lSubscriptionFacilityDetail.CurrentPlanID = Convert.ToInt16(dt.Rows[i]["CurrentPlanID"].ToString());
                        lSubscriptionFacilityDetail.TotalFreeDelivery = Convert.ToInt16(dt.Rows[i]["TotalFreeDelivery"].ToString());
                        if (dt.Rows[i]["BalanceFreeDelivery"].ToString() != string.Empty)
                        {
                            lSubscriptionFacilityDetail.BalanceFreeDelivery = Convert.ToInt16(dt.Rows[i]["BalanceFreeDelivery"].ToString());
                        }
                        if (dt.Rows[i]["TotalSavingPerMonth"].ToString() != string.Empty)
                        {
                            lSubscriptionFacilityDetail.TotalSavingPerMonth = Convert.ToDecimal(dt.Rows[i]["TotalSavingPerMonth"].ToString());
                        }
                        if (dt.Rows[i]["TotalPurchasePerMonth"].ToString() != string.Empty)
                        {
                            lSubscriptionFacilityDetail.TotalPurchasePerMonth = Convert.ToDecimal(dt.Rows[i]["TotalPurchasePerMonth"].ToString());
                        }
                        if (dt.Rows[i]["PurchaseAsPerMonth"].ToString() != string.Empty)
                        {
                            lSubscriptionFacilityDetail.PurchaseAsPerMonth = Convert.ToDecimal(dt.Rows[i]["PurchaseAsPerMonth"].ToString());
                        }
                        lSubscriptionFacilityDetail.StartDate = Convert.ToDateTime(dt.Rows[i]["StartDate"].ToString());
                        lSubscriptionFacilityDetail.EndDate = Convert.ToDateTime(dt.Rows[i]["EndDate"].ToString());
                        lSubscriptionFacilityDetail.NoOfDaysRemain = Convert.ToInt32(dt.Rows[i]["DiffDate"]);
                        lSubscriptionFacilityDetailViewModel.Add(lSubscriptionFacilityDetail);


                    }
                }


            }
            catch (Exception)
            {

                throw;
            }
            return lSubscriptionFacilityDetailViewModel;

        }
    }
}
