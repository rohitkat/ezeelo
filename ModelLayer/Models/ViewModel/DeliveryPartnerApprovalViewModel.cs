using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class DeliveryPartnerApprovalViewModelList
    {
        public List<DeliveryPartnerApprovalViewModel> dpList { get; set; }
    }

    public class DeliveryPartnerApprovalViewModel
    {
        public Int64 UserLoginID { get; set; }
        public string BusinessTypePrefix { get; set; }
        public string Name { get; set; }
        public Int64 OwnerId { get; set; }
    }
}
