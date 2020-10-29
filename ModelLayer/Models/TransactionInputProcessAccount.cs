using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class TransactionInputProcessAccount
    {
        public long ID { get; set; }
        public long LeadgerHeadID { get; set; }
        public int ReceivedPaymentModeID { get; set; }
        public Nullable<long> TransactionInputID { get; set; }
        public long CustomerOrderID { get; set; }
        public decimal Amount { get; set; }
        public long ReceivedFromUserLoginID { get; set; }
        public bool PODReceived { get; set; }
        public string Narration { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual LedgerHead LedgerHead { get; set; }
        public virtual PaymentMode PaymentMode { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual TransactionInput TransactionInput { get; set; }
        public virtual UserLogin UserLogin { get; set; }
    }
}
