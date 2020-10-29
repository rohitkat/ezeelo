using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class MerchantAppOrderDetailsViewModel
    {
        public long ID { get; set; }
        public string ProductName { get; set; }
        public string ColorName { get; set; }
        public string SizeName { get; set; }
        public string DimensionName { get; set; }
        public string MaterialName { get; set; }
        public int Qty { get; set; }
        public decimal SaleRate { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
