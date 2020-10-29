using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class MerchantWiseOrderStatusCountViewModel 
    {
        public int PLACED { get; set; }
        public int CONFIRM { get; set; }
        public int PACKED { get; set; }
        public int DISPATCHED_FROM_SHOP { get; set; }
        public int IN_GODOWN { get; set; }
        public int DISPATCHED_FROM_GODOWN { get; set; }
        public int DELIVERED { get; set; }
        public int RETURNED { get; set; }
        public int CANCELLED { get; set; }

    }
}
