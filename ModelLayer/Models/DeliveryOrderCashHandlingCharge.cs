using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class DeliveryOrderCashHandlingCharge
    {
        public long ID { get; set; }
        public long DeliveryOrderDetailID { get; set; }
        public int HoursSpend { get; set; }
        public decimal TotalAmountCharged { get; set; }
        public string ChargeType { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual DeliveryOrderDetail DeliveryOrderDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
    }
}
