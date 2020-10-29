using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class PayoutFreezeDataReportViewModel
    {
        public long UserLoginId { get; set; }
        public string Name { get; set; }
        public string EmailId { get; set; }
        public string PhoneNo { get; set; }
        public decimal QRP { get; set; }
        public bool Status { get; set; }
        public int DelOrdCount { get; set; }
        public decimal TotalOrdAmt { get; set; }
        public decimal TotalRetailPoints { get; set; }
        public decimal ERP { get; set; }
        public decimal EzeeMoney { get; set; }
        public string OrdCode { get; set; }//Yashaswi 22-1-19
        public string TransID { get; set; }//Yashaswi 22-1-19

        public decimal TotalInActivePoints { get; set; } //amit
        public decimal InActivePoints { get; set; } //amit
        public decimal ActivePoints { get; set; } //amit


    }
}
