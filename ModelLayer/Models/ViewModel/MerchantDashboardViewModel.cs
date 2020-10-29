using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class MerchantDashboardViewModel
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string ShopLogo { get; set; }
        public System.DateTime OpeningTime { get; set; }
        public System.DateTime ClosingTime { get; set; }
        public int PLACED { get; set; }
        public decimal Review { get; set; }

    }
}
