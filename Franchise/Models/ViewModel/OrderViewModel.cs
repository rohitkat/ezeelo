using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Franchise.Models.ViewModel
{
    public class OrderViewModel
    {
        public long CustomerOrderID { get; set; }
        public string OrderCode { get; set; }
        public DateTime CreateDate { get; set; }
        public decimal OrderAmount { get; set; }
    }
}