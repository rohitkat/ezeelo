using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
   public class QuotationReplyItem
    {
       public long ID { get; set; }
       public long QuotationSupplierListID { get; set; }
       public long QuotationID { get; set; }
       public long ReplyFromSupplierID { get; set; }
       public long ReplyFromWarehouseID { get; set; }
       public long ProductID { get; set; }
       public string ProductNickname { get; set; }
       public long ProductVarientID { get; set; }
       public int Quantity { get; set; }
       public decimal UnitPrice { get; set; }
       public decimal MRP { get; set; }
       public decimal SaleRate { get; set; }
       public decimal CGSTAmount { get; set; }
       public decimal SGSTAmount { get; set; }
       public decimal IGSTAmount { get; set; }
       public decimal Amount { get; set; }
       public string Remark { get; set; }
    }
}
