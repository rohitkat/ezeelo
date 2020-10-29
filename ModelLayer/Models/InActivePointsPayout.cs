using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{[Table("InActivePointsPayout")]
   public class InActivePointsPayout
    {
        [Key]
        public long Id { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal? CoinRate { get; set; }
        public decimal TotalInActivePoints { get; set; }
        public decimal PayableInActivePoints { get; set; }
        public decimal PayableAmount { get; set; }
       
        public bool IsPaid { get; set; }
        public int DeliveredOrdCount { get; set; }
        public int ActiveUserCount { get; set; }
        public int TotalUserCount { get; set; }
        public DateTime FreezeDate { get; set; }
        public long FreezeBy { get; set; }
        public DateTime? PaidDate { get; set; }
        public long? PaidBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceId { get; set; }
        public string DeviceType { get; set; }


    }
}
