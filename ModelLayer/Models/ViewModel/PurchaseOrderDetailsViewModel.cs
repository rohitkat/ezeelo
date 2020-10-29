using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
   public class PurchaseOrderDetailsViewModel
    {
       public long PurchaseOrderDetailsID { get; set; }
       public long SupplierID { get; set; }
       public long PurchaseOrderID { get; set; }
       public long ProductID { get; set; }
       public long ProductVarientID { get; set; }
       public string Nickname { get; set; }
       public string ItemName { get; set; }
       public string HSNCode { get; set; }
       public int Quantity{get;set;}
       public int AvailableQuantity { get; set; }
       public decimal UnitPrice { get; set; }
       public long SKUID { get; set; }////Added by Priti
       public decimal GSTInPer { get; set; }////Added by Priti
       public Nullable<decimal> GSTAmount { get; set; }////Added by Priti
       public Nullable<decimal> CGSTAmount { get; set; }////Added by Priti
       public Nullable<decimal> SGSTAmount { get; set; }////Added by Priti
       public double? MRP { get; set; }   ///Added by Priti 
       public long BrandID { get; set; }  ///Added by Priti 
       public string BrandName { get; set; }///Added by Priti 
       public decimal POValue { get; set; }////Added by Priti

       public decimal BasicCost { get; set; }     ///Added by Priti   
       public decimal DiscountInPer { get; set; }    ///Added by Priti   
       public Nullable<decimal> DiscountAmount { get; set; }////Added by Priti
       public string CreateByName { get; set; }////Added by Priti
       public string ModifyByName { get; set; }////Added by Priti   

       public decimal TotalAmount { get; set; }  ///Added by Priti 

       public string EANCode { get; set; }
       public decimal TaxableValue { get; set; }///Added by Priti  
       public decimal ProductAmount { get; set; }
       public string ProductRemark { get; set; }
       public string StockThumbPath { get; set; }
       public Nullable<long> RateCalculationID { get; set; }
        public long? RateMatrixExtensionId { get; set; } //Added by Yashaswi 10-4-2019
        public string SKUUnit { get; set; }
       public DateTime? ModifiedDate { get; set; }

        //add by Priti
        public double? DecidedSalePrice { get; set; }
        public double? Margin { get; set; }
    

    }
}
