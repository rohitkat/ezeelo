using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("Log_QRP_Master")]
    public class LogQRPMaster
    {
        [Key]
        public long ID { get; set; }
        public long? Franchise_ID { get; set; }
        public string City { get; set; }
        public decimal? Current_QRP { get; set; }
        public decimal? Min_QRP { get; set; }
        public decimal? Max_QRP { get; set; }
        public DateTime? Last_Create_Date { get; set; }
        public int Hour { get; set; }
    }
}
