using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class CustomerRatingAndFeedback
    {
        public int ID { get; set; }
        public long PersonalDetailID { get; set; }
        public long OwnerID { get; set; }
        public int RatingID { get; set; }
        public Nullable<int> Point { get; set; }
        public string Feedback { get; set; }
        public bool IsApproved { get; set; }
        public Nullable<long> ApprovedBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual Rating Rating { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
    }
}
