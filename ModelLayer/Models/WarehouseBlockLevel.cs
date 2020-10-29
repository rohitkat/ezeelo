using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;


namespace ModelLayer.Models
{
     [Table("WarehouseBlockLevel")]
     public class WarehouseBlockLevel
    {
        public long ID { get; set; }
        public long WarehouseBlockID { get; set; }
        public string Name { get; set; }
        public int AlphabeteID { get; set; }
        public int Columns { get; set; }       

        [NotMapped]
        public SelectList WarehouseBlockLevelList { get; set; }
    }
}
