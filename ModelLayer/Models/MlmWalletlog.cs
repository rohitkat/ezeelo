using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("MlmWalletlog")]
    public class MlmWalletlog
    {
        [Key]
        public long ID { get; set; }
        public long CustomerOrderId { get; set; }
        public Decimal Amount { get; set; }
        public bool IsDebit { get; set; }
        public bool IsCredit { get; set; }
        public long UserLoginID { get; set; }
        public long PromotionalPayoutId { get; set; }
        public long PayoutId { get; set; }
        public Decimal CurrentAmt { get; set; }
        public long Createdby { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string NetworkID { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public long EwalletRefund_TableID { get; set; }
    }
}
