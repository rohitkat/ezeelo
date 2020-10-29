using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Franchise.Models.ViewModel
{
    public class NewOrderStatusViewModel
    {
        public int PLACED { get; set; }
        public int CONFIRM { get; set; }
        public int PACKED { get; set; }
        public int DISPATCH_FROM_SHOP { get; set; }
        public int IN_GODOWN { get; set; }
        public int DISPATCH_FROM_GODOWN { get; set; }
        public int DELIVERED { get; set; }
        public int RETURNED { get; set; }
        public int CANCELLED { get; set; }
        public int ABANDONED { get; set; } //Added by Zubair on 01-12-2017
        public int TotalOrder { get; set; }
    }
}