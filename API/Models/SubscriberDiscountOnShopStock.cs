using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class SubscriberDiscountOnShopStock
    {
        public long UserLoginId { get; set; }
        public List<SubscriberDiscountOnShopStockList> SubscriberDiscountOnShopStockList { get; set; }


    }
    public class SubscriberDiscountOnShopStockList
    {
        public long shopStockId { get; set; }

        public int Quantity { get; set; }
    }
}