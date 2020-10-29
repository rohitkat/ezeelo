using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ModelLayer.Models.ViewModel
{
     public class ProductDetailsSPViewModel
    {

        public long WarehouseID { get; set; }
        public string Warehouse { get; set; }
        public long SKUID { get; set; }
        public string Manifecturer { get; set; }

        public string SupplierName { get; set; }
        public string SKUUnit { get; set; }
        public string SKUName { get; set; }
        public int  AvailableInStock { get; set; }
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
        public long? RetailPoint { get; set; }     ////AS Buissness point 
        public decimal BusinessPoints { get; set; }



        [Display(Name = "Terms and Conditions")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "You gotta tick the box!")]
        public bool IsSelected { get; set; }
    
        public DateTime CreateDate { get; set; }
        public DateTime OrderDate { get; set; }
     
        public string FromDate { get; set; }
        public string ToDate { get; set; }

        public int SaleQty { get; set; }
        public decimal? SaleValue { get; set; }
        public decimal InventoryValue { get; set; }
         public string item_image_Path { get; set; } /// Added for img on 23-11-2018
         public long ProductId { get; set; }/// Added for img on 23-11-2018
         public decimal? DiscountPer { get; set; }/// Added for img on 23-11-2018
      
    }
     public class ProductDetailsSPViewModelList
        {
         public List<ProductDetailsSPViewModel> lProductDetailsSPViewModel = new List<ProductDetailsSPViewModel>();
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

