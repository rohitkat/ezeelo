using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class OTP
    {
        public long ID { get; set; }
        public string SessionCode { get; set; }
        public string OTP1 { get; set; }
        public System.DateTime ExpirationTime { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }

        //-- Add By Ashish Nagrale --/
        // Hide from Ashish for Live
       /* public string OrderCode { get; set; }
        public string ShopOrderCode { get; set; }
        public decimal PayableAmount { get; set; }*/
        //-- End Add --/
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
    }
}
