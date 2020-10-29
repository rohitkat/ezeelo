using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("CashbackPointsPayout")]
    public class CashbackPointsPayout
    {
        [Key]
        public long ID { get; set; }
        public long EzeeMoneyPayoutID { get; set; }
        public decimal TotalCashbackPoints { get; set; }
        public decimal PayableEzeeMoney { get; set; }
        public DateTime CreateDate { get; set; }
    }

    [Table("CashbackPointsPayoutDetail")]
    public class CashbackPointsPayoutDetail
    {
        [Key]
        public long ID { get; set; }
        public long CashbackPointsPayoutD { get; set; }
        public long UserLoginId { get; set; }
        public decimal CashbackPoints { get; set; }
        public decimal EzeeMoney { get; set; }
    }
}
