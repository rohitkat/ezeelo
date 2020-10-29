using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
 public   class LeadersOrderViewModel
    {
        public string FullName { get; set; }
        public string EmailID { get; set; }
        public string PrimaryMobile { get; set; }

        public long ID_Ref { get; set; }
        public long UserID { get; set; }
        public string Ref_Id { get; set; }
        public DateTime? Join_date_ref { get; set; }
        public bool? Status_ref { get; set; }
        public DateTime? Activate_date_ref { get; set; }
        public string Refered_Id_ref { get; set; }
        public bool? request { get; set; }
        public bool? request_active { get; set; }


        //CUSTOMER ORDER TABLE

       

        public string OrderCode { get; set; }
        public long UserLoginID { get; set; }
        public Nullable<long> ReferenceCustomerOrderID { get; set; }
        public decimal OrderAmount { get; set; }
        public Nullable<int> NoOfPointUsed { get; set; }
        public Nullable<decimal> ValuePerPoint { get; set; }
        public string CoupenCode { get; set; }
        public Nullable<decimal> CoupenAmount { get; set; }
        public string PAN { get; set; }
        public string PaymentMode { get; set; }
        public decimal PayableAmount { get; set; }
       
        public string SecondoryMobile { get; set; }
       
        public int PincodeID { get; set; }
        public Nullable<int> AreaID { get; set; }

       
        public decimal BusinessPointsTotal { get; set; }
        public Nullable<decimal> MLMAmountUsed { get; set; }
        //End
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }


        public string Pincode { get; set; }
        public string AreaName { get; set; }
        public string ShippingAddress { get; set; }
        public string ParentName { get; set; }  // added by amit 03-12-18

        //  public bool IsMLM { get; set; } // Adde by amit on 29-11-18
        //public DateTime? FromDate { get; set; }
        //public DateTime? ToDate { get; set; }

    }
}
