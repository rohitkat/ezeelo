using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class PremiumShopPrimaryMarket_ViewModel
    {
        public long ID { get; set; }
        public int FranchiseID { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int PriorityLevel { get; set; }
        public int Level { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> CreateBy { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual Category Category { get; set; }
        public virtual Franchise Franchise { get; set; }
        public virtual Shop Shop { get; set; }
    }
}
