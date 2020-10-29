using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Franchise.Models.ViewModel
{
    public class CustomerOrderSystemLogViewModel
    {
        public long ID { get; set; }
        public long COID { get; set; }
        public string PersonName { get; set; }
        public DateTime CreateDate { get; set; }
        public decimal OrderAmount { get; set; }
        public decimal? CoupenAmount { get; set; }
        public decimal DeliveryCharge { get; set; }
        public decimal PayableAmount { get; set; }
        public string ShippingAddr { get; set; }
        public string Area { get; set; }
        public string Pincode { get; set; }
        public string City { get; set; }
        public string PrimaryMobile { get; set; }
        public string SecondaryMobile { get; set; }
        public List<CustomerOrderDetailSystemLogViewModel> CustomerOrderDetailSystemLogList { get; set; }
    }
}