using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class OTPLog
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string EmailOTP { get; set; }
        public string MobileOTP { get; set; }  

        public DateTime OTPExpire{ get; set; }
        public bool IsValidated { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }

    }
}
