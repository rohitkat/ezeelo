using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class Specification
    {
        public Specification()
        {
            this.CategorySpecifications = new List<CategorySpecification>();
            this.ProductSpecifications = new List<ProductSpecification>();
            this.Specification1 = new List<Specification>();
            this.TempProductSpecifications = new List<TempProductSpecification>();
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public Nullable<int> ParentSpecificationID { get; set; }
        public int Level { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual List<CategorySpecification> CategorySpecifications { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual List<ProductSpecification> ProductSpecifications { get; set; }
        public virtual List<Specification> Specification1 { get; set; }
        public virtual Specification Specification2 { get; set; }
        public virtual List<TempProductSpecification> TempProductSpecifications { get; set; }
    }
}
