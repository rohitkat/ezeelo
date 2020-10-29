using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModelLayer.Models.ViewModel
{
   public class WarehouseBlockViewModel
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public long LevelID { get; set; }
        public string LevelName { get; set; }
        public string PalletName { get; set; }
        public int TotalLocations { get; set; }
        public long AlphabeteID { get; set; }

       [Required(ErrorMessage="Please select Warehouse zone")]
        public long WarehouseZoneID { get; set; }       
        public string ZoneName { get; set; }
        public int StorageTypeID { get; set; }
       
        //[Range(1,100,ErrorMessage="Value must be between 1 to 100")]
        public int Rows { get; set; }
        public bool IsPallet { get; set; }
        public int Columns { get; set; }
        public Nullable<Decimal> Height { get; set; }
        public Nullable<Decimal> Length { get; set; }
        public Nullable<Decimal> Width { get; set; }
        public Nullable<Decimal> Weight { get; set; }
        public WarehouseBlock lWarehouseBlock { get; set; }
        public List<WarehouseBlockLevel> lWarehouseBlockLevel { get; set; }
        public List<WarehouseBlockLocation> lWarehouseBlockLocations { get; set; }
       
    }
}
