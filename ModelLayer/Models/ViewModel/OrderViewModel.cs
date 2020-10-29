//-----------------------------------------------------------------------
// <copyright file="OrderViewModel.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{

  public class OrderViewModel
  {
      public int ID { get; set; }
      public CustomerOrder CustomerOrder { get; set; }
      public List<CustomerOrderDetail> CustomerOrderDetail { get; set; }
      public List<ShopWiseDeliveryCharges> shopWiseDeliveryCharges { get; set; }

      /*New Correction As Required 
      * Changes By :- Pradnyakar Badge
      * Dated      :- 14-12-2015
      * Suggested By Tesjwee and Sumeet
      */
      public Nullable<DateTime> ScheduleDate { get; set; }
      public Nullable<int> ScheduleID { get; set; }


      /*Tax on order*/
      //public List<TaxOnOrder> lTaxOnOrder { get; set; }
      public List<CalulatedTaxesRecord> lCalulatedTaxesRecord { get; set; }


      //save earn used amount details
      public EarnDetail lEarnDetail { get; set; } 
        public bool IsBoostPlan { get; set; }
    }
 

}
