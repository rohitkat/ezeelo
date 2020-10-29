using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("KYCApprovalLog")]
    public class KYCApprovalLog
    {
        [Key]
        public long ID { get; set; }
        public long UserLoginId { get; set; }
        public long KYCID { get; set; }
        public bool IsApproved { get; set; }
        public string Remark { get; set; }
        public DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public string NetworkIP { get; set; }
    }
}
