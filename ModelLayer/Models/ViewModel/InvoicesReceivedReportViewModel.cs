using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ModelLayer.Models.ViewModel
{
    public class InvoicesReceivedReportViewModel
    {
        public long WarehouseID { get; set; }


        public string SKUName { get; set; }
        public long SKUID { get; set; }
        public string SKUUnit { get; set; }

        public string InvoiceCode { get; set; }
        public string Supplier { get; set; }
        //public string Buyer { get; set; }
        public string PONumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime InvoicePaidDate { get; set; }
        public int TotalTax { get; set; }
        public string Status { get; set; }
        public DateTime CreateDate { get; set; }
        public decimal DebitNoteAmount { get; set; }
        public decimal CreditNoteAmount { get; set; }
        public Decimal Amount { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string HSNCode { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public Nullable<int> GSTInPer { get; set; }////Added by Priti
        public Nullable<decimal> GSTAmount { get; set; }////Added by Priti
        public Nullable<decimal> SGSTAmount { get; set; }////Added by Priti
        public Nullable<decimal> CGSTAmount { get; set; }////Added by Priti
        public Decimal? MRP { get; set; }////Added by Priti 
        public string Entity { get; set; }  // by Priti
        public bool IsFulfillmentCenter { get; set; }// Added by Priti 
        public string Franchises { get; set; }// Added by Priti 
        public string City { get; set; }// Added by Priti 
        public long SupplierID { get; set; }// Added by Priti 

    }
    public class InvoicesReceivedReportViewModelList
    {
        public List<InvoicesReceivedReportViewModel> lInvoicesReceivedReportViewModel = new List<InvoicesReceivedReportViewModel>();
        public long SupplierID { get; set; }

        public string SupplierName { get; set; }
        public long WarehouseID { get; set; }
        public string WarehouseName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public SelectList WarehouseList { get; set; }
        public DateTime Fdate { get; set; }
        public DateTime TDate { get; set; }
    }
}






