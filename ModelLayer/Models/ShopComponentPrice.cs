using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class ShopComponentPrice
    {
        public int ID { get; set; }
        [Required]
        public int ComponentID { get; set; }
        public long ShopID { get; set; }
        public Nullable<int> ComponentUnitID { get; set; }
        public decimal PerUnitRateInRs { get; set; }
        public decimal PerUnitRateInPer { get; set; }
        public Nullable<int> DependentOnComponentID { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual Component Component { get; set; }
        public virtual Component Component1 { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual Shop Shop { get; set; }
    }
}
