using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("Log_Leaders_Payout_Master")]
   public class LogLeadersPayout
    {
        [Key]
        public long ID { get; set; }
        [DisplayName("Min Reserved")]
        public decimal? Min_Resereved { get; set; }
        [DisplayName("GST Amount")]
        public decimal? GST { get; set; }
    [DisplayName("TDS Amount")]
        public decimal? TDS { get; set; }
        [DisplayName("Processing Fees")]
        public decimal? Processing_Fees { get; set; }
        public decimal? Penalty { get; set; }
        public DateTime? Last_Create_Date { get; set; }
      
    }
}
