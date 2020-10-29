using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ModelLayer.Models.Enum
{
    public class OfferType
    {
        public enum OFFER_TYPE
        {
           FLAT_DISCOUNT_OFFER = 1,
            FREE_OFFER=2,
            COMBO_OFFER=3,
            COMPONENT_OFFER = 4,
            WALLET_BALANCE_OFFER = 5,
            COUPON_OFFER=6
        }
    }
}