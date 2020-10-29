using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class Sent_Mail
    {
        public long Sent_Mail_ID { get; set; }
        public int Mail_Type_ID { get; set; }
        public Nullable<long> Login_ID { get; set; }
        public string Email_ID { get; set; }
        public string Mobile { get; set; }
        public Nullable<bool> IsOffer { get; set; }
        public Nullable<int> OfferID { get; set; }
        public Nullable<int> RemainingDays { get; set; }
        public Nullable<System.DateTime> Sent_Date { get; set; }
        public string Remarks { get; set; }
        public virtual Email_Type Email_Type { get; set; }
        public string UserName { get; set; }
    }
}
