using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class ReferDetailViewModel
    {
     public ReferDetailViewModel()
        {
            this.ReferAndEarnTransactions = new List<ReferAndEarnTransaction>();
        }

        public long ID { get; set; }
        public long ReferAndEarnSchemaID { get; set; }
        public long UserID { get; set; }
     
        public Nullable<long> ReferenceID { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual ReferAndEarnSchema ReferAndEarnSchema { get; set; }
        public virtual ICollection<ReferAndEarnTransaction> ReferAndEarnTransactions { get; set; }
        public List<SubReferDetail> lSubReferDetail { get; set; }


       
 
    }

    public class SubReferDetail
    {
        public int ID { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Message { get; set; }
    }

    public class ReferAndEarnSchemaName
    {
        public long ID { get; set; }
        public string Name { get; set; }
    }

    public class ReferCustomerViewModel
    {
        public List<SubReferDetail> lSubReferDetail { get; set; }
        public long ReferAndEarnSchemaID { get; set; }
        public long UID { get; set; }
    }
}
