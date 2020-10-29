using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
  public class ExcelEzeeloCustomerOrder
    {
       
        public string OrderCode { get; set; }
        public decimal? OrderAmount { get; set; }

        public Decimal TotalBusinessPoints { get; set; } // added by amit on 03-12-18

        public string PaymentMode { get; set; }
       
        public decimal? PayableAmount { get; set; }
       
        public string PrimaryMobile { get; set; }
        public string SecondoryMobile { get; set; }
        public string ShippingAddress { get; set; }
       
        public string Pincode { get; set; }
        public string CityName { get; set; }
        public string AreaName { get; set; }
      
        public System.DateTime OrderDate { get; set; }
        public string EmailID { get; set; }
        public string FullName { get; set; }
       
      
       


       

        public DateTime? CreateDate { get; set; }  // added by amit on 11/7/2018 for MLM Customer Orders
        
    }
}
