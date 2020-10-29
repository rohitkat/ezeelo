using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    // addded by amit on 23-8-18
   public class LeadersPayoutRequestViewModel
    {
       //Bank Details
        public long UserLoginID { get; set; }
       // public string FullName { get; set; }

        public string AccountHolderName { get; set; }

        public string AdhaarNo { get; set; }
        public string PanNo { get; set; }
        public int BankID { get; set; }
        public string AccountNo { get; set; }
        public string BankIFSC { get; set; }
        public string BranchName { get; set; }
        public string AccountType { get; set; }

        public string BankName { get; set; }

        public string BenificiaryName { get; set; }
        
        public Decimal? RequestedAmount { get; set; }
        public Decimal? GST { get; set; }
        public Decimal? GSTAmount { get; set; }
        public Decimal? TDS { get; set; }

        public Decimal? TDSAmount { get; set; }
        public Decimal? MinReserved { get; set; }
        public Decimal?  MinReservedAmount { get; set; }
        public Decimal?  ProcessingFees { get; set; }
        public Decimal? Penalty { get; set; }
        public Decimal?  PenaltyAmount { get; set; }

        public Decimal? TotalAmount { get; set; }
        public Decimal? EzeeloWalletCash { get; set; }
        public Decimal? RedeamableCash { get; set; }

        public DateTime? CreateDate { get; set; }

        public int? RequestedStatus { get; set; }

        public string Status { get; set; }

        public string Email { get; set; }

        public string BeneficiaryEmail { get; set; }  // on 13-2-19

        public bool IsActive { get; set; }

        public int RequestID { get; set; }

        public string TransactionID { get; set; }

        public string Mobile { get; set; }
        public string isAllowed { get; set; }
        public string Name { get; set; }
        public Decimal? ProcessingFeesAmount { get; set; }
    }
    
}
