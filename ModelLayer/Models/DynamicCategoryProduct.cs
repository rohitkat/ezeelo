using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class DynamicCategoryProduct
    {
        public long ID { get; set; }
        public int FranchiseID { get; set; }
        public long ProductID { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime EndDate { get; set; }
        public Nullable<int> SequenceOrder { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public string NetworkIP { get; set; }
        public string Remarks { get; set; }
        public virtual Franchise Franchise { get; set; }
        public virtual Product Product { get; set; }
    }
}
