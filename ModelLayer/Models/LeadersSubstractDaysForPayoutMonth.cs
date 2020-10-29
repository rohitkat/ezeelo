using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{[Table("LeadersSubstractDaysForPayoutMonth")]
   public class LeadersSubstractDaysForPayoutMonth
    {
        [Key]
        public int ID { get; set; }
        public int SubstractNoDays { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int ModifyBy { get; set; }
        public bool IsActive { get; set; }

    }
}
