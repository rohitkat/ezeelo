using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
   public class ExecelBankPayoutRequestViewModel
    {
        public string BenificiaryAccountNumber { get; set; }
        public string BenificiaryAccountName { get; set; }
        public decimal InstrumentAmount { get; set; }
        public string IFSC { get; set; }
        public string BenificiaryEmail { get; set; }
        public DateTime? TransactionDate { get; set; }
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

        public bool PaymentStatus { get; set; }
        public string PaymentRemark { get; set; }

       // public DateTime? CreateDate { get; set; }

    }
}
