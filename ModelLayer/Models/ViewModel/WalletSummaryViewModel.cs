using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    // Added by amit on 19/9/18
   public class WalletSummaryViewModel
    {
        public long UserLoginID { get; set; }
        public string Description { get; set; }
        public string WidrawlCreadit { get; set; }
        public string Status { get; set; }
        public Decimal Amount { get; set; }
        public DateTime? TransactionDate { get; set; }

        public String OrderCode { get; set; } // added on 24-9-18

        public long TransactionTypeID { get; set; } 


    }
}
