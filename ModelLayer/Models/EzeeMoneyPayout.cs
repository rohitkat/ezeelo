using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("EzeeMoneyPayout")]
    public class EzeeMoneyPayout
    {
        [Key]
        public long Id { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal? CoinRate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalERP { get; set; }
        public decimal PayableAmount { get; set; }
        public decimal PayableERP { get; set; }
        public bool IsPaid { get; set; }
        public int DeliveredOrdCount { get; set; }
        public int ActiveUserCount { get; set; }
        public int TotalUserCount { get; set; }
        public DateTime FreezeDate { get; set; }
        public long FreezeBy { get; set; }
        public DateTime? PaidDate { get; set; }
        public long? PaidBy { get; set; }
        public string NetworkIP { get; set; }

        public decimal? TotalInActivePoints { get; set; }  // added by amit
        public bool? IsInactivePaid { get; set; } // Added by Sonali for Inactive Payout check
        public DateTime? InactivePaidDate { get; set; }// Added by Sonali for Inactive Payout Date
        public DateTime? InactiveFreezeDate { get; set; }// Added by Sonali for Freeze date of Inactive Payout

    }
}
