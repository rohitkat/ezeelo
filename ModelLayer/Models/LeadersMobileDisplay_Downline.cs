using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{[Table("LeadersMobileDisplay_Downline")]
  public  class LeadersMobileDisplay_Downline
    {
        [Key]
        public int ID { get; set; }
        public bool IsMobileDisplay { get; set; }
        public bool IsEmailDisplay { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int ModifyBy { get; set; }



    }
}
