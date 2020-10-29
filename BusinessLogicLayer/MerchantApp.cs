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
    public class MerchantApp : MerchantManagment
    {
        public MerchantApp(System.Web.HttpServerUtility server)
            : base(server)
        {
        }

        private static EzeeloDBContext db = new EzeeloDBContext();
        ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
        
        public static List<MerchantDashboardViewModel> DashboardDetails(long UserLoginId)
        {
            List<MerchantDashboardViewModel> lMerchantDashboardViewModel = new List<MerchantDashboardViewModel>();
            try
            {
                ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                SqlConnection con = new SqlConnection(readCon.DB_CONNECTION);

                SqlCommand sqlComm = new SqlCommand("Select_MerchantDetail", con);
                sqlComm.CommandType = CommandType.StoredProcedure;
                sqlComm.Parameters.AddWithValue("@UserID", UserLoginId);
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                DataTable dt = new DataTable();
                da.Fill(dt);
                //con.Close();
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        MerchantDashboardViewModel lMerchantDashboardDetails = new MerchantDashboardViewModel();
                        lMerchantDashboardDetails.ID = Convert.ToInt16(dt.Rows[i]["ID"].ToString());
                        lMerchantDashboardDetails.Name = dt.Rows[i]["Name"].ToString();
                        lMerchantDashboardDetails.OpeningTime = Convert.ToDateTime(dt.Rows[i]["OpeningTime"].ToString());
                        lMerchantDashboardDetails.ClosingTime = Convert.ToDateTime(dt.Rows[i]["ClosingTime"].ToString());
                        lMerchantDashboardDetails.PLACED = Convert.ToInt16(dt.Rows[i]["PLACED"].ToString());
                        string src = ImageDisplay.LoadShopLogo(Convert.ToInt16(dt.Rows[i]["ID"].ToString()), ProductUpload.IMAGE_TYPE.Approved);
                        lMerchantDashboardDetails.ShopLogo = src;
                        Review objreview = new Review();
                        DisplayReviewsViewModel lDisplayReviewsViewModel = new DisplayReviewsViewModel();
                        lDisplayReviewsViewModel = objreview.GetReviews(Convert.ToInt16(dt.Rows[i]["ID"].ToString()), BusinessLogicLayer.Review.REVIEWS.SHOP);
                        lMerchantDashboardDetails.Review = Convert.ToDecimal(lDisplayReviewsViewModel.AvgPoints.AvgRatingPonts);
                        lMerchantDashboardViewModel.Add(lMerchantDashboardDetails);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return lMerchantDashboardViewModel;
        }

        public static List<MerchantAppOrderListViewModel> OrderList(long UserLoginId , int OrderStatus)
        {
            List<MerchantAppOrderListViewModel> lMerchantAppOrderListViewModel = new List<MerchantAppOrderListViewModel>();
            try
            {
                ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                SqlConnection con = new SqlConnection(readCon.DB_CONNECTION);

                SqlCommand sqlComm = new SqlCommand("Select_MerchantWiseOrderList", con);
                sqlComm.CommandType = CommandType.StoredProcedure;
                sqlComm.Parameters.AddWithValue("@UserID", UserLoginId);
                sqlComm.Parameters.AddWithValue("@OrderStatus", OrderStatus);
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        MerchantAppOrderListViewModel lMerchantAppOrderList = new MerchantAppOrderListViewModel();
                        lMerchantAppOrderList.ID = Convert.ToInt32(dt.Rows[i]["ID"].ToString());
                        lMerchantAppOrderList.CustomerName = dt.Rows[i]["CustomerName"].ToString();
                        lMerchantAppOrderList.ShopOrderCode = dt.Rows[i]["ShopOrderCode"].ToString();
                        lMerchantAppOrderList.CreateDate = Convert.ToDateTime(dt.Rows[i]["OrderDate"].ToString());
                        lMerchantAppOrderList.TotalAmount = Convert.ToDecimal(dt.Rows[i]["TotalAmount"].ToString());
                        try { lMerchantAppOrderList.ActualTimeFrom = Convert.ToDateTime(dt.Rows[i]["ActualTimeFrom"].ToString()); }
                        catch (Exception ex) { }
                        try{ lMerchantAppOrderList.ActualTimeTo = Convert.ToDateTime(dt.Rows[i]["ActualTimeTo"].ToString()); }
                        catch (Exception ex1){}
                        
                        lMerchantAppOrderList.TotalItem = Convert.ToInt16(dt.Rows[i]["TotalItem"].ToString());
                        lMerchantAppOrderListViewModel.Add(lMerchantAppOrderList);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return lMerchantAppOrderListViewModel;
        }

        public static List<MerchantAppOrderDetailsViewModel> OrderDetails(long UserLoginId , int OrderStatus, long OrderId)
        {
            List<MerchantAppOrderDetailsViewModel> lMerchantAppOrderDetailsViewModel = new List<MerchantAppOrderDetailsViewModel>();
            try
            {
                ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                SqlConnection con = new SqlConnection(readCon.DB_CONNECTION);

                SqlCommand sqlComm = new SqlCommand("Select_MerchantWiseOrderDetail", con);
                sqlComm.CommandType = CommandType.StoredProcedure;
                sqlComm.Parameters.AddWithValue("@UserID", UserLoginId);
                sqlComm.Parameters.AddWithValue("@OrderStatus", OrderStatus);
                sqlComm.Parameters.AddWithValue("@OrderID", OrderId);
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        MerchantAppOrderDetailsViewModel lMerchantAppOrderDetails = new MerchantAppOrderDetailsViewModel();
                        lMerchantAppOrderDetails.ID = Convert.ToInt32(dt.Rows[i]["ID"].ToString());
                        lMerchantAppOrderDetails.ProductName = dt.Rows[i]["ProductName"].ToString();
                        lMerchantAppOrderDetails.ColorName = dt.Rows[i]["ColorName"].ToString();
                        lMerchantAppOrderDetails.SizeName = dt.Rows[i]["SizeName"].ToString();
                        lMerchantAppOrderDetails.DimensionName = dt.Rows[i]["DimensionName"].ToString();
                        lMerchantAppOrderDetails.MaterialName = dt.Rows[i]["MaterialName"].ToString();
                        lMerchantAppOrderDetails.Qty = Convert.ToInt16(dt.Rows[i]["Qty"].ToString());
                        lMerchantAppOrderDetails.SaleRate = Convert.ToDecimal(dt.Rows[i]["SaleRate"].ToString());
                        lMerchantAppOrderDetails.TotalAmount = Convert.ToDecimal(dt.Rows[i]["TotalAmount"].ToString());
                        lMerchantAppOrderDetailsViewModel.Add(lMerchantAppOrderDetails);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return lMerchantAppOrderDetailsViewModel;
        }



        //public static Boolean UpdateOrderTrack(long UserLoginId, int OrderStatus, long OrderId)
        //{
        //    try
        //    {
        //        ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
        //        SqlConnection con = new SqlConnection(readCon.DB_CONNECTION);

        //        SqlCommand sqlComm = new SqlCommand("InsertUpdate_MerchantWiseOrderStatus", con);
        //        sqlComm.CommandType = CommandType.StoredProcedure;
        //        sqlComm.Parameters.AddWithValue("@UserId", UserLoginId);
        //        sqlComm.Parameters.AddWithValue("@OrderStatus", OrderStatus);
        //        sqlComm.Parameters.AddWithValue("@OrderID", OrderId);
        //        con.Open();
        //        sqlComm.ExecuteNonQuery();
        //        con.Close();
        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //        throw;      
        //    }
        //}
        public static int UpdateOrderTrack(long UserLoginId, int OrderStatus, long OrderId, ref int Result)
        {
            Result = 0;
            try
            {
                ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                SqlConnection con = new SqlConnection(readCon.DB_CONNECTION);
                con.Open();
                SqlParameter parm = new SqlParameter("@return", SqlDbType.Int);
                SqlCommand sqlComm = new SqlCommand("InsertUpdate_MerchantWiseOrderStatus", con);
                sqlComm.CommandType = CommandType.StoredProcedure;
                parm.Direction = ParameterDirection.ReturnValue;
                sqlComm.Parameters.AddWithValue("@UserId", UserLoginId);
                sqlComm.Parameters.AddWithValue("@OrderStatus", OrderStatus);
                sqlComm.Parameters.AddWithValue("@OrderID", OrderId);
                sqlComm.Parameters.Add(parm);
                sqlComm.ExecuteNonQuery();
                Result = Convert.ToInt32(parm.Value);
                con.Close();
            }
            catch (Exception)
            {
                Result = 103; //Exception Found
                throw;
            }
            return Result;
        }


        public static List<MerchantWiseOrderStatusCountViewModel> MerchantWiseOrderStatusCount(long UserLoginId)
        {
            List<MerchantWiseOrderStatusCountViewModel> lMerchantWiseOrderStatusCountViewModel = new List<MerchantWiseOrderStatusCountViewModel>();
            try
            {
                ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                SqlConnection con = new SqlConnection(readCon.DB_CONNECTION);

                SqlCommand sqlComm = new SqlCommand("Select_MerchantWiseOrderStatus", con);
                sqlComm.CommandType = CommandType.StoredProcedure;
                sqlComm.Parameters.AddWithValue("@UserID", UserLoginId);
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                DataTable dt = new DataTable();
                da.Fill(dt);
                //con.Close();
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        MerchantWiseOrderStatusCountViewModel lMerchantWiseOrderStatusCounts = new MerchantWiseOrderStatusCountViewModel();
                        lMerchantWiseOrderStatusCounts.PLACED = Convert.ToInt16(dt.Rows[i]["PLACED"].ToString());
                        lMerchantWiseOrderStatusCounts.CONFIRM = Convert.ToInt16(dt.Rows[i]["CONFIRM"].ToString());
                        lMerchantWiseOrderStatusCounts.PACKED = Convert.ToInt16(dt.Rows[i]["PACKED"].ToString());
                        lMerchantWiseOrderStatusCounts.DISPATCHED_FROM_SHOP = Convert.ToInt16(dt.Rows[i]["DISPATCHED_FROM_SHOP"].ToString());
                        lMerchantWiseOrderStatusCounts.IN_GODOWN = Convert.ToInt16(dt.Rows[i]["IN_GODOWN"].ToString());
                        lMerchantWiseOrderStatusCounts.DISPATCHED_FROM_GODOWN = Convert.ToInt16(dt.Rows[i]["DISPATCHED_FROM_GODOWN"].ToString());
                        lMerchantWiseOrderStatusCounts.DELIVERED = Convert.ToInt16(dt.Rows[i]["DELIVERED"].ToString());
                        lMerchantWiseOrderStatusCounts.RETURNED = Convert.ToInt16(dt.Rows[i]["RETURNED"].ToString());
                        lMerchantWiseOrderStatusCounts.CANCELLED = Convert.ToInt32(dt.Rows[i]["CANCELLED"]);
                        lMerchantWiseOrderStatusCountViewModel.Add(lMerchantWiseOrderStatusCounts);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return lMerchantWiseOrderStatusCountViewModel;
        }


        public static int UpdateShopProfile(long UserLoginId, DateTime OpeningTime, DateTime ClosingTime, ref int Result)
        {
            Result = 0;
            try
            {
                ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                SqlConnection con = new SqlConnection(readCon.DB_CONNECTION);
                con.Open();
                SqlParameter parm = new SqlParameter("@return", SqlDbType.Int);
                SqlCommand sqlComm = new SqlCommand("Update_ShopTime", con);
                sqlComm.CommandType = CommandType.StoredProcedure;
                parm.Direction = ParameterDirection.ReturnValue;
                sqlComm.Parameters.AddWithValue("@UserId", UserLoginId);
                sqlComm.Parameters.AddWithValue("@OpeningTime", OpeningTime);
                sqlComm.Parameters.AddWithValue("@ClosingTime", ClosingTime);
                sqlComm.Parameters.Add(parm);
                sqlComm.ExecuteNonQuery();
                Result = Convert.ToInt32(parm.Value);
                con.Close();
            }
            catch (Exception)
            {
                Result = 103; //Exception Found
                throw;
            }
            return Result;
        }
    }
}
