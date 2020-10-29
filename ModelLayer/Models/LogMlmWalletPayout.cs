using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{[Table("LogMlmWalletPayout")]
  public class LogMlmWalletPayout
    {
        [Key]
        public long ID { get; set; }
        public long UserLoginID { get; set; }
        public Decimal? Current_WalletAmount { get; set; }
        public Decimal? Request_Amount { get; set; }
        public Decimal? TransactionPoints { get; set; }
        public bool AddOrSub { get; set; }
        public DateTime Create_Date { get; set; }
        public string Create_By { get; set; }
        public long? LeadersPayoutRequestID { get; set; }


    }
}
