using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class Chat
    {
        public Chat()
        {
            this.UserChats = new List<UserChat>();
        }

        public int ID { get; set; }
        public Nullable<long> CRMPersonalDetailID { get; set; }
        public string Name { get; set; }
        [Required(ErrorMessage = "Email is required (we promise not to spam you!)")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [RegularExpression(@"^(?:\d{10}|00\d{10}|\+\d{2}\d{8})$", ErrorMessage = "Please enter proper primary mobile No.")]
        public string Mobile { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual PersonalDetail PersonalDetail2 { get; set; }
        public virtual List<UserChat> UserChats { get; set; }
    }
}
