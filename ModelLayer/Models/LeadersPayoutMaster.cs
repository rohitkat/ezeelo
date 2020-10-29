using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("Leaders_Payout_Master")]
   public class LeadersPayoutMaster
    {
       [Key]
        public long ID { get; set; }
        public decimal? Min_Resereved  { get; set; }
        public decimal? GST { get; set; }
        public decimal? TDS { get; set; }
        public decimal? Processing_Fees { get; set; }
        public decimal? Penalty { get; set; }
        public DateTime? Create_Date { get; set; }
        public DateTime? Modify_Date { get; set; }
        public string Network_IP { get; set; }
        public string Divice_ID { get; set; }




    }
}
