using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ModelLayer.Models.ViewModel
{
    public class InventoryReportViewModel
    {

        public long WarehouseID { get; set; }
        public string Warehouse { get; set; }
        public long SKUID { get; set; }
        public string Manifecturer { get; set; }

        public string SupplierName { get; set; }
        public string SKUUnit { get; set; }
        public string SKUName { get; set; }
        public int AvailableInStock { get; set; }
        public int ReorderLevel { get; set; }
        public long ProductVarient { get; set; }
        public double Amount { get; set; }
        public decimal BuyRatePerUnit { get; set; }
        public int InitialQuantity { get; set; }
        public string BatchCode { get; set; }
        //public long SupplierID { get; set; }
        public string InvoiceCode { get; set; }
        public Nullable<DateTime> ExpiryDate { get; set; }
        public Nullable<decimal> MRP { get; set; }
        public Nullable<decimal> SaleRate { get; set; }
        public int AvailableQuantity { get; set; }
        public decimal? RetailPoint { get; set; }     ////AS Buissness point
        public long PurchaseOrderID { get; set; }    //Added by Priti on 19-12-2018
        public long ProductID { get; set; }    //Added by Priti on 19-12-2018



        [Display(Name = "Terms and Conditions")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "You gotta tick the box!")]
        public bool IsSelected { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime OrderDate { get; set; }
        public string Category1 { get; set; }
        public string Category2 { get; set; }
        public string Category3 { get; set; }
        public int Category1CategoryID { get; set; }
        public int Category2CategoryID { get; set; }
        public int Category3CategoryID { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }

        public int SaleQty { get; set; }
        public decimal? SaleValue { get; set; }
        public decimal InventoryValue { get; set; }
        public long InvoiceID { get; set; } /// Added by Priti on 3-1-2
    public int ReturnQuantity { get; set; }/// Added by Priti on 5-1-2019
        public string Status { get; set; }/// Added by Priti on  5-1-2019
        public string Entity { get; set; }  // by Priti
       public bool IsFulfillmentCenter { get; set; }// Added by Priti 
       public string Franchises { get; set; }// Added by Priti 
        public string City { get; set; }// Added by Priti 
    }
    public class InventoryReportViewModelList
    {
        public List<InventoryReportViewModel> lInventoryReportViewModel = new List<InventoryReportViewModel>();
        public long SupplierID { get; set; }

        public bool IsSelected { get; set; }
        public long WarehouseID { get; set; }
        public string WarehouseName { get; set; }
        public bool Checkbox { get; set; }

        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public SelectList WarehouseList { get; set; }
    }
}

