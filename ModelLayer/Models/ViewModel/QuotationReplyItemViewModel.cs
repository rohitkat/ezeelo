using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
   public class QuotationReplyItemViewModel
    {
        public long QuotationSupplierListID { get; set; }
        public long QuotationID { get; set; }
        public long ReplyFromSupplierID { get; set; }
        public long ReplyFromWarehouseID { get; set; }
        public long ProductID { get; set; }
        public long ProductVarientID { get; set; }
        public string Nickname { get; set; }
        public string ItemName { get; set; }
        public string HSNCode { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public Nullable<decimal> SGSTAmount { get; set; }
        public Nullable<decimal> CGSTAmount { get; set; }
        public Nullable<decimal> IGSTAmount { get; set; }
        public decimal ProductAmount { get; set; }
        public string ProductRemark { get; set; }
        public string StockThumbPath { get; set; }       
    }
}
