using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class CouponDetailsViewModel
    {
        public int Result { get; set; }
        public string UserMessage { get; set; }
        public double VoucherAmount { get; set; }
        public double VoucherPercent { get; set; }
        public double MinimumPurchaseAmount { get; set; }
        public double MinimumPurchaseQuantity { get; set; }
        public bool IsFreeDelivery { get; set; }
        public bool IsCoupenUsed { get; set; }
        public decimal PayableAmount { get; set; }//Added by Sonali on 19-02-2019
        public decimal WalletAmount { get; set; }//Added by Sonali on 19-02-2019
    }
}
