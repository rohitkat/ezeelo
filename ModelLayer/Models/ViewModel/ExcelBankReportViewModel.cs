using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
   public class ExcelBankReportViewModel
    {
        public long UserLoginID { get; set; }  // on 22-1-19
        public string Name { get; set; }
        public string Mobile { get; set; }
       
        public string EmailID { get; set; }

        public string BenificiaryAccountNumber { get; set; }
        public string BenificiaryAccountName { get; set; }
        public int RequestID { get; set; }
        public decimal PaidAmount { get; set; }

        public string IFSC { get; set; }
        public string BenificiaryEmail { get; set; }
        public string TransactionDate { get; set; }  // on 23-1-19
       
       
        public string BenificiaryBankName { get; set; }

        public bool? PaymentStatus { get; set; }
        public string PaymentRemark { get; set; }

        public DateTime? CreateDate { get; set; }


       // public int? RequestedStatus { get; set; }  // on 2-1-19

        public string StatusPayment { get; set; } // on 2-1-19
        public string TransactionID { get; set; }
       
       
        // public DateTime CreateDate { get; set; }

        





    }
}
