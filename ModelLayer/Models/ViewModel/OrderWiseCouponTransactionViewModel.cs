using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class OrderWiseCouponTransactionViewModel
    {    
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set;}
        public string CouponCode { get; set; }
        [Key]
        public int? CouponSchemeID { get; set; }       
        public string CouponScheme { get; set; }
        public decimal CouponAmount { get; set;}
        public string OrderCode { get; set; }
        public string CustomerName { get; set;}
        public string PrimaryMobile { get; set; }
        public DateTime OrderDate { get; set;}
        public decimal OrderAmount { get; set; }
        public decimal PayableAmount { get; set; }       

    }
    public class SelectCouponListFromScheme 
    {
        public int ID { get; set; }
        public string CoupenCode { get; set; }
    }
}
