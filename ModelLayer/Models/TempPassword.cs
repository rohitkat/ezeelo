using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    public partial class TempPassword
    {   
        public long ID { get; set; }      
        public long UserLoginID { get; set; }    
        public string TempPassword1 { get; set; }
        public Nullable<System.DateTime> LoginTime { get; set; }

        public long FranchiseID { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }      
    }
}
