using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{

    // Created by Amit on date 26/07/2018 for Excel of No. of Leaders List
 public class ExcelLeadersUserViewModel
    {


        public string FullName { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }
        public DateTime CreateDate { get; set; }


        [DisplayName("Ref ID")]
        public string Ref_Id { get; set; }
        [DisplayName("Join Date Ref")]
        public DateTime Join_date_ref { get; set; }
       

        
        [DisplayName("Last Activate")]
        public DateTime Activate_date_ref { get; set; }
        [DisplayName("Refered Id Ref")]
        public string Refered_Id_ref { get; set; }

        public decimal RP { get; set; }
        public decimal LeftQRP { get; set; }
        public decimal ERP { get; set; }
    }
}
