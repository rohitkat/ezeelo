using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Franchise.Models.ViewModel
{
    public class NewCustomerOrderDetailListViewModel
    {
        public long COID { get; set; }
        public bool IsBusinessBoosterOrder { get; set; }
        public List<CustomerOrderDetail> customerOrderDetails { get; set; }
    }
}