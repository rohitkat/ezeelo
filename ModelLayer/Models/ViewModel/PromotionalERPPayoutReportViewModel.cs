using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ModelLayer.Models.ViewModel
{
    public class PromotionalERPPayoutReportViewModel
    {
        public PromotionalERPPayout objEzeeMoneyPayout = new PromotionalERPPayout();
        public List<SelectListItem> listPromotionalERPPayoutDetails { get; set; }
        //public List<PromotionalERPPayoutDetails> listPromotionalERPPayoutDetails = new List<PromotionalERPPayoutDetails>();
        //public List<EzeeMoneyPayoutDetails> PayoutDateFilter { get; set; }
        public List<SelectListItem> PayoutDateFilter { get; set; }
        public bool All { get; set; }
        public List<PromotionalERPPayoutDetail_ReportViewModel> List { get; set; }
    }
    public class PromotionalERPPayoutDetail_ReportViewModel
    {
        public long Id { get; set; }
        public string PaidDate { get; set; }
        public long UserLoginId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public decimal ERP { get; set; }
        public decimal EzeeMoney { get; set; }
        public string ReferenceText { get; set; }
        public bool ActiveUser { get; set; }
        public int Level { get; set; }
        public long EzeeMoneyPayoutId { get; set; }
        public decimal Amount { get; set; }

    }
        public class PromotionalERPPayout_ReportViewModel
    {
        public List<PromotionalERPPayoutDetail_ReportViewModel> listPromotionalERPPayoutDetails = new List<PromotionalERPPayoutDetail_ReportViewModel>();
        public long UserloginId { get; set; }
    }
    public class PromotionalERPPayoutReportExportToExcel_List
    {
        public int SrNo { get; set; }
        public long UserloginId { get; set; }
        public string PaidDate { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public decimal ERP { get; set; }
        public decimal EzeeMoney { get; set; }
        public string ReferenceText { get; set; }

    }
}