using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
  public partial class PurchaseOrderReplyDetail
    {
        public long ID { get; set; }
        public long PurchaseOrderReplyID { get; set; }
        public long PurchaseOrderDetailID { get; set; }
        public long ProductID { get; set; }
        public Nullable<long> ProductVarientID { get; set; }
        public string ProductNickname { get; set; }
        public Boolean IsExtraItem { get; set; }       
        public int Quantity { get; set; }
        public decimal BuyRatePerUnit { get; set; }     
        public decimal MRP { get; set; }
        public decimal SaleRate { get; set; }
        public decimal Amount { get; set; }
        public Nullable<int> GSTInPer { get; set; }
        public decimal CGSTAmount { get; set; }
        public decimal SGSTAmount { get; set; }
        public decimal IGSTAmount { get; set; }       
        public string Remark { get; set; }
        public Boolean IsActive { get; set; }
        public virtual PurchaseOrderReply PurchaseOrderReply { get; set; }
    }
}
