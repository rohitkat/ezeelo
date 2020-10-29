using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models.ViewModel
{
   public class PurchaseOrderReplyDetailViewModel
    {
        public long PurchaseOrderReplyDetailsID { get; set; }
        public long SupplierID { get; set; }
        public long PurchaseOrderReplyID { get; set; }
        public long PurchaseOrderDetailID { get; set; }
        public long WarehouseStockID { get; set; }
        public long ProductID { get; set; }
        public long ProductVarientID { get; set; }
        public string ProductNickname { get; set; }
        public string ItemName { get; set; }
        public string HSNCode { get; set; }
        public int RequiredQuantity { get; set; }

        [Required(ErrorMessage = "Please enter Quantity")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Please enter Rate Per Unit")]
        public decimal BuyRatePerUnit { get; set; }
        public decimal MRP { get; set; }
        public decimal SaleRate { get; set; }
        public decimal ProductAmount { get; set; }
        public decimal Amount { get; set; }
        public int ReorderLevel { get; set; }
        public int AvailableQuantity { get; set; }
        public int TempQuantity { get; set; }
        public string ProductRemark { get; set; }
        public string StockThumbPath { get; set; }       
        public Boolean IsExtraItem { get; set; }             
      
        public decimal OrderAmount { get; set; }
        public Nullable<int> GSTInPer { get; set; }
        public Nullable<decimal> CGSTAmount { get; set; }
        public Nullable<decimal> SGSTAmount { get; set; }
        public Nullable<decimal> IGSTAmount { get; set; }
        public long LocationID { get; set; }
        public string Location { get; set; }
        public string BatchCode { get; set; }
        //public decimal? TaxableValue { get; set; }///Added by Priti
        public Nullable<decimal> TotalGst { get; set; }///Added by Priti      
        public decimal TaxableValue { get; set; }///Added by Priti   
       //Priti
        public string EANCode { get; set; }        ///Added by Priti    
        public long SKUID { get; set; }           ///Added by Priti
        public string SKUUnit { get; set; }    ///Added by Priti
        public Nullable<decimal> GSTAmount { get; set; }////Added by Priti
        public decimal DiscountInPer { get; set; }    ///Added by Priti  
        public Nullable<decimal> DiscountAmount { get; set; }////Added by Priti
        public decimal UnitPrice { get; set; }   ////Added by Priti
        public decimal BasicCost { get; set; }     ///Added by Priti  
        public long BrandID { get; set; }    ///Added by Priti  
        public string CreateByName { get; set; }    ///Added by Priti  
        public string ModifyByName { get; set; }    ///Added by Priti  
        public string BrandName { get; set; }    ///Added by Priti
    }
}
