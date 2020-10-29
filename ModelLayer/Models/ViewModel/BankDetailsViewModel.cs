using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class BankDetailsViewModel
    {
        public long ID { get; set; }

        public int BusinessTypeID { get; set; }

        public long OwnerID { get; set; }

        public int BankID { get; set; }

        public string BranchName { get; set; }

        public string IFSCcode { get; set; }

        public string MICRCode { get; set; }

        public string AccountNumber { get; set; }

        public int BankAccountTypeID { get; set; }

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
