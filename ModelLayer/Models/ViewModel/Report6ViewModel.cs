using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{

    // Added  by amit 
 public class Report6ViewModel
    {

       // public long ID { get; set; }
        public string Customer { get; set; }
        public string OrderCode { get; set; }
        public string Mobile { get; set; }
       
        // cityName
        public DateTime? OrderDate { get; set; }  // order Date
        public DateTime? DeliveryDate { get; set; } // delevered date
        public decimal PayableAmount { get; set; }
        public string PaymentMode { get; set; }
        public string DeviceType { get; set; }
        public bool IsMLMUser { get; set; }
        public bool IsActive { get; set; }
        public string CheckMLMUser { get; set; }
        public string CheckActiveUser { get; set; }

        //public int OrderStatus { get; set; }
        public string ReferralID { get; set; }  //refferal id
        public string ReferBy { get; set; }  //refered by
        public string Parent { get; set; }  // email

        public string Level { get; set; }

        public string Status { get; set; }
        public decimal RPEarned_Order { get; set; }  // retail points
        public double RP_Distribution_Level0 { get; set; }
        public string Current_Level_User { get; set; }
        public double RP_Distribution_Level1 { get; set; }
        public string Level1_User { get; set; }
        public double RP_Distribution_Level2 { get; set; }
        public string Level2_User { get; set; }
        public double RP_Distribution_Level3 { get; set; }
        public string Level3_User { get; set; }
        public double RP_Distribution_Level4 { get; set; }
        public string Level4_User { get; set; }


       
        public decimal EzeeMoney { get; set; }
        public int PincodeID { get; set; }
        public string City { get; set; } 
        public string ShippingAddress { get; set; }


        public decimal? MlmAmountUsed { get; set; }  // added by amit 
        public string Pincode { get; set; }  //added by amit




       // public string FullName { get; set; }   // added for BusinessValueReport
       // public long OrderID { get; set; }    // added for BusinessValueReport

        //public Double Upline1 { get; set; }
        //public Double CurrentLevel { get; set; }
        //public Double Upline2 { get; set; }
        //public Double Upline3 { get; set; }
        //public Double Upline4 { get; set; }


       



        



        
    }
}
