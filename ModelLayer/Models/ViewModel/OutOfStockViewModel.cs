using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ModelLayer.Models.ViewModel
{
    public class OutOfStockViewModel
    {
        public long WarehouseID { get; set; }
        public string SKUName { get; set; }
        public string SKUUnit { get; set; }
        public long SKUID { get; set; }
        public string Manifecturer { get; set; }
        public string SupplierName { get; set; }
        public string HSNCode { get; set; }
        public int AvailableQuantity { get; set; }
        public string BrandName { get; set; }
        public string Supplier { get; set; }
        public DateTime? UsualDeliveryTime { get; set; }

        // public DateTime OutofStockDate { get; set; }
        public int MaxStockLevel { get; set; }
        public int ReorderLevel { get; set; }
        public DateTime? ReorderHitDate { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public DateTime CreateDate { get; set; }
        public int InitialQuantity { get; set; }   // Added by Priti on 17-11-2018
        public DateTime? OutOfStockDate { get; set; }// Added by Priti on 17-11-2018
        public string BatchCode { get; set; }// Added by Priti on 17-11-2018
        public string Category1 { get; set; }// Added by Priti on 17-11-2018
        public string Category2 { get; set; }// Added by Priti on 17-11-2018
        public string Category3 { get; set; }// Added by Priti on 17-11-2018
        public Nullable<decimal> MRP { get; set; }// Added by Priti on 17-11-2018
        public Nullable<decimal> SaleRate { get; set; }// Added by Priti on 17-11-2018

        public decimal? RetailPoint { get; set; }     ////AS Buissness point // Added by Priti on 17-11-2018

        public Nullable<DateTime> ExpiryDate { get; set; }// Added by Priti on 17-11-2018
        public double Amount { get; set; }// Added by Priti on 17-11-2018
        public decimal BuyRatePerUnit { get; set; }// Added by Priti on 17-11-2018
        public string InvoiceCode { get; set; }// Added by Priti on 17-11-2018
        public int AvailableInStock { get; set; }// Added by Priti on 17-11-2018
        public string Entity { get; set; }  // by Priti
        public bool IsFulfillmentCenter { get; set; }// Added by Priti 
        public string Franchises { get; set; }// Added by Priti 
        public string City { get; set; }// Added by Priti 
        public long SupplierID { get; set; }// Added by Priti 
        //public string Entity { get; set; }// Added by Priti 

        //public bool IsFulfillmentCenter { get; set; }// Added by Priti 
        //public string Franchises { get; set; }// Added by Priti 
        public int? SoldQty { get; set; }//Added by Rumana

    }
    public class OutOfStockViewModelList
    {
        public List<OutOfStockViewModel> lOutOfStockViewModel = new List<OutOfStockViewModel>();
        public long SupplierID { get; set; }
        public string SupplierName { get; set; }
        public long WarehouseID { get; set; }
        public string WarehouseName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public SelectList WarehouseList { get; set; }
        [NotMapped]
        public bool IsChecked { get; set; }
        public List<OutOfStocKReportViewModelOnPlaced> lOutofStockReportViewModel = new List<OutOfStocKReportViewModelOnPlaced>();//Added by Rumana
    }
}




