using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    // Added by Amit for OrderDetails
   public class LeadersOrderDetailsViewModel
    {
        public long ProductID { get; set; }
        public string OrderCode { get; set; }
        public int Qty { get; set; }
        public Decimal? MRP { get; set; }
        public Decimal? DiscountAmount { get; set; }
        public Decimal? Amount { get; set; }
        public Decimal? TotalAmount { get; set; }
        public Decimal? BusinessPointsTotal { get; set; }
        public long sku { get; set; }

        public string ItemImage { get; set; }
        public string ItemDescription { get; set; }

        public int OrderStatus { get; set; }  // added by amit on 7-2-19
        public string OrdStatus { get; set; } // added by amit on 7-2-19





    }
}
