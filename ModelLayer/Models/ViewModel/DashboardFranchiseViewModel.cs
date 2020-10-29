using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class DashboardFranchiseViewModel
    {
        public string TOTALFRANCHISE_NAME { get; set; }
        public long TOTALPALCED { get; set; }
        public long TOTALDELIVERED { get; set; }

        public List<DashboardLogisticViewModel> dashboardLogisticViewModel { get; set; }

        public List<DashboardFranchisePincodeViewModel> dashboardFranchisePincodeViewModel { get; set; }
           
    }
    public class FranchiseDshboardCustomerRet
    {
        public List<DashboardFranchiseViewModel> listDashboardFranchiseViewModel { get; set; }

        public FranchiseDashboardCustomerRetention lFranchiseDashboardCustomerRetention { get; set; }
    }
}
