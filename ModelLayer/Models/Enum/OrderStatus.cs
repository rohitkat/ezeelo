using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ModelLayer.Models.Enum
{

    public enum ORDER_STATUS
    {
        PENDING = 0,
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

}