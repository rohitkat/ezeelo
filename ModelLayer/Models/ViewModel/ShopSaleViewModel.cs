using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class ShopSaleViewModel
    {
        public long CustomerOrderID { get; set; }
        public string CustomerOrderCode { get; set; }
        public string ShopName { get; set; }
        public long ShopID { get; set; }
        public string Address { get; set; }
        public bool IsDeliveryOutSource { get; set; }
        public string ContactPerson { get; set; }
        public string NearestLandmark { get; set; }
        public TimeSpan? OpeningTime { get; set; }
        public TimeSpan? ClosingTime { get; set; }
        public string WeeklyOff { get; set; }
        public bool IsFreeHomeDelivery { get; set; }
        public decimal MinimumAmountForFreeDelivery { get; set; }
        public bool IsLive { get; set; }
        public int TotalOrder { get; set; }
        public DateTime OrderedDate { get; set; }
    }
}
