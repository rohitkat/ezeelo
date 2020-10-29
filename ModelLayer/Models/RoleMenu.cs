using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class RoleMenu
    {
        public int ID { get; set; }
        public int RoleID { get; set; }
        public Nullable<int> MenuID { get; set; }
        public Nullable<bool> CanRead { get; set; }
        public Nullable<bool> CanWrite { get; set; }
        public Nullable<bool> CanDelete { get; set; }
        public Nullable<bool> CanPrint { get; set; }
        public Nullable<bool> CanExport { get; set; }
        public Nullable<bool> CanImport { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual Menu Menu { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual Role Role { get; set; }
    }
}
