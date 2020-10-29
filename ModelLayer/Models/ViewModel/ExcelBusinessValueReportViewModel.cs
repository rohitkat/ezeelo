using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
  public  class ExcelBusinessValueReportViewModel
    {
      [DisplayName("Order ID")]
        public long CustomerOrderID { get; set; }

        public string EmailID { get; set; }
       
        public string OrderCode { get; set; }
        public decimal? OrderAmount { get; set; }
      
      
       
       
       
        public System.DateTime OrderDate { get; set; }
        
        public decimal BusinessPointsTotal { get; set; } 
     

       

        public decimal BusinessValue { get; set; } // added by amit on 30/7/2018 for BusinessValue Report


    }
}
