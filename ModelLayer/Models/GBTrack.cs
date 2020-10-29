using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class GBTrack
    {
        public long ID { get; set; }
        public string PageURL { get; set; }
        public Nullable<long> UserLoginId { get; set; }
        public Nullable<System.DateTime> InTime { get; set; }
        public Nullable<System.DateTime> OutTime { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
    }
}
