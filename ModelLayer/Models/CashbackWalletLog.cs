using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("CashbackWalletLog")]
    public class CashbackWalletLog
    {
        [Key]
        public long ID { get; set; }
        public long UserLoginID { get; set; }
        public long CustomerOrderID { get; set; }
        public long CashbackPayoutID { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }
        public string Remark { get; set; }
        public DateTime ModifyDate { get; set; }
        public long MerchantPayoutID { get; set; }
    }
}
