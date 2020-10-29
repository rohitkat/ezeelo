using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModelLayer.Models
{
    [Table("Leaders_Income_Master")]
   public class LeadersIncomeMaster
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
        public double? Level15 { get; set; }
        public double? Level16 { get; set; }
        public double? LevelR7 { get; set; }
        public double? LevelR8 { get; set; }
        public double? LevelR9 { get; set; }
        public double? LevelR10 { get; set; }
        public double? LevelR11 { get; set; }
        public double? LevelR12 { get; set; }
        public double? LevelR13 { get; set; }
        public double? LevelR14 { get; set; }
        public double? LevelR15 { get; set; }
        public double? LevelR16 { get; set; }




        public DateTime? Modify_Date { get; set; }


    }
}
