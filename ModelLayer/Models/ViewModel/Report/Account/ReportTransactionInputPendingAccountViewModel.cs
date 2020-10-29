using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel.Report.Account
{
    public class ReportTransactionInputPendingAccountViewModel
    {
        public long CustomerOrderID { get; set; }
        public string OrderCode { get; set; }
        public long ReferenceCustomerOrderID { get; set; }
        public decimal OrderAmount { get; set; }
        public string PaymentMode { get; set; }
        public decimal PayableAmount { get; set; }
        public DateTime CreateDate { get; set; }
        // Extra Added -------
        public int FranchiseID { get; set; }
    }
}
