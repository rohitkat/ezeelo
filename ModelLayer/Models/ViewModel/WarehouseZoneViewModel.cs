using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
  public  class WarehouseZoneViewModel
    {
      public long ZoneID { get; set; }
      public long WarehouseZoneID { get; set; }
      public string Zone { get; set; }
      public string Abbreviation { get; set; }
    }
}
