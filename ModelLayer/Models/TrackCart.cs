using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class TrackCart
    {
        public long ID { get; set; }
        public Nullable<long> CartID { get; set; }
        public Nullable<long> UserLoginID { get; set; }
        public Nullable<long> ShopStockID { get; set; }
        public Nullable<int> Qty { get; set; }
        public string Mobile { get; set; }
        public string Stage { get; set; }
        public string Lattitude { get; set; }
        public string Longitude { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<long> CreateBy { get; set; }
        public string City { get; set; }
        public Nullable<int> FranchiseID { get; set; }//added
        public string IMEI_NO { get; set; }
        public virtual Cart Cart { get; set; }
    }
}
