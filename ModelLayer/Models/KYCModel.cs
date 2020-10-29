using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{[Table("KYC")]
  public class KYCModel
    {
        public long ID { get; set; }
        public long UserLoginID { get; set; }
        public string AdhaarNo { get; set; }
        public string PanNo { get; set; }
        public int BankID { get; set; }
        public string AccountNo { get; set; }
        public string BankIFSC { get; set; }
        public string BranchName { get; set; }
        public string AccountType { get; set; }
        public string AdhaarImageUrl { get; set; }
        public string PanImageUrl { get; set; }
        public string PassbookImageUrl { get; set; }
        public string Reference { get; set; }
        public bool IsVerified { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreateBy { get; set; }
        public long? ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }

        public string BenificiaryName { get; set; } //on 15-1
        public string BenificiaryEmail { get; set; }

        public string KYCFormURL { get; set; }  // on 15-2


    }
}
