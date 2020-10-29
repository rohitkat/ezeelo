using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ModelLayer.Models.ViewModel
{
    public class EzeeMoneyPayoutReportViewModel
    {
        public EzeeMoneyPayout objEzeeMoneyPayout = new EzeeMoneyPayout();
        public List<EzeeMoneyPayoutDetails> listEzeeMoneyPayoutDetails = new List<EzeeMoneyPayoutDetails>();
        //public List<EzeeMoneyPayoutDetails> PayoutDateFilter { get; set; }
        public List<SelectListItem> PayoutDateFilter { get; set; }
        public bool All { get; set; }
        public bool Status { get; set; }
        public long UserloginId { get; set; }
    }

    public class EzeeMoneyPayout_ReportViewModel
    {
        public List<EzeeMoneyPayoutDetails> listEzeeMoneyPayoutDetails = new List<EzeeMoneyPayoutDetails>();
        public bool Status { get; set; }
        public long UserloginId { get; set; }
    }
    public class ExportToExcel_List
    {
        public int SrNo { get; set; }
        public long UserloginId { get; set; }
        public string Name { get; set; }
        public string EmailId { get; set; }
        public string PhoneNo { get; set; }
        public decimal TotalOrdAmt { get; set; }
        public decimal TotalRetailPoints { get; set; }
      
        public decimal ERP { get; set; }
        public string Status { get; set; }
        public decimal EzeeMoney { get; set; }
        public decimal QRP { get; set; }
    }
}
