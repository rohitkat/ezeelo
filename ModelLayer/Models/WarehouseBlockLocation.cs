using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModelLayer.Models
{
     [Table("WarehouseBlockLocation")]
   public class WarehouseBlockLocation
    { 
        [Key]
        public long ID { get; set; }
        public long WarehouseBlockLevelID { get; set; }
        public string Name { get; set; }
        public string LocationShortName { get; set; }
        public Decimal Height { get; set; }
        public Decimal Length { get; set; }
        public Decimal Width { get; set; }
        public Decimal Volume { get; set; }
        public int Status { get; set; }
        public long LastModifyBy { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }

        //[NotMapped]
        //public SelectList WarehouseBlockLocationList { get; set; }
       
    }
}
