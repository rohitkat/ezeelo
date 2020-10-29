using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel.Report.Account
{
    public class ReportTransactionInputProcessAccountViewModel
    {
        public long ID { get; set; }
        public long LeadgerHeadID { get; set; }
        public string LeadgerHead { get; set; }
        public int ReceivedPaymentModeID { get; set; }
        public string ReceivedPaymentMode { get; set; }
        public long TransactionInputID { get; set; }
        public long CustomerOrderID { get; set; }
        public string OrderCode { get; set; }
        public decimal Amount { get; set; }
        public long ReceivedFromUserLoginID { get; set; }
        public string ReceivedFromUser { get; set; }
        public Boolean PODReceived { get; set; }
        public string Narration { get; set; }
        public DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public string CreateByPersonalDetail { get; set; }
        public DateTime ModifyDate { get; set; }
        public long ModifyBy { get; set; }
        public string ModifyByPersonalDetail { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        //-- Extra Added -------
        public int FranchiseID { get; set; }
    }
}
