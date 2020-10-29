using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("MLMWalletDetails")]
    public class MLMWalletDetails
    {
        [Key]
        public long ID { get; set; }
        public long UserLoginId { get;set;}
        public long EzeeMoneyPayoutId { get; set; }
        public decimal ERP { get; set; }
        public decimal Amount { get; set; }
        public decimal? InactivePoint { get; set; }//Added by sonali for maintain log of Inactive payout
        public decimal? InactiveAmount { get; set; }//Added by sonali for maintain log of Inactive payout
    }
}
