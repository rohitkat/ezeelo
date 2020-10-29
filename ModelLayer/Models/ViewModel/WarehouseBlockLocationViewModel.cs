using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
   public class WarehouseBlockLocationViewModel
    {
       public long ColumnID { get; set; }
        public long LocationID { get; set; }
        public long WarehouseBlockLevelID { get; set; }
        public long AlphabeteID { get; set; }
        public string WarehouseZoneName { get; set; }
        public string BlockName { get; set; }
        public string LevelName { get; set; }
        public string LocationFullName { get; set; }
        public string LocationShortname { get; set; }
        public Nullable<Decimal> Height { get; set; }
        public Nullable<Decimal> Length { get; set; }
        public Nullable<Decimal> Width { get; set; }
        public Nullable<Decimal> Volume { get; set; }
        public int Status { get; set; }
        public string LocationStatus { get; set; }
        public long LastModifyBy { get; set; }
        public bool IsActive { get; set; }
        public int AvailableQty { get; set; }

    }
}
