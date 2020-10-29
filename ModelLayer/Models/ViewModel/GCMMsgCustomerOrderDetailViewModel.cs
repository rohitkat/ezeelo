using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class GCMMsgCustomerOrderDetailViewModel
    {
        public List<CustomerOrderViewModel> lOrderList { get; set; }
        //public List<CustomerOrderDetailViewModel> lOrderProductsList { get; set; }
    }
    public class GCMMsgCustOrderDetail
    {
        public List<CustomerOrderViewModel> lOrderList { get; set; }
        public List<CustomerOrderDetailViewModel> lOrderProductsList { get; set; }
    }
}
