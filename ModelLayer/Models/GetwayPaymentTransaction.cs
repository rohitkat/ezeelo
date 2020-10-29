using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class GetwayPaymentTransaction
    {
        public int ID { get; set; }
        public string PaymentMode { get; set; }
        public Nullable<long> FromUID { get; set; }
        public Nullable<long> ToUID { get; set; }
        public Nullable<long> AccountTransactionId { get; set; }
        public string PaymentGetWayTransactionId { get; set; }
        public Nullable<int> Status { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> TransactionDate { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public Nullable<long> CustomerOrderID { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
    }
}
