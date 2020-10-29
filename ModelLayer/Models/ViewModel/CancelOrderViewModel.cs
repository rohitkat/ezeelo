using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class CancelOrderViewModel
    {
        public long OrderID { get; set; }
        public string OrderNo { get; set; }
        public long ShopStockID { get; set; }
        public string Description { get; set; }
    }
}
