using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class SubscribedDiscountOnCategoryViewModel
    {
        public long RootLevelCategoryId { get; set; }
        public decimal Amount { get; set; }
        public decimal Percent { get; set; }

    }

    public class OrderDetailsCartShopStock
    {
        public long shopStockId { get; set; }

        public int Quantity { get; set; }
    }
}
