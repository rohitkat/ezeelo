using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models.ViewModel
{
   public class InvoiceDetailViewModel
    {
        public long InvoiceDetailID { get; set; }
        public long PurchaseOrderDetailsID { get; set; }
        public long SupplierID { get; set; }
        public long PurchaseOrderID { get; set; }
        public long ProductID { get; set; }
        public long ProductVarientID { get; set; }
        public string Nickname { get; set; }
        public string ItemName { get; set; }
        public string HSNCode { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

       [Required(ErrorMessage = "Sale Rate is Requied!")]
        public Nullable<decimal> SaleRate { get; set; }

       [Required(ErrorMessage = "MRP is Requied!")]
        public Nullable<decimal> MRP { get; set; }
        public decimal ProductAmount { get; set; }
        public string ProductRemark { get; set; }
        public string StockThumbPath { get; set; }
        public decimal BuyRatePerUnit { get; set; }
        public Boolean IsExtraItem { get; set; }

        [Required(ErrorMessage = "Please enter Receieved Quantity")]
        public Nullable<int> ReceivedQuantity { get; set; }

       [Required(ErrorMessage = "Please enter Reorder Level")]
        public int ReorderLevel { get; set; }
        public decimal Amount { get; set; }
        public decimal OrderAmount { get; set; }
        public Nullable<int> GSTInPer { get; set; }
        public Nullable<decimal> CGSTAmount { get; set; }
        public Nullable<decimal> SGSTAmount { get; set; }
        public Nullable<decimal> IGSTAmount { get; set; }       
        public string Remark { get; set; }
        public Nullable<DateTime> ExpiryDate { get; set; }
        public bool IsItemExistsInShop { get; set; }
        public long ShopStockID { get; set; }
        public int ItemsAllotedToLocation { get; set; }
        public decimal TaxableValue { get; set; }///Added by Priti  
        public List<AutoSearchWarehouseLocation> lAutoSearchWarehouseLocations { get; set; }

    }
}
