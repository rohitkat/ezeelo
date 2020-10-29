using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ModelLayer.Models.ViewModel
{
    public class ReturnReportViewModel
    {
        public long ID { get; set; }
        public long WarehouseID { get; set; }
        public long SKUID { get; set; }
        public long WarehouseStockID { get; set; }
        public long WarehouseReturnStockId { get; set; }
        public string item_name { get; set; }
        [Display(Name = "Total Product Quantity In Stock")]
        public long ProductID { get; set; }
        public decimal ReturnRatePerUnit { get; set; }
        [Display(Name = "Return Rate Per Unit")]
        public long ProductVarientID { get; set; }
        public string ItemName { get; set; }
        public string HSNCode { get; set; }
        public int ReturnQuantity { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Please select Supplier name")]
        public long SupplierID { get; set; }
        public string SupplierName { get; set; }
        public string SubReason { get; set; }
        public string Reason { get; set; }
        public string ProductVarient { get; set; }
        public DateTime CreateDate { get; set; }
        public long ReasonId { get; set; }
        public long SubReasonId { get; set; }
        public string Brand { get; set; }
        public int BatchAvlQty { get; set; }
        public string Remark { get; set; }
        public string Manufacturer { get; set; }
        public string batch_code { get; set; }
        [Display(Name = "Buy Rate Per Unit")]
        public DateTime ReturnDate { get; set; }
        public string InvoiceCode { get; set; }

        public string TotalItems { get; set; }


        public int return_qty { get; set; }
        [Display(Name = "Total Amount")]
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        //public SelectList ReasonList { get; set; }
        //public SelectList SubReasonList { get; set; }
        public string Entity { get; set; }  // by Priti
        public bool IsFulfillmentCenter { get; set; }// Added by Priti 
        public string Franchises { get; set; }// Added by Priti 
        public string City { get; set; }// Added by Priti 
        public int GST { get; set; }

    }


    public class ReturnReportViewModelList
    {
        public List<ReturnReportViewModel> lReturnReportViewModel = new List<ReturnReportViewModel>();
        public long SupplierID { get; set; }

        public string SupplierName { get; set; }
        public long? WarehouseID { get; set; }
        public string WarehouseName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public SelectList WarehouseList { get; set; }
    }
}
