using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelLayer.Models;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using ModelLayer.Models.ViewModel.Report.Account;

namespace BusinessLogicLayer.Account
{
    public class Report
    {
        private string fConnectionString = System.Configuration.ConfigurationSettings.AppSettings["DB_CON"].ToString();
        public Report() { }
        public List<ReportTransactionInputViewModel> ReportTransactionViewModel(DateTime pFromDate, DateTime pToDate)
        {
            List<ReportTransactionInputViewModel> lReportTransactionViewModels = new List<ReportTransactionInputViewModel>();
            try
            {
                DataTable lDataTableCustomerOrder = new DataTable();
                SqlConnection con = new SqlConnection(fConnectionString);
                SqlCommand sqlComm = new SqlCommand("ReportTransactionInput", con);
                sqlComm.CommandType = CommandType.StoredProcedure;
                sqlComm.Parameters.AddWithValue("@FromDate", SqlDbType.DateTime).Value = pFromDate;
                sqlComm.Parameters.AddWithValue("@ToDate", SqlDbType.DateTime).Value = pToDate;
                con.Open();

                SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                DataTable dt = new DataTable();
                da.Fill(dt);
                lReportTransactionViewModels = Common.Helper.CreateListFromTable<ReportTransactionInputViewModel>(dt);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ReportTransactionViewModel]", "Problem in loading Report" + Environment.NewLine + ex.Message);
            }
            return lReportTransactionViewModels;
        }

        public List<ReportTransactionInputProcessAccountViewModel> GetCashReceipts(int pFranchiseID, DateTime pFromDate, DateTime pToDate)
        {
            List<ReportTransactionInputProcessAccountViewModel> lReportTransactionInputProcessAccountViewModel = new List<ReportTransactionInputProcessAccountViewModel>();
            try
            {
                DataTable lDataTableCustomerOrder = new DataTable();
                SqlConnection con = new SqlConnection(fConnectionString);
                SqlCommand sqlComm = new SqlCommand("ReportTransactionInputProcessAccount", con);
                sqlComm.CommandType = CommandType.StoredProcedure;
                sqlComm.Parameters.AddWithValue("@pFranchiseID", SqlDbType.Int).Value = pFranchiseID;
                sqlComm.Parameters.AddWithValue("@pFromDate", SqlDbType.DateTime).Value = pFromDate;
                sqlComm.Parameters.AddWithValue("@pToDate", SqlDbType.DateTime).Value = pToDate;
                con.Open();

                SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                DataTable dt = new DataTable();
                da.Fill(dt);
                lReportTransactionInputProcessAccountViewModel = Common.Helper.CreateListFromTable<ReportTransactionInputProcessAccountViewModel>(dt);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[GetCashReceipts]", "Problem in loading Report" + Environment.NewLine + ex.Message);
            }
            return lReportTransactionInputProcessAccountViewModel;
        }

        public List<ReportTransactionInputPendingAccountViewModel> GetAccountPending(int pFranchiseID)
        {
            List<ReportTransactionInputPendingAccountViewModel> lReportTransactionInputPendingAccountViewModel = new List<ReportTransactionInputPendingAccountViewModel>();
            try
            {
                DataTable lDataTableCustomerOrder = new DataTable();
                SqlConnection con = new SqlConnection(fConnectionString);
                SqlCommand sqlComm = new SqlCommand("ReportTransactionInputPendingAccount", con);
                sqlComm.CommandType = CommandType.StoredProcedure;
                sqlComm.Parameters.AddWithValue("@pFranchiseID", SqlDbType.Int).Value = pFranchiseID;
                con.Open();

                SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                DataTable dt = new DataTable();
                da.Fill(dt);
                lReportTransactionInputPendingAccountViewModel = Common.Helper.CreateListFromTable<ReportTransactionInputPendingAccountViewModel>(dt);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[GetAccountPending]", "Problem in loading Report" + Environment.NewLine + ex.Message);
            }
            return lReportTransactionInputPendingAccountViewModel.OrderByDescending(x => x.CustomerOrderID).ToList();
        }
    }
}
