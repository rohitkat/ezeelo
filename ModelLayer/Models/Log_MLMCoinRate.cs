using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("Log_MLMCoinRate")]
   public class Log_MLMCoinRate
    {
        [Key]
        public long ID { get; set; }
       
        public DateTime? StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        public Decimal? Rate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        public long? ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }

        public DateTime? Last_Create_Date { get; set; }

    }
}
