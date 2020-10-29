using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class DashboardFranchisePincodeViewModel
    {
        public string PINCODE { get; set; }
        public long ORDERCOUNTPLACED { get; set; }

        public long ORDERCOUNTDELIVERED { get; set; }
    }
}
