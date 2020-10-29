using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
  public class APICustomerOrder
    {
      [Required(ErrorMessage="Please provide customer login ID")]
        public long UserLoginID { get; set; }
        public Nullable<long> ReferenceCustomerOrderID { get; set; }
       [Required(ErrorMessage = "Please provide order amount")]
        public decimal OrderAmount { get; set; }
        public Nullable<int> NoOfPointUsed { get; set; }
        public Nullable<decimal> ValuePerPoint { get; set; }
        public string CoupenCode { get; set; }
        public Nullable<decimal> CoupenAmount { get; set; }
        public string PAN { get; set; }
       [Required(ErrorMessage = "Please provide Payment Mode e.g COD etc")]
        public string PaymentMode { get; set; }
       [Required(ErrorMessage = "Please provide Payable Amount")]
        public decimal PayableAmount { get; set; }
       [Required(ErrorMessage = "Please provide primary Mobile No")]
        public string PrimaryMobile { get; set; }
        public string SecondoryMobile { get; set; }
       [Required(ErrorMessage = "Please provide shipping address")]
        public string ShippingAddress { get; set; }
      [Required(ErrorMessage = "Please provide shipping pincode")]
        public int PincodeID { get; set; }
        public Nullable<int> AreaID { get; set; }
        //  public decimal BusinessPointsTotal { get; set; } // Added by Sonali for MLM on 18/09/2018
        public decimal WalletAmountUsed { get; set; } // Added by Sonali for MLM on 18/09/2018

        /*New Correction As Required 
         * Changes By :- Pradnyakar Badge
         * Dated      :- 14-12-2015
         * Suggested By Tesjwee and Sumeet
         */
     //   public DateTime ScheduleDate { get; set; }
       // public int ScheduleID { get; set; }

    }
}
