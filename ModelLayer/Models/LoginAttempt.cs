using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class LoginAttempt
    {
        public long ID { get; set; }
        public long UserLoginID { get; set; }
        public int AttemptCount { get; set; }
        public System.DateTime ExpirationTime { get; set; }
        public bool IsLocked { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual UserLogin UserLogin { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
    }
}
