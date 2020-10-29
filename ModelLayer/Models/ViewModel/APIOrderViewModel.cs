using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
  public class APIOrderViewModel
    {
      public int ID { get; set; }
      public string GcmRegID { get; set; }  //Added by Ashwini Meshram for Push Notification

      public APICustomerOrder CustomerOrder { get; set; }
      public List<APICustomerOrderDetails> CustomerOrderDetail { get; set; }
      public List<ShopWiseDeliveryCharges> shopWiseDeliveryCharges { get; set; }

      /*New Correction As Required 
      * Changes By :- Pradnyakar Badge
      * Dated      :- 14-12-2015
      * Suggested By Tesjwee and Sumeet
      */
      public Nullable<DateTime> ScheduleDate { get; set; }
      public Nullable<int> ScheduleID { get; set; }

      //save earn used amount details
     // public EarnDetail lEarnDetail { get; set; } 
      public Nullable<decimal> UsedEarnAmount { get; set; }
      public Nullable<decimal> EarnAmount { get; set; }
      public Nullable<decimal> RemainingAmount { get; set; }
      public int? FranchiseId { get; set; }//Added By Sonali_12-11-2018
      public int? CartId { get; set; }//Added By Sonali_12-11-2018
      public string IsOnlinePayment { get; set; }//Added By Sonali_12-11-2018
    }
}
