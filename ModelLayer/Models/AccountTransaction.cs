using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class AccountTransaction
    {
        public long ID { get; set; }
        public long UserLoginID { get; set; }
        public long GandhibaghTransactionID { get; set; }
        public long LedgerHeadID { get; set; }
        public string Narration { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string TotalAmountType { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual GandhibaghTransaction GandhibaghTransaction { get; set; }
        public virtual LedgerHead LedgerHead { get; set; }
        public virtual UserLogin UserLogin { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
    }
}
