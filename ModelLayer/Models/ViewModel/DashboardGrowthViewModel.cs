using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
     public class DashboardGrowthViewModel
     {
         public int ID { get; set; }
         public string Name { get; set; }
         public int OrderTarget { get; set; }
         public int ActualOrder { get; set; }
         public int MonthlyOrderTarget { get; set; }
         public string ShortAccessOrder { get; set; }
         public int GMVTarget { get; set; }
         public string ActualGMV { get; set; }
         public string MonthlyGMVTarget { get; set; }
         public string ShortAccessGMV { get; set; }
         public string Date { get; set; }
         public int? FromMonth { get; set; }
         public int? ToMonth { get; set; }
         public int? Year { get; set; }
         public int AvgOrder { get; set; }
         public int AvgGMV { get; set; }
         public int AvgOrderTarget { get; set; }
         public int AvgGMVTarget { get; set; }
         public int? FranchiseID { get; set; }
        // public int? CumulativeOrderTarget { get; set; }
         public string CumulativeOrderTarget { get; set; }
         public string CumulativeActualOrder { get; set; }
         public string CumulativeGMVTarget { get; set; }
         public string CumulativeActualGMV { get; set; }
         public int? RunningOrderTarget { get; set; }
         public int RunningActualOrder { get; set; }
         public int RunningGMVTarget { get; set; }
         public int RunningActualGMV { get; set; }
         public int ShortAccessGMVMCO { get; set; }
         public int ShortAccessOrderMCO { get; set; }
         public int GMVTargetMCO { get; set; }
         public int ActualGMVMCO { get; set; }
    }

    
}
