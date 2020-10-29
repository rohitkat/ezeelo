using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class PaymentProcessViewModel
    {
        public LogonDetailsViewModel LogonDetails { get; set; }
        public List<CustomerShippingAddressesViewModel> listShippingAddress { get; set; }
        public GuestCheckoutViewModel GuestCheckout { get; set; }
        public CheckOTP CheckOTP { get; set; }
        public AvailablePaymentModesViewModel PaymentModes { get; set; }

        public ShopProductVarientViewModelCollection shoppingCartDetail { get; set; }
    }
}
