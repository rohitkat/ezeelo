using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
   public class WarehouseBlockLevelViewModel
    {
       public long WarehouseBlockLevelID { get; set; }
        public long WarehouseBlockID { get; set; }
        public int AlphabeteID { get; set; }
        public string Name { get; set; }
        public int Columns { get; set; }
    }
}
