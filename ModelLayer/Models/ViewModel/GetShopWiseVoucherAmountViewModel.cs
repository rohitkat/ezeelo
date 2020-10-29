using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
  public class GetShopWiseVoucherAmountViewModel
    {
      public CouponDetailsViewModel CoupenDetails { get; set; }
      public long ShopIDForFreeDelivery { get; set; }
    }
}
