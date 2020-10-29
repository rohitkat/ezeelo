using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Franchise.Models.ViewModel
{
    public class CustomerOrderAndItsCustomerOrderDetailViewModel
    {
        public NewCustomerOrderViewModel customerOrderViewModel { get; set; }
        public List<NewCustomerOrderViewModel> lThisCustomerPreviousOrderViewModel { get; set; }
        public List<NewCustomerOrderDetailViewModel> CustomerOrderDetailViewModels { get; set; }
    }
}