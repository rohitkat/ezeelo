using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace ModelLayer.Models
{
    public partial class Bank
    {
        public Bank()
        {
            this.OwnerBanks = new List<OwnerBank>();
        }

        public int ID { get; set; }
        
        [Required(ErrorMessage="Bank Name is Required")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Bank Name must be between 3 - 150 characters ")]        
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
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual List<OwnerBank> OwnerBanks { get; set; }
    }
}
