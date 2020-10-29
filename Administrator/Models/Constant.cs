using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Administrator.Models
{
    public static class Constant
    {
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
            PLACED = 1,
            CONFIRM = 2,
            PACKED = 3,
            DISPATCHED_FROM_SHOP = 4,
            IN_GODOWN = 5,
            DISPATCHED_FROM_GODOWN = 6,
            DELIVERED = 7,
            RETURNED = 8,
            CANCELLED = 9
        }

        public enum DELIVERY_TYPE
        {
            NORMAL = 1,
            EXPRESS = 2
        }

        public enum BUSINESS_TYPE
        {
            MERCHNAT_SHOP = 1,
            FRANCHISE = 2,
            DELIVERY_PARTNER = 3,
            SUPER_ADMIN = 4,
            PRODUCT = 1003,
            ADMINISTRATOR = 1004
        }

        public enum DAYS_TYPE
        {
            SUNDAY = 1,
            MONDAY = 2,
            TUESDAY = 3,
            WEDNESDAY = 4,
            THURSDAY = 5,
            FRIDAY = 6,
            SATURDAY = 7
        }

        public enum BEHAVIOR_TYPE
        {
            CONSTANT = 1,
            DECREMENT = 2,
            INCREMENT = 3
        }
    }
}