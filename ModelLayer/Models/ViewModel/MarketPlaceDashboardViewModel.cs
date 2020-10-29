using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class MarketPlaceDashboardViewModel
    {
        public string ShopName { get; set; }
        public string ShopImagePath { get; set; }
        public string ContactPerson { get; set; }
        public decimal BillAmount { get; set; }
        public decimal Commission { get; set; }
        public int TotalTransaction { get; set; }
        public int TotalUser { get; set; }
        public decimal Recharge { get; set; }
        public decimal PendingRecharge { get; set; }
        public List<TransReport> TransactionList { get; set; }
        public List<RatingReport> RatingList { get; set; }
        public decimal Rating { get; set; }
        public int DaysLeft { get; set; }
        public string RechargeRemark { get; set; }
        public string DaysLeftRemark { get; set; }
        public string RatingRemark { get; set; }
        public long MerchantId { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public decimal PendingTransactionValue { get; set; }
    }
    public class TransReport
    {
        public long SrNO { get; set; }
        public string TransactionCode { get; set; }
        public long UserLoginId { get; set; }
        public string UserName { get; set; }
        public string MobileNo { get; set; }
        public decimal BillAmount { get; set; }
        public decimal Commission { get; set; }
        public DateTime TransactionDate { get; set; }
        public bool isPayable { get; set; }
    }

    public class TranasactionReportViewModel
    {
        public List<TransReport> TransactionList { get; set; }
        public List<RatingReport> RatingList { get; set; }
        public List<Passbook> Passbook { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }

    public class MerchantShopImages
    {
        public string ShopImagePath { get; set; }
        public List<MerchantBanner> list { get; set; }
        public List<MerchantBannerUpdateRequest> requestList { get; set; }
    }
    public class RatingReport
    {
        public string Name { get; set; }
        public decimal Rating { get; set; }
        public string Review { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class BarChart
    {
        public string y { get; set; }
        public int a { get; set; }
        public int b { get; set; }
    }

    public class PieChart
    {
        public string label { get; set; }
        public int value { get; set; }
    }

    public class Passbook
    {
        public long SrNo { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        public bool IsPayable { get; set; }
    }
}
