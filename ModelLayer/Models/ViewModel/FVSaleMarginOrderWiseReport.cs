using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models.ViewModel
{
    public class FVSaleMarginOrderWiseReport
    {
        public DateTime? OrderDate { get; set; }
        public string SKUName { get; set; }
        public long SKUID { get; set; }
        public string SKUUnit { get; set; }
        public string OrderCode { get; set; }
        public DateTime? OrderDeliveryDate { get; set; }
        public  string CustomerName { get; set; }
        public string PaymentMode { get; set; }
        public decimal? ProductTotalAmount { get; set; }
        public decimal? DeliveryCharge { get; set; }
        public decimal? WalletAmountUsed { get; set; }
        public decimal? TotalMargin { get; set; }
        public decimal? RetailPointsonOrder { get; set; }

    }
}
