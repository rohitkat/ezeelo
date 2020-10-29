using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class CorporateShippingFacility
    {
        public CorporateShippingFacility()
        {
            this.CorporateOrderShippingFacilityDetails = new List<CorporateOrderShippingFacilityDetail>();
        }

        public long ID { get; set; }
        public string Name { get; set; }
        public decimal AmountInRs { get; set; }
        public decimal AmountInPer { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual ICollection<CorporateOrderShippingFacilityDetail> CorporateOrderShippingFacilityDetails { get; set; }
    }
}
