
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Franchise.Models.ViewModel;
using Franchise.Models;
using BusinessLogicLayer;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace Franchise.SalesOrder
{
    public class CustomerDetailReport
    {
        private static string fConnectionString = WebConfigurationManager.ConnectionStrings["EzeeloDBContext"].ToString();
        public List<CustomerViewModel> GetCustomerDetail(long? UserLoginID, int FranchiseID)
        {
            List<CustomerViewModel> lCust = new List<CustomerViewModel>();
            try
            {
                DataTable lDataTableCustomerOrder = new DataTable();
                SqlConnection con = new SqlConnection(fConnectionString);
                SqlCommand sqlComm = new SqlCommand("ReportFranchiseCustomerList", con);
                sqlComm.CommandType = CommandType.StoredProcedure;
                sqlComm.Parameters.AddWithValue("@pUserLoginID", SqlDbType.BigInt).Value = UserLoginID;
                sqlComm.Parameters.AddWithValue("@FranchiseID", SqlDbType.Int).Value = FranchiseID;
                con.Open();

                SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                DataTable dt = new DataTable();
                da.Fill(dt);
                
                lCust = Common.Helper.CreateListFromTable<CustomerViewModel>(dt);

                //List<CustomerOrderReportViewModel> lCustomerOrderViewModels = ListOrders(null);

                //foreach (CustomerViewModel customerViewModel in lCust)
                //{
                //    try
                //    {
                //        customerViewModel.TotalAmount = lCustomerOrderViewModels.Where(x => x.UserLoginID == customerViewModel.UserLoginID).Sum(y => y.PayableAmount);
                //        customerViewModel.TotalOrder = lCustomerOrderViewModels.Count(x => x.UserLoginID == customerViewModel.UserLoginID);
                //        customerViewModel.LastPurchasedDate = lCustomerOrderViewModels.Where(x => x.UserLoginID == customerViewModel.UserLoginID).OrderByDescending(x => x.CreateDate).FirstOrDefault().CreateDate;
                //    }
                //    catch (Exception ex1)
                //    {

                //    }
                //}
            }
            catch (Exception)
            {
                throw;
            }
            return lCust;
        }

        public List<TaxOnOrderViewModel> getTaxOnOrderViewModels( long CustomerOrderId)
        {
            List<TaxOnOrderViewModel> lTax = new List<TaxOnOrderViewModel>();
            try
            {
                SqlConnection con = new SqlConnection(fConnectionString);
                SqlCommand sqlComm = new SqlCommand("GetTaxOnOrder", con);
                sqlComm.CommandType = CommandType.StoredProcedure;
                sqlComm.Parameters.AddWithValue("@CustomerOrderId", SqlDbType.BigInt).Value = CustomerOrderId;
                con.Open();

                SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                DataTable dt = new DataTable();
                da.Fill(dt);

                lTax = Common.Helper.CreateListFromTable<TaxOnOrderViewModel>(dt);

            }
            catch (Exception)
            {
                throw;
            }
            return lTax.ToList();
        }

        public List<NewCustomerOrderViewModel> ListOrders(long? pUserLoginID, int FranchiseID, DateTime FromDate, DateTime ToDate,int PageIndex, int pageSize,
            string OrderCode,string OrderStatus,string Customer,decimal? TotalAmountMin,decimal? TotalAmountMax,string RegMobile,
            string PaymentMode,string DeliveryTime, out List<OrderListStatusCount> lStatusCounts)
        {
            List<NewCustomerOrderViewModel> lCustomerOrderViewModels = new List<NewCustomerOrderViewModel>();
            lStatusCounts = new List<OrderListStatusCount>();
            try
            {
                SqlConnection con = new SqlConnection(fConnectionString);
                SqlCommand sqlComm = new SqlCommand("ReportFranchiseCustomerOrder_New", con);
                sqlComm.CommandType = CommandType.StoredProcedure;
                sqlComm.Parameters.AddWithValue("@FranchiseID", SqlDbType.Int).Value = FranchiseID;
                sqlComm.Parameters.AddWithValue("@FromDate", SqlDbType.DateTime).Value = FromDate;
                sqlComm.Parameters.AddWithValue("@ToDate", SqlDbType.DateTime).Value = ToDate;
                sqlComm.Parameters.AddWithValue("@PageIndex", SqlDbType.Int).Value = PageIndex;
                sqlComm.Parameters.AddWithValue("@PageSize", SqlDbType.Int).Value = pageSize;
                sqlComm.Parameters.AddWithValue("@OrderCode", SqlDbType.NVarChar).Value = OrderCode;
                sqlComm.Parameters.AddWithValue("@OrderStatus", SqlDbType.NVarChar).Value = OrderStatus;
                sqlComm.Parameters.AddWithValue("@Customer", SqlDbType.NVarChar).Value = Customer;
                sqlComm.Parameters.AddWithValue("@TotalAmountMin", SqlDbType.Decimal).Value = TotalAmountMin;
                sqlComm.Parameters.AddWithValue("@TotalAmountMax", SqlDbType.Decimal).Value = TotalAmountMax;
                sqlComm.Parameters.AddWithValue("@RegMobile", SqlDbType.NVarChar).Value = RegMobile;
                sqlComm.Parameters.AddWithValue("@PaymentMode", SqlDbType.NVarChar).Value = PaymentMode;
                sqlComm.Parameters.AddWithValue("@DeliveryTime", SqlDbType.NVarChar).Value = DeliveryTime;
                con.Open();

                SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                DataTable dtCount = new DataTable();
                da.Fill(ds);
                if (ds.Tables.Count == 2)
                {
                    dt = ds.Tables[0];
                    dtCount = ds.Tables[1];
                }
                lCustomerOrderViewModels = Common.Helper.CreateListFromTable<NewCustomerOrderViewModel>(dt);
                lStatusCounts = Common.Helper.CreateListFromTable<OrderListStatusCount>(dtCount);
                if (pUserLoginID != null)
                {
                    lCustomerOrderViewModels = lCustomerOrderViewModels.Where(x => x.UserLoginID == pUserLoginID).ToList();
                }
                #region process
                lCustomerOrderViewModels = (from COVM in lCustomerOrderViewModels
                                            group COVM by new
                                            {
                                                COVM.COID,
                                                COVM.OrderStatus
                                            } into gcs
                                            select new NewCustomerOrderViewModel
                                            {
                                                COID = gcs.Key.COID,
                                                OrderCode = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).OrderCode,
                                                OrderStatus = gcs.Key.OrderStatus,
                                                UserLoginID = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).UserLoginID,
                                                Customer = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).Customer,
                                                RegMobile = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).RegMobile,
                                                RegEmail = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).RegEmail,
                                                ReferenceCustomerOrderID = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).ReferenceCustomerOrderID,
                                                OrderAmount = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).OrderAmount,
                                                NoOfPointUsed = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).NoOfPointUsed,
                                                ValuePerPoint = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).ValuePerPoint,
                                                CoupenCode = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).CoupenCode,
                                                CoupenAmount = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).CoupenAmount,
                                                PAN = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).PAN,
                                                PaymentMode = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).PaymentMode,
                                                PayableAmount = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).PayableAmount,
                                                PrimaryMobile = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).PrimaryMobile,
                                                SecondoryMobile = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).SecondoryMobile,
                                                ShippingAddress = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).ShippingAddress,
                                                Area = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).Area,
                                                AreaID = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).AreaID,
                                                Pincode = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).Pincode,
                                                PincodeID = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).PincodeID,
                                                City = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).City,
                                                CityID = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).CityID,
                                                CreateDate = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).CreateDate,
                                                CreateBy = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).CreateBy,
                                                CreatedByUser = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).CreatedByUser,
                                                ModifyDate = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).ModifyDate,
                                                ModifyBy = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).ModifyBy,
                                                ModifyByUser = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).ModifyByUser,
                                                NetworkIP = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).NetworkIP,
                                                DeviceType = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).DeviceType,
                                                DeviceID = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).DeviceID,
                                                DeliveryDate = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).DeliveryDate,
                                                DeliveryTime = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).DeliveryTime,
                                                DeliveryType = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).DeliveryType.ToString().ToLower(),//,
                                                RetailPoints = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).RetailPoints,//Yashaswi 30-7-2018
                                                IsLeader = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).IsLeader,//Yashaswi 30-7-2018
                                                MLMAmountUsed = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).MLMAmountUsed,    //Added BY Priti   on 24-12-2018
                                                //TotalRecord = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).TotalRecord,
                                                IsBusinessBoosterPlanOrder = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).IsBusinessBoosterPlanOrder,
                                                CashbackPoints = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).CashbackPoints
                                            }).ToList(); //Apply if want to take only 50 records(.Take(50))
                #endregion
            }
            catch (Exception)
            {
                throw;
            }
            return lCustomerOrderViewModels;
        }

        public List<NewCustomerOrderDetailViewModel> CustomerOrderDetail(long pCustomerOrderID)
        {
            List<NewCustomerOrderDetailViewModel> lCustomerOrderDetailViewModels = new List<NewCustomerOrderDetailViewModel>();
            try
            {
                DataTable lDataTableCustomerOrder = new DataTable();
                SqlConnection con = new SqlConnection(fConnectionString);
                SqlCommand sqlComm = new SqlCommand("ReportCustomerOrderDetail", con);
                sqlComm.CommandType = CommandType.StoredProcedure;
                sqlComm.Parameters.AddWithValue("@pCOID", SqlDbType.BigInt).Value = pCustomerOrderID;
                con.Open();

                SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                DataTable dt = new DataTable();
                da.Fill(dt);
                lCustomerOrderDetailViewModels = Common.Helper.CreateListFromTable<NewCustomerOrderDetailViewModel>(dt);
            }
            catch (Exception)
            {
                throw;
            }
            return lCustomerOrderDetailViewModels;
        }

        public List<ProductListViewModel> ProductWithStock(string ProductName, int FranchiseID)
        {
            List<ProductListViewModel> lProductList = new List<ProductListViewModel>();
            try
            {
                DataTable lDataTableCustomerOrder = new DataTable();
                SqlConnection con = new SqlConnection(fConnectionString);
                SqlCommand sqlComm = new SqlCommand("SELECT_PRODUCTWITHSTOCKDETAILS", con);
                sqlComm.CommandType = CommandType.StoredProcedure;
                sqlComm.Parameters.AddWithValue("@FranchiseID", SqlDbType.Int).Value = FranchiseID;
                sqlComm.Parameters.AddWithValue("@ProductName", SqlDbType.VarChar).Value = ProductName;
                con.Open();

                SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                DataTable dt = new DataTable();
                da.Fill(dt);
                lProductList = Common.Helper.CreateListFromTable<ProductListViewModel>(dt);
            }
            catch (Exception)
            {
                throw;
            }
            return lProductList;
        }
    }
}
