using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class TrackSearch
    {
        public long ID { get; set; }
        public Nullable<long> UserLoginID { get; set; }
        public Nullable<long> CategoryID { get; set; }
        public Nullable<long> ShopID { get; set; }
        public string ProductName { get; set; }
        public string Lattitude { get; set; }
        public string Longitude { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public DateTime? CreateDate { get; set; }
        public Nullable<long> CreateBy { get; set; }
        public string City { get; set; }
        public Nullable<int> FranchiseID { get; set; }//added
        public string IMEI_NO { get; set; }
        public int TotalCount { get; set; }
    }
}
