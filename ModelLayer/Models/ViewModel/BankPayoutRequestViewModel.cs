using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
   public class BankPayoutRequestViewModel
    {
        public long UserLoginID { get; set; }  // on 22-1-19

        public string BenificiaryAccountNumber { get; set; }
        public string BenificiaryAccountName { get; set; }
        public decimal InstrumentAmount { get; set; }
        public string IFSC { get; set; }
        public string BenificiaryEmail { get; set; }
        public string TransactionDate { get; set; }  // on 23-1-19
        public string TransactionType { get; set; }
        public string InfoToBenificiary { get; set; }
        public string DebitAccountNarration { get; set; }
        public string PaymentDetails1 { get; set; }
        public string PaymentDetails2 { get; set; }
        public string PaymentDetails3 { get; set; }
        public string PaymentDetails4 { get; set; }
        public string PaymentDetails5 { get; set; }
        public string PaymentDetails6 { get; set; }
        public string PaymentDetails7 { get; set; }
        public string BenificiaryBankName { get; set; }

        public bool?  PaymentStatus { get; set; }
        public string PaymentRemark { get; set; }

        public DateTime? CreateDate { get; set; }


        public int? RequestedStatus { get; set; }  // on 2-1-19

        public string StatusPayment { get; set; } // on 2-1-19

        public string TransactionID { get; set; }

        public int? RequestID { get; set; }  // on 14-2-19

        public DateTime? RequestStatus_Date { get; set; }

        public String FullName { get; set; }
        public String Email { get; set; }

        public string Mobile { get; set; }









    }
}
