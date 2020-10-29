using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModelLayer.Models
{
    [Table("WarehouseStockDeliveryDetail")]
    public partial class WarehouseStockDeliveryDetail
    {
        [Key]
        public long ID { get; set; }
        public long PurchaseOrderReplyDetailID { get; set; }
        public long WarehouseStockID { get; set; }
        public int Quantity { get; set; }
        public double? ClaimAmountPerUnit { get; set; }
        public bool? IsReplied { get; set; } //Added on 05-07-2018


    }
}
