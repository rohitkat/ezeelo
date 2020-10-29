using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class ProductOfferViewModel
    {
        public long OfferID { get; set; }
        public string OfferName { get; set; }

        public long ComponentId { get; set; }
        public string ComponentName { get; set; }
        public string OfferDescription { get; set; }
        public bool IsFree { get; set; }
        public int MinPurchaseQty { get; set; }
        public int FreeOty { get; set; }
        public decimal DiscountInRs { get; set; }
        public decimal DiscountInPercent { get; set; }
        public System.DateTime StartDateTime { get; set; }
        public System.DateTime EndDateTime { get; set; }

    }
}
