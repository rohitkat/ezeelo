using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ModelLayer.Models.ViewModel
{
    public class MerchantTransactionReport
    {
        public List<MerchantTransactionViewModel> list { get; set; }
        public long? merchantId { get; set; }
        public List<SelectListItem> MerchantList { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
    public class MerchantTransactionViewModel
    {
        public long ID { get; set; }
        public string TransactionCode { get; set; }
        public string ShopName { get; set; }
        public long MerchantTransactionId { get; set; }
        public decimal RelationshipManagerPercentage { get; set; }
        public decimal CompanyPercentage { get; set; }
        public decimal BuyerPercentage { get; set; }
        public decimal NetworkPercentage { get; set; }
        public decimal Part5thPercentage { get; set; }
        public decimal GST { get; set; }
        public bool IsApplied { get; set; }
        public decimal BillAmount { get; set; }
        public decimal Commission { get; set; }
        public decimal CalculatedCommission { get; set; }
        public decimal CommissionToERP { get; set; }
        public decimal CompanyERP { get; set; }
        public decimal Part5thERP { get; set; }
        public string RelationshipManagerName { get; set; }
        public long RelationshipManager_UserloginId { get; set; }
        public decimal RelationshipManagerERP { get; set; }
        public string BuyerName { get; set; }
        public long BuyerUserLoginID { get; set; }
        public decimal BuyerERP { get; set; }
        public decimal NetworkERP { get; set; }
        public string Level1_Name { get; set; }
        public long Level1_UserLoginID { get; set; }
        public decimal Level1ERP { get; set; }
        public string Level2_Name { get; set; }
        public long Level2_UserLoginID { get; set; }
        public decimal Level2ERP { get; set; }
        public string Level3_Name { get; set; }
        public long Level3_UserLoginID { get; set; }
        public decimal Level3ERP { get; set; }
        public string Level4_Name { get; set; }
        public long Level4_UserLoginID { get; set; }
        public decimal Level4ERP { get; set; }
        public string Level5_Name { get; set; }
        public long Level5_UserLoginID { get; set; }
        public decimal Level5ERP { get; set; }
        public string Level6_Name { get; set; }
        public long Level6_UserLoginID { get; set; }
        public decimal Level6ERP { get; set; }
        public bool IsPaid { get; set; }
        public DateTime TransactionDate { get; set; }
        public bool IsPayable { get; set; }
    }

    public class MerchantReport
    {
        public List<MerchantRatingReviewList> ratingReviewLists { get; set; }
        public List<MerchantPendingTransaction> pendingTrans { get; set; }
        public List<SelectListItem> MerchantList { get; set; }
        public long? MerchantID { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }

    public class MerchantRatingReviewList
    {
        public long SrNo { get; set; }
        public long UserLoginID { get; set; }
        public long MerchantID { get; set; }
        public string FranchiseName { get; set; }
        public string City { get; set; }
        public string Name { get; set; }
        public decimal Rating { get; set; }
        public string Review { get; set; }
        public bool isdisplay { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class MerchantPendingTransaction
    {
        public long SrNo { get; set; }
        public string FranchiseName { get; set; }
        public string City { get; set; }
        public string ContactPerson { get; set; }
        public string ContactNumber { get; set; }
        public string LeaderContactNo { get; set; }
        public decimal PendingCommission { get; set; }
    }

    public class MerchantGSTTransaction
    {
        public string FranchiseName { get; set; }
        public string GSTIN { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string TransactionCode { get; set; }
        public DateTime Date { get; set; }
        public decimal BillAmount { get; set; }
        public decimal Commission { get; set; }
        public decimal GSTPer { get; set; }
        public bool IsApplied { get; set; }
        public decimal GST { get; set; }
        public decimal TaxableValue { get; set; }
    }

    public class GSTReportViewModel
    {
        public List<SelectListItem> MerchantList { get; set; }
        public long MerchantID { get; set; }
        public List<SelectListItem> StateList { get; set; }
        public long? StateId { get; set; }
        public List<SelectListItem> CityList { get; set; }
        public long? CityId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public Merchant Merchant { get; set; }
        public List<MerchantGSTTransaction> TransactionList { get; set; }
    }

    public class MerchantAdminDashboard
    {
        public int LiveMerchant { get; set; }
        public int NewRegistered { get; set; }
        public int PendingKYCReg { get; set; }
        public int PendingKYCApproval { get; set; }
        public int RegFeePending { get; set; }
        public int PendingRechargeReq { get; set; }
        public int RechargeBelow30 { get; set; }
        public int ZeroBalance { get; set; }
        public int PendingTrans { get; set; }
    }
    public class MerchantDashboardAdminReport
    {
        public long SrNo { get; set; }
        public string Shop { get; set; }
        public string City { get; set; }
        public string ServiceName { get; set; }
        public string ContactPerson { get; set; }
        public string ContactNumber { get; set; }
        public decimal BalanceAmount { get; set; }
        public string Status { get; set; }
        public string UserName { get; set; }
    }
}