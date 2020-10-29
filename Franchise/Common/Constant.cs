using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Franchise.Common
{
    public class Constant
    {
        public const long CRM_OWNER_ID = 1;
        public const string CRM_EMP_CODE = "GBCC";
        public const string FRANCHISE_CODE = "GBFR";
        public enum DELIVERY_PARTNER_SERVICE_LEVEL
        {
            INTERNATIONAL = 1,
            INDIA = 2,
            STATE = 3,
            DISTRICT = 4,
            CITY = 5,
            PINCODE = 6
        }

        public enum ORDER_STATUS
        {
            ONLINE_PAYMENT_PENDING = 0,
            PLACED = 1,
            CONFIRM = 2,
            PACKED = 3,
            DISPATCHED_FROM_SHOP = 4,
            IN_GODOWN = 5,
            DISPATCHED_FROM_GODOWN = 6,
            DELIVERED = 7,
            RETURNED = 8,
            CANCELLED = 9,
            ABANDONED = 10 //Added by Zubair on 01-12-2017
        }

        public enum DELIVERY_TYPE
        {
            NORMAL = 1,
            EXPRESS = 2
        }

        public enum BusinessType
        {
            MERCHANT_SHOP = 1,
            FRANCHISE = 2,
            DELIVERY_PARTNER = 3,
            SUPER_ADMIN = 5
        }
        public enum REVIEW_TYPE
        {
            PENDING = 1,
            APPROVED = 2,
            REJECTED = 3
        }

        public const string RATING_NAME_FOR_PRODUCT = "Product Quality";

        public enum RECEIVE_ORDER_ON_CALL_STATUS
        {
            YES = 1,
            NO = 2
        }

    }
}