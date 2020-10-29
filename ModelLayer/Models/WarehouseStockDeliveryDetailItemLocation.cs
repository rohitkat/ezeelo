using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModelLayer.Models
{
      [Table("WarehouseStockDeliveryDetailItemLocation")]
   public class WarehouseStockDeliveryDetailItemLocation
    {
        [Key]
        public long ID { get; set; }
        public long WarehouseStockDeliveryDetailID { get; set; }
        public long ItemLocationID { get; set; }
        public int Quantity { get; set; }
    }
}
