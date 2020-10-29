using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    // Created by Amit on 26/7/2018 for Leaders Order 
public  class ExcelLeadersOrderViewModel
    {
       
        //CUSTOMER ORDER TABLE

        public string FullName { get; set; }
        public string EmailID { get; set; }

        public System.DateTime CreateDate { get; set; }
       
        public string OrderCode { get; set; }
        public string Ref_Id { get; set; }
        public DateTime? Join_date_ref { get; set; }

        [DisplayName("Activate Date")]
        public DateTime? Activate_date_ref { get; set; }
    [DisplayName("Refer By")]
        public string Refered_Id_ref { get; set; }
       
      

        public decimal OrderAmount { get; set; }
        
       
        public string PaymentMode { get; set; }
        public decimal PayableAmount { get; set; }
        public string PrimaryMobile { get; set; }
        public string SecondoryMobile { get; set; }
       
       


        public decimal BusinessPointsTotal { get; set; }
        public Nullable<decimal> MLMAmountUsed { get; set; }
        //End
       
        public string Pincode { get; set; }
        public string AreaName { get; set; }
        public string ShippingAddress { get; set; }

       
    }
}
