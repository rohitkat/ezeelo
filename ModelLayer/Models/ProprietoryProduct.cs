using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class ProprietoryProduct
    {
        public long ID { get; set; }
        public long ShopID { get; set; }
        public long ProductID { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual Product Product { get; set; }
        public virtual Shop Shop { get; set; }
    }
}
