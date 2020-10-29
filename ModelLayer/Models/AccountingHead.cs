using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class AccountingHead
    {
        public AccountingHead()
        {
            this.LedgerHeads = new List<LedgerHead>();
        }

        public long ID { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual List<LedgerHead> LedgerHeads { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
    }
}
