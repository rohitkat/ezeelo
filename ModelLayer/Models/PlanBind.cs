using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class PlanBind
    {
        public PlanBind()
        {
            this.PlanBindCategories = new List<PlanBindCategory>();
        }

        public int ID { get; set; }
        public int PlanID { get; set; }
        public int Type { get; set; }
        public int Level { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual Plan Plan { get; set; }
        public virtual List<PlanBindCategory> PlanBindCategories { get; set; }
    }
}
