using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class FranchiseDashboardCustomerRetention
    {
        public long NewCustomer { get; set; }
        public long DriftCustomer { get; set; }
        public long PromisiongCustomer { get; set; }
        public long LoyalCustomer { get; set; }
        public long RedAlertCustomer { get; set; }
        public long SleepersCustomer { get; set; }     
    }
  
}
