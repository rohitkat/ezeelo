using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class GandhibaghTransaction
    {
        public GandhibaghTransaction()
        {
            this.AccountTransactions = new List<AccountTransaction>();
        }

        public long ID { get; set; }
        public int ChargeID { get; set; }
        public string ChargeType { get; set; }
        public Nullable<int> FromBusinessTypeID { get; set; }
        public Nullable<long> FromPersonalDetailId { get; set; }
        public Nullable<int> ToBusinessTypeID { get; set; }
        public Nullable<long> ToPersonalDetailID { get; set; }
        public decimal ApplicablePercent { get; set; }
        public decimal ApplicableRupee { get; set; }
        public string Particular { get; set; }
        public Nullable<long> CustomerOrderDetailID { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> TransactionAmount { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public string Remark { get; set; }
        public virtual List<AccountTransaction> AccountTransactions { get; set; }
        public virtual BusinessType BusinessType { get; set; }
        public virtual BusinessType BusinessType1 { get; set; }
        public virtual BusinessType BusinessType2 { get; set; }
        public virtual Charge Charge { get; set; }
        public virtual CustomerOrderDetail CustomerOrderDetail { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual PersonalDetail PersonalDetail2 { get; set; }
        public virtual PersonalDetail PersonalDetail3 { get; set; }
    }
}
