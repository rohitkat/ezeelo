using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class ApplicationAlgorithm
    {
        public int ID { get; set; }
        public int ApplicationListID { get; set; }
        public int AlgorithmListID { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual AlgorithmList AlgorithmList { get; set; }
        public virtual ApplicationList ApplicationList { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
    }
}
