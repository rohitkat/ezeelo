using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class PayablePricesViewModel
    {
        public decimal FinalAmount { get; set; }
        public decimal SavedRs { get; set; }
        public decimal SavedPercent { get; set; }
    }
}
