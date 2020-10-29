using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class VehicleType
    {
        public VehicleType()
        {
            this.DeliveryPartners = new List<DeliveryPartner>();
        }

        public int ID { get; set; }

        [Required(ErrorMessage = "Vehicle Type Name is Required")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Vehicle Type Name must be between 5 - 50 characters ")] 
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public virtual List<DeliveryPartner> DeliveryPartners { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
    }
}
