using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    public partial class ShopMenuPriority
    {
        public long ID { get; set; }
        public long ShopID { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int Level { get; set; }
        public int SequenceOrder { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string ImageName { get; set; }
        public string Remarks { get; set; }
        public virtual Category Category { get; set; }
        public virtual Shop Shop { get; set; }
    }
}
