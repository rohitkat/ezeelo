using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class LoginSecurityAnswer
    {
        public long ID { get; set; }
        public Nullable<long> UserLoginID { get; set; }
        public Nullable<int> SecurityQuestionID { get; set; }
        public string Answer { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual SecurityQuestion SecurityQuestion { get; set; }
        public virtual UserLogin UserLogin { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
    }
}
