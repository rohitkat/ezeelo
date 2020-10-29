using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class ChargeStage
    {
        public ChargeStage()
        {
            this.Charges = new List<Charge>();
        }

        public int ID { get; set; }

        [Required(ErrorMessage = "Name is Required")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Name must be between 5 to 50 Chatacter")]
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual List<Charge> Charges { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
    }
}
