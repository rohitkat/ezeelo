using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModelLayer.Models
{
      [Table("ItemLocation")]
   public class ItemLocation
    {
        [Key]
        public long ID { get; set; }
        public long InvoiceDetailID { get; set; }
        public Nullable<long> WarehouseStockID { get; set; }
        public long WarehouseBlockLocationID { get; set; }
        public int InitialQuantity { get; set; }
        public int AvailableQuantity { get; set; }
        public long CreateBy { get; set; }
        public Nullable<DateTime> CreateDate{get; set;}
        public long ModifyBy { get; set; }
        public Nullable<DateTime> ModifyDate { get; set; }
    }
}
