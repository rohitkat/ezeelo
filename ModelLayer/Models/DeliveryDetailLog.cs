using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class DeliveryDetailLog
    {

        public long ID { get; set; }
        public long DeliveryOrderDetailID { get; set; }
        [Required(ErrorMessage = "Description is required")]
        [StringLength(150)]
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
        public virtual DeliveryOrderDetail DeliveryOrderDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
    }
}
