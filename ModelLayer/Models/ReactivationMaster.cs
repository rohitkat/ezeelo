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
    [Table("Leaders_Reactivation_Master")]
  public class ReactivationMaster
    {
        [Key]
        public long ID { get; set; }
        [DisplayName("Leadesr Status")]
        public string Leaders_Status { get; set; }
        public string Display_Status { get; set; }
        [DisplayName("No Of Days")]
        public int? No_Of_Days { get; set; }
        [DisplayName("Activation Fees")]
        public decimal? Activation_Fees { get; set; }
        [DisplayName("Penalty Percentage")]
        public int? Penalty_Percentage { get; set; }
        [DisplayName("Penalty Amount")]
        public decimal? Penalty_Amount { get; set; }
        [DisplayName("Penalty On")]
        public string Penalty_On { get; set; }
        [DisplayName("Current Status")]
        public bool? Current_Status { get; set; }
        public DateTime? Create_Date { get; set; }
        public DateTime? Modify_Date { get; set; }


         


    }
}
