using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ModelLayer.Models.ViewModel
{
    public class MerchantPayoutViewModel
    {
        public List<SelectListItem> PayoutDateFilter { get; set; }
        public int Pay { get; set; }
        public List<MerchantPaoutDetailsViewModel> PayoutDetails { get; set; }
        public List<MerchantTransactionViewModel> TransactionList { get; set; }
        public PayoutViewModel payout { get; set; }
    }

    public class PayoutViewModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal RMERP { get; set; }
        public decimal L0ERP { get; set; }
        public decimal UptoL6ERP { get; set; }
        public decimal UnpaidERP { get; set; }
        public decimal RMInactivePoints { get; set; }
        public decimal PrevoiusUnpaidERP { get; set; }
        public decimal UptoL6InactivePoints { get; set; }
        public int TotalUser { get; set; }
        public decimal TotalERP { get; set; }
        public decimal PayableERP { get; set; }
        public decimal Company { get; set; }
        public decimal Part5th { get; set; }
    }

    public class MerchantPaoutDetailsViewModel
    {
        public long UserLoginId { get; set; }
        public string Name { get; set; }
        public decimal RMERP { get; set; }
        public decimal L0ERP { get; set; }
        public decimal UptoL6ERP { get; set; }
        public decimal RMInactivePoint { get; set; }
        public decimal UptoL6InactivePoint { get; set; }
        public decimal UnpaidERP { get; set; }
        public decimal PreviousUnpaidERP { get; set; }
        public decimal TotalERP { get; set; }
        public decimal PayableERP { get; set; }
        public bool Status { get; set; }
        public bool BBPSubscriber { get; set; }
    }

}
