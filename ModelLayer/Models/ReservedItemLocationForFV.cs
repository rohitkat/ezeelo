using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModelLayer.Models
{
     [Table("ReservedItemLocationForFV")]
    public class ReservedItemLocationForFV
    {
        [Key]
        public long ID { get; set; }
        public long CustomerOrderDetailID { get; set; }
        public long ItemLocationID { get; set; }
        public int ReservedQuantity { get; set; }
        public int OrderStatus { get; set; }
    }
}
