using BusinessLogicLayer;
using MarketPartner.Filter;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MarketPartner.Controllers
{
    [SessionExpire]
    [Authorize]
    public class ReportController : Controller
    {
        BoosterPlanData BD = new BoosterPlanData();
        public ActionResult TransactionReport()
        {
            TranasactionReportViewModel obj = new TranasactionReportViewModel();
            DateTime now = DateTime.Now;
            obj.FromDate = new DateTime(now.Year, now.Month, 1);
            obj.ToDate = obj.FromDate.AddMonths(1).AddDays(-1);
            long MerchantID = Convert.ToInt64(Session["MerchantID"]);
            List<TransReport> TransactionList = new List<TransReport>();
            obj.TransactionList = GetTransactionReportData(MerchantID, obj.FromDate, obj.ToDate);
            return View(obj);
        }

        [HttpPost]
        public ActionResult TransactionReport(TranasactionReportViewModel obj)
        {
            long MerchantID = Convert.ToInt64(Session["MerchantID"]);
            List<TransReport> TransactionList = new List<TransReport>();
            obj.TransactionList = GetTransactionReportData(MerchantID, obj.FromDate, obj.ToDate);
            return View(obj);
        }

        public List<TransReport> GetTransactionReportData(long MerchantID, DateTime FromDate,DateTime ToDate)
        {
            List<TransReport> TransactionList = new List<TransReport>();
            List<SqlParameter> sp = new List<SqlParameter>()
                {
                    new SqlParameter() {ParameterName = "@MerchantId", SqlDbType = SqlDbType.BigInt, Value= MerchantID},
                    new SqlParameter() {ParameterName = "@fromdate", SqlDbType = SqlDbType.DateTime, Value= FromDate},
                    new SqlParameter() {ParameterName = "@todate", SqlDbType = SqlDbType.DateTime, Value= ToDate},
                };
            DataSet ds = new DataSet();
            ds = BD.GetData("MerchantUserTransactionReport", sp);
            if (ds.Tables.Count > 0)
            {
                TransactionList = BusinessLogicLayer.Helper.CreateListFromTable<TransReport>(ds.Tables[0]);
            }
            return TransactionList;
        }

        public List<RatingReport> GetRatingReviewReportData(long MerchantID, DateTime FromDate, DateTime ToDate)
        {
            List<RatingReport> RatingReport = new List<RatingReport>();
            List<SqlParameter> sp = new List<SqlParameter>()
                {
                    new SqlParameter() {ParameterName = "@MerchantId", SqlDbType = SqlDbType.BigInt, Value= MerchantID},
                    new SqlParameter() {ParameterName = "@fromdate", SqlDbType = SqlDbType.DateTime, Value= FromDate},
                    new SqlParameter() {ParameterName = "@todate", SqlDbType = SqlDbType.DateTime, Value= ToDate},
                };
            DataSet ds = new DataSet();
            ds = BD.GetData("RatingReviewReport", sp);
            if (ds.Tables.Count > 0)
            {
                RatingReport = BusinessLogicLayer.Helper.CreateListFromTable<RatingReport>(ds.Tables[0]);
            }
            return RatingReport;
        }

        public List<Passbook> GetPassbookData(long MerchantID)
        {
            List<Passbook> Passbook = new List<Passbook>();
            List<SqlParameter> sp = new List<SqlParameter>()
                {
                    new SqlParameter() {ParameterName = "@MerchantId", SqlDbType = SqlDbType.BigInt, Value= MerchantID},
                    //new SqlParameter() {ParameterName = "@fromdate", SqlDbType = SqlDbType.DateTime, Value= FromDate},
                    //new SqlParameter() {ParameterName = "@todate", SqlDbType = SqlDbType.DateTime, Value= ToDate},
                };
            DataSet ds = new DataSet();
            ds = BD.GetData("MerchantPassbook", sp);
            if (ds.Tables.Count > 0)
            {
                Passbook = BusinessLogicLayer.Helper.CreateListFromTable<Passbook>(ds.Tables[0]);
            }
            return Passbook;
        }


        public ActionResult RatingReviewReport()
        {
            TranasactionReportViewModel obj = new TranasactionReportViewModel();
            DateTime now = DateTime.Now;
            obj.FromDate = new DateTime(now.Year, now.Month, 1);
            obj.ToDate = obj.FromDate.AddMonths(1).AddDays(-1);
            long MerchantID = Convert.ToInt64(Session["MerchantID"]);
            List<RatingReport> RatingList = new List<RatingReport>();
            obj.RatingList = GetRatingReviewReportData(MerchantID, obj.FromDate, obj.ToDate);
            return View(obj);
        }

        [HttpPost]
        public ActionResult RatingReviewReport(TranasactionReportViewModel obj)
        {
            long MerchantID = Convert.ToInt64(Session["MerchantID"]);
            List<RatingReport> TransactionList = new List<RatingReport>();
            obj.RatingList = GetRatingReviewReportData(MerchantID, obj.FromDate, obj.ToDate);
            return View(obj);
        }

        public ActionResult PassbookReport()
        {
            TranasactionReportViewModel obj = new TranasactionReportViewModel();
            long MerchantID = Convert.ToInt64(Session["MerchantID"]);
            List<Passbook> Passbook = new List<Passbook>();
            obj.Passbook = GetPassbookData(MerchantID);
            return View(obj);
        }

    }
}