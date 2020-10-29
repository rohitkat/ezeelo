using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class GcmUser
    {
        public long ID { get; set; }
        public string GcmRegID { get; set; }
        public string EmailID { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public Nullable<long> UserLoginID { get; set; }
        public string  MCOID { get; set; }
    }
}
