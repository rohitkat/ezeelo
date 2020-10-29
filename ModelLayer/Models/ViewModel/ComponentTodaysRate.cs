using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    class ComponentTodaysRate
    {
        public int ComponentID { get; set; }
        public string ComponentName { get; set; }
        public long ShopID { get; set; }
        public int ComponentUnitID { get; set; }
        public string ComponentUnitName { get; set; }
        public decimal PerUnitRateInRs { get; set; }
        public decimal PerUnitRateInPer { get; set; }
        public int DependentOnComponentID { get; set; }
        public int DepComponentUnitID { get; set; }
    }
}
