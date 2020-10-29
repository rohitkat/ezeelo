using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class ReferDetail
    {
        public ReferDetail()
        {
            this.ReferAndEarnTransactions = new List<ReferAndEarnTransaction>();
        }

        public long ID { get; set; }
        public long ReferAndEarnSchemaID { get; set; }
        public long UserID { get; set; }
        [RegularExpression(@"^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$", ErrorMessage = "Please enter proper Email")]
        public string Email { get; set; }
        [RegularExpression(@"^(?:\d{10}|00\d{10}|\+\d{2}\d{8})$", ErrorMessage = "Please enter proper Mobile No.")]
        public string Mobile { get; set; }
        public Nullable<long> ReferenceID { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual ReferAndEarnSchema ReferAndEarnSchema { get; set; }
        public virtual ICollection<ReferAndEarnTransaction> ReferAndEarnTransactions { get; set; }
    }
}
