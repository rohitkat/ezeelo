using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("Log_Leaders_Income_Master")]
   public class LogLeadersIncomeMaster
    {
        [Key]
        public int ID { get; set; }
        public double? Level0 { get; set; }
        public double? Level1 { get; set; }
        public double? Level2 { get; set; }
        public double? Level3 { get; set; }
        public double? Level4 { get; set; }
        public double? Level5 { get; set; }
        public double? Level6 { get; set; }
        public double? Level7 { get; set; }
        public double? Level8 { get; set; }
        public double? Level9 { get; set; }
        public double? Level10 { get; set; }
        public double? Level11 { get; set; }
        public double? Level12 { get; set; }
        public double? Level13 { get; set; }
        public double? Level14 { get; set; }
        public DateTime? Last_Modify_Date { get; set; }
    }
}
