using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class AssignedTrackOrderDelivery
    {
       /* public int SrNo { get; set; }
        public string Particular { get; set; }
        public string Weight { get; set; }
        public decimal MRP { get; set; }
        public decimal Rate { get; set; }
        public int Qty { get; set; }
        public decimal Saving { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMode { get; set; }*/

        public decimal TotalSaving { get; set; }
        public int TotalItem { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal CouponAmount { get; set; }
        public decimal DeliveryCharges { get; set; }
        public decimal BillAmount { get; set; }
        public string PaymentMode { get; set; }

       public List<AssignedDeliveryOrderList> AssignedDeliveryOrderList = new List<AssignedDeliveryOrderList>();

    }
}
