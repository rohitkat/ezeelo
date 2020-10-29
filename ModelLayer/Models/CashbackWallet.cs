using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("CashbackWallet")]
    public class CashbackWallet
    {
        [Key]
        public long ID { get; set; }
        public long UserLoginID { get; set; }
        public decimal Points { get; set; }
        public decimal Amount { get; set; }
        public long ModifyBy { get; set; }
        public DateTime ModifyDate { get; set; }
    }
}
