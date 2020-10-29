using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace ModelLayer.Models
{
    public partial class Salutation
    {
        public Salutation()
        {
            this.PersonalDetails = new List<PersonalDetail>();
        }

        public int ID { get; set; }
        [Required(ErrorMessage="Please Enter the Salutation Name")]
        [StringLength(5, MinimumLength = 2, ErrorMessage = "Salutation Name must be between 2 - 5 characters ")]
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
        public virtual List<PersonalDetail> PersonalDetails { get; set; }
    }
}
