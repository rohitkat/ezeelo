using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Franchise.Models.ViewModel
{
    public class CustomerOrderAndItsOrderStatusViewModel
    {
        public OrderStatusViewModel orderStatusViewModel { get; set; }
        public List<CustomerOrderViewModel> customerOrderViewModels { get; set; }
    }
}