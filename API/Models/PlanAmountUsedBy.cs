using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class PlanAmountUsedBy
    {
        public long UserLoginId { get; set; }
        public long CustOrderId { get; set; }
        public List<OrderDetailsCartShopStockList> OrderDetailsCartShopStockList { get; set; }
        public List<SubscribedFacilityList> SubscribedFacilityList { get; set; }
    }
    public class OrderDetailsCartShopStockList
    {
        public long shopStockId { get; set; }

        public int Quantity { get; set; }
    }
    public class SubscribedFacilityList
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public int BehaviorType { get; set; }
        public decimal FacilityValue { get; set; }
    }
}