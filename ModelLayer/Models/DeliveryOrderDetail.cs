using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class DeliveryOrderDetail
    {
        public DeliveryOrderDetail()
        {
            this.DeliveryDetailLogs = new List<DeliveryDetailLog>();
            this.DeliveryOrderCashHandlingCharges = new List<DeliveryOrderCashHandlingCharge>();
        }

        public long ID { get; set; }
        public int DeliveryPartnerID { get; set; }
        public string ShopOrderCode { get; set; }
        public decimal Weight { get; set; }
        public decimal OrderAmount { get; set; }
        public decimal DeliveryCharge { get; set; }
        public decimal GandhibaghCharge { get; set; }
        public string DeliveryType { get; set; }
        public bool IsMyPincode { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual List<DeliveryDetailLog> DeliveryDetailLogs { get; set; }
        public virtual List<DeliveryOrderCashHandlingCharge> DeliveryOrderCashHandlingCharges { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual DeliveryPartner DeliveryPartner { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
    }
}
