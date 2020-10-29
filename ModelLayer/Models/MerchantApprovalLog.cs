using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("MerchantApprovalLog")]
    public class MerchantApprovalLog
    {
        [Key]
        public long ID { get; set; }
        public long MerchantID { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
        public string NetworkIP { get; set; }
        public string Remark { get; set; }
    }
}
