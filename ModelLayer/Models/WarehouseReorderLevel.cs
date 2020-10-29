using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
   public class WarehouseReorderLevel
    {
        public long ID { get; set; }
        public long WarehouseID { get; set; }
        public long ProductID { get; set; }
        public long ProductVarientID { get; set; }
        public int AvailableQuantity { get; set; }
        public int ReorderLevel { get; set; }
        public DateTime? ReorderHitDate { get; set; }  //Added by Priti 

        public static bool Status { get; set; }

    }
}
