using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace ModelLayer.Models
{
    public partial class PaymentMode
    {
        public PaymentMode()
        {
            this.TransactionInputProcessAccounts = new List<TransactionInputProcessAccount>();
        }
        public int ID { get; set; }
        [Required(ErrorMessage = "Payment Mode is Required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Payment Mode must be between 3 to 50 Chatacter")]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
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
        public virtual ICollection<TransactionInputProcessAccount> TransactionInputProcessAccounts { get; set; }

    }
}
