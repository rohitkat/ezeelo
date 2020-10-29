using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("Log_Leaders_Reactivation_Master")]
  public  class LogLeadersReactivationMaster
    {
        [Key]
        public int ID { get; set; }
       
        public string Leaders_Status { get; set; }
       
        public int? No_Of_Days { get; set; }
       
        public decimal? Activation_Fees { get; set; }
        
        public int? Penalty_Percentage { get; set; }
       
        public decimal? Penalty_Amount { get; set; }
        
        public string Penalty_On { get; set; }
        public DateTime? Last_Create_Date { get; set; }
        public bool? Current_Status { get; set; }



    }
}
