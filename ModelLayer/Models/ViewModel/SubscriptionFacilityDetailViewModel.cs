using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
   public class SubscriptionFacilityDetailViewModel
    {
        public long ID { get; set; }
        public string Facility { get; set; }
        public string SubscriptionPlan { get; set; }
        public decimal Fees { get; set; }
        public int NoOfDays { get; set; }
        public int NoOfDaysRemain { get; set; }
        public int NoOfCoupens { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public Nullable<bool> IsActive { get; set; }

        public int CurrentPlanID { get; set; }
        public int TotalFreeDelivery { get; set; }
        public int BalanceFreeDelivery { get; set; }
        public decimal TotalSavingPerMonth { get; set; }
        public decimal TotalPurchasePerMonth { get; set; }
        public decimal PurchaseAsPerMonth { get; set; }

    }
}
