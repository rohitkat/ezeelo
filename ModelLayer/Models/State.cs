using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class State
    {
        public State()
        {
            this.Districts = new List<District>();
        }

        public long ID { get; set; }

        [Required(ErrorMessage = "Please Enter the State Name")]
        [StringLength(150, MinimumLength = 4, ErrorMessage = "State Name must be between 4 - 150 characters ")]
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual List<District> Districts { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        //------Added by Shaili on 16-07-19----------------//
        public virtual ICollection<Merchant> Merchant { get; set; }
        //------------End--------------------------------//
    }
}
