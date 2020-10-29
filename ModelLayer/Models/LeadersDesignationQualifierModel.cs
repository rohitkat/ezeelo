using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("Leaders_Designation_Qualifier")]
   public class LeadersDesignationQualifierModel
    {
        public int ID { get; set; }
        public string Short_Name { get; set; }
        public string Designation { get; set; }
        public int? At_Level { get; set; }
        public int? Downline_Current_Level { get; set; }
        public int? Min_User { get; set; }
        public int? Min_Active_User { get; set; }
        public int? Total_User { get; set; }
        public string Min_Designation { get; set; }
        public string Display_Name { get; set; }
        public bool? Status { get; set; }
    }
}
