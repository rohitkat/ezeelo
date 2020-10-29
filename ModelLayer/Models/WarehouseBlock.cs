using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace ModelLayer.Models
{
     [ Table("WarehouseBlock")]
  public   class WarehouseBlock
    {
    [Key]
        public long ID { get; set; }
        public long WarehouseZoneID { get; set; }
        public string Name { get; set; }
        public int Rows { get; set; }
        public bool IsPallet { get; set; }
        public int Columns { get; set; }
        public Decimal Height { get; set; }
        public Decimal Length { get; set; }
        public Decimal  Width { get; set; }
        public Decimal Weight { get; set; }
        public DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        public long? ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        [NotMapped]
        public SelectList WarehouseBlockList { get; set; }  

    }
}
