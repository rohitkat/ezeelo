using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class Material
    {
        public Material()
        {
            this.CategoryMaterials = new List<CategoryMaterial>();
            this.ProductVarients = new List<ProductVarient>();
            this.TempProductVarients = new List<TempProductVarient>();
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual List<CategoryMaterial> CategoryMaterials { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual List<ProductVarient> ProductVarients { get; set; }
        public virtual List<TempProductVarient> TempProductVarients { get; set; }
    }
}
