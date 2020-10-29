using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class ShopProductComponentViewModel
    {
        public int ComponentID { get; set; }
        public string ComponentName { get; set; }
        public Nullable<decimal> ComponentWeight { get; set; }
        public Nullable<int> ComponentUnitID { get; set; }
        public string ComponentUnitName { get; set; }
        
        public int UnitID { get; set; }
        public int Quantity { get; set; }
        public decimal PerUnitRateInRs { get; set; }
        public decimal PerUnitRateInPer { get; set; }
        public string DependentComponentName { get; set; }
        public decimal ComponentRate { get; set; }
        public long ShopStockID { get; set; }
        public long ProductVarientID { get; set; }
        public Nullable<int> DependentOnComponentID { get; set; }
    }
}
